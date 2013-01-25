using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class ImportForm : Form, INotifyProgress {
		private IImporter<WordList> importer;
		private WordList importedWordList;

		public ImportForm() {
			InitializeComponent();

			importerSelection.SelectedIndexChanged += ImporterSelectionChanged;
			Closed += ImportFormClosed;

			importerSelection.BeginUpdate();
			Assembly assembly = Assembly.GetExecutingAssembly();

			foreach (Type type in assembly.GetTypes()) {
				Attribute[] attributes = Attribute.GetCustomAttributes(type);
				foreach (var attribute in attributes.OfType<ImporterUIAttribute>()) {
					if(!attribute.ImporterType.GetInterfaces().Contains(typeof(IImporter<WordList>)))
						continue;

					var importerAttribute = (ImporterAttribute)Attribute.GetCustomAttribute(attribute.ImporterType, typeof(ImporterAttribute));
					if (importerAttribute == null)
						continue;

					var descAttribute = (ImporterDescriptionAttribute)Attribute.GetCustomAttribute(attribute.ImporterType, typeof(ImporterDescriptionAttribute));
                    
                    string name = importerAttribute.Name;
                    string description = type.Name;

					if (descAttribute != null)
						description = Resources.ImporterDescriptions.ResourceManager.GetString(
							descAttribute.ResourceIdentifier, CultureInfo.CurrentUICulture)
							?? descAttribute.Description ?? description;

					if (name != null)
						importerSelection.Items.Add(new ImporterItem(type, name, description));
				}
			}
			importerSelection.EndUpdate();

			if (importerSelection.Items.Count > 0) {
				importerSelection.SelectedIndex = 0;
			    if (CurrentUI != null)
                    CurrentUI.Focus();
			}
		}

		void ImportFormClosed(object sender, EventArgs e) {
			if (importer != null)
				importer.Dispose();
		}

		#region Completion
		void ImporterCompleted(object sender, ImportCompletedEventArgs<WordList> e) {
			if (InvokeRequired) {
				Invoke(new EventHandler<ImportCompletedEventArgs<WordList>>(ImporterCompleted), sender, e);
			} else {
				if (e.Cancelled || e.Error != null)
					ImportFailed(e.Error);
				else
					ImportCompleted(e.ImportedObject);
			}
		}

		private void ImportFailed(Exception exception) {
			string message = exception != null ? exception.Message : Resources.Errors.TheOperationWasCancelled;
			CurrentUI = new Controls.ErrorUI(
				string.Format(CultureInfo.CurrentUICulture, Resources.Errors.ImportFailedWithMessage, message, "\n\n"),
				exception != null ? exception.StackTrace : "");
		}

	    private void ImportCompleted(WordList result) {
			Debug.Assert(!InvokeRequired, "ImportCompleted called on a secondary thread");
			importedWordList = result;
			CurrentUI = null;
			if(importedWordList.ID.HasValue)
				ListBuilder.Open(importedWordList.ID.Value);
			Close();
		}
		#endregion

		private void ImporterSelectionChanged(object sender, EventArgs e) {
			var item = (importerSelection.SelectedItem as ImporterItem);
		    if (item == null)
		        return;
		    
            if (importer != null) {
		        UnwireImporterEvents();
		        importer.Dispose();
		    }

		    var newUI = (IImporterUI<WordList>)Activator.CreateInstance(item.Type);
		    importer = newUI.Importer;
		    WireImporterEvents();

		    newUI.Finished += ImporterUIFinished;
		    CurrentUI = (Control)newUI;
		}
		   
		private Control CurrentUI {
			get {
			    return content.HasChildren ? content.Controls[0] : null;
			}
		    set {
				if (content.Controls.Count > 0) {
					if (content.Controls[0] is IImporterUI<WordList>)
						((IImporterUI<WordList>)content.Controls[0]).Finished -= ImporterUIFinished;
					else if (content.Controls[0] is Controls.ProgressUI)
						((Controls.ProgressUI)content.Controls[0]).Cancelled -= ProgressUICancelled;

				    var d = content.Controls[0] as IDisposable;
				    if (d != null)
				        d.Dispose();
				}

				content.Controls.Clear();
				if (value is Controls.ProgressUI) {
					value.Dock = DockStyle.Fill;
					((Controls.ProgressUI)value).Cancelled += ProgressUICancelled;
					content.Controls.Add(value);
				} else if (value != null) {
					value.Dock = DockStyle.Fill;
					content.Controls.Add(value);
				}
			}
		}

		void ProgressUICancelled(object sender, EventArgs e) {
			if (importer == null)
				return;

			importer.Cancel();
			ImporterSelectionChanged(importerSelection, EventArgs.Empty);
		}

		private void ImporterUIFinished(object sender, EventArgs e) {
			((IImporterUI<WordList>)sender).Finished -= ImporterUIFinished;

			if (importerSelection.SelectedItem is ImporterItem) {
			    var importerUI = CurrentUI as IImporterUI<WordList>;
			    if (importerUI != null)
			        importerUI.Apply();
				CurrentUI = null;

				var progressUI = new Controls.ProgressUI();
				CurrentUI = progressUI;

				// Try is needed here too in case the import fails synchronously.
				try {
					importer.BeginImport();
				} catch (ImportException ex) {
					ImportFailed(ex);
				}
			}
		}

		private void WireImporterEvents() {
			importer.Completed += ImporterCompleted;
			importer.ProgressChanged += ImporterProgressChanged;
		}

		private void UnwireImporterEvents() {
			importer.Completed -= ImporterCompleted;
			importer.ProgressChanged -= ImporterProgressChanged;
		}

		#region Progress Report
		delegate void SetProgressMessageDelegate(string message, int? percent);

		public void SetProgressMessage(string message, int? percent) {
			if (InvokeRequired) {
				Invoke(new SetProgressMessageDelegate(SetProgressMessage), new object[] { message, percent });
			} else {
				var progress = CurrentUI as Controls.ProgressUI;
				if (progress != null) {
					progress.Message = message;
					progress.Percent = percent;
				}
			}
		}

		void ImporterProgressChanged(object sender, ProgressMessageEventArgs e) {
			SetProgressMessage(e.Message, e.Percentage);
		}
		#endregion
	}
}