using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Duplicate = Szotar.Sqlite.SqliteDataStore.Duplicate;

namespace Szotar.WindowsForms.Forms {
	public partial class FindDuplicates : Form {
		Queue<KeyValuePair<Duplicate, Duplicate>> duplicates;

		public FindDuplicates() {
			InitializeComponent();

			if (components == null)
				components = new Container();

			duplicates = new Queue<KeyValuePair<Duplicate,Duplicate>>(DataStore.Database.FindDuplicateListItems());
			GoToNext();

			leftTermEditor.ItemDeleted += new EventHandler(termEditor_ItemDeleted);
			rightTermEditor.ItemDeleted += new EventHandler(termEditor_ItemDeleted);

			Font boldFont = new Font(this.Font, FontStyle.Bold);
			components.Add(new DisposableComponent(boldFont));
			leftListName.Font = rightListName.Font = boldFont;
		}

		void termEditor_ItemDeleted(object sender, EventArgs e) {
			GoToNext();
		}

		void GoToNext() {
			for (; ; ) {
				if (duplicates.Count == 0) {
					deleteLeft.Enabled = deleteRight.Enabled = next.Enabled = false;
					leftTermEditor.Item = null;
					rightTermEditor.Item = null;
					leftListName.Text = rightListName.Text = string.Empty;
					return;
				}
				var pair = duplicates.Dequeue();
				if (Update(pair.Key, pair.Value))
					return;
			}
		}

		private void deleteLeft_Click(object sender, EventArgs e) {
			rightTermEditor.Save();
			leftTermEditor.Item.Owner.Remove(leftTermEditor.Item);
		}

		private void deleteRight_Click(object sender, EventArgs e) {
			leftTermEditor.Save();
			rightTermEditor.Item.Owner.Remove(rightTermEditor.Item);
		}

		private void next_Click(object sender, EventArgs e) {
			leftTermEditor.Save();
			rightTermEditor.Save();
			GoToNext();
		}

		bool Update(Duplicate left, Duplicate right) {
			var leftItem = DataStore.Database.GetWordList(left.SetID).FirstOrDefault(x => x.Phrase == left.Phrase && x.Translation == left.Translation);
			var rightItem = DataStore.Database.GetWordList(right.SetID).FirstOrDefault(x => x.Phrase == right.Phrase && x.Translation == right.Translation);
			if (leftItem == null || rightItem == null)
				return false;

			leftTermEditor.Item = leftItem;
			rightTermEditor.Item = rightItem;
			leftListName.Text = leftItem.Owner.Name ?? Properties.Resources.DefaultListName;
			rightListName.Text = rightItem.Owner.Name ?? Properties.Resources.DefaultListName;
			return true;
		}

		private void close_Click(object sender, EventArgs e) {
			leftTermEditor.Save();
			rightTermEditor.Save();
			Close();
		}
	}
}
