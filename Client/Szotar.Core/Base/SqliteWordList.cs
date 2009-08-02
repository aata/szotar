using System.Collections.Generic;
using System.Data.Common;
using CultureInfo = System.Globalization.CultureInfo;
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;

//SqliteWordList: an SQLite-backed word list. All changes made are immediately
//synchronised with the database, and can be undone or re-done. It also has an
//in-memory version, stored in a BindingList, which provides the ListChanged 
//events and fast read access.

//Despite being database-backed, this list is NOT thread-safe!

//The undo list is composed of a zipper list of ICommand objects which can be
//performed and undone. See Undo.cs for more information.

//The undo functionality is global to the list, NOT specific to each editor,
//so this can obviously cause problems if multiple consumers are editing the
//list at the same time. However, having separate undo lists doesn't really
//make much sense either, unless the undo lists were somehow able to update
//each other such that undoing an action in one list updates the other undo 
//list's state such that it is correct. Maybe this would be something to 
//explore at a later date, but I suspect that the provided workarounds will 
//suffice for now.

//The UI should ensure that only one UI element is providing editing services 
//for the word list, to avoid undo-related confusion.

//To work around this somewhat, the WordList class will provide for out-of-band
//updates that aren't stored in the undo list. This makes writing ICommand 
//objects more difficult as they should preserve out-of-band changes when they
//undone and then re-done. For example, inserting a WordListEntry, updating its 
//TimesFailed count out-of-band, then undoing the insertion and re-doing it 
//should re-insert the *edited* row, not the original row. Such cases are
//usually marked in the comments where they occur.

//Note: it should be an error to perform deletion or insertion commands out-of-band, 
//as such things could cause normal undo/redo functionality to fail.

//I don't normally use #region, but I think this code is clearer with it.
namespace Szotar.Sqlite {
	public class SqliteWordList : WordList {
		Worker worker;
		BindingList<WordListEntry> list;
		long? id;

		public SqliteDataStore DataStore { get; private set; }
		public override long? ID {
			get { return id; }
		}

		public SqliteWordList(SqliteDataStore store, long setID) {
			DataStore = store;
			id = setID;
			worker = new Worker(store, this);
			list = new BindingList<WordListEntry>(worker.GetAllEntries());

			list.ListChanged += new ListChangedEventHandler(list_ListChanged);
		}

		//Re-raise the ListChanged event with the SqliteWordList as the originator.
		//Using BindingList as the in-memory list makes this very easy.
		void list_ListChanged(object sender, ListChangedEventArgs e) {
			RaiseListChanged(e);
		}

		protected override void Dispose(bool disposing) {
			if (disposing)
				worker.Dispose();

			base.Dispose(disposing);
		}

		//Delete first then raise seems the more sensible option, given the name, but still...
		//What if something wants to use the list before it's deleted?
		public override void DeleteWordList() {
			DataStore.DeleteWordList(ID.Value);

			//RaiseDeleted() not needed. SqliteDataStore will do that for us, as an an extra safety 
			//measure for if SqliteDataStore's DeleteWordList is called without calling DeleteWordList 
			//on the WordList itself.
		}

		#region Metadata
		//Should assigning null to this really raise an exception?
		public override string Author {
			get { return (string)worker.GetWordListProperty("Author"); }
			set {
				if (value == null)
					throw new ArgumentNullException();
				worker.SetWordListProperty("Author", value);
				RaisePropertyChanged("Author");
			}
		}

		public override string Name {
			get { return (string)worker.GetWordListProperty("Name"); }
			set {
				if (value == null)
					throw new ArgumentNullException();
				worker.SetWordListProperty("Name", value);
				RaisePropertyChanged("Name");
			}
		}

		public override string Language {
			get { return (string)worker.GetWordListProperty("Language"); }
			set {
				if (value == null)
					throw new ArgumentNullException();
				worker.SetWordListProperty("Language", value);
				RaisePropertyChanged("Language");
			}
		}

		public override string Url {
			get { return (string)worker.GetWordListProperty("Url"); }
			set {
				if (value == null)
					throw new ArgumentNullException();
				worker.SetWordListProperty("Url", value);
				RaisePropertyChanged("Url");
			}
		}

		//Will probably want both a Created and Modified property in the future.
		public override System.DateTime? Date {
			get { return (DateTime?)worker.GetWordListProperty("Created"); }
			set {
				if (value == null)
					throw new ArgumentNullException();
				worker.SetWordListProperty("Created", value);
				RaisePropertyChanged("Date");
			}
		}
		#endregion

		public override T GetProperty<T>(WordListEntry entry, WordList.EntryProperty property) {
			int index = IndexOf(entry);
			Debug.Assert(index >= 0);
			return (T)worker.GetProperty(index, property.ToString());
		}

		/// <summary>Set a property of an entry in the database (but not in the in-memory list).</summary>
		public override void SetProperty(WordListEntry entry, WordList.EntryProperty property, object value) {
			int index = IndexOf(entry);
			Debug.Assert(index >= 0);
			worker.SetProperty(index, property.ToString(), value);
		}

		#region List implementation
		public override void Add(WordListEntry item) {
			Insert(Count, item);
		}

		public override void Insert(int index, WordListEntry item) {
			if (index > Count || index < 0)
				throw new ArgumentOutOfRangeException("index");
			UndoList.Do(new Insertion(this, index, item));
		}

		public override void Insert(int index, IList<WordListEntry> items) {
			if (index > Count || index < 0)
				throw new ArgumentOutOfRangeException("index");
			UndoList.Do(new MultipleInsertion(this, index, items));
		}

		public override bool Remove(WordListEntry item) {
			bool existed = list.Contains(item);
			UndoList.Do(new Deletion(this, IndexOf(item)));
			return existed;
		}

		public override void RemoveAt(int index) {
			if (index >= Count || index < 0)
				throw new ArgumentOutOfRangeException("index");
			UndoList.Do(new Deletion(this, index));
		}

		public override void RemoveAt(IEnumerable<int> indices) {
			UndoList.Do(new Deletion(this, indices));
		}

		public override void SwapRows(IEnumerable<int> indices)
		{
 			UndoList.Do(new SwapRowsCommand(this, indices));
		}

		public override void Clear() {
			UndoList.Do(new Deletion(this, EnumRange(0, list.Count - 1)));
		}

		protected IEnumerable<int> EnumRange(int from, int to) {
			while (from <= to)
				yield return from++;
		}

		public override bool Contains(WordListEntry item) {
			return list.Contains(item);
		}

		public override void CopyTo(WordListEntry[] array, int arrayIndex) {
			list.CopyTo(array, arrayIndex);
		}

		public override int Count {
			get { return list.Count; }
		}

		public override bool IsReadOnly {
			get { return false; }
		}

		public override WordListEntry this[int index] {
			get { return list[index]; }
			set { UndoList.Do(new SetItem(this, index, value));	}
		}

		public override int IndexOf(WordListEntry item) {
			return list.IndexOf(item);
		}

		public override IEnumerator<WordListEntry> GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion

		#region Worker
		//Worker does the dirty database work. It's nicer if it's separated from the rest of the list code.
		protected class Worker : SqliteObject {
			SqliteWordList list;

			//These are called particularly often, any performance enhancements are useful.
			DbCommand insertCommand, deleteCommand;
			DbParameter insertCommandSetID, insertCommandListPosition, insertCommandPhrase,
				insertCommandTranslation, deleteCommandSetID, deleteCommandListPosition;

			public Worker(SqliteDataStore store, SqliteWordList list)
				: base(store) {
				this.list = list;

				insertCommand = Connection.CreateCommand();
				insertCommand.CommandText = @"
					UPDATE VocabItems 
						SET ListPosition = ListPosition + 1	
						WHERE SetID = $SetID AND ListPosition >= $ListPosition;

					INSERT INTO VocabItems 
						(Phrase, Translation, SetID, ListPosition)
						VALUES ($Phrase, $Translation, $SetID, $ListPosition);";

				insertCommandSetID = insertCommand.CreateParameter();
				insertCommandSetID.ParameterName = "SetID";
				insertCommandSetID.Value = list.ID;
				insertCommand.Parameters.Add(insertCommandSetID);

				insertCommandListPosition = insertCommand.CreateParameter();
				insertCommandListPosition.ParameterName = "ListPosition";
				insertCommand.Parameters.Add(insertCommandListPosition);

				insertCommandPhrase = insertCommand.CreateParameter();
				insertCommandPhrase.ParameterName = "Phrase";
				insertCommand.Parameters.Add(insertCommandPhrase);

				insertCommandTranslation = insertCommand.CreateParameter();
				insertCommandTranslation.ParameterName = "Translation";
				insertCommand.Parameters.Add(insertCommandTranslation);

				deleteCommand = Connection.CreateCommand();
				deleteCommand.CommandText = @"
					DELETE From VocabItems 
						WHERE SetID = $SetID AND ListPosition = $ListPosition; 
					UPDATE VocabItems SET ListPosition = ListPosition - 1 
						WHERE SetId = $SetID AND ListPosition > $ListPosition";

				deleteCommandSetID = deleteCommand.CreateParameter();
				deleteCommandSetID.ParameterName = "SetID";
				deleteCommand.Parameters.Add(deleteCommandSetID);

				deleteCommandListPosition = deleteCommand.CreateParameter();
				deleteCommandListPosition.ParameterName = "ListPosition";
				deleteCommand.Parameters.Add(deleteCommandListPosition);
			}

			protected override void Dispose(bool disposing) {
				// Don't need to dispose of the parameters.
				insertCommand.Dispose();
				deleteCommand.Dispose();

				base.Dispose(disposing);
			}

			// Inserts a single item at the given index.
			// This should be avoided unless there really is only one item to be added, because it
			// will probably be quite slow.
			public void Insert(int index, WordListEntry item) {
				if (item == null)
					throw new System.ArgumentNullException();
				if (index < 0 || index > list.Count)
					throw new System.ArgumentOutOfRangeException("index");

				using (var txn = Connection.BeginTransaction()) {
					insertCommandSetID.Value = list.ID;
					insertCommandListPosition.Value = index;
					insertCommandPhrase.Value = item.Phrase;
					insertCommandTranslation.Value = item.Translation;

					insertCommand.ExecuteNonQuery();

					txn.Commit();
				}
			}

			//Inserts a list of entries at the given indices.
			public void Insert(int index, IList<WordListEntry> entries) {
				if (index < 0)
					throw new System.ArgumentOutOfRangeException("index");
				if (entries.Count == 0)
					return;

				using (var txn = Connection.BeginTransaction()) {
					insertCommandSetID.Value = list.ID;

					foreach (WordListEntry item in entries) {
						insertCommandListPosition.Value = index;
						insertCommandPhrase.Value = item.Phrase;
						insertCommandTranslation.Value = item.Translation;

						insertCommand.ExecuteNonQuery();

						index++;
					}

					txn.Commit();
				}
			}

			//Inserts a list of (index, entry) into the database.
			//This is used by the Deletion command's Redo method to speed up re-inserting the deleted entries.
			public void Insert(IList<KeyValuePair<int, WordListEntry>> entries) {
				if (entries.Count == 0)
					return;

				using (var txn = Connection.BeginTransaction()) {
					insertCommandSetID.Value = list.ID;

					foreach (var kvp in entries) {
						insertCommandListPosition.Value = kvp.Key;
						var item = kvp.Value;
						insertCommandPhrase.Value = item.Phrase;
						insertCommandTranslation.Value = item.Translation;

						insertCommand.ExecuteNonQuery();
					}

					txn.Commit();
				}
			}

			/// <summary>Removes one element from the list. Avoid using this if there are multiple elements.</summary>
			public void RemoveAt(int index) {
				using (var txn = Connection.BeginTransaction()) {
					deleteCommandSetID.Value = list.ID;
					deleteCommandListPosition.Value = index;
					deleteCommand.ExecuteNonQuery();

					txn.Commit();
				}
			}

			/// <summary>Removes a contiguous block of elements from the list.
			/// It's faster than the other Remove methods.</summary>
			public void RemoveAt(int index, int count) {
				using (var txn = Connection.BeginTransaction()) {
					ExecuteSQL(@"
						DELETE From VocabItems 
							WHERE SetID = ? AND ListPosition BETWEEN ? AND ?", 
						list.ID, index, index + count - 1);
					ExecuteSQL(@"
						UPDATE VocabItems 
							SET ListPosition = ListPosition - ? 
							WHERE SetID = ? AND ListPosition > ?", 
						count, list.ID, index);

					txn.Commit();
				}
			}

			/// <summary>Removes the items at the specified positions in the list.</summary>
			public void RemoveAt(IEnumerable<int> indices) {
				if (indices == null)
					throw new System.ArgumentNullException();

				using (var txn = Connection.BeginTransaction()) {
					deleteCommandSetID.Value = list.ID;

					foreach (int i in indices) {
						deleteCommandListPosition.Value = i;
						deleteCommand.ExecuteNonQuery();
					}

					txn.Commit();
				}
			}

			/// <summary>Creates WordListEntry instances from the database at the specified list positions. 
			/// Avoid if possible, use the in-memory list.</summary>
			public IList<WordListEntry> GetEntries(IEnumerable<int> indices) {
				if (indices == null)
					throw new System.ArgumentNullException();

				using (var command = Connection.CreateCommand()) {
					var sb = new System.Text.StringBuilder();
					sb.Append(@"
						SELECT Phrase, Translation
							FROM VocabItems WHERE SetID = ? AND ListPosition IN (");

					int i = 0;
					foreach (int index in indices) {
						if (i++ > 0)
							sb.Append(@", ");
						sb.Append(index);
					}

					sb.Append(@") ORDER BY ListPosition ASC");
					command.CommandText = sb.ToString();
					AddParameter(command, list.ID);

					using (var reader = command.ExecuteReader())
						return GetEntries(reader);
				}
			}

			public IList<WordListEntry> GetAllEntries() {
				var reader = SelectReader(@"
					TYPES Text, Text;
					SELECT Phrase, Translation
						FROM VocabItems 
						WHERE SetID = ? 
						ORDER BY ListPosition ASC", list.ID);

				return GetEntries(reader);
			}

			protected IList<WordListEntry> GetEntries(DbDataReader reader) {
				var result = new List<WordListEntry>();

				while (reader.Read()) {
					if (reader.IsDBNull(0) || reader.IsDBNull(1))
						throw new System.Data.StrongTypingException("One of the fields of a word list entry was DBNull.");

					string phrase = (string)reader.GetValue(0);
					string translation = (string)reader.GetValue(1);

					var entry = new WordListEntry(this.list, phrase, translation);
					result.Add(entry);
				}

				return result;
			}

			public object GetProperty(int index, string name) {
				using (var cmd = Connection.CreateCommand()) {
					cmd.CommandText = string.Format(
							CultureInfo.InvariantCulture, 
							@"SELECT {0} FROM VocabItems WHERE SetID = ? AND ListPosition = ?",
							name);
					AddParameter(cmd, list.ID);
					AddParameter(cmd, index);

					return cmd.ExecuteScalar();
				}
			}

			public void SetProperty(int index, string name, object value) {
				using (var txn = Connection.BeginTransaction()) {
					ExecuteSQL(
						string.Format(
							CultureInfo.InvariantCulture,
							@"UPDATE VocabItems SET {0} = ? WHERE SetID = ? AND ListPosition = ?", 
							name),
						value, list.ID, index);

					txn.Commit();
				}
			}

			public void SetEntry(int index, WordListEntry item) {
				using (var txn = Connection.BeginTransaction()) {
					ExecuteSQL(@"
						UPDATE VocabItems 
							SET Phrase = ?, Translation = ?
							WHERE SetID = ? AND ListPosition = ?",
						item.Phrase, item.Translation, 
						list.ID, index);

					txn.Commit();
				}
			}

			public void SetWordListProperty(string property, object value) {
				using (var txn = Connection.BeginTransaction()) {
					ExecuteSQL(
						string.Format(
							CultureInfo.InvariantCulture, 
							@"UPDATE Sets SET {0} = ? WHERE id = ?", 
							property),
						value, list.ID);

					txn.Commit();
				}
			}

			public object GetWordListProperty(string property) {
				return Select(
					string.Format(
						CultureInfo.InvariantCulture, 
						@"SELECT {0} FROM Sets WHERE id = ?", 
						property),
					list.ID);
			}
		
			public void SwapRows(IEnumerable<int> indices) {
				var sb = new StringBuilder();
				sb.Append(@"
					UPDATE VocabItems 
						SET Phrase = Translation, Translation = Phrase 
						WHERE SetID = ? AND ListPosition IN (");
				bool first = true;
				foreach(int i in indices) {
					if(!first)
						sb.Append(", ");
					else
						first = false;
					sb.Append(i);
				}
				sb.Append(@");");

				ExecuteSQL(sb.ToString(), list.ID);
			}
		}
		#endregion

		protected abstract class Command : ICommand {
			protected SqliteWordList owner;
			protected Worker worker;
			protected IList<WordListEntry> list;

			public Command(SqliteWordList owner) {
				this.worker = owner.worker;
				this.list = owner.list;
				this.owner = owner;
			}

			public abstract void Do();
			public abstract void Undo();
			public virtual void Redo() { Do(); }
			public abstract string Description { get; }
		}

		///<summary>Updates an entire item in the list.</summary>
		protected class SetItem : Command {
			int index;
			WordListEntry item;
			WordListEntry oldItem;

			public SetItem(SqliteWordList owner, int index, WordListEntry item)
				: base(owner) {
				this.index = index;
				this.item = item;
			}

			public override void Do() {
				oldItem = list[index];
				list[index] = item;
				worker.SetEntry(index, item);
			}

			public override void Undo() {
				item = list[index];

				list[index] = oldItem;
				worker.SetEntry(index, oldItem);
			}

			public override string Description {
				get { return LocalizationProvider.Default.Strings["InternalOperation"] ?? "(Internal Operation)"; }
			}
		}

		///<summary>Inserts one item into the list.</summary>
		protected class Insertion : Command {
			int index;
			WordListEntry item;

			public Insertion(SqliteWordList owner, int index, WordListEntry item)
				: base(owner) {
				this.index = index;
				this.item = item;
			}

			public override void Do() {
				// Safe with respect to external changes.
				// Obviously the item cannot be changed while it is not part of the list.
				worker.Insert(index, item);
				list.Insert(index, item);
			}

			public override void Undo() {
				// Needed for safeness with respect to external changes.
				// I write this with the practice functionality in mind: if practicing and editing a list concurrently,
				// the undo situation gets a bit hairy. There should only be one *editor* open for the list at once, but
				// there may be other things which are changing the list (e.g. practice window).
				item = list[index];

				worker.RemoveAt(index);
				list.RemoveAt(index);
			}

			public override string Description {
				get { return LocalizationProvider.Default.Strings["Inserted1Item"] ?? "Inserted 1 item"; }
			}
		}

		/// <summary>Inserts many items into the list. Faster than inserting one item.</summary>
		protected class MultipleInsertion : Command {
			int index;
			IList<WordListEntry> items;

			public MultipleInsertion(SqliteWordList owner, int index, IList<WordListEntry> items)
				: base(owner) {
				this.index = index;
				this.items = items;
			}

			public override void Do() {
				worker.Insert(index, items);
				for (int i = 0; i < items.Count; ++i)
					list.Insert(index + i, items[i]);
			}

			public override void Undo() {
				worker.RemoveAt(index, items.Count);

				// Sidestep oddities related to OOB changes by re-reading the items as they are now.
				for (int i = 0; i < items.Count; ++i) {
					items[i] = list[index];
					list.RemoveAt(index);
				}
			}

			public override string Description {
				get { 
					return string.Format( 
						LocalizationProvider.Default.Strings["InsertedNItems"] ?? "Inserted {0} items", 
						items.Count); 
				}
			}
		}

		/// <summary>Sets a single property of a single item in the list.</summary>
		protected class SetValue<T> : Command {
			int index;
			T oldValue, newValue;
			WordList.EntryProperty property;

			public SetValue(SqliteWordList owner, int index, WordList.EntryProperty property, T oldValue, T newValue)
				: base(owner) {
				this.index = index;
				this.oldValue = oldValue;
				this.newValue = newValue;
				this.property = property;
			}

			private void SetTo(T value) {
				worker.SetProperty(index, property.ToString(), value);
				list[index].SetProperty(property, value);
			}

			public override void Do() {
				SetTo(newValue);
			}

			public override void Undo() {
				// Again, sidestep oddities related to OOB changes. 
				// This is probably very unlikely to happen with the SetValue command, however.
				newValue = (T)list[index].GetProperty(property);

				SetTo(oldValue);
			}

			public override string Description {
				get { return string.Format(
					LocalizationProvider.Default.Strings["ChangedXToY"] ?? "Changed \"{0}\" to \"{1}\"", oldValue, newValue);
				}
			}
		}

		/// <summary>Deletes one or more items from the list.</summary>
		protected class Deletion : Command {
			List<KeyValuePair<int, WordListEntry>> items = new List<KeyValuePair<int, WordListEntry>>();
			List<int> indices;
			List<int> reverseIndices;

			public Deletion(SqliteWordList owner, IEnumerable<int> indices)
				: base(owner) {
				foreach (int i in indices)
					items.Add(new KeyValuePair<int, WordListEntry>(i, 
						new WordListEntry(owner, list[i].Phrase, list[i].Translation)
						));

				// The indices must be in order for the deletion to proceed correctly.
				// Deleting lower indices before higher indices would result in shifting, and
				// subsequent deletions may delete the wrong entry.
				items.Sort((k1, k2) => k1.Key.CompareTo(k2.Key));

				this.indices = items.ConvertAll(kvp => kvp.Key);
			}

			public Deletion(SqliteWordList owner, params int[] indices)
				: this(owner, (IEnumerable<int>)indices) {
			}

			public IEnumerable<int> Indices {
				get {
					return indices;
				}
			}

			public IEnumerable<int> ReverseIndices {
				get {
					if (reverseIndices == null) {
						var r = new List<int>(indices);
						r.Reverse();
						reverseIndices = r;
					}
					return reverseIndices;
				}
			}

			public override void Do() {
				worker.RemoveAt(ReverseIndices);
				foreach (int i in ReverseIndices)
					list.RemoveAt(i);
			}

			public override void Undo() {
				// There can't be any funny stuff related to OOB changes here, since any entry 
				// can't be edited while it's deleted.
				// TODO: Modifying a deleted item should raise an exception.

				worker.Insert(items);
				foreach (var kvp in items)
					list.Insert(kvp.Key, kvp.Value);
			}

			public override string Description {
				get { return string.Format(LocalizationProvider.Default.Strings["DeletedNItems"] ?? "Deleted {0} items", items.Count); }
			}
		}

		/// <summary>Swaps the phrase and translation of the entries at the given indices. Safe with regards to
		/// out-of-band changes, since it stores no data.</summary>
		protected class SwapRowsCommand : Command {
			IEnumerable<int> indices;

			public SwapRowsCommand(SqliteWordList owner, IEnumerable<int> indices) 
				: base(owner)
			{
				this.indices = indices;
			}

			public override void Do() {
				worker.SwapRows(indices);
				foreach (int i in indices) {
					list[i] = new WordListEntry(owner,
						list[i].Translation, list[i].Phrase);
				}
			}

			public override void Undo() { Do(); }

			public override string Description { 
				get { 
					return LocalizationProvider.Default.Strings["SwappedNRows"];
				}
			}
		}

		public override void Undo() {
			if(UndoList.UndoItemCount > 0)
				UndoList.Undo(1);
		}

		public override void Redo() {
			if (UndoList.RedoItemCount > 0)
				UndoList.Redo(1);
		}
	}
}