using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class ListBuilder : Form {
		WordList list;
		bool isNewList = false;

		Control queuedUpdateControl;
		Action queuedUpdateAction;
		Timer queuedUpdateTimer;

		public WordList WordList { get { return list; } }

		public ListBuilder() :
			this(
				DataStore.Database.CreateSet(
					Properties.Resources.DefaultListName,
					GuiConfiguration.UserNickname,
					null, null, DateTime.Now)) 
		{
			isNewList = true;
			MakeRecent();
		}

        // Constructor is protected in order to make sure no other word lists are open for this list.
		protected ListBuilder(WordList wordList) {
			InitializeComponent();

			this.list = wordList;
			grid.DataSource = wordList;

			this.UpdateTitle();
			name.Text = list.Name;
			author.Text = list.Author;
			url.Text = list.Url;
            UpdateEntryCount();

			grid.ColumnRatio = GuiConfiguration.ListBuilderColumnRatio;
			meta.Height = GuiConfiguration.ListBuilderMetadataSectionHeight;

			this.Layout += new LayoutEventHandler(ListBuilder_Layout);
			this.Closing += new CancelEventHandler(ListBuilder_Closing);
			editMetadata.Click += new EventHandler(editMetadata_Click);
			copyAsCsv.Click += new EventHandler(copyAsCsv_Click);
			sort.Click += new EventHandler(sort_Click);
			swapAll.Click += new EventHandler(swapAll_Click);
			shadow.MouseDown += new MouseEventHandler(shadow_MouseDown);
			shadow.MouseMove += new MouseEventHandler(shadow_MouseMove);
			name.TextChanged += new EventHandler(name_TextChanged);
			author.TextChanged += new EventHandler(author_TextChanged);
			url.TextChanged += new EventHandler(url_TextChanged);
			showStartPage.Click += new EventHandler(showStartPage_Click);
			close.Click += new EventHandler(close_Click);
			deleteList.Click += new EventHandler(deleteList_Click);
			grid.ColumnRatioChanged += new EventHandler(grid_ColumnRatioChanged);

			undo.Click += delegate { list.Undo(); };
			redo.Click += delegate { list.Redo(); };
			editMenu.DropDownOpening += new EventHandler(editMenu_DropDownOpening);
			itemContextMenu.Opening += new CancelEventHandler(itemContextMenu_Opening);
            mainMenu.Items.Add(new TagMenu() { WordList = list });

			grid.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(grid_ColumnHeaderMouseClick);

			cutMI.Click += delegate { grid.Cut(); };
			copyMI.Click += delegate { grid.Copy(); };
			pasteMI.Click += delegate { grid.Paste(); };
			cutCM.Click += delegate { grid.Cut(); };
			copyCM.Click += delegate { grid.Copy(); };
			pasteCM.Click += delegate { grid.Paste(); };

			swap.Click += new EventHandler(swap_Click);

			deleteMI.Click += new EventHandler(remove_Click);
			deleteCM.Click += new EventHandler(remove_Click);



			WireListEvents();
			MakeRecent();

			queuedUpdateTimer = new Timer() { Interval = 150, Enabled = false };
			components.Add(queuedUpdateTimer);
			queuedUpdateTimer.Tick += new EventHandler(queuedUpdateTimer_Tick); 
		}

		/// <summary>Opens a ListBuilder for the given Word List, or focusses an existing ListBuilder 
		/// if one exists.</summary>
		public static ListBuilder Open(long setID) {
			foreach (Form f in Application.OpenForms) {
				var lb = f as ListBuilder;
				if (lb != null && lb.WordList.ID == setID) {
					lb.BringToFront();
					return lb;
				}
			}

			var list = DataStore.Database.GetWordList(setID);
            if (list == null)
                return null;

			var form = new ListBuilder(list);
			form.Show();
			return form;
		}

		private void WireListEvents() {
			list.PropertyChanged += new PropertyChangedEventHandler(list_PropertyChanged);
			list.ListDeleted += new EventHandler(list_ListDeleted);
			list.ListChanged += new ListChangedEventHandler(list_ListChanged);
		}

		private void UnwireListEvents() {
			list.PropertyChanged -= new PropertyChangedEventHandler(list_PropertyChanged);
			list.ListDeleted -= new EventHandler(list_ListDeleted);
			list.ListChanged -= new ListChangedEventHandler(list_ListChanged);
		}

		void grid_ColumnRatioChanged(object sender, EventArgs e) {
			GuiConfiguration.ListBuilderColumnRatio = grid.ColumnRatio;
		}

		void list_ListDeleted(object sender, EventArgs e) {
			// Suppress the "would you like to keep this list?" message.
			this.Closing -= new CancelEventHandler(ListBuilder_Closing);
			UnwireListEvents();

			Close();
		}

		void MakeRecent() {
            list.Accessed = DateTime.Now;
            DataStore.Database.RaiseWordListOpened(list.ID);
		}

		void remove_Click(object sender, EventArgs e) {
			list.RemoveAt(new List<int>(grid.SelectedEntryIndices));
		}

		void swap_Click(object sender, EventArgs e) {
			list.SwapRows(new List<int>(grid.SelectedEntryIndices));
		}

		void swapAll_Click(object sender, EventArgs e) {
			var rows = new List<int>();
			for (int i = 0; i < list.Count; ++i)
				rows.Add(i);

			list.SwapRows(rows);
		}

		void sort_Click(object sender, EventArgs e) {
			list.Sort((a, b) => a.Phrase.CompareTo(b.Phrase));
		}

		int? sortColumn;
		bool sortAscending;
		void grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
			int direction = 1;

			if (sortColumn == e.ColumnIndex) {
				sortAscending = !sortAscending;
				direction = sortAscending ? 1 : -1;
			} else {
				sortAscending = true;
			}

			if (e.ColumnIndex == 0) {
				list.Sort((a, b) => direction * a.Phrase.CompareTo(b.Phrase));
				sortColumn = 0;
			} else if (e.ColumnIndex == 1) {
				list.Sort((a, b) => direction * a.Translation.CompareTo(b.Translation));
				sortColumn = 1;
			}
		}

		void list_ListChanged(object sender, ListChangedEventArgs e) {
			sortColumn = null;

            if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.Reset)
                UpdateEntryCount();
		}

		private void UpdateTitle() {
			Text = string.Format("{0} - {1}", list.Name, Application.ProductName);
		}

		bool CanCut() {
			return CanCopy();
		}

		bool CanCopy() {
			return grid.SelectedEntryCount > 0;
		}

		bool CanPaste() {
			return grid.CanPaste;
		}

		void editMenu_DropDownOpening(object sender, EventArgs e) {
			string undoDesc = list.UndoDescription;
			string redoDesc = list.RedoDescription;

			if (undoDesc == null) {
				undo.Enabled = false;
				undo.Text = Properties.Resources.Undo;
			} else {
				undo.Enabled = true;
				undo.Text = string.Format(Properties.Resources.UndoSpecific, undoDesc);
			}

			if (redoDesc == null) {
				redo.Enabled = false;
				redo.Text = Properties.Resources.Redo;
			} else {
				redo.Enabled = true;
				redo.Text = string.Format(Properties.Resources.RedoSpecific, redoDesc);
			}

			cutMI.Enabled = CanCut();
			copyMI.Enabled = CanCopy();
			pasteMI.Enabled = CanPaste();
		}

		void itemContextMenu_Opening(object sender, CancelEventArgs e) {
			cutCM.Enabled = CanCut();
			copyCM.Enabled = CanCopy();
			pasteCM.Enabled = CanPaste();
		}

		#region Metadata Bindings
		void queuedUpdateTimer_Tick(object sender, EventArgs e) {
			queuedUpdateTimer.Stop();

			if (queuedUpdateControl != null && queuedUpdateAction != null) {
				queuedUpdateControl = null;
				queuedUpdateAction();
			}
		}

		void QueueControlUpdate(Control control, Action action) {
			if (queuedUpdateControl == control) {
				queuedUpdateTimer.Stop();
				queuedUpdateTimer.Start();
				return;
			}

			// If there's already a queued update on a different control, execute it.
			if (queuedUpdateControl != null) {
				queuedUpdateTimer.Stop();
				queuedUpdateAction();
			}

			queuedUpdateAction = action;
			queuedUpdateControl = control;
			queuedUpdateTimer.Start();
		}

		// TODO: The database will be updated every time a character is typed... not good.
		// It would be better if the database update were delayed until no keys have been
		// pressed for, say, 200 milliseconds.
		void url_TextChanged(object sender, EventArgs e) {
			list.Url = url.Text;
		}

		void author_TextChanged(object sender, EventArgs e) {
			list.Author = author.Text;
		}

		void name_TextChanged(object sender, EventArgs e) {
			list.Name = name.Text;
			MakeRecent();
		}

        void UpdateEntryCount() {
            entriesLabel.Text = string.Format(Properties.Resources.NEntries, list.Count);
        }

		void list_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
				case "Name":
					if (!name.Text.Equals(list.Name))
						name.Text = list.Name;
					this.UpdateTitle();
					break;
				case "Author":
					if (!author.Text.Equals(list.Author))
						author.Text = list.Author;
					break;
				case "Url":
					if (!url.Text.Equals(list.Url))
						url.Text = list.Url;
					break;
			}
		}
		#endregion

        #region Drop Shadow
        public class ShadowTag {
			public System.Drawing.Point Down { get; set; }
			public int OriginalHeight { get; set; }
		}

		private void shadow_MouseMove(object sender, MouseEventArgs e) {
			ShadowTag tag = shadow.Tag as ShadowTag;
			if (e.Button == MouseButtons.Left && tag != null) {
				System.Drawing.Point down = tag.Down;
				meta.Height += down.Y - e.Y;
				UpdateGridHeight();
				GuiConfiguration.ListBuilderMetadataSectionHeight = meta.Height;
			}
		}

		private void shadow_MouseDown(object sender, MouseEventArgs e) {
			shadow.Tag = new ShadowTag { Down = e.Location, OriginalHeight = shadow.Height };
		}
        #endregion

        private void ListBuilder_Closing(object sender, CancelEventArgs e) {
			UnwireListEvents();

			if (isNewList && list.Count == 0) {
				var dr = MessageBox.Show(
					Properties.Resources.ConfirmKeepNewWordList,
					Properties.Resources.ConfirmKeepNewWordListCaption,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button1);

				if (dr == DialogResult.Cancel) {
					e.Cancel = true;
					return;
				} else if (dr == DialogResult.No) {
					list.DeleteWordList();
				}
			}
		}

		private void editMetadata_Click(object sender, EventArgs e) {
			editMetadata.Checked = !editMetadata.Checked;
			if (editMetadata.Checked)
				ShowMeta();
			else
				HideMeta();
		}

		private void copyAsCsv_Click(object sender, EventArgs e) {
			var sb = new StringBuilder();

			foreach (WordListEntry pair in list) {
				var phrase = pair.Phrase;
				var translation = pair.Translation;

				bool quotePhrase = phrase.Contains("\"") || phrase.Contains(",") || phrase.Contains("\n"),
					quoteTranslation = translation.Contains("\"") || translation.Contains(",") || translation.Contains("\n");
				if (quotePhrase)
					sb.Append('"');
				sb.Append(phrase.Replace("\"", "\"\""));
				if (quotePhrase)
					sb.Append('"');

				sb.Append(',');

				if (quoteTranslation)
					sb.Append('"');
				sb.Append(translation.Replace("\"", "\"\""));
				if (quoteTranslation)
					sb.Append('"');

				sb.AppendLine();
			}

			if (sb.Length > 0)
				Clipboard.SetText(sb.ToString());
		}

		private void ListBuilder_Layout(object sender, LayoutEventArgs e) {
			grid.Top = mainMenu.Bottom;
			UpdateGridHeight();
			grid.Width = ClientSize.Width;
		}

		private void UpdateGridHeight() {
			grid.Height = ClientSize.Height - grid.Top - (meta.Visible ? meta.Height : 0);
		}

		private void ShowMeta() {
			meta.Visible = true;
			grid.Height -= meta.Height;
		}

		private void HideMeta() {
			meta.Visible = false;
			grid.Height += meta.Height;
		}

		public void AddPair(string phrase, string translation) {
			System.Diagnostics.Debug.Assert(!InvokeRequired);

			list.Add(new WordListEntry(list, phrase, translation));
			grid.DataSource = list;
		}

		public void AddEntries(IEnumerable<TranslationPair> entries) {
			System.Diagnostics.Debug.Assert(!InvokeRequired);

			var realEntries = new List<WordListEntry>();
			foreach (var entry in entries)
				realEntries.Add(new WordListEntry(list, entry.Phrase, entry.Translation));

			list.Insert(list.Count, realEntries);

			grid.DataSource = list;
		}

		public bool ShowMetadata {
			get {
				return meta.Visible;
			}
			set {
				// Don't do anything it if it hasn't changed since Show/HideMeta will break in such cases.
				if (value != ShowMetadata) {
					if (value)
						ShowMeta();
					else
						HideMeta();
				}
			}
		}

		public void ScrollToPosition(int position) {
			grid.ScrollToIndex(position);
		}

        public bool ScrollToItem(string phrase, string translation) {
            for (int i = 0; i < list.Count; i++) {
                if (list[i].Phrase == phrase && list[i].Translation == translation) {
                    ScrollToPosition(i);
                    return true;
                }
            }
            return false;
        }

		void showStartPage_Click(object sender, EventArgs e) {
            ShowForm.Show<StartPage>();
		}

		private void deleteList_Click(object sender, EventArgs e) {
			var dr = MessageBox.Show(
				Properties.Resources.ConfirmDeleteWordList,
				Properties.Resources.ConfirmDeleteWordListCaption,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button1);

			if (dr == DialogResult.OK) {
				list.DeleteWordList();

				// The form will close automatically, because deleting the list will 
				// fire the list's ListDeleted event, which this form subscribes to.
			}
		}

		private void close_Click(object sender, EventArgs e) {
			Close();
		}

		// For now, simply insert the new items at the end of the list.
		// There might be a better way to do this.
		void Paste(List<List<string>> lines, int? row) {
			var entries = new List<WordListEntry>();

			foreach (var line in lines)
				if (line.Count >= 2)
					entries.Add(new WordListEntry(list, line[0], line[1]));

			if (entries.Count > 0) {
				list.Insert(row ?? list.Count, entries);
				grid.Refresh(); // XXX Is this necessary?
			}
		}

		// Detect comma-separated/tab-separated based on the paste content.
		private void pasteCSV_Click(object sender, EventArgs e) {
			int validCSV, validTSV;
			List<List<string>> csv, tsv;

			// TODO: Look at getting CSV directly from the clipboard.
			// It's more work than it sounds: there doesn't seem to be a consensus on what the exact
			// format of that data is.
			// Excel in particular writes it in UTF-8 (or the windows code page, according to some)
			// with a null terminator.

			// It's plain text: use guesswork to figure out if it's TSV or CSV.
			// Tab-separated is rarer, so if it works with tabs, it's probably that.
			string text = Clipboard.GetText();
			csv = CsvUtilities.ParseCSV(',', text, out validCSV);
			tsv = CsvUtilities.ParseCSV('\t', text, out validTSV);

			if (validTSV >= validCSV)
				Paste(tsv, null);
			else
				Paste(csv, null);
		}

		private void Practice(PracticeMode mode) {
			if (!list.ID.HasValue) {
				ProgramLog.Default.AddMessage(LogType.Error, "WordList {0} has no ID", list.Name);
				return;
			}

			PracticeWindow.OpenNewSession(mode, new[] { new ListSearchResult(list.ID.Value) });

		}

		private void flashcards_Click(object sender, EventArgs e) {
			Practice(PracticeMode.Flashcards);
		}

		private void learn_Click(object sender, EventArgs e) {
			Practice(PracticeMode.Learn);
		}
	}
}
