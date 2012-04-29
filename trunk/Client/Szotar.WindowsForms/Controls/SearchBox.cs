using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
	public partial class SearchBox : UserControl {
		string text, promptText;
		Font promptFont;
		Color foreColor, promptForeColor;

		TextBox textBox;

		public SearchBox() {
			InitializeComponent();

			text = string.Empty;
			prompting = true;

			textBox = new TextBox { Dock = DockStyle.Fill };
			textBox.GotFocus += new EventHandler(SearchBox_Enter);
			textBox.LostFocus += new EventHandler(SearchBox_Leave);
			textBox.TextChanged += new EventHandler(SearchBox_TextChanged);
			Controls.Add(textBox);

			textBox.KeyUp += new KeyEventHandler(textBox_KeyUp);

			foreColor = SystemColors.ControlText;
			promptForeColor = SystemColors.GrayText;

			promptFont = new Font(Font, FontStyle.Italic);

			components = components ?? new Container();
			components.Add(new DisposableComponent(promptFont));

			canPrompt = true;

			UpdatePrompt();
		}

		[Browsable(true)]
		public event EventHandler Search;

		void textBox_KeyUp(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
				e.Handled = true;
				var h = Search;
				if (h != null)
					h(this, new EventArgs());
			}
		}

		void SearchBox_TextChanged(object sender, EventArgs e) {
			if (!canPrompt && textBox.Text != text) {
				Text = textBox.Text;
				OnTextChanged(new EventArgs());
			}
		}

		public override string Text {
			get {
				return text;
			}
			set {
				if (text == value)
					return;

				textBox.Text = value ?? string.Empty;
				text = value ?? string.Empty;
				if (canPrompt)
					OnTextChanged(new EventArgs());
				Prompting = string.IsNullOrEmpty(text);
			}
		}

		bool canPrompt, prompting;
		protected bool Prompting {
			get {
				return prompting;
			}
			set {
				if (value == prompting)
					return;

				prompting = value;
				UpdatePrompt(); 
			}
		}

		void SearchBox_Leave(object sender, EventArgs e) {
			canPrompt = true;
			UpdatePrompt();
		}

		void SearchBox_Enter(object sender, EventArgs e) {
			canPrompt = false;
			UpdatePrompt();
		}

		void UpdatePrompt() {
			if (canPrompt && prompting) {
				textBox.Font = promptFont;
				textBox.Text = promptText;
				textBox.ForeColor = promptForeColor;
			} else {
				textBox.Font = Font;
				textBox.Text = text;
				textBox.ForeColor = foreColor;
			}
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
			set { 
				promptText = value; 
				UpdatePrompt();
			}
		}

		public void SelectAll() {
			textBox.SelectAll();
		}
	}
}
