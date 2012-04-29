using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Szotar {
	public class SectionInfo {
		public string Name { get; set; }
		public DateTime Date { get; set; }
		public string Maintainer { get; set; }
		public int? ItemCount { get; set; }
		public string Copyright { get; set; }
		/// <summary>Size in bytes</summary>
		public int Size { get; set; }
	}

	public interface IImportListener {
		void ReportError(string error);
		void ReportError(string headWord, string error);
		void ReportWarning(string warning);

	}

	/// <summary>
	/// Imports a dictionary section. An importer needs only to support a single import at once.
	/// </summary>
	public interface IDictionarySectionImporter : IDisposable {
		/// <summary>Imports a dictionary section from a file. This function, if called by DualSectionImporter, will
		/// be called on a separate thread. If the operation is cancelled, this method should throw an OperationCanceledException.
		/// </summary>
		/// <param name="path">File from which to import the dictionary.</param>
		/// <param name="info">Information about the dictionary section such as the name, maintainer...</param>
		/// <returns>A section of the dictionary as a SimpleDictionary.Section. </returns>
		/// <exception cref="System.OperationCanceledException"/>
		SimpleDictionary.Section Import(string path, out SectionInfo info);

		event EventHandler<ProgressMessageEventArgs> ProgressChanged;

		/// <summary>
		/// This method should either block until the operation has been cancelled, or throw an exception.
		/// A NotSupportedException should be thrown if the importer does not support cancellation.
		/// </summary>
		void Cancel();
	}

	namespace Dbf {
		using System.Data.Common;

		[Importer("DBF", typeof(IDictionarySection))]
		[ImporterDescription("DBF Importer",  "Dbf")]
		public class Importer : IDictionarySectionImporter {
			long shouldCancel = 0L;

			public SimpleDictionary.Section Import(string path, out SectionInfo info) {
				var entries = new SortedDictionary<string, List<Translation>>();
				string fromName, toName;

				using (var conn = Connect(path)) {
					int rows = 0;
					using (var getRowCount = conn.CreateCommand()) {
						getRowCount.CommandText = "SELECT COUNT(*) FROM SZOTAR;";
						rows = Convert.ToInt32(getRowCount.ExecuteScalar());
					}

					int row = 0, lastPercent = -1;
					using (var cmd = conn.CreateCommand()) {
						cmd.CommandText = "SELECT * FROM " + System.IO.Path.GetFileNameWithoutExtension(path) + ";";
						using (var reader = cmd.ExecuteReader()) {
							fromName = reader.GetName(0);
							toName = reader.GetName(1);

							while (reader.Read()) {
								row++;

								if (Interlocked.Read(ref shouldCancel) > 0)
									throw new OperationCanceledException();
								var key = reader.GetString(0);
								var data = reader.GetString(1);

								int percent = row * 100 / (rows != 0 ? rows : 1);
								if (percent > lastPercent) {
									OnProgressChanged(string.Format("Row {0} of {1}", row, rows), percent);
									lastPercent = percent;
								}

								List<Translation> translations;
								if (entries.TryGetValue(key, out translations)) {
									translations.Add(new Translation(data));
								} else {
									entries.Add(key, new List<Translation>(new Translation[] { new Translation(data) }));
								}
							}
						}
					}
				}

				var actualEntries = new List<Entry>();
				foreach(var kv in entries)
					actualEntries.Add(new Entry(kv.Key, kv.Value));
				var section = new SimpleDictionary.Section(actualEntries, true, null);
				foreach(var e in actualEntries)
					e.Tag = new EntryTag(section, null);

				info = new SectionInfo {
					Date = DateTime.Now,
					ItemCount = actualEntries.Count,
					Size = 0, 
					Name = fromName + "-" + toName
				};

				return section;
			}

			public event EventHandler<ProgressMessageEventArgs> ProgressChanged;
			private void OnProgressChanged(string message, int? percent) {
				EventHandler<ProgressMessageEventArgs> temp = ProgressChanged;
				if (temp != null)
					temp(this, new ProgressMessageEventArgs(message, percent));
			}

			DbConnection Connect(string path) {
				var builder = new DbConnectionStringBuilder();
				builder.Add("Provider", "Microsoft.Jet.OLEDB.4.0");
				path = System.IO.Path.GetDirectoryName(path);
				builder.Add("Data Source", path);
				builder.Add("Extended Properties", "dBASE III");
				var conn = new System.Data.OleDb.OleDbConnection(builder.ConnectionString);
				conn.Open();
				return conn;
			}

			public void Cancel() {
				Interlocked.Increment(ref shouldCancel);
			}

			public void Dispose() {
			}
		}
	}

	namespace Zbedic {
		using System.IO;
		using System.Threading;
		using GZipStream = System.IO.Compression.GZipStream;

		[Importer("Zbedic", typeof(IDictionarySection))]
		[ImporterDescription("Zbedic Importer", "Zbedic")]
		public class Importer : IDictionarySectionImporter {
			private long shouldCancel = 0L;

			public SectionInfo GetInfo(string path) {
				using (Stream fileStream = File.OpenRead(path)) {
					using (GZipStream stream = new GZipStream(fileStream, System.IO.Compression.CompressionMode.Decompress, false)) {
						using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
							return ReadInfoFromStream(reader);
						}
					}
				}
			}

			public SimpleDictionary.Section Import(string path, out SectionInfo info) {
				List<Entry> entries = new List<Entry>();

				using (Stream fileStream = File.OpenRead(path)) {
					using (GZipStream stream = new GZipStream(fileStream, System.IO.Compression.CompressionMode.Decompress, false)) {
						using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
							info = ReadInfoFromStream(reader);
							if (reader.EndOfStream)
								throw new ImportException("The file has no entries or is not a Zbedic dictionary.");
							int percentage = 0, lastPercentage = 0;
							long compressedSize = new FileInfo(path).Length;

							string phrase = reader.ReadLine();

							while (true) {
								{
									if (Interlocked.CompareExchange(ref shouldCancel, 0L, 1L) > 0)
										throw new OperationCanceledException();
								}

								string line = reader.ReadLine();
								string[] bits = line.Split(new char[] { '\0' }, 2, StringSplitOptions.RemoveEmptyEntries);
								List<Entry> senses;
								try {
									senses = ParseSenses(bits[0], phrase);
								} catch (InvalidDataException) {
									ProgramLog.Default.AddMessage(LogType.Error, "Invalid word sense for phrase \"{0}\" in {1}: {2}", phrase, path, bits[0]);
									continue;
								}
								if (bits.Length > 1) {
									entries.AddRange(senses);

									string suffix = string.Empty;
									int n, max;
									if (info.ItemCount.HasValue) {
										suffix = " entries";
										max = info.ItemCount.Value;
										n = entries.Count;
									} else {
										suffix = " bytes";
										// This won't be perfect due to buffering, but it doesn't have to be.
										max = (int)compressedSize;
										n = (int)fileStream.Position;
									}

									percentage = 100 * n / max;
									if (percentage > lastPercentage)
										OnProgressChanged(string.Format("{0}% completed ({1} of {2}{3})", percentage, n, max, suffix), percentage);
									lastPercentage = percentage;

									phrase = bits[1].Normalize(); // Set phrase for next entry
								} else {
									break;
								}
							}
						}
					}
				}

				// This sort is needed because of the {hw} tag which can change the entry's headword.
				entries.Sort((a, b) => a.Phrase.CompareTo(b.Phrase));
				var section = new SimpleDictionary.Section(entries, true, null);
				foreach (var e in entries)
					e.Tag = new EntryTag(section, null);
				return section;
			}

			public event EventHandler<ProgressMessageEventArgs> ProgressChanged;

			// Signal the import thread to cancel. It returns before the thread is canceled,
			// so the import thread may live on for a short while, but that shouldn't prove
			// a problem.
			public void Cancel() {
				Interlocked.Increment(ref shouldCancel);
			}

			#region Private Implementation
			private void OnProgressChanged(string message, int? percent) {
				EventHandler<ProgressMessageEventArgs> temp = ProgressChanged;
				if (temp != null)
					temp(this, new ProgressMessageEventArgs(message, percent));
			}

			private string ReadToNewLineOrNull(StreamReader sr, out char delim) {
				StringBuilder sb = new StringBuilder();
				while (true) {
					int c = sr.Read();
					if (c == 0 || c == '\n' || c == -1) {
						delim = (char)c;
						return sb.ToString();
					}
					sb.Append((char)c);
				}
			}

			/// <summary>
			/// Reads the dictionary info, and positions the reader after the last entry in the stream.
			/// </summary>
			/// <param name="reader">StreamReader from which to read the dictionary information</param>
			/// <returns>A SectionInfo containing data that could be found in file.</returns>
			private SectionInfo ReadInfoFromStream(StreamReader reader) {
				SectionInfo info = new SectionInfo();

				while (true) {
					char delim;
					string line;
					try {
						line = ReadToNewLineOrNull(reader, out delim);
					} catch (InvalidDataException) {
						throw new ImportException("The file is not a Zbedic dictionary.");
					}

					string[] bits = line.Split(new char[] { '=' }, 2);
					try {
						if (bits.Length > 1) {
							switch (bits[0]) {
								case "builddate":
									info.Date = DateTime.Parse(bits[1]);
									break;
								case "copyright":
									info.Copyright = bits[1];
									break;
								case "id":
									info.Name = bits[1];
									break;
								case "dict-size":
									info.Size = int.Parse(bits[1]);
									break;
								case "maintainer":
									info.Maintainer = bits[1];
									break;
								case "items":
									info.ItemCount = int.Parse(bits[1]);
									break;
							}
						}
					} catch (ArgumentException) {
					} catch (FormatException) {
					} catch (OverflowException) {
					}

					if (delim != '\n')
						break;
				}

				return info;
			}

			// Returns the text that should be inserted in the element's place.
			delegate string SubElementHandler(string tag);

			// Returns null if the enumerator was at the end of the enumerable.
			// tagName is set if readStartTag is true.
			string Parse(StringEnumerator e, SubElementHandler subElementHandler, bool readStartTag, string expectedEndTag, out string outTagName) {
				if (!readStartTag && expectedEndTag == null) {
					throw new ArgumentException("readStartTag is false and expectedEndTag is null.");
				}

				outTagName = null;
				if (readStartTag) {
					// Skip to opening brace (there could be space between two top-level tags!)
					// Or there could be none, in which case it's no problem, there just aren't any
					// more top-level tags.
					while (true) {
						if (!e.Valid)
							return null;
						if (e.Current == '{')
							break;
						e.MoveNext();
					}

					StringBuilder tag = new StringBuilder();

					bool closingBraceFound = false;
					while (e.MoveNext()) {
						if (e.Current == '}') {
							closingBraceFound = true;
							e.MoveNext();
							break;
						}
						tag.Append(e.Current);
					}

					if (!closingBraceFound)
						throw new InvalidDataException("Expected: }, reached end of entry");

					outTagName = tag.ToString();
					expectedEndTag = "/" + outTagName;
				}

				char last = default(char);
				bool inTag = false;
				StringBuilder
					textContent = new StringBuilder(),
					tagName = new StringBuilder();

				if (e.Current == '}')
					throw new InvalidDataException("Unexpected '}' in entry (no corresponding '{').");

				while (true) {
					char c = e.Current;
					switch (last == '\\' ? '\0' : c) { //Use default case if last was backslash
						case '{':
							inTag = true;
							break;
						case '}':
							if (tagName.Length > 0 && tagName[0] == '/') {
								// This is a common error, it seems, and might be worth working around.
								// Won't work yet. We need to be able to rewind to the {/ss} for the calling Parse.
								//if (tagName.ToString() == "/ss" && expectedEndTag == "/ex")
								//	return textContent.ToString();

								if (tagName.ToString() != expectedEndTag)
									throw new InvalidDataException("Expected: {" + expectedEndTag + "}, Found: {" + tagName + "}");



								// Don't forget to consume the closing brace!
								e.MoveNext();

								return textContent.ToString();
							}

							// In the days of old, this used to be a common occurance meaning that the parser
							// was broken. However, it is more stable now, and some dictionaries do seem to 
							// contain this error.
							if (tagName.Length == 0)
								throw new InvalidDataException("Unexpected '}', no matching '{'.");

							if (!e.MoveNext())
								throw new InvalidDataException("Expected {/" + tagName.ToString() + ", found end of entry");

							// subElementHandler is responsible for reading the {/tag} (or not, in
							// the case of {hw/}, {img/} and {br/}
							string replacement = subElementHandler(tagName.ToString());
							if (!string.IsNullOrEmpty(replacement))
								textContent.Append(replacement);
							tagName.Length = 0;
							inTag = false;

							// The child element met an end of stream. No problem, though.
							if (!e.Valid)
								return textContent.ToString();
							continue; // Skip the MoveNext() call, subElementHandler did it
						case '\\':
							break;
						default:
							(inTag ? tagName : textContent).Append(c);
							break;
					}
					last = c;

					if (!e.MoveNext())
						break;
				}

				// Some dictionaries seem to expect to be allowed to do "{s}{ss}translation".
				// I'm fine with that, I guess.
				//throw new InvalidDataException("Expected: {" + expectedEndTag + "}, reached end of entry");
				return textContent.ToString();
			}

			// Parses an element that is known not to contain sub-elements (significant structural ones, anyway).
			// If a sub-element is encountered, its text is processed in the same manner.
			// The headWord needs to be known in case {hw/} is encountered.
			// (Though perhaps we should just replace it with "~"...)
			string ParseStringElement(StringEnumerator e, string tag, string headWord) {
				string unused;
				if (tag == "hw/")
					return headWord;
				if (tag == "br/")
					return "\n";
				if (tag.StartsWith("img ")) {
					return null;
				}

				return Parse(e, nestedTag => {
					return ParseStringElement(e, nestedTag, headWord);
				}, false, "/" + tag, out unused).Normalize();
			}

			// This is a subsense in bedic terminology. It can contain {sa}, {hw}, {ex}, {ct},
			// all of which are expected to contain only text, but we don't even need them anyway (yet).
			private IList<Translation> ParseTranslation(StringEnumerator e, string headWord) {
				string unused;
				string str = Parse(e, tag => {
					if (tag == "hw/")
						return headWord;
					if (tag == "br/")
						return "\n";
					if (tag == "hw") {
						Debug.Print("{hw} setter tag not supported yet");
						ParseStringElement(e, tag, headWord);
					}
					if (tag == "em" || tag == "de")
						return ParseStringElement(e, tag, headWord);
					ParseStringElement(e, tag, headWord);
					return null;
				}, false, "/ss", out unused);

				if (str == null)
					throw new InvalidDataException("Expected {/ss}, reached end of entry.");

				// This bit should be optional.
				// First, try ';', because it generally overrides ','. Thus, on the off chance that the 
				// translations contain commas, the result will be better. Failing that, let's try commas 
				// from other languages.
				List<Translation> list = new List<Translation>();
				string[] parts = str.Split(';');
				if (parts.Length == 1)
					parts = str.Split(',');
				if (parts.Length == 1) {
					char[] foreignCommas = new char[] { '\u060C' };
					parts = str.Split(foreignCommas);
				}

				foreach (string s in parts) {
					string trimmed = s.Trim();
					if (trimmed.Length == 0)
						continue;
					list.Add(new Translation(trimmed.Normalize()));
				}

				return list;
			}

			// Better than the default String enumerator because it allows checking to see
			// if the current enumerator is valid rather than passing it around as a method 
			// parameter and because it allows you to look at what will be enumerated next.
			sealed class StringEnumerator : IEnumerator<char> {
				string str;
				int i;
				bool valid;

				public StringEnumerator(string str) {
					this.str = str;
					Reset();
				}

				public bool Valid { get { return valid; } }

				/// <summary>A look at what will be enumerated next, (for debugging purposes).</summary>
				public string LookAhead {
					get {
						if (!valid)
							return null;
						return str.Substring(i, 15);
					}
				}

				public char Current {
					get {
						if (!valid)
							throw new InvalidOperationException("The enumeration has finished or hasn't started.");
						return str[i];
					}
				}

				// The class is sealed, no worries.
				public void Dispose() {
				}

				object System.Collections.IEnumerator.Current {
					get {
						if (!valid)
							throw new InvalidOperationException("The enumeration has finished or hasn't started.");
						return str[i];
					}
				}

				public bool MoveNext() {
					i++;
					return valid = i < str.Length;
				}

				public void Reset() {
					i = -1;
					valid = false;
				}
			}

			List<Entry> ParseSenses(string definitions, string headWord) {
				List<Entry> senses = new List<Entry>();
				StringEnumerator e = new StringEnumerator(definitions);
				e.MoveNext();
				while (true) {
					Entry entry = new Entry(headWord, new List<Translation>());
					string topLevelTag = null;
					string s = Parse(e, tag => {
						if (tag == "pr") {
							ParseStringElement(e, tag, headWord);
							// Set sense pronunciation
						} else if (tag == "ps") {
							ParseStringElement(e, tag, headWord);
							// Set sense part of speech
						} else if (tag == "ss") {
							foreach (Translation tr in ParseTranslation(e, headWord)) {
								// Strings have been trimmed and normalized by ParseTranslation.
								if (!entry.Translations.Contains(tr))
									entry.Translations.Add(tr);
							}
							return null;
						} else {
							ParseStringElement(e, tag, headWord);
						}
						return null;
					}, true, null, out topLevelTag);

					// Check if the enumeration was finished.
					if (s == null)
						break;

					if (topLevelTag == "s") {
						// Combine senses when they're not substantially different.
						// Senses with the same headword, part of speech and pronunciation are considered the same.
						// TODO: update when Pr/PoS added to Entry
						bool merged = false;
						foreach (Entry other in senses) {
							if (other.Phrase == entry.Phrase) {
								merged = true;
								foreach (Translation tr in entry.Translations)
									other.Translations.Add(tr);
								break;
							}
						}

						if (!merged)
							senses.Add(entry);
					} else if (topLevelTag == "pr") {
						// Set sense pronunciation
					} else if (topLevelTag == "ps") {
						// Why would this be at the top level?
					}
				}
				return senses;
			}
			#endregion

			#region Dispose
			public void Dispose() {
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing) {
				if (disposing) {
				}
			}
			#endregion
		}
	}
}
