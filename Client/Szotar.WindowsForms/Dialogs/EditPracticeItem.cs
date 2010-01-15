using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Dialogs {
	public partial class EditPracticeItem : Form {
		PracticeItem item;

		public EditPracticeItem(PracticeItem item) {
			InitializeComponent();

			this.item = item;
			phrase.Text = item.Phrase;
			translation.Text = item.Translation;
		}

		public string Phrase {
			get {
				return phrase.Text;
			}
		}

		public string Translation {
			get {
				return translation.Text;
			}
		}
	}
}
