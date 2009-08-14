using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Reflection;

using System.Windows.Forms;
using Szotar.WindowsForms.Importing.DictionaryImporting;
using Szotar.WindowsForms.Importing;

namespace Szotar.WindowsForms.Controls {
	public partial class DualImporterUI : UserControl, IImporterUI<IBilingualDictionary> {
		Control currentUI;
		DualSectionImporter importer;

		IDictionarySectionImporter firstImporter, secondImporter;
		string firstPath, secondPath;

		private void PopulateImporterTypes() {
			importerType.BeginUpdate();
			Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			foreach (Type type in assembly.GetTypes()) {
				string name = null;
				string description = type.Name;

				Attribute[] attributes = Attribute.GetCustomAttributes(type);
				foreach (Attribute attribute in attributes) {
					if (attribute is ImporterAttribute && ((ImporterAttribute)attribute).Type == typeof(IDictionarySection))
						name = ((ImporterAttribute)attribute).Name;
					else if (attribute is ImporterDescriptionAttribute)
						description = ((ImporterDescriptionAttribute)attribute).GetLocalizedDescription(type, System.Globalization.CultureInfo.CurrentUICulture);
				}

				if (name != null) {
					importerType.Items.Add(new Szotar.WindowsForms.Forms.ImporterItem(type, name, description));
				}
			}
			importerType.EndUpdate();

			if (importerType.Items.Count > 0)
				importerType.SelectedIndex = 0;
		}

		public DualImporterUI(DualSectionImporter importer) {
			InitializeComponent();

			this.importer = importer;
			CurrentUI = methodSelect;

			PopulateImporterTypes();

			importTypeDual.CheckedChanged += new EventHandler(ImportTypeChanged);
			importTypeGenerate.CheckedChanged += new EventHandler(ImportTypeChanged);
			importTypeBlank.CheckedChanged += new EventHandler(ImportTypeChanged);

			importTypeDual.MouseDoubleClick += new MouseEventHandler(ImportTypeDoubleClick);
			importTypeGenerate.MouseDoubleClick += new MouseEventHandler(ImportTypeDoubleClick);
			importTypeBlank.MouseDoubleClick += new MouseEventHandler(ImportTypeDoubleClick);

			methodSelectNext.Click += new EventHandler(methodSelectNext_Click);
			fileSelectNext.Click += new EventHandler(fileSelectNext_Click);

			browse.Click += new EventHandler(browse_Click);
			fileName.TextChanged += new EventHandler(fileName_TextChanged);
			importerType.SelectedIndexChanged += new EventHandler(importerType_SelectedIndexChanged);

			table.Resize += new EventHandler(table_Resize);
		}

		void table_Resize(object sender, EventArgs e) {
			helpText1.Width = helpText2.Width = helpText3.Width = table.ClientSize.Width;
		}

		void browse_Click(object sender, EventArgs e) {
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "All files (*.*)|*.*";
			ofd.Multiselect = false;

			if (ofd.ShowDialog() == DialogResult.OK) {
				fileName.Text = ofd.FileName;
			}
		}

		void UpdateFileSelectNext() {
			fileSelectNext.Enabled = fileName.TextLength > 0 && importerType.SelectedIndex != -1;
		}

		void importerType_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateFileSelectNext();
		}

		void fileName_TextChanged(object sender, EventArgs e) {
			UpdateFileSelectNext();
		}

		void fileSelectNext_Click(object sender, EventArgs e) {
			IDictionarySectionImporter importer = (IDictionarySectionImporter)Activator.CreateInstance(
				((Szotar.WindowsForms.Forms.ImporterItem)importerType.SelectedItem).Type);

			if (firstImporter == null) {
				firstImporter = importer;
				firstPath = fileName.Text;

				if (!importTypeDual.Checked) {
					CurrentUI = null;
					OnFinished();
				} else {
					fileName.Text = string.Empty;
					pleaseSelect.Text = Properties.Resources.DictionarySectionSelectSecondHalf;
					fileSelectNext.Text = Properties.Resources.BeginCommand;
					fileName.Focus();
				}
			} else {
				secondImporter = importer;
				secondPath = fileName.Text;

				CurrentUI = null;
				OnFinished();
			}
		}

		void ImportTypeDoubleClick(object sender, MouseEventArgs e) {
			methodSelectNext_Click(sender, e);
		}

		void methodSelectNext_Click(object sender, EventArgs e) {
			if (!importTypeDual.Checked)
				fileSelectNext.Text = Properties.Resources.BeginCommand;
			CurrentUI = fileSelect;
			fileName.Focus();
		}

		void ImportTypeChanged(object sender, EventArgs e) {
			methodSelectNext.Enabled = true;
		}

		public Control CurrentUI {
			get {
				return currentUI;
			}
			set {
				// Avoid changing anything where not necessary.
				// It may introduce visual artifacts.
				if (value == currentUI)
					return;

				// Set the dock before it becomes visible (it should be set anyway, however).
				if (value != null)
					value.Dock = DockStyle.Fill;
				foreach (Control panel in Controls)
					panel.Visible = panel == value;
				currentUI = value;
			}
		}

		#region IImporterUI
		public void Apply() {
			importer.SetImporters(firstImporter, firstPath, secondImporter, secondPath, importTypeGenerate.Checked);
		}

		public event EventHandler Finished;
		private void OnFinished() {
			EventHandler temp = Finished;
			if (temp != null)
				temp(this, EventArgs.Empty);
		}

		public IImporter<IBilingualDictionary> Importer {
			get { return importer; }
		}
		#endregion
	}
}
