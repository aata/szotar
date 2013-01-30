using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Szotar.Quizlet {
	public class SyncOperation {
		private bool begun;
		private long[] setIDs;

		public SyncOperation(IEnumerable<long> setIDs) {
			if (setIDs == null)
				throw new ArgumentNullException("setIDs");

			this.setIDs = setIDs.ToArray();
		}

		public Task Begin(CancellationToken cancel) {
			if (begun)
				throw new InvalidOperationException("The synchronisation is already in progress.");
			begun = true;

			var api = new QuizletApi();

			var sets = setIDs.Select(DataStore.Database.GetWordList).ToArray();
			if (sets.Any(s => s.SyncID == null))
				throw new InvalidOperationException("Cannot synchronise a list that has no SyncID.");

			return Task.WhenAll(from set in sets select SyncSet(api, set, cancel));
		}

		// TODO Sync tags
		private async Task SyncSet(IQuizletService api, WordList list, CancellationToken cancel) {
			Debug.Assert(list.SyncID != null, "list.SyncID != null");
			Debug.Assert(list.ID != null, "list.ID != null");

			if (list.SyncNeeded) {
				try {
					var info = await api.FetchSetInfo(list.SyncID.Value, cancel);
					
					// Find updates from Quizlet
					var missingLocally = (from qTerm in info.Terms
										  where list.All(e => !SameTerm(e.Phrase, e.Translation, qTerm.Term, qTerm.Definition))
										  select new TranslationPair(qTerm.Term, qTerm.Definition)).ToList();

					var missingRemotely = (from lTerm in list
										   where info.Terms.All(t => !SameTerm(lTerm.Phrase, lTerm.Translation, t.Term, t.Definition))
										   select new TranslationPair(lTerm.Phrase, lTerm.Translation)).ToList();

					var modifiedLocally = list.SyncNeeded;
					var modifiedRemotely = info.Modified > list.SyncDate || !list.SyncDate.HasValue;
						
					if (modifiedLocally && modifiedRemotely) {
						// Merge conflict!
					} else if (modifiedRemotely) {
						// Apply deletions from Quizlet
						for (int i = 0; i < missingRemotely.Count; i++) {
							var qd = missingRemotely[i];
							list.Remove(e => SameTerm(e.Phrase, e.Translation, qd.Phrase, qd.Translation));
						}

						// Apply insertions from Quizlet
						foreach (var qi in missingLocally)
							list.Add(new WordListEntry(list, qi.Phrase, qi.Translation));
					} else {
						// Just push a full update of the entries to Quizlet
						var set = GetSetModel(list);
						
						// Update properties that aren't stored locally
						await api.UpdateSet(set, cancel);
					}
				} catch (Exception e) {
					OnItemError(list, e);
					return;
				}
			}

			OnItemFinished(list);
		}

		private static SetModel GetSetModel(WordList list) {
			Debug.Assert(list.ID != null, "list.ID != null");

			Uri uri;
			Uri.TryCreate(list.Url, UriKind.Absolute, out uri);

			return new SetModel {
				Author = list.Author,
				SetID = list.ID.Value,
				Title = list.Name,
				Uri = uri,
				Terms = (from e in list
				         select new TermModel { Term = e.Phrase, Definition = e.Translation }).ToList()
			};
		}

		private static bool SameTerm(string phrase, string translation, string phrase2, string translation2) {
			phrase = phrase.Normalize();
			translation = translation.Normalize();
			phrase2 = phrase2.Normalize();
			translation2 = translation2.Normalize();
			return phrase == phrase2 && translation == translation2;
		}

#region Events

		public delegate void ItemFinishedEventHandler(SyncOperation sender, WordList set);
		public event ItemFinishedEventHandler ItemFinished;

		protected void OnItemFinished(WordList set) {
			var h = ItemFinished;
			if (h != null)
				SynchronizationContext.Current.Post(x => h(this, set), null);
		}

		public delegate void ItemErrorEventHandler(SyncOperation sender, WordList set, Exception exception);
		public event ItemErrorEventHandler ItemError;

		protected void OnItemError(WordList set, Exception exception) {
			var h = ItemError;
			if (h != null)
				SynchronizationContext.Current.Post(x => h(this, set, exception), null);
		}

		public delegate void MergeConflictEventHandler(SyncOperation sender, WordList set, MergeConflict conflict);
		public event MergeConflictEventHandler MergeConflict;

		protected void OnMergeConflict(WordList set, MergeConflict conflict) {
			var h = MergeConflict;
			if (h != null)
				SynchronizationContext.Current.Post(x => h(this, set, conflict), null);
		}
#endregion
	}

	public class MergeConflict {
		
	}
}