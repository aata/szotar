using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class DictionaryInfoEditor : Form {
		IBilingualDictionary dict;
		bool saveOnClose;
		string initialTitle;

		public DictionaryInfoEditor(IBilingualDictionary dict, bool saveOnClose) {
			InitializeComponent();

			initialTitle = Text;			

			name.Text = dict.Name;
			author.Text = dict.Author;
			url.Text = dict.Url;
			firstLanguage.Text = dict.FirstLanguage;
			secondLanguage.Text = dict.SecondLanguage;

			this.dict = dict;
			this.saveOnClose = saveOnClose;

			save.Click += new EventHandler(save_Click);
			cancel.Click += delegate { Close(); };

			name.TextChanged += delegate { UpdateNames(); };
			UpdateNames();
		}

		void UpdateNames() {
			Text = string.Format(initialTitle, name.Text);
			group.Text = name.Text;
		}

		void save_Click(object sender, EventArgs e) {
			if (!IsDirty())
				return;

			dict.Name = name.Text;
			dict.Author = author.Text;
			dict.Url = url.Text;
			dict.FirstLanguage = firstLanguage.Text;
			dict.SecondLanguage = secondLanguage.Text;

			if (saveOnClose) {
				try {
					dict.Save();
				} catch (InvalidOperationException ex) {
					ProgramLog.Default.AddMessage(
						LogType.Error,
						"Error saving dictionary information for {0}: {1}",
						dict.Name,
						ex.Message);
				}
			}

			Close();
		}

		bool IsDirty() {
			return 
				dict.Name != name.Text ||
				dict.Author != author.Text ||
				dict.Url != url.Text ||
				dict.FirstLanguage != firstLanguage.Text ||
				dict.SecondLanguage != secondLanguage.Text
				;
		}
	}
}
