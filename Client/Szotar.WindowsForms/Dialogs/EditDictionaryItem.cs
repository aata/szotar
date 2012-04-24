using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Dialogs {
    public partial class EditDictionaryItem : Form {
        IDictionarySection dictionary;
        Entry entry;

        public EditDictionaryItem(IDictionarySection dictionary, string phraseLanguage, string translationLanguage, Entry entry = null) {
            InitializeComponent();

            this.dictionary = dictionary;
            this.entry = entry;

            if (!string.IsNullOrEmpty(phraseLanguage))
                phraseLabel.Text = phraseLanguage + ":";

            if (!string.IsNullOrEmpty(translationLanguage))
                translationsLabel.Text = translationLanguage + ":";

            if (Adding) {
                this.Text = Properties.Resources.AddDictionaryItem;
                complete.Text = Properties.Resources.Add;
            } else {
                this.Text = Properties.Resources.EditDictionaryItem;
                complete.Text = Properties.Resources.Save;
            }

            if (entry != null) {
                phrase.Text = entry.Phrase;
                foreach (var t in entry.Translations) {
                    translations.Items.Add(new ListViewItem(new string[] { t.Value }));
                }
            }

            ThemeHelper.UseExplorerTheme(translations);

            translations.Resize += delegate {
                AutoSizeColumns();
            };
            AutoSizeColumns();
        }

        void AutoSizeColumns() {
            translations.Columns[0].Width = translations.ClientSize.Width - 1;
        }

        bool Adding { get { return entry == null; } }

        private void complete_Click(object sender, EventArgs e) {
            var translations = this.translations.Items.Cast<ListViewItem>().Select(lvi => new Translation(lvi.Text)).ToArray();
            
            if (Adding) {
                entry = new Entry(phrase.Text, translations);
            } else {
                entry.Phrase = phrase.Text;
                entry.Translations = translations;
                dictionary.EntryModified(entry);
            }
        }

        private void addTranslation_Click(object sender, EventArgs e) {
            translations.Items.Add(new ListViewItem(new string[] { translation.Text, translation.Text }));
            AutoSizeColumns();
            translation.ResetText();
        }

        private void deleteMI_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in translations.SelectedItems)
                translations.Items.Remove(item);
        }

        private void listContextMenu_Opening(object sender, CancelEventArgs e) {
            if (translations.Items.Count == 0)
                e.Cancel = true;
        }

        private void translations_DragDrop(object sender, DragEventArgs e) {
            var clientXY = translations.PointToClient(new Point(e.X, e.Y));
            
            var dragToItem = translations.GetItemAt(clientXY.X, clientXY.Y);

            int insertIndex = dragToItem == null ? -1 : dragToItem.Index;
            if (insertIndex < 0)
                insertIndex = translations.Items.Count - 1;

            if (e.Effect == DragDropEffects.Move) {
                var item = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                if (item != null && item.ListView == translations) {
                    item.ListView.Items.Remove(item);
                    translations.Items.Insert(insertIndex, item);
                }
            }
        }

        private void translations_ItemDrag(object sender, ItemDragEventArgs e) {
            translations.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void translations_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetFormats().Contains("System.Windows.Forms.ListViewItem"))
                e.Effect = DragDropEffects.Move;
        }

        private void translation_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                addTranslation_Click(sender, e);
        }
    }
}
