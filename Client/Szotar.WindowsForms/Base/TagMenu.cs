using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Szotar{
    public class TagMenu : ToolStripMenuItem {
        ToolStripTextBox customTag;
        WordList list;

        public TagMenu() {
            Text = "&Tags";

            customTag = new ToolStripTextBox();
            customTag.KeyUp += new KeyEventHandler(customTag_KeyUp);

            DropDownOpening += new EventHandler(TagMenu_DropDownOpening);
        }

        public override bool HasDropDownItems {
            get {
                return true;
            }
        }

        void TagMenu_DropDownOpening(object sender, EventArgs e) {
            DropDownItems.Clear();

            if (list == null)
                return;

            var currentTags = list.Tags;
            var tags = DataStore.Database.GetTags(false);

            foreach (var tag in tags) {
                var item = new ToolStripMenuItem(tag.Key);
                item.Tag = tag.Key;
                item.CheckOnClick = true;
                item.Checked = currentTags.Contains(tag.Key);
                item.CheckedChanged += new EventHandler(item_CheckedChanged);
                DropDownItems.Insert(DropDownItems.Count, item);
            }

            DropDownItems.Insert(DropDownItems.Count, customTag);
        }

        void customTag_KeyUp(object sender, KeyEventArgs e) {
            string tag = ((ToolStripTextBox)sender).Text.Trim();
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(tag)) {
                list.Tag(tag);
                DropDown.Close();
                ((ToolStripTextBox)sender).Clear();
                e.Handled = true;
            }
        }

        public WordList WordList {
            get { return list; }
            set {
                list = value;
                Visible = list != null;
            }
        }

        void item_CheckedChanged(object sender, EventArgs e) {
            var item = (ToolStripMenuItem)sender;
            string tag = (string)item.Tag;
            if (item.Checked)
                WordList.Tag(tag);
            else
                WordList.Untag(tag);
        }
    }
}
