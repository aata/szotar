using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Szotar.WindowsForms {
	static class Program {
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			Szotar.LocalizationProvider.Default = new LocalizationProvider();
			ToolStripManager.Renderer = new ToolStripAeroRenderer(ToolbarTheme.Toolbar);
			DataStore.Database.WordListDeleted += new EventHandler<Szotar.Sqlite.WordListDeletedEventArgs>(Database_WordListDeleted);

			switch (GuiConfiguration.StartupAction) {
				case "Dictionary":
					string dict = GuiConfiguration.StartupDictionary;
					if (!string.IsNullOrEmpty(dict)) {
						try {
							DictionaryInfo info = new SimpleDictionary.Info(dict);
							new Forms.LookupForm(info).Show();
						} catch (System.IO.IOException) { //TODO: Access/permission exceptions?
						} catch (DictionaryLoadException) {
							//Maybe there should be some UI for this (it's there, but not loadable?)...
						}
						RunUntilNoForms();
						break;
					}
					goto case "StartPage";
				case "StartPage":
					new Forms.StartPage().Show();
					RunUntilNoForms();
					break;
				case "Practice":
					new Forms.PracticeWindow().Show();
					RunUntilNoForms();
					break;
				case "ImportDictionary":
					new Forms.DictionaryImport().Show();
					RunUntilNoForms();
					break;
			}

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

		//Used to convert a TEI dictionary into a SimpleDictionary.
		//Whenever possible, this should be moved to the Tools project.
		static void ConvertFromTei() {
			IBilingualDictionary td = new TeiDictionary(@"..\..\eng-hun-huge.xml");
			SimpleDictionary sd = new SimpleDictionary(td.ForwardsSection, td.ReverseSection);
			sd.Name = "English-Hungarian dictionary";
			sd.Author = "freedict.org / Vonyó Attila";
			sd.FirstLanguage = "English";
			sd.SecondLanguage = "Hungarian";
			sd.Url = "http://almos.vein.hu/~vonyoa/";

			//We're probably in Debug/bin.
			//After all, this function is only here for debugging.
			sd.Write(@"..\..\Data\Dictionaries\en-hu.dict");
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

			if (Configuration.Default.NeedsSaving)
				Configuration.Default.Save();
		}
	}
}
