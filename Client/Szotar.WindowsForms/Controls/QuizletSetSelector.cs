using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Szotar.Quizlet;

namespace Szotar.WindowsForms.Controls {
	using System.Globalization;
	using System.Text.RegularExpressions;
	using System.Threading;

	[ImporterUI(typeof(QuizletImporter))]
	public partial class QuizletSetSelector : UserControl, IImporterUI<WordList> {
		long? selectedSet;
	    readonly QuizletImporter importer;
		CancellationTokenSource cts;
	    readonly DisposableComponent disposableComponent;
		bool searching;

		public IImporter<WordList> Importer {
			get { return importer; }
		}

		public QuizletSetSelector() {
			InitializeComponent();

			importer = new QuizletImporter();

			if (components == null)
				components = new System.ComponentModel.Container();

			cts = new CancellationTokenSource();
			components.Add(disposableComponent = new DisposableComponent(cts));

			searching = false;
			importButton.Enabled = false;
			searchResults.SelectedIndexChanged += SearchResultsSelectedIndexChanged;
		}

		public event EventHandler Finished;
		private void OnFinished() {
			EventHandler h = Finished;
			if (h != null)
				h(this, new EventArgs());
		}

		public void Apply() {
			if (selectedSet != null)
				importer.Set = selectedSet.Value;
		}

		private void StartSearch() {
			AbortRequest();

            new QuizletApi().SearchSets(searchBox.Text.Trim(), cts.Token).ContinueWith(t => {
                if (t.Exception != null)
                    SearchError(t.Exception);
                else if (t.IsCanceled)
                    SearchError(new OperationCanceledException());
                else 
                    SetResults(t.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());

			searching = true;
			progressBar.Visible = true;
			searchButton.Text = Properties.Resources.Abort;
		}

		// Aborts the current web request, if any.
		private void AbortRequest() {
		    if (!searching)
		        return;

		    cts.Cancel();
		    cts.Dispose();
		    disposableComponent.Thing = cts = new CancellationTokenSource();
		    searching = false;
		    progressBar.Visible = false;
		    searchButton.Text = Properties.Resources.Search;
		}

		private void SearchError(Exception e) {
            var ae = e as AggregateException;
            if (ae != null && ae.InnerExceptions.Count == 1)
                e = ae.InnerExceptions[0];

			searching = false;
			searchButton.Text = Properties.Resources.Search;
			progressBar.Visible = false;

			searchResults.Items.Clear();
			searchResults.Items.Add("Error: " + (e != null ? e.Message : "<Unknown>"));
			searchResults.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			searchResults.Enabled = false;
		}

		private void SetResults(IEnumerable<SetModel> results) {
			searching = false;
			searchButton.Text = Properties.Resources.Search;
			progressBar.Visible = false;

			searchResults.Enabled = true;
			searchResults.BeginUpdate();
			searchResults.Items.Clear();
			foreach (var set in results) {
				var item = new ListViewItem(new[] { 
					set.Title, 
					set.Author, 
					set.Created.ToString("d", CultureInfo.CurrentUICulture), 
					set.TermCount.ToString(CultureInfo.CurrentUICulture), 
					set.SetID.ToString(CultureInfo.CurrentUICulture) }) {Tag = set.SetID};
			    searchResults.Items.Add(item);
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
			    var match = Regex.Match(manualInput.Text, "(\\d+)", RegexOptions.CultureInvariant);
				if (match.Success) {
				    int parseTest;
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
		private void ImportButtonClick(object sender, EventArgs e) {
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

		private void SearchResultsItemActivate(object sender, EventArgs e) {
			if (searchResults.SelectedIndices.Count == 0)
				return;

			ImportButtonClick(sender, e);
		}

		private void SearchButtonClick(object sender, EventArgs e) {
			if (searching)
				AbortRequest();
			else
				StartSearch();
		}

		private void SearchBoxSearch(object sender, EventArgs e) {
			if (searching)
				AbortRequest();
			StartSearch();
		}

		void SearchResultsSelectedIndexChanged(object sender, EventArgs e) {
			UpdateImportButton();
		}

		private void ManualInputTextChanged(object sender, EventArgs e) {
			UpdateImportButton();
		}

		private void ManualInputKeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == (char)Keys.Return) {
				e.Handled = true;
				ImportButtonClick(null, null);
			}
		}

		//Decide whether or not the import button should be allowed.
		private void TabControlTabIndexChanged(object sender, EventArgs e) {
			UpdateImportButton();
		}
		#endregion Events

		private void AttributionLinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			Process.Start("http://quizlet.com/");
		}
	}
}
