using System;
using System.Windows.Forms;

namespace Szotar.WindowsForms.UITests {
	public partial class ToolStripTester : Form {
		public ToolStripTester() {
			InitializeComponent();
		}

		private void themes_SelectedIndexChanged(object sender, EventArgs e) {
			if (themes.SelectedIndex < 0)
				return;

			if ((string)themes.Items[themes.SelectedIndex] == "System") {
				toolStripPanel1.Renderer = toolStrip1.Renderer = menuStrip1.Renderer = contextMenuStrip1.Renderer =
					new ToolStripSystemRenderer();
				return;
			}

			toolStripPanel1.Renderer = toolStrip1.Renderer = menuStrip1.Renderer = contextMenuStrip1.Renderer = 
				new ToolStripAeroRenderer(GetToolbarTheme());
		}

		ToolbarTheme GetToolbarTheme() {
			switch ((string)themes.Items[themes.SelectedIndex]) {
				case "Media":
					return ToolbarTheme.MediaToolbar;
				case "Communications":
					return ToolbarTheme.CommunicationsToolbar;
				case "BrowserTabBar":
					return ToolbarTheme.BrowserTabBar;
				default:
					return ToolbarTheme.Toolbar;
			}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e) {
			this.RightToLeft = ((CheckBox)sender).Checked ? RightToLeft.Yes : RightToLeft.No;
		}
	}
}
