using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Szotar.WindowsForms.Importing;

namespace Szotar.WindowsForms.Controls {
    public partial class DictionarySectionUI : UserControl, IImporterUI<IBilingualDictionary> {
        IDictionarySectionImporter importer;
        DualSectionImporter dualSection;

        public DictionarySectionUI(DualSectionImporter dualSection, IDictionarySectionImporter importer) {
            InitializeComponent();

            this.dualSection = dualSection;
            this.importer = importer;
        }

        private void browse_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files (*.*)|*.*";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK) {
                fileName.Text = ofd.FileName;
            }
        }

        private void fileSelectNext_Click(object sender, EventArgs e) {
            OnFinished();
        }

        #region IImporterUI
        public void Apply() {
            dualSection.SetImporters(importer, fileName.Text, null, string.Empty, generateSecondHalf.Checked);
        }

        public event EventHandler Finished;
        private void OnFinished() {
            EventHandler temp = Finished;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public IImporter<IBilingualDictionary> Importer {
            get { return dualSection; }
        }
        #endregion

        private void fileName_TextChanged(object sender, EventArgs e) {
            fileSelectNext.Enabled = fileName.TextLength > 0;
        }
    }
}
