using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class DictionaryInfoEditor : Form {
		IBilingualDictionary dict;
		bool saveOnClose;

		public DictionaryInfoEditor(IBilingualDictionary dict, bool saveOnClose) {
			InitializeComponent();

			this.dict = dict;
			this.saveOnClose = saveOnClose;

			objects.DisplayMember = "Name";
			objects.Items.Add(new Item { Name = "Dictionary", Object = dict });
			objects.Items.Add(new Item { Name = "Forwards Section", Object = dict.ForwardsSection });
			objects.Items.Add(new Item { Name = "Backwards Section", Object = dict.ReverseSection });
			objects.SelectedIndex = 0;
		}

		private void dictionaries_SelectedIndexChanged(object sender, EventArgs e) {
			properties.SelectedObject = ((Item)objects.SelectedItem).Object;
			properties.Visible = true;
		}

		private void closeButton_Click(object sender, EventArgs e) {
			if (saveOnClose) {
				try {
					dict.Save();
				} catch (InvalidOperationException) {
					// Oh well. I tried.
				}
			}

			Close();
		}

		class Item {
			public object Object { get; set; }
			public string Name { get; set; }
		}
	}
}
