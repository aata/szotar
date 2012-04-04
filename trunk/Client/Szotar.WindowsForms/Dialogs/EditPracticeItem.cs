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

        public static PracticeItem Show(PracticeItem item) {
            var dialog = new Dialogs.EditPracticeItem(item);
            if (dialog.ShowDialog() == DialogResult.OK) {
                var existed = DataStore.Database.UpdateWordListEntry(
                    item.SetID,
                    item.Phrase,
                    item.Translation,
                    dialog.Phrase,
                    dialog.Translation);

                // TODO: This should possibly be made into an actual message, since it would go against 
                // the user's expectations. However, explaining the reason why it didn't work would 
                // be equally confusing.
                // A third (and perhaps best) option would be to modify the item based on its unique ID 
                // (which word list entries do actually have, even though they are not currently 
                // retrieved by SqliteWordList.)
                if (!existed)
                    ProgramLog.Default.AddMessage(LogType.Debug, "Attempting to update word list entry that no longer exists: {0}, {1}", item.Phrase, item.Translation);

                return new PracticeItem(item.SetID, dialog.Phrase, dialog.Translation);
            }

            return null;
        }
	}
}
