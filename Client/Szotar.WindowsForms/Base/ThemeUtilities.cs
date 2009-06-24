using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Szotar.WindowsForms {
	public static class ThemeHelper {
		internal static class NativeMethods {
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			public static extern IntPtr SendMessage
				(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

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
				LVS_EX_LABELTIP = 0x4000,
				LVM_GETEXTENDEDLISTVIEWSTYLE = 0x1000 + 55,
				LVM_SETEXTENDEDLISTVIEWSTYLE = 0x1000 + 54,
				LVS_EX_FULLROWSELECT = 0x20;
		}

		public static bool UseExplorerTheme(params Control[] controls) {
			try {
				foreach (Control control in controls) {
					NativeMethods.SetWindowTheme(control.Handle, "explorer", null);

					if (control is TreeView) {
						int extStyle =
							NativeMethods.TVS_EX_DOUBLEBUFFER |
							NativeMethods.TVS_EX_AUTOHSCROLL |
							NativeMethods.TVS_EX_FADEINOUTEXPANDOS;

						NativeMethods.SendMessage(
							control.Handle, 
							NativeMethods.TVM_SETEXTENDEDSTYLE, 
							IntPtr.Zero, 
							new IntPtr(extStyle));

						TreeView tv = control as TreeView;
						tv.ShowLines = false;
						tv.HotTracking = true;

					} else if (control is ListView) {
						ListView lv = control as ListView;

						int extLvStyle = NativeMethods.SendMessage(
							control.Handle, 
							NativeMethods.LVM_GETEXTENDEDLISTVIEWSTYLE, 
							IntPtr.Zero, 
							IntPtr.Zero).ToInt32();

						extLvStyle |= NativeMethods.LVS_EX_FULLROWSELECT 
							| NativeMethods.LVS_EX_DOUBLEBUFFER 
							| NativeMethods.LVS_EX_LABELTIP;
						
						NativeMethods.SendMessage(
							control.Handle, 
							NativeMethods.LVM_SETEXTENDEDLISTVIEWSTYLE, 
							IntPtr.Zero, 
							new IntPtr(extLvStyle));
					}
				}
			} catch (DllNotFoundException) {
				return false;
			}

			return true;
		}
	}
}