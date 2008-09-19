using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Text;

namespace Szotar.WindowsForms.Forms {
	public partial class LookupForm : Form {
		public IBilingualDictionary Dictionary { get; private set; }

		SearchMode searchMode, displayedSearchMode;
		ListBuilder listBuilder;

		CultureInfo sourceCulture;
		CultureInfo targetCulture;

		IList<SearchResult> results;
		bool ctrlHeld = false;

		class LookupFormFileIsInUse : FileIsInUse {
			LookupForm form;

			public LookupFormFileIsInUse(LookupForm form, string path)
				: base(path)
			{
				this.form = form;
				base.CanClose = true;
				base.WindowHandle = form.Handle;
			}

			//Who knows what thread this will be invoked on?
			public override void CloseFile() {
				form.Invoke(new EventHandler((s, e) => {
					form.Close();
				}));
			}
		}

		public LookupForm(IBilingualDictionary dictionary)
			: this()
		{			
			components.Add(new DisposableComponent(new LookupFormFileIsInUse(this, dictionary.Path)));
			components.Add(new DisposableComponent(dictionary));

			Dictionary = dictionary;

			//Set the menu items' Text to match the names of the dictionary sections.
			forwards.Text = Dictionary.ForwardsSection.Name;
			backwards.Text = Dictionary.ReverseSection.Name;

			//TODO: This really needs testing. It could make things completely unusable...
			//I can't even remember if they're used...
			try {
				if (dictionary.FirstLanguageCode != null && dictionary.SecondLanguage != null) {
					sourceCulture = new CultureInfo(dictionary.FirstLanguageCode);
					targetCulture = new CultureInfo(dictionary.SecondLanguageCode);
				}
			} catch (ArgumentException) {
				//One of the cultures wasn't supported. In that case, set both cultures to null, because it
				//isn't worth having only one.
				sourceCulture = targetCulture = null;
			}

			InitialiseView();

			mainMenu.Renderer = contextMenu.Renderer = toolStripPanel.Renderer = new ToolStripAeroRenderer(ToolbarTheme.CommunicationsToolbar);
		}

		public LookupForm(DictionaryInfo dictionaryInfo)
			: this(dictionaryInfo.GetFullInstance()) 
		{
		}

		private LookupForm() {
			InitializeComponent();
			RegisterSettingsChangedEventHandlers();

			Icon = Properties.Resources.DictionaryIcon;
		}

		private void RemoveEventHandlers() {
			UnregisterSettingsChangedEventHandlers();
		}

		//Call this after the dictionaries are initialised.
		private void InitialiseView() {
			//Updates the mode switching button.
			SearchMode = SearchMode;

			this.Closed += new EventHandler(LookupForm_Closed);
			searchBox.TextChanged += new EventHandler(searchBox_TextChanged);
			this.InputLanguageChanged += new InputLanguageChangedEventHandler(LookupForm_InputLanguageChanged);
			grid.CellFormatting += new DataGridViewCellFormattingEventHandler(grid_CellFormatting);
			grid.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged);
			
			//Show custom tooltips that don't get in the way of the mouse and don't disappear so quickly.
			grid.MouseMove += new MouseEventHandler(grid_MouseMove);
			grid.MouseLeave += new EventHandler(grid_MouseLeave);
			grid.ShowCellToolTips = false;

			UpdateResults();

			//By now, the columns should have been created.
			grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			grid.Columns[0].FillWeight = GuiConfiguration.LookupFormColumn1FillWeight;
			grid.Columns[1].Resizable = DataGridViewTriState.False;

			ignoreAccentsCheck.CheckedChanged += new EventHandler(ignoreAccentsCheck_CheckedChanged);
			ignoreCaseCheck.CheckedChanged += new EventHandler(ignoreCaseCheck_CheckedChanged);

			this.Shown += new EventHandler(LookupForm_Shown);
			this.KeyDown += (s, e) => { if (e.KeyCode == Keys.ControlKey) ctrlHeld = true; };
			this.KeyUp += (s, e) => { if(e.KeyCode == Keys.ControlKey) ctrlHeld = false; };
		}

		void LookupForm_Shown(object sender, EventArgs e) {
			this.PerformLayout();
			searchBox.Focus();
		}

		#region Appearance
		void grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
			MatchType matchType = results != null && e.RowIndex < results.Count ? results[e.RowIndex].MatchType : MatchType.NormalMatch;

			switch (matchType) {
				case MatchType.PerfectMatch:
					e.CellStyle.BackColor = Color.DarkGoldenrod;
					e.CellStyle.ForeColor = Color.White;
					break;
				case MatchType.StartMatch:
					e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? Color.LightGoldenrodYellow : Color.PaleGoldenrod;
					break;
				default:
					e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? Color.White : Color.WhiteSmoke;
					break;
			}
		}
		#endregion

		#region Settings Bindings
		//If I do re-enable this, I should at least remove the handlers when the form closes.
		private void RegisterSettingsChangedEventHandlers() {
			Configuration.Default.SettingChanged += new EventHandler<SettingChangedEventArgs>(SettingChanging);
		}

		private void UnregisterSettingsChangedEventHandlers() {
			Configuration.Default.SettingChanged -= new EventHandler<SettingChangedEventArgs>(SettingChanging);
		}

		//Update UI state.
		void SettingChanging(object sender, SettingChangedEventArgs e) {
			if (e.SettingName == "IgnoreAccents" || e.SettingName == "IgnoreCase") {
				//This shouldn't fire the CheckedChanged/SettingChanged events in an infinite loop.
				if (e.SettingName == "IgnoreAccents")
					ignoreAccentsMenuItem.Checked = ignoreAccentsCheck.Checked = GuiConfiguration.IgnoreAccents;
				else if (e.SettingName == "IgnoreCase")
					ignoreCaseMenuItem.Checked = ignoreCaseCheck.Checked = GuiConfiguration.IgnoreCase;

				//We might want to skip this if nothing was actually changed.
				UpdateResults();
			}
			/*else if (e.SettingName == "LookupFormColumn1FillWeight") {
				grid.Columns[0].FillWeight = Properties.Settings.Default.LookupFormColumn1FillWeight;
			}*/
			//<- naturally, this breaks when it's actually this form that is doing the resizing.
		}

		void grid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e) {
			GuiConfiguration.LookupFormColumn1FillWeight = grid.Columns[0].FillWeight;
		}
		#endregion

		#region Search code
		private IDictionarySection GetSectionBySearchMode(SearchMode mode) {
			return mode == SearchMode.Forward ? Dictionary.ForwardsSection : Dictionary.ReverseSection;
		}

		private void UpdateResults() {
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();

			ISearchDataSource dict = this.GetSectionBySearchMode(this.SearchMode);
			SearchMode finalSearchMode = this.SearchMode;

			string search = searchBox.RealText.Trim().Normalize();

			results = new List<SearchResult>();
			int foundAt = -1;
			bool hadPerfect = false;
			int i = 0;

			//Look for the first result containing the search term at the start.
			foreach (SearchResult result in dict.Search(search, ignoreAccentsCheck.Checked, ignoreCaseCheck.Checked)) {
				if (result.MatchType != MatchType.NormalMatch && foundAt < 0 && !hadPerfect)
					foundAt = i;
				if (result.MatchType == MatchType.PerfectMatch && !hadPerfect) {
					foundAt = i;
					hadPerfect = true;
				}

				results.Add(result);
				i++;
			}

			if (results.Count == 0) {
				finalSearchMode = SearchMode == SearchMode.Backward ? SearchMode.Forward : SearchMode.Backward;
				dict = this.GetSectionBySearchMode(finalSearchMode);
				foreach (SearchResult result in dict.Search(search, ignoreAccentsCheck.Checked, ignoreCaseCheck.Checked)) {
					if (result.MatchType != MatchType.NormalMatch && foundAt < 0 && !hadPerfect)
						foundAt = i;
					if (result.MatchType == MatchType.PerfectMatch && !hadPerfect) {
						foundAt = i;
						hadPerfect = true;
					}

					results.Add(result);
					i++;
				}
				if (results.Count == 0)
					finalSearchMode = SearchMode;
			}

			grid.VirtualMode = true;
			grid.DataSource = results;

			grid.Columns[0].HeaderText = finalSearchMode == SearchMode.Forward ? Dictionary.FirstLanguage : (Dictionary.SecondLanguageReverse ?? Dictionary.SecondLanguage);
			grid.Columns[1].HeaderText = finalSearchMode == SearchMode.Forward ? Dictionary.SecondLanguage : (Dictionary.FirstLanguageReverse ?? Dictionary.FirstLanguage);

			if (foundAt >= 0) {
				grid.FirstDisplayedScrollingRowIndex = foundAt;
				grid.FirstDisplayedScrollingColumnIndex = 0;
			}

			displayedSearchMode = finalSearchMode;
			stopwatch.Stop();

			grid.ClearSelection();
			grid.PerformLayout();

			Text = string.Format(CultureInfo.CurrentUICulture, "{0} - {1} results ({2} ms)", Application.ProductName, results.Count, stopwatch.ElapsedMilliseconds);
		}

		private void SwitchMode() {
			SearchMode = SearchMode == SearchMode.Forward ? SearchMode.Backward : SearchMode.Forward;
		}

		private void FocusSearchField() {
			searchBox.Focus();
			searchBox.SelectAll();
		}
		#endregion

		#region ToolTip
		ToolTip infoTip;
		int infoTipRow;
		string infoTipText;
		Point currentInfoTipMouseLocation;

		string GetInfoTipTitle(int rowIndex) {
			if (rowIndex < 0 || rowIndex > results.Count)
				return null;
			return results[rowIndex].Phrase;
		}

		string GetInfoTipText(int rowIndex) {
			if (rowIndex < 0 || rowIndex > results.Count)
				return null;

			//Use the cached version in the current tooltip if possible.
			if (infoTipRow == rowIndex)
				return infoTipText;

			SearchMode dsm = DisplayedSearchMode;
			Entry entry = results[rowIndex].Entry;
			if (entry.Translations == null)
				GetSectionBySearchMode(DisplayedSearchMode).GetFullEntry(entry);

			ISearchDataSource otherSide = GetSectionBySearchMode(dsm == SearchMode.Forward ? SearchMode.Backward : SearchMode.Forward);
			StringBuilder sb = new StringBuilder();
			foreach (Translation term in entry.Translations) {
				if (sb.Length > 0)
					sb.Append(",");
				sb.Append(term.Value);
			}
			string search = sb.ToString();
			sb.Length = 0;

			foreach (SearchResult sr in otherSide.Search(search, false, false)) {
				if (sr.MatchType == MatchType.PerfectMatch) {
					sb.AppendLine(SanitizeToolTipLine(sr.Phrase + " -> " + sr.Translation));
				}
			}

			if (sb.Length > 0)
				return sb.ToString();
			return null;
		}
		
		//Remove some common annoyances with tooltips (really wide tooltips).
		//Currently not needed (tooltip size is limited, text wraps)
		string SanitizeToolTipLine(string line) {
			return line;

			const int maxWidth = 200;
			if (line.Length < maxWidth)
				return line;

			var sb = new StringBuilder();
			while (line.Length > maxWidth) {
				if(sb.Length > 0)
					sb.Append("\t");

				//Find first space before maxWidth characters.
				int i = line.LastIndexOf(" ", maxWidth);
				if(i == -1) {
					//No spaces before maxWidth?! As a last resort, increase the width a little.
					i = line.LastIndexOf(" ", maxWidth + 20);
					if(i == -1) {
						sb.AppendLine(line);
						return sb.ToString();
					}
				}

				sb.AppendLine(line.Substring(0, i));
				line = line.Substring(i + 1).Trim();
			}

			sb.AppendLine(line);

			return sb.ToString();
		}

		void grid_MouseMove(object sender, MouseEventArgs e) {
			if(e.Button != MouseButtons.None)
				return;

			if (e.Location == currentInfoTipMouseLocation)
				return;
			currentInfoTipMouseLocation = e.Location;

			var hitTest = grid.HitTest(e.X, e.Y);
			if(hitTest.Type != DataGridViewHitTestType.Cell && hitTest.Type != DataGridViewHitTestType.RowHeader) {
				if(infoTip != null && infoTip.Active)
					infoTip.Hide(grid);
				infoTipText = null;
				infoTipRow = -1;
				return;
			}

			if (hitTest.RowIndex == infoTipRow)
				return;

			string text = GetInfoTipText(hitTest.RowIndex);

			if (text == null) {
				if(infoTip != null)
					infoTip.Hide(grid);
				return;
			}

			infoTipRow = hitTest.RowIndex;
			infoTipText = text;

			if (infoTip == null) {
				infoTip = new ToolTip(components);
				infoTip.StripAmpersands = false;
				infoTip.UseAnimation = false;
				infoTip.Popup += (s, e3) => { e3.ToolTipSize = new Size(Math.Min(e3.ToolTipSize.Width, grid.Width), e3.ToolTipSize.Height);  };
			}

			infoTip.ToolTipTitle = GetInfoTipTitle(hitTest.RowIndex);

			int offset = grid.GetRowDisplayRectangle(hitTest.RowIndex, true).Height;
			infoTip.Show(text, grid, e.X + offset, e.Y + offset);
		}

		void grid_MouseLeave(object sender, EventArgs e) {
			if (infoTip != null && infoTip.Active)
				infoTip.Hide(grid);
		}
		#endregion

		#region Properties
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Description("Specifies the initial searching mode of the form.")]
		[DefaultValue(typeof(SearchMode), "SearchMode.Forward")]
		[Category("Search")]
		public SearchMode SearchMode {
			get { return searchMode; }
			set {
				searchMode = value;
				if (SearchMode == SearchMode.Forward)
					switchMode.Text = forwards.Text;
				else
					switchMode.Text = backwards.Text;
				UpdateResults();
			}
		}

		public SearchMode DisplayedSearchMode {
			get { return displayedSearchMode; }
			set { displayedSearchMode = value; }
		}
		#endregion

		#region Layout code
		private void UpdateGridPosition() {
			grid.Height = ClientSize.Height - grid.Top;
			grid.Width = ClientSize.Width;
		}
		#endregion

		#region Misc Control Events
		/// <summary>
		/// Updates the current search results to reflect the new search terms.
		/// </summary>
		private void searchBox_TextChanged(object sender, EventArgs e) {
			UpdateResults();
		}

		/// <summary>
		/// Switches the current search mode from Forward to Backward (and vice versa), and focusses the 
		/// search field.
		/// </summary>
		private void switchMode_Click(object sender, EventArgs e) {
			SwitchMode();
			FocusSearchField();
		}

		private class DraggableRowSet {
			IList<TranslationPair> rows;

			public DraggableRowSet(List<SearchResult> rows) {
				this.rows = rows.ConvertAll(x => new TranslationPair(x.Phrase, x.Translation));
			}

			public override string ToString() {
				var sb = new System.Text.StringBuilder();
				foreach (TranslationPair row in rows) {
					sb.AppendLine(string.Format("{0} -- {1}", row.Phrase, row.Translation));
				}

				return sb.ToString();
			}
		}

		//TODO: Need to find out why the list scrolls to the beginning on the first click
		//(apparently only if there's a lot of entries, though.)
		private void grid_MouseDown(object sender, MouseEventArgs e) {
			return;
			if(e.Button == MouseButtons.Left) {
				var hit = grid.HitTest(e.X, e.Y);

				var indices = new List<int>();

				if (hit.RowIndex >= 0) {
					if (grid.Rows[hit.RowIndex].Selected)
						foreach (DataGridViewRow row in grid.SelectedRows)
							indices.Add(row.Index);
					else if (ctrlHeld)
						indices.Add(hit.RowIndex);
				} else {
					return;
				}

				if(indices.Count > 0) {
					indices.Sort();
					var rowset = new DraggableRowSet(indices.ConvertAll(i => results[i]));

					DataObject data = new DataObject(rowset);
					data.SetText(rowset.ToString());
					grid.DoDragDrop(data, DragDropEffects.Copy);
				}
			}
		}

		private void grid_CellMouseDoubleClick(object sender, DataGridViewCellEventArgs e) {
			if(results != null && e.RowIndex >= 0) {
				SearchResult sr = results[e.RowIndex];

				var sb = new StringBuilder();
				var otherSide = GetSectionBySearchMode(DisplayedSearchMode == SearchMode.Backward ? SearchMode.Forward : SearchMode.Backward);
				
				foreach (Translation t in sr.Entry.Translations) {
					sb.Append(t.Value);
					sb.Append(": ");
					int n = 0;
					foreach(var rsr in otherSide.Search(t.Value, false, false)) {
						if (rsr.MatchType == MatchType.PerfectMatch) {
							foreach (var tr in rsr.Entry.Translations) {
								if (n++ > 0)
									sb.Append(", ");
								sb.Append(tr.Value);
							}
						}
					}
					sb.AppendLine();
				}

				MessageBox.Show(sb.ToString());
			}
		}
		#endregion

		#region Form events
		/// <summary>
		/// Some keyboard shortcuts to invoke the GC. Quite useful for testing how much memory is really
		/// in use. Also clears the search when Escape is pressed.
		/// </summary>
		private void LookupForm_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.F9)
				GC.Collect(GC.MaxGeneration);
			else if (e.KeyCode == Keys.F8)
				GC.Collect(1);
			else if (e.KeyCode == Keys.F7)
				GC.Collect(0);
			else if (e.KeyCode == Keys.Escape)
				searchBox.Text = string.Empty;
		}

		/// <summary>
		/// The ListBuilder associated with this form was closed, so remove the reference to it.
		/// </summary>
		void listBuilder_Closed(object sender, EventArgs e) {
			((Form)sender).Closed -= new EventHandler(listBuilder_Closed);
			listBuilder = null;
		}

		void LookupForm_Closed(object sender, EventArgs e) {
			if (listBuilder != null)
				listBuilder.Close();
			GuiConfiguration.IgnoreCase = ignoreCaseCheck.Checked;
			GuiConfiguration.IgnoreAccents = ignoreAccentsCheck.Checked;
			GuiConfiguration.Save();

			RemoveEventHandlers();

			//This is done as a hint to the GC, because forms which are open for a long time 
			//might not trigger a gen2 collection when closing. 
			//We want to reclaim the memory in case there are other windows open.
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
		}

		void LookupForm_InputLanguageChanged(object sender, InputLanguageChangedEventArgs e) {
			if (targetCulture != null && targetCulture.Equals(e.InputLanguage.Culture)) {
				SearchMode = SearchMode.Backward;
			} else if (sourceCulture != null && sourceCulture.Equals(e.InputLanguage.Culture)) {
				SearchMode = SearchMode.Forward;
			}
		}
		#endregion

		#region Menu Events
		#region File Menu
		/// <summary>
		/// Shows the start page. Attempt to find an existing start page, or create one if none exists.
		/// </summary>
		private void showStartPage_Click(object sender, EventArgs e) {
			StartPage.ShowStartPage();
		}
		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Close();
		}
		#endregion

		#region Search Menu
		private void forwards_Click(object sender, EventArgs e) {
			SearchMode = SearchMode.Forward;
			FocusSearchField();
		}

		private void backwards_Click(object sender, EventArgs e) {
			SearchMode = SearchMode.Backward;
			FocusSearchField();
		}

		private void switchModeMenuItem_Click(object sender, EventArgs e) {
			SwitchMode();
			FocusSearchField();
		}

		private void focusSearchField_Click(object sender, EventArgs e) {
			FocusSearchField();
		}

		private void clearSearchToolStripMenuItem_Click(object sender, EventArgs e) {
			searchBox.Text = String.Empty;
		}

		private void ignoreAccentsMenuItem_CheckedChanged(object sender, EventArgs e) {
			GuiConfiguration.IgnoreAccents = ignoreAccentsMenuItem.Checked;
		}

		private void ignoreCaseMenuItem_Click(object sender, EventArgs e) {
			GuiConfiguration.IgnoreCase = ignoreCaseMenuItem.Checked;
		}

		void ignoreCaseCheck_CheckedChanged(object sender, EventArgs e) {
			GuiConfiguration.IgnoreCase = ignoreCaseCheck.Checked;
		}

		void ignoreAccentsCheck_CheckedChanged(object sender, EventArgs e) {
			GuiConfiguration.IgnoreAccents = ignoreAccentsCheck.Checked;
		}

		//Would it perhaps be better to wrap in the case where no more are found?
		private void nextPerfectMatch_Click(object sender, EventArgs e) {
			if (results != null) {
				int index = grid.FirstDisplayedScrollingRowIndex;
				while (++index < results.Count) {
					if (results[index].MatchType == MatchType.PerfectMatch) {
						grid.FirstDisplayedScrollingRowIndex = index;
						return;
					}
				}
			}
			System.Media.SystemSounds.Beep.Play();
		}

		private void previousPerfectMatch_Click(object sender, EventArgs e) {
			if (results != null) {
				int index = grid.FirstDisplayedScrollingRowIndex;
				while (--index >= 0) {
					if (results[index].MatchType == MatchType.PerfectMatch) {
						grid.FirstDisplayedScrollingRowIndex = index;
						return;
					}
				}
			}
			System.Media.SystemSounds.Beep.Play();
		}
		#endregion

		#region List Menu
		private void newList_Click(object sender, EventArgs e) {
			new ListBuilder().Show();
		}

		private void importList_Click(object sender, EventArgs e) {
			new Forms.ImportForm().Show();
		}

		/// <summary>
		/// Populates the list of recent Lists. Adds menu items which call OpenRecentFile on click.
		/// </summary>
		private void recentLists_DropDownOpening(object sender, EventArgs e) {
			recentLists.DropDownItems.Clear();

			var recent = new RecentListStore();
			foreach (ListInfo li in recent.GetLists()) {
				var handler = new EventHandler(this.OpenRecentFile);
				var item = new ToolStripMenuItem(li.Name, null, handler);
				item.Tag = li;
				recentLists.DropDownItems.Add(item);
				//recentListsToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(s, null, this.OpenRecentFile));
			}

			if (recentLists.DropDownItems.Count == 0) {
				var emptyItem = new ToolStripMenuItem(Resources.LookupForm.NoLists);
				emptyItem.Enabled = false;
				recentLists.DropDownItems.Add(emptyItem);
			}
		}

		private void OpenRecentFile(object sender, EventArgs e) {
			try {
				ListInfo info = ((sender as ToolStripMenuItem).Tag as ListInfo);
				var list = new WordList(info.Path);
				new ListBuilder(list).Show();
			} catch (System.IO.IOException x) {
				MessageBox.Show(x.Message);
			}
		}
		#endregion

		#region Dictionary Menu
		private void editInformationToolStripMenuItem_Click(object sender, EventArgs e) {
			new DictionaryInfoEditor(Dictionary, true).ShowDialog();
		}

		private void importDictionary_Click(object sender, EventArgs e) {
			new Forms.DictionaryImport().Show();
		}
		#endregion

		#region Tools Menu
		private void options_Click(object sender, EventArgs e) {
			new Forms.Preferences().ShowDialog();
		}
		#endregion

		#region Context Menu
		//Yay for hiding bugs instead of fixing them.
		private void addToList_Click(object sender, EventArgs e) {
			WordList list = new WordList();
			list.Capacity = grid.SelectedRows.Count;

			foreach (DataGridViewRow row in grid.SelectedRows) {
				list.Add(new TranslationPair(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), true));
			}

			if (listBuilder != null) {
				foreach (TranslationPair pair in list)
					listBuilder.AddPair(pair);
			} else {
				list.Author = GuiConfiguration.UserRealName;
				listBuilder = new ListBuilder(list);
				listBuilder.Closed += new EventHandler(listBuilder_Closed);
				listBuilder.Show();
			}
		}

		private void copy_Click(object sender, EventArgs e) {
			if (grid.SelectedRows.Count == 0)
				return;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach (DataGridViewRow row in grid.SelectedRows) {
				SearchResult result = (SearchResult)row.DataBoundItem;
				sb.Append(result.Phrase).Append(" -- ").Append(result.Translation).AppendLine();
			}

			Clipboard.SetText(sb.ToString());
		}

		/// <summary>
		/// Perform a reverse lookup on the first item in the selection. Reverse lookup in this case
		/// refers to the opposite direction to the currently displayed direction (for whatever reason
		/// that may be).
		/// </summary>
		private void reverseLookupToolStripMenuItem_Click(object sender, EventArgs e) {
			if (grid.SelectedCells.Count > 0) {
				DataGridViewCell cell = grid.SelectedCells[0];
				searchMode = DisplayedSearchMode == SearchMode.Forward ? SearchMode.Backward : SearchMode.Forward;

				//Why I can't just access it on the cell itself, I have NO idea. But it seems to work.
				searchBox.Text = cell.OwningRow.Cells[1].Value.ToString();

				//Update the UI
				SearchMode = searchMode;
			}
		}
		#endregion
		#endregion
	}

	public enum SearchMode {
		Forward,
		Backward
	}
}
