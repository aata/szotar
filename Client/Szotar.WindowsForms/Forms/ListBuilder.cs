using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class ListBuilder : Form {
		WordList list;
		
		public WordList WordList { get { return list; } }

		public ListBuilder() : 
			this(
				DataStore.Database.CreateSet(
					Properties.Resources.DefaultListName, 
					GuiConfiguration.UserNickname, 
					null, null, DateTime.Now))
		{
			MakeRecent();

			grid.ColumnRatio = GuiConfiguration.ListBuilderColumnRatio;
		}

		public ListBuilder(WordList wordList) {
			InitializeComponent();

			this.list = wordList;
			grid.DataSource = wordList;

			this.UpdateTitle();
			name.Text = list.Name;
			author.Text = list.Author;
			url.Text = list.Url;

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

			swap.Click += new EventHandler(swap_Click);
			remove.Click += new EventHandler(remove_Click);

			WireListEvents();
			MakeRecent();
		}

		/// <summary>Opens a ListBuilder for the given Word List, or focusses an existing ListBuilder 
		/// if one exists.</summary>
		public static ListBuilder Open(long setID) {
			foreach(Form f in Application.OpenForms) {
				var lb = f as ListBuilder;
				if(lb != null && lb.WordList.ID == setID) {
					lb.BringToFront();
					return lb;
				}
			}

			var list = DataStore.Database.GetWordList(setID);
			var form = new ListBuilder(list);
			form.Show();
			return form;
		}

		private void WireListEvents() {
			//list.ListChanged += new ListChangedEventHandler(OnListChanged);
			list.PropertyChanged += new PropertyChangedEventHandler(list_PropertyChanged);
			list.ListDeleted += new EventHandler(list_ListDeleted);
		}

		private void UnwireListEvents() {
			//list.ListChanged -= new ListChangedEventHandler(OnListChanged);
			list.PropertyChanged -= new PropertyChangedEventHandler(list_PropertyChanged);
			list.ListDeleted -= new EventHandler(list_ListDeleted);
		}

		void grid_ColumnRatioChanged(object sender, EventArgs e) {
			GuiConfiguration.ListBuilderColumnRatio = grid.ColumnRatio;
		}

		void list_ListDeleted(object sender, EventArgs e) {
			Close();
		}

		void MakeRecent() {
			var recent = Configuration.RecentLists ?? new List<ListInfo>();

			recent.RemoveAll(e => e.ID == list.ID);

			recent.Insert(0, new ListInfo() {
				Name = list.Name,
				Author = list.Author,
				Url = list.Url,
				Date = list.Date,
				ID = list.ID,
				Language = list.Language,
				TermCount = list.Count
			});

			var max = Configuration.RecentListsSize;
			if (recent.Count > max)
				recent.RemoveRange(max, recent.Count - max);

			Configuration.RecentLists = recent;
		}

		void remove_Click(object sender, EventArgs e) {
			list.RemoveAt(grid.SelectedIndices);
		}

		void swap_Click(object sender, EventArgs e) {
			list.SwapRows(grid.SelectedIndices);
		}

		void swapAll_Click(object sender, EventArgs e) {
			var rows = new List<int>();
			for(int i = 0; i < list.Count; ++i)
				rows.Add(i);

			list.SwapRows(rows);
		}

		//TODO: Get this working.
		//Presumably using the underlying database's sort is easiest.
		void sort_Click(object sender, EventArgs e) {
			//List<TranslationPair> items = new List<TranslationPair>(list);
			//items.Sort();
			//editingList.RaiseListChangedEvents = false;
			//editingList.Clear();
			//foreach (TranslationPair pair in items)
			//    editingList.Add(pair);
			//editingList.RaiseListChangedEvents = true;
			//editingList.ResetBindings();
		}

		private void UpdateTitle() {
			Text = string.Format("{0} - {1}", list.Name, Application.ProductName);
		}

		#region Metadata Bindings
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

		void list_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
				case "Name":
					if(!name.Text.Equals(list.Name))
						name.Text = list.Name;
					this.UpdateTitle();
					break;
				case "Author":
					if(!author.Text.Equals(list.Author))
						author.Text = list.Author;
					break;
				case "Url":
					if (!url.Text.Equals(list.Url))
						url.Text = list.Url;
					break;
			}
		}
		#endregion

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

		private void ListBuilder_Closing(object sender, CancelEventArgs e) {
			UnwireListEvents();
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

			if(sb.Length > 0)
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

		//Metadata
		private void ShowMeta() {
			meta.Visible = true;
			grid.Height -= meta.Height;
		}

		private void HideMeta() {
			meta.Visible = false;
			grid.Height += meta.Height;
		}

		//Public methods
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
				//Don't do anything it if it hasn't changed since Show/HideMeta will break in such cases.
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

		void showStartPage_Click(object sender, EventArgs e) {
			StartPage.ShowStartPage(null);
		}

		private void deleteList_Click(object sender, EventArgs e) {
			// TODO: Confirm this (it can't yet be undone)
			list.DeleteWordList();

			//The form will close automatically, because deleting the list will 
			//fire the list's ListDeleted event, which this form subscribes to.
		}

		private void close_Click(object sender, EventArgs e) {
			Close();
		}

		//The 'valid' count is the count of rows which produced two columns when parsed.
		List<List<string>> ParseCSV(char delim, string text, out int validCount) {
			validCount = 0;
			var lines = new List<List<string>>();
			var line = new List<string>();

			using (var reader = new StringReader(text)) {
				char last = '\0', c;
				int read;
				bool escaped = false;
				var cur = new StringBuilder();
				while ((read = reader.Read()) != -1) {
					c = (char)read;
					if (c == '"') {
						if (last == '"')
							cur.Append(c);
						else if (last == delim)
							escaped = !escaped;
						else
							cur.Append(c);
					} else if (c == '\r') {
					} else if (c == '\n') {
						if (escaped) {
							//Newlines aren't useful to us. Replace them with spaces instead.
							cur.Append(' ');
						} else {
							line.Add(cur.ToString());
							cur.Length = 0;
							lines.Add(line);
							line = new List<string>();
						}
					} else if (c == delim) {
						if (escaped) {
							cur.Append(c);
						} else {
							line.Add(cur.ToString());
							cur.Length = 0;
						}
					} else {
						cur.Append(c);
					}

					last = c;
				}

				if (cur.Length > 0)
					line.Add(cur.ToString());
			}

			if (line.Count > 0)
				lines.Add(line);

			foreach (var x in lines)
				if (x.Count >= 2)
					validCount++;

			return lines;
		}

		// For now, simply insert the new items at the end of the list.
		// There might be a better way to do this.
		void Paste(List<List<string>> lines) {
			var entries = new List<WordListEntry>();

			foreach (var line in lines)
				if (line.Count >= 2)
					entries.Add(new WordListEntry(list, line[0], line[1]));

			if (entries.Count > 0) {
				list.Insert(list.Count, entries);
				grid.Refresh(); //XXX Is this necessary?
			}
		}

		// Detect comma-separated/tab-separated based on the paste content.
		private void pasteCSV_Click(object sender, EventArgs e) {
			int validCSV, validTSV;
			List<List<string>> csv, tsv;
			string text = Clipboard.GetText();
			csv = ParseCSV(',', text, out validCSV);
			tsv = ParseCSV('\t', text, out validTSV);

			if (validTSV >= validCSV)
				Paste(tsv);
			else
				Paste(csv);
		}

		private void practiceThis_Click(object sender, EventArgs e) {
			if (!list.ID.HasValue) {
				ProgramLog.Default.AddMessage(LogType.Error, "WordList {0} has no ID", list.Name);
				return;
			}

			PracticeWindow.OpenNewSession(new[] { new ListSearchResult(list.ID.Value) });
		}
	}
}
