using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Szotar.WindowsForms.Controls {
	public class WebBrowserWithErrors : WebBrowser {
		AxHost.ConnectionPointCookie cookie;
		EventHelper helper;

		[PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void CreateSink() {
			base.CreateSink();

			// Create an instance of the client that will handle the event
			// and associate it with the underlying ActiveX control.
			helper = new EventHelper(this);
			cookie = new AxHost.ConnectionPointCookie(
				ActiveXInstance, helper, typeof(DWebBrowserEvents2));
		}

		[PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void DetachSink() {
			// Disconnect the client that handles the event
			// from the underlying ActiveX control.
			if (cookie != null) {
				cookie.Disconnect();
				cookie = null;
			}
			base.DetachSink();
		}

		public event WebBrowserNavigateErrorEventHandler NavigateError;
		protected virtual void OnNavigateError(WebBrowserNavigateErrorEventArgs e) {
			if (NavigateError != null)
				NavigateError(this, e);
		}

		// Handles the NavigateError event from the underlying ActiveX 
		// control by raising the NavigateError event defined in this class.
		private class EventHelper : StandardOleMarshalObject, DWebBrowserEvents2 {
			private readonly WebBrowserWithErrors parent;

			public EventHelper(WebBrowserWithErrors parent) {
				this.parent = parent;
			}

			public void NavigateError(object pDisp, ref object url,
				ref object frame, ref object statusCode, ref bool cancel) {
				// Raise the NavigateError event.
				parent.OnNavigateError(
					new WebBrowserNavigateErrorEventArgs(
					(string)url, (string)frame, (int)statusCode, cancel));
			}
		}
	}

	// Represents the method that will handle the WebBrowserWithErrors.NavigateError event.
	public delegate void WebBrowserNavigateErrorEventHandler(
		object sender,
		WebBrowserNavigateErrorEventArgs e);

	// Provides data for the WebBrowserWithErrors.NavigateError event.
	public class WebBrowserNavigateErrorEventArgs : EventArgs {
		public WebBrowserNavigateErrorEventArgs(String url, String frame, Int32 statusCode, Boolean cancel) {
			Url = url;
			Frame = frame;
			StatusCode = statusCode;
			Cancel = cancel;
		}

		public string Url { get; set; }
		public string Frame { get; set; }
		public int StatusCode { get; set; }
		public bool Cancel { get; set; }
	}

	// Imports the NavigateError method from the OLE DWebBrowserEvents2 
	// interface. 
	[ComImport, Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D"),
	InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
	TypeLibType(TypeLibTypeFlags.FHidden)]
	interface DWebBrowserEvents2 {
		[DispId(271)]
		void NavigateError(
			[In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
			[In] ref object url, [In] ref object frame,
			[In] ref object statusCode, [In, Out] ref bool cancel);
	}
}