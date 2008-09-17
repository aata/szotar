using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class PracticeWindow : Form {
		public PracticeWindow() {
			InitializeComponent();

			mainMenu.Renderer = new ToolStripAeroRenderer(ToolbarTheme.MediaToolbar);
		}
	}
}
