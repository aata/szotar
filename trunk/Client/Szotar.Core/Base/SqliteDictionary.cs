using System;
using System.Data;
using System.Data.SQLite;
using System.Data.Common;
using System.Collections.Generic;

namespace Szotar {
    static class DbExtensions {
        public static DbCommand AddParam(this DbCommand command, ref DbParameter param) {
            command.Parameters.Add(param = command.CreateParameter());
            return command;
        }
    }

    public class SqliteDictionary : Sqlite.SqliteDatabase, IBilingualDictionary {
        Section forwardsSection, reverseSection;

        List<DbCommand> commands;

        protected SqliteDictionary(string path)
            : base(path) {
            forwardsSection = new Section(this, true);
            reverseSection = new Section(this, false);

            InitCommands();
        }

        static Dictionary<string, NullWeakReference<SqliteDictionary>> openDicts
            = new Dictionary<string,NullWeakReference<SqliteDictionary>>();

        public static SqliteDictionary FromPath(string path) {
            // Handle cases where paths are not normalized.
            path = System.IO.Path.GetFullPath(path);

            NullWeakReference<SqliteDictionary> weakRef;
            if(openDicts.TryGetValue(path, out weakRef) && weakRef.IsAlive && weakRef.Target.Connection.State == ConnectionState.Open)
                return weakRef.Target;

            var dict = new SqliteDictionary(path);
            openDicts[path] = new NullWeakReference<SqliteDictionary>(dict);
            return dict;
        }

        public void AddEntries(IList<Entry> forwards, IList<Entry> backwards, Action<int, int> notify = null) {
            if (notify == null) {
                forwardsSection.AddEntries(forwards, null);
                reverseSection.AddEntries(backwards, null);
            } else {
                forwardsSection.AddEntries(forwards, (i, n) => notify(i, n + backwards.Count));
                reverseSection.AddEntries(backwards, (i, n) => notify(i + forwards.Count, n + forwards.Count));
            }
        }

        protected override void  Dispose(bool disposing) {
            foreach (var cmd in commands)
                cmd.Dispose();

 	        base.Dispose(disposing);
        }

        #region Properties
        string name;
        public string Name {
            get {
                if (name != null)
                    return name;
                return name = GetMetadata("Name", "").ToString();
            }
            set {
                SetMetadata("Name", name = value);
            }
        }

        string author;
        public string Author {
            get {
                if (author != null)
                    return author;
                return author = GetMetadata("Author", "").ToString();
            }
            set {
                SetMetadata("Author", author = value);
            }
        }

        public new string Path {
            get {
                return base.Path;
            }
            set {
                string oldPath = Path;

                base.Path = System.IO.Path.GetFullPath(value);

                // Modify the list of open dictionaries.
                oldPath = System.IO.Path.GetFullPath(oldPath);
                if (openDicts.ContainsKey(oldPath)) {
                    openDicts.Remove(oldPath);
                    openDicts[Path] = new NullWeakReference<SqliteDictionary>(this);
                }
            }
        }

        string url;
        public string Url {
            get {
                if (url != null)
                    return url;
                return url = GetMetadata("Url", "").ToString();
            }
            set {
                SetMetadata("Url", url = value);
            }
        }

        public DictionaryInfo Info {
            get {
                var epath = Path;
                return new DictionaryInfo {
                    Author = Author,
                    Name = Name,
                    Path = Path,
                    Url = Url,
                    // Make sure the DictionaryInfo doesn't contain a reference to the SqliteDictionary object.
                    GetFullInstance = () => new SqliteDictionary(epath)
                };
            }
        }

        string firstLanguage;
        public string FirstLanguage {
            get {
                if (firstLanguage != null)
                    return firstLanguage;
                return firstLanguage = GetMetadata("FirstLanguage", "").ToString();
            }
            set {
                SetMetadata("FirstLanguage", firstLanguage = value);
            }
        }

        string secondLanguage;
        public string SecondLanguage {
            get {
                if (secondLanguage != null)
                    return secondLanguage;
                return secondLanguage = GetMetadata("SecondLanguage", "").ToString();
            }
            set {
                SetMetadata("SecondLanguage", secondLanguage = value);
            }
        }

        string firstLanguageCode;
        public string FirstLanguageCode {
            get {
                if (firstLanguageCode != null)
                    return firstLanguageCode;
                return firstLanguageCode = GetMetadata("FirstLanguageCode", "").ToString();
            }
            set {
                SetMetadata("FirstLanguageCode", firstLanguageCode = value);
            }
        }

        string secondLanguageCode;
        public string SecondLanguageCode {
            get {
                if (secondLanguageCode != null)
                    return secondLanguageCode;
                return secondLanguageCode = GetMetadata("SecondLanguageCode", "").ToString();
            }
            set {
                SetMetadata("SecondLanguageCode", secondLanguageCode = value);
            }
        }
        #endregion

        #region SQL
        DbCommand getRowCountF;
        DbCommand getRowCountR;
        DbCommand findWord;
        DbParameter findWord_Value;
        DbCommand getTranslations;
        DbParameter getTranslations_Phrase;
        DbCommand getPhrases;
        DbParameter getPhrases_Translation;

        DbCommand removeEntry;
        DbParameter removeEntry_From;
        DbParameter removeEntry_To;

        DbCommand search;
        DbParameter search_Term;
        DbCommand searchCI;
        DbParameter searchCI_Term;
        DbCommand searchNA;
        DbParameter searchNA_Term;
        DbCommand searchCINA;
        DbParameter searchCINA_Term;
        DbCommand addWord;
        DbParameter addWord_Value;
        DbParameter addWord_ValueNA;
        DbCommand addEntry;
        DbParameter addEntry_From;
        DbParameter addEntry_To;

        void InitCommands() {
            commands = new List<DbCommand>();

            getRowCountF = CreateCommand("SELECT COUNT(FromID) FROM Entries GROUP BY FromID");
            getRowCountR = CreateCommand("SELECT COUNT(ToID) FROM Entries GROUP BY ToID");

            findWord = CreateCommand("SELECT WordID FROM Words WHERE Value = ?").AddParam(ref findWord_Value);

            getTranslations = CreateCommand("SELECT Value FROM Words INNER JOIN Entries ON Entries.ToID = Words.WordID WHERE Entries.FromID = ?")
                .AddParam(ref getTranslations_Phrase);

            getPhrases = CreateCommand("SELECT Value FROM Words INNER JOIN Entries ON Entries.FromID = Words.WordID WHERE Entries.ToID = ?")
                .AddParam(ref getPhrases_Translation);

            removeEntry = CreateCommand(@"
                DELETE From Entries WHERE FromID = $From AND ToID = $To;
                UPDATE Words WHERE WordID = $From SET fCount = fCount - 1;
                UPDATE Words WHERE WordID = $To SET rCount = rCount - 1;
                DELETE From Words WHERE fCount <= 0 AND rCount <= 0;")
                .AddParam(ref removeEntry_From)
                .AddParam(ref removeEntry_To);
            removeEntry_From.ParameterName = "From";
            removeEntry_To.ParameterName = "To";

            // Second parameter to GetMatch is ignoreAccents - we don't need it.
            search = CreateCommand("SELECT WordID, Value, GetMatch(0, 0, Value, ?) AS MatchType FROM Words WHERE MatchType != -1;").AddParam(ref search_Term);
            searchCI = CreateCommand("SELECT WordID, Value, GetMatch(1, 0, Value, ?) AS MatchType FROM Words WHERE MatchType != -1;").AddParam(ref searchCI_Term);
            searchNA = CreateCommand("SELECT WordID, Value, GetMatch(0, 0, ValueNA, ?) AS MatchType FROM Words WHERE MatchType != -1;").AddParam(ref searchNA_Term);
            searchCINA = CreateCommand("SELECT WordID, Value, GetMatch(1, 0, ValueNA, ?) AS MatchType FROM Words WHERE MatchType != -1;").AddParam(ref searchCINA_Term);

            addWord = CreateCommand("INSERT INTO Words (Value, ValueNA, fCount, rCount) VALUES (?, ?, 0, 0);")
                .AddParam(ref addWord_Value)
                .AddParam(ref addWord_ValueNA);

            addEntry = CreateCommand(@"
                INSERT INTO Entries (FromID, ToID) VALUES ($From, $To);
                UPDATE Words SET fCount = fCount + 1 WHERE WordID = $From;
                UPDATE Words SET rCount = rCount + 1 WHERE WordID = $To;")
                .AddParam(ref addEntry_From)
                .AddParam(ref addEntry_To);
            addEntry_From.ParameterName = "From";
            addEntry_To.ParameterName = "To";
                                                         
        }

        DbCommand CreateCommand(string commandText) {
            var cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            commands.Add(cmd);
            return cmd;
        }

        void InitDatabase() {
            ExecuteSQL(@"
                CREATE TABLE Words (
                    WordID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Value TEXT NOT NULL,
                    ValueNA TEXT NOT NULL,
                    fCount INTEGER NOT NULL,
                    rCount INTEGER NOT NULL);

                CREATE INDEX Words_IndexV ON Words (Value);
                CREATE INDEX Words_IndexVN ON Words (ValueNA);

                CREATE TABLE Entries (
                    FromID INTEGER NOT NULL,
                    ToID INTEGER NOT NULL);

                CREATE INDEX Entries_IndexF ON Entries (FromID);
                CREATE INDEX Entries_IndexT ON Entries (ToID);
                ");
        }
        #endregion

        class Section : IDictionarySection {
            SqliteDictionary dict;
            bool forwards;

            public Section(SqliteDictionary dict, bool forwards) {
                this.dict = dict;
                this.forwards = forwards;
            }

            public int HeadWords {
                get { 
                    return Convert.ToInt32((forwards ? dict.getRowCountF : dict.getRowCountR).ExecuteScalar());
                }
            }

            public void GetFullEntry(Entry entry) {
                if (entry.Translations != null || entry.Tag == null || entry.Tag.Data == null)
                    return;

                long wordID = Convert.ToInt64(entry.Tag.Data);
                var cmd = forwards ? dict.getTranslations : dict.getPhrases;
                cmd.Parameters[0].Value = wordID;

                var translations = new List<Translation>();
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read())
                        translations.Add(new Translation(reader.GetString(0)));
                }

                entry.Translations = translations;
            }

            public IEnumerable<SearchResult> Search(string search, bool ignoreAccents, bool ignoreCase) {
                DbCommand command;

                if (ignoreAccents)
                    command = ignoreCase ? dict.searchCINA : dict.searchNA;
                else
                    command = ignoreCase ? dict.searchCI : dict.search;

                command.Parameters[0].Value = search;

                var results = new List<SearchResult>();
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        long wordID = reader.GetInt64(0);
                        string word = reader.GetString(1);
                        MatchType matchType = (MatchType)reader.GetInt32(2);

                        var entry = new Entry(word, null);
                        entry.Tag = new EntryTag(this, wordID);
                        results.Add(new SearchResult(entry, matchType));
                    }
                }
                return results;
            }

            public void AddEntries(IList<Entry> entries, Action<int, int> notify = null) {
                using (var txn = dict.Connection.BeginTransaction()) {
                    foreach (var e in entries)
                        AddEntry(e);

                    txn.Commit();
                }
            }

            void AddEntry(Entry e) {
                long from = FindOrAddWord(e.Phrase, e.PhraseNoAccents);

                foreach (var tr in e.Translations) {
                    long to = FindOrAddWord(tr.Value, Searcher.RemoveAccents(tr.Value));
                    dict.addEntry_From.Value = from;
                    dict.addEntry_To.Value = to;
                    dict.addEntry.ExecuteNonQuery();
                }
            }

            long FindOrAddWord(string word, string wordNA) {
                dict.findWord_Value.Value = word;
                object result = dict.findWord.ExecuteScalar();

                if (result == null) {
                    dict.addWord_Value.Value = word;
                    dict.addWord_ValueNA.Value = wordNA;
                    dict.addWord.ExecuteNonQuery();
                    return dict.GetLastInsertRowID();
                } else {
                    return Convert.ToInt64(result);
                }
            }

            public IEnumerator<Entry> GetEnumerator() {
                // TODO: Specialize this.
                foreach(var sr in Search("", false, false))
                    yield return sr.Entry;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                return this.GetEnumerator();
            }
        }

        public IDictionarySection ForwardsSection {
            get { return forwardsSection; }
        }

        public IDictionarySection ReverseSection {
            get { return reverseSection; }
        }

        public void Save() {
        }

        protected override int ApplicationSchemaVersion() {
            return 1;
        }

        protected override void IncrementalUpgradeSchema(int toVersion) {
            switch (toVersion) {
                case 1: InitDatabase(); break;
                default: throw new ArgumentOutOfRangeException("toVersion");
            }
        }
    }

    [Obsolete]
    class SqliteSection : Sqlite.SqliteObject, IDictionarySection {
        int section;

        public SqliteSection(SqliteDictionary dict, int section) : base(dict) {
            this.section = section;
        }

        public int HeadWords {
            get {
                return Convert.ToInt32(Select(@"SELECT COUNT(*) FROM Phrases WHERE Section = ?", section));
            }
        }

        DbCommand selectTranslations;
        DbParameter selectTranslationsPhraseID;
        public void GetFullEntry(Entry entry) {
            if (entry.Tag != null && entry.Tag.Data != null) {
                if (selectTranslations == null) {
                    selectTranslations = conn.CreateCommand();
                    selectTranslations.CommandText = @"TYPES String; SELECT Translation FROM Translations WHERE PhraseID = ?";
                    selectTranslationsPhraseID = selectTranslations.CreateParameter();
                    selectTranslations.Parameters.Add(selectTranslationsPhraseID);
                }

                selectTranslationsPhraseID.Value = Convert.ToInt32(entry.Tag.Data);

                using (var reader = selectTranslations.ExecuteReader()) {
                    var translations = new List<Translation>();
                    while (reader.Read())
                        translations.Add(new Translation(reader.GetString(0)));

                    entry.Translations = translations;
                }
            }
        }

        public IEnumerable<SearchResult> Search(string search, bool ignoreAccents, bool ignoreCase) {
            string searchString;
            if (string.IsNullOrEmpty(search))
                searchString = "%";
            else
                searchString = "%" + search.Replace("%", "%%") + "%";

            var srs = new List<SearchResult>();
            Metrics.Measure("Search for " + search, delegate {
                using (var reader = SelectReader(
                    @"TYPES Integer, String, Integer;
                        SELECT PhraseID, Phrase, GetMatch(?, ?, Phrase, ?) AS i FROM Phrases 
                        WHERE Section = ? AND i != -1", ignoreCase ? 1 : 0, ignoreAccents ? 1 : 0, search, section)) {

                    while (reader.Read()) {
                        var sr = GetSearchResult(search, reader.GetString(1), reader.GetInt32(0), (MatchType)reader.GetInt32(2));
                        if (sr != null)
                            srs.Add(sr);
                    }
                }
            });

            return srs;
        }

        SearchResult GetSearchResult(string search, string phrase, int phraseID, MatchType matchType) {
            var entry = new Entry(phrase, null);
            entry.Tag = new EntryTag(this, phraseID);
            return new SearchResult(entry, matchType);
        }

        public IEnumerator<Entry> GetEnumerator() {
            Entry current = null;

            using (var reader = SelectReader(@"TYPES String, String, 
                SELECT Phrase, Translation
                FROM Phrases, Translation
                WHERE Phrases.PhraseID = Translation.TranslationID AND Section = ?", section)) {

                string phrase = reader.GetString(0);
                string translation = reader.GetString(1);

                if (current != null && phrase == current.Phrase) {
                    current.Translations.Add(new Translation(translation));
                } else {
                    if (current != null)
                        yield return current;
                    current = new Entry(phrase, new List<Translation>());
                    current.Tag = new EntryTag(this, null);
                    current.Translations.Add(new Translation(translation));
                }
            }

            if (current != null)
                yield return current;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return (System.Collections.IEnumerator)this.GetEnumerator();
        }

        public void AddEntries(IList<Entry> entries, Action<int, int> notify) {
            int i = 0;
            var txn = conn.BeginTransaction();

            try {
                using (var addPhrase = conn.CreateCommand()) {
                    var phraseParam = addPhrase.CreateParameter();
                    addPhrase.Parameters.Add(phraseParam);
                    var sectionParam = addPhrase.CreateParameter();
                    addPhrase.Parameters.Add(sectionParam);
                    addPhrase.CommandText = @"INSERT INTO Phrases (Phrase, Section) VALUES (?, ?)";
                    sectionParam.Value = section;

                    using (var addTranslation = conn.CreateCommand()) {
                        var ownerPhrase = addTranslation.CreateParameter();
                        addTranslation.Parameters.Add(ownerPhrase);
                        var translationParam = addTranslation.CreateParameter();
                        addTranslation.Parameters.Add(translationParam);
                        addTranslation.CommandText = @"INSERT INTO Translations (PhraseID, Translation) VALUES (?, ?)";

                        foreach (var e in entries) {
                            phraseParam.Value = e.Phrase;
                            addPhrase.ExecuteNonQuery();

                            if (++i % 1024 == 0) {
                                // txn.Commit();
                                // txn.Dispose();
                                // txn = conn.BeginTransaction();
                                if (notify != null)
                                    notify(i - 1, entries.Count);
                            }

                            foreach (var t in e.Translations) {
                                ownerPhrase.Value = GetLastInsertRowID();
                                translationParam.Value = t.Value;
                                addTranslation.ExecuteNonQuery();
                            }
                        }
                    }
                }
                txn.Commit();
            } finally {
                txn.Dispose();
            }
        }

        protected override void Dispose(bool disposing) {
            if (selectTranslations != null)
                selectTranslations.Dispose();
            base.Dispose(disposing);
        }
    }

    [SQLiteFunction(Name = "GetMatch", FuncType = FunctionType.Scalar, Arguments = 4)]
    public class GetMatch : SQLiteFunction {
        public GetMatch() { }

        public override object Invoke(object[] args) {
            var x = args[2].ToString();
            var y = args[3].ToString();
            bool ignoreCase = Convert.ToInt32(args[0]) == 1;
            bool ignoreAccents = Convert.ToInt32(args[1]) == 1;

            if (ignoreAccents) {
                var mt = Searcher.GetMatch(x, y, ignoreCase);
                if (mt == null)
                    return -1;
                return mt.Value;
            }

            var i = x.IndexOf(y, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            if (i == 0) {
                if (x.Length == y.Length)
                    return (int)MatchType.PerfectMatch;
                return (int)MatchType.StartMatch;
            } else if (i > 0) {
                return (int)MatchType.NormalMatch;
            } else {
                return -1;
            }
        }
    }
}