using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Szotar.WindowsForms.Forms {
	public partial class PracticeWindow : Form, IPracticeOverseer {
		PracticeQueue queue;
		IPracticeMode mode;

		public PracticeWindow(IList<ListSearchResult> items, PracticeMode mode) {
			InitializeComponent();

			mainMenu.Renderer = new ToolStripAeroRenderer(ToolbarTheme.MediaToolbar);

			var terms = DataStore.Database.GetItems(items);
			if (terms.Count > 0) {
				queue = new PracticeQueue(terms);
				this.mode = new FlashcardMode();
				this.mode.Start(panel, this);
			}

			this.FormClosed += delegate {
				if (this.mode != null)
					this.mode.Stop();
			};

			components = components ?? new Container();
			components.Add(new DisposableComponent(this.mode));
		}

		public PracticeWindow() : this(new ListSearchResult[]{}, PracticeMode.SearchMode) {
		}

		public static void OpenNewSession(IList<ListSearchResult> items) {
			new PracticeWindow(items, PracticeMode.Default).Show();
		}

		public void MarkSuccess(PracticeItem item) {
		}

		public void MarkFailure(PracticeItem item) {
		}

		public PracticeItem FetchNextItem() {
			return queue.TakeOne();
		}
	}
}
