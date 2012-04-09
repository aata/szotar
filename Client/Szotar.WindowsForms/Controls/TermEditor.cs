using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
    public partial class TermEditor : UserControl {
        WordListEntry item;

        public TermEditor() {
            InitializeComponent();
        }

        protected override void OnControlRemoved(ControlEventArgs e) {
            Item = null;

            base.OnControlRemoved(e);
        }

        public WordList List {
            get { return item == null ? null : item.Owner; }
        }

        public WordListEntry Item {
            get { return item; }
            set {
                if (item == value)
                    return;
                if (item != null)
                    UnwireEventHandlers();
                item = value;
                if (item != null)
                    WireEventHandlers();
                Update();
            }
        }

        private void WireEventHandlers() {
            List.ListChanged += new ListChangedEventHandler(list_ListChanged);
            List.ListDeleted += new EventHandler(list_ListDeleted);
        }

        void list_ListDeleted(object sender, EventArgs e) {
            Item = null;
            RaiseItemDeleted();
        }

        void list_ListChanged(object sender, ListChangedEventArgs e) {
            if (e.ListChangedType == ListChangedType.ItemDeleted) {
                if (!List.Contains(item)) {
                    Item = null;
                    RaiseItemDeleted();
                }
            }
        }

        public event EventHandler ItemDeleted;
        void RaiseItemDeleted() {
            var h = ItemDeleted;
            if (h != null)
                h(this, new EventArgs());
        }

        private void UnwireEventHandlers() {
            List.ListChanged -= new ListChangedEventHandler(list_ListChanged);
            List.ListDeleted -= new EventHandler(list_ListDeleted);
        }

        new void Update() {
            if (item != null) {
                phrase.Text = item.Phrase;
                translation.Text = item.Translation;
                phrase.Enabled = translation.Enabled = true;
            } else {
                phrase.Clear();
                translation.Clear();
                phrase.Enabled = translation.Enabled = false;
            }
        }

        public void Save() {
            if (item != null) {
                item.Phrase = phrase.Text;
                item.Translation = translation.Text;
            }
        }
    }
}
