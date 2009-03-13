using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;
using System.Diagnostics;

namespace Szotar.WindowsForms.Controls {
	[ToolboxBitmap(typeof(System.Windows.Forms.DataGridView))]
	public partial class DictionaryGrid : UserControl {
		WordList source;
		bool showMutableRows;
		bool allowNewRows;
		bool ignoreNextListChangedEvent = false;

		WordListEntry pairInEdit = null;
		int? rowInEdit = null;
		bool dirty = false;

		public DictionaryGrid() {
			InitializeComponent();
			InitializeGridStyles();
			InitializeVirtualMode();

			this.Disposed += (s, e) => UnwireDataSourceEvents();
		}

		public DictionaryGrid(WordList dataSource)
			: this() {
			DataSource = dataSource;
		}

		void grid_DataError(object sender, DataGridViewDataErrorEventArgs e) {
			e.ThrowException = false;
		}

		public void UpdateData() {
			var x = DataSource;
			DataSource = null;
			DataSource = x;
		}

		#region Data Binding
		private void WireDataSourceEvents() {
			if (source is IBindingList)
				((IBindingList)source).ListChanged += new ListChangedEventHandler(source_ListChanged);
		}

		private void UnwireDataSourceEvents() {
			if (source is IBindingList) 
				((IBindingList)source).ListChanged -= new ListChangedEventHandler(source_ListChanged);
		}

		void source_ListChanged(object sender, ListChangedEventArgs e) {
			if (ignoreNextListChangedEvent) {
				ignoreNextListChangedEvent = false;
				return;
			}

			switch (e.ListChangedType) {
				case ListChangedType.ItemAdded:
					if (rowInEdit != null && rowInEdit >= e.NewIndex)
						rowInEdit++;
					grid.Rows.Insert(e.NewIndex, 1);
					grid.InvalidateRow(e.NewIndex);
					grid.Invalidate();
					grid.Update();
					break;
				case ListChangedType.ItemChanged:
					grid.InvalidateRow(e.NewIndex);
					break;
				case ListChangedType.ItemDeleted:
					if (rowInEdit > e.NewIndex)
						rowInEdit--;
					else if (rowInEdit == e.NewIndex) {
						grid.CancelEdit();
						rowInEdit = null;
						pairInEdit = null;
					}
					grid.Rows.RemoveAt(e.NewIndex);
					break;
				case ListChangedType.ItemMoved:
					if (rowInEdit != null) {
						if (rowInEdit == e.OldIndex) {
							rowInEdit = e.NewIndex;
						} else if (rowInEdit < e.OldIndex && rowInEdit > e.NewIndex) {
							//Moved row was before the current row, but moved to after it.
							rowInEdit--;
						} else if (rowInEdit > e.OldIndex && rowInEdit >= e.NewIndex) {
							//Moved row was after current row, but moved to before it
							rowInEdit++;
						}
					}
					DataGridViewRow row = grid.Rows[e.OldIndex];
					grid.Rows.RemoveAt(e.OldIndex);
					grid.Rows.Insert(e.NewIndex, row);
					break;
				case ListChangedType.Reset:
					grid.Invalidate();
					break;
			}
		}
		#endregion

		#region Virtual Mode implementation
		public WordList DataSource {
			get { return source; }
			set {
				if (value == null) {
					UnwireDataSourceEvents();
					grid.RowCount = 1;
					grid.Enabled = false;
					source = null;
				} else if (source != value) {
					//This includes the row for new records.
					source = value;
					WireDataSourceEvents();
					grid.Enabled = true;
					grid.AllowUserToAddRows = allowNewRows;
					grid.RowCount = source.Count + (allowNewRows ? 1 : 0);
					grid.Refresh();
				}
			}
		}

		private void InitializeVirtualMode() {
			grid.VirtualMode = true;
			grid.CellValueNeeded += new DataGridViewCellValueEventHandler(grid_CellValueNeeded);
			grid.CellValuePushed += new DataGridViewCellValueEventHandler(grid_CellValuePushed);
			grid.NewRowNeeded += new DataGridViewRowEventHandler(grid_NewRowNeeded);
			grid.RowValidated += new DataGridViewCellEventHandler(grid_RowValidated);
			grid.RowDirtyStateNeeded += new QuestionEventHandler(grid_RowDirtyStateNeeded);
			grid.CancelRowEdit += new QuestionEventHandler(grid_CancelRowEdit);
			grid.UserDeletingRow += new DataGridViewRowCancelEventHandler(grid_UserDeletingRow);
			grid.KeyUp += new KeyEventHandler(grid_KeyUp);

			DataGridViewColumn phraseColumn = new DataGridViewTextBoxColumn();
			phraseColumn.HeaderText = Properties.Resources.PhraseDefaultHeader;
			phraseColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			phraseColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			grid.Columns.Add(phraseColumn);

			DataGridViewColumn translationColumn = new DataGridViewTextBoxColumn();
			translationColumn.HeaderText = Properties.Resources.TranslationDefaultHeader;
			translationColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			translationColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			grid.Columns.Add(translationColumn);

			DataSource = null;
		}

		void grid_KeyUp(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.None) {
				source.RemoveAt(SelectedIndices);
				e.Handled = true;
			} else if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control) {
				source.Undo();
				e.Handled = true;
			} else if (
				(e.KeyCode == Keys.Z && e.Modifiers == (Keys.Control | Keys.Shift)) ||
				(e.KeyCode == Keys.Y && e.Modifiers == Keys.Control))
			{
				source.Redo();
				e.Handled = true;
			}
		}

		void grid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
			e.Cancel = true; //Handled by KeyUp event
			return;

			if (source == null)
				return;

			if (e.Row.Index < source.Count) {
				//If the user has deleted an existing row, remove the
				//corresponding TranslationPair object from the data source.
				//Suppress the ListChangedType.ItemRemoved event.
				ignoreNextListChangedEvent = true;
				if(e.Row.Index < source.Count) //We can't remove the editing row from the data source!
					source.RemoveAt(e.Row.Index);
			} else {
				//If the user has deleted a newly created row, release
				//the corresponding TranslationPair object.
				rowInEdit = null;
				pairInEdit = null;
			}
		}

		void grid_CancelRowEdit(object sender, QuestionEventArgs e) {
			if (source == null)
				return;

			//Count-2 because there will be a new extender row, which will be 
			//occupying Count-1, thus the currently edited row will be Count-2.
			if (rowInEdit == grid.Rows.Count - 2 && rowInEdit == source.Count) {
				// If the user has canceled the edit of a newly created row, 
				// replace the corresponding TranslationPair object with a new, empty one.
				pairInEdit = new WordListEntry(null);
			} else {
				// If the user has canceled the edit of an existing row, 
				// release the corresponding TranslationPair object.
				pairInEdit = null;
				rowInEdit = null;
			}
		}

		void grid_RowDirtyStateNeeded(object sender, QuestionEventArgs e) {
			e.Response = dirty;
			Debug.WriteLine(string.Format("Queried dirty state for R{0}C{1}: {2}", grid.CurrentCell.RowIndex, grid.CurrentCell.ColumnIndex, dirty));
		}

		//This event occurs whenever the user changes the current row.
		void grid_RowValidated(object sender, DataGridViewCellEventArgs e) {
			Debug.WriteLine(string.Format("Row R{0}C{1} validated", e.RowIndex, e.ColumnIndex));

			if (e.RowIndex >= source.Count && e.RowIndex != grid.Rows.Count - 1) {
				Debug.WriteLine(string.Format("  This is a new row."));

				//This could happen if either the last row was deleted or the editing row was committed to the list.
				//In the case of a deleted row, is doing nothing the correct behaviour?
				if (pairInEdit != null) {
					//Suppress the ListChangedType.ItemAdded event.
					//(But only if we're actually doing something! Don't move this back out of the if statement!)
					ignoreNextListChangedEvent = true;

					pairInEdit.AddTo(source, source.Count);
					pairInEdit = null;
					rowInEdit = null;
				} else
					Debug.Assert(rowInEdit == null);
			} else if (pairInEdit != null && e.RowIndex < source.Count) {
				Debug.WriteLine(string.Format("  This is a normal row."));
				//A normal row was edited, and the changes were committed.

				//This check is absolutely necessary. RowValidated is often called spuriously by the DataGridView
				//even when the user hasn't done anything. It could also happen when inserting a group of rows.
				//In that case, we certainly don't want to set the i'th element because that incurs a database
				//access in SqliteWordList - and what's more, currently, the command is re-created every time.
				//In short, it's weird.
				if (grid.IsCurrentRowDirty) {
					Debug.WriteLine(string.Format("  Row was dirty.", e.RowIndex, e.ColumnIndex));
					//Suppress the ListChangedType.ItemChanged event.
					ignoreNextListChangedEvent = true;

					pairInEdit.Owner = source;
					source[e.RowIndex] = pairInEdit;
				} else
					Debug.WriteLine(string.Format("  Row was not dirty.", e.RowIndex, e.ColumnIndex));
				pairInEdit = null;
				rowInEdit = null;
			} else if (grid.ContainsFocus) {
				Debug.WriteLine(string.Format("  No row was being edited."));

				pairInEdit = null;
				rowInEdit = null;
			}

			dirty = false;
		}

		//Create a new TranslationPair when the user edits the row for new records.
		void grid_NewRowNeeded(object sender, DataGridViewRowEventArgs e) {
			pairInEdit = new WordListEntry(null);
			rowInEdit = grid.Rows.Count - 1;
		}

		//Called when the user enters data for a cell and commits that change.
		//Stores the edited value in the TranslationPair representing the edited row.
		//This TranslationPair will be committed when the row is left.
		void grid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e) {
			WordListEntry rowSource;

			if (source == null)
				return;

			Debug.WriteLine(string.Format("Cell value pushed at R{0}C{1}: {2}", e.RowIndex, e.ColumnIndex, e.Value));

			if (e.RowIndex < source.Count) {
				Debug.WriteLine(string.Format("  Normal row."));

				if (pairInEdit == null) {
					Debug.WriteLine(string.Format("  pairInEdit was null"));
					var we = source[e.RowIndex];
					pairInEdit = new WordListEntry(null, we.Phrase, we.Translation, we.TimesTried, we.TimesFailed);
				} else {
					Debug.WriteLine(string.Format("  pairInEdit was non-null"));
				}
				rowSource = pairInEdit;
				rowInEdit = e.RowIndex;
			} else {
				Debug.WriteLine(string.Format("  New row."));
				//If the user previously deleted the row for new data (for example, by
				//cancelling the row edit) the pairInEdit will be null.
				if (pairInEdit == null) {
					pairInEdit = new WordListEntry(null);
					rowInEdit = e.RowIndex;
					Debug.WriteLine(string.Format("  pairInEdit was null"));
				} else
					Debug.WriteLine(string.Format("  pairInEdit was non-null"));
				rowSource = pairInEdit;
			}

			//If the row was emptied, the Value property may be null!
			if (e.ColumnIndex == 0)
				rowSource.Phrase = e.Value != null ? e.Value.ToString() : "";
			else
				rowSource.Translation = e.Value != null ? e.Value.ToString() : "";

			dirty = true;
		}

		//Called when the DataGridView requests the data for a cell.
		void grid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
			var rowSource = RowSource(e.RowIndex);

			if (rowSource != null) {
				if (e.ColumnIndex == 0)
					e.Value = rowSource.Phrase;
				else
					e.Value = rowSource.Translation;
			}
		}

		private WordListEntry RowSource(int row) {
			if (source == null || row > source.Count)
				return null;

			return row >= source.Count || row == rowInEdit ? pairInEdit : source[row];
		}
		#endregion

		#region Properties
		[Browsable(true), Description("Gets or sets whether the grid marks mutable rows in a different colour."), Category("Display"), DefaultValue(true)]
		public bool ShowMutableRows {
			get { return showMutableRows; }
			set { showMutableRows = value; }
		}

		public ColumnNameIndexer ColumnNames {
			get {
				return new ColumnNameIndexer(grid);
			}
		}

		public class ColumnNameIndexer {
			DataGridView gridView;
			public ColumnNameIndexer(DataGridView gridView) {
				this.gridView = gridView;
			}

			public string this[int index] {
				get {
					return gridView.Columns[index].HeaderText;
				}
				set {
					gridView.Columns[index].HeaderText = value;
				}
			}
		}

		[Browsable(true)]
		public bool AllowNewItems {
			get { return allowNewRows; }
			set {
				allowNewRows = value;
				if (source != null)
					grid.AllowUserToAddRows = allowNewRows;
			}
		}

		[Browsable(true)]
		public ContextMenuStrip ItemContextMenu {
			get { return grid.ContextMenuStrip; }
			set { grid.ContextMenuStrip = value; }
		}
		#endregion

		#region Selection
		public IEnumerable<int> SelectedIndices {
			get {
				foreach (DataGridViewRow row in grid.SelectedRows)
					if (row.Index < source.Count)
						yield return row.Index;
			}
		}

		public int SelectionSize {
			get {
				int count = grid.SelectedRows.Count;
				//TODO: Test.
				if (grid.Rows.Count > 0 && grid.AllowUserToAddRows && grid.Rows[grid.RowCount - 1].Selected)
					count--;
				Debug.Assert(count >= 0);
				return count;
			}
		}
		#endregion

		#region Appearance
		private void InitializeGridStyles() {
			grid.CellFormatting += new DataGridViewCellFormattingEventHandler(grid_CellFormatting);
			//grid.DataError += new DataGridViewDataErrorEventHandler(grid_DataError);

			grid.Font = this.Font;

			grid.ColumnHeadersVisible = true;
			grid.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
			grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
			grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
			grid.ColumnHeadersDefaultCellStyle.Font = new Font(this.Font.Name, this.Font.Size * 1.1f, this.Font.Style, this.Font.Unit);
			grid.RowHeadersVisible = false;

			grid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
			grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			grid.AllowUserToResizeColumns = false;
			grid.AllowUserToResizeRows = false;
			grid.AllowUserToOrderColumns = false;

			grid.EnableHeadersVisualStyles = false;
			grid.AllowUserToResizeRows = false;
			grid.AllowUserToOrderColumns = false;

			grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		}

		void grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
			if (source == null)
				return;

			if (e.RowIndex == grid.RowCount - 1 && grid.AllowUserToAddRows) {
				//This is the new record row, not being typed in
				e.CellStyle.NullValue = e.ColumnIndex == 0 ? "Add Phrase Here" : "Add Translation Here";
				e.CellStyle.BackColor = Color.BurlyWood;
				e.CellStyle.SelectionBackColor = Color.BurlyWood;
				e.CellStyle.ForeColor = Color.White;
				e.CellStyle.SelectionForeColor = Color.White;
				return;
			} else if (e.RowIndex >= source.Count) {
				//Don't style the temporary row for now. It doesn't work very well.

				//This row was the new record row, but the user typed in it.
				//e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
				//if(ShowMutableRows)
				//    e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? Color.LightGoldenrodYellow : Color.PaleGoldenrod;
				//else
				//    e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? Color.WhiteSmoke : Color.White;
				return;
			}

			WordListEntry rowSource = null;
			if (source != null)
				rowSource = source[e.RowIndex];

			e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? SystemColors.Control : SystemColors.ControlLightLight;
			e.CellStyle.SelectionBackColor = (e.RowIndex % 2 == 0) ? Color.SteelBlue : Color.DeepSkyBlue;
			e.CellStyle.SelectionForeColor = Color.Black;
		}
		#endregion

		public void ScrollToIndex(int position) {
			grid.FirstDisplayedScrollingRowIndex = Math.Max(0, Math.Min(position, grid.RowCount - 1));
		}
	}
}