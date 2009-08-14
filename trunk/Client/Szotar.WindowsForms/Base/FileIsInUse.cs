using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Szotar.WindowsForms {
	[Flags]
	public enum FileIsInUseCapabilities : int {
		Closable = 1,
		HasWindow = 2
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComVisible(true)]
	[Guid("64a1cbf0-3a1a-4461-9158-376969693950")]
	public interface IFileIsInUse {
		void GetAppName([MarshalAs(UnmanagedType.LPWStr)] out string name);
		void GetUsage(out int usageType);
		void GetCapabilities(out FileIsInUseCapabilities capabilities);
		void GetSwitchToHWND(out IntPtr hwnd);
		void CloseFile();
	}

	public enum FileUsageType {
		Playing,
		Editing,
		Generic = 2
	}

	public class FileIsInUse : IDisposable, IFileIsInUse {
		int? cookie;
		IMoniker moniker;

		public FileIsInUse(string path) {
#if MONO
			return;
#endif
			try {
				int hresult = NativeMethods.CreateFileMoniker(path, out moniker);

				// Failing to create a moniker is bad. It's likely that nothing else will work
				// in this case. Perhaps it should be an exception though.
				if (hresult < 0)
					return;
			} catch (DllNotFoundException) {
				return;
			}

			UsageType = FileUsageType.Generic;
			IsInUse = true;
			CanClose = false;
		}

		IRunningObjectTable GetTable() {
			if (moniker == null)
				return null; //No point.

			IRunningObjectTable table;

			int hresult = NativeMethods.GetRunningObjectTable(0, out table);
			if (hresult < 0)
				return null;

			return table;
		}

		private void Register() {
			if (moniker == null)
				return; // No need to raise an error. This feature is merely a nicety anyway.

			if (cookie == null) {
				IRunningObjectTable table = GetTable();
				if (table != null) {
					object comObject = (IFileIsInUse)this;
					cookie = table.Register((int)(NativeMethods.RotFlags.RegistrationKeepsAlive), comObject, moniker);
				}
			} else {
				throw new InvalidOperationException("File is already registered as in use");
			}
		}

		private void Revoke() {
			if (moniker == null)
				return;

			if (cookie.HasValue) {
				IRunningObjectTable table = GetTable();
				if (table != null) {
					try {
						table.Revoke(cookie.Value);
					} catch (COMException) {
						// Realistically, there is no point trying to handle this exception.
					}
				}
			}
		}

		bool Supported {
			get { return moniker != null; }
		}

		public bool IsInUse {
			get {
				return cookie.HasValue;
			}
			set {
				if (value) {
					if (cookie == null)
						Register();
				} else {
					Revoke();
				}
			}
		}

		public IntPtr WindowHandle { get; set; }
		public bool CanClose { get; set; }
		public FileUsageType UsageType { get; set; }

		internal class NativeMethods {
			[DllImport("ole32.dll")]
			public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable rot);
			[DllImport("ole32.dll")]
			public static extern int CreateFileMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszPathName, out IMoniker ppmk);
			[Flags]
			public enum RotFlags : int {
				RegistrationKeepsAlive = 1,
				/// <summary>
				/// In order to use this flag, the application must have its executable name in the AppID section
				/// of the registry. We probably don't need it, anyway, it seems like a remoting thing?
				/// </summary>
				AllowAnyClient = 2
			}
		}

		public virtual void CloseFile() {
			throw new NotImplementedException();
		}

		public virtual void GetAppName(out string name) {
			name = System.Windows.Forms.Application.ProductName;
		}

		public virtual void GetCapabilities(out FileIsInUseCapabilities capabilities) {
			capabilities = 0;
			if (CanClose)
				capabilities |= FileIsInUseCapabilities.Closable;
			if (WindowHandle != IntPtr.Zero)
				capabilities |= FileIsInUseCapabilities.HasWindow;
		}

		public virtual void GetSwitchToHWND(out IntPtr hwnd) {
			if (WindowHandle != IntPtr.Zero) {
				hwnd = WindowHandle;
			} else {
				hwnd = IntPtr.Zero;
				throw new InvalidOperationException();
			}
		}

		public virtual void GetUsage(out int usageType) {
			usageType = (int)UsageType;
		}

		public void Dispose() {
			Dispose(true);
		}

		public void Dispose(bool disposing) {
			if (disposing) {
				Revoke();
			}
			GC.SuppressFinalize(this);
		}

		~FileIsInUse() {
			Dispose(true);
		}
	}
}
