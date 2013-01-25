using System;
using System.Linq;
using System.Windows.Forms;
using Szotar.WindowsForms.Properties;

namespace Szotar{
	public class TagMenu : ToolStripMenuItem {
	    readonly ToolStripTextBox customTag;
		WordList list;

		public TagMenu() {
			base.Text = Resources.TagsMenuHeader;

			customTag = new ToolStripTextBox();
			customTag.KeyUp += CustomTagKeyUp;

			DropDownOpening += OnDropDownOpening;
		}

		public override bool HasDropDownItems {
			get {
				return true;
			}
		}

		void OnDropDownOpening(object sender, EventArgs e) {
			DropDownItems.Clear();

			if (list == null)
				return;

			var currentTags = list.Tags;
			var tags = DataStore.Database.GetTags(false);

			foreach (var tag in tags) {
				var item = new ToolStripMenuItem(tag.Key)
				           {Tag = tag.Key, CheckOnClick = true, Checked = currentTags.Contains(tag.Key)};
			    item.CheckedChanged += ItemCheckedChanged;
				DropDownItems.Insert(DropDownItems.Count, item);
			}

			DropDownItems.Insert(DropDownItems.Count, customTag);
		}

		void CustomTagKeyUp(object sender, KeyEventArgs e) {
			string tag = ((ToolStripTextBox)sender).Text.Trim();
		    if (e.KeyCode != Keys.Enter || string.IsNullOrEmpty(tag))
		        return;
		    
            list.Tag(tag);
		    DropDown.Close();
		    ((ToolStripTextBox)sender).Clear();
		    e.Handled = true;
		}

		public WordList WordList {
			get { return list; }
			set {
				list = value;
				Visible = list != null;
			}
		}

		void ItemCheckedChanged(object sender, EventArgs e) {
			var item = (ToolStripMenuItem)sender;
			var tag = (string)item.Tag;
			if (item.Checked)
				WordList.Tag(tag);
			else
				WordList.Untag(tag);
		}
	}
}
