using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace Szotar.WindowsForms {
	static class Program {
		[STAThread]
		static void Main(string[] commandLine) {
			try {
				RealMain(commandLine);
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
		static void RealMain(string[] commandLine) {
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

			ProtocolHandler.Register("szotar", "Szótár URL");
			new SingleInstanceController().Run(commandLine);
		}

		private class SingleInstanceController : WindowsFormsApplicationBase {
			public SingleInstanceController() {
				IsSingleInstance = true;
				ShutdownStyle = ShutdownMode.AfterAllFormsClose;
				Application.Idle += OnIdle;
			}

			private static void OnIdle(object sender, EventArgs eventArgs) {
				if (Configuration.Default.NeedsSaving)
					Configuration.Default.Save();
			}

			protected override bool OnStartup(StartupEventArgs args) {
				HandleCommandLine(args.CommandLine);
				return true;
			}

			private static bool HandleCommandLine(ReadOnlyCollection<string> commandLine) {
				return false;
			}

			protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs) {
				if (HandleCommandLine(eventArgs.CommandLine))
					return;

				eventArgs.BringToForeground = true;
			}

			protected override void OnCreateMainForm() {
				switch (GuiConfiguration.StartupAction) {
					case "Practice":
						MainForm =
							new Forms.PracticeWindow(DataStore.Database.GetSuggestedPracticeItems(GuiConfiguration.PracticeDefaultCount),
							                         PracticeMode.Learn);
						break;

					case "Dictionary":
						string dict = GuiConfiguration.StartupDictionary;
						if (string.IsNullOrEmpty(dict))
							break;

						Exception error = null;

						try {
							DictionaryInfo info = new SimpleDictionary.Info(dict);
							MainForm = new Forms.LookupForm(info);
						} catch (System.IO.IOException e) {
							// TODO: Access/permission exceptions?
							error = e;
						} catch (DictionaryLoadException e) {
							error = e;
							// Maybe there should be some UI for this (it's there, but not loadable?)...
						}

						if (error != null)
							ProgramLog.Default.AddMessage(LogType.Error, "Could not load dictionary {0}: {1}", dict, error.Message);
						break;
				}

				if (MainForm == null)
					MainForm = new Forms.StartPage();
			}
		}
	}
}
