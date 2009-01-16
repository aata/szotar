using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Szotar.WindowsForms.Forms {
	public partial class StartPage : Form {
		public StartPage() {
			InitializeComponent();

			dictionaries.BeginUpdate();
			foreach (DictionaryInfo dict in Szotar.Dictionary.GetAll()) {
				ListViewItem item = new ListViewItem(new string[] { dict.Name, dict.Author });
				item.Tag = dict;
				item.ImageKey = "Dictionary";
				dictionaries.Items.Add(item);
			}
			dictionaries.EndUpdate();

			ThemeHelper.UseExplorerTheme(dictionaries);
		}

		private void languagePairs_ItemActivate(object sender, EventArgs e) {
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

		public static void ShowStartPage() {
			foreach (Form form in Application.OpenForms) {
				if (form is StartPage) {
					form.BringToFront();
					return;
				}
			}

			new StartPage().Show();
		}
	}
}