using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
	public partial class SearchBox : UserControl {
		string text, promptText;
	    Font promptFont;
	    readonly Color foreColor, promptForeColor;

	    readonly TextBox textBox;

		public SearchBox() {
			InitializeComponent();

			text = string.Empty;
			prompting = true;

			textBox = new TextBox { Dock = DockStyle.Fill };
			textBox.GotFocus += SearchBoxEnter;
			textBox.LostFocus += SearchBoxLeave;
			textBox.TextChanged += SearchBoxTextChanged;
			Controls.Add(textBox);

			textBox.KeyPress += TextBoxKeyPress;

			foreColor = SystemColors.ControlText;
			promptForeColor = SystemColors.GrayText;

			promptFont = new Font(base.Font, FontStyle.Italic);

			components = components ?? new Container();
			components.Add(new DisposableComponent(promptFont));

			canPrompt = true;

			UpdatePrompt();
		}

		[Browsable(true)]
		public event EventHandler Search;

		void TextBoxKeyPress(object sender, KeyPressEventArgs e) {
		    if (e.KeyChar != (char)Keys.Enter)
		        return;
		    
            e.Handled = true;
		    var h = Search;
		    if (h != null)
		        h(this, new EventArgs());
		}

		void SearchBoxTextChanged(object sender, EventArgs e) {
		    if (canPrompt || textBox.Text == text)
		        return;
		    
            Text = textBox.Text;
		    OnTextChanged(new EventArgs());
		}

        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);

            promptFont = new Font(Font, FontStyle.Italic);
            if (prompting)
                textBox.Font = promptFont;

            //foreach (var dc in components.Components.OfType<DisposableComponent>()) {
            //    dc.Dispose();
            //    break;
            //}

            //components.Add(new DisposableComponent(promptFont));
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

		void SearchBoxLeave(object sender, EventArgs e) {
			canPrompt = true;
			UpdatePrompt();
		}

		void SearchBoxEnter(object sender, EventArgs e) {
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
			    return string.IsNullOrEmpty(promptText) ? Properties.Resources.SearchPrompt : promptText;
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
