using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Net;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Szotar.WindowsForms.Importing.DictionaryImporting {
	/// <summary>Imports a dictionary from two separate parts, each representing a different
	/// half of the dictionary.</summary>
	[Importer("Default importer", typeof(IBilingualDictionary))]
	[ImporterDescription("Imports and joins two halves of a dictionary", "ImporterDescriptions", "DualSection")]
	public class DualSectionImporter : IImporter<IBilingualDictionary> {
		AsyncOperation operation;
		protected IDictionarySectionImporter first, second;
		protected string firstPath, secondPath;
		protected bool shouldGenerateSecondHalf;
		protected long state = 0; //0 - Ready
		                          //1 - Importing
		                          //2 - Cancelled?

		/// <summary>
		/// Sets the two importers used to import the two sections of the dictionary.
		/// </summary>
		public void SetImporters(IDictionarySectionImporter first, string firstPath,
			IDictionarySectionImporter second, string secondPath, bool generateSecondHalf)
		{
			if (this.first != null || this.second != null)
				throw new InvalidOperationException();

			this.shouldGenerateSecondHalf = generateSecondHalf && (second == null);

			this.first = first;
			this.firstPath = firstPath;
			this.second = second;
			this.secondPath = secondPath;
		}

		public IImporterUI<IBilingualDictionary> CreateUI() {
			return new Controls.DualImporterUI(this);
		}

		/// <summary>Begins the import with the first importer.</summary>
		public void BeginImport() {
			if (first == null)
				throw new InvalidOperationException();
			if (Interlocked.Read(ref state) != 0)
				throw new InvalidOperationException();

			operation = AsyncOperationManager.CreateOperation(this);

			Thread workerThread = new Thread(new ThreadStart(this.WorkerThread));
			workerThread.Start();
		}

		private void WorkerThread() {
			if(Interlocked.Exchange(ref state, 1L) == 0) {
				try {
					SimpleDictionary.Section firstResult;
					try {
						WireEvents(first);
						firstResult = first.Import(firstPath);
					} finally {
						UnwireEvents(first);
					}

					if (Interlocked.Read(ref state) == 2)
						throw new OperationCanceledException();

					SimpleDictionary.Section secondResult = null;
					if (second != null) {
						try {
							WireEvents(second);
							secondResult = second.Import(secondPath);
						} finally {
							UnwireEvents(second);
						}
					}

					if (secondResult == null)
						secondResult = new SimpleDictionary.Section(new List<Entry>(), null);

					Finish(firstResult, secondResult);
				} catch (OperationCanceledException ex) {
					OnCompleted(null, ex, true, null);
				} catch (NotSupportedException ex) {
					OnCompleted(null, ex, false, null);
				} catch (InvalidOperationException ex) {
					OnCompleted(null, ex, false, null);
				} catch (ImportException ex) {
					OnCompleted(null, ex, false, null);
				} finally {
					//Set the state back to 0 (Ready)
					//Don't need to check the return value.
					Interlocked.Exchange(ref state, 0L);
				}
			} else {
				//Already importing!
				OnCompleted(null, new InvalidOperationException(), false, null);
			}
		}

		void ImporterProgressChanged(object sender, ProgressMessageEventArgs e) {
			CallOnMainThread(new SendOrPostCallback(delegate(object arg) {
				EventHandler<ProgressMessageEventArgs> temp = ProgressChanged;

				//Set the percentage to null, for now.
				//TODO: Allow localization
				ProgressMessageEventArgs args = new ProgressMessageEventArgs(
					string.Format(CultureInfo.CurrentUICulture, "(Part {1}): {0}", e.Message, sender == first ? 1 : 2), null);

				if (temp != null)
					temp(this, args);
			}));
		}

		private void Finish(IDictionarySection firstSection, IDictionarySection secondSection) {
			if (shouldGenerateSecondHalf)
				secondSection = GenerateSecondHalf(firstSection);

			SimpleDictionary dict = new SimpleDictionary(firstSection, secondSection);

			//It only has to be a guess, because the user can override it.
			dict.Name = dict.ForwardsSection.Name;

			OnCompleted(dict, null, false, null);
		}

		#region Implementation details
		private void OnCompleted (IBilingualDictionary result, Exception exception, bool cancelled, object state) {
			operation.PostOperationCompleted(new System.Threading.SendOrPostCallback(delegate(object postState) {
				EventHandler<ImportCompletedEventArgs<IBilingualDictionary>> h = Completed;
				if (h != null)
					h(this, new ImportCompletedEventArgs<IBilingualDictionary>(result, exception, cancelled, state));
			}), null);
		}

		private void OnProgressChanged (string message, int? percent) {
			operation.Post(new System.Threading.SendOrPostCallback(delegate(object postState) {
				EventHandler<ProgressMessageEventArgs> h = ProgressChanged;
				if (h != null)
					h(this, new ProgressMessageEventArgs(message, percent));
			}), null);
		}

		private void CallOnMainThread(SendOrPostCallback target) {
			operation.Post(target, null);
		}

		private void WireEvents(IDictionarySectionImporter importer) {
			importer.ProgressChanged += new EventHandler<ProgressMessageEventArgs>(ImporterProgressChanged);
		}

		private void UnwireEvents(IDictionarySectionImporter importer) {
			importer.ProgressChanged -= new EventHandler<ProgressMessageEventArgs>(ImporterProgressChanged);
		}
		#endregion

		public void Cancel() {
			if (Interlocked.CompareExchange(ref state, 1, 2) != 1)
				throw new InvalidOperationException(Resources.Errors.ImportHadNotBegun);

			if (first != null) {
				first.Cancel();
				first.Dispose();
				first = null;

				//The second one hasn't started yet, so no need to dispose it, and it
				//cannot start importing because state is now set to 2.
				if (second != null) {
					second.Dispose();
					second = null;
				}
			} else if (second != null) {
				second.Cancel();
				second.Dispose();
			}

			//We shouldn't get here, but it's not really an error.
			////Wait, what?
		}

		protected SimpleDictionary.Section GenerateSecondHalf(IDictionarySection firstSection) {
			Dictionary<string, List<string>> entries = new Dictionary<string,List<string>>();

			StringPool pool = new StringPool();

			//Normalize all translations (and entries, for good measure).
			//Also, pool them so that the "Contains" method of the List<string> works.
			foreach (Entry entry in firstSection) {
				entry.Phrase = pool.Pool(entry.Phrase.Normalize());
				foreach (Translation t in entry.Translations) {
					t.Value = pool.Pool(t.Value.Normalize());
				}
			}

			//Perform the actual reversion.
			foreach (Entry entry in firstSection) {
				foreach (Translation t in entry.Translations) {
					List<string> reverse;
					if (entries.TryGetValue(t.Value, out reverse)) {
						if (!reverse.Contains(entry.Phrase))
							reverse.Add(entry.Phrase);
					} else {
						reverse = new List<string>();
						entries.Add(t.Value, reverse);
						if (!reverse.Contains(entry.Phrase))
							reverse.Add(entry.Phrase);
					}
				}
			}

			//Convert it into a list.
			List<Entry> list = new List<Entry>();
			foreach (KeyValuePair<string, List<string>> kv in entries) {
				Entry e = new Entry(kv.Key, new List<Translation>());
				foreach (string t in kv.Value)
					e.Translations.Add(new Translation(t));
				list.Add(e);
			}

			//Sort the list of entries and the list of translations in each entry.
			list.Sort((a, b) => (a.Phrase.CompareTo(b.Phrase)));

			//Sort each entry. (Is this really necessary/useful?)
			foreach (Entry entry in list)
				((List<Translation>)entry.Translations).Sort((a, b) => a.Value.CompareTo(b.Value));
			SimpleDictionary.Section section = new SimpleDictionary.Section(list, null);

			//Try to guess at a name for this side of the dictionary.
			//This could be a bit cleverer, probably...
			foreach (char delim in new char[] { '\u21d4', '\u2194', '\u2014', '-' }) {
				string[] bits = firstSection.Name.Split(new char[] { delim }, 2);
				if (bits.Length == 2) {
					section.Name = bits[1] + delim + bits[0];
					break;
				}
			}

			return section;
		}

		public event EventHandler<ImportCompletedEventArgs<IBilingualDictionary>> Completed;
		public event EventHandler<ProgressMessageEventArgs> ProgressChanged;

		#region Dispose
		public void Dispose() {
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if(first != null)
					first.Dispose();
				if (second != null)
					second.Dispose();
			}
		}

		~DualSectionImporter() {
			Dispose(false);
		}
		#endregion
	}

	public class SectionInfo {
		public string Name { get; set; }
		public DateTime Date { get; set; }
		public string Maintainer { get; set; }
		public int? ItemCount { get; set; }
		public string Copyright { get; set; }

		/// <summary>
		/// Size in bytes
		/// </summary>
		public int Size { get; set; }
	}

	/// <summary>
	/// Imports a dictionary section. An importer needs only to support a single import at once.
	/// </summary>
	public interface IDictionarySectionImporter : IDisposable {
		SectionInfo GetInfo(string path);

		/// <summary>Imports a dictionary section from a file. This function, if called by DualSectionImporter, will
		/// be called on a separate thread. If the operation is cancelled, this method should throw an OperationCanceledException.
		/// </summary>
		/// <param name="path">File from which to import the dictionary.</param>
		/// <returns>A section of the dictionary as a SimpleDictionary.Section. </returns>
		/// <exception cref="System.OperationCanceledException"/>
		SimpleDictionary.Section Import(string path);

		event EventHandler<ProgressMessageEventArgs> ProgressChanged;

		/// <summary>
		/// This method should either block until the operation has been cancelled, or throw an exception.
		/// A NotSupportedException should be thrown if the importer does not support cancellation.
		/// </summary>
		void Cancel();
	}

	namespace Zbedic {
		using GZipStream = System.IO.Compression.GZipStream;
		using System.IO;
		using System.Threading;

		[Importer("Zbedic", typeof(IDictionarySection))]
		[ImporterDescription("Zbedic Importer", "ImporterDescriptions", "Zbedic")]
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

			public SimpleDictionary.Section Import(string path) {
				List<Entry> entries = new List<Entry>();

				SectionInfo info;

				using (Stream fileStream = File.OpenRead(path)) {
					using (GZipStream stream = new GZipStream(fileStream, System.IO.Compression.CompressionMode.Decompress, false)) {
						using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
							info = ReadInfoFromStream(reader);
							if (reader.EndOfStream)
								throw new ImportException("The file has no entries or is not a Zbedic dictionary.");
							bool showProgress = info.ItemCount.HasValue;
							int percentage, lastPercentage = 0;

							string phrase = reader.ReadLine();

							while (true) {
								{
									if (Interlocked.CompareExchange(ref shouldCancel, 0L, 1L) > 0)
										throw new OperationCanceledException();
								}

								string line = reader.ReadLine();
								string[] bits = line.Split(new char[] { '\0' }, 2, StringSplitOptions.RemoveEmptyEntries);
								List<Translation> translations = ParseTranslations(bits[0]);
								if (bits.Length > 1) {
									entries.Add(new Entry(phrase.Normalize(), translations));

									if (showProgress) {
										percentage = 100 * entries.Count / info.ItemCount.Value;
										if (percentage > lastPercentage)
											OnProgressChanged(string.Format("{0}% completed ({1} of {2})", percentage, entries.Count, info.ItemCount.Value), percentage);
										lastPercentage = percentage;
									}

									phrase = bits[1]; //Set phrase for next entry
								} else {
									break;
								}
							}
						}
					}
				}

				entries.Sort((a, b) => a.Phrase.CompareTo(b.Phrase));
				SimpleDictionary.Section section = new SimpleDictionary.Section(entries, null);
				section.Name = info.Name;
				return section;
			}

			public event EventHandler<ProgressMessageEventArgs> ProgressChanged;

			//Signal the import thread to cancel. It returns before the thread is canceled,
			//so the import thread may live on for a short while, but that shouldn't prove
			//a problem.
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

			private List<Translation> ParseTranslations(string str) {
				List<Translation> translations = new List<Translation>();
				StringBuilder current = new StringBuilder();

				for (int i = 0; i < str.Length; ++i) {
					char c = str[i];
					if (c == '{') {
						StringBuilder tag = new StringBuilder();
						while (i < str.Length && str[i] != '}') {
							tag.Append(str[i]);
							++i;
						}

						if (tag.ToString() == "{/ss") {
							translations.Add(new Translation(current.ToString().Trim()));
							current.Length = 0;
						}
					} else if (c == ',') {
						translations.Add(new Translation(current.ToString().Trim()));
						current.Length = 0;
					} else {
						current.Append(c);
					}
				}

				//The return point should be inside the for loop, but in malformed cases,
				//we might end up here.
				return translations;
			}
			#endregion

			#region Dispose
			public void Dispose() {
				Dispose(true);

				//Just in case a derived class implements a finalizer.
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
