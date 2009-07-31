using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Szotar {
	//Because of the "Path" member of IDataStore, it is necessary to explicitly refer to System.IO.Path,
	//and I find it cleaner to omit the "System".
	using IO = System.IO;

	public interface IDataStore {
		IEnumerable<FileInfo> GetFiles(string relativePath, Regex nameRegex, bool recurse);
		void EnsureDirectoryExists(string relativePath);
		string Path { get; }
		bool Writable { get; }
	}

	public class DataStore : IDataStore {
		static IDataStore userDataStore, programDataStore, combinedDataStore;

		public string Path { get; private set; }
		public bool Writable { get; private set; }

		protected DataStore(string path, bool writable) {
			Path = path;
			Writable = writable;
		}

		//The intended purpose of this method is for enumeration of files of a 
		//particular type. Perhaps it should be replaced with an iterator method.
		private DirectoryInfo GetSubDirectory(string relativePath) {
			return new DirectoryInfo(IO.Path.Combine(this.Path, relativePath));
		}

		public IEnumerable<FileInfo> GetFiles(string relativePath, Regex nameRegex, bool recurse) {
			DirectoryInfo subDir = GetSubDirectory(relativePath);
			if(!subDir.Exists)
				yield break;
			
			foreach (FileInfo fi in subDir.GetFiles()) {
				//Depth-first recursion. (Hopefully there is no recursive file structure.)
				if (((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory) && recurse) {
					foreach (FileInfo sfi in GetFiles(IO.Path.Combine(relativePath, fi.Name), nameRegex, true)) {
						yield return sfi;
					}
				} else if ((fi.Attributes & FileAttributes.Directory) == 0) {
					if (nameRegex.Match(fi.Name).Success)
						yield return fi;
				}
			}
		}

		public void EnsureDirectoryExists(string relativePath) {
			if (Writable == false)
				throw new InvalidOperationException();

			DirectoryInfo di = new DirectoryInfo(IO.Path.Combine(Path, relativePath));
			if(!di.Exists)
				di.Create();
		}

		#region Static Instances
		public static IDataStore UserDataStore {
			get {
				if (userDataStore != null)
					return userDataStore;
				
				return userDataStore = new DataStore(Configuration.UserDataStore, true);
			}
		}

		public static IDataStore ProgramDataStore {
			get {
				if (programDataStore != null)
					return programDataStore;
				
				string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
				if(string.IsNullOrEmpty(exePath))
					exePath = "./Something.exe";
				string dirPath = IO.Path.Combine(IO.Path.GetDirectoryName(exePath), "Data");
				
				//Ensure that this path exists, as currently it's the cause of numerous ugly exceptions.
				//This may sound silly, considering the ProgramDataStore isn't even writable, but it 
				//is better in two ways:
				// - if the location is writable, there is no problem.
				// - if the location is not writable, it fails immediately rather than at an indeterminate time.
				DirectoryInfo di = new IO.DirectoryInfo(dirPath);
				if(!di.Exists)
					di.Create();
				
				return programDataStore = new DataStore(
					dirPath, false);
			}
		}

		public static IDataStore CombinedDataStore {
			get {
				if (combinedDataStore != null)
					return combinedDataStore;
				return combinedDataStore = new CombinedDataStore(UserDataStore, ProgramDataStore);
			}
		}

		private static Sqlite.SqliteDataStore database;
		public static Sqlite.SqliteDataStore Database {
			get {
				InitializeDatabase();
				return database;
			}
		}

		/// <summary>Call this method if you need to be sure of when the database is initialized
		/// (e.g. to catch versioning exceptions)</summary>
		/// <remarks>Really, this should be necessary, not optional...</remarks>
		public static void InitializeDatabase() {
			if (database != null)
				 return;
			database = new Sqlite.SqliteDataStore(IO.Path.Combine(UserDataStore.Path, "database.sqlite"));
		}
		#endregion
	}

	/// <summary>
	/// Combines the User and Program data stores. Any writes go to the user data store.
	/// </summary>
	class CombinedDataStore : IDataStore {
		//The user data store should be writable, but the program data store should never be writable.
		public bool Writable {
			get { return User.Writable; }
		}

		public string Path {
			get { return User.Path; }
		}

		public IDataStore User { get; private set; }
		public IDataStore Program { get; private set; }

		public CombinedDataStore(IDataStore user, IDataStore program) {
			User = user;
			Program = program;
		}

		public IEnumerable<FileInfo> GetFiles(string relativePath, Regex nameRegex, bool recurse) {
			foreach (FileInfo fi in User.GetFiles(relativePath, nameRegex, recurse))
				yield return fi;
			foreach (FileInfo fi in Program.GetFiles(relativePath, nameRegex, recurse))
				yield return fi;
		}

		public void EnsureDirectoryExists(string relativePath) {
			User.EnsureDirectoryExists(relativePath);
		}
	}
}
