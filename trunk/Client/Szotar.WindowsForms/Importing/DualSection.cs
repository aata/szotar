using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Szotar.WindowsForms.Importing {
    /// <summary>Imports a dictionary from two separate parts, each representing a different
    /// half of the dictionary.</summary>
    [Importer("Default importer", typeof(IBilingualDictionary))]
    [ImporterDescription("Imports and joins two halves of a dictionary", "DualSection")]
    public class DualSectionImporter : IImporter<IBilingualDictionary> {
        AsyncOperation operation;
        protected IDictionarySectionImporter first, second;
        protected string firstPath, secondPath;
        protected bool shouldGenerateSecondHalf;
        // 0 = Ready, 1 = Importing, 2 = Cancelled?
        protected long state = 0;
        protected SectionInfo sectionInfo;

        /// <summary>
        /// Sets the two importers used to import the two sections of the dictionary.
        /// </summary>
        public void SetImporters(IDictionarySectionImporter first, string firstPath,
            IDictionarySectionImporter second, string secondPath, bool generateSecondHalf) {
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
            if (Interlocked.Exchange(ref state, 1L) == 0) {
                try {
                    SimpleDictionary.Section firstResult;
                    try {
                        WireEvents(first);
                        firstResult = first.Import(firstPath, out sectionInfo);
                    } finally {
                        UnwireEvents(first);
                    }

                    if (Interlocked.Read(ref state) == 2)
                        throw new OperationCanceledException();

                    SimpleDictionary.Section secondResult = null;
                    if (second != null) {
                        try {
                            WireEvents(second);
                            SectionInfo unused;
                            secondResult = second.Import(secondPath, out unused);
                        } finally {
                            UnwireEvents(second);
                        }
                    }

                    if (secondResult == null)
                        secondResult = new SimpleDictionary.Section(new List<Entry>(), true, null);

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
                    // Set the state back to 0 (Ready)
                    // Don't need to check the return value.
                    Interlocked.Exchange(ref state, 0L);
                }
            } else {
                // Already importing!
                OnCompleted(null, new InvalidOperationException(), false, null);
            }
        }

        void ImporterProgressChanged(object sender, ProgressMessageEventArgs e) {
            CallOnMainThread(new SendOrPostCallback(delegate(object arg) {
                EventHandler<ProgressMessageEventArgs> temp = ProgressChanged;

                // Set the percentage to null, for now.
                ProgressMessageEventArgs args = new ProgressMessageEventArgs(
                    string.Format(System.Globalization.CultureInfo.CurrentUICulture, "(Part {1}): {0}", e.Message, sender == first ? 1 : 2), e.Percentage);

                if (temp != null)
                    temp(this, args);
            }));
        }

        private void Finish(SimpleDictionary.Section firstSection, SimpleDictionary.Section secondSection) {
            if (shouldGenerateSecondHalf)
                secondSection = GenerateSecondHalf(firstSection);

            //var dict = new SimpleDictionary(firstSection, secondSection);
            var dict = SqliteDictionary.FromPath(System.IO.Path.GetTempFileName());
            try {
                dict.AddEntries(
                    System.Linq.Enumerable.ToArray(firstSection),
                    System.Linq.Enumerable.ToArray(secondSection),
                    delegate(int i, int n) {
                        if (Interlocked.Read(ref state) == 2)
                            throw new OperationCanceledException();
                        OnProgressChanged(string.Format("Writing file: {0} of {1}", i, n), i * 100 / (n != 0 ? n : 1));
                    });
            } catch (OperationCanceledException ex) {
                OnCompleted(null, ex, true, null);
                return;
            }

            // It only has to be a guess, because the user can override it.
            if (sectionInfo != null) {
                dict.Name = sectionInfo.Name;

                // Attempt to guess at the names of the languages.
                foreach (char delim in new char[] { '\u21d4', '\u2194', '\u2014', '-' }) {
                    string[] bits = dict.Name.Split(new char[] { delim }, 2);
                    if (bits.Length == 2) {
                        dict.FirstLanguage = bits[0];
                        dict.SecondLanguage = bits[1];
                        break;
                    }
                }
            }

            OnCompleted(dict, null, false, null);
        }

        #region Implementation details
        private void OnCompleted(IBilingualDictionary result, Exception exception, bool cancelled, object state) {
            operation.PostOperationCompleted(new System.Threading.SendOrPostCallback(delegate(object postState) {
                EventHandler<ImportCompletedEventArgs<IBilingualDictionary>> h = Completed;
                if (h != null)
                    h(this, new ImportCompletedEventArgs<IBilingualDictionary>(result, exception, cancelled, state));
            }), null);
        }

        private void OnProgressChanged(string message, int? percent) {
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
            if (importer != null) // Most likely because of Cancel()
                importer.ProgressChanged -= new EventHandler<ProgressMessageEventArgs>(ImporterProgressChanged);
        }
        #endregion

        public void Cancel() {
            if (Interlocked.CompareExchange(ref state, 2, 1) != 1)
                throw new InvalidOperationException(Resources.Errors.ImportHadNotBegun);

            if (first != null) {
                first.Cancel();
                first.Dispose();
                first = null;

                // The second one hasn't started yet, so no need to dispose it, and it
                // cannot start importing because state is now set to 2.
                if (second != null) {
                    second.Dispose();
                    second = null;
                }
            } else if (second != null) {
                second.Cancel();
                second.Dispose();
            }
        }

        protected SimpleDictionary.Section GenerateSecondHalf(IDictionarySection firstSection) {
            Dictionary<string, List<string>> entries = new Dictionary<string, List<string>>();

            StringPool pool = new StringPool();

            // Normalize all translations (and entries, for good measure).
            // Also, pool them so that the "Contains" method of the List<string> works.
            foreach (Entry entry in firstSection) {
                entry.Phrase = pool.Pool(entry.Phrase.Normalize());

                foreach (Translation t in entry.Translations) {
                    t.Value = pool.Pool(t.Value.Normalize());
                }
            }

            // Perform the actual reversion.
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

            // Convert it into a list.
            List<Entry> list = new List<Entry>();
            foreach (KeyValuePair<string, List<string>> kv in entries) {
                var translations = new List<Translation>();
                Entry e = new Entry(kv.Key, translations);

                foreach (string t in kv.Value)
                    translations.Add(new Translation(t));

                // Sort the list of translations in each entry.
                translations.Sort((a, b) => a.Value.CompareTo(b.Value));

                list.Add(e);
            }

            // Sort the list of entries.
            list.Sort((a, b) => (a.Phrase.CompareTo(b.Phrase)));

            SimpleDictionary.Section section = new SimpleDictionary.Section(list, true, null);
            foreach (var e in list)
                e.Tag = new EntryTag(section, null);
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
                if (first != null)
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
}