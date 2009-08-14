using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Szotar {
	public interface IListStore {
		string Name { get; }
		IEnumerable<ListInfo> GetLists();
	}

	public class RecentListStore : IListStore {
		public string Name {
			get {
				return "Recent Lists"; //FIXME Properties.Resources.RecentListStoreName;
			}
		}

		public IEnumerable<ListInfo> GetLists() {
			var recent = Configuration.RecentLists;

			if (recent == null)
				yield break;

			foreach (ListInfo li in recent) {
				yield return li;
			}
		}
	}

	public class SqliteListStore : IListStore {
		Sqlite.SqliteDataStore database;

		public SqliteListStore(Sqlite.SqliteDataStore database) {
			this.database = database;
		}

		public string Name {
			get { return "All Lists"; }
		}

		public IEnumerable<ListInfo> GetLists() {
			return database.GetAllSets();
		}
	}
}
