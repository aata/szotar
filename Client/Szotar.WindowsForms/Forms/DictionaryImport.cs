using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using Szotar.WindowsForms.Importing;

namespace Szotar.WindowsForms.Forms {
	public partial class DictionaryImport : Form, INotifyProgress {
		private IImporter<IBilingualDictionary> importer;
		private IBilingualDictionary imported;

		public DictionaryImport() {
			InitializeComponent();

			select.SelectedIndexChanged += new EventHandler(select_SelectedIndexChanged);
			this.Closed += new EventHandler(DictionaryImport_Closed);

			select.BeginUpdate();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type type in assembly.GetTypes()) {
                    string name = null;
                    string description = type.Name;

                    Attribute[] attributes = Attribute.GetCustomAttributes(type);
                    foreach (Attribute attribute in attributes) {
                        if (attribute is ImporterAttribute && ((ImporterAttribute)attribute).Type == typeof(IBilingualDictionary))
                            name = ((ImporterAttribute)attribute).Name;
                        else if (attribute is ImporterAttribute && ((ImporterAttribute)attribute).Type == typeof(IDictionarySection))
                            name = ((ImporterAttribute)attribute).Name;
                        else if (attribute is ImporterDescriptionAttribute)
                            description = Resources.ImporterDescriptions.ResourceManager.GetString(
                                ((ImporterDescriptionAttribute)attribute).ResourceIdentifier, CultureInfo.CurrentUICulture) ?? description;
                    }

                    if (name != null) {
                        ImporterItem item = new ImporterItem(type, name, description);

                        // Not to be vain, or anything, but I think this should be the default choice of importer.
                        if (type == typeof(Importing.DualSectionImporter)) {
                            select.Items.Insert(0, item);
                        } else {
                            select.Items.Add(item);
                        }
                    }
                }
            }

			select.EndUpdate();

			if (select.Items.Count > 0)
				select.SelectedIndex = 0;
		}

		void select_SelectedIndexChanged(object sender, EventArgs e) {
			ImporterItem item = (select.SelectedItem as ImporterItem);
			if (item != null) {
				if (importer != null) {
					UnwireImporterEvents();
					importer.Dispose();
				}

                IImporterUI<IBilingualDictionary> newUI = null;

                if (item.Type.GetInterfaces().Contains(typeof(IDictionarySectionImporter))) {
                    var si = (IDictionarySectionImporter)item.Type.GetConstructor(new Type[] { }).Invoke(new object[] { });
                    var dsi = new DualSectionImporter();
                    newUI = new Controls.DictionarySectionUI(dsi, si);
                    importer = dsi;
                } else if(item.Type.IsSubclassOf(typeof(IBilingualDictionary))) {
                    importer = (IImporter<IBilingualDictionary>)item.Type.GetConstructor(new Type[] { }).Invoke(new object[] { });

                    foreach (Type t in importer.GetType().Assembly.GetTypes()) {
                        var attr = Attribute.GetCustomAttribute(t, typeof(ImporterUIAttribute)) as ImporterUIAttribute;
                        if (attr != null) {
                            newUI = (IImporterUI<IBilingualDictionary>)Activator.CreateInstance(t, importer);
                            break;
                        }
                    }
				}

                WireImporterEvents();
                
                if (newUI == null) {
                    CurrentUI = new Controls.ErrorUI("No UI for " + item.Name, "");
                    UnwireImporterEvents();
                    importer.Dispose();
                    importer = null;
                    return;
                }

				newUI.Finished += new EventHandler(this.ImporterUIFinished);
				CurrentUI = (Control)newUI;
			}
		}

		void DictionaryImport_Closed(object sender, EventArgs e) {
			if (importer != null)
				importer.Dispose();
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
					if (content.Controls[0] is IImporterUI<IBilingualDictionary>)
						((IImporterUI<IBilingualDictionary>)content.Controls[0]).Finished -= new EventHandler(this.ImporterUIFinished);
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

			try {
				importer.Cancel();
				select_SelectedIndexChanged(select, EventArgs.Empty);
			} catch (NotSupportedException) {
				((Controls.ProgressUI)sender).CancelFailed();
			}
		}

		#region Events
		private void WireImporterEvents() {
			importer.Completed += new EventHandler<ImportCompletedEventArgs<IBilingualDictionary>>(importer_Completed);
			importer.ProgressChanged += new EventHandler<ProgressMessageEventArgs>(importer_ProgressChanged);
		}

		private void UnwireImporterEvents() {
			importer.Completed -= new EventHandler<ImportCompletedEventArgs<IBilingualDictionary>>(importer_Completed);
			importer.ProgressChanged -= new EventHandler<ProgressMessageEventArgs>(importer_ProgressChanged);
		}
		#endregion

		#region Completion
		void importer_Completed(object sender, ImportCompletedEventArgs<IBilingualDictionary> e) {
			if (InvokeRequired) {
				Invoke(new EventHandler<ImportCompletedEventArgs<IBilingualDictionary>>(this.importer_Completed), sender, e);
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

		delegate void ImportCompletedDelegate(IBilingualDictionary result);
		private void ImportCompleted(IBilingualDictionary result) {
			Debug.Assert(!InvokeRequired, "ImportCompleted called on a secondary thread");

			try {
				if (CurrentUI is Controls.ProgressUI) {
					Controls.ProgressUI prog = (Controls.ProgressUI)CurrentUI;
					prog.Percent = 100;
                    prog.Message = Properties.Resources.ImportFinished;
				}

				this.imported = result;

				// Modal dialog simplifies things slightly.
				new DictionaryInfoEditor(imported, false).ShowDialog();

				string root = DataStore.UserDataStore.Path;
				string name = imported.Name ?? Properties.Resources.DefaultDictionaryName;

				DataStore.UserDataStore.EnsureDirectoryExists(Configuration.DictionariesFolderName);

				// Attempt to save with a sane name; failing that, use a GUID as the name; otherwise report the error.
				var ipc = Path.GetInvalidPathChars();
				if (name.IndexOfAny(ipc) >= 0) {
					// TODO: Sanitize file name!
				}

				string newPath = Path.Combine(Path.Combine(root, Configuration.DictionariesFolderName), name) + ".dict";

				// We don't want to overwrite an existing dictionary.
                if (File.Exists(newPath)) {
                    name = Guid.NewGuid().ToString("D");
                    newPath = Path.Combine(Path.Combine(root, Configuration.DictionariesFolderName), name) + ".dict";
                }

                imported.Path = newPath;

				// If the dictionary can't save, we should delete the half-written file.
				// TODO: this should probably avoid deleting the file if the error was caused
				// by the file already existing (say, if it was created between calls). It would be
				// kind of rare though.
				try {
					imported.Save();
				} catch (SystemException) { // XXX Lazy.
					File.Delete(imported.Path);
					throw;
				}

				new LookupForm(imported).Show();
				Close();
			} catch (ApplicationException ex) {
				ReportException(ex);
			} catch (IOException ex) {
				ReportException(ex);
			} catch (InvalidOperationException ex) {
				ReportException(ex);
			} catch (ArgumentException ex) {
				ReportException(ex);
			}
		}

		void ReportException(Exception ex) {
			CurrentUI = new Controls.ErrorUI(ex.Message, ex.StackTrace);
		}

		void ImporterUIFinished(object sender, EventArgs e) {
			((IImporterUI<IBilingualDictionary>)sender).Finished -= new EventHandler(this.ImporterUIFinished);

			if (select.SelectedItem != null && select.SelectedItem is ImporterItem) {
				if (CurrentUI is IImporterUI<IBilingualDictionary>)
					((IImporterUI<IBilingualDictionary>)CurrentUI).Apply();
				CurrentUI = null;

				Controls.ProgressUI progressUI = new Controls.ProgressUI();
				CurrentUI = progressUI;

				importer.BeginImport();
			}
		}
		#endregion

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
}
