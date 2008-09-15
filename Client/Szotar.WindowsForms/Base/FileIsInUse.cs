using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Asztal.Szótár {
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComVisible(true)]
	[Guid("64a1cbf0-3a1a-4461-9158-376969693950")]
	public interface IFileIsInUse {
		void GetAppName([MarshalAs(UnmanagedType.LPWStr)] out string name);
		void GetUsage(out int usageType);
		void GetCapabilities(out int capabilities);
		void GetSwitchToHWND(out IntPtr hwnd);
		void CloseFile();
	}

	public class FileIsInUse : IDisposable, IFileIsInUse {
		int? cookie;
		IMoniker moniker;

		#region IDisposable
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
		#endregion

		public FileIsInUse(string path) {
			try {
				int hresult = NativeMethods.CreateFileMoniker(path, out moniker);

				//Failing to create a moniker is bad. It's likely that nothing else will work
				//in this case. Perhaps it should be an exception though.
				if (hresult < 0)
					return;
			} catch (DllNotFoundException) {
				return;
			}

			UsageType = FileUsageType.Generic;
			IsInUse = true;
			CanClose = false;
		}

		#region IRunningObjectTable stuff
		IRunningObjectTable GetTable() {
			IRunningObjectTable table;

			int hresult = NativeMethods.GetRunningObjectTable(0, out table);
			if (hresult < 0)
				return null;

			return table;
		}

		private void Register() {
			if (cookie == null) {
				IRunningObjectTable table = GetTable();
				if (table != null) {
					object comObject = this;
					cookie = table.Register((int)(NativeMethods.RotFlags.RegistrationKeepsAlive), comObject, moniker);
				}
			} else {
				throw new InvalidOperationException("File is already registered as in use");
			}
		}

		private void Revoke() {
			if (cookie.HasValue) {
				IRunningObjectTable table = GetTable();
				if (table != null) {
					try {
						table.Revoke(cookie.Value);
					} catch (COMException) {
						//Realistically, there is no point trying to handle this exception.
					}
				}
			}
		}
		#endregion

		bool Supported {
			get { return moniker != null; }
		}

		bool IsInUse {
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

		IntPtr WindowHandle {
			get;
			set;
		}

		bool CanClose { get; set; }

		enum FileUsageType {
			Playing,
			Editing,
			Generic = 2
		}

		FileUsageType UsageType {
			get;
			set;
		}

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

		#region IFileIsInUse
		public void CloseFile() {
			throw new NotImplementedException();
		}

		public void GetAppName(out string name) {
			name = System.Windows.Forms.Application.ProductName;
		}

		public void GetCapabilities(out int capabilities) {
			capabilities = 0;
			if (CanClose)
				capabilities |= 1;
			if (WindowHandle != IntPtr.Zero)
				capabilities |= 2;
		}

		public void GetSwitchToHWND(out IntPtr hwnd) {
			if (WindowHandle != IntPtr.Zero) {
				hwnd = WindowHandle;
			} else {
				hwnd = IntPtr.Zero;
				throw new InvalidOperationException();
			}
		}

		public void GetUsage(out int usageType) {
			usageType = (int)UsageType;
		}
		#endregion
	}
}
