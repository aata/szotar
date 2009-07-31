using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using IO = System.IO;

namespace Szotar.Sqlite {
	[global::System.Serializable]
	public class DatabaseVersionException : Exception {
		public DatabaseVersionException() { }
		public DatabaseVersionException(string message) : base(message) { }
		public DatabaseVersionException(string message, Exception inner) : base(message, inner) { }
		protected DatabaseVersionException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	public abstract class SqliteDatabase : SqliteObject {
		string path;

		public SqliteDatabase(string path)
			: base(OpenDatabase(path)) {
			this.path = path;
			conn.Open();

			Init();
		}

		static DbConnection OpenDatabase(string path) {
			string dir = IO.Path.GetDirectoryName(path);
			if (!IO.Directory.Exists(dir))
				IO.Directory.CreateDirectory(dir);
#if !MONO
			return new System.Data.SQLite.SQLiteConnection("Data Source=" + path);
#else
			return new Mono.Data.Sqlite.SqliteConnection("Data Source=" + path);
#endif
		}

		protected string Path {
			get { return path; }
		}

		private void Init() {
			int appSchemaVer = this.ApplicationSchemaVersion();
			Debug.Assert(appSchemaVer >= 1, "Desired SQLite database schema version should be greater than 0");

			int dbVer = GetDatabaseSchemaVersion();
			if (dbVer < appSchemaVer) {
				using (var txn = conn.BeginTransaction()) {
					UpgradeSchema(dbVer, appSchemaVer);
					SetDatabaseSchemaVersion(appSchemaVer);
					txn.Commit();
				}
			} else if (dbVer > appSchemaVer) {
				throw new DatabaseVersionException("The SQLite database is created by a newer version of this application.");
			}
		}

		protected abstract int ApplicationSchemaVersion();
		protected virtual void UpgradeSchema(int fromVersion, int toVersion) { UpgradeSchemaInIncrements(fromVersion, toVersion); }
		protected virtual void IncrementalUpgradeSchema(int toVersion) { }

		protected bool TableExists(string name) {
			return conn.GetSchema("Tables").Select(string.Format("Table_Name = '{0}'", name)).Length > 0;
		}

		protected int GetDatabaseSchemaVersion() {
			if (TableExists("Info")) {
				using (var cmd = conn.CreateCommand()) {
					cmd.CommandText = "TYPES integer; SELECT Info.Value FROM Info WHERE Info.Name = 'Version'";

					return Convert.ToInt32(cmd.ExecuteScalar());
				}
			} else {
				return 0;
			}
		}

		protected void SetDatabaseSchemaVersion(int version) {
			using (var txn = conn.BeginTransaction()) {
				if (!TableExists("Info")) {
					using (var cmd = conn.CreateCommand()) {
						cmd.CommandText = "CREATE TABLE Info (Name TEXT PRIMARY KEY, Value TEXT)";
						cmd.ExecuteNonQuery();
					}
				}

				if (Select(@"SELECT Value FROM Info WHERE Name = 'Version'") == null)
					ExecuteSQL(@"INSERT INTO Info (Name, Value) VALUES ('Version', ?)", version.ToString());
				else
					ExecuteSQL(@"UPDATE Info SET Value = ? WHERE Name = 'Version'", version.ToString());

				txn.Commit();
			}
		}

		protected void UpgradeSchemaInIncrements(int fromVersion, int toVersion) {
			if (fromVersion >= toVersion)
				throw new ArgumentException("Can't downgrade the application database. What happen?", "fromVersion");

			for (; fromVersion < toVersion; ++fromVersion)
				IncrementalUpgradeSchema(fromVersion + 1);
		}

		protected override void Dispose(bool disposing) {
			if (disposing)
				conn.Dispose();

			base.Dispose(disposing);
		}
	}

	public abstract class SqliteObject : IDisposable {
		protected DbConnection conn;

		public SqliteObject(SqliteObject other) {
			this.conn = other.conn;
		}

		protected SqliteObject(DbConnection connection) {
			this.conn = connection;
		}

		/// <summary>
		/// Get the ID of the last inserted row. This is equal to the primary key of that row, if one exists.
		/// If multiple threads are modifying the database, this value is unpredictable. 
		/// So, please, don't do that.
		/// </summary>
		protected long GetLastInsertRowID() {
			var lastInsertCommand = conn.CreateCommand();
			lastInsertCommand.CommandText = @"SELECT last_insert_rowid()";

			return (long)lastInsertCommand.ExecuteScalar();
		}

		protected void ExecuteSQL(string sql, params object[] parameters) {
			using (DbCommand command = conn.CreateCommand()) {
				command.CommandText = sql;

				foreach (object p in parameters)
					AddParameter(command, p);

				command.ExecuteNonQuery();
			}
		}

		protected object Select(string sql, params object[] parameters) {
			using (DbCommand command = conn.CreateCommand()) {
				command.CommandText = sql;

				foreach (object p in parameters)
					AddParameter(command, p);

				var res = command.ExecuteScalar();
				if (res is DBNull)
					return null;
				return res;
			}
		}

		protected DbDataReader SelectReader(string sql, params object[] parameters) {
			using (DbCommand command = conn.CreateCommand()) {
				command.CommandText = sql;

				foreach (object p in parameters) {
					var param = command.CreateParameter();
					param.Value = p;
					command.Parameters.Add(param);
				}

				return command.ExecuteReader();
			}
		}

		protected void AddParameter(DbCommand command, object value) {
			var param = command.CreateParameter();
			param.Value = value;
			command.Parameters.Add(param);
		}

		protected DbConnection Connection { get { return conn; } }

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
		}
	}

	public class WordListDeletedEventArgs : EventArgs {
		public long SetID { get; set; }
	}

	public class SqliteDataStore : SqliteDatabase {
		Dictionary<long, NullWeakReference<SqliteWordList>> wordLists = new Dictionary<long, NullWeakReference<SqliteWordList>>();

		public SqliteDataStore(string path)
			: base(path) {
		}

		protected override void UpgradeSchema(int fromVersion, int toVersion) {
			using (var txn = Connection.BeginTransaction()) {
				UpgradeSchemaInIncrements(fromVersion, toVersion);

				txn.Commit();
			}
		}

		protected override int ApplicationSchemaVersion() {
			return 2;
		}

		protected override void IncrementalUpgradeSchema(int toVersion) {
			if (toVersion == 1)
				InitDatabase();
			if (toVersion == 2)
				UpgradePracticeSchema();
			else
				throw new ArgumentOutOfRangeException("toVersion");
		}

		// Initialize the database to schema version 1, the initial version.
		private void InitDatabase() {
			ExecuteSQL(@"
				CREATE TABLE VocabItems (
					id INTEGER PRIMARY KEY AUTOINCREMENT, 
					Phrase TEXT NOT NULL, 
					Translation TEXT NOT NULL, 
					SetID INTEGER NOT NULL,	
					ListPosition INTEGER NOT NULL, 
					TimesTried INTEGER NOT NULL, 
					TimesFailed INTEGER NOT NULL);

				CREATE INDEX VocabItems_IndexP ON VocabItems (Phrase);
				CREATE INDEX VocabItems_IndexT ON VocabItems (Translation);
				CREATE INDEX VocabItems_IndexS ON VocabItems (SetID);
				CREATE INDEX VocabItems_IndexSO ON VocabItems (SetID, ListPosition);

				CREATE TABLE Sets (
					id INTEGER PRIMARY KEY AUTOINCREMENT, 
					Name TEXT NOT NULL, 
					Author TEXT, 
					Language TEXT, 
					Url TEXT, 
					Created Date);

				CREATE INDEX Sets_Index ON Sets (id);

				CREATE TABLE SetProperties (SetID INTEGER NOT NULL, Property TEXT, Value TEXT);
				CREATE INDEX SetProperties_Index ON SetProperties (SetID, Property);

				CREATE TABLE SetMemberships (ChildID INTEGER NOT NULL, ParentID INTEGER NOT NULL);
				CREATE INDEX SetMemberships_Index ON SetMemberships (ChildID, ParentID);");
		}

		// Upgrade the schema for the new practice functionality.
		//  * Remove the TimesTried and TimesFailed columns (at long last)
		//  * Add a practice history. Each and every practice item is recorded, along with a date,
		//    so this may have privacy issues. It will be used to focus on words that are difficult
		//    to get right, words that have not yet been practiced, and other such things.
		private void UpgradePracticeSchema() {
			// SQLite does not implement dropping columns.
			// This makes things a little more difficult than it has to be.
			ExecuteSQL(@"
				DROP INDEX VocabItems_IndexP;
				DROP INDEX VocabItems_IndexT;
				DROP INDEX VocabItems_IndexS;
				DROP INDEX VocabItems_IndexSO;

				ALTER TABLE VocabItems RENAME TO VocabItemsOld;
				CREATE TABLE VocabItems (
					id INTEGER PRIMARY KEY AUTOINCREMENT,
					Phrase TEXT NOT NULL COLLATE NOCASE,
					Translation TEXT NOT NULL COLLATE NOCASE,
					SetID INTEGER NOT NULL,
					ListPosition INTEGER NOT NULL);
				
				CREATE INDEX VocabItems_IndexP ON VocabItems (Phrase);
				CREATE INDEX VocabItems_IndexT ON VocabItems (Translation);
				CREATE INDEX VocabItems_IndexS ON VocabItems (SetID);
				CREATE INDEX VocabItems_IndexSO ON VocabItems (SetID, ListPosition);

				INSERT INTO VocabItems (id, Phrase, Translation, SetID, ListPosition)
					SELECT id, Phrase, Translation, SetID, ListPosition
					FROM VocabItemsOld;
				DROP TABLE VocabItemsOld;

				CREATE TABLE PracticeHistory (
					id INTEGER PRIMARY KEY AUTOINCREMENT,
					Phrase TEXT NOT NULL COLLATE NOCASE,
					Translation TEXT NOT NULL COLLATE NOCASE,
					SetID INTEGER NOT NULL,
					Created DATE NOT NULL,
					Result INTEGER NOT NULL -- 1 for success, 0 for failure
					);

				-- Are these necessary?
				CREATE INDEX PracticeHistory_IndexP ON PracticeHistory (Phrase);
				CREATE INDEX PracticeHistory_IndexT ON PracticeHistory (Translation);
				CREATE INDEX PracticeHistory_IndexPTS ON PracticeHistory (Phrase, Translation, SetID);
				CREATE INDEX PracticeHistory_IndexC ON PracticeHistory (Created);");
		}

		/// <param name="setID">The SetID of the word list</param>
		/// <returns>An existing SqliteWordList instance, if one exists, otherwise a newly-created SqliteWordList.</returns>
		public SqliteWordList GetWordList(long setID) {
			SqliteWordList wl = null;
			NullWeakReference<SqliteWordList> list;

			if (wordLists.TryGetValue(setID, out list))
				wl = list.Target;

			if (wl != null)
				return wl;

			wl = new SqliteWordList(this, setID);
			wordLists[setID] = new NullWeakReference<SqliteWordList>(wl);
			return wl;
		}

		public SqliteWordList CreateSet(string name, string author, string language, string url, DateTime? date) {
			long setID;

			using (var txn = Connection.BeginTransaction()) {
				ExecuteSQL("INSERT INTO Sets (Name, Author, Language, Url, Created) VALUES (?, ?, ?, ?, ?)",
					name, author, language, url, date.HasValue ? (object)date.Value : (object)DBNull.Value);

				setID = GetLastInsertRowID();
				txn.Commit();
			}

			var wl = new SqliteWordList(this, setID);
			wordLists[setID] = new NullWeakReference<SqliteWordList>(wl);
			return wl;
		}

		private ListInfo ListInfoFromReader(DbDataReader reader) {
			var list = new ListInfo();

			list.ID = reader.GetInt64(0);
			list.Name = reader.GetString(1); //Can't be null
			if (!reader.IsDBNull(2))
				list.Author = reader.GetString(2);
			if (!reader.IsDBNull(3))
				list.Language = reader.GetString(3);
			if (!reader.IsDBNull(4))
				list.Url = reader.GetString(4);
			if (!reader.IsDBNull(5))
				list.Date = reader.GetDateTime(5);
			if (!reader.IsDBNull(6))
				list.TermCount = reader.GetInt64(6);

			return list;
		}

		public IEnumerable<ListInfo> GetAllSets() {
			using (var reader = this.SelectReader("TYPES Integer, Text, Text, Text, Text, Date, Integer; SELECT id, Name, Author, Language, Url, Created, (SELECT count(*) FROM VocabItems WHERE SetID = Sets.id) FROM Sets ORDER BY id ASC")) {
				while (reader.Read())
					yield return ListInfoFromReader(reader);
			}
		}

		public class WordSearchResult {
			public string Phrase { get; set; }
			public string Translation { get; set; }
			public string SetName { get; set; }
			public long SetID { get; set; }
			public int ListPosition { get; set; }
		}

		public IEnumerable<WordSearchResult> SearchAllEntries(string query) {
			var sb = new StringBuilder();

			if (string.IsNullOrEmpty(query)) {
				sb.Append("%");
			} else {
				sb.Append("%");
				foreach (char c in query) {
					switch (c) {
						//']' on its own doesn't need to be escaped.
						case '%':
						case '_':
						case '[':
							sb.Append('[').Append(c).Append(']');
							break;
						default:
							sb.Append(c);
							break;
					}
				}
				sb.Append("%");
			}

			using (var reader = this.SelectReader(
				"TYPES Integer, Text, Text, Text, Integer;" +
				"SELECT SetID, Name, Phrase, Translation, ListPosition FROM VocabItems JOIN Sets ON (VocabItems.SetID = Sets.id)" +
				"WHERE Phrase LIKE ? OR Translation LIKE ? ORDER BY SetID ASC, Phrase ASC", sb.ToString(), sb.ToString())) {

				while (reader.Read()) {
					var wsr = new WordSearchResult();
					wsr.SetID = reader.GetInt64(0);
					wsr.SetName = reader.GetString(1);
					wsr.Phrase = reader.GetString(2);
					wsr.Translation = reader.GetString(3);
					wsr.ListPosition = reader.GetInt32(4);

					yield return wsr;
				}
			}
		}

		//This function also raised the ListDeleted event on the WordList, if one exists.
		public void DeleteWordList(long setID) {
			NullWeakReference<SqliteWordList> wlr;
			if (wordLists.TryGetValue(setID, out wlr))
				wordLists.Remove(setID);

			using (var txn = Connection.BeginTransaction()) {
				ExecuteSQL("DELETE FROM Sets WHERE id = ?", setID);
				ExecuteSQL("DELETE FROM VocabItems WHERE SetID = ?", setID);
				ExecuteSQL("DELETE FROM SetProperties WHERE SetID = ?", setID);
				ExecuteSQL("DELETE FROM SetMemberships WHERE ChildID = ? OR ParentID = ?", setID, setID);

				txn.Commit();
			}

			var handler = WordListDeleted;
			if (handler != null)
				handler(this, new WordListDeletedEventArgs { SetID = setID });

			if (wlr != null) {
				var wl = wlr.Target;
				if (wl != null)
					wl.RaiseDeleted();
			}
		}

		//Note: there's also a ListDeleted event on the WordList itself, which may be preferable.
		public event EventHandler<WordListDeletedEventArgs> WordListDeleted;

		public List<PracticeItem> GetItems(IList<ListSearchResult> items) {
			if (items.Count == 0)
				return new List<PracticeItem>();

			var results = new List<PracticeItem>();

			using (var txn = Connection.BeginTransaction()) {
				var query = new StringBuilder(
					@"SELECT SetID, Phrase, Translation
				      FROM VocabItems
				      WHERE SetID IN (");

				bool hasListItems = false;

				foreach (var r in items) {
					if (r.Position == null) {
						query.Append(r.SetID).Append(", ");
						hasListItems = true;
					}
				}

				if (hasListItems) {
					// Remove the final comma
					query.Length -= 2;
					query.Append(@") ORDER BY SetID, ListPosition;");

					using (var cmd = Connection.CreateCommand()) {
						cmd.CommandText = query.ToString();

						GetResults(cmd, results);
					}
				}

				using (var cmd = Connection.CreateCommand()) {
					cmd.CommandText =
						@"SELECT SetID, Phrase, Translation
					      FROM VocabItems
					      WHERE SetID = ? AND ListPosition = ?";

					var setID = cmd.CreateParameter();
					var pos = cmd.CreateParameter();

					cmd.Parameters.Add(setID);
					cmd.Parameters.Add(pos);

					foreach (var r in items) {
						if (r.Position == null)
							continue;
						else
							pos.Value = r.Position.Value;

						setID.Value = r.SetID;

						GetResults(cmd, results);
					}
				}
			}

			return results;
		}

		// Call this from inside a transaction.
		// The command should return the results as such:
		// SetID, Phrase, Translation
		void GetResults(DbCommand command, ICollection<PracticeItem> output) {
			using (var reader = command.ExecuteReader()) {
				if (reader.FieldCount != 3)
					throw new System.Data.StrongTypingException("Practice query should only return 3 columns");

				if (reader.IsDBNull(0) || reader.IsDBNull(1) || reader.IsDBNull(2))
					throw new System.Data.StrongTypingException("Practice query returned a null value in one of the columns");

				while (reader.Read()) {
					var item = new PracticeItem() {
						SetID = reader.GetInt64(0),
						Phrase = reader.GetString(1),
						Translation = reader.GetString(2)
					};

					output.Add(item);
				}

			}
		}

		// Using yield return here would probably be a bad idea, since the connection
		// would only be closed if the consumer consumes the whole list.
		/// <summary>Returns the ListInfo instances for those items which were lists and not single vocab items.</summary>
		public List<ListInfo> GetListInformation(List<ListSearchResult> lists) {
			var answer = new List<ListInfo>();

			using (var txn = Connection.BeginTransaction()) {
				using (var command = Connection.CreateCommand()) {
					command.CommandText =
						@"TYPES Integer, Text, Text, Text, Text, Date, Integer; 
						 SELECT id, Name, Author, Language, Url, Created, 
						     (SELECT count(*) FROM VocabItems WHERE SetID = Sets.id)
						 FROM Sets
						 WHERE id = ?";

					var param = command.CreateParameter();

					foreach (var list in lists) {
						if (list.Position.HasValue)
							continue;

						param.Value = list.SetID;

						using (var reader = command.ExecuteReader())
							while (reader.Read())
								answer.Add(ListInfoFromReader(reader));
					}
				}
			}

			return answer;
		}
	}
}