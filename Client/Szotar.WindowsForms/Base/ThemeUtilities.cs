using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Szotar.WindowsForms {
	public static class ThemeHelper {
		internal static class NativeMethods {
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			public static extern IntPtr SendMessage
				(IntPtr hWnd, int msg, int wParam, int lParam);

			[DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
			public static extern int SetWindowTheme
				(IntPtr hWnd, String pszSubAppName, String pszSubIdList);

			public const int 
				TVS_EX_FADEINOUTEXPANDOS = 0x40,
				TVS_EX_AUTOHSCROLL = 0x20,
				TVS_EX_DOUBLEBUFFER = 0x4,
				TVS_NOHSCROLL = 0x8000,
				TVM_SETEXTENDEDSTYLE = 0x1100 + 44,
				LVS_EX_DOUBLEBUFFER = 0x10000,
				LVM_SETEXTENDEDSTYLE = 0x1000 + 54;

		}

		public static bool UseExplorerTheme(Control control) {
			try {
				NativeMethods.SetWindowTheme(control.Handle, "explorer", null);

				if (control is TreeView) {
					int extStyle = 
						NativeMethods.TVS_EX_DOUBLEBUFFER |
						NativeMethods.TVS_EX_AUTOHSCROLL | 
						NativeMethods.TVS_EX_FADEINOUTEXPANDOS;

					NativeMethods.SendMessage(control.Handle, NativeMethods.TVM_SETEXTENDEDSTYLE, 0, extStyle);

					TreeView tv = control as TreeView;
					tv.ShowLines = false;
					tv.HotTracking = true;
				} else if (control is ListView) {
					ListView lv = control as ListView;

					int extStyle = NativeMethods.LVS_EX_DOUBLEBUFFER;
					NativeMethods.SendMessage(control.Handle, NativeMethods.LVM_SETEXTENDEDSTYLE, 0, extStyle);
				}
				return true;
			} catch (System.DllNotFoundException) {
				return false;
			}
		}
	}
}