using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
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

		protected string Path {
			get { return path; }
            set {
                conn.Close();
                System.IO.File.Move(path, value);
                path = value;
                conn.ConnectionString = "Data Source=" + path;
                conn.Open();
            }
		}

		private void Init() {
            ExecuteSQL("PRAGMA foreign_keys = ON");

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
            using (var txn = conn.BeginTransaction()) {
                if (fromVersion >= toVersion)
                    throw new ArgumentException("Can't downgrade the application database. What happen?", "fromVersion");

                for (; fromVersion < toVersion; ++fromVersion)
                    IncrementalUpgradeSchema(fromVersion + 1);


                txn.Commit();
            }
		}

        protected object GetMetadata(string name, object defaultValue) {
            return Select(@"SELECT Value FROM Info WHERE Name = ?", name) ?? defaultValue;
        }

        protected void SetMetadata(string name, object value) {
            if (Select(@"SELECT Value FROM Info WHERE Name = ?", name) == null)
                ExecuteSQL(@"INSERT INTO Info (Name, Value) VALUES (?, ?)", name, value.ToString());
            else
                ExecuteSQL(@"UPDATE Info SET Value = ? WHERE Name = ?", value.ToString(), name);
        }
        
		protected override void Dispose(bool disposing) {
			if (disposing)
				conn.Dispose();

			base.Dispose(disposing);
		}
	}

	public abstract class SqliteObject : IDisposable {
		protected DbConnection conn;
        bool ownsConnection;

		public SqliteObject(SqliteObject other) {
			this.conn = other.conn;
            ownsConnection = false;
		}

		protected SqliteObject(DbConnection connection) {
			this.conn = connection;
            ownsConnection = true;
		}

        protected static DbConnection OpenDatabase(string path) {
            string dir = IO.Path.GetDirectoryName(path);
            if (!IO.Directory.Exists(dir))
                IO.Directory.CreateDirectory(dir);
#if !MONO
            return new System.Data.SQLite.SQLiteConnection("Data Source=" + path);
#else
			return new Mono.Data.Sqlite.SqliteConnection("Data Source=" + path);
#endif
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

		// Returns the number of rows affected.
		protected int ExecuteSQL(string sql, params object[] parameters) {
			using (DbCommand command = conn.CreateCommand()) {
				command.CommandText = sql;

				foreach (object p in parameters)
					AddParameter(command, p);

				return command.ExecuteNonQuery();
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
            if(ownsConnection)
                conn.Dispose();
		}
	}

    public class WordListEventArgs : EventArgs {
        public long SetID { get; private set; }

        public WordListEventArgs(long id) {
            SetID = id;
        }
    };

	public class SqliteDataStore : SqliteDatabase {
		Dictionary<long, NullWeakReference<SqliteWordList>> wordLists = new Dictionary<long, NullWeakReference<SqliteWordList>>();

		public SqliteDataStore(string path)
			: base(path) {
		}

		protected override int ApplicationSchemaVersion() {
			return 4;
		}

		protected override void IncrementalUpgradeSchema(int toVersion) {
			switch (toVersion) {
				case 1: InitDatabase(); break;
				case 2: UpgradePracticeSchema(); break;
                case 3: AddAccessedColumn(); break;
                case 4: AddTagsTable(); break;
				default: throw new ArgumentOutOfRangeException("toVersion");
			}
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

				CREATE TABLE SetProperties (SetID INTEGER NOT NULL, Property TEXT, Value TEXT, FOREIGN KEY (SetID) REFERENCES Sets(id));
				CREATE INDEX SetProperties_Index ON SetProperties (SetID, Property);");
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
					ListPosition INTEGER NOT NULL,
                    FOREIGN KEY (SetID) REFERENCES Sets(id));
				
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
					Result INTEGER NOT NULL, -- 1 for success, 0 for failure
                    FOREIGN KEY(SetID) REFERENCES Sets(id)
					);

				-- Are these necessary?
				CREATE INDEX PracticeHistory_IndexP ON PracticeHistory (Phrase);
				CREATE INDEX PracticeHistory_IndexT ON PracticeHistory (Translation);
				CREATE INDEX PracticeHistory_IndexPTS ON PracticeHistory (Phrase, Translation, SetID);
				CREATE INDEX PracticeHistory_IndexC ON PracticeHistory (Created);");
		}

        private void AddAccessedColumn() {
            ExecuteSQL(@"
                ALTER Table SETS
                ADD COLUMN Accessed DATE");
        }

        private void AddTagsTable() {
            ExecuteSQL(@"
                CREATE TABLE Tags (
                    tag TEXT NOT NULL,
                    SetID INTEGER NOT NULL,
                    FOREIGN KEY(SetID) REFERENCES Sets(id)
                );");
        }

        #region Tags
        public IEnumerable<KeyValuePair<string, int>> GetTags(bool orderByName = true) {
            var reader = SelectReader("SELECT tag AS t, count(SetID) AS n FROM Tags GROUP BY tag ORDER BY " + (orderByName ? "t ASC" : "n DESC, t ASC"));
            while (reader.Read())
                yield return new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1));
        }

        public IEnumerable<ListInfo> SearchByTag(string tag) {
            var reader = SelectReader(@"
                SELECT id, Name, Author, Language, Url, Created, Accessed, (SELECT count(*) FROM VocabItems WHERE SetID = id) 
                FROM Tags INNER JOIN Sets ON Tags.SetID = Sets.id
                WHERE tag = ?
                ORDER BY Name ASC", tag);
            while (reader.Read())
                yield return ListInfoFromReader(reader);
        }

        public IEnumerable<string> GetTags(long setID) {
            var reader = SelectReader("SELECT tag FROM Tags WHERE SetID = ?", setID);
            while (reader.Read())
                yield return reader.GetString(0);
        }

        public bool HasTag(string tag, long setID) {
            return Convert.ToInt64(Select("SELECT count(*) FROM Tags WHERE tag = ? AND SetID = ?", tag, setID)) > 0;
        }

        public void Tag(string tag, long setID) {
            // Need to check this here or we may violate the uniqueness/foreign key constraint.
            if(!HasTag(tag, setID) && WordListExists(setID))
                ExecuteSQL("INSERT INTO Tags (tag, SetID) VALUES (?, ?)", tag, setID);
        }

        public void Untag(string tag, long setID) {
            ExecuteSQL("DELETE FROM Tags WHERE tag = ? AND SetID = ?", tag, setID);
        }
        #endregion

        /// <param name="setID">The SetID of the word list</param>
		/// <returns>An existing SqliteWordList instance, if one exists, otherwise a newly-created SqliteWordList.</returns>
		public SqliteWordList GetWordList(long setID) {
			SqliteWordList wl = null;
			NullWeakReference<SqliteWordList> list;

			if (wordLists.TryGetValue(setID, out list))
				wl = list.Target;

			if (wl != null)
				return wl;

			wl = SqliteWordList.FromSetID(this, setID);
            if (wl == null)
                return null;

			wordLists[setID] = new NullWeakReference<SqliteWordList>(wl);
			return wl;
		}

        public bool WordListExists(long id) {
            return GetWordList(id) != null;
        }

		public SqliteWordList CreateSet(string name, string author, string language, string url, DateTime? date) {
			long setID;

			using (var txn = Connection.BeginTransaction()) {
				ExecuteSQL("INSERT INTO Sets (Name, Author, Language, Url, Created) VALUES (?, ?, ?, ?, ?)",
					name, author, language, url, date.HasValue ? (object)date.Value : (object)DBNull.Value);

				setID = GetLastInsertRowID();
				txn.Commit();
			}

			var wl = SqliteWordList.FromSetID(this, setID);
            if (wl == null)
                return null;
			wordLists[setID] = new NullWeakReference<SqliteWordList>(wl);
			return wl;
		}

        public IEnumerable<ListInfo> GetRecentSets(int limit) {
            var reader = SelectReader(@"
                TYPES Integer, Text, Text, Text, Text, Date, Date, Integer; 
                SELECT id, Name, Author, Language, Url, Created, Accessed, (SELECT count(*) FROM VocabItems WHERE SetID = Sets.id)
                FROM Sets
                WHERE Accessed NOT NULL
                ORDER BY Accessed DESC
                LIMIT " + limit.ToString());

            while (reader.Read()) {
                yield return ListInfoFromReader(reader);
            }
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
                list.Accessed = reader.GetDateTime(6);
            if (!reader.IsDBNull(7))
                list.TermCount = reader.GetInt64(7);

			return list;
		}

		public IEnumerable<ListInfo> GetAllSets() {
			using (var reader = this.SelectReader(@"
                TYPES Integer, Text, Text, Text, Text, Date, Date, Integer; 
                SELECT id, Name, Author, Language, Url, Created, Accessed,
                    (SELECT count(*) FROM VocabItems WHERE SetID = Sets.id) FROM Sets ORDER BY Name ASC")) {
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

		public bool UpdateWordListEntry(long setID, string oldPhrase, string oldTranslation, 
			string newPhrase, string newTranslation) {
		
            // Modifying the database directly doesn't notify existing word lists of the change.
            // This solution does so with less work.
            var list = GetWordList(setID);

            bool found = false;
            foreach (var item in list) {
                if (item.Phrase == oldPhrase && item.Translation == oldTranslation) {
                    item.Phrase = newPhrase;
                    item.Translation = newTranslation;
                    found = true;
                    // Don't stop yet; there may be more identical items.
                }
            }

            return found;
		}

        public void AddPracticeHistory(long setID, string phrase, string translation, bool correct) {
            ExecuteSQL(@"INSERT INTO PracticeHistory (SetID, Phrase, Translation, Result, Created)
                         VALUES (?, ?, ?, ?, datetime('now'))", setID, phrase, translation, correct ? 1 : 0);
        }

        public event EventHandler<WordListEventArgs> WordListAccessed;

        public void RaiseWordListOpened(long? id) {
            if (!id.HasValue)
                return;

            var handler = WordListAccessed;
            if (handler != null)
                handler(this, new WordListEventArgs(id.Value)); 
        }

		//This function also raised the ListDeleted event on the WordList, if one exists.
		public void DeleteWordList(long setID) {
			NullWeakReference<SqliteWordList> wlr;
			if (wordLists.TryGetValue(setID, out wlr))
				wordLists.Remove(setID);

			using (var txn = Connection.BeginTransaction()) {
				ExecuteSQL("DELETE FROM VocabItems WHERE SetID = ?", setID);
				ExecuteSQL("DELETE FROM SetProperties WHERE SetID = ?", setID);
                ExecuteSQL("DELETE FROM PracticeHistory WHERE SetID = ?", setID);
                ExecuteSQL("DELETE FROM Tags WHERE SetID = ?", setID);
                ExecuteSQL("DELETE FROM Sets WHERE id = ?", setID);

				txn.Commit();
			}

			var handler = WordListDeleted;
			if (handler != null)
				handler(this, new WordListEventArgs(setID));

			if (wlr != null) {
				var wl = wlr.Target;
				if (wl != null)
					wl.RaiseDeleted();
			}
		}

		//Note: there's also a ListDeleted event on the WordList itself, which may be preferable.
		public event EventHandler<WordListEventArgs> WordListDeleted;

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

				while (reader.Read()) {
					if (reader.IsDBNull(0) || reader.IsDBNull(1) || reader.IsDBNull(2))
						throw new System.Data.StrongTypingException("Practice query returned a null value in one of the columns");

					var item = new PracticeItem(
						reader.GetInt64(0),
						reader.GetString(1),
						reader.GetString(2));

					output.Add(item);
				}

			}
		}

		// Using yield return here would probably be a bad idea, since the connection
		// would only be closed if the consumer consumes the whole list.
		/// <summary>Returns the ListInfo instances for those items which were lists and not single vocab items.</summary>
		public List<ListInfo> GetListInformation(IEnumerable<ListSearchResult> lists) {
			var answer = new List<ListInfo>();

			using (var txn = Connection.BeginTransaction()) {
				using (var command = Connection.CreateCommand()) {
					command.CommandText =
						@"TYPES Integer, Text, Text, Text, Text, Date, Date, Integer; 
						 SELECT id, Name, Author, Language, Url, Created, Accessed, 
						     (SELECT count(*) FROM VocabItems WHERE SetID = Sets.id)
						 FROM Sets
						 WHERE id = ?";

					var param = command.CreateParameter();
                    command.Parameters.Add(param);

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

        private List<PracticeItem> GetPracticeItems() {
            var reader = this.SelectReader(@"
                SELECT VocabItems.Phrase, VocabItems.Translation, VocabItems.SetID, Result, Created
                FROM VocabItems LEFT JOIN PracticeHistory
                    ON PracticeHistory.SetID = VocabItems.SetID 
                    AND PracticeHistory.Phrase = VocabItems.Phrase 
                    AND PracticeHistory.Translation = VocabItems.Translation  
                ORDER BY VocabItems.SetID, VocabItems.Phrase, VocabItems.Translation, Created");

            var items = new List<PracticeItem>();
            PracticeItem item = null;

            while (reader.Read()) {
                string phrase = reader.GetString(0);
                string translation = reader.GetString(1);
                long setID = reader.GetInt64(2);
                bool? correct = reader.IsDBNull(3) ? (bool?)null : reader.GetBoolean(3);
                DateTime? created = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);

                if (item == null || item.Phrase != phrase || item.Translation != translation) {
                    item = new PracticeItem(setID, phrase, translation, new PracticeHistory());
                    items.Add(item);
                }

                if (correct != null && created != null)
                    item.History.Add(created.Value, correct.Value);
            }

            return items;
        }

        public List<ListInfo> GetSetsByPracticeHistory() {
            var items = from g in (from x in GetPracticeItems()
                                   group x by x.SetID into set
                                   select new { Set = set, Score = set.Average(x => Math.Pow(x.History.Importance, 2)) })
                        orderby g.Score descending
                        select g.Set.Key;

            return GetListInformation(from x in items select new ListSearchResult(x));
        }

        public class Duplicate {
            public long SetID { get; set; }
            public string Phrase { get; set; }
            public string Translation { get; set; }
            public string ListName { get; set; }
            public int PracticeCount { get; set; }
        }

        public IEnumerable<KeyValuePair<Duplicate, Duplicate>> FindDuplicateListItems() {
            var reader = SelectReader(@"
                CREATE TEMP VIEW IF NOT EXISTS DupL AS Select V.SetID, V.Phrase, V.Translation, V.ListPosition, S.Name, (SELECT count(*) FROM PracticeHistory AS P WHERE P.SetID = V.SetID AND P.Phrase = V.Phrase AND P.Translation = V.Translation) as PracticeCount FROM VocabItems AS V JOIN Sets AS S ON V.SetID = S.ID;
                CREATE TEMP VIEW IF NOT EXISTS DupR AS Select V.SetID, V.Phrase, V.Translation, V.ListPosition, S.Name, (SELECT count(*) FROM PracticeHistory AS P WHERE P.SetID = V.SetID AND P.Phrase = V.Phrase AND P.Translation = V.Translation) as PracticeCount FROM VocabItems AS V JOIN Sets AS S ON V.SetID = S.ID;                
                SELECT L.SetID, L.Phrase, L.Translation, L.Name, L.PracticeCount, R.SetID, R.Phrase, R.Translation, R.Name, R.PracticeCount FROM DupL AS L JOIN DupR AS R ON (L.Phrase = R.Phrase OR L.Translation = R.Translation) AND (L.SetID < R.SetID OR (L.SetID = R.SetID AND L.ListPosition < R.ListPosition))");
            while (reader.Read()) {
                var left = new Duplicate {
                    SetID = reader.GetInt64(0),
                    Phrase = reader.GetString(1),
                    Translation = reader.GetString(2),
                    ListName = reader.GetString(3),
                    PracticeCount = reader.GetInt32(4),
                };
                var right = new Duplicate {
                    SetID = reader.GetInt64(5),
                    Phrase = reader.GetString(6),
                    Translation = reader.GetString(7),
                    ListName = reader.GetString(8),
                    PracticeCount = reader.GetInt32(9),
                };
                yield return new KeyValuePair<Duplicate, Duplicate>(left, right);
            }
        }

        public List<PracticeItem> GetSuggestedPracticeItems(int limit) {
            var items = GetPracticeItems();

            // Results in the items with the highest importance being placed first in the list.
            items.Sort(new Comparison<PracticeItem>((a, b) => -a.History.Importance.CompareTo(b.History.Importance)));

            var chosen = new List<PracticeItem>();
            var rng = new Random();
            var distribution = new NormalDistribution(rng);
            while (chosen.Count < limit && items.Count > 0) {
                double x = Math.Abs(distribution.NextDouble(3) / 3);
                // ~68% chance that x < 1/3
                // ~95% chance that x < 2/3
                int index = (int)Math.Floor(items.Count * x);
                if (index >= items.Count)
                    index--;
                chosen.Add(items[index]);
                items.RemoveAt(index);
            }
            chosen.Shuffle(rng);

            return chosen;
        }
    }

    // A normal distribution with a mean of 0 and standard deviation of 1.
    class NormalDistribution {
        Random random;
        double? next;

        public NormalDistribution(Random random) {
            this.random = random;
        }

        // Uses the Box-Muller transform to generate two independent and normally distributed pseudorandom numbers.
        // One is returned; the other is saved for the next call.
        public double NextDouble() {
            if (next.HasValue) {
                double d = next.Value;
                next = null;
                return d;
            }

            double u1 = random.NextDouble(), u2 = random.NextDouble();
            double r = Math.Sqrt(-2 * Math.Log(u1));
            double theta = 2 * Math.PI * u2;

            next = r * Math.Sin(theta);
            return r * Math.Cos(theta);
        }

        // Only produces pseudorandom numbers with an absolute value less than or equal to the given amount.
        // Theoretically, this could take an infinite amount of time to generate a number. Approximately 68%
        // of numbers generated are less than 1, 95% less than 2, 99.7% less than 3, so it should not pose a problem.
        public double NextDouble(double max) {
            double d;
            do {
                d = NextDouble();
            } while (Math.Abs(d) > max);
            return d;
        }
    }
}