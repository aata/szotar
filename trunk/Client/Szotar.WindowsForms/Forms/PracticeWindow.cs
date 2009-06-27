using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Szotar.WindowsForms.Forms {
	public partial class PracticeWindow : Form, IPracticeWindow {
		PracticeQueue queue;
		IPracticeMode mode;

		public PracticeWindow(IList<ListSearchResult> items, PracticeMode whichMode) {
			InitializeComponent();

			mainMenu.Renderer = new ToolStripAeroRenderer(ToolbarTheme.MediaToolbar);

			var terms = DataStore.Database.GetItems(items);
			if (terms.Count > 0) {
				queue = new PracticeQueue(terms);
				SetMode(new FlashcardMode());
			}

			this.FormClosed += delegate {
				if (mode != null)
					mode.Stop();
			};

			components = components ?? new Container();
			components.Add(new DisposableComponent(mode));
		}

		private void SetMode(IPracticeMode mode) {
			this.mode = mode;
			mode.Start(this);
		}

		public PracticeWindow() 
			: this(new ListSearchResult[]{}, PracticeMode.SearchMode) 
		{ }

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

		ToolStrip IPracticeWindow.Controls {
			get { return mainMenu; }
		}

		Panel IPracticeWindow.GameArea {
			get { return panel; }
		}

		int IPracticeWindow.ItemCount {
			get { return queue.Length; }
		}

		int IPracticeWindow.Position {
			get { return queue.Index; }
		}

		private void exit_Click(object sender, EventArgs e) {
			Close();
		}

		private void showStartPage_Click(object sender, EventArgs e) {
			StartPage.ShowStartPage(null);
		}
	}
}
