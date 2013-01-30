using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Szotar.WindowsForms {
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
			} catch (Sqlite.DatabaseVersionException e) {
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

			if (!string.IsNullOrEmpty(GuiConfiguration.UiLanguage)) {
				try {
					// According to MSDN, we should set both CurrentCulture and CurrentUICulture.
					// http://msdn.microsoft.com/en-us/library/w7x1y988.aspx
					var culture = new CultureInfo(GuiConfiguration.UiLanguage);

					Thread.CurrentThread.CurrentCulture = culture;
					Thread.CurrentThread.CurrentUICulture = culture;
				} catch (ArgumentException) {
					ProgramLog.Default.AddMessage(LogType.Error, "The UI language \"{0}\" is invalid.", GuiConfiguration.UiLanguage);
				}
			}

			if(CheckForExistingInstance())
				return;

			ProtocolHandler.Register("szotar", "Szótár URL");

			switch (GuiConfiguration.StartupAction) {
				case "StartPage":
					new Forms.StartPage().Show();
					break;

				case "Practice":
					new Forms.PracticeWindow(DataStore.Database.GetSuggestedPracticeItems(GuiConfiguration.PracticeDefaultCount), PracticeMode.Learn).Show();
					break;

				case "Dictionary":
				default:
					string dict = GuiConfiguration.StartupDictionary;
					if (string.IsNullOrEmpty(dict))
						goto case "StartPage";

					Exception error = null;

					try {
						DictionaryInfo info = new SimpleDictionary.Info(dict);
						new Forms.LookupForm(info).Show();
					} catch (System.IO.IOException e) { // TODO: Access/permission exceptions?
						error = e;
					} catch (DictionaryLoadException e) {
						error = e;
						// Maybe there should be some UI for this (it's there, but not loadable?)...
					}

					if (error != null) {
						ProgramLog.Default.AddMessage(LogType.Error, "Could not load dictionary {0}: {1}", dict, error.Message);
						goto case "StartPage";
					}
					break;
			}

			RunUntilNoForms();
		}

		// TODO Send message with arguments to existing instance
		private static bool CheckForExistingInstance() {
			bool created;
			new EventWaitHandle(false, EventResetMode.ManualReset, @"Local\Szotar.SingleInstance#" + Environment.UserDomainName + "#" + Environment.UserName, out created);
			return !created;
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
			Application.Idle += ApplicationIdle;
		}

		[System.Diagnostics.DebuggerStepThrough]
		void ApplicationIdle(object sender, EventArgs e) {
			// ApplicationContext also calls ExitThread, except it only waits for one form.
			if (Application.OpenForms.Count == 0)
				ExitThread();

			if (Configuration.Default.NeedsSaving)
				Configuration.Default.Save();
		}
	}
}
