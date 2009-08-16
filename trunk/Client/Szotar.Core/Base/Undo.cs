using System.Collections.Generic;

namespace Szotar {
	public interface ICommand {
		void Do();
		void Undo();
		void Redo();
		string Description { get; }
	}

	public class UndoList {
		// The newest undo items are at the end of the list.
		List<ICommand> undoItems = new List<ICommand>();
		// The newest redo items are at the end of the list.
		List<ICommand> redoItems = new List<ICommand>();

		public void Undo(int count) {
			if (count > UndoItemCount)
				throw new System.ArgumentOutOfRangeException();

			for (int i = 0; i < count; ++i) {
				var item = undoItems[undoItems.Count - 1];
				item.Undo();
				redoItems.Add(item);
				undoItems.RemoveAt(undoItems.Count - 1);
			}
		}

		public void Redo(int count) {
			if (count > RedoItemCount)
				throw new System.ArgumentOutOfRangeException();

			for (int i = 0; i < count; ++i) {
				var item = redoItems[redoItems.Count - 1];
				item.Redo();
				undoItems.Add(item);
				redoItems.RemoveAt(redoItems.Count - 1);
			}
		}

		public int UndoItemCount { get { return undoItems.Count; } }
		public int RedoItemCount { get { return redoItems.Count; } }

		public IEnumerable<string> UndoItemDescriptions {
			get {
				for (int i = undoItems.Count - 1; i >= 0; ++i)
					yield return undoItems[i].Description;
			}
		}

		public IEnumerable<string> RedoItemDescriptions {
			get {
				for (int i = redoItems.Count - 1; i >= 0; ++i)
					yield return redoItems[i].Description;
			}
		}

		public void Do(ICommand item) {
			item.Do();
			undoItems.Add(item);
			redoItems.Clear();
		}

		public void Clear() {
			undoItems.Clear();
			redoItems.Clear();
		}
	}
}