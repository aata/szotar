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
	/// <summary>Adds methods for programmatic selection, and facilitates multi-row drag and drop.</summary>
	class MouseFixDataGridView : DataGridView {
		#region Drag and Drop

		// Delayed selection change.
		bool deferredSelection;
		int deferredSelectionRow;
		int deferredSelectionColumn;
		Keys deferredSelectionModifiers;

		protected override void OnDragOver(DragEventArgs e) {
			deferredSelection = false;

			base.OnDragOver(e);
		}

		public Rectangle DragRectangle { get; protected set; }
		public int DragRow { get; protected set; }

		protected override void OnDragDrop(DragEventArgs e) {
			try {
				base.OnDragDrop(e);
			} finally {
				DragRectangle = Rectangle.Empty;
			}
		}

		// Computes the drag box. When the mouse leaves this with the button held down, the drag and drop
		// operation starts. Also modifies the right-click behaviour.
		// This also defers some of the selection behaviour to the OnMouseUp routine, so that it doesn't
		// interfere with drag and drop.
		protected override void OnMouseDown(MouseEventArgs e) {
			HitTestInfo hitTest;

			if (e.Button != MouseButtons.Left) {
				// If the right-clicked item is outside the current selection, select that item.
				// This is more like how normal list boxes work.
				if (e.Button == MouseButtons.Right) {
					hitTest = HitTest(e.X, e.Y);

					if (hitTest.Type == DataGridViewHitTestType.Cell && !IsRowSelected(hitTest.RowIndex))
						SelectRow(hitTest.RowIndex, hitTest.ColumnIndex);
				}

				base.OnMouseDown(e);
				return;
			}

			hitTest = HitTest(e.X, e.Y);

			// Don't drag the New Row.
			if (hitTest.Type == DataGridViewHitTestType.Cell) {
				int hitRow = hitTest.RowIndex;
				DragRow = hitRow;

				var dragSize = SystemInformation.DragSize;
				DragRectangle = new Rectangle(
					new Point(e.X - dragSize.Width / 2, e.Y - dragSize.Height / 2),
					dragSize);

				// We can safely extend the selection before dragging, but we must use the default selection
				// logic, because only that has access to the location of the anchor cell, which is needed
				// to extend a multiple selection.
				bool useDefaultSelectionLogic = false;
				switch (Control.ModifierKeys) {
					case Keys.Shift:
					case Keys.Control | Keys.Shift:
						useDefaultSelectionLogic = true;
						break;
				}

				// If there is currently a multiple selection and the user left-clicks on a part of that
				// selection, we don't want to set the selection to the hit row, since the user might want 
				// to drag the current selection.
				// The selection will be updated in the mouse up handler, so we save the row and column
				// for that purpose.
				if (IsRowSelected(hitRow)
					&& SelectedRowCount > 1
					&& !useDefaultSelectionLogic) {
					deferredSelection = true;
					deferredSelectionRow = hitRow;
					deferredSelectionColumn = hitTest.ColumnIndex;
					deferredSelectionModifiers = Control.ModifierKeys;
					return;
				}
			} else {
				DragRectangle = Rectangle.Empty;
			}

			base.OnMouseDown(e);
		}

		// Re-implements some of the behaviour that is deferred by in OnMouseDown because it would
		// interfere with drag and drop.
		protected override void OnMouseUp(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left && deferredSelection) {
				deferredSelection = false;

				bool isCurrentCell =
					deferredSelectionRow == CurrentCellAddress.Y &&
					deferredSelectionColumn == CurrentCellAddress.X;

				switch (deferredSelectionModifiers) {
					case Keys.None:
						SelectRow(deferredSelectionRow);

						// A click on the active cell begins editing the cell.
						if (isCurrentCell)
							BeginEdit(true);
						break;

					case Keys.Shift:
					case Keys.Shift | Keys.Control:
						// Handled in MouseDown.
						break;

					case Keys.Control:
						ToggleSelection(deferredSelectionRow);
						break;
				}
			}

			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left && !DragRectangle.IsEmpty && !DragRectangle.Contains(e.Location))
				deferredSelection = false;

			base.OnMouseMove(e);
		}

		public bool IsRowVisible(int row) {
			return row >= FirstDisplayedScrollingRowIndex 
				&& row <= FirstDisplayedScrollingRowIndex + DisplayedRowCount(false);
		}
		#endregion

		#region Drop Marker
		int dropMarkerRow;

		/// <summary>Places a drop marker at the specified row, removing any other drop markers.</summary>
		/// <param name="row">The row to place a drop marker at, or -1 to not place a drop marker.</param>
		public void PlaceDropMarker(int row) {
			if (row == dropMarkerRow)
				return;

			if (dropMarkerRow != -1)
				InvalidateRow(dropMarkerRow);

			dropMarkerRow = row;

			if(row != -1)
				InvalidateRow(row);
		}

		/// <summary>Equivalent to PlaceDropMarker(-1).</summary>
		public void RemoveDropMarker() {
			if (dropMarkerRow != -1)
				InvalidateRow(dropMarkerRow);

			dropMarkerRow = -1;
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			if (dropMarkerRow != -1) {
				var rect = GetRowDisplayRectangle(dropMarkerRow, false);
				rect.Height = 2; // SystemInformation.SizingBorderWidth;

				if(rect.IntersectsWith(e.ClipRectangle))
					using (var brush = new SolidBrush(Color.FromArgb(64, 64, 64)))
						e.Graphics.FillRectangle(brush, rect);
			}
		}
		#endregion

		#region Selection
		public bool IsRowSelected(int row) {
			return (Rows.GetRowState(row) & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected;
		}

		public void SelectRow(int row, int activeColumn) {
			ClearSelection();
			SetCurrentCellAddressCore(activeColumn, row, true, true, true);
			SetSelectedRowCore(row, true);

			if (!IsRowVisible(row))
				FirstDisplayedScrollingRowIndex = row;
		}

		public void SelectRow(int row) {
			SelectRow(row, 0);
		}

		// Selects a contiguous block of rows and changes the active cell to the be the first cell
		// of the first row of that block.
		public void SelectRowBlock(int firstRow, int count) {
			ClearSelection();

			// We need to change this because the currently active row and the row selection are
			// separate things. Typing or pressing F2 acts on the active row, which need not even 
			// be part of the selection.
			// Apparently we also need to set the current cell prior to setting the selection, 
			// otherwise the selection is reset to a single item.
			SetCurrentCellAddressCore(0, firstRow, true, true, true);

			for (int i = firstRow; i < firstRow + count; ++i)
				SetSelectedRowCore(i, true);

			// Make sure the rows are visible, at least partially.
			if (!IsRowVisible(firstRow))
				FirstDisplayedScrollingRowIndex = firstRow;
		}

		public void ToggleSelection(int row) {
			SetSelectedRowCore(row, !IsRowSelected(row));
		}

		/// <summary>
		/// Returns the amount of rows selected rows, including the new row and any uncommited rows.
		/// </summary>
		public int SelectedRowCount {
			get {
				return Rows.GetRowCount(DataGridViewElementStates.Selected);
			}
		}
		#endregion
	}

	[ToolboxBitmap(typeof(System.Windows.Forms.DataGridView))]
	public partial class DictionaryGrid : UserControl {
		WordList source;
		bool showMutableRows;
		bool allowNewRows;
		bool ignoreNextListChangedEvent = false;
		float columnRatio = 0.5f;

		WordListEntry pairInEdit = null;
		int? rowInEdit = null;
		bool dirty = false;

		public DictionaryGrid() {
			InitializeComponent();
			InitializeGridStyles();
			InitializeVirtualMode();
			InitializeDragAndDrop();

			this.Disposed += (s, e) => UnwireDataSourceEvents();
			grid.ColumnWidthChanged += (s, e) => {
				float ratio = (float)grid.Columns[0].Width / (float)grid.ClientSize.Width;

				if (Math.Abs(ratio - columnRatio) <= 0.02)
					return;

				columnRatio = ratio;

				EventHandler h = ColumnRatioChanged;
				if (h != null)
					h(this, new EventArgs());
			};
		}

		public DictionaryGrid(WordList dataSource)
			: this() {
			DataSource = dataSource;
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
					grid.CancelEdit();
					UpdateData();
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
			phraseColumn.Resizable = DataGridViewTriState.True;
			phraseColumn.FillWeight = columnRatio;
			grid.Columns.Add(phraseColumn);

			DataGridViewColumn translationColumn = new DataGridViewTextBoxColumn();
			translationColumn.HeaderText = Properties.Resources.TranslationDefaultHeader;
			translationColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			translationColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			translationColumn.FillWeight = 1.0f - columnRatio;
			grid.Columns.Add(translationColumn);

			DataSource = null;
		}

		void grid_DataError(object sender, DataGridViewDataErrorEventArgs e) {
			e.ThrowException = false;
		}

		void grid_KeyUp(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.None) {
				DeleteSelection();
				e.Handled = true;
			} else if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control) {
				source.Undo();
				e.Handled = true;
			} else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control) {
				Copy();
				e.Handled = true;
			} else if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control) {
				Cut();
				e.Handled = true;
			} else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control) {
				Paste();
				e.Handled = true;
			} else if (
				(e.KeyCode == Keys.Z && e.Modifiers == (Keys.Control | Keys.Shift)) ||
				(e.KeyCode == Keys.Y && e.Modifiers == Keys.Control)) {
				source.Redo();
				e.Handled = true;
			}
		}

		void DeleteSelection() {
			// When the delete key is pressed, the selected rows are deleted. To do this, we don't actually delete the rows 
			// from the grid: we delete the rows from the DataSource, and let the binding take care of the rest.
			// 
			// There's a snag, however: the editing row. If a row is in edit and hasn't been added to the list yet (i.e. rowInEdit >= source.Count)
			// then we can't actually delete it from the data source. So we delete it from the grid. However, this raises a slight problem too: when
			// the cell is deleted, the selection moves onto another cell -- we could end up deleting that cell, too, if we're not careful.
			// 
			// To mitigate this, we use the InternalSelectedIndices, not SelectedIndices: this returns rows not in the data source,
			// unlike SelectedIndices. Then we remove the editing row, delete it, and use the previous selection to 
			// guide us in what else to delete.
			//
			// Any other rows that aren't present in the data source (such as the New Row when it's not selected)
			// are simply removed from the rowsToDelete.

			var rowsToDelete = new List<int>(SelectedRowIndices);

			Debug.Print("Deleting rows:");
			foreach (int row in rowsToDelete) {
				Debug.Print("  Row {0}: {1} -- {2}{3}",
					row,
					grid[0, row].Value,
					grid[1, row].Value,
					row == rowInEdit ? " (being edited)" : "");
			}

			if (rowInEdit.HasValue && rowInEdit.Value >= source.Count) {
				rowsToDelete.Remove(rowInEdit.Value);
				grid.CancelEdit();
			}

			rowsToDelete.RemoveAll(x => x >= source.Count);

			source.RemoveAt(rowsToDelete);
		}

		void grid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
			// This is handled by the KeyUp event now.
			Debug.Print("Warning: UserDeletingRow fired (shouldn't this event have already been processed?)");
			e.Cancel = true;
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

					if (pairInEdit.Phrase == null)
						pairInEdit.Phrase = string.Empty;
					if (pairInEdit.Translation == null)
						pairInEdit.Translation = string.Empty;

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
			pairInEdit = new WordListEntry(null, null, null);
			rowInEdit = grid.Rows.Count - 1;

			Debug.Print("New row added to the grid: e.Row = {0}, rowInEdit = {1}", e.Row, rowInEdit.Value);
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
					pairInEdit = new WordListEntry(null, we.Phrase, we.Translation);
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

		#region Copy and paste
		public DataObject MakeDataObjectFromSelection() {
			return Selection().MakeDataObject();
		}

		public WordListEntries Selection() {
			var entries = new List<WordListEntry>();
			foreach (var i in SelectedEntryIndices)
				entries.Add(source[i]);

			return new WordListEntries(source, entries);
		}

		void PutSelectionOnClipboard(bool remove) {
			var entries = new List<WordListEntry>();
			var indices = new List<int>(SelectedEntryIndices);
			foreach (var i in indices) {
				if(remove)
					entries.Add(new WordListEntry(null, source[i].Phrase, source[i].Translation));
				else
					entries.Add(source[i]);
			}

			Clipboard.SetDataObject(new WordListEntries(null, entries).MakeDataObject());
			if (remove)
				source.RemoveAt(indices);
		}

		public void Cut() {
			PutSelectionOnClipboard(true);
		}

		public void Copy() {
			PutSelectionOnClipboard(false);
		}

		public bool CanPaste {
			get {
				return WordListEntries.CanConvertFrom(Clipboard.GetDataObject());
			}
		}

		void Paste(WordListEntries entries, int? row) {
			// Clone the entries if they're from a different word list.
			// Maybe this is a bit paranoid.
			for (int i = 0; i < entries.Items.Count; ++i)
				if (entries.Items[i].Owner != source)
					entries.Items[i] = entries.Items[i].Clone();

			int actualRow = row ?? source.Count;
			source.Insert(actualRow, entries.Items);

			grid.SelectRowBlock(actualRow, entries.Items.Count);
		}

		public void Paste() {
			int row = grid.Rows.GetFirstRow(DataGridViewElementStates.Selected);

			// If there's no item selected, just paste at the end.
			if (row < 0 || row >= source.Count)
				row = source.Count;

			var data = Clipboard.GetDataObject();
			var entries = WordListEntries.FromDataObject(data);

			if (entries != null && entries.Items.Count > 0) {
				Paste(entries, row);
			} else {
				string text = Clipboard.GetText().Trim();
				if (text.Contains("\n"))
					return;

				// If the cell is being edited, Ctrl-V will be handled by the textbox itself
				// so this code is only visited when a paste occurs with textual data on an
				// entry which isn't currently being edited.

				WordListEntry item = RowSource(row);
				if (item == null || row == rowInEdit) {
					Paste(new WordListEntries(source, new[] { new WordListEntry(source, text, string.Empty) }), row);
					return;
				}

				string phrase = item.Phrase, translation = item.Translation;
				if(grid.CurrentCellAddress.X == 0)
					phrase = text;
				else
					translation = text;

				if (row >= 0 && row < source.Count) {
					// Re-create the row source so that an undo list entry gets made.
					source[row] = new WordListEntry(source, phrase, translation);

					if(!grid.IsRowVisible(row))
						grid.FirstDisplayedScrollingRowIndex = row;
				}
			}
		}
		#endregion

		#region Drag and drop
		// This code is partly taken from/inspired by the DataGridView FAQ by Mark Rideout:
		// http://www.windowsforms.net/Samples/Go%20To%20Market/DataGridView/DataGridView%20FAQ.doc

		WordListEntries dragData;

		void InitializeDragAndDrop() {
			grid.AllowDrop = true;
			grid.DragOver += new DragEventHandler(grid_DragOver);
			grid.DragDrop += new DragEventHandler(grid_DragDrop);
			grid.MouseMove += new MouseEventHandler(grid_MouseMove);
			grid.DragLeave += new EventHandler(grid_DragLeave);
		}

		// Starts a drag operation if the mouse leaves the drag box with the button held down.
		void grid_MouseMove(object sender, MouseEventArgs e) {
			if (e.Button != MouseButtons.Left)
				return;

			if (!grid.DragRectangle.IsEmpty && !grid.DragRectangle.Contains(e.Location)) {
				bool hitSelectedRow = grid.IsRowSelected(grid.DragRow);

				// The drag data has to be computed when the mouse moves out of the drag rectangle, not on MouseDown,
				// because we need to wait for the mouse down to possibly affect the selection (such as extending it
				// or contracting it, which are done at MouseDown time).
				var rows = new List<WordListEntry>();
				if (hitSelectedRow) {
					// TODO: This won't include the row currently being edited, if one exists!
					foreach (int row in SelectedEntryIndices)
						rows.Add(source[row]);
				} else {
					// TODO: For now, let's not try to drag uncommitted rows.
					if (grid.DragRow < 0 || grid.DragRow > source.Count)
						return;

					rows.Add(source[grid.DragRow]);
					grid.SelectRow(grid.DragRow);
				}

				dragData = new WordListEntries(source, rows);
				var data = dragData.MakeDataObject();

				var result = grid.DoDragDrop(data, DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Scroll);

				// If we moved the items, we have to delete the original ones now.
				// This doesn't exactly look great in the undo list, but it's very unlikely that 
				// we'll be able to tie together undo items related to multiple lists.
				if (result == DragDropEffects.Move) {
					var indices = new List<int>();
					foreach (var entry in dragData.Items) {
						int i = source.IndexOf(entry);
						indices.Add(i);
					}

					source.RemoveAt(indices);
				}
			}
		}

		// Chooses at which index the row will be dropped, given mouse co-ordinates.
		int GetDropRow(int x, int y) {
			return GetDropRow(x, y, grid.HitTest(x, y));
		}

		int GetDropRow(int x, int y, DataGridView.HitTestInfo hit) {
			// If we're not dropped onto a row, figure out where is the best place to
			// put the dropped items.
			if (hit.Type == DataGridViewHitTestType.ColumnHeader) {
				return 0;
			} else if (hit.Type == DataGridViewHitTestType.None) {
				return source.Count;
			} else if (hit.RowIndex == -1) {
				if (y <= 0)
					return 0;
				return source.Count;
			}

			int row = hit.RowIndex;
		
			// For example, the New Row was dropped onto.
			if (row >= source.Count)
				return source.Count;

			// If we're on the top half of a row, we drop above the row.
			// If we're on the bottom half of a row, we drop below the row.
			var rect = grid.GetRowDisplayRectangle(row, false);
			var middle = rect.Top + rect.Height / 2;

			if (y > middle)
				return Math.Min(row + 1, source.Count);

			return row;
		}

		// Finishes the drag operation and updates the selection.
		void grid_DragDrop(object sender, DragEventArgs e) {
			grid.RemoveDropMarker();

			var clientPoint = grid.PointToClient(new Point(e.X, e.Y));
			int dropRow = GetDropRow(clientPoint.X, clientPoint.Y);

			if (dropRow < 0)
				return;
			if (dropRow >= source.Count)
				dropRow = source.Count;

			var entries = WordListEntries.FromDataObject(e.Data);

			// Were the entries moved or copied?
			bool moved = false;

			if(entries != null && entries.WordList == this.source) {
				// Copying from the grid to itself requires a bit more care.

				if (e.Effect == DragDropEffects.Move) {
					moved = true;

					// Dragging rows from this grid to itself: i.e. re-ordering the rows.
					source.MoveRows(new List<int>(entries.Indices), dropRow);

					// Set the drag result to None, so that the results won't be removed *again*.
					e.Effect = DragDropEffects.None;
				} else {
					CopyRowsFromTo(entries, dropRow);
				}

			} else if (entries != null) {
				// Copying/moving from some other word list, or a non-wordlist source
				CopyRowsFromTo(entries, dropRow);
			}

			// TODO: This doesn't seem to work if the drop row is the last row in the grid.
			if (entries != null && entries.Items.Count > 0) {
				int firstRow = dropRow;

				// If we're moving, not copying, we don't know where the rows were actually put,
				// because the rows may have been shifted. Instead, find where they actually are.
				if (moved)
					firstRow = source.IndexOf(entries.Items[0]);

				if (firstRow > -1)
					grid.SelectRowBlock(firstRow, entries.Items.Count);
			}
		}

		void CopyRowsFromTo(WordListEntries entries, int dropRow) {
			var copies = new List<WordListEntry>();
			foreach (var e in entries.Items)
				copies.Add(e.Clone());

			source.Insert(dropRow, copies);
		}

		// Determine the drag effect (copy/move), scroll the grid, and update the drop marker.
		void grid_DragOver(object sender, DragEventArgs e) {
			var clientPoint = grid.PointToClient(new Point(e.X, e.Y));
			var hitTest = grid.HitTest(clientPoint.X, clientPoint.Y);
			int rowIndex = hitTest.RowIndex;

			grid.PlaceDropMarker(GetDropRow(clientPoint.X, clientPoint.Y, hitTest));

			bool hasEntries = e.Data.GetDataPresent(typeof(WordListEntries));
			bool hasText = e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text);
			bool hasSearchResults = e.Data.GetDataPresent(typeof(TranslationPair[]));

			// Shift = 4, Ctrl = 8
			bool copyOverride = (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy && (e.KeyState & 8) == 8;
			bool moveOverride = (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move && (e.KeyState & 4) == 4;

			if (hasEntries)
				e.Effect = DragDropEffects.Move;
			else if (hasText || hasSearchResults)
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;

			if (e.Effect != DragDropEffects.None) {
				if (copyOverride)
					e.Effect = DragDropEffects.Copy;
				else if (moveOverride)
					e.Effect = DragDropEffects.Move;
			}

			// Scroll if the mouse is close to the top or bottom of the grid.
			if (e.Effect != DragDropEffects.None) {
				if (hitTest.Type == DataGridViewHitTestType.ColumnHeader)
					rowIndex = 0;
				else if (hitTest.Type == DataGridViewHitTestType.None)
					rowIndex = grid.RowCount;

				if (rowIndex != -1) {
					if (rowIndex < grid.FirstDisplayedScrollingRowIndex + 2
						&& grid.FirstDisplayedScrollingRowIndex > 0)
						grid.FirstDisplayedScrollingRowIndex--;
					else if (rowIndex >= grid.FirstDisplayedScrollingRowIndex + grid.DisplayedRowCount(false) - 2
						&& grid.FirstDisplayedScrollingRowIndex + grid.DisplayedRowCount(false) < grid.RowCount)
						grid.FirstDisplayedScrollingRowIndex++;
				}
			}
		}

		// Stop drawing the drop marker while the cursor is outside the grid.
		void grid_DragLeave(object sender, EventArgs e) {
			grid.RemoveDropMarker();
		}
		#endregion

		#region Selection 
		/// <summary>Works like SelectedIndices, but returns the New Row too, if it is selected.</summary>
		/// <remarks>The DataGridView.SelectedRows property is inefficient: it can cause shared rows
		/// to become unshared, for one. It should be avoided.</remarks>
		protected IEnumerable<int> SelectedRowIndices {
			get {
				for (int index = grid.Rows.GetFirstRow(DataGridViewElementStates.Selected);
					index >= 0;
					index = grid.Rows.GetNextRow(index, DataGridViewElementStates.Selected)) 
				{
					yield return index;
				}
			}
		}

		/// <summary>Returns the indices of the selected rows on the grid, in ascending order. 
		/// If there is a New Row that has not yet been added to the data source, it is not included.</summary>
		public IEnumerable<int> SelectedEntryIndices {
			get {
				for (int index = grid.Rows.GetFirstRow(DataGridViewElementStates.Selected);
					index >= 0;
					index = grid.Rows.GetNextRow(index, DataGridViewElementStates.Selected))
				{
					if (index < source.Count)
						yield return index;
				}
			}
		}

		public int SelectedEntryCount {
			get {
				int count = grid.Rows.GetRowCount(DataGridViewElementStates.Selected);
				for (int i = source.Count; i < grid.RowCount; ++i)
					if (grid.IsRowSelected(i))
						count--;
				return count;
			}
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

		public event EventHandler ColumnRatioChanged;

		/// <summary>The ratio of the left column to the width of the control, in percent.</summary>
		public float ColumnRatio {
			get {
				return grid.Columns[0].FillWeight;
			}
			set {
				if (value == columnRatio)
					return;

				if (grid.Columns.Count >= 2) {
					grid.Columns[0].FillWeight = value;
					grid.Columns[1].FillWeight = 1.0f - value;
				}

				columnRatio = value;
				EventHandler h = ColumnRatioChanged;
				if (h != null)
					h(this, new EventArgs());
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

			bool showEditPrompt =
				(e.RowIndex == rowInEdit && string.IsNullOrEmpty(pairInEdit.Phrase) && string.IsNullOrEmpty(pairInEdit.Translation))
				|| (e.RowIndex == grid.RowCount - 1 && grid.AllowUserToAddRows);

			if (showEditPrompt)
				e.CellStyle.NullValue = e.ColumnIndex == 0 ? "Add Phrase Here" : "Add Translation Here";
			
			if (e.ColumnIndex == grid.CurrentCellAddress.X && e.RowIndex == grid.CurrentCellAddress.Y) {
				e.CellStyle.BackColor = e.CellStyle.SelectionBackColor = Color.DarkSlateBlue;
				e.CellStyle.ForeColor = e.CellStyle.SelectionForeColor = Color.White;
				return;
			}

			if (showEditPrompt) {
				//This is either the new record row, not being typed in, or the new record row which is now being edited.
				e.CellStyle.BackColor = Color.BurlyWood;
				e.CellStyle.SelectionBackColor = Color.BurlyWood;
				e.CellStyle.ForeColor = Color.White;
				e.CellStyle.SelectionForeColor = Color.White;
				return;
			}

			if (e.RowIndex >= source.Count) {
				//Don't style the temporary row for now. It doesn't work very well.
				return;
			}

			WordListEntry rowSource = null;
			if (source != null)
				rowSource = source[e.RowIndex];

			e.CellStyle.BackColor = (e.RowIndex % 2 == 0) ? SystemColors.Control : SystemColors.ControlLightLight;
			//e.CellStyle.SelectionBackColor = (e.RowIndex % 2 == 0) ? Color.SteelBlue : Color.DeepSkyBlue;
			e.CellStyle.SelectionBackColor = (e.RowIndex % 2 == 0) ? Color.SteelBlue : Color.SkyBlue;
			e.CellStyle.SelectionForeColor = Color.Black;
		}
		#endregion

		public void ScrollToIndex(int position) {
			grid.FirstDisplayedScrollingRowIndex = Math.Max(0, Math.Min(position, grid.RowCount - 1));
		}
	}
}