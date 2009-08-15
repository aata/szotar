using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using P = System.IO.Path;

namespace Szotar {
	[TypeDescriptionProvider(typeof(LocalizedTypeDescriptionProvider<SimpleDictionary>))]
	public class SimpleDictionary : IBilingualDictionary {
		Section forwards, backwards;
		Utf8LineReader reader;
		const string magicNumber = "sdict-magic";
		string revisionID = string.Empty;

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

			public int HeadWords {
				get { return entries.Count; }
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

			public void GetFullEntry(Entry stub) {
				//We've already got the full entry, so no work to do.
				if (stub.Translations != null)
					return;
				if (stub.Tag == null)
					throw new ArgumentException("The in-memory dictionary entry has no entry tag, and cannot be fully loaded.");

				Entry full = dictionary.GetFullEntry(stub, this);
				stub.Translations = full.Translations;

				// Actually, this should still be kept. It allows for saving the dictionary cache.
				//stub.Tag = null;
			}
		}

		public class Info : DictionaryInfo {
			public Info(string path) {
				using (SimpleDictionary dict = new SimpleDictionary(path, false, false))
					Load(dict);

				GetFullInstance = () => new SimpleDictionary(Path);
			}

			public Info(SimpleDictionary dict) {
				Load(dict);

				// Keep a weak reference to the dictionary. This way we don't have to load the 
				// dictionary again if we re-open it soon after we close it.
				var weak = new NullWeakReference<SimpleDictionary>(dict);
				GetFullInstance = () => FromWeak(weak);
			}

			private SimpleDictionary FromWeak(NullWeakReference<SimpleDictionary> weak) {
				var dict = weak.Target;
				if (dict != null)
					return dict;

				return new SimpleDictionary(Path);
			}

			void Load(SimpleDictionary dict) {
				Path = dict.Path;
				Name = dict.Name;
				Author = dict.Author;
				Url = dict.Url;

				SectionNames = dict.SectionNames;
				if (dict.FirstLanguage != null || dict.SecondLanguage != null)
					Languages = new string[] { dict.FirstLanguage, dict.SecondLanguage };
				SectionSizes = dict.SectionSizes;
			}
		}

		DictionaryInfo IBilingualDictionary.Info {
			get {
				return new Info(this);
			}
		}

		public SimpleDictionary(string path)
			: this(path, true, true) {
		}

		public SimpleDictionary(string path, bool poolStrings, bool full) {
			var timer = new System.Diagnostics.Stopwatch();
			timer.Start();

			Path = path;

			if (full) {
				// Get the revision ID so we can check if the cache is correct.
				using (var info = new SimpleDictionary(path, poolStrings, false)) {
					revisionID = info.revisionID;

					// Knowing the name is useful too, for debugging.
					Name = info.Name;
				}

				if (LoadCache()) {
					Metrics.LogMeasurement(string.Format("Loading {0} from cache", this.Name), timer.Elapsed);
					return;
				}
			}

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

					if (line.StartsWith("f ") || line.StartsWith("b ") || line.StartsWith("t ")) {
						if (!full) {
							Metrics.LogMeasurement(string.Format("Loaded {0} header", this.Name), timer.Elapsed);
							return; // We got to the actual entries in the dictionary, but we just want the headers.
						}

						string rest = Uri.UnescapeDataString(line.Substring(2).Normalize());
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

					// We're only looking for file-level properties, because Load doesn't need entry data.
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
							case "languages":
								FirstLanguage = bits[1];
								SecondLanguage = bits[2];
								break;
							case "language-codes":
								FirstLanguageCode = bits[1];
								SecondLanguageCode = bits[2];
								break;
							case "sorted":
								sorted = true;
								break;
							case "headwords-hint":
								this.SectionSizes = new int[] { int.Parse(bits[1]), int.Parse(bits[2]) };
								break;
							case "revision-id":
								this.revisionID = bits[1];
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

			if (!full && reader != null) {
				reader.Dispose();
				reader = null;
			}

			Metrics.LogMeasurement(string.Format("Loading {0} without cache", this.Name), timer.Elapsed);

			SaveCache();
		}

		private void LoadMissingEntries() {
			// TODO: Implement this.
			// Instead of calling GetFullEntry for every entry, read the file sequentially.
			// This should be faster.
		}

		protected void OpenFile() {
			reader = new Utf8LineReader(Path);
		}

		public Entry GetFullEntry(Entry entryStub, Section section) {
			long bytePosition = (long)entryStub.Tag.Data;
			Entry entry = null, current = null;
			Translation lastTrans = null;

			if (reader == null)
				OpenFile();

			reader.Seek(bytePosition);
			while (!reader.EndOfStream) {
				string line = reader.ReadLine();
				ApplyLine(line, null, ref current, ref lastTrans);

				// We want to return if a new entry was defined and we already have an entry.
				if (current != entry) {
					if (entry != null)
						return entry;
					else
						entry = current;
				}
			}

			if (entry == null)
				throw new ArgumentException("Got a null entry when loading from the given tag.", "entryStub");
			// We reached the end of the file.
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
		protected bool ApplyLine(string line, StringPool pool, ref Entry entry, ref Translation lastTranslation) {
			pool = pool ?? new StringPool();

			if (line.StartsWith("t ")) {
				var tr = Uri.UnescapeDataString(line.Substring(2).Normalize());
				entry.Translations.Add(lastTranslation = new Translation(tr));
				return true;
			}

			// When called from Load, this doesn't actually get executed. Load special-cases those
			// properties to load only the phrase, not the translation or metadata.
			// Return true, I guess.
			if (line.StartsWith("f ") || line.StartsWith("b ")) {
				string headWord = Uri.UnescapeDataString(pool.Pool(line.Substring(2)).Normalize());
				entry = new Entry(headWord, new List<Translation>());
				return true;
			} 

			string[] bits = line.Split(' ');
			for (int i = 0; i < bits.Length; ++i)
				bits[i] = Uri.UnescapeDataString(bits[i]);

			//Note: f, b, and t don't need %20, so we can't use bits.
			if (bits.Length > 0) {
				switch (bits[0]) {
					//It may make more sense to have a separate string pool for PoS information.
					case "ps":
						//I've temporarily removed PoS, as it's going to be moved to Entry anyway. (Probably)
						//lastTranslation.PartOfSpeech = pool.Pool(bits[1]);
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
		private string Escape(string s, bool escapeSpace) {
			var sb = new StringBuilder();

			foreach (char c in s) {
				switch (c) {
					case ' ':
						if (escapeSpace)
							sb.Append("%20");
						else
							sb.Append(' ');
						break;
					case '\0': sb.Append("%00"); break;
					case '%': sb.Append("%25"); break;
					case '\n': sb.Append("%0D"); break;
					case '\r': sb.Append("%0A"); break;
					default: sb.Append(c); break;
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Writes all entries to a file. There's no way to tell if the dictionary entries have been modified,
		/// as of yet, so currently we must write the file whether it's necessary or not.
		/// </summary>
		/// <param name="path">The file name (relative or absolute) to write to.</param>
		public void Write(string path) {
			// As soon as we start writing the file, the indices in the cache file will probably be invalid.
			DestroyCache();

			// Update the revision ID. Instead of incrementing a number, pick a random GUID and use that.
			// If incrementing were used, then if a dictionary were copied to a different computer, modified,
			// and copied back to the original computer, where the dictionary had also been modified, the 
			// cache revision ID on the original computer would still match that of the dictionary file, despite
			// being invalid.
			revisionID = Guid.NewGuid().ToString();

			Metrics.Measure(string.Format("Writing dictionary {0} to file", this.Name), delegate {
				Metrics.Measure(string.Format("Fully loading {0} before writing", this.Name), delegate {
					// First, fully load any entry stubs before we start writing, to avoid massive data loss...
					foreach (Entry e in ForwardsSection)
						ForwardsSection.GetFullEntry(e);
					foreach (Entry e in ReverseSection)
						ReverseSection.GetFullEntry(e);
				});

				// Now dispose of the reader so that we can unlock the file.
				// The file needs to be truncated anyway, so it's not possible to share the file stream
				// with the Line Reader.
				if (reader != null) {
					reader.Dispose();
					reader = null;
				}

				using (Stream stream = File.Open(path, FileMode.Create)) {
					using (var writer = new LineWriter(stream)) {
						writer.WriteLine(magicNumber);

						if (Name != null)
							writer.WriteLine("name " + Uri.EscapeDataString(Name));
						if (Author != null)
							writer.WriteLine("author " + Uri.EscapeDataString(Author));
						if (Url != null)
							writer.WriteLine("url " + Uri.EscapeDataString(Url));
						writer.WriteLine("revision-id " + Uri.EscapeDataString(revisionID));

						// TODO: WritePairedProperty
						writer.WriteLine(string.Format("headwords-hint {0} {1}", forwards.HeadWords, backwards.HeadWords));
						if (FirstLanguage != null || SecondLanguage != null)
							writer.WriteLine(string.Format("languages {0} {1}", Uri.EscapeDataString(FirstLanguage), Uri.EscapeDataString(SecondLanguage)));
						if (FirstLanguageCode != null || SecondLanguageCode != null)
							writer.WriteLine(string.Format("language-codes {0} {1}", Uri.EscapeDataString(FirstLanguageCode), Uri.EscapeDataString(SecondLanguageCode)));
						writer.WriteLine("sorted");

						foreach (Entry entry in ForwardsSection)
							WriteEntry(writer, "f ", entry);

						foreach (Entry entry in ReverseSection)
							WriteEntry(writer, "b ", entry);
					}
				}
			});

			SaveCache();
		}

		/// <summary>
		/// Writes an entry and its annotations to a TextWriter.
		/// </summary>
		/// <param name="type">Type of entry (usually 'b' or 'f')</param>
		/// <param name="entry">Entry instance to be written</param>
		void WriteEntry(LineWriter writer, string type, Entry entry) {
			// Reset the tag so that writing the cache can work.
			entry.Tag = new EntryTag(entry.Tag.DictionarySection, writer.Position);

			writer.Write(type);
			writer.WriteLine(Escape(entry.Phrase, false));

			foreach (Translation tr in entry.Translations) {
				writer.Write("t ");
				writer.WriteLine(Escape(tr.Value, false));

				//if (tr.PartOfSpeech != null) {
				//    writer.Write("ps ");
				//    writer.WriteLine(Escape(tr.PartOfSpeech, true));
				//}
			}
		}

		public SimpleDictionary(Section forwards, Section backwards) {
			this.forwards = forwards;
			this.backwards = backwards;
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

		#region Caching
		static readonly int CacheFormatVersion = 2;

		string CachePath() {
			return P.ChangeExtension(Path, ".dict.cache");
		}

		void DestroyCache() {
			File.Delete(CachePath());
		}

		// Saves the dictionary cache, on a separate thread.
		void SaveCache() {
			string path = CachePath();

			string
				name = Name ?? string.Empty,
				author = Author ?? string.Empty,
				url = Url ?? string.Empty,
				firstLanguage = FirstLanguage ?? string.Empty,
				firstLanguageCode = FirstLanguageCode ?? string.Empty,
				secondLanguage = SecondLanguage ?? string.Empty,
				secondLanguageCode = SecondLanguageCode ?? string.Empty;

			// We have to save a copy of the list of entries in case it gets modified while the thread is running.
			var forwardsEntries = new List<Entry>(forwards.HeadWords);
			foreach (var entry in forwards)
				forwardsEntries.Add(entry.Clone());
			var backwardsEntries = new List<Entry>(backwards.HeadWords);
			foreach (var entry in backwards)
				backwardsEntries.Add(entry.Clone());

			new Thread(new ThreadStart(delegate {
				Metrics.Measure(string.Format("Saving dictionary cache for {0}", this.Name), delegate {
					try {
						using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None)) {
							using (var writer = new BinaryWriter(stream)) {
								writer.Write(CacheFormatVersion);
								writer.Write(this.revisionID ?? "");

								foreach (var str in new[] { name, author, url, firstLanguage, firstLanguageCode, secondLanguage, secondLanguageCode })
									writer.Write(str);

								foreach (var section in new[] { forwardsEntries, backwardsEntries }) {
									writer.Write(section.Count);
									foreach (var entry in section) {
										writer.Write(entry.Phrase);

										writer.Write((long)entry.Tag.Data);
									}
								}
							}
						}
					} catch (IOException e) {
						// If we can't save the cache, it's not a huge problem.
						ProgramLog.Default.AddMessage(LogType.Error, "Can't save cache for dictionary {0}: {1}", name, e.Message);
					}
				});
			})).Start();
		}

		bool LoadCache() {
			string path = CachePath();

			if (!File.Exists(path))
				return false;

			var dictModified = File.GetLastWriteTimeUtc(this.Path);
			var cacheModified = File.GetLastWriteTimeUtc(path);

			if (cacheModified < dictModified) {
				ProgramLog.Default.AddMessage(LogType.Error, "Cache file for {0} was invalid because the last write time was earlier than the dictionary file.", this.Path);
				return false;
			}

			try {
				using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					using (var reader = new BinaryReader(stream)) {
						int version = reader.ReadInt32();

						if (version > CacheFormatVersion)
							return false;

						// Check the cache revision ID against the dictionary's revision ID.
						if (version > 1) {
							var revisionID = reader.ReadString();
							if (revisionID != this.revisionID) {
								ProgramLog.Default.AddMessage(LogType.Warning, "Cache file for {0} did not have the same revision ID as the dictionary itself", this.Name);
								reader.Close();
								File.Delete(path);
								return false;
							}
						} else {
							// Version 1 caches don't include a revision ID, and thus aren't reliable.
							ProgramLog.Default.AddMessage(LogType.Warning, "Cache file for {0} was of version 1, which does not include revision IDs, and was thus deleted", this.Name);
							reader.Close();
							File.Delete(path);
							return false;
						}

						this.Name = reader.ReadString();
						this.Author = reader.ReadString();
						this.Url = reader.ReadString();
						this.FirstLanguage = reader.ReadString();
						this.FirstLanguageCode = reader.ReadString();
						this.SecondLanguage = reader.ReadString();
						this.SecondLanguageCode = reader.ReadString();

						forwards = LoadCacheSection(reader);
						backwards = LoadCacheSection(reader);

						return true;
					}
				}
			} catch (IOException) { // EndOfStreamException falls into this.
				// Maybe it's corrupt and this will fix it.
				File.Delete(path);
				return false;
			}
		}

		Section LoadCacheSection(BinaryReader reader) {
			int capacity = reader.ReadInt32();

			var entries = new List<Entry>(capacity);
			var section = new Section(entries, this);

			for (int i = 0; i < capacity; ++i)
				entries.Add(LoadCacheEntry(reader, section));

			return section;
		}

		private Entry LoadCacheEntry(BinaryReader reader, Section section) {
			string phrase = reader.ReadString();

			//int translationCount = reader.ReadInt32();
			//var translations = new List<Translation>(translationCount);
			//for(int i = 0; i < translationCount; ++i)
			//    translations.Add(new Translation(reader.ReadString()));

			return new Entry(phrase, null) { Tag = new EntryTag(section, reader.ReadInt64()) };
		}
		#endregion

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

		public string Name { get; set; }
		public string Author { get; set; }
		public string Url { get; set; }
		public string FirstLanguage { get; set; }
		public string SecondLanguage { get; set; }
		[Browsable(false)]
		public string FirstLanguageCode { get; set; }
		[Browsable(false)]
		public string SecondLanguageCode { get; set; }

		[Browsable(false)]
		public string[] SectionNames { get; protected set; }
		[Browsable(false)]
		public int[] SectionSizes { get; protected set; }
		#endregion

		#region Dispose
		public void Dispose() {
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (reader != null)
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
			get;
			set;
		}

		public int Calls {
			get;
			protected set;
		}

		public int Count {
			get { return values.Count; }
		}
	}
}
