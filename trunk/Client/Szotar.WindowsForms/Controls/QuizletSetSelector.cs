using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
	using System.Globalization;
	using System.IO;
	using System.Text.RegularExpressions;
	using Szotar.WindowsForms.Importing;
	using Szotar.WindowsForms.Importing.WordListImporting;

	public partial class QuizletSetSelector : UserControl, IImporterUI<WordList> {
		IAsyncResult result;
		WebRequest currentRequest;
		private Nullable<int> selectedSet;
		QuizletImporter importer;
		Exception exception;

		public IImporter<WordList> Importer {
			get { return importer; }
		}

		public QuizletSetSelector(IImporter<WordList> importer) {
			if (!(importer is QuizletImporter))
				throw new ArgumentException("QuizletSetSelector cannot Apply to a " + importer == null ? "NullReference" : importer.GetType().Name);

			this.importer = (QuizletImporter)importer;
			InitializeComponent();

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
			Uri uri = new Uri("http://quizlet.com/search/" + Uri.EscapeUriString(search) + "/");
			currentRequest = WebRequest.Create(uri);

			progressBar.Visible = true;
			searchButton.Text = "&Abort";
			result = currentRequest.BeginGetResponse(new AsyncCallback(this.SearchReturned), null);
		}

		// Aborts the current web request, if any.
		private void AbortRequest() {
			if (currentRequest != null) {
				currentRequest.Abort();
				currentRequest = null;
				progressBar.Visible = false;
				searchButton.Text = "&Search";
			}
		}

		private void SearchReturned(IAsyncResult result) {
			// Sometimes this actually asserts, presumably because SearchReturned executes before result
			// is assigned in StartSearch.
			// System.Diagnostics.Debug.Assert(result == this.result);

			try {
				WebResponse response = currentRequest.EndGetResponse(result);
				currentRequest = null;

				using (Stream stream = response.GetResponseStream()) {
					string text;
					using (TextReader reader = new StreamReader(stream, Encoding.UTF8))
						text = reader.ReadToEnd();
					try {
						QuizletSearchResults results = new QuizletSearchResults(text);
						result = null;
						Invoke(new SetResultsDelegate(this.SetResults), new object[] { results });
					} catch (InvalidDataException ide) {
						exception = ide;
						result = null;
						Invoke(new SetResultsDelegate(this.SetResults), new object[] { null });
					}
				}
			} catch (WebException we) {
				exception = we;
				result = null;
				Invoke(new SetResultsDelegate(this.SetResults), new object[] { null });
			}
		}

		private delegate void SetResultsDelegate(IEnumerable<QuizletSetInfo> results);
		private void SetResults(IEnumerable<QuizletSetInfo> results) {
			result = null;

			searchButton.Text = "&Search";
			progressBar.Visible = false;
			if (results == null) {
				searchResults.Items.Clear();
				searchResults.Items.Add("Error: " + (exception != null ? exception.Message : "<Unknown>"));
				searchResults.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
				searchResults.Enabled = false;
				return;
			}

			searchResults.Enabled = true;
			searchResults.BeginUpdate();
			searchResults.Items.Clear();
			foreach (QuizletSetInfo set in results) {
				ListViewItem lvitem = new ListViewItem(new string[] { set.Name, set.Creator, set.Created, set.Terms.ToString(CultureInfo.CurrentUICulture), set.ID.ToString(CultureInfo.CurrentUICulture) });
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
						importButton.Enabled = item.Tag is int;
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
						selectedSet = (int)item.Tag;
			} else if (tabControl.SelectedTab == manualTab) {
				//It should be impossible for this to throw an exception.
				Match match = Regex.Match(manualInput.Text, "\\d+");
				selectedSet = int.Parse(match.Captures[0].Value);
			}

			OnFinished();
		}

		private void searchButton_Click(object sender, EventArgs e) {
			if (result != null) {
				AbortRequest();
				return;
			}
			StartSearch();
		}

		private void searchBox_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == '\r') {
				e.Handled = true;
				StartSearch();
			}
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
	}

	[Serializable]
	class QuizletSetInfo {
		private string creator;
		private string name;
		private int numberOfTerms;
		private int setID;
		private string created;

		public string Name { get { return name; } }
		public string Creator { get { return creator; } }
		public int ID { get { return setID; } }
		public int Terms { get { return numberOfTerms; } }
		public string Created { get { return created; } }

		public QuizletSetInfo(int setID, string name, string creator, int numberOfTerms, string created) {
			this.name = name;
			this.creator = creator;
			this.numberOfTerms = numberOfTerms;
			this.setID = setID;
			this.created = created;
		}
	}

	class QuizletSearchResults : IEnumerable<QuizletSetInfo> {
		private ICollection<QuizletSetInfo> sets = new List<QuizletSetInfo>();

		public QuizletSearchResults(string content) {
			if (content.IndexOf("<p>Nothing on Quizlet matched your search") > -1)
				return;

			// Probably also not needed.
			int i = content.IndexOf("<div id='sets'>");
			if (i == -1)
				throw new InvalidDataException("Could not parse Quizlet data.");
			content = content.Substring(i);

			// This regex... it is evil.
			// (1) set ID
			// (2) set name
			// (3) creator
			// (4) amount of terms
			// (5) date created
			Regex regex = new Regex(
				"<tr.*?>\\s*<td.*?>.*?</td>\\s*<td.*?><strong.*?><a\\s+href=\"/set/(\\d+)/\">(.*?)</a></strong>" +
				"<small.*?>.*?<a.*?>(.*?)</a></small></td>\\s*" +
				"<td.*?>(\\d+)[^<]+<td.*?><span.*?>([^<]*)</span></td></tr>"
				, RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);

			Match match;
			while ((match = regex.Match(content)).Success) {
				sets.Add(new QuizletSetInfo(int.Parse(match.Groups[1].Value), match.Groups[2].Value, match.Groups[5].Value, int.Parse(match.Groups[4].Value), match.Groups[3].Value));
				content = content.Substring(match.Length + match.Index);
			}
		}

		IEnumerator<QuizletSetInfo> IEnumerable<QuizletSetInfo>.GetEnumerator() {
			return sets.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return sets.GetEnumerator();
		}
	}
}
