using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Szotar {
	public interface IListStore {
		IEnumerable<ListInfo> GetLists();
	}

	public class RecentListStore : IListStore {
		public IEnumerable<ListInfo> GetLists() {
            return DataStore.Database.GetRecentSets(Configuration.RecentListsSize);
		}
	}

	public class SqliteListStore : IListStore {
		Sqlite.SqliteDataStore database;

		public SqliteListStore(Sqlite.SqliteDataStore database) {
			this.database = database;
		}

		public IEnumerable<ListInfo> GetLists() {
			return database.GetAllSets();
		}
	}
}
