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
			int fullyLoadedEntries;

			public Section(IList<Entry> entries, bool translationsLoaded, SimpleDictionary dictionary) {
				this.entries = entries;
				this.dictionary = dictionary;
				if (translationsLoaded)
					fullyLoadedEntries = entries.Count;
			}

			public SimpleDictionary Dictionary {
				get { return dictionary; }
			}

			public int HeadWords {
				get { return entries.Count; }
			}

			public int FullyLoadedCount {
				get { return fullyLoadedEntries; }
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
				// We've already got the full entry, so no work to do.
				if (stub.Translations != null)
					return;

				System.Diagnostics.Debug.Assert(stub.Tag != null);

				Entry full = dictionary.GetFullEntry(stub, this);
				stub.Translations = full.Translations;
				fullyLoadedEntries++;
			}

			public void FillOutPartialEntry(Entry partial, Entry full) {
				if (partial.Translations == null) {
					partial.Translations = full.Translations;
					fullyLoadedEntries++;
				}
			}

			public Entry FindEntry(string phrase) {
				int lower = 0, upper = entries.Count - 1;

				while (true) {
					if (upper < lower)
						return null;

					int mid = lower + (upper - lower) / 2;

					int c = phrase.CompareTo(entries[mid].Phrase);
					if (c < 0)
						upper = mid - 1;
					else if (c > 0)
						lower = mid + 1;
					else
						return entries[mid];
				}
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

			bool alreadySorted;
			OpenFile(full);
			LoadHeader(true, out alreadySorted);

			if (!full) {
				Metrics.LogMeasurement(string.Format("Loading {0} header", this.Name), timer.Elapsed);
				return;
			}

			if (LoadCache()) {
				Metrics.LogMeasurement(string.Format("Loading {0} from cache", this.Name), timer.Elapsed);
				return;
			}

			// TODO: Compare the time to load with/without translations.
			LoadEntries(alreadySorted, false);

			Metrics.LogMeasurement(string.Format("Loading {0} without cache", this.Name), timer.Elapsed);

			SaveCache();
		}

		public SimpleDictionary(Section forwards, Section backwards) {
			this.forwards = forwards;
			this.backwards = backwards;
		}

		protected void OpenFile(bool full) {
			if (reader != null)
				reader.Seek(0);
			else {
				try {
					// We don't actually write to the file, but requesting ReadWrite access ensures
					// that nobody else has the file open for writing (unless someone else set FileShare.Write,
					// but that's unlikely, and this program doesn't do that).
					//
					// Opening with FileAccess.Read would mean that other programs could have the file open and 
					// modify it, thus invaliding the file offsets stored in the partially-loaded entries.
					//
					// We need to open with FileShare.Read because the main window and other programs need to 
					// read the dictionary header to populate the list. (This should probably change.)
					
					// Of course, if we're just reading the dictionary header, we only request read access.
					var access = full ? FileAccess.ReadWrite : FileAccess.Read;

					// If we're only loading a header, we have to set FileShare.ReadWrite or the call 
					// fails if the file is already open for ReadWrite.
					// If we can open the file for Read access, it should be safe to read from without it 
					// being modified (when writing to the dictionary, we specify FileShare.None).
					var share = full ? FileShare.Read : FileShare.ReadWrite;

					reader = new Utf8LineReader(Path, access, share);
				} catch (IOException e) {
					throw new DictionaryLoadException(e.Message, e);
				} catch (ArgumentException e) {
					throw new DictionaryLoadException(e.Message, e);
				} catch (UnauthorizedAccessException e) {
					throw new DictionaryLoadException(e.Message, e);
				}
			}
		}

		protected void CloseFile() {
			if (reader != null) {
				reader.Dispose();
				reader = null;
			}
		}

		void SkipHeader() {
			bool sorted;
			LoadHeader(false, out sorted);
		}

		void LoadHeader(bool applyProperties, out bool sorted) {
			long bytePos = 0;

			reader.Seek(0);

			sorted = false;
			bool firstLine = true;

			while (!reader.EndOfStream) {
				string line = reader.ReadLine(out bytePos);

				if (firstLine) {
					if(line != magicNumber)
						throw new DictionaryLoadException("The file being loaded is either corrupt or not a dictionary (wrong magic number).");

					firstLine = false;
					continue;
				}

				if (line.StartsWith("f ") || line.StartsWith("b ") || line.StartsWith("t ")) {
					reader.Seek(bytePos);
					return;
				}

				if (!applyProperties)
					continue;

				string[] bits = line.Split(' ');
				for (int i = 0; i < bits.Length; ++i)
					bits[i] = Uri.UnescapeDataString(bits[i]);

				if (bits.Length > 0) {
					string bit1 = bits.Length > 1 ? bits[1] : string.Empty;
					string bit2 = bits.Length > 2 ? bits[2] : string.Empty;

					switch (bits[0]) {
						case "author":
							Author = bit1;
							break;
						case "name":
							Name = bit1;
							break;
						case "url":
							Url = bit1;
							break;
						case "languages":
							FirstLanguage = bit1;
							SecondLanguage = bit2;
							break;
						case "language-codes":
							FirstLanguageCode = bit1;
							SecondLanguageCode = bit2;
							break;
						case "sorted":
							sorted = true;
							break;
						case "headwords-hint":
							int first, second;
							if(int.TryParse(bit1, out first) && int.TryParse(bit2, out second))
								this.SectionSizes = new int[] { first, second };
							break;
						case "revision-id":
							this.revisionID = bit1;
							break;
						default:
							ProgramLog.Default.AddMessage(LogType.Debug, "Unknown dictionary file metadata in {0}: {1}", Name ?? Path, line);
							break;
					}
				}
			}

			// This would mean there's no entries in the dictionary.
			ProgramLog.Default.AddMessage(LogType.Warning, "Dictionary {0} has no entries", Name ?? Path);
		}

		void LoadEntries(bool alreadySorted, bool loadTranslations) {
			try {
				var forwardsEntries = new List<Entry>();
				var backwardsEntries = new List<Entry>();
				this.forwards = new Section(forwardsEntries, loadTranslations, this);
				this.backwards = new Section(backwardsEntries, loadTranslations, this);

				LoadEntriesWith(loadTranslations, (tag, entry) => {
					if (tag == 'f')
						forwardsEntries.Add(entry);
					else
						backwardsEntries.Add(entry);
				});

				if (!alreadySorted) {
					forwardsEntries.Sort((a, b) => a.Phrase.CompareTo(b.Phrase));
					backwardsEntries.Sort((a, b) => a.Phrase.CompareTo(b.Phrase));
				}
			} catch (IOException ex) {
				throw new DictionaryLoadException(ex.Message, ex);
			} catch (ArgumentException ex) {
				throw new DictionaryLoadException(ex.Message, ex);
			} catch (UnauthorizedAccessException ex) {
				throw new DictionaryLoadException(ex.Message, ex);
			}
		}

		void LoadEntriesWith(bool loadTranslations, Action<char, Entry> action) {
			try {
				Entry entry = null;
				char tag = 'f';

				while (!reader.EndOfStream) {
					long bytePos;
					string line = reader.ReadLine(out bytePos);

					if (line.StartsWith("f ") || line.StartsWith("b ") || line.StartsWith("t ")) {
						string value = Uri.UnescapeDataString(line.Substring(2).Normalize());
						switch (line[0]) {
							case 'f':
								if(entry != null)
									action(tag, entry);

								tag = 'f';
								entry = new Entry(value, loadTranslations ? new List<Translation>() : null);
								entry.Tag = new EntryTag(forwards, bytePos);
								break;

							case 'b':
								if (entry != null)
									action(tag, entry);

								tag = 'b';
								entry = new Entry(value, loadTranslations ? new List<Translation>() : null);
								entry.Tag = new EntryTag(backwards, bytePos);
								break;

							case 't':
								if (!loadTranslations)
									break;

								if(entry != null)
									entry.Translations.Add(new Translation(value));
								else
									ProgramLog.Default.AddMessage(LogType.Error, "Dictionary {0} has a translation record before an entry was started", Name ?? Path);

								break;

							default:
								ProgramLog.Default.AddMessage(LogType.Debug, "Unknown dictionary entry metadata in {0}: {1}", Name ?? Path, line);
								break;
						}
					}
				}

				if (entry != null)
					action(tag, entry);
			} catch (IOException ex) {
				throw new DictionaryLoadException(ex.Message, ex);
			} catch (ArgumentException ex) {
				throw new DictionaryLoadException(ex.Message, ex);
			} catch (UnauthorizedAccessException ex) {
				throw new DictionaryLoadException(ex.Message, ex);
			}
		}

		private void PreloadPartialEntries() {
			Metrics.Measure(string.Format("Preload loading entries of {0} before writing", this.Name), delegate {
				foreach (var e in forwards)
					e.FullyLoad();
				foreach (var e in backwards)
					e.FullyLoad();
			});
		}

		protected Entry GetFullEntry(Entry entryStub, Section section) {
			try {
				if (reader == null)
					OpenFile(true);

				reader.Seek((long)entryStub.Tag.Data);

				// Check that the entry in the file vaguely matches the data we have.
				string filePhrase = Uri.UnescapeDataString(reader.ReadLine().Substring(2)).Normalize();
				if (filePhrase != entryStub.Phrase)
					throw new ArgumentException("The phrase found in the file differed from the entry's phrase.", "entryStub");

				var translations = new List<Translation>();

				while (!reader.EndOfStream) {
					string line = reader.ReadLine();

					if (line.StartsWith("f ") || line.StartsWith("b ")) {
						entryStub.Translations = translations;
						return entryStub;
					}

					if (line.StartsWith("t ")) {
						var value = Uri.UnescapeDataString(line.Substring(2)).Normalize();
						translations.Add(new Translation(value));
					}
				}

				entryStub.Translations = translations;
				return entryStub;
			} catch (IOException ex) {
				throw new DictionaryLoadException(ex.Message, ex);
			} catch (ArgumentException ex) {
				throw new DictionaryLoadException(ex.Message, ex);
			} catch (UnauthorizedAccessException ex) {
				throw new DictionaryLoadException(ex.Message, ex);
			}
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
		protected void Write(string path) {
			// Update the revision ID. Instead of incrementing a number, pick a random GUID and use that.
			// If incrementing were used, then if a dictionary were copied to a different computer, modified,
			// and copied back to the original computer, where the dictionary had also been modified, the 
			// cache revision ID on the original computer would still match that of the dictionary file, despite
			// being invalid.
			revisionID = Guid.NewGuid().ToString();

			// If we're not writing cautiously, we can't read the entries on-demand while the temporary output file 
			// is written.
			if (path == Path)
				CloseFile();

			PreloadPartialEntries();

			Metrics.Measure(string.Format("Writing dictionary {0} to file", this.Name), delegate {
				// Now dispose of the reader so that we can unlock the file.
				// The file needs to be truncated anyway, so it's not possible to share the file stream
				// with the Line Reader.
				if (reader != null) {
					reader.Dispose();
					reader = null;
				}

				using (var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None)) {
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
		}

		/// <summary>
		/// Writes an entry and its annotations to a TextWriter.
		/// </summary>
		/// <param name="type">Type of entry (usually 'b' or 'f')</param>
		/// <param name="entry">Entry instance to be written</param>
		void WriteEntry(LineWriter writer, string type, Entry entry) {
			entry.FullyLoad();

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

		public void Save() {
			if (Path != null) {
				try {
					string tempFile = P.GetTempFileName();
					string backupFile = P.GetTempFileName();

					// Write the new version to a temporary file, first. If that fails, the actual dictionary file
					// is not modified.
					Write(tempFile);

					// Try to make a backup of the file. If that isn't possible for some reason, just
					// delete it, since we've written the replacement file.
					try {
						CloseFile();
						DestroyCache();
						File.Move(Path, backupFile);
					} catch (IOException) {
						File.Delete(Path);
					} catch (UnauthorizedAccessException) {
						File.Delete(Path);
					}

					// Overwrite the dictionary file with the new version.
					File.Move(tempFile, Path);

					// Try to open the file again, so that other people can't modify it.
					// If it fails, we can try again later.
					try {
						OpenFile(true);
					} catch (IOException) {
					} catch (UnauthorizedAccessException) { }

					File.Delete(backupFile);

					SaveCache();
				} catch (IOException ex) {
					throw new DictionarySaveException(ex.Message, ex);
				} catch (UnauthorizedAccessException ex) {
					throw new DictionarySaveException(ex.Message, ex);
				} catch (ArgumentException ex) {
					throw new DictionarySaveException(ex.Message, ex);
				}
			} else {
				throw new InvalidOperationException("Can't save a dictionary that doesn't have a Path");
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
			var section = new Section(entries, false, this);

			for (int i = 0; i < capacity; ++i)
				entries.Add(LoadCacheEntry(reader, section));

			return section;
		}

		private Entry LoadCacheEntry(BinaryReader reader, Section section) {
			string phrase = reader.ReadString();

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
