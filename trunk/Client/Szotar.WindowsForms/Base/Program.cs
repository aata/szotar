using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Szotar.WindowsForms {
	using Szotar.Json;

	static class Program {
		[STAThread]
		static void Main() {
			try {
				RealMain();
			} catch (System.IO.FileNotFoundException e) {
				if (e.FileName.StartsWith("Szotar.Core"))
					Errors.DllNotFound("Szotar.Core.dll", e);
				else
					Errors.FileNotFound(e);
			}
		}

		// If Szotar.Core.dll doesn't exist, a FileNotFoundException is thrown as soon as a 
		// method referring to that assembly is JIT compiled. Accordingly, we wrap Main in
		// a try/catch block to provide a better error message.
		// This is obviously moot if ILMerge is used, but that can't be taken for granted.
		static void RealMain() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Szotar.LocalizationProvider.Default = new LocalizationProvider();
			ToolStripManager.Renderer = new ToolStripAeroRenderer(ToolbarTheme.Toolbar);

			try {
				DataStore.InitializeDatabase();
			} catch (Szotar.Sqlite.DatabaseVersionException e) {
				Errors.NewerDatabaseVersion(e);
				return;
			} catch (System.IO.FileNotFoundException e) {
				if (e.FileName.StartsWith("System.Data.SQLite"))
					Errors.DllNotFound("System.Data.SQLite.dll", e);
				else
					Errors.FileNotFound(e);
				return;
			} catch (System.IO.IOException e) {
				Errors.CannotOpenDatabase(e);
				return;
			} catch (System.Data.Common.DbException e) {
				Errors.CannotOpenDatabase(e);
				return;
			}

			DataStore.Database.WordListDeleted += new EventHandler<Szotar.Sqlite.WordListDeletedEventArgs>(Database_WordListDeleted);

			switch (GuiConfiguration.StartupAction) {
				case "StartPage":
					new Forms.StartPage().Show();
					break;

				case "Practice":
					new Forms.PracticeWindow().Show();
					break;

				case "Dictionary":
				default:
					string dict = GuiConfiguration.StartupDictionary;
					if (string.IsNullOrEmpty(dict))
						goto case "StartPage";

					try {
						DictionaryInfo info = new SimpleDictionary.Info(dict);
						new Forms.LookupForm(info).Show();
					} catch (System.IO.IOException) { // TODO: Access/permission exceptions?
						goto case "StartPage";
					} catch (DictionaryLoadException) {
						// Maybe there should be some UI for this (it's there, but not loadable?)...
						goto case "StartPage";
					}

					break;
			}

			RunUntilNoForms();
			return;
		}

		static void Database_WordListDeleted(object sender, Szotar.Sqlite.WordListDeletedEventArgs e) {
			var recent = Configuration.RecentLists;
			if (recent != null) {
				recent.RemoveAll(r => r.ID == e.SetID);
				Configuration.RecentLists = recent;
				Configuration.Save();
			}
		}

		private static void RunUntilNoForms() {
			Application.Run(new SzotarContext());
		}
	}

	/// <summary>
	/// Waits until all Forms are closed before exiting the main thread. The default
	/// application context exits after the initial Form is closed.
	/// </summary>
	public class SzotarContext : ApplicationContext {
		public SzotarContext() {
			// This is easier than adding a handler to the Close event of every form that opens.
			Application.Idle += new EventHandler(Application_Idle);
		}

		void Application_Idle(object sender, EventArgs e) {
			// ApplicationContext also calls ExitThread, except it only waits for one form.
			if (Application.OpenForms.Count == 0)
				this.ExitThread();

			if (Configuration.Default.NeedsSaving) {
				Configuration.Default.Save();
			}
		}
	}
}
