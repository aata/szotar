using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Szotar {
	public abstract class SyncWordList : IBindingList, IList<WordListEntry>, IDisposable {
		public SyncWordList() {
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

		public event ListChangedEventHandler ListChanged;
		protected void RaiseListChanged(ListChangedEventArgs eventArgs) {
			var handler = ListChanged;
			if (handler != null) {
				handler(this, eventArgs);
			}
		}

		public enum EntryProperty {
			Phrase,
			Translation,
			TimesTried,
			TimesFailed
		}

		public abstract T GetProperty<T>(WordListEntry entry, EntryProperty property);
		public abstract void SetProperty<T>(WordListEntry entry, EntryProperty property, T value);

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
		}
	}

	public class WordListEntry : System.ComponentModel.INotifyPropertyChanged {
		SyncWordList owner;
		string phrase;
		string translation;
		long tried, failed;

		public WordListEntry(SyncWordList owner) {
			this.owner = owner;
		}

		public WordListEntry(SyncWordList owner, string phrase, string translation, long tried, long failed) {
			this.owner = owner;
			this.phrase = phrase;
			this.translation = translation;
			this.tried = tried;
			this.failed = failed;
		}

		public string Phrase {
			get { return phrase; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				owner.SetProperty<string>(this, SyncWordList.EntryProperty.Phrase, value);
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
				owner.SetProperty<string>(this, SyncWordList.EntryProperty.Translation, value);
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
				owner.SetProperty<long>(this, SyncWordList.EntryProperty.TimesTried, value);
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
				owner.SetProperty<long>(this, SyncWordList.EntryProperty.TimesFailed, value);
			}
		}

		public void SetTimesFailed(long value) {
			if (value < 0)
				throw new ArgumentOutOfRangeException();

			failed = value;
			RaisePropertyChanged("TimesFailed");
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string property) {
			var handler = PropertyChanged;
			if (handler != null) {
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(property));
			}
		}

		public void SetProperty<T>(SyncWordList.EntryProperty property, T newValue) {
			switch (property) {
				case SyncWordList.EntryProperty.Phrase:
					SetPhrase((string)(object)newValue);
					break;
				case SyncWordList.EntryProperty.Translation:
					SetTranslation((string)(object)newValue);
					break;
				case SyncWordList.EntryProperty.TimesTried:
					SetTimesTried((long)(object)newValue);
					break;
				case SyncWordList.EntryProperty.TimesFailed:
					SetTimesFailed((long)(object)newValue);
					break;
			}
		}
	}
}