using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

//TODO: Hide stack trace by default, store visibility in GuiConfiguration.
namespace Szotar.WindowsForms.Controls {
	public partial class ErrorUI : UserControl {
		Bitmap errorBitmap;

		public ErrorUI(string error, string stack) {
			InitializeComponent();
			
			icon.Image = errorBitmap = SystemIcons.Error.ToBitmap();

			ErrorText = error;
			StackTrace = stack;
		}

		public string StackTrace {
			get {
				return stackTrace.Text;
			}
			set {
				stackTrace.Text = value;
			}
		}

		public string ErrorText {
			get { return text.Text; }
			set { text.Text = value; }
		}
	}
}
