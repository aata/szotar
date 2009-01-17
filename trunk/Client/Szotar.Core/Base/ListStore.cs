using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Szotar {
	[Serializable]
	public class ListInfo {
		public long? ID { get; set; }
		public string Name { get; set; }
		public string Author { get; set; }
		public string Language { get; set; }
		public string Url { get; set; }
		public DateTime? Date { get; set; }

		public ListInfo() {
		}
	}
	
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
