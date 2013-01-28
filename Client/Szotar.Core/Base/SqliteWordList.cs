using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CultureInfo = System.Globalization.CultureInfo;

// SqliteWordList: an SQLite-backed word list. All changes made are immediately
// synchronised with the database, and can be undone or re-done. It also has an
// in-memory version, stored in a BindingList, which provides the ListChanged 
// events and fast read access.

// Despite being database-backed, this list is NOT thread-safe!

// The undo list is composed of a zipper list of ICommand objects which can be
// performed and undone. See Undo.cs for more information.

// The undo functionality is global to the list, NOT specific to each editor,
// so this can obviously cause problems if multiple consumers are editing the
// list at the same time. However, having separate undo lists doesn't really
// make much sense either, unless the undo lists were somehow able to update
// each other such that undoing an action in one list updates the other undo 
// list's state such that it is correct. Maybe this would be something to 
// explore at a later date, but I suspect that the provided workarounds will 
// suffice for now.

// The UI should ensure that only one UI element is providing editing services 
// for the word list, to avoid undo-related confusion.

// Note: it should be an error to perform deletion or insertion commands out-of-band, 
// as such things could cause normal undo/redo functionality to fail.
namespace Szotar.Sqlite {
	public class SqliteWordList : WordList {
		readonly Worker worker;
		readonly BindingList<WordListEntry> list;
		readonly long id;
		bool raiseListEvents = true;
		readonly UndoList<Command> undoList;

		public SqliteDataStore DataStore { get; private set; }

		public override long? ID {
			get { return id; }
		}

		protected SqliteWordList(SqliteDataStore store, long setID) {
			DataStore = store;
			id = setID;
			worker = new Worker(store, this);
			undoList = new UndoList<Command>();

			list = new BindingList<WordListEntry>(worker.GetAllEntries());
			list.ListChanged += BindingListChanged;
		}

		public static SqliteWordList FromSetID(SqliteDataStore store, long setID) {
			var list = new SqliteWordList(store, setID);
			if (!list.worker.Exists()) {
				list.Dispose();
				return null;
			}

			return list;
		}

		//Re-raise the ListChanged event with the SqliteWordList as the originator.
		//Using BindingList as the in-memory list makes this very easy.
		void BindingListChanged(object sender, ListChangedEventArgs e) {
			if (raiseListEvents)
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
			DataStore.DeleteWordList(id);

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
		public override DateTime? Date {
			get { return (DateTime?)worker.GetWordListProperty("Created"); }
			set {
				if (value == null)
					throw new ArgumentNullException();
				worker.SetWordListProperty("Created", value);
				RaisePropertyChanged("Date");
			}
		}

		public override DateTime? Accessed {
			get { return (DateTime?)worker.GetWordListProperty("Accessed"); }
			set {
				if (value == null)
					throw new ArgumentNullException();
				worker.SetWordListProperty("Accessed", value);
				RaisePropertyChanged("Accessed");
			}
		}

		public override T GetProperty<T>(WordListEntry entry, EntryProperty property) {
			int index = IndexOf(entry);
			Debug.Assert(index >= 0);
			return (T)worker.GetProperty(index, property.ToString());
		}

		/// <summary>Set a property of an entry in the database (but not in the in-memory list).</summary>
		public override void SetProperty(WordListEntry entry, EntryProperty property, object value) {
			int index = IndexOf(entry);
			Debug.Assert(index >= 0);
			worker.SetProperty(index, property.ToString(), value);
		}

		public override string[] Tags { get { return DataStore.GetTags(id).ToArray(); } }
		public override void Tag(string tag) { DataStore.Tag(tag, id); }
		public override void Untag(string tag) { DataStore.Untag(tag, id); }
		public override bool HasTag(string tag) { return DataStore.HasTag(tag, id); }
		#endregion

		#region List implementation
		public override void Add(WordListEntry item) {
			Insert(Count, item);
		}

		public override void Insert(int index, WordListEntry item) {
			if (index > Count || index < 0)
				throw new ArgumentOutOfRangeException("index");
			undoList.Do(new Insertion(this, index, item));
		}

		public override void Insert(int index, IList<WordListEntry> items) {
			if (index > Count || index < 0)
				throw new ArgumentOutOfRangeException("index");
			undoList.Do(new MultipleInsertion(this, index, items));
		}

		public override bool Remove(WordListEntry item) {
			bool existed = list.Contains(item);
			undoList.Do(new Deletion(this, IndexOf(item)));
			return existed;
		}

		public override void RemoveAt(int index) {
			if (index >= Count || index < 0)
				throw new ArgumentOutOfRangeException("index");
			undoList.Do(new Deletion(this, index));
		}

		public override void RemoveAt(IList<int> indices) {
			undoList.Do(new Deletion(this, indices));
		}

		public override void SwapRows(IList<int> indices) {
			undoList.Do(new SwapRowsCommand(this, indices));
		}

		public override void Clear() {
			undoList.Do(new Deletion(this, EnumRange(0, list.Count - 1)));
		}

		public override void Sort(Comparison<WordListEntry> comparison) {
			undoList.Do(new SortCommand(this, comparison));
		}

		public override void MoveRows(IList<int> rows, int destinationRowIndex) {
			undoList.Do(new MoveRowsCommand(this, rows, destinationRowIndex));
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
			set { undoList.Do(new SetItem(this, index, value)); }
		}

		public override int IndexOf(WordListEntry item) {
			return list.IndexOf(item);
		}

		public override IEnumerator<WordListEntry> GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion

		/// <summary>
		/// Disables list changed events during the duration of the update action, and then raises a Reset list event afterwards.
		/// </summary>
		/// <param name="update">The update action is allowed to work with the internal binding list 
		/// for the duration of the update, to avoid adding new undo items.</param>
		protected void PerformMajorUpdate(Action update) {
			try {
				raiseListEvents = false;
				update();
			} finally {
				raiseListEvents = true;
				list.ResetBindings();
			}
		}

		protected void MaybePerformMajorUpdate(bool major, Action update) {
			if (major)
				PerformMajorUpdate(update);
			else
				update();
		}

		#region Worker
		//Worker does the dirty database work. It's nicer if it's separated from the rest of the list code.
		protected class Worker : SqliteObject {
			readonly SqliteWordList list;

			//These are called particularly often, any performance enhancements are useful.
			readonly DbCommand insertCommand, deleteCommand, existsCommand;
			readonly DbParameter insertCommandSetID, insertCommandListPosition, insertCommandPhrase,
				insertCommandTranslation, deleteCommandSetID, deleteCommandListPosition, existsCommandID;

			public Worker(SqliteObject store, SqliteWordList list)
				: base(store) {
				this.list = list;

				existsCommand = Connection.CreateCommand();
				existsCommand.CommandText = @"SELECT Count(*) FROM Sets WHERE Sets.id = ?";
				existsCommandID = existsCommand.CreateParameter();
				existsCommand.Parameters.Add(existsCommandID);
				existsCommandID.Value = list.ID;

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
				existsCommand.Dispose();
				deleteCommand.Dispose();

				base.Dispose(disposing);
			}

			public bool Exists() {
				return Convert.ToInt64(existsCommand.ExecuteScalar()) > 0;
			}

			/// <summary>
			/// Inserts a single item at the given index.
			/// </summary>
			/// <remarks>This should be avoided unless there really is only one item to be added, 
			/// because it will probably be quite slow with many items.</remarks>
			public void Insert(int index, WordListEntry item) {
				if (item == null)
					throw new ArgumentNullException();
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException("index");

				using (var txn = Connection.BeginTransaction()) {
					insertCommandSetID.Value = list.ID;
					insertCommandListPosition.Value = index;
					insertCommandPhrase.Value = item.Phrase;
					insertCommandTranslation.Value = item.Translation;

					insertCommand.ExecuteNonQuery();

					txn.Commit();
				}
			}

			/// <summary>
			/// Inserts a list of entries at the given indices.
			/// </summary>
			public void Insert(int index, IList<WordListEntry> entries) {
				if (index < 0)
					throw new ArgumentOutOfRangeException("index");
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

			/// <summary>
			/// Inserts a list of (index, entry) into the database.
			/// </summary>
			/// <remarks>This is used by the Deletion command's Redo method to speed up re-inserting the deleted entries.</remarks>
			/// <param name="entries"></param>
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
			/// TODO: This could be made faster by making the sorting optional.
			public void RemoveAt(IEnumerable<int> indices) {
				if (indices == null)
					throw new ArgumentNullException();

				var sorted = new List<int>(indices);
				sorted.Sort();

				int shift = 0;

				using (var txn = Connection.BeginTransaction()) {
					deleteCommandSetID.Value = list.ID;

					foreach (int i in sorted) {
						deleteCommandListPosition.Value = i - shift++;
						deleteCommand.ExecuteNonQuery();
					}

					txn.Commit();
				}
			}

			/// <summary>Creates WordListEntry instances from the database at the specified list positions. 
			/// Avoid if possible, use the in-memory list.</summary>
			public IList<WordListEntry> GetEntries(IEnumerable<int> indices) {
				if (indices == null)
					throw new ArgumentNullException();

				using (var command = Connection.CreateCommand()) {
					var sb = new StringBuilder();
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
				using (var reader = SelectReader(@"
					TYPES Text, Text;
					SELECT Phrase, Translation
						FROM VocabItems 
						WHERE SetID = ? 
						ORDER BY ListPosition ASC", list.ID)) {

					return GetEntries(reader);
				}
			}

			protected IList<WordListEntry> GetEntries(DbDataReader reader) {
				var result = new List<WordListEntry>();

				while (reader.Read()) {
					if (reader.IsDBNull(0) || reader.IsDBNull(1))
						throw new StrongTypingException("One of the fields of a word list entry was DBNull.");

					var phrase = (string)reader.GetValue(0);
					var translation = (string)reader.GetValue(1);

					var entry = new WordListEntry(list, phrase, translation);
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
				foreach (int i in indices) {
					if (!first)
						sb.Append(", ");
					else
						first = false;
					sb.Append(i);
				}
				sb.Append(@");");

				ExecuteSQL(sb.ToString(), list.ID);
			}

			// Renumbers the items by setting the ListPosition property on the moved 
			// item to be the negation of the new index. When all movements have taken place,
			// the negatively-indexed items are set to positive.
			// 
			// It is the caller's responsibility to ensure that the movements relation is a
			// bijection: if it is not, two items may end up with the same index!
			public void ReorderList(Action<Action<int, int>> movements) {
				using (var txn = Connection.BeginTransaction()) {
					using (var cmd = Connection.CreateCommand()) {
						cmd.CommandText =
							@"UPDATE VocabItems
								SET ListPosition = $NewPos 
								WHERE SetID = $SetID AND ListPosition = $OldPos";

						var newPos = cmd.CreateParameter();
						newPos.ParameterName = "NewPos";
						cmd.Parameters.Add(newPos);

						var setID = cmd.CreateParameter();
						setID.ParameterName = "SetID";
						setID.Value = list.id;
						cmd.Parameters.Add(setID);

						var oldPos = cmd.CreateParameter();
						oldPos.ParameterName = "OldPos";
						cmd.Parameters.Add(oldPos);

						Action<int, int> moveItem = (from, to) => {
							if (from == to)
								return;

							oldPos.Value = from;
							newPos.Value = -to - 1; // Otherwise, 0 maps to 0.
							cmd.ExecuteNonQuery();
						};

						movements(moveItem);
					}

					ExecuteSQL(
						@"UPDATE VocabItems
							SET ListPosition = -(ListPosition + 1)
							WHERE SetID = ? AND ListPosition < 0",
						list.id);

					txn.Commit();
				}
			}
		}
		#endregion

		#region Commands
		protected abstract class Command : ICommand {
			protected SqliteWordList owner;
			protected Worker worker;
			protected IList<WordListEntry> list;

			protected Command(SqliteWordList owner) {
				worker = owner.worker;
				list = owner.list;
				this.owner = owner;
			}

			public abstract void Do();
			public abstract void Undo();
			public virtual void Redo() { Do(); }
			public abstract string Description { get; }
			public abstract int AffectedRows { get; }
		}

		///<summary>Updates an entire item in the list.</summary>
		protected class SetItem : Command {
			readonly int index;
			WordListEntry item;
			WordListEntry oldItem;

			public SetItem(SqliteWordList owner, int index, WordListEntry item)
				: base(owner)
			{
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
				get {
					string desc = null;
					if (item.Phrase != oldItem.Phrase && item.Translation != oldItem.Translation) {
					} else if(item.Phrase != oldItem.Phrase) {
						desc = string.Format(
							LocalizationProvider.Default.Strings["ChangedXToY"],
							oldItem.Phrase,
							item.Phrase);
					} else if(item.Translation != oldItem.Translation) {
						desc = string.Format(
							LocalizationProvider.Default.Strings["ChangedXToY"],
							oldItem.Translation,
							item.Translation);
					}

					// If both or no items changed, use this.
					desc = desc ?? 
						string.Format(
							LocalizationProvider.Default.Strings["ChangedItemToXandY"],
							item.Phrase,
							item.Translation);

					return desc;
				}
			}

			public override int AffectedRows {
				get { return 1; }
			}
		}

		///<summary>Inserts one item into the list.</summary>
		protected class Insertion : Command {
			readonly int index;
			readonly WordListEntry item;

			public Insertion(SqliteWordList owner, int index, WordListEntry item)
				: base(owner)
			{
				this.index = index;
				this.item = item;
			}

			public override void Do() {
				worker.Insert(index, item);
				list.Insert(index, item);
			}

			public override void Undo() {
				worker.RemoveAt(index);
				list.RemoveAt(index);
			}

			public override string Description {
				get { return LocalizationProvider.Default.Strings["Inserted1Item"]; }
			}

			public override int AffectedRows {
				get { return 1; }
			}
		}

		/// <summary>Inserts many items into the list. Faster than inserting one item.</summary>
		protected class MultipleInsertion : Command {
			readonly int index;
			readonly IList<WordListEntry> items;

			public MultipleInsertion(SqliteWordList owner, int index, IList<WordListEntry> items)
				: base(owner)
			{
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

				for (int i = 0; i < items.Count; ++i)
					list.RemoveAt(index);
			}

			public override string Description {
				get {
					return string.Format(
						LocalizationProvider.Default.Strings["InsertedNItems"],
						items.Count);
				}
			}

			public override int AffectedRows {
				get { return items.Count; }
			}
		}

		/// <summary>Sets a single property of a single item in the list.</summary>
		protected class SetValue<T> : Command {
			readonly int index;
			readonly T oldValue, newValue;
			readonly EntryProperty property;

			public SetValue(SqliteWordList owner, int index, EntryProperty property, T oldValue, T newValue)
				: base(owner)
			{
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
				SetTo(oldValue);
			}

			public override string Description {
				get {
					return string.Format(
						LocalizationProvider.Default.Strings["ChangedXToY"] ?? "Change \"{0}\" to \"{1}\"", oldValue, newValue);
				}
			}

			public override int AffectedRows {
				get { return 1; }
			}
		}

		/// <summary>Deletes one or more items from the list.</summary>
		protected class Deletion : Command {
			readonly List<KeyValuePair<int, WordListEntry>> items = new List<KeyValuePair<int, WordListEntry>>();
			readonly List<int> indices;
			List<int> reverseIndices;

			public Deletion(SqliteWordList owner, IEnumerable<int> indices)
				: base(owner)
			{
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
				: this(owner, (IEnumerable<int>)indices) 
			{ }

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
				get { return string.Format(LocalizationProvider.Default.Strings["DeletedNItems"], items.Count); }
			}

			public override int AffectedRows {
				get { return items.Count; }
			}
		}

		/// <summary>Swaps the phrase and translation of the entries at the given indices. Safe with regards to
		/// out-of-band changes, since it stores no data.</summary>
		protected class SwapRowsCommand : Command {
			readonly IList<int> indices;

			public SwapRowsCommand(SqliteWordList owner, IList<int> indices)
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
					return string.Format(
						LocalizationProvider.Default.Strings["SwappedNRows"],
						indices.Count);
				}
			}

			public override int AffectedRows {
				get { return indices.Count; }
			}
		}

		// Sorts the list, storing a list of movements required to get from the unsorted list to
		// the sorted list, which can be used to reverse the operation.
		protected class SortCommand : Command {
			struct Movement {
				public int FromIndex { get; set; }
				public int ToIndex { get; set; }
			};

			readonly Comparison<WordListEntry> comparison;
			readonly List<Movement> movements = new List<Movement>();

			public SortCommand(SqliteWordList owner, Comparison<WordListEntry> comparison) 
				: base(owner)
			{
				if (comparison == null)
					throw new ArgumentNullException("comparison");

				this.comparison = comparison;
			}

			public override void Do() {
				var original = new List<WordListEntry>(owner);

				var sorted = new List<WordListEntry>(owner);
				sorted.Sort(comparison);

				// Generate the list of item movements.
				for (int i = 0; i < original.Count; ++i) {
					int newIndex = sorted.IndexOf(original[i]);

					Debug.Assert(newIndex >= 0);

					movements.Add(new Movement {
						FromIndex = i,
						ToIndex = newIndex
					});
				}

				owner.PerformMajorUpdate(delegate {
					// BindingList doesn't have a Sort method: no problem, though.
					list.Clear();
					foreach (var e in sorted)
						list.Add(e);

					worker.ReorderList(move => {
						foreach (var movement in movements)
							move(movement.FromIndex, movement.ToIndex);
					});
				});
			}

			public override void Undo() {
				owner.PerformMajorUpdate(delegate {
					var sorted = new List<WordListEntry>(list);
					list.Clear();

					for (int i = 0; i < sorted.Count; ++i)
						list.Add(null);

						// Perform the movements backwards.
						foreach (var m in movements)
							list[m.FromIndex] = sorted[m.ToIndex];

					worker.ReorderList(move => {
						foreach (var movement in movements)
							move(movement.ToIndex, movement.FromIndex);
					});
				});
			}

			public override string Description {
				get { return LocalizationProvider.Default.Strings["SortedList"]; }
			}

			public override int AffectedRows {
				get { return movements.Count; }
			}
		}

		// Moves a set of rows within the list to a specific row index.
		// If multiple rows are moved, they retain their order in the list, and are all inserted
		// at the given position.
		protected class MoveRowsCommand : Command {
			readonly List<int> rows;
			readonly int destination;
			int shiftedDestination;
			readonly bool idempotent;

			public MoveRowsCommand(SqliteWordList owner, IList<int> rows, int destination)
				: base(owner)
			{
				foreach (int i in rows)
					if (i < 0 || i >= list.Count)
						throw new ArgumentException("rows");

				if (destination < 0 || destination > list.Count)
					throw new ArgumentException("destination");

				// We require that the list of rows is in ascending order.
				this.rows = new List<int>(rows);
				this.rows.Sort();

				this.destination = destination;

				// An optimisation: check if the operation would do nothing.
				bool contiguous = true;
				for (int i = 0; i < rows.Count - 1; ++i) {
					if (rows[i] + 1 != rows[i + 1]) {
						contiguous = false;
						break;
					}
				}

				idempotent = rows.Count == 0
					|| (contiguous && destination >= rows[0] && destination <= rows[0] + rows.Count);
			}

			public override void Do() {
				if (idempotent)
					return;

				// Get the values being moved, so we can insert them later.
				var values = new List<WordListEntry>(rows.Count);

				// Account for the index shift caused by removing items.
				// Alternately, we could remove the items backwards. But we don't.
				int shift = 0;
				shiftedDestination = destination;

				foreach (int i in rows) {
					values.Add(list[i - shift]);
					list.RemoveAt(i - shift);

					shift++;
					if (i < destination)
						shiftedDestination--;
				}

				var insertWhere = shiftedDestination;
				foreach (var entry in values)
					list.Insert(insertWhere++, entry);

				// The database modification is simpler when expressed as a removal and re-insertion.
				// (It would be possible to speed this up with a dedicated function, but much more complex.)
				worker.RemoveAt(rows);

				// The shifted destination is the correct place to insert.
				worker.Insert(shiftedDestination, values);
			}

			public override void Undo() {
				if (idempotent)
					return;

				var values = new List<WordListEntry>(rows.Count);

				for (int i = 0; i < rows.Count; ++i) {
					values.Add(list[shiftedDestination]);
					list.RemoveAt(shiftedDestination);
				}

				for (int i = 0; i < rows.Count; ++i)
					list.Insert(rows[i], values[i]);

				// Again, the database modification code is more simply expressed in terms of removals and insertions.
				worker.RemoveAt(shiftedDestination, rows.Count);

				var insertions = new List<KeyValuePair<int, WordListEntry>>(rows.Count);
				for (int i = 0; i < rows.Count; ++i)
					insertions.Add(new KeyValuePair<int, WordListEntry>(rows[i], values[i]));

				worker.Insert(insertions);
			}

			public override string Description {
				get {
					if (rows.Count == 1)
						return LocalizationProvider.Default.Strings["Moved1Row"];

					return string.Format(LocalizationProvider.Default.Strings["MovedNRows"], rows.Count);
				}
			}

			public override int AffectedRows {
				get { return rows.Count; }
			}
		}

		static bool IsMajor(Command command) {
			return command.AffectedRows > 8;
		}
		#endregion

		#region Undo
		void Do(Command command) {
			MaybePerformMajorUpdate(IsMajor(command), () => undoList.Do(command));
		}

		public override void Undo() {
			var cmd = undoList.UndoCommand;
			if (cmd != null)
				MaybePerformMajorUpdate(IsMajor(cmd), () => undoList.Undo(1));
		}

		public override void Redo() {
			var cmd = undoList.RedoCommand;
			if (cmd != null)
				MaybePerformMajorUpdate(IsMajor(cmd), () => undoList.Redo(1));
		}

		public override string UndoDescription {
			get { return undoList.UndoItemDescriptions.FirstOrDefault(); }
		}

		public override string RedoDescription {
			get { return undoList.RedoItemDescriptions.FirstOrDefault(); }
		}
		#endregion
	}
}