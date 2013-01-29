using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Linq;

// This form displays a large amount of rows on the DataGridView control. For information on how to 
// keep good performance with large datasets, see Best Practices for Scaling the Windows Forms 
// DataGridView Control:
//  * http://msdn.microsoft.com/en-us/library/ha5xt0d9.aspx
//
// The main points to take away from that article are:
//  * Don't access grid Row objects, because it could cause them to become unshared. 
//    Prefer grid methods such as GetCellState, etc.
//  * Don't access Cell objects either. Especially don't add tooltips or context menus 
//    to individual cells. Use the Cell*Needed events.
//  * Use full-row or full-column selection modes, rather than per-cell.
//  * Apply styles to the row templates or use the events; don't set cell styles.
//  * Try not to make rows become unshared.
// Note: Mono 2.4 doesn't support shared rows. This is probably causing much of the performance loss.
namespace Szotar.WindowsForms.Forms {
	public partial class LookupForm : Form {
		public IBilingualDictionary Dictionary { get; private set; }

		SearchMode searchMode, displayedSearchMode;
		IList<SearchResult> results;

		readonly CultureInfo sourceCulture, targetCulture;
		DisposableComponent listFontComponent;
		Font defaultGridFont;

		class LookupFormFileIsInUse : FileIsInUse {
			readonly LookupForm form;

			public LookupFormFileIsInUse(LookupForm form, string path)
				: base(path)
			{
				this.form = form;
				CanClose = true;
				WindowHandle = form.Handle;
			}

			// Who knows what thread this will be invoked on?
			public override void CloseFile() {
				form.Invoke(new Action(() => form.Close()));
			}
		}

		#region Initialization
		public LookupForm(IBilingualDictionary dictionary)
			: this() {
			components.Add(new DisposableComponent(new LookupFormFileIsInUse(this, dictionary.Path)));
			components.Add(new DisposableComponent(dictionary));

			Dictionary = dictionary;

			var mru = GuiConfiguration.RecentDictionaries ?? new MruList<DictionaryInfo>(10);
			mru.Update(dictionary.Info);
			GuiConfiguration.RecentDictionaries = mru;

			Font listFont = GuiConfiguration.GetListFont();
			if (listFont != null) {
				components.Add(listFontComponent = new DisposableComponent(listFont));
				defaultGridFont = grid.Font;
				grid.Font = listFont;
			}

			// TODO: This really needs testing. It could make things completely unusable...
			// I can't even remember if they're used...
			try {
				if (dictionary.FirstLanguageCode != null && dictionary.SecondLanguage != null) {
					sourceCulture = new CultureInfo(dictionary.FirstLanguageCode);
					targetCulture = new CultureInfo(dictionary.SecondLanguageCode);
				}
			} catch (ArgumentException) {
				// One of the cultures wasn't supported. In that case, set both cultures to null,
				// because it isn't worth having only one.
				sourceCulture = targetCulture = null;
			}

			InitialiseView();

			Load += delegate {
				UpdateResults();
			};
			Show();
		}

		/// <summary>Open the given dictionary, possibly loading from a file, 
		/// in a new LookupForm window.</summary>
		public LookupForm(DictionaryInfo dictionaryInfo)
			: this(dictionaryInfo.GetFullInstance()) { }

		/// <summary>Load the dictionary from the given path into a new LookupForm window.</summary>
		public LookupForm(string dictionaryPath)
			: this(new SimpleDictionary(dictionaryPath)) { }

		private LookupForm() {
			InitializeComponent();
			RegisterSettingsChangedEventHandlers();

			Icon = Properties.Resources.DictionaryIcon;
		}

		private void RemoveEventHandlers() {
			UnregisterSettingsChangedEventHandlers();
		}

		// Call this after the dictionaries are initialised.
		private void InitialiseView() {
			// Updates the mode switching button.
			SearchMode = SearchMode;

			AdjustGridRowHeight();

			InitializeDragAndDrop();

			searchBox.TextChanged += SearchBoxTextChanged;

			// Show custom tooltips that don't get in the way of the mouse and don't disappear so quickly.
			grid.MouseMove += GridMouseMove;
			grid.MouseLeave += GridMouseLeave;
			grid.ShowCellToolTips = false;

			// Bind an empty result set to the grid: this causes the columns to be created.
			grid.DataSource = results = new List<SearchResult>();
			grid.RowUnshared += (s, e) => {
				if (e.Row.Index >= results.Count)
					return;

				var result = results[e.Row.Index];

				ProgramLog.Default.AddMessage(LogType.Debug, "LookupForm: Row #{0} has become unshared: {1}, {2}",
					e.Row.Index,
					result.Phrase,
					result.Translation);
			};

			// By now, the columns should have been created.
			grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			grid.Columns[0].FillWeight = GuiConfiguration.LookupFormColumn1FillWeight;
			grid.Columns[1].Resizable = DataGridViewTriState.False;
			grid.CellFormatting += GridCellFormatting;
			grid.ColumnWidthChanged += GridColumnWidthChanged;

			ignoreAccentsCheck.Checked = ignoreAccentsMenuItem.Checked = GuiConfiguration.IgnoreAccents;
			ignoreAccentsCheck.Click += IgnoreAccentsCheckClick;
			ignoreCaseCheck.Checked = ignoreCaseMenuItem.Checked = GuiConfiguration.IgnoreCase;
			ignoreCaseCheck.Click += IgnoreCaseCheckClick;

			Shown += LookupFormShown;
			Closed += LookupFormClosed;
			InputLanguageChanged += LookupFormInputLanguageChanged;
			grid.KeyUp += GridKeyUp;

			mainMenu.Renderer = contextMenu.Renderer = toolStripPanel.Renderer = new ToolStripAeroRenderer(ToolbarTheme.CommunicationsToolbar);
			exitMenuItem.Text = string.Format(exitMenuItem.Text, Application.ProductName);
			fileMenu.DropDownOpening += FileMenuDropDownOpening;
		}

		void LookupFormShown(object sender, EventArgs e) {
			PerformLayout();
			searchBox.Focus();
		}
		#endregion

		#region Appearance
		/// <summary>Colours a cell differently based on how well the result matched the search term
		/// and whether the cell is on an alternating row.</summary>
		void GridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
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

		void AdjustGridRowHeight() {
			int pixels;

			using (var g = grid.CreateGraphics()) {
				float inches = grid.Font.SizeInPoints / 72;
				const double lineHeight = 1.9;
				pixels = (int)(Math.Round(lineHeight * inches * g.DpiY));
			}

			grid.RowTemplate.Height = pixels;

			// Forces the grid to re-apply its template settings -- there must be a better way.
			UpdateResults();
			//if (grid.RowCount > 0) {
			//	grid.Rows.SharedRow(0).Height = pixels;
			//	grid.Invalidate();
			//}
		}
		#endregion

		#region Settings Bindings
		private void RegisterSettingsChangedEventHandlers() {
			Configuration.Default.SettingChanged += SettingChanging;
		}

		private void UnregisterSettingsChangedEventHandlers() {
			Configuration.Default.SettingChanged -= SettingChanging;
		}

		// Update UI state.
		void SettingChanging(object sender, SettingChangedEventArgs e) {
			if (e.SettingName == "IgnoreAccents" || e.SettingName == "IgnoreCase") {
				// This shouldn't fire the CheckedChanged/SettingChanged events in an infinite loop.
				if (e.SettingName == "IgnoreAccents")
					ignoreAccentsMenuItem.Checked = ignoreAccentsCheck.Checked = GuiConfiguration.IgnoreAccents;
				else if (e.SettingName == "IgnoreCase")
					ignoreCaseMenuItem.Checked = ignoreCaseCheck.Checked = GuiConfiguration.IgnoreCase;

				// We might want to skip this if nothing was actually changed.
				UpdateResults();
			} else if (e.SettingName == "ListFontName" || e.SettingName == "ListFontSize") {
				// Note: this is slightly inefficient -- if both are set at once it redisplays twice.
				Font disposeOf = null;
				if (listFontComponent != null) {
					components.Remove(listFontComponent);
					disposeOf = grid.Font;
				}
				var font = GuiConfiguration.GetListFont();
				if (font != null) {
					if (defaultGridFont == null)
						defaultGridFont = grid.Font;
					listFontComponent = new DisposableComponent(font);
					grid.Font = font;
				} else {
					grid.Font = defaultGridFont;
				}
				if (disposeOf != null)
					disposeOf.Dispose();
				AdjustGridRowHeight();
			}
		}

		void GridColumnWidthChanged(object sender, DataGridViewColumnEventArgs e) {
			GuiConfiguration.LookupFormColumn1FillWeight = grid.Columns[0].FillWeight;
		}
		#endregion

		#region Search code
		IDictionarySection GetSectionBySearchMode(SearchMode mode) {
			return mode == SearchMode.Forward ? Dictionary.ForwardsSection : Dictionary.ReverseSection;
		}

		void UpdateResults() {
			var dict = GetSectionBySearchMode(SearchMode);
			var finalSearchMode = SearchMode;

			string search = searchBox.Text.Trim().Normalize();

			results = new List<SearchResult>();
			int foundAt = -1;
			bool hadPerfect = false;
			int i = 0;

			// Look for the first result containing the search term at the start.
			foreach (var result in dict.Search(search, ignoreAccentsCheck.Checked, ignoreCaseCheck.Checked)) {
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
				dict = GetSectionBySearchMode(finalSearchMode);
				foreach (var result in dict.Search(search, ignoreAccentsCheck.Checked, ignoreCaseCheck.Checked)) {
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

			if (foundAt >= 0) {
				grid.FirstDisplayedScrollingRowIndex = foundAt;
				grid.FirstDisplayedScrollingColumnIndex = 0;
			}

			displayedSearchMode = finalSearchMode;
			UpdateButtonNames();

			grid.ClearSelection();
			grid.PerformLayout();

			UpdateTitle();
		}

		void UpdateTitle() {
			Text = string.Format(CultureInfo.CurrentUICulture, "{0} - {1} results", Dictionary.Name, results.Count);
		}

		void SwitchMode() {
			SearchMode = SearchMode == SearchMode.Forward ? SearchMode.Backward : SearchMode.Forward;
		}

		void FocusSearchField() {
			searchBox.Focus();
			searchBox.SelectAll();
		}
		#endregion

		#region ToolTip
		// Keep a cached version of the most recent tooltip: this should speed things up a little.
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

			// Use the cached version in the current tooltip if possible.
			if (infoTipRow == rowIndex)
				return infoTipText;

			var dsm = DisplayedSearchMode;
			var entry = results[rowIndex].Entry;
			if (entry.Translations == null)
				GetSectionBySearchMode(DisplayedSearchMode).GetFullEntry(entry);

			var otherSide = GetSectionBySearchMode(dsm == SearchMode.Forward ? SearchMode.Backward : SearchMode.Forward);
			var sb = new StringBuilder(string.Join(",", from term in entry.Translations select term.Value));
			string search = sb.ToString();
			sb.Length = 0;

			foreach (var sr in otherSide.Search(search, false, false))
				if (sr.MatchType == MatchType.PerfectMatch)
					sb.AppendLine(SanitizeToolTipLine(sr.Phrase + " -> " + sr.Translation));

			return sb.Length > 0 ? sb.ToString() : null;
		}

		/// <summary>Removes some common annoyances with tooltips (really wide tooltips).</summary>
		/// <remarks>Currently obsolete (tooltip size is limited, text wraps).</remarks>
		static string SanitizeToolTipLine(string line) {
			return line;
		}

		void GridMouseMove(object sender, MouseEventArgs e) {
			if (e.Button != MouseButtons.None)
				return;

			if (e.Location == currentInfoTipMouseLocation)
				return;
			currentInfoTipMouseLocation = e.Location;

			var hitTest = grid.HitTest(e.X, e.Y);
			if (hitTest.Type != DataGridViewHitTestType.Cell && hitTest.Type != DataGridViewHitTestType.RowHeader) {
				if (infoTip != null && infoTip.Active)
					infoTip.Hide(grid);
				infoTipText = null;
				infoTipRow = -1;
				return;
			}

			if (hitTest.RowIndex == infoTipRow)
				return;

			string text = GetInfoTipText(hitTest.RowIndex);

			if (text == null) {
				if (infoTip != null)
					infoTip.Hide(grid);
				return;
			}

			infoTipRow = hitTest.RowIndex;
			infoTipText = text;

			if (infoTip == null) {
				infoTip = new ToolTip(components) { StripAmpersands = false, UseAnimation = false };
				infoTip.Popup += (s, e3) => { e3.ToolTipSize = new Size(Math.Min(e3.ToolTipSize.Width, grid.Width), e3.ToolTipSize.Height); };
			}

			infoTip.ToolTipTitle = GetInfoTipTitle(hitTest.RowIndex);

			int offset = grid.GetRowDisplayRectangle(hitTest.RowIndex, true).Height;

			// This usually happens due to bugs and import errors. Either way, it's bad.
			if (text.Length > 5000) {
				infoTip.Hide(grid);
				return;
			}

			infoTip.Show(text, grid, e.X + offset, e.Y + offset);
		}

		void GridMouseLeave(object sender, EventArgs e) {
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
				if (searchMode == value)
					return;
				searchMode = value;
				switchMode.Text = SearchMode == SearchMode.Forward ? forwards.Text : backwards.Text;
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
		/// <summary>Updates the current search results to reflect the new search terms.</summary>
		private void SearchBoxTextChanged(object sender, EventArgs e) {
			UpdateResults();
		}

		/// <summary>Switches the current search mode from Forward to Backward (and vice versa), 
		/// and focuses the search field.</summary>
		private void SwitchModeClick(object sender, EventArgs e) {
			SwitchMode();
			FocusSearchField();
		}

		/// <summary>Performs a reverse-lookup of the cell that was double-clicked.</summary>
		/// <remarks>This is somewhat redundant now that the tooltips do this too, but
		/// the tooltips are quite limited, and it is harder to interact with them.</remarks>
		private void GridCellMouseDoubleClick(object sender, DataGridViewCellEventArgs e) {
			if (results == null || e.RowIndex < 0)
				return;
			SearchResult sr = results[e.RowIndex];

			var sb = new StringBuilder();
			var otherSide = GetSectionBySearchMode(DisplayedSearchMode == SearchMode.Backward ? SearchMode.Forward : SearchMode.Backward);

			foreach (var t in sr.Entry.Translations) {
				sb.Append(t.Value);
				sb.Append(": ");
				int n = 0;
				foreach (var rsr in otherSide.Search(t.Value, false, false)) {
					if (rsr.MatchType == MatchType.PerfectMatch) {
						if (rsr.Entry.Translations == null)
							otherSide.GetFullEntry(rsr.Entry);

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
		#endregion

		#region Form events
		/// <summary>
		/// Some keyboard shortcuts to invoke the GC. Quite useful for testing how much memory is really
		/// in use. Also clears the search when Escape is pressed.
		/// </summary>
		private void LookupFormKeyDown(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.F9:
					GC.Collect(GC.MaxGeneration);
					break;
				case Keys.F8:
					GC.Collect(1);
					break;
				case Keys.F7:
					GC.Collect(0);
					break;
				case Keys.Escape:
					searchBox.Text = string.Empty;
					break;
				case Keys.F10: {
					int shared = 0;
					for (int i = 0; i < grid.Rows.Count; ++i)
						if (grid.Rows.SharedRow(i).Index == -1)
							shared++;
					MessageBox.Show(string.Format("Shared rows: {0} of {1}", shared, grid.Rows.Count), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					break;
					}
				case Keys.F11: {
					var dict = Dictionary as SimpleDictionary;
					if (dict != null) {
						var forward = (SimpleDictionary.Section)Dictionary.ForwardsSection;
						var backward = (SimpleDictionary.Section)Dictionary.ReverseSection;

						MessageBox.Show(
							string.Format(
								"Debug statistics for {0}:\n\nForwards section: {1} of {2} fully loaded\nBackwards section: {3} of {4} fully loaded",
								dict.Name ?? dict.Path,
								forward.FullyLoadedCount,
								forward.HeadWords,
								backward.FullyLoadedCount,
								backward.HeadWords
								),
							Dictionary.Name,
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
					}
					break;
				}
			}
		}

		void LookupFormClosed(object sender, EventArgs e) {
			GuiConfiguration.Save();

			RemoveEventHandlers();

			// This is done as a hint to the GC, because forms which are open for a long time 
			// might not trigger a gen2 collection when closing. 
			// We want to reclaim the memory in case there are other windows open.
			// GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
			GC.Collect(GC.MaxGeneration);
		}

		void LookupFormInputLanguageChanged(object sender, InputLanguageChangedEventArgs e) {
			if (targetCulture != null && targetCulture.Equals(e.InputLanguage.Culture)) {
				SearchMode = SearchMode.Backward;
			} else if (sourceCulture != null && sourceCulture.Equals(e.InputLanguage.Culture)) {
				SearchMode = SearchMode.Forward;
			}
		}
		#endregion

		#region Grid methods
		public int SelectedRowCount() {
			// The SelectedRows and SelectedColumns collections can be inefficient [BPSWFDC].
			return grid.Rows.GetRowCount(DataGridViewElementStates.Selected);
		}
		#endregion

		#region Drag and drop
		Rectangle dragBox;

		void InitializeDragAndDrop() {
			grid.MouseDown += GridMouseDown;
			grid.MouseMove += GridMouseMoveDrag;
		}

		void GridMouseDown(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				var dragSize = SystemInformation.DragSize;

				dragBox = new Rectangle(
					e.X - dragSize.Width / 2,
					e.Y - dragSize.Height / 2,
					dragSize.Width,
					dragSize.Height);
			} else if (e.Button == MouseButtons.Right) {
				// If the right-clicked item is outside the current selection, select that item.
				// This is more like how normal list boxes work.
				if (e.Button == MouseButtons.Right) {
					int row = grid.HitTest(e.X, e.Y).RowIndex;

					if (row >= 0 && row < grid.RowCount && (grid.Rows.GetRowState(row) 
							& DataGridViewElementStates.Selected) == DataGridViewElementStates.None) {
						grid.ClearSelection();
					
						// TODO: This makes the row become unshared. I have no idea how to stop this
						// happening.
						grid.Rows[row].Selected = true;
					}
				}
			}
		}

		void GridMouseMoveDrag(object sender, MouseEventArgs e) {
			if (e.Button != MouseButtons.Left)
				return;

			if (dragBox == Rectangle.Empty || dragBox.Contains(e.Location))
				return;

			var data = MakeDataObjectFromSelection();

			grid.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Scroll);
			dragBox = Rectangle.Empty;
		}

		DataObject MakeDataObjectFromSelection() {
			var data = new DataObject();

			var selection = new List<TranslationPair>(GetSelectedTranslationPairs()).ToArray();
			data.SetData(typeof(TranslationPair[]), selection);

			var sb = new StringBuilder();

			// Add CSV formatted data.
			foreach (var sr in selection) {
				CsvUtilities.AppendCSVValue(sb, sr.Phrase);
				sb.Append(',');
				CsvUtilities.AppendCSVValue(sb, sr.Translation);
				sb.AppendLine();
			}
			data.SetData(DataFormats.CommaSeparatedValue, sb.ToString());

			// Add plain text representation.
			sb.Length = 0;
			foreach (var sr in selection)
				sb.Append(sr.Phrase).Append(" -- ").AppendLine(sr.Translation);
			data.SetData(DataFormats.UnicodeText, sb.ToString());
			data.SetData(DataFormats.Text, sb.ToString());

			return data;
		}
		#endregion

		#region Clipboard
		void GridKeyUp(object sender, KeyEventArgs e) {
			if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C) {
				CopySelection();
				e.Handled = true;
			}
		}

		void CopySelection() {
			if (SelectedRowCount() <= 0)
				return;

			var data = MakeDataObjectFromSelection();
			Clipboard.SetDataObject(data, true);
		}

		bool CanCopy() {
			return SelectedRowCount() > 0;
		}
		#endregion

		#region Menu Events
		#region File Menu
		/// <summary>Shows the start page. Attempt to find an existing start page,
		/// or create one if none exists.</summary>
		private void ShowStartPageClick(object sender, EventArgs e) {
			ShowForm.Show<StartPage>();
		}

		private void CloseWindowClick(object sender, EventArgs e) {
			Close();
		}

		/// <summary>Exit the entire program.</summary>
		private void ExitMenuItemClick(object sender, EventArgs e) {
			Application.Exit();
		}

		/// <summary>Populates the list of recent dictionaries when the File menu opens.</summary>
		/// <remarks>The items it adds are tagged with "MRU", so that they can be removed again later.</remarks>
		void FileMenuDropDownOpening(object sender, EventArgs e) {
			//Remove existing entries
			var items = fileMenu.DropDownItems;
			for (int i = 0; i < items.Count; ) {
				if ((string)(items[i].Tag) == "MRU")
					items.RemoveAt(i);
				else
					i++;
			}

			// Add new entries
			var mru = GuiConfiguration.RecentDictionaries.Entries;
			var index = items.IndexOf(exitMenuItem);
			if (index == -1)
				index = items.Count;

			int count = 0;
			for (int i = 0; i < mru.Count; ++i) {
				var info = mru[i];
				if (info.Path != this.Dictionary.Path && System.IO.File.Exists(info.Path)) {
					var item = new ToolStripMenuItem(
						mru[i].Name,
						null,
						delegate { OpenDictionary(info); }
						) { Tag = "MRU" };

					items.Insert(index, item);
					count++;
					index++;
				}
			}

			if (count > 0) {
				var item = new ToolStripSeparator { Tag = "MRU" };
				items.Insert(index, item);
			}
		}
		#endregion

		#region Search Menu
		private void ForwardsClick(object sender, EventArgs e) {
			SearchMode = SearchMode.Forward;
			FocusSearchField();
		}

		private void BackwardsClick(object sender, EventArgs e) {
			SearchMode = SearchMode.Backward;
			FocusSearchField();
		}

		private void SwitchModeMenuItemClick(object sender, EventArgs e) {
			SwitchMode();
			FocusSearchField();
		}

		void FocusSearchFieldClick(object sender, EventArgs e) {
			FocusSearchField();
		}

		void ClearSearchClick(object sender, EventArgs e) {
			searchBox.Text = String.Empty;
		}

		void IgnoreAccentsMenuItemClick(object sender, EventArgs e) {
			GuiConfiguration.IgnoreAccents = ignoreAccentsMenuItem.Checked;
		}

		void IgnoreCaseMenuItemClick(object sender, EventArgs e) {
			GuiConfiguration.IgnoreCase = ignoreCaseMenuItem.Checked;
		}

		void IgnoreCaseCheckClick(object sender, EventArgs e) {
			GuiConfiguration.IgnoreCase = ignoreCaseCheck.Checked;
		}

		void IgnoreAccentsCheckClick(object sender, EventArgs e) {
			GuiConfiguration.IgnoreAccents = ignoreAccentsCheck.Checked;
		}

		/// <summary>Navigates to the next exact match in the search results.</summary>
		/// <remarks>Would it perhaps be better to wrap in the case where no more are found?</remarks>
		void NextPerfectMatchClick(object sender, EventArgs e) {
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

		/// <summary>Navigates to the previous exact match in the results.</summary>
		/// <seealso cref="NextPerfectMatchClick"/>
		void PreviousPerfectMatchClick(object sender, EventArgs e) {
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
		void NewListClick(object sender, EventArgs e) {
			new ListBuilder().Show();
		}

		void OpenListClick(object sender, EventArgs e) {
			ShowForm.Show<StartPage>();
		}

		void ImportListClick(object sender, EventArgs e) {
			ShowForm.Show<ImportForm>();
		}

		/// <summary>
		/// Populates the list of recent Lists. Adds menu items which call OpenRecentFile on click.
		/// </summary>
		void RecentListsDropDownOpening(object sender, EventArgs e) {
			recentLists.DropDownItems.Clear();

			foreach (var li in DataStore.Database.GetRecentSets(Configuration.RecentListsSize)) {
				var handler = new EventHandler(OpenRecentFile);
				var item = new ToolStripMenuItem(li.Name, null, handler) { Tag = li };
				recentLists.DropDownItems.Add(item);
			}

			if (recentLists.DropDownItems.Count == 0) {
				var emptyItem = new ToolStripMenuItem(Resources.LookupForm.NoLists) { Enabled = false };
				recentLists.DropDownItems.Add(emptyItem);
			}
		}

		static void OpenRecentFile(object sender, EventArgs e) {
			var info = (((ToolStripMenuItem) sender).Tag as ListInfo);
			if (info != null && info.ID.HasValue)
				ListBuilder.Open(info.ID.Value);
		}
		#endregion

		#region Dictionary Menu
		private void DictionaryMenuDropDownOpening(object sender, EventArgs e) {
			copyMI.Enabled = CanCopy();
		}

		private void CopyMIClick(object sender, EventArgs e) {
			CopySelection();
		}

		void EditInformationClick(object sender, EventArgs e) {
			var dr = new DictionaryInfoEditor(Dictionary, true).ShowDialog();

			if (dr == DialogResult.OK) {
				UpdateTitle();
				UpdateButtonNames();
			}
		}

		void UpdateButtonNames() {
			// Set the menu items' Text to match the names of the dictionary sections.
			// The mode switch button is based on these.
			forwards.Text = Dictionary.FirstLanguage + "-" + Dictionary.SecondLanguage;
			backwards.Text = Dictionary.SecondLanguage + "-" + Dictionary.FirstLanguage;
			switchMode.Text = SearchMode == SearchMode.Forward ? forwards.Text : backwards.Text;
			grid.Columns[0].HeaderText = displayedSearchMode == SearchMode.Forward ? Dictionary.FirstLanguage : Dictionary.SecondLanguage;
			grid.Columns[1].HeaderText = displayedSearchMode == SearchMode.Forward ? Dictionary.SecondLanguage : Dictionary.FirstLanguage;
		}

		void ImportDictionaryClick(object sender, EventArgs e) {
			ShowForm.Show<DictionaryImport>();
		}

		private void AddEntryClick(object sender, EventArgs e) {
			string phraseLanguage = displayedSearchMode == SearchMode.Forward ? Dictionary.FirstLanguage : Dictionary.SecondLanguage;
			string translationLanguage = displayedSearchMode == SearchMode.Forward ? Dictionary.SecondLanguage: Dictionary.FirstLanguage;

			var dr = new Dialogs.EditDictionaryItem(GetSectionBySearchMode(displayedSearchMode), phraseLanguage, translationLanguage).ShowDialog();
			if (dr == DialogResult.OK)
				UpdateResults();
		}
		#endregion

		#region Tools Menu
		void DictsFolderClick(object sender, EventArgs e) {
			DataStore.UserDataStore.EnsureDirectoryExists(Configuration.DictionariesFolderName);
			string path = System.IO.Path.Combine(DataStore.UserDataStore.Path, Configuration.DictionariesFolderName);
			System.Diagnostics.Process.Start(path);
		}

		void CharMapClick(object sender, EventArgs e) {
			System.Diagnostics.Process.Start("charmap.exe");
		}

		void DebugLogClick(object sender, EventArgs e) {
			ShowForm.Show<LogViewerForm>();
		}

		void OptionsClick(object sender, EventArgs e) {
			ShowForm.Show<Preferences>();
		}
		#endregion

		#region Context Menu
		private void ContextMenuOpening(object sender, CancelEventArgs _) {
			copyCM.Enabled = CanCopy();			

			var open = new List<long>();

			addTo.DropDownItems.Clear();

			// Find out what word list editors are already open.
			foreach (var f in Application.OpenForms) {
				var lb = f as ListBuilder;
				if (lb != null) {
					open.Add(lb.WordList.ID.Value);
					var item = new ToolStripMenuItem(lb.WordList.Name, null, (s, e) => AddToExistingList(lb.WordList.ID.Value));
					addTo.DropDownItems.Add(item);
				}
			}

			var recent = DataStore.Database.GetRecentSets(Configuration.RecentListsSize).ToList();
			recent.RemoveAll(r => open.Contains(r.ID.Value));

			if (recent.Count > 0 && open.Count > 0)
				addTo.DropDownItems.Add(new ToolStripSeparator());

			foreach (var info in recent) {
				var info_ = info; // HACK: Copy for closure
				var item = new ToolStripMenuItem(info.Name, null, (s, e) => AddToExistingList(info_.ID.Value));
				addTo.DropDownItems.Add(item);
			}

			addTo.Visible = addTo.DropDownItems.Count > 0;

			deleteMI.Enabled = editMI.Enabled = grid.Rows.GetRowCount(DataGridViewElementStates.Selected) > 0;
		}

		private void AddToExistingList(long listID) {
			var form = ListBuilder.Open(listID);
			if(form != null)
				AddEntries(form);
		}

		IEnumerable<int> GetSelectedIndices() {
			// Enumerate selected rows in index order. (It's faster not to use SelectedRows.)
			for (int index = grid.Rows.GetFirstRow(DataGridViewElementStates.Selected);
				index >= 0;
				index = grid.Rows.GetNextRow(index, DataGridViewElementStates.Selected)) {
				yield return index;
			}
		}

		IEnumerable<SearchResult> GetSelectedResults() {
			return from i in GetSelectedIndices()
				   select results[i];
		}

		IEnumerable<TranslationPair> GetSelectedTranslationPairs() {
			return from r in GetSelectedResults()
				   select new TranslationPair(r.Phrase, r.Translation);
		}

		private void AddEntries(ListBuilder lb) {
			// TODO: Maybe pass this method an IEnumerable instead.
			lb.AddEntries(GetSelectedTranslationPairs());
		}

		private void AddToListClick(object sender, EventArgs e) {
			var lb = new ListBuilder();
			AddEntries(lb);
			lb.Show();
		}

		void CopyCMClick(object sender, EventArgs e) {
			CopySelection();
		}

		/// <summary>
		/// Perform a reverse lookup on the first item in the selection. Reverse lookup in this case
		/// refers to the opposite direction to the currently displayed direction (for whatever reason
		/// that may be).
		/// </summary>
		private void ReverseLookupToolStripMenuItemClick(object sender, EventArgs e) {
			int index = grid.Rows.GetFirstRow(DataGridViewElementStates.Selected);
			if (index >= 0) {
				searchMode = DisplayedSearchMode == SearchMode.Forward ? SearchMode.Backward : SearchMode.Forward;
				searchBox.Text = results[index].Translation;
			}
		}

		private void EditMIClick(object sender, EventArgs e) {
			int index = grid.Rows.GetFirstRow(DataGridViewElementStates.Selected);
			if (index < 0)
				return;

			string phraseLanguage = displayedSearchMode == SearchMode.Forward ? Dictionary.FirstLanguage : Dictionary.SecondLanguage;
			string translationLanguage = displayedSearchMode == SearchMode.Forward ? Dictionary.SecondLanguage : Dictionary.FirstLanguage;

			var dr = new Dialogs.EditDictionaryItem(GetSectionBySearchMode(displayedSearchMode), phraseLanguage, translationLanguage, results[index].Entry).ShowDialog();
			if (dr == DialogResult.OK)
				UpdateResults();
		}

		private void DeleteMIClick(object sender, EventArgs e) {
			var remove = GetSelectedResults().Select(sr => sr.Entry).ToArray();
			GetSectionBySearchMode(displayedSearchMode).RemoveEntries(remove);
		}
		#endregion
		#endregion

		/// <summary>
		/// Opens a dictionary window for the specified dictionary, using an 
		/// existing window if possible.
		/// </summary>
		/// <param name="dict">The dictionary to open.</param>
		public static void OpenDictionary(DictionaryInfo dict) {
			if (dict == null)
				return;

			try {
				ShowForm.Show(
					form => form.Dictionary.Path == dict.Path,
					() => new LookupForm(dict));
			} catch (System.IO.IOException e) {
				Errors.CouldNotLoadDictionary(dict.Name, dict.Path, e);
			} catch (DictionaryLoadException e) {
				Errors.CouldNotLoadDictionary(dict.Name, dict.Path, e);
			}
		}
	}

	public enum SearchMode {
		Forward,
		Backward
	}
}
