using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Szotar {
	public class SimpleDictionary : IBilingualDictionary {
		Section forwards, backwards;
		Utf8LineReader reader;
		static string magicNumber = "sdict-magic";

		public class Section : IDictionarySection {
			IList<Entry> entries;
			SimpleDictionary dictionary;

			public Section(IList<Entry> entries, SimpleDictionary dictionary) {
				this.entries = entries;
				this.dictionary = dictionary;
			}
			
			public SimpleDictionary Dictionary { 
				get { return dictionary; }
			}

			[DisplayName("HeadWords"), Description("HeadWords")]
			public int HeadWords {
				get { return entries.Count; }
			}

			[DisplayName("Name"), Description("Name")]
			public string Name {
				get; set;
			}

			public IEnumerator<Entry> GetEnumerator() {
				return entries.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return entries.GetEnumerator();
			}

			public IEnumerable<SearchResult> Search(string search, bool ignoreAccents, bool ignoreCase) {
				return Searcher.Search(this, search, ignoreAccents, ignoreCase);
			}

			public Entry GetFullEntry(Entry stub) {
				//We've already got the full entry, so no work to do.
				if (stub.Translations != null)
					return stub;
				if (stub.Tag == null)
					throw new ArgumentException("The in-memory dictionary entry has no entry tag, and cannot be fully loaded.");

				Entry full = dictionary.GetFullEntry(stub, this);
				ReplaceEntry(stub, full);
				return full;
			}

			protected void ReplaceEntry(Entry entryStub, Entry fullEntry) {
				int index = entries.IndexOf(entryStub);
				if (index < 0)
					throw new ArgumentException();
				entries[index] = fullEntry;
			}
		}
		
		public class Info : DictionaryInfo {
			public Info(string path) {
				Path = path;
				SimpleDictionary dict = new SimpleDictionary(path, false, false);
				
				Name = dict.Name;
				Author = dict.Author;
				Url = dict.Url;
				
				SectionNames = dict.SectionNames;
				if(dict.FirstLanguage != null || dict.SecondLanguage != null)
					Languages = new string[] { dict.FirstLanguage, dict.SecondLanguage };
				SectionSizes = dict.SectionSizes;
			}
			
			//TODO: Could (should?) be changed to store a WeakReference.
			//Note that StartPage will try not to open the same dictionary twice anyway.
			public override IBilingualDictionary GetFullInstance() {
				return new SimpleDictionary(Path);
			}
		}
		
		public SimpleDictionary(string path) : this(path, true, true) {
		}

		public SimpleDictionary(string path, bool poolStrings, bool full) {
			Path = path;

			var forwardsEntries = new List<Entry>();
			var backwardsEntries = new List<Entry>();

			bool sorted = false;

			this.forwards = new Section(forwardsEntries, this);
			this.backwards = new Section(backwardsEntries, this);

			StringPool pool = new StringPool();

			OpenFile();
			bool firstLine = true;

			try {
				while (!reader.EndOfStream) {
					long bytePos;
					string line = reader.ReadLine(out bytePos);

					if (firstLine) {
						if (line != magicNumber)
							throw new DictionaryLoadException("The file you are loading is probably not a dictionary (wrong magic number!)");
						firstLine = false;
					}
					
					if(line.StartsWith("f ") || line.StartsWith("b ") || line.StartsWith("t ")) {
						if(!full)
							return; //We got to the actual entries in the dictionary, but we just want the headers.

						string rest = Uri.UnescapeDataString(line.Substring(2));
						switch (line[0]) {
							case 'f': {
									Entry e = new Entry(rest, null);
									e.Tag = new EntryTag(forwards, bytePos);
									forwardsEntries.Add(e);
									break;
								}
							case 'b': {
									Entry e = new Entry(rest, null);
									e.Tag = new EntryTag(backwards, bytePos);
									backwardsEntries.Add(e);
									break;
								}
							case 't':
								break;
						}
						
						continue;
					}

					//We're only looking for file-level properties, because Load doesn't need entry data.
					string[] bits = line.Split(' ');
					for (int i = 0; i < bits.Length; ++i)
						bits[i] = Uri.UnescapeDataString(bits[i]);

					if (bits.Length > 0) {
						switch (bits[0]) {
							case "author":
								Author = bits[1];
								break;
							case "name":
								Name = bits[1];
								break;
							case "url":
								Url = bits[1];
								break;
							case "sections":
								this.forwards.Name = bits[1];
								this.backwards.Name = bits[2];
								this.SectionNames = new string[] { bits[1], bits[2] };
								break;
							case "languages":
								FirstLanguage = bits[1];
								SecondLanguage = bits[2];
								break;
							case "languages-reverse":
								FirstLanguageReverse = bits[1];
								SecondLanguageReverse = bits[2];
								break;
							case "language-codes":
								FirstLanguageCode = bits[1];
								SecondLanguageCode = bits[2];
								break;
							case "sizes":
								this.SectionSizes = new int[] { int.Parse(bits[1]), int.Parse(bits[2]) };
								break;
							case "sorted":
								sorted = true;
								break;
							default:
								break;
						}
					}
				}
			} catch (IOException ex) {
				throw new DictionaryLoadException(ex.Message, ex);
			}

			if (!sorted) {
				Comparison<Entry> comparer = (a, b) => a.Phrase.CompareTo(b.Phrase);
				forwardsEntries.Sort(comparer);
				backwardsEntries.Sort(comparer);
			}
		}

		protected void OpenFile() {
			reader = new Utf8LineReader(Path);
		}

		public Entry GetFullEntry(Entry entryStub, Section section) {
			System.Diagnostics.Debug.Print("Getting entry {0}", entryStub.Phrase);
			long bytePosition = (long)entryStub.Tag.Data;
			Entry entry = null, current = null;
			Translation lastTrans = null;
			string[] bits;

			if (reader == null)
				OpenFile();

			reader.Seek(bytePosition);
			while (!reader.EndOfStream) {
				string line = reader.ReadLine();
				ApplyLine(line, null, ref current, ref lastTrans, out bits);

				//We want to return if a new entry was defined and we already have an entry.
				if (current != entry) {
					if (entry != null)
						return entry;
					else
						entry = current;
				}
			}

			if (entry == null)
				throw new ArgumentException("Got a null entry when loading from the given tag.", "entryStub");
			//We reached the end of the file.
			return entry;
		}

		/// <summary>Takes a line from the dictionary source file and applies its properties to the entry and translation given, where applicable.
		/// If the property is a translation, it will be added to the entry and <paramref name="lastTranslation"/> will be updated.
		/// If the property defines a new entry, <paramref name="entry"/> will be updated and false will be returned.</summary>
		/// <param name="pool">A string pool to pool strings in, if necessary.</param>
		/// <param name="entry">Will be updated if a new entry is started.</param>
		/// <param name="lastTranslation">Will be updated if a translation is added to the entry.</param>
		/// <param name="bits">The individual parts of the line, unescaped.</param>
		/// <returns><value>True</value> if the property could be applied to the entry, otherwise false (e.g. the property is a file-level property).</returns>
		/// <remarks>Is the return value really needed?</remarks>
		protected bool ApplyLine(string line, StringPool pool, ref Entry entry, ref Translation lastTranslation, out string[] bits) {
			bits = line.Split(' ');
			for (int i = 0; i < bits.Length; ++i)
				bits[i] = Uri.UnescapeDataString(bits[i]);

			//This may be unoptimal.
			if (pool == null)
				pool = new StringPool();

			if (bits.Length > 0) {
				switch (bits[0]) {
					case "t":
						bits[1] = pool.Pool(bits[1].Normalize());
						entry.Translations.Add(lastTranslation = new Translation(bits[1]));
						break;

					//It may make more sense to have a separate string pool for PoS information.
					case "pos":
						lastTranslation.PartOfSpeech = pool.Pool(bits[1]);
						break;

					//When called from Load, this doesn't actually get executed. Load special-cases those
					//properties to load only the phrase, not the translation or metadata.
					//Return true, I guess.
					case "f": case "b":
						bits[1] = pool.Pool(bits[1].Normalize());
						entry = new Entry(bits[1], new List<Translation>());
						break;

					//It couldn't be applied. Return false so that the caller can try applying file-level properties.
					default:
						return false;
				}
			}

			return true;
		}

		/// <summary>Escapes characters \0, %, space, \n and \r. Used to avoid wasting 
		/// space URI encoding unicode values.</summary>
		/// <param name="s">String to be encoded</param>
		/// <returns>Encoded string, suitable for use with System.Uri.UnescapeDataString</returns>
		private string Escape(string s) {
			return s.Replace("\0", "%00").Replace("%", "%25").Replace(" ", "%20").Replace("\n", "%0D").Replace("\r", "%0A");
		}

		/// <summary>
		/// Writes all entries to a file. There's no way to tell if the dictionary entries have been modified,
		/// as of yet, so currently we must write the file whether it's necessary or not.
		/// </summary>
		/// <param name="path">The file name (relative or absolute) to write to.</param>
		public void Write(string path) {
			//First, fully load any entry stubs before we start writing, to avoid massive data loss...
			foreach (Entry e in ForwardsSection)
				ForwardsSection.GetFullEntry(e);
			foreach (Entry e in ReverseSection)
				ReverseSection.GetFullEntry(e);

			//Now dispose of the reader so that we can unlock the file.
			//The file needs to be truncated anyway, so it's not possible to share the file stream
			//with the Line Reader.
			if (reader != null) {
				reader.Dispose();
				reader = null;
			}

			using (Stream stream = File.Open(path, FileMode.Create, FileAccess.Write)) {
				using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8)) {
					writer.WriteLine(magicNumber);

					if (Name != null)
						writer.WriteLine("name " + Uri.EscapeDataString(Name));
					if (Author != null)
						writer.WriteLine("author " + Uri.EscapeDataString(Author));
					if (Url != null)
						writer.WriteLine("url " + Uri.EscapeDataString(Url));

					//TODO: WritePairedProperty
					if (ForwardsSection.Name != null || ReverseSection.Name != null)
						writer.WriteLine(string.Format("sections {0} {1}", Uri.EscapeDataString(ForwardsSection.Name), Uri.EscapeDataString(ReverseSection.Name)));
					if (FirstLanguage != null || SecondLanguage != null)
						writer.WriteLine(string.Format("languages {0} {1}", Uri.EscapeDataString(FirstLanguage), Uri.EscapeDataString(SecondLanguage)));
					if (FirstLanguageReverse != null || SecondLanguageReverse != null)
						writer.WriteLine(string.Format("languages-reverse {0} {1}", Uri.EscapeDataString(FirstLanguageReverse), Uri.EscapeDataString(SecondLanguageReverse)));
					if (FirstLanguageCode != null || SecondLanguageCode != null)
						writer.WriteLine(string.Format("language-codes {0} {1}", Uri.EscapeDataString(FirstLanguageCode), Uri.EscapeDataString(SecondLanguageCode)));
					writer.WriteLine("sorted");

					foreach (Entry entry in ForwardsSection)
						WriteEntry(writer, "f", entry);

					foreach (Entry entry in ReverseSection)
						WriteEntry(writer, "b", entry);
				}
			}
		}

		/// <summary>
		/// Writes an entry and its annotations to a TextWriter.
		/// </summary>
		/// <param name="type">Type of entry (usually 'b' or 'f')</param>
		/// <param name="entry">Entry instance to be written</param>
		void WriteEntry(TextWriter writer, string type, Entry entry) {
			writer.WriteLine(type + " " + Escape(entry.Phrase));
			
			foreach (Translation tr in entry.Translations) {
				writer.WriteLine("t " + Escape(tr.Value));
				if (tr.PartOfSpeech != null) {
					writer.Write("pos ");
					writer.WriteLine(tr.PartOfSpeech);
				}
			}
		}

		public SimpleDictionary(Section forwards, Section backwards) {
			this.forwards = forwards;
			this.backwards = backwards;
		}

		/// <remarks>This method is preferable to the one taking IEnumerable because it preserves names.</remarks>
		public SimpleDictionary(IDictionarySection forwards, IDictionarySection backwards) {
			this.forwards = new Section(new List<Entry>(forwards), this);
			this.forwards.Name = forwards.Name;
			this.backwards = new Section(new List<Entry>(backwards), this);
			this.backwards.Name = backwards.Name;
		}

		public SimpleDictionary(IEnumerable<Entry> forwards, IEnumerable<Entry> backwards) {
			this.forwards = new Section(new List<Entry>(forwards), this);
			this.backwards = new Section(new List<Entry>(backwards), this);
		}

		public void Save() {
			if (Path != null) {
				Write(Path);
			} else {
				throw new InvalidOperationException();
			}
		}
		
		#region Properties
		[Browsable(false)]
		public string Path { get; set; }

		[Browsable(false)]
		public IDictionarySection ForwardsSection {
			get { return forwards; }
		}

		[Browsable(false)]
		public IDictionarySection ReverseSection {
			get { return backwards; }
		}

		[DisplayName("Name"), Description("Name")]
		public string Name { get; set; }

		[DisplayName("Author"), Description("Author")]
		public string Author { get; set; }

		[DisplayName("Url"), Description("Url")]
		public string Url { get; set; }

		[DisplayName("FirstLanguage"), Description("FirstLanguage")]
		public string FirstLanguage { get; set; }

		[DisplayName("SecondLanguage"), Description("SecondLanguage")]
		public string SecondLanguage { get; set; }

		[DisplayName("FirstLanguage"), Description("FirstLanguageReverse")]
		public string FirstLanguageReverse { get; set; }

		[DisplayName("SecondLanguage"), Description("SecondLanguageReverse")]
		public string SecondLanguageReverse { get; set; }

		[DisplayName("FirstLanguageCode"), Description("FirstLanguageReverseCode")]
		public string FirstLanguageCode { get; set; }

		[DisplayName("SecondLanguageCode"), Description("SecondLanguageReverseCode")]
		public string SecondLanguageCode { get; set; }
		
		public string[] SectionNames { get; protected set; }
		public int[] SectionSizes { get; protected set; }
		#endregion

		#region Dispose
		public void Dispose() {
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if(reader != null)
					reader.Dispose();
			}
		}
		#endregion
	}

	[System.Diagnostics.DebuggerDisplay("StringPool ({Count} unique of {Calls} pooled)")]
	public class StringPool {
		//In lieu of a proper HashSet, this will do.
		private Dictionary<string, string> values = new Dictionary<string, string>();

		public StringPool() {
			ShouldPool = true;
		}

		public string Pool(string str) {
			Calls++;
			if (!ShouldPool)
				return str;

			string existing;
			if (values.TryGetValue(str, out existing)) {
				//Leave str for the garbage collector to feed upon.
				//Ｉ　ｆｅｅｌ　ｋｉｎｄ　ｏｆ　ｂａｄ　ａｂｏｕｔ　ｉｔ　：（ 
				return existing;
			} else {
				values.Add(str, str);
				return str;
			}
		}

		public bool ShouldPool {
			get; set;
		}

		public int Calls {
			get; protected set;
		}

		public int Count {
			get { return values.Count; }
		}
	}
}
