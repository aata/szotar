using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
	// See http://geekswithblogs.net/CPound/archive/2006/02/27/70834.aspx
	// Simply setting DoubleBuffered to true doesn't seem to have any effect, but this does.
	public class ListViewNF : ListView {
		public ListViewNF() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			// NB. This won't work in partially trusted code.
			SetStyle(ControlStyles.EnableNotifyMessage, true);
		}

		protected override void OnNotifyMessage(Message m) {
			if (m.Msg != 0x14) // WM_ERASEBKGND
				base.OnNotifyMessage(m);
		}
	}
}