using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WordSearchResult = Szotar.Sqlite.SqliteDataStore.WordSearchResult;

namespace Szotar.WindowsForms.Controls {
	public partial class ListSearch : UserControl {
		private List<IListStore> listStores = new List<IListStore>();

		bool ReallyDesignMode {
			get {
				// http://www.ben.geek.nz/controldesignmode-misbehaving/
				return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
			}
		}

		public ListSearch() {
			InitializeComponent();

			MaxItems = 100;

			if (ReallyDesignMode)
				return;

			listStores.Add(new RecentListStore());
			listStores.Add(new SqliteListStore(DataStore.Database));

			search.Click += (s, e) => UpdateResults();
			searchBox.TextChanged += (s, e) => UpdateResults();
			results.Resize += (s, e) => ResizeColumns();
			Disposed += (s, e) => UnwireDBEvents();

			ThemeHelper.UseExplorerTheme(results);

			WireDBEvents();
			UpdateResults();
		}

		void WireDBEvents() {
			DataStore.Database.WordListDeleted += new EventHandler<Szotar.Sqlite.WordListDeletedEventArgs>(Database_WordListDeleted);
		}

		void UnwireDBEvents() {
			DataStore.Database.WordListDeleted -= new EventHandler<Szotar.Sqlite.WordListDeletedEventArgs>(Database_WordListDeleted);
		}

		// When a word list is deleted, remove it from the results view.
		void Database_WordListDeleted(object sender, Szotar.Sqlite.WordListDeletedEventArgs e) {
			for (int i = 0; i < results.Items.Count; ) {
				var tag = results.Items[i].Tag;
				if (tag is ListInfo && ((ListInfo)tag).ID == e.SetID)
					results.Items.RemoveAt(i);
				else if (tag is WordSearchResult && ((WordSearchResult)tag).SetID == e.SetID)
					results.Items.RemoveAt(i);
				else
					i++;
			}
		}

		private bool PopulateResults(int? maxCount) {
			foreach (IListStore store in listStores) {
				string storeName = store.Name;
				ListViewGroup group = new ListViewGroup(storeName);
				results.Groups.Add(group);
				foreach (ListInfo list in store.GetLists()) {
					if (searchBox.Text.Length > 0 && list.Name.IndexOf(searchBox.Text, StringComparison.CurrentCultureIgnoreCase) == -1)
						continue;

					if (results.Items.Count > maxCount)
						return false;

					ListViewItem item = new ListViewItem(
						new string[] { list.Name, 
						               list.Date.HasValue ? list.Date.Value.ToString() : string.Empty, 
									   list.TermCount.HasValue ? string.Format(Properties.Resources.NTerms, list.TermCount.Value) : string.Empty });
					item.Tag = list;
					item.Group = group;
					results.Items.Add(item);
				}
			}

			if (!string.IsNullOrEmpty(searchBox.Text)) {
				var group = new ListViewGroup(Properties.Resources.SearchResults);
				results.Groups.Add(group);
				foreach (var wsr in DataStore.Database.SearchAllEntries(searchBox.Text)) {
					if (results.Items.Count > maxCount)
						return false;

					ListViewItem item = new ListViewItem(
							new[] { wsr.Phrase, 
							         wsr.Translation, 
							         wsr.SetName });
					item.Tag = wsr;
					item.Group = group;
					results.Items.Add(item);
				}
			}

			return true;
		}

		private void UpdateResults() {
			if (ReallyDesignMode)
				return;

			results.BeginUpdate();
			results.Items.Clear();

			results.View = View.Details;

			if (!PopulateResults(MaxItems)) {
				// Add an extra item that, when activated, will show all results.
				ListViewGroup group = new ListViewGroup(Properties.Resources.ShowMore);
				var text = string.Format(Properties.Resources.OnlyNResultsShown, MaxItems);
				var item = new ListViewItem(text);
				item.Tag = "Truncated";
				item.Group = group;
				item.ToolTipText = text;

				results.Groups.Add(group);
				results.Items.Add(item);
			}

			thirdColumn.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			ResizeColumns();

			results.EndUpdate();
		}

		public void ResizeColumns() {
			var available = results.ClientSize.Width - thirdColumn.Width;
			firstColumn.Width = available / 2;
			secondColumn.Width = results.ClientSize.Width - thirdColumn.Width - firstColumn.Width;
		}

		private void results_ItemActivate(object sender, EventArgs e) {
			var lists = Accept();

			var h = ListsChosen;
			if (h != null)
				h(this, new ListsChosenEventArgs(lists));
		}

		public List<ListSearchResult> Accept() {
			var lists = new List<ListSearchResult>();

			if (results.SelectedItems.Count == 0)
				return new List<ListSearchResult>();

			foreach (ListViewItem item in results.SelectedItems) {
				object tag = item.Tag;
				if (tag is ListInfo) {
					var list = (ListInfo)tag;
					lists.Add(new ListSearchResult(list.ID.Value));
				} else if (tag is Szotar.Sqlite.SqliteDataStore.WordSearchResult) {
					var wsr = (Szotar.Sqlite.SqliteDataStore.WordSearchResult)tag;
					lists.Add(new ListSearchResult(wsr.SetID, wsr.ListPosition));
				} else if (tag.Equals("Truncated")) {
					// Taking this path when many items are selected would probably be annoying.
					if (results.SelectedItems.Count == 1) {
						MaxItems = null;
						UpdateResults();
						return new List<ListSearchResult>();
					}
				}
			}

			return lists;
		}

		public event EventHandler<ListsChosenEventArgs> ListsChosen;

		/// <summary>The context menu shown for the results list.</summary>
		public ContextMenuStrip ListContextMenuStrip {
			get { return results.ContextMenuStrip; }
			set { results.ContextMenuStrip = value; }
		}

		/// <summary>The maximum number of results shown before the list is truncated.</summary>
		/// <remarks>If the value is null, all the results are shown.</remarks>
		public int? MaxItems { get; set; }
	}

	public class ListsChosenEventArgs : EventArgs {
		public IList<ListSearchResult> Chosen { get; set; }

		public ListsChosenEventArgs(IList<ListSearchResult> ids) {
			Chosen = ids;
		}
	}
}
