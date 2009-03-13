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
				if (DesignMode)
					return true;
				if (Site != null && Site.DesignMode)
					return true;
				return false;
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
			ThemeHelper.SetNoHScroll(results);

			WireDBEvents();
			UpdateResults();
		}

		void WireDBEvents() {
			DataStore.Database.WordListDeleted += new EventHandler<Szotar.Sqlite.WordListDeletedEventArgs>(Database_WordListDeleted);
		}

		void UnwireDBEvents() {
			DataStore.Database.WordListDeleted -= new EventHandler<Szotar.Sqlite.WordListDeletedEventArgs>(Database_WordListDeleted);
		}

		//When a word list is deleted, remove it from the results view.
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
					if (searchBox.RealText.Length > 0 && list.Name.IndexOf(searchBox.Text, StringComparison.CurrentCultureIgnoreCase) == -1)
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

			if (!string.IsNullOrEmpty(searchBox.RealText)) {
				var group = new ListViewGroup(Properties.Resources.SearchResults);
				results.Groups.Add(group);
				foreach (var wsr in DataStore.Database.SearchAllEntries(searchBox.RealText)) {
					if (results.Items.Count > maxCount)
						return false;

					ListViewItem item = new ListViewItem(
							new string[] { wsr.Phrase, 
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

		[Localizable(true)]
		[Browsable(true)]
		[DisplayName("Accept Text"), Category("List Search")]
		[Description("The text which be shown on the confirmation button. This property is localisable.")]
		[DefaultValue("&Practice")]
		public string AcceptText {
			get {
				return acceptButton.Text;
			}
			set {
				acceptButton.Text = value;
			}
		}

		private void results_ItemActivate(object sender, EventArgs e) {
			Accept();
		}

		private void acceptButton_Click(object sender, EventArgs e) {
			Accept();
		}

		private void Accept() {
			if (results.SelectedItems.Count == 0)
				return;

			List<ListOpen> lists = new List<ListOpen>();
			foreach (ListViewItem item in results.SelectedItems) {
				object tag = item.Tag;
				if (tag is ListInfo) {
					var list = (ListInfo)tag;
					lists.Add(new ListOpen { SetID = list.ID.Value });
				} else if(tag is Szotar.Sqlite.SqliteDataStore.WordSearchResult) {
					var wsr = (Szotar.Sqlite.SqliteDataStore.WordSearchResult)tag;
					lists.Add(new ListOpen { SetID = wsr.SetID, Position = wsr.ListPosition });
				} else if(tag.Equals("Truncated")) {
					// Taking this path when many items are selected would probably be annoying.
					if (results.SelectedItems.Count == 1) {
						MaxItems = null;
						UpdateResults();
						return;
					}
				}
			}

			EventHandler<ListsChosenEventArgs> handler = ListsChosen;
			if (handler != null)
				handler(this, new ListsChosenEventArgs(lists));
		}

		public event EventHandler<ListsChosenEventArgs> ListsChosen;

		public int? MaxItems { get; set; }
	}

	public struct ListOpen {
		public long SetID { get; set; }
		public int Position { get; set; }
	}

	public class ListsChosenEventArgs : EventArgs {
		public IList<ListOpen> Chosen { get; set; }

		public ListsChosenEventArgs(IList<ListOpen> ids) {
			Chosen = ids;
		}
	}
}
