using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Preferences {
	[PreferencePage("Display", Parent = typeof(Categories.General), Importance = 5)]
	public partial class Display : PreferencePage {
		string fontName;
		float fontSize;
        bool setLanguage;

		public Display() {
			InitializeComponent();

			fontName = GuiConfiguration.ListFontName;
			fontSize = GuiConfiguration.ListFontSize;

            try {
                var culture = new System.Globalization.CultureInfo(GuiConfiguration.UiLanguage).Name;
                if (culture == "en-US")
                    englishUS.Checked = true;
                else if (culture == "en-GB")
                    englishGB.Checked = true;
                else if (culture == "hu-HU")
                    hungarian.Checked = true;
            } catch(ArgumentException) {
                // The UI language isn't any of the supported languages.
            }

            setLanguage = false;

			UpdateListFontButton();
			listFontButton.Click += new EventHandler(listFontButton_Click);

            englishUS.Click += new EventHandler(LanguageChanged);
            englishGB.Click += new EventHandler(LanguageChanged);
            hungarian.Click += new EventHandler(LanguageChanged);
		}

        void LanguageChanged(object sender, EventArgs e) {
            setLanguage = true;
        }

		void listFontButton_Click(object sender, EventArgs e) {
			FontDialog fd = new FontDialog();
			if (fontName != null) {
				fd.Font = new Font(fontName, fontSize);
			}
			if (fd.ShowDialog() == DialogResult.OK) {
				fontName = fd.Font.Name;
				fontSize = fd.Font.Size;
				UpdateListFontButton();
			}
		}

		public void UpdateListFontButton() {
			if (fontName != null) {
				listFontButton.Text = string.Format("{0} ({1}pt)", fontName, fontSize);
			} else {
				listFontButton.Text = "(Default)";
			}
		}

		public override void Commit() {
			if (fontName != null) {
				GuiConfiguration.ListFontName = fontName;
				GuiConfiguration.ListFontSize = fontSize;
			} else {
				Configuration.Default.Delete("ListFontName");
				Configuration.Default.Delete("ListFontSize");
			}

            if (setLanguage) {
                if (englishUS.Checked)
                    GuiConfiguration.UiLanguage = "en-US";
                if (englishGB.Checked)
                    GuiConfiguration.UiLanguage = "en-GB";
                else if (hungarian.Checked)
                    GuiConfiguration.UiLanguage = "hu-HU";
            }
		}
	}
}
