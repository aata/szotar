using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Szotar {
	[Serializable]
	public class ListInfo {
		public string Path { get; set; }
		public string Name { get; set; }

		public ListInfo(string path, string name) {
			Path = path;
			Name = name;
		}

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
			ListInfo[] recent = Configuration.RecentLists;

			if (recent == null)
				yield break;

			foreach (ListInfo li in recent) {
				yield return li;
			}
		}
	}

	public class DefaultListStore : IListStore {
		public string Name {
			get {
				return "Untitled List"; //FIXME Properties.Resources.DefaultListStoreName;
			}
		}

		public string DefaultDirectory {
			get {
				return Configuration.UserListsStore;
			}

			set {
				DirectoryInfo di = new DirectoryInfo(value);
				if (!di.Exists)
					di.Create();
				
				Configuration.UserListsStore = value;
			}
		}

		public IEnumerable<ListInfo> GetLists() {
			DirectoryInfo di = new DirectoryInfo(DefaultDirectory);
			FileInfo[] files;

			try {
				files = di.GetFiles();
			} catch (DirectoryNotFoundException) {
				yield break;
			}

			foreach (FileInfo fi in files) {
				yield return new ListInfo
				{
					Path = fi.FullName,
					Name = Path.GetFileNameWithoutExtension(fi.FullName)
				};
			}
		}
	}
}
