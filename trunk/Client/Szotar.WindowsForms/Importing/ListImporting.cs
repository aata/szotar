using System;
using System.Text;
using System.Collections.Generic;

namespace Szotar.WindowsForms.Importing.WordListImporting {
	using System.Net;
	using System.Globalization;
	using System.IO;
	using System.Xml;
	using System.ComponentModel;
	using System.Xml.XPath;

	[ImporterAttribute("Quizlet", typeof(WordList))]
	public class QuizletImporter : IImporter<WordList> {
		long? setID;
        System.Threading.CancellationTokenSource cts;
		bool disposed;

		public QuizletImporter()
			: this(null) 
        {
                cts = new System.Threading.CancellationTokenSource();
		}

		public QuizletImporter(long? setID) {
			this.setID = setID;
		}

		public long? Set {
			get { return setID; }
			set { setID = value; }
		}

		public IImporterUI<WordList> CreateUI() {
			return new Controls.QuizletSetSelector(this);
		}

		public void BeginImport() {
			if (disposed)
				throw new ObjectDisposedException("QuizletImporter");
			
            if (setID == null) {
                OnCompleted(null, new InvalidOperationException("Cannot begin quizlet import when Set is null"), false, null);
                return;
			}

			OnProgressChanged(string.Format(CultureInfo.CurrentUICulture, Resources.Quizlet.ContactingServer, "quizlet.com"), null);

            new QuizletAPI().GetSetInfo(
                setID.Value, 
                set => {
                    if (set.Terms == null) {
                        OnCompleted(null, new FormatException("JSON for set information did not contain the terms of the set"), false, null);
                        return;
                    }

                    var list = DataStore.Database.CreateSet(set.Title, set.Author, null, set.Uri.ToString(), DateTime.Now);
                    foreach (var pair in set.Terms)
                        list.Add(new WordListEntry(list, pair.Phrase, pair.Translation));
                    OnCompleted(list, null, false, null);
                }, 
                e => {
                    OnCompleted(null, e, e is OperationCanceledException, null);
                },
                cts.Token);
        }

		public void Cancel() {
            cts.Cancel();
		}

		public event EventHandler<ImportCompletedEventArgs<WordList>> Completed;

		private void OnCompleted(WordList result, Exception exception, bool cancelled, object state) {
			EventHandler<ImportCompletedEventArgs<WordList>> h = Completed;
			if (h != null)
				h(this, new ImportCompletedEventArgs<WordList>(result, exception, cancelled, state));
		}

		public event EventHandler<ProgressMessageEventArgs> ProgressChanged;

		private void OnProgressChanged(string message, int? percent) {
			EventHandler<ProgressMessageEventArgs> h = ProgressChanged;
			if (h != null)
				h(this, new ProgressMessageEventArgs(message, percent));
		}

		#region Dispose
		public void Dispose() {
			disposed = true;

			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (cts != null) {
					cts.Dispose();
                    cts = null;
				}
			}
		}

		~QuizletImporter() {
			Dispose(false);
		}
		#endregion
	}
}
