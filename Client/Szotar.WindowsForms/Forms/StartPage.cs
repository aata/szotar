using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Szotar.WindowsForms.Forms {
	using ListOpen = Controls.ListOpen;

	public partial class StartPage : Form {
		public StartPage() {
			InitializeComponent();

			ThemeHelper.UseExplorerTheme(dictionaries);

			listSearch.ListsChosen += new EventHandler<Szotar.WindowsForms.Controls.ListsChosenEventArgs>(listSearch_ListsChosen);

			if (DesignMode)
				return;

			dictionaries.BeginUpdate();
			foreach (DictionaryInfo dict in Szotar.Dictionary.GetAll()) {
				ListViewItem item = new ListViewItem(new string[] { dict.Name, dict.Author });
				item.Tag = dict;
				item.ImageKey = "Dictionary";
				dictionaries.Items.Add(item);
			}
			dictionaries.EndUpdate();
		}

		void listSearch_ListsChosen(object sender, Szotar.WindowsForms.Controls.ListsChosenEventArgs e) {
			var opened = new List<ListOpen>();

			foreach (var current in e.Chosen) {
				var i = opened.FindIndex(x => x.SetID == current.SetID);
				if (i != -1)
					opened[i] = new ListOpen { 
						SetID = current.SetID,
						Position = Math.Min(opened[i].Position, current.Position) };
				else
					opened.Add(current);
			}

			foreach (var open in opened) {
				var lb = new ListBuilder(DataStore.Database.GetWordList(open.SetID));
				lb.ScrollToPosition(open.Position);
				lb.Show();
			}
		}

		private void OnItemActivate(object sender, EventArgs e) {
			DictionaryInfo dict = dictionaries.SelectedItems[0].Tag as DictionaryInfo;

			try {
				LookupForm.OpenDictionary(dict);
				this.Close();
			} catch (DictionaryLoadException ex) {
				MessageBox.Show(this,
					string.Format(CultureInfo.CurrentUICulture, Resources.Errors.CouldNotLoadDictionary, (dictionaries.SelectedItems[0].Tag as DictionaryInfo).Name, ex.Message),
					ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public static StartPage ShowStartPage(StartPageTab? tab) {
			StartPage sp = null;

			foreach (Form form in Application.OpenForms) {
				if (form is StartPage) {
					sp = (StartPage)form;
					form.BringToFront();
					break;
				}
			}

			if (sp == null) {
				sp = new StartPage();
				sp.Show();
			}

			if (tab.HasValue)
				sp.SwitchToTab(tab.Value);
			return sp;
		}

		private void SwitchToTab(StartPageTab tab) {
			if (tab == StartPageTab.Dictionaries)
				tasks.SelectedIndex = 0;
			else if(tab == StartPageTab.Practice)
				tasks.SelectedIndex = 1;
		}
	}

	public enum StartPageTab {
		Dictionaries,
		Practice
	}
}