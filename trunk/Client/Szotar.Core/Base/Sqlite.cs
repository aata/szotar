using System;
using System.Data.Common;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace Szotar.Sqlite {
	static class SqliteFactory {
		static Type type;
		static Assembly assembly;

		public static DbConnection CreateConnection(string path) {
			Reflect();

			DbConnection conn = (DbConnection)Activator.CreateInstance(type);
			conn.ConnectionString = "Data Source=" + path;
			return conn;
		}

		private static void Reflect() {
			if (type != null)
				return;

			string relativePath;
			string typeName;

			switch (Environment.OSVersion.Platform) {
				case PlatformID.Win32NT:
					if (IntPtr.Size == 8) {
						//IA64 is currently not supported by Szótár.
						relativePath = "Dependencies/x64/System.Data.SQLite.dll";
					} else {
						relativePath = "Dependencies/x86/System.Data.SQLite.dll";
					}

					string exePath = Assembly.GetExecutingAssembly().Location;
					string dependenciesDirectory;
					if(!string.IsNullOrEmpty(exePath))
						dependenciesDirectory = Path.GetDirectoryName(exePath);
					else
						dependenciesDirectory = Environment.CurrentDirectory;
					
					string path = Path.Combine(dependenciesDirectory, relativePath);
					assembly = Assembly.LoadFile(path);
					
					typeName = "System.Data.SQLite.SQLiteConnection";
					break;
				default:
					//FIXME Should probably use full assembly name
					assembly = Assembly.LoadWithPartialName("Mono.Data.SqliteClient");
					typeName = "Mono.Data.SqliteClient.SqliteConnection";
					break;
			}
			
			type = assembly.GetType(typeName);
		}
	}

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
			: base(SqliteFactory.CreateConnection(path)) {
			this.path = path;
			conn.Open();

			Init();
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

				using (var cmd = conn.CreateCommand()) {
					cmd.CommandText = "INSERT INTO Info (Name, Value) VALUES ('Version', ?)";
					var param = cmd.CreateParameter();
					cmd.Parameters.Add(param);
					param.Value = version.ToString();
					cmd.ExecuteNonQuery();
				}

				txn.Commit();
			}
		}

		protected void UpgradeSchemaInIncrements(int fromVersion, int toVersion) {
			if (fromVersion >= toVersion)
				throw new ArgumentException(new ArgumentException().Message, "fromVersion");

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

		protected long GetLastInsertRowID() {
			var lastInsertCommand = conn.CreateCommand();
			lastInsertCommand.CommandText = "SELECT last_insert_rowid()";

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

				return command.ExecuteScalar();
			}
		}
		
		protected DbDataReader SelectReader(string sql, params object[] parameters) {
			using (DbCommand command = conn.CreateCommand()) {
				command.CommandText = sql;

				foreach (object p in parameters) {
					var param = command.CreateParameter();
					param.Value = p;
					command.Parameters.Add(p);
				}

				return command.ExecuteReader();
			}
		}

		protected void AddParameter(DbCommand command, object value) {
			var param = command.CreateParameter();
			param.Value = value;
			command.Parameters.Add(param);
		}

		#region Accessors
		protected DbConnection Connection { get { return conn; } }
		#endregion

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
		}
	}
	
	public class SqliteDictionary : SqliteDatabase, IBilingualDictionary {
		protected override void UpgradeSchema(int fromVersion, int toVersion) {
			UpgradeSchemaInIncrements(fromVersion, toVersion);
		}

		protected override void IncrementalUpgradeSchema(int version) {
			switch (version) {
				case 1:
					InitialiseDatabase();
					break;
				default:
					//FIXME Resources.Errors.CannotUpgradeDatabaseToVersion
					throw new ArgumentException(string.Format("Can't upgrade to database version {0}", version), "version");
			}
		}

		private void InitialiseDatabase() {
			using (var cmd = Connection.CreateCommand()) {
				cmd.CommandText = "CREATE TABLE Entries(id INTEGER PRIMARY KEY AUTOINCREMENT, Section INTEGER NOT NULL, Phrase TEXT NOT NULL, PhraseWithoutAccents TEXT)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE INDEX Entries_Phrase ON Entries (Phrase, Section)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE INDEX Entries_PhraseWithoutAccents ON Entries (PhraseWithoutAccents, Section)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE TABLE Translations(id INTEGER PRIMARY KEY AUTOINCREMENT, phraseID INTEGER NOT NULL, Value TEXT NOT NULL)";
				cmd.ExecuteNonQuery();
			}
		}

		protected override int ApplicationSchemaVersion() {
			return 1;
		}

		public SqliteDictionary(string path) : base(path) { }

		public SqliteDictionary(string path, IDictionarySection forward, IDictionarySection backward)
			: base(path) {
			using (var txn = Connection.BeginTransaction()) {
				InsertEntriesFromSource(ForwardSectionID, forward);
				InsertEntriesFromSource(BackwardSectionID, forward);

				txn.Commit();
			}

			ForwardsSection = new Section(this, ForwardSectionID);
			ReverseSection = new Section(this, BackwardSectionID);
		}

		protected void InsertEntriesFromSource(int section, IDictionarySection source) {
			using (var insertEntryCommand = Connection.CreateCommand()) {
				insertEntryCommand.CommandText = "INSERT INTO Entries (Section, Phrase, PhraseWithoutAccents) VALUES (?, ?, ?)";
				insertEntryCommand.Parameters.Add(insertEntryCommand.CreateParameter());
				insertEntryCommand.Parameters.Add(insertEntryCommand.CreateParameter());
				insertEntryCommand.Parameters.Add(insertEntryCommand.CreateParameter());
				insertEntryCommand.Parameters[0].Value = section;

				using (var insertTranslationCommand = Connection.CreateCommand()) {
					insertTranslationCommand.CommandText = "INSERT INTO TRANSLATIONS(phraseID, Value) VALUES (?, ?)";
					insertTranslationCommand.Parameters.Add(insertTranslationCommand.CreateParameter());
					insertTranslationCommand.Parameters.Add(insertTranslationCommand.CreateParameter());
					foreach (Entry e in source) {
						insertEntryCommand.Parameters[1].Value = e.Phrase;
						insertEntryCommand.Parameters[2].Value = e.PhraseNoAccents;

						insertEntryCommand.ExecuteNonQuery();
						insertTranslationCommand.Parameters[0].Value = this.GetLastInsertRowID();

						foreach (Translation t in e.Translations) {
							insertTranslationCommand.Parameters[1].Value = t.Value;
							insertTranslationCommand.ExecuteNonQuery();
						}
					}
				}
			}
		}

		public string GetStringOption(string name) {
			if (TableExists("Info")) {
				using (var cmd = Connection.CreateCommand()) {
					cmd.CommandText = "TYPES text; SELECT Info.Value FROM Info WHERE Info.Name = ?";
					cmd.Parameters.Add(cmd.CreateParameter());
					cmd.Parameters[0].Value = name;

					return (string)cmd.ExecuteScalar();
				}
			} else {
				return null;
			}
		}

		public void SetStringOption(string name, string value) {
			using (var cmd = Connection.CreateCommand()) {
				cmd.CommandText = "UPDATE Info SET Info.Value = ? WHERE Info.Name = ?";
				cmd.Parameters.Add(cmd.CreateParameter());
				cmd.Parameters[0].Value = value;
				cmd.Parameters.Add(cmd.CreateParameter());
				cmd.Parameters[0].Value = name;

				cmd.ExecuteNonQuery();
			}
		}

		protected static int ForwardSectionID {
			get { return 0; }
		}

		protected static int BackwardSectionID {
			get { return 1; }
		}

		public IDictionarySection ForwardsSection { get; private set; }
		public IDictionarySection ReverseSection { get; private set; }

		public string Name {
			get { return GetStringOption("Name"); }
			set { SetStringOption("Name", value); }
		}

		public string Author {
			get { return GetStringOption("Author"); }
			set { SetStringOption("Author", value); }
		}

		string IBilingualDictionary.Path {
			get { return base.Path; }
			set { throw new InvalidOperationException(); }
		}

		public string FirstLanguage {
			get { return GetStringOption("FirstLanguage"); }
			set { SetStringOption("FirstLanguage", value); }
		}

		public string SecondLanguage {
			get { return GetStringOption("SecondLanguage"); }
			set { SetStringOption("SecondLanguage", value); }
		}

		public string FirstLanguageReverse {
			get { return GetStringOption("FirstLanguageReverse"); }
			set { SetStringOption("FirstLanguageReverse", value); }
		}

		public string SecondLanguageReverse {
			get { return GetStringOption("SecondLanguageReverse"); }
			set { SetStringOption("SecondLanguageReverse", value); }
		}

		public string FirstLanguageCode {
			get { return GetStringOption("FirstLanguageCode"); }
			set { SetStringOption("FirstLanguageCode", value); }
		}

		public string SecondLanguageCode {
			get { return GetStringOption("SecondLanguageCode"); }
			set { SetStringOption("SecondLanguageCode", value); }
		}

		void IBilingualDictionary.Save() {
			//Nothing needed.
		}

		public class Section : IDictionarySection {
			SqliteDictionary dict;
			int sectionID;

			public Section(SqliteDictionary dict, int sectionID) {
				if (sectionID != SqliteDictionary.BackwardSectionID && sectionID != SqliteDictionary.ForwardSectionID)
					throw new ArgumentException();

				this.dict = dict;
				this.sectionID = sectionID;
			}

			private string MakeOptionName(string baseName) {
				return baseName + sectionID;
			}

			public string Name {
				get { return dict.GetStringOption(MakeOptionName("SectionName")); }
				set { dict.SetStringOption(MakeOptionName("SectionName"), value); }
			}

			public string Author {
				get { return dict.GetStringOption(MakeOptionName("SectionAuthor")); }
				set { dict.SetStringOption(MakeOptionName("SectionAuthor"), value); }
			}

			public int HeadWords {
				get {
					using (DbCommand cmd = dict.Connection.CreateCommand()) {
						cmd.CommandText = "SELECT Count(*) FROM Info";
						return Convert.ToInt32(cmd.ExecuteScalar());
					}
				}
			}

			public IEnumerable<SearchResult> Search(string terms, bool ignoreAccents, bool ignoreCase) {
				return new List<SearchResult>(new SearchResult[] { new SearchResult(new Entry("hello", new List<Translation>(new Translation[] { new Translation("szervusz") })), MatchType.NormalMatch) });
			}

			public IEnumerator<Entry> GetEnumerator() {
				return new List<Entry>(new Entry[] { new Entry("to try sg out", new List<Translation>(new Translation[] { new Translation("kiprobalni") })) }).GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return new Entry[] { new Entry("to try sg out", new List<Translation>(new Translation[] { new Translation("kiprobalni") })) }.GetEnumerator();
			}

			Entry IDictionarySection.GetFullEntry(Entry entry) {
				return entry;
			}
		}
	}

	public class SqliteDataStore : SqliteDatabase {
		Dictionary<long, NullWeakReference<SqliteWordList>> wordLists = new Dictionary<long, NullWeakReference<SqliteWordList>>();

		public SqliteDataStore(string path)
			: base(path) {
		}

		protected override void UpgradeSchema(int fromVersion, int toVersion) {
			UpgradeSchemaInIncrements(fromVersion, toVersion);
		}

		protected override int ApplicationSchemaVersion() {
			return 1;
		}

		protected override void IncrementalUpgradeSchema(int toVersion) {
			if (toVersion == 1)
				InitDatabase();
		}

		private void InitDatabase() {
			ExecuteSQL("CREATE TABLE VocabItems (id INTEGER PRIMARY KEY AUTOINCREMENT, Phrase TEXT NOT NULL, Translation TEXT NOT NULL, SetID INTEGER NOT NULL, ListPosition INTEGER NOT NULL, TimesTried INTEGER NOT NULL, TimesFailed INTEGER NOT NULL)");
			ExecuteSQL("CREATE INDEX VocabItems_IndexP ON VocabItems (Phrase)");
			ExecuteSQL("CREATE INDEX VocabItems_IndexT ON VocabItems (Translation)");
			ExecuteSQL("CREATE INDEX VocabItems_IndexS ON VocabItems (SetID)");
			ExecuteSQL("CREATE INDEX VocabItems_IndexSO ON VocabItems (SetID, ListPosition)");
			//ExecuteSQL("CREATE INDEX VocabItems_IndexK ON VocabItems (Knowledge)");

			ExecuteSQL("CREATE TABLE Sets (id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Author TEXT, Language TEXT)");
			ExecuteSQL("CREATE INDEX Sets_Index ON Sets (id)");

			ExecuteSQL("CREATE TABLE SetProperties (SetID INTEGER NOT NULL, Property TEXT, Value TEXT)");
			ExecuteSQL("CREATE INDEX SetProperties_Index ON SetProperties (SetID, Property)");

			ExecuteSQL("CREATE TABLE SetMemberships (ChildID INTEGER NOT NULL, ParentID INTEGER NOT NULL)");
			ExecuteSQL("CREATE INDEX SetMemberships_Index ON SetMemberships (ChildID, ParentID)");
		}

		/// <param name="setID">The SetID of the word list</param>
		/// <returns>An existing SqliteWordList instance, if one exists, otherwise a newly-created SqliteWordList.</returns>
		public SqliteWordList GetWordList(long setID) {
			SqliteWordList wl = null;
			NullWeakReference<SqliteWordList> list;
			if (wordLists.TryGetValue(setID, out list)) {
				wl = list.Target;
			}

			if(wl != null)
				return wl;

			wl = new SqliteWordList(this, setID);
			wordLists[setID] = new NullWeakReference<SqliteWordList>(wl);
			return wl;
		}

		public long CreateSet(string name, string author, string language) {
			using (var txn = Connection.BeginTransaction()) {
				ExecuteSQL("INSERT INTO Sets (Name, Author, Language) VALUES (?, ?, ?)",
					name, author, language);

				txn.Commit();
			}

			return Convert.ToInt32(GetLastInsertRowID());
		}
	}
}