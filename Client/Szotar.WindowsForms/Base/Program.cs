using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Szotar.WindowsForms {
	static class Program {
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			switch (GuiConfiguration.StartupAction) {
				case "Dictionary":
					string dict = GuiConfiguration.StartupDictionary;
					if (!string.IsNullOrEmpty(dict)) {
						try {
							DictionaryInfo info = new SimpleDictionary.Info(dict);
							new Forms.LookupForm(info).Show();
							RunUntilNoForms();
							break;
						} catch (System.IO.IOException) { //TODO: Access/permission exceptions?
						} catch (DictionaryLoadException) {
							//Maybe there should be some UI for this (it's there, but not loadable?)...
						}
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
			}

			return;
		}

		private static void RunUntilNoForms() {
			Application.Run(new SzotarContext());
		}

		//TODO: Get working with Dictionary.GetAll
		//static void OpenFirstLanguagePair() {
		//    //For now, just load the first language pair and open a dictionary for it.
		//    foreach (LanguagePair p in LanguagePair.GetAllLanguagePairs()) {
		//        //Hack (but it's only for debugging anyway)
		//        if (p.Name.Contains("abridged"))
		//            continue;
		//        Application.Run(new Forms.LookupForm(p));
		//        break;
		//    }
		//}

		//Used to convert a TEI dictionary into a SimpleDictionary.
		//Whenever possible, this should be moved to the Tools solution.
		static void ConvertFromTei() {
			IBilingualDictionary td = new TeiDictionary(@"..\..\eng-hun-huge.xml");
			SimpleDictionary sd = new SimpleDictionary(td.ForwardsSection, td.ReverseSection);
			sd.Name = "English-Hungarian dictionary";
			sd.Author = "freedict.org / Vonyó Attila";
			sd.ForwardsSection.Name = "English-Hungarian";
			sd.ReverseSection.Name = "Hungarian-English";
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
			//This is easier than adding a handler to the Close event of every form that opens.
			Application.Idle += new EventHandler(Application_Idle);
		}

		void Application_Idle(object sender, EventArgs e) {
			//ApplicationContext also calls ExitThread, except it only waits for one form.
			if (Application.OpenForms.Count == 0)
				this.ExitThread();
		}
	}
}