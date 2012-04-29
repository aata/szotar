using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class StartPage : Form {
		TagMenu tagMenu;

		public StartPage() {
			InitializeComponent();

			// http://www.ben.geek.nz/controldesignmode-misbehaving/
			if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
				return;

			exitProgram.Text = string.Format(exitProgram.Text, Application.ProductName);

			recentItems.ListsChosen += new EventHandler(recentItems_ListsChosen);

			tagMenu = new TagMenu();
			listContextMenu.Items.Insert(listContextMenu.Items.IndexOf(newListCM), tagMenu);
		}

		#region List Search
		private void listContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
			var selectedLists = recentItems.SelectedLists();

			listContextMenuSeparator.Visible = flashcardsListMI.Visible = learnListMI.Visible = selectedLists.Count > 0;

			if (selectedLists.Count == 1)
				tagMenu.WordList = Sqlite.SqliteWordList.FromSetID(DataStore.Database, selectedLists[0].SetID);
			else
				tagMenu.WordList = null;
		}

		private void newListMI_Click(object sender, EventArgs e) {
			new ListBuilder().Show();
		}

		private void openListMI_Click(object sender, EventArgs e) {
			OpenTags(recentItems.SelectedTags());
			OpenLists(recentItems.SelectedLists());
			OpenDictionaries(recentItems.SelectedDictionaries());
		}

		private void recentItems_ListsChosen(object sender, EventArgs e) {
			OpenTags(recentItems.SelectedTags());
			OpenLists(recentItems.SelectedLists());
			OpenDictionaries(recentItems.SelectedDictionaries());
		}

		private void flashcardsListMI_Click(object sender, EventArgs e) {
			PracticeLists(PracticeMode.Flashcards, recentItems.SelectedLists());
		}

		private void learnListMI_Click(object sender, EventArgs e) {
			PracticeLists(PracticeMode.Learn, recentItems.SelectedLists());
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
				if (i != -1) {
					if (!opened[i].PositionHint.HasValue || opened[i].PositionHint.Value > (current.PositionHint ?? int.MaxValue)) 
						opened[i] = current;
				} else {
					opened.Add(current);
				}
			}

			return opened;
		}
		private void OpenTags(IList<string> tags) {
			if (tags.Count > 0)
				recentItems.SearchTerm = "tag:" + tags[0];
		}

		private void OpenDictionaries(IList<DictionaryInfo> dicts) {
			foreach(var di in dicts)
				LookupForm.OpenDictionary(di);
		}

		private void OpenLists(IList<ListSearchResult> chosen) {
			foreach (var open in UniqueLists(chosen))
				ListBuilder.Open(open.SetID).ScrollToResult(open);
		}

		private void PracticeLists(PracticeMode mode, IList<ListSearchResult> chosen) {
			var items = new List<ListSearchResult>();

			if (chosen == null || chosen.Count == 0)
				return;

			foreach (var item in chosen) {
				if (item.HasItem
					|| items.FindIndex(x => x.SetID == item.SetID && !x.HasItem) < 0)
					items.Add(item);
			}

			PracticeWindow.OpenNewSession(mode, items);
		}
		#endregion

		#region Menu items
		private void close_Click(object sender, EventArgs e) {
			Close();
		}

		private void exitProgram_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void debugLog_Click(object sender, EventArgs e) {
			ShowForm.Show<LogViewerForm>();
		}

		private void options_Click(object sender, EventArgs e) {
			ShowForm.Show<Preferences>();
		}

		private void about_Click(object sender, EventArgs e) {
			ShowForm.Show<About>();
		}

		private void practiceRandom_Click(object sender, EventArgs e) {
			new Forms.PracticeWindow(DataStore.Database.GetSuggestedPracticeItems(GuiConfiguration.PracticeDefaultCount), PracticeMode.Learn).Show();
		}
		#endregion

		private void findDuplicates_Click(object sender, EventArgs e) {
			ShowForm.Show<FindDuplicates>();
		}

		private void importDictionaryMI_Click(object sender, EventArgs e) {
			ShowForm.Show<DictionaryImport>();
		}

		private void importListMI_Click(object sender, EventArgs e) {
			ShowForm.Show<ImportForm>();
		}
	}
}