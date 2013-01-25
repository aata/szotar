using System;
using System.ComponentModel;
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
			List.ListChanged += ListChanged;
			List.ListDeleted += ListDeleted;
		}

		void ListDeleted(object sender, EventArgs e) {
			Item = null;
			RaiseItemDeleted();
		}

		void ListChanged(object sender, ListChangedEventArgs e) {
            if (e.ListChangedType == ListChangedType.ItemDeleted) {
                if (item == null || List == null)
                    return;

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
			List.ListChanged -= ListChanged;
			List.ListDeleted -= ListDeleted;
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
