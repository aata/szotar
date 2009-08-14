using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Szotar.WindowsForms.Preferences;

namespace Szotar.WindowsForms.Preferences {
	[PreferencePage("Startup", Parent = typeof(Categories.General), Importance = 20)]
	public partial class Startup : PreferencePage {
		public Startup() {
			InitializeComponent();

			infoLabel.Text = string.Format(infoLabel.Text, Application.ProductName);

			string startupDictionary = GuiConfiguration.StartupDictionary;

			list.BeginUpdate();
			list.DisplayMember = "Name";
			foreach (DictionaryInfo dict in Szotar.Dictionary.GetAll()) {
				list.Items.Add(dict);
				if (startupDictionary == dict.Path)
					list.SelectedIndex = list.Items.Count - 1;
			}
			list.EndUpdate();

			switch (GuiConfiguration.StartupAction) {
				case "StartPage":
					startPage.Checked = true;
					break;
				case "Dictionary":
					dictionary.Checked = true;
					break;
				case "Practice":
					practice.Checked = true;
					break;
			}
		}

		public override void Commit() {
			if (startPage.Checked) {
				GuiConfiguration.StartupAction = "StartPage";
			} else if (dictionary.Checked) {
				GuiConfiguration.StartupAction = "Dictionary";
				object dict = list.SelectedItem;
				if (dict != null) {
					GuiConfiguration.StartupDictionary = ((DictionaryInfo)dict).Path;
				} else {
					GuiConfiguration.StartupDictionary = null;
				}
			} else if (practice.Checked) {
				GuiConfiguration.StartupAction = "Practice";
			}
		}

		private void list_SelectedIndexChanged(object sender, EventArgs e) {
			dictionary.Checked = true;
		}
	}
}
