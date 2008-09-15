using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
	public partial class ListSearch : UserControl {
		private List<IListStore> listStores = new List<IListStore>();

		public ListSearch() {
			InitializeComponent();

			listStores.Add(new RecentListStore());
			listStores.Add(new DefaultListStore());

			searchBox.TextChanged += new EventHandler(searchBox_TextChanged);
			ThemeHelper.UseExplorerTheme(results);
		}

		void searchBox_TextChanged(object sender, EventArgs e) {
			UpdateResults();
		}

		private void UpdateResults() {
			results.BeginUpdate();
			results.Items.Clear();

			results.View = View.Details;

			foreach (IListStore store in listStores) {
				string storeName = store.Name;
				ListViewGroup group = new ListViewGroup(storeName);
				results.Groups.Add(group);
				foreach (ListInfo list in store.GetLists()) {
					if (searchBox.RealText.Length > 0 && list.Name.IndexOf(searchBox.Text, StringComparison.CurrentCultureIgnoreCase) == -1)
						continue;

					ListViewItem item = new ListViewItem(new string[] { list.Name, list.Path, storeName });
					item.Tag = list;
					item.Group = group;
					results.Items.Add(item);
				}
			}

			results.EndUpdate();

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

			List<string> lists = new List<string>();
			foreach (ListViewItem item in results.SelectedItems)
				lists.Add(((ListInfo)item.Tag).Path);

			EventHandler<ListsChosenEventArgs> handler = ListsChosen;
			if (handler != null)
				handler(this, new ListsChosenEventArgs(lists));
		}

		public event EventHandler<ListsChosenEventArgs> ListsChosen;
	}

	public class ListsChosenEventArgs : EventArgs {
		IList<string> paths;

		public IList<string> Paths {
			get { return paths; }
			private set { paths = value; }
		}

		public ListsChosenEventArgs(IList<string> paths) {
			Paths = paths;
		}
	}

	internal class SearchResult {
		string name, path, source;

		public string Source {
			get { return source; }
			set { source = value; }
		}

		public string Path {
			get { return path; }
			set { path = value; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public SearchResult(string name, string path, string source) {
			Name = name;
			Path = path;
			Source = source;
		}
	}
}
