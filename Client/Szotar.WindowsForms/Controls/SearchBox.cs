using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
	public partial class SearchBox : TextBox {
		internal String promptText;
		internal Font promptFont;
		internal Font realFont;
		internal Color realForeColor;
		internal Color promptColor;
		internal bool isPrompting = true;

		public SearchBox() {
			InitializeComponent();

			if (DesignMode == false) {
				realFont = Font;
				realForeColor = ForeColor;

				this.Enter += new EventHandler(SearchBox_Enter);
				this.Leave += new EventHandler(SearchBox_Leave);

				Text = string.Empty;
				SetPrompt();
			}
		}

		/// <summary>
		/// Sets the base font of the SearchBox instance.
		/// </summary>
		/// <remarks>Designer-generated code constructs a SearchBox *then* sets the font, so for DPI-awareness (as an example) we must track changes to the Font.</remarks>
		public override Font Font {
			get { return base.Font; }
			set {
				realFont = value;
				base.Font = value;
			}
		}

		void SearchBox_Leave(object sender, EventArgs e) {
			if (Text.Length == 0) {
				isPrompting = true;
				SetPrompt();
			}
		}

		internal void ClearPrompt() {
			if (this.DesignMode == true)
				return;

			isPrompting = false;

			base.Font = realFont;
			base.Text = String.Empty;
			ForeColor = realForeColor;
		}

		internal void SetPrompt() {
			if (this.DesignMode == true)
				return;

			if (isPrompting) {
				if (PromptFont != null)
					base.Font = PromptFont;
				base.Text = promptText;
				ForeColor = SystemColors.GrayText;
			}
		}

		public string RealText {
			get {
				if (isPrompting)
					return String.Empty;
				else
					return Text;
			}
		}

		public override string Text {
			get {
				return base.Text;
			}
			set {
				if (string.IsNullOrEmpty(value))
					SetPrompt();
				else if (isPrompting)
					ClearPrompt();

				base.Text = value;
			}
		}

		public bool IsPrompting {
			get { return isPrompting; }
		}

		/// <summary>
		/// Clears the prompt and resets the font to the original font.
		/// </summary>
		void SearchBox_Enter(object sender, EventArgs e) {
			if (isPrompting) {
				ClearPrompt();
			}
		}

		/// <summary>
		/// The font which will be used to display the search prompt.
		/// </summary>
		[Browsable(true)]
		public Font PromptFont {
			get {
				if (promptFont == null)
					return new Font(realFont, FontStyle.Italic);
				return promptFont;
			}
			set { promptFont = value; SetPrompt(); }
		}

		/// <summary>
		/// The color which will be used to display the search prompt.
		/// </summary>
		[Browsable(true)]
		public Color PromptColor {
			get {
				return SystemColors.GrayText;
			}
			set { promptColor = value; SetPrompt(); }
		}

		/// <summary>
		/// The text which will be displayed when the search box is empty and unfocused.
		/// </summary>
		[Browsable(true), Localizable(true), AmbientValue("")]
		public String PromptText {
			get {
				if (string.IsNullOrEmpty(promptText))
					return "Search";
				return promptText;
			}
			set { promptText = value; SetPrompt(); }
		}
	}
}
