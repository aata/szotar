using System;
using System.Threading;
using System.Threading.Tasks;
using Szotar.Sqlite;

namespace Szotar.Quizlet {
    [ImporterAttribute("Quizlet", typeof(WordList))]
    [ImporterDescription("Import a set from Quizlet", "Quizlet")]
	public class QuizletImporter : IImporter<WordList> {
		long? setID;
		CancellationTokenSource cts;
		bool disposed;
	    readonly IStringTable stringTable;

		public QuizletImporter()
			: this(null) {
			cts = new CancellationTokenSource();
			stringTable = LocalizationProvider.Default.GetStringTable("Quizlet");
		}

		public QuizletImporter(long? setID) {
			this.setID = setID;
		}

		public long? Set {
			get { return setID; }
			set { setID = value; }
		}

		public async Task<WordList> BeginImportAsTask() {
			if (disposed)
				throw new ObjectDisposedException("QuizletImporter");

            if (setID == null)
                throw new InvalidOperationException("Cannot begin quizlet import when Set is null");

		    var api = new QuizletApi();
			OnProgressChanged(stringTable["ContactingServer"].FormatUI(api.ServiceUri.Host), null);
            
		    var set = await api.FetchSetInfo(setID.Value, cts.Token);
		    if (set.Terms == null)
		        throw new FormatException("JSON for set information did not contain the terms of the set");

		    var list = DataStore.Database.CreateSet(set.Title, set.Author, null, set.Uri.ToString(), DateTime.Now);
			foreach (var term in set.Terms)
				list.Add(new WordListEntry(list, term.Term, term.Definition));

		    return list;
		}

        public void BeginImport() {
            BeginImportAsTask().ContinueWith(t => OnCompleted(t.Result, t.Exception, t.IsCanceled, null));
        }

        public void Cancel() {
			cts.Cancel();
		}

        public event EventHandler<ProgressMessageEventArgs> ProgressChanged;
		public event EventHandler<ImportCompletedEventArgs<WordList>> Completed;

		private void OnCompleted(WordList result, Exception exception, bool cancelled, object state) {
			var h = Completed;
			if (h != null)
				h(this, new ImportCompletedEventArgs<WordList>(result, exception, cancelled, state));
		}

		private void OnProgressChanged(string message, int? percentage) {
			var h = ProgressChanged;
			if (h != null)
				h(this, new ProgressMessageEventArgs(message, percentage));
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
