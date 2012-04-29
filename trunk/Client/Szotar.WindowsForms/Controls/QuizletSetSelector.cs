using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
	using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;

    [ImporterUI(typeof(QuizletImporter))]
	public partial class QuizletSetSelector : UserControl, IImporterUI<WordList> {
		long? selectedSet;
		QuizletImporter importer;
        CancellationTokenSource cts;
        DisposableComponent disposableComponent;
        bool searching;

		public IImporter<WordList> Importer {
			get { return importer; }
		}

		public QuizletSetSelector() {
			InitializeComponent();

            importer = new QuizletImporter();

            if (this.components == null)
                this.components = new System.ComponentModel.Container();

            cts = new CancellationTokenSource();
            components.Add(disposableComponent = new DisposableComponent(cts));

            searching = false;
			importButton.Enabled = false;
			searchResults.SelectedIndexChanged += new EventHandler(searchResults_SelectedIndexChanged);
		}

		public event EventHandler Finished;
		private void OnFinished() {
			EventHandler h = Finished;
			if (h != null)
				h(this, new EventArgs());
		}

		public void Apply() {
			importer.Set = selectedSet.Value;
		}

		private void StartSearch() {
			AbortRequest();

			string search = searchBox.Text;
            new QuizletAPI().SearchSets(searchBox.Text.Trim(), SetResults, SearchError, cts.Token);

            searching = true;
			progressBar.Visible = true;
			searchButton.Text = "&Abort";
		}

		// Aborts the current web request, if any.
		private void AbortRequest() {
            if (searching) {
                cts.Cancel();
                cts.Dispose();
                disposableComponent.Thing = cts = new CancellationTokenSource();
                searching = false;
                progressBar.Visible = false;
                searchButton.Text = "&Search";
            }
		}

        private void SearchError(Exception e) {
            searching = false;
            searchButton.Text = "&Search";
            progressBar.Visible = false;

            searchResults.Items.Clear();
            searchResults.Items.Add("Error: " + (e != null ? e.Message : "<Unknown>"));
            searchResults.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            searchResults.Enabled = false;
        }

        private void SetResults(List<QuizletAPI.SetInfo> results) {
            searching = false;
			searchButton.Text = "&Search";
			progressBar.Visible = false;

			searchResults.Enabled = true;
			searchResults.BeginUpdate();
			searchResults.Items.Clear();
            foreach (QuizletAPI.SetInfo set in results) {
				ListViewItem lvitem = new ListViewItem(new string[] { 
                    set.Title, 
                    set.Author, 
                    set.Created.ToString("d", CultureInfo.CurrentUICulture), 
                    set.TermCount.ToString(CultureInfo.CurrentUICulture), 
                    set.ID.ToString(CultureInfo.CurrentUICulture) });
				lvitem.Tag = set.ID;
				searchResults.Items.Add(lvitem);
			}
			searchResults.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			searchResults.EndUpdate();

			UpdateImportButton();
		}

		/// <summary>
		/// Decide on whether the import button should be enabled, based on the current input's validity
		/// </summary>
		private void UpdateImportButton() {
			if (tabControl.SelectedTab == searchTab) {
				importButton.Enabled = searchResults.Enabled && searchResults.SelectedItems.Count > 0;
				if (importButton.Enabled)
					foreach (ListViewItem item in searchResults.SelectedItems)
						importButton.Enabled = item.Tag is long;
			} else if (tabControl.SelectedTab == manualTab) {
				int parseTest;
				Match match = Regex.Match(manualInput.Text, "(\\d+)", RegexOptions.CultureInvariant);
				if (match.Success) {
					if (int.TryParse(match.Captures[0].Value, out parseTest)) {
						importButton.Enabled = true;
						errorProvider.SetError(manualInput, null);
					} else {
						errorProvider.SetError(manualInput, string.Format(CultureInfo.CurrentUICulture, Resources.Quizlet.NotAValidSetID, match.Captures[0].Value));
						importButton.Enabled = true;
					}
				} else {
					importButton.Enabled = false;
					errorProvider.SetError(manualInput, string.Format(CultureInfo.CurrentUICulture, Resources.Quizlet.NotAValidSetIDOrUrl, manualInput.Text));
				}
			}
		}

		#region Events
		private void importButton_Click(object sender, EventArgs e) {
			if (tabControl.SelectedTab == searchTab) {
				if (searchResults.Enabled)
					foreach (ListViewItem item in searchResults.SelectedItems)
						selectedSet = (long)item.Tag;
			} else if (tabControl.SelectedTab == manualTab) {
				//It should be impossible for this to throw an exception.
				Match match = Regex.Match(manualInput.Text, "\\d+");
				selectedSet = int.Parse(match.Captures[0].Value);
			}

			OnFinished();
		}

        private void searchResults_ItemActivate(object sender, EventArgs e) {
            if (searchResults.SelectedIndices.Count == 0)
                return;

            importButton_Click(sender, e);
        }

		private void searchButton_Click(object sender, EventArgs e) {
            if (searching)
				AbortRequest();
			else
    			StartSearch();
		}

        private void searchBox_Search(object sender, EventArgs e) {
            if (searching)
                AbortRequest();
            StartSearch();
        }

		void searchResults_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateImportButton();
		}

		private void manualInput_TextChanged(object sender, EventArgs e) {
			UpdateImportButton();
		}

		private void manualInput_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == '\r') {
				e.Handled = true;
				importButton_Click(null, null);
			}
		}

		//Decide whether or not the import button should be allowed.
		private void tabControl_TabIndexChanged(object sender, EventArgs e) {
			UpdateImportButton();
		}
		#endregion Events

        private void attribution_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("http://quizlet.com/");
        }
	}
}
