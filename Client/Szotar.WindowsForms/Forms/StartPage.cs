using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class StartPage : Form {
		public StartPage() {
			InitializeComponent();

			ThemeHelper.UseExplorerTheme(dictionaries, recentDictionaries, recentLists);
			recentDictionaries.Scrollable = recentLists.Scrollable = false;

			listSearch.ListsChosen += new EventHandler<Controls.ListsChosenEventArgs>(listSearch_ListsChosen);

			if (DesignMode)
				return;

			dictionaries.BeginUpdate();
			var dicts = new List<DictionaryInfo>(Szotar.Dictionary.GetAll());
			dicts.Sort((d1, d2) => d1.Name.CompareTo(d2.Name));
			foreach (DictionaryInfo dict in dicts) {
				int size = dict.SectionSizes != null ? dict.SectionSizes[0] + dict.SectionSizes[1] : 0;
				ListViewItem item = new ListViewItem(new [] { dict.Name, size > 0 ? size.ToString() : "", dict.Author });
				item.Tag = dict;
				item.ImageKey = "Dictionary";
				dictionaries.Items.Add(item);
			}
			dictionaries.EndUpdate();

			PopulateRecentDictionaries();
			PopulateRecentLists();

			recentDictionaries.ItemActivate += new EventHandler(recentDictionaries_ItemActivate);
			recentDictionaries.Resize += new EventHandler((s, e) => recentDictionaries.Columns[0].Width = recentDictionaries.ClientSize.Width);
			recentDictionaries.Columns[0].Width = recentDictionaries.ClientSize.Width;

			recentLists.ItemActivate += new EventHandler(recentLists_ItemActivate);
			recentLists.Resize += new EventHandler((s, e) => recentLists.Columns[0].Width = recentLists.ClientSize.Width);
			recentLists.Columns[0].Width = recentLists.ClientSize.Width;

			//DistributeColumns(recentDictionaries, 100);
			//DistributeColumns(recentLists, 100);
			//DistributeColumns(listView1, 100);
			//DistributeColumns(dictionaries, 65, 10, 25);

			DataStore.UserDataStore.EnsureDirectoryExists(Configuration.DictionariesFolderName);
			fileSystemWatcher.Path = System.IO.Path.Combine(DataStore.UserDataStore.Path, Configuration.DictionariesFolderName);
			fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(fileSystemWatcher_Created);
			fileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(fileSystemWatcher_Deleted);
			fileSystemWatcher.Renamed += new System.IO.RenamedEventHandler(fileSystemWatcher_Renamed);

			Configuration.Default.SettingChanged += new EventHandler<SettingChangedEventArgs>(SettingChanged);
			this.FormClosed += new FormClosedEventHandler(OnFormClosed);
		}

		private static void DistributeColumns(ListView lv, params int[] weights) {
			if (weights.Length < lv.Columns.Count)
				throw new ArgumentException("Not enough arguments to DistributeColumns: the target ListView has more columns than weights given", "weights");

			int totalWeight = 0;
			foreach (var w in weights)
				totalWeight += w;

			EventHandler handler = new EventHandler((s, e) => {
				//lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				int sum = 0;
				foreach (ColumnHeader column in lv.Columns) {
					if(column.Index == lv.Columns.Count - 1) {
						column.Width = lv.ClientSize.Width - sum - 5;
					} else {
						column.Width = (int)Math.Floor((float)weights[column.Index] / (float)totalWeight * lv.ClientSize.Width);
						sum += column.Width;
					}
				}
			});
			handler(null, null);
			lv.Resize += handler;
		}

		// Unhook event handlers attached to external objects, to prevent keeping this object alive unnecessarily.
		void OnFormClosed(object sender, FormClosedEventArgs e) {
			Configuration.Default.SettingChanged -= new EventHandler<SettingChangedEventArgs>(SettingChanged);
		}

		void SettingChanged(object sender, SettingChangedEventArgs e) {
			if (e.SettingName == "RecentLists") {
			} else if (e.SettingName == "RecentDictionaries") {
				PopulateRecentDictionaries();
			}
		}

		#region Recent Dictionaries
		void recentDictionaries_ItemActivate(object sender, EventArgs e) {
			foreach (ListViewItem item in recentDictionaries.SelectedItems) {
				if (item.Tag != null && item.Tag is string)
					LookupForm.OpenDictionary(item.Tag as string);
			}
		}

		private void PopulateRecentDictionaries() {
			var rd = GuiConfiguration.RecentDictionaries;
			if(rd == null) {
				recentDictionaries.Items.Clear();
				return;
			}

			WithUpdate(recentDictionaries, lv => {
				lv.Items.Clear();
				for (int i = 0; lv.Items.Count < 6 && i < rd.Entries.Count; ++i) {
					if (System.IO.File.Exists(rd.Entries[i].Path)) {
						var item = new ListViewItem(new[] { rd.Entries[i].Name });
						item.Tag = rd.Entries[i].Path;
						item.Text = rd.Entries[i].Name;
						item.ImageKey = "Dictionary";
						lv.Items.Add(item);
					}
				}
			});
		}
		#endregion

		#region Recent Lists
		long? ListIDFromRecentList(ListViewItem item) {
			if (item.Tag != null && item.Tag is long)
				return (long)item.Tag;
			return null;
		}

		void recentLists_ItemActivate(object sender, EventArgs e) {
			OpenSelectedRecentLists();
		}

		void OpenSelectedRecentLists() {			
			foreach (ListViewItem item in recentLists.SelectedItems) {
				long? id = ListIDFromRecentList(item);
				if (id != null)
					ListBuilder.Open(id.Value);
			}
		}

		void PracticeSelectedRecentLists() {
			var lists = new List<ListSearchResult>();

			foreach (ListViewItem item in recentLists.SelectedItems) {
				long? id = ListIDFromRecentList(item);
				if (id != null)
					lists.Add(new ListSearchResult(id.Value));
			}

			PracticeLists(lists);
		}

		private void PopulateRecentLists() {
			var mru = Configuration.RecentLists;
			if (mru == null) {
				recentLists.Items.Clear();
				return;
			}

			WithUpdate(recentLists, lv => {
				lv.Items.Clear();
				for (int i = 0; i < 6 && i < mru.Count; ++i) {
					var item = new ListViewItem(new[] { mru[i].Name });
					item.Tag = mru[i].ID;
					item.Text = mru[i].Name;
					item.ImageKey = "List";
					lv.Items.Add(item);
				}
			});
		}
		#endregion

		#region File system watching
		void fileSystemWatcher_Renamed(object sender, System.IO.RenamedEventArgs e) {
			foreach (ListViewItem item in dictionaries.Items) {
				var di = item.Tag as DictionaryInfo;
				// The best test we can really do is Path.Equals. Hopefully that's good enough and normalisation
				// issues won't exist.
				if (di != null && System.IO.Path.Equals(di.Path, e.OldFullPath)) {
					di.Path = e.FullPath;
				}
			}
		}

		void fileSystemWatcher_Deleted(object sender, System.IO.FileSystemEventArgs e) {
			foreach (ListViewItem item in dictionaries.Items) {
				var di = item.Tag as DictionaryInfo;
				if (di != null && System.IO.Path.Equals(di.Path, e.FullPath)) {
					dictionaries.Items.Remove(item);
					// The item paths should be unique. If that isn't the case, there's something else wrong.
					// Thus we can safely return and avoid using the iterator after invalidating it.
					return;
				}
			}
		}

		// It seems like a bad idea to watch for created files. The files may not be fully written, and may even
		// be locked, by the time the Created event is fired.
		// There should either be a mechanism to refresh or a delay in the adding of this. If there is a delay, it should
		// also take into account the possibility that the file has been deleted before the delay completed.
		void fileSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e) {
			//TODO: Implement this.
		}
		#endregion

		#region List Search
		private void openList_Click(object sender, EventArgs e) {
			OpenLists(listSearch.Accept());
		}

		private void listSearch_ListsChosen(object sender, Controls.ListsChosenEventArgs e) {
			OpenLists(e.Chosen);
		}

		private void practiceList_Click(object sender, EventArgs e) {
			PracticeLists(listSearch.Accept());
		}

		T? NullableMin<T>(T? x, T? y)
			where T : struct, IComparable<T> {
			if (x == null && y == null)
				return null;
			else if (x == null)
				return y;
			else if (y == null)
				return x;

			if (x.Value.CompareTo(y.Value) <= 0)
				return x;
			else
				return y;
		}

		private IList<ListSearchResult> UniqueLists(IList<ListSearchResult> chosen) {
			var opened = new List<ListSearchResult>();

			foreach (var current in chosen) {
				var i = opened.FindIndex(x => x.SetID == current.SetID);
				if (i != -1)
					opened[i] = new ListSearchResult(
						current.SetID,
						NullableMin(opened[i].Position, current.Position));
				else
					opened.Add(current);
			}

			return opened;
		}

		#endregion

		#region Context Menu
		private void openListMI_Click(object sender, EventArgs e) {
			if (ActiveControl == recentLists)
				OpenSelectedRecentLists();
			else if (ActiveControl == listSearch)
				OpenLists(listSearch.Accept());
		}

		private void practiceListMI_Click(object sender, EventArgs e) {
			if (ActiveControl == recentLists)
				PracticeSelectedRecentLists();
			else if (ActiveControl == listSearch)
				PracticeLists(listSearch.Accept());
		}
		#endregion

		private void OpenLists(IList<ListSearchResult> chosen) {
			foreach (var open in UniqueLists(chosen))
				ListBuilder.Open(open.SetID).ScrollToPosition(open.Position ?? 0);
		}

		private void PracticeLists(IList<ListSearchResult> chosen) {
			var items = new List<ListSearchResult>();

			if (chosen == null || chosen.Count == 0)
				return;

			foreach (var item in chosen) {
				if (item.Position.HasValue
					|| items.FindIndex(x => x.SetID == item.SetID && x.Position == null) < 0
					)
					items.Add(item);
			}

			PracticeWindow.OpenNewSession(items);
		}

		private void OnDictionaryItemActivate(object sender, EventArgs e) {
			DictionaryInfo dict = dictionaries.SelectedItems[0].Tag as DictionaryInfo;

			try {
				LookupForm.OpenDictionary(dict);
			} catch (DictionaryLoadException ex) {
				MessageBox.Show(this,
					string.Format(
						CultureInfo.CurrentUICulture, 
						Resources.Errors.CouldNotLoadDictionary,
						dict.Name, 
						ex.Message),
					ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public static StartPage ShowStartPage(StartPageTab? tab) {
			StartPage sp = null;

			foreach (Form form in Application.OpenForms) {
				if (form is StartPage) {
					sp = (StartPage)form;
					form.BringToFront();
					break;
				}
			}

			if (sp == null) {
				sp = new StartPage();
				sp.Show();
			}

			if (tab.HasValue)
				sp.SwitchToTab(tab.Value);
			return sp;
		}

		private void SwitchToTab(StartPageTab tab) {
			if (tab == StartPageTab.Main)
				tasks.SelectedIndex = 0;
			else if (tab == StartPageTab.Dictionaries)
				tasks.SelectedIndex = 1;
			else if(tab == StartPageTab.WordLists)
				tasks.SelectedIndex = 2;
		}

		private void tasks_Selected(object sender, TabControlEventArgs e) {
			if (e.Action == TabControlAction.Selected && e.TabPage == dictionaryTab) {
				// Makes it easier to scroll with the mouse wheel after clicking the tab.
				// This causes usability problems of its own, however.
				// TODO: Investigate sending missed scroll events to the ListView.
				dictionaries.Select();
			}
		}

		private static void WithUpdate(ListView list, Action<ListView> action) {
			list.BeginUpdate();
			try {
				action(list);
			} finally {
				list.EndUpdate();
			}
		}

		private void reportBug_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			System.Diagnostics.Process.Start("http://code.google.com/p/szotar/issues/entry?template=Defect%20report%20from%20user");
		}
	}

	public enum StartPageTab {
		Main,
		Dictionaries,
		WordLists
	}
}