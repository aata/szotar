using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Szotar {
	public abstract class WordList : IBindingList, IList<WordListEntry>, IDisposable, INotifyPropertyChanged {
		public WordList() {
			UndoList = new UndoList();
		}

		public UndoList UndoList { get; private set; }

		#region Unneeded or redundant crap
		bool IBindingList.AllowEdit { get { return true; } }
		bool IBindingList.AllowNew { get { return false; } }
		bool IBindingList.AllowRemove { get { return true; } }
		bool IBindingList.IsSorted { get { return false; } }
		bool IBindingList.SupportsChangeNotification { get { return true; } }
		bool IBindingList.SupportsSearching { get { return false; } }
		bool IBindingList.SupportsSorting { get { return false; } }
		ListSortDirection IBindingList.SortDirection { get { throw new NotSupportedException(); } }
		PropertyDescriptor IBindingList.SortProperty { get { throw new NotSupportedException(); } }
		void IBindingList.AddIndex(PropertyDescriptor property) { throw new NotSupportedException(); }
		void IBindingList.RemoveIndex(PropertyDescriptor property) { throw new NotSupportedException(); }
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) { throw new NotSupportedException(); }
		void IBindingList.RemoveSort() { throw new NotSupportedException(); }
		int IBindingList.Find(PropertyDescriptor property, object key) { throw new NotSupportedException(); }
		object IBindingList.AddNew() { throw new NotSupportedException(); }
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return false; } }
		object IList.this[int index] {
			get { return this[index]; }
			set { this[index] = (WordListEntry)value; }
		}
		System.Collections.IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		int IList.Add(object value) { Add((WordListEntry)value); return Count - 1; }
		void IList.Clear() { Clear(); }
		bool IList.Contains(object value) { return Contains((WordListEntry)value); }
		int IList.IndexOf(object value) { return IndexOf((WordListEntry)value); }
		void IList.Remove(object value) { Remove((WordListEntry)value); }
		void IList.RemoveAt(int index) { RemoveAt(index); }
		void IList.Insert(int index, object value) { Insert(index, (WordListEntry)value); }
		void ICollection.CopyTo(Array array, int index) { throw new NotSupportedException(); }
		object ICollection.SyncRoot { get { return null; } }
		bool ICollection.IsSynchronized { get { return false; } }
		int ICollection.Count { get { return Count; } }
		#endregion

		public abstract WordListEntry this[int index] { get; set; }
		public abstract int IndexOf(WordListEntry item);
		public abstract void Insert(int index, WordListEntry item);
		public abstract void RemoveAt(int index);
		public abstract int Count { get; }
		public abstract bool IsReadOnly { get; }
		public abstract void Add(WordListEntry item);
		public abstract void Clear();
		public abstract bool Contains(WordListEntry item);
		public abstract void CopyTo(WordListEntry[] array, int arrayIndex);
		public abstract bool Remove(WordListEntry item);
		public abstract IEnumerator<WordListEntry> GetEnumerator();

		//Specific to WordList
		public abstract void RemoveAt(IEnumerable<int> indices);
		public abstract void SwapRows(IEnumerable<int> indices);

		public event ListChangedEventHandler ListChanged;
		protected void RaiseListChanged(ListChangedEventArgs eventArgs) {
			var handler = ListChanged;
			if (handler != null)
				handler(this, eventArgs);
		}

		public enum EntryProperty {
			Phrase,
			Translation,
			TimesTried,
			TimesFailed
		}

		public abstract T GetProperty<T>(WordListEntry entry, EntryProperty property);
		public abstract void SetProperty(WordListEntry entry, EntryProperty property, object value);

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
		}

		public abstract long? ID { get; }
		public abstract string Name { get; set; }
		public abstract string Author { get; set; }
		public abstract string Language { get; set; }
		public abstract string Url { get; set; }
		public abstract DateTime? Date { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public abstract void DeleteWordList();
		public abstract void Insert(int index, IList<WordListEntry> range);

		public event EventHandler ListDeleted;
		internal void RaiseDeleted() {
			var handler = ListDeleted;
			if (handler != null)
				handler(this, new EventArgs());
		}

		//This is getting to be quite a complex interface!
		public abstract void Undo();
		public abstract void Redo();

		public void GetListInfo() {
			throw new NotImplementedException();
		}
	}

	public class WordListEntry : System.ComponentModel.INotifyPropertyChanged {
		WordList owner;
		string phrase;
		string translation;
		long tried, failed;

		public WordListEntry(WordList owner) {
			this.owner = owner;
			phrase = translation = string.Empty;
		}

		public WordListEntry(WordList owner, string phrase, string translation, long tried, long failed) {
			this.owner = owner;
			this.phrase = phrase;
			this.translation = translation;
			this.tried = tried;
			this.failed = failed;

			//This is really the wrong place to put this, but it's better to have it not break than break.
			if (phrase == null)
				phrase = string.Empty;
			if (translation == null)
				translation = string.Empty;
		}

		public void AddTo(WordList owner, int index) {
			Owner = owner;
			owner.Insert(index, this);
		}

		public string Phrase {
			get { return phrase; }
			set {
				if (value == null)
					throw new ArgumentNullException();

				SetPhrase(value);
				if(owner != null)
					owner.SetProperty(this, WordList.EntryProperty.Phrase, value);
			}
		}

		public void SetPhrase(string value) {
			if (value == null)
				throw new ArgumentNullException();

			phrase = value;
			RaisePropertyChanged("Phrase");
		}

		public string Translation {
			get { return translation; }
			set {
				if (value == null) 
					throw new ArgumentNullException();

				SetTranslation(value);
				if(owner != null)
					owner.SetProperty(this, WordList.EntryProperty.Translation, value);
			}
		}

		public void SetTranslation(string value) {
			if (value == null)
				throw new ArgumentNullException();

			translation = value;
			RaisePropertyChanged("Translation");
		}

		public long TimesTried {
			get { return tried; }
			set {
				SetTimesTried(value);
				if (owner != null)
					owner.SetProperty(this, WordList.EntryProperty.TimesTried, value);
			}
		}

		public void SetTimesTried(long value) {
			if (value < 0)
				throw new ArgumentOutOfRangeException();

			tried = value;
			RaisePropertyChanged("TimesTried");
		}

		public long TimesFailed {
			get { return failed; }
			set {
				SetTimesFailed(value);
				if (owner != null)
					owner.SetProperty(this, WordList.EntryProperty.TimesFailed, value);
			}
		}

		public void SetTimesFailed(long value) {
			if (value < 0)
				throw new ArgumentOutOfRangeException();

			failed = value;
			RaisePropertyChanged("TimesFailed");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string property) {
			var handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(property));
			}
		}

		//You know, I don't think this type parameter is particularly useful.
		internal object GetProperty(WordList.EntryProperty property) {
			switch (property) {
				case WordList.EntryProperty.Phrase:
					return phrase;
				case WordList.EntryProperty.Translation:
					return translation;
				case WordList.EntryProperty.TimesTried:
					return tried;
				case WordList.EntryProperty.TimesFailed:
					return failed;
			}
			throw new ArgumentException("property");
		}

		public void SetProperty(WordList.EntryProperty property, object newValue) {
			switch (property) {
				case WordList.EntryProperty.Phrase:
					SetPhrase((string)newValue);
					break;
				case WordList.EntryProperty.Translation:
					SetTranslation((string)newValue);
					break;
				case WordList.EntryProperty.TimesTried:
					SetTimesTried((long)newValue);
					break;
				case WordList.EntryProperty.TimesFailed:
					SetTimesFailed((long)newValue);
					break;
			}
			throw new ArgumentException("property");
		}

		public WordList Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				owner = value;
				Debug.Assert(value.IndexOf(this) == -1);
			}
		}
	}
}