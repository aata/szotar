using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using WordSearchResult = Szotar.Sqlite.SqliteDataStore.WordSearchResult;

namespace Szotar.WindowsForms.Controls {
	public partial class ListSearch : UserControl {
		private List<IListStore> listStores = new List<IListStore>();
        private List<DictionaryInfo> dictionaries;

		bool ReallyDesignMode {
			get {
				// http://www.ben.geek.nz/controldesignmode-misbehaving/
				return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
			}
		}


        readonly IListStore recentListStore;
		public ListSearch() {
			InitializeComponent();

			MaxItems = 100;

			if (ReallyDesignMode)
				return;

			listStores.Add(recentListStore = new RecentListStore());
			listStores.Add(new SqliteListStore(DataStore.Database));

			search.Click += (s, e) => UpdateResults();
			searchBox.TextChanged += (s, e) => UpdateResults();
			results.Resize += (s, e) => ResizeColumns();
			Disposed += (s, e) => UnwireDBEvents();

            // It is much better to use a FileSystemWatcher and keep track of what dictionaries are available
            // than loading the whole list of dictionaries each time the search term changes.
            DataStore.UserDataStore.EnsureDirectoryExists(Configuration.DictionariesFolderName);
            dictionaries = Dictionary.GetAll().ToList();
            fileSystemWatcher.Path = System.IO.Path.Combine(DataStore.UserDataStore.Path, Configuration.DictionariesFolderName);
            fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(fileSystemWatcher_Created);
            fileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(fileSystemWatcher_Deleted);
            fileSystemWatcher.Renamed += new System.IO.RenamedEventHandler(fileSystemWatcher_Renamed);
            
            ThemeHelper.UseExplorerTheme(results);

			WireDBEvents();
			UpdateResults();

            this.Load += delegate { UpdateResults(); };
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
            results.Groups.Clear();

            if (ShowDictionaries) {
                ListViewGroup dictsGroup = new ListViewGroup(Properties.Resources.Dictionaries);
                results.Groups.Add(dictsGroup);

                Action<DictionaryInfo> addItem = dict => {
                    int entries = dict.SectionSizes == null ? 0 : dict.SectionSizes.Sum();

                    var item = new ListViewItem(new string[] { 
                        dict.Name, 
                        entries > 0 ? string.Format(Properties.Resources.NEntries, entries) : string.Empty,
                        dict.Author ?? string.Empty
                    }, "Dictionary");
                    item.Group = dictsGroup;
                    item.Tag = dict;
                    results.Items.Add(item);
                };

                string term = searchBox.Text.Trim().ToLower();
                if (term.Length == 0) {
                    dictsGroup.Header = Properties.Resources.RecentDictionaries;
                    foreach(var dict in GuiConfiguration.RecentDictionaries.Entries)
                        if (System.IO.File.Exists(dict.Path))
                            addItem(dict);
                } else { 
                    foreach(var dict in dictionaries)
                        if (dict.Name.ToLower().Contains(term))
                            addItem(dict);
                }
            }

            var recentLists = recentListStore.GetLists().ToList();

			foreach (IListStore store in listStores) {
				string storeName = store == recentListStore ? Properties.Resources.RecentListStoreName : Properties.Resources.DefaultListStoreName;
				ListViewGroup group = new ListViewGroup(storeName);
				results.Groups.Add(group);
				foreach (ListInfo list in store.GetLists()) {
					if (searchBox.Text.Length > 0 && list.Name.IndexOf(searchBox.Text, StringComparison.CurrentCultureIgnoreCase) == -1)
						continue;

					if (results.Items.Count > maxCount)
						return false;

                    if (store != recentListStore && recentLists.Count(a => a.ID == list.ID) > 0)
                        continue;

					ListViewItem item = new ListViewItem(
						new string[] { list.Name, 
						               list.Date.HasValue ? list.Date.Value.ToString() : string.Empty, 
									   list.TermCount.HasValue ? string.Format(Properties.Resources.NTerms, list.TermCount.Value) : string.Empty },

                        "List");
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

        // Keeps the dictionary list up-to-date and adds/removes items from the UI when the file system changes.
        #region File system watching
        void fileSystemWatcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            foreach (ListViewItem item in results.Items)
            {
                var di = item.Tag as DictionaryInfo;
                // The best test we can really do is Path.Equals. Hopefully that's good enough and normalisation
                // issues won't exist.
                if (di != null && System.IO.Path.Equals(di.Path, e.OldFullPath))
                {
                    di.Path = e.FullPath;
                }
            }

            foreach (var di in dictionaries)
                if (System.IO.Path.Equals(di.Path, e.OldFullPath))
                    di.Path = e.FullPath;
        }

        void fileSystemWatcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            foreach (ListViewItem item in results.Items)
            {
                var di = item.Tag as DictionaryInfo;
                if (di != null && System.IO.Path.Equals(di.Path, e.FullPath))
                {
                    results.Items.Remove(item);
                    // The item paths should be unique. If that isn't the case, there's something else wrong.
                    // Thus we can safely return and avoid using the iterator after invalidating it.
                    return;
                }
            }

            dictionaries.RemoveAll(di => System.IO.Path.Equals(di.Path, e.FullPath));
        }

        // It seems like a bad idea to watch for created files. The files may not be fully written, and may even
        // be locked, by the time the Created event is fired.
        // There should either be a mechanism to refresh or a delay in the adding of this. If there is a delay, it should
        // also take into account the possibility that the file has been deleted before the delay completed.
        void fileSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            if (System.IO.Path.GetExtension(e.FullPath) != ".dict" && System.IO.Path.GetExtension(e.FullPath) != ".dictx")
                return;

            var timer = new Timer();
            timer.Interval = 1000;
            EventHandler handler = null;
            handler = new EventHandler(delegate
            {
                timer.Stop();
                timer.Tick -= handler;
                timer.Dispose();
                dictionaries = Dictionary.GetAll().ToList();
                UpdateResults();
            });
            timer.Tick += handler;
            timer.Start();
        }
        #endregion

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
			var lists = AcceptLists();
            var dicts = AcceptDictionaries();

			var h = ListsChosen;
			if (h != null)
				h(this, new ListsChosenEventArgs(lists, dicts));
		}

        public List<DictionaryInfo> AcceptDictionaries() {
            var dicts = new List<DictionaryInfo>();

            foreach (ListViewItem item in results.SelectedItems) {
                object tag = item.Tag;
                if (tag is DictionaryInfo)
                    dicts.Add((DictionaryInfo)tag);
            }

            return dicts;
        }

		public List<ListSearchResult> AcceptLists() {
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

        [Browsable(true)]
        [Description("Whether or not dictionaries are shown in the results view.")]
        public bool ShowDictionaries { get; set; }
	}

	public class ListsChosenEventArgs : EventArgs {
        public IList<DictionaryInfo> ChosenDictionaries { get; set; }
		public IList<ListSearchResult> Chosen { get; set; }

		public ListsChosenEventArgs(IList<ListSearchResult> ids, IList<DictionaryInfo> dicts) {
			Chosen = ids;
            ChosenDictionaries = dicts;
		}
	}
}
