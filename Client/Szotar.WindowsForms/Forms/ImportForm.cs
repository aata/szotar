using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Szotar.WindowsForms.Importing;

namespace Szotar.WindowsForms.Forms {
	public partial class ImportForm : Form, INotifyProgress {
		private IImporter<WordList> importer;
		private WordList importedWordList = null;

		public ImportForm() {
			InitializeComponent();

			importerSelection.SelectedIndexChanged += new EventHandler(importerSelection_SelectedIndexChanged);
			this.Closed += new EventHandler(ImportForm_Closed);

			importerSelection.BeginUpdate();
			Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			foreach (Type type in assembly.GetTypes()) {
				string name = null;
				string description = type.Name;

				Attribute[] attributes = Attribute.GetCustomAttributes(type);
				foreach (Attribute attribute in attributes) {
					if (attribute is ImporterAttribute && ((ImporterAttribute)attribute).Type == typeof(WordList))
						name = ((ImporterAttribute)attribute).Name;
					else if (attribute is ImporterDescriptionAttribute)
						description = ((ImporterDescriptionAttribute)attribute).GetLocalizedDescription(type, System.Globalization.CultureInfo.CurrentUICulture);
				}

				if (name != null) {
					importerSelection.Items.Add(new ImporterItem(type, name, description));
				}
			}
			importerSelection.EndUpdate();

			if (importerSelection.Items.Count > 0)
				importerSelection.SelectedIndex = 0;

		}

		void ImportForm_Closed(object sender, EventArgs e) {
			if (importer != null)
				importer.Dispose();
		}

		#region Completion
		void importer_Completed(object sender, ImportCompletedEventArgs<WordList> e) {
			if (InvokeRequired) {
				Invoke(new EventHandler<ImportCompletedEventArgs<WordList>>(this.importer_Completed), sender, e);
			} else {
				if (e.Cancelled || e.Error != null)
					ImportFailed(e.Error);
				else
					ImportCompleted(e.ImportedObject);
			}
		}

		//To do: show some sort of non-modal error UI.
		private void ImportFailed(Exception exception) {
			string message = exception != null ? exception.Message : Resources.Errors.TheOperationWasCancelled;
			CurrentUI = new Controls.ErrorUI(
				string.Format(CultureInfo.CurrentUICulture, Resources.Errors.ImportFailedWithMessage, message, "\n\n"),
				exception != null ? exception.StackTrace : "");
		}

		delegate void ImportCompletedDelegate(WordList result);
		private void ImportCompleted(WordList result) {
			System.Diagnostics.Debug.Assert(!InvokeRequired, "ImportCompleted called on a secondary thread");
			this.importedWordList = result;
			CurrentUI = null;
			new ListBuilder(importedWordList).Show();
			Close();
		}
		#endregion

		private void importerSelection_SelectedIndexChanged(object sender, EventArgs e) {
			ImporterItem item = (importerSelection.SelectedItem as ImporterItem);
			if (item != null) {
				if (importer != null) {
					UnwireImporterEvents();
					importer.Dispose();
				}

				importer = (IImporter<WordList>)item.Type.GetConstructor(new Type[] { }).Invoke(new object[] { });
				WireImporterEvents();

				IImporterUI<WordList> newUI = importer.CreateUI();
				newUI.Finished += new EventHandler(this.ImporterUIFinished);
				CurrentUI = (Control)newUI;
			}
		}

		private Control CurrentUI {
			get {
				if (content.HasChildren)
					return content.Controls[0];
				else
					return null;
			}
			set {
				if (content.Controls.Count > 0) {
					if (content.Controls[0] is IImporterUI<WordList>)
						((IImporterUI<WordList>)content.Controls[0]).Finished -= new EventHandler(this.ImporterUIFinished);
					else if (content.Controls[0] is Controls.ProgressUI)
						((Controls.ProgressUI)content.Controls[0]).Cancelled -= new EventHandler(progressUI_Cancelled);

					if (content.Controls[0] is IDisposable)
						((IDisposable)content.Controls[0]).Dispose();
				}

				content.Controls.Clear();
				if (value is Controls.ProgressUI) {
					value.Dock = DockStyle.Fill;
					((Controls.ProgressUI)value).Cancelled += new EventHandler(progressUI_Cancelled);
					content.Controls.Add(value);
				} else if (value != null) {
					value.Dock = DockStyle.Fill;
					content.Controls.Add(value);
				}
			}
		}

		void progressUI_Cancelled(object sender, EventArgs e) {
			if (importer == null)
				return;

			importer.Cancel();
			importerSelection_SelectedIndexChanged(importerSelection, EventArgs.Empty);
		}

		private void ImporterUIFinished(object sender, EventArgs e) {
			((IImporterUI<WordList>)sender).Finished -= new EventHandler(this.ImporterUIFinished);

			if (importerSelection.SelectedItem != null && importerSelection.SelectedItem is ImporterItem) {
				if (CurrentUI is IImporterUI<WordList>) {
					((IImporterUI<WordList>)CurrentUI).Apply();
				}
				CurrentUI = null;

				Controls.ProgressUI progressUI = new Controls.ProgressUI();
				CurrentUI = progressUI;

				//Try is needed here too in case the import fails synchronously
				try {
					importer.BeginImport();
				} catch (ImportException ex) {
					CurrentUI = new Szotar.WindowsForms.Controls.ErrorUI(ex.Message, ex.StackTrace);
				}
			}
		}

		private void WireImporterEvents() {
			importer.Completed += new EventHandler<ImportCompletedEventArgs<WordList>>(importer_Completed);
			importer.ProgressChanged += new EventHandler<ProgressMessageEventArgs>(importer_ProgressChanged);
		}

		private void UnwireImporterEvents() {
			importer.Completed -= new EventHandler<ImportCompletedEventArgs<WordList>>(importer_Completed);
			importer.ProgressChanged -= new EventHandler<ProgressMessageEventArgs>(importer_ProgressChanged);
		}

		#region Progress Report
		delegate void SetProgressMessageDelegate(string message, int? percent);

		public void SetProgressMessage(string message, int? percent) {
			if (InvokeRequired) {
				Invoke(new SetProgressMessageDelegate(this.SetProgressMessage), new object[] { message, percent });
			} else {
				Controls.ProgressUI progress = CurrentUI as Controls.ProgressUI;
				if (progress != null) {
					progress.Message = message;
					progress.Percent = percent;
				}
			}
		}

		void importer_ProgressChanged(object sender, ProgressMessageEventArgs e) {
			SetProgressMessage(e.Message, e.Percentage);
		}
		#endregion
	}

	public class ImporterItem {
		private string name, description;
		private Type type;

		public ImporterItem(Type type, string name, string description) {
			this.type = type;
			this.name = name;
			this.description = description;
		}

		public Type Type { get { return type; } }
		public string Name { get { return name; } }
		public string Description { get { return description; } }

		public override string ToString() {
			return name + " - " + description;
		}
	}
}