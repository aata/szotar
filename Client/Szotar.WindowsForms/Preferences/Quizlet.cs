using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Szotar.Quizlet;

namespace Szotar.WindowsForms.Preferences {
	[PreferencePage("Quizlet", Parent = typeof(Categories.General), Importance = 5)]
	public partial class Quizlet : PreferencePage, IProgress<ProgressChangedEventArgs> {
		private string randomState;
		private readonly QuizletApi api;
		private CancellationTokenSource cts;

		public Quizlet() {
			InitializeComponent();

			api = new QuizletApi();

			if (api.Credentials.HasValue) {
				logInOut.Text = Properties.Resources.LogOut;
				userName.Text = Configuration.UserName;
			} else {
				logInOut.Text = Properties.Resources.LogIn;
			}
		}

		partial void DisposeCts() {
			if (cts != null) {
				cts.Dispose();
				cts = null;
			}
		}

		public override void Commit() {
		}

		private void LogInOut(object sender, EventArgs e) {
			if (api.Credentials.HasValue) {
				// Log out
				foreach (var setting in new[] { "UserName", "AccessToken", "AccessTokenExpiry" })
					Configuration.Default.Delete(setting);
			} else {
				StartLogin();
			}
		}

		private void StartLogin() {
			// Log in - leave the button enabled in case it fails
			var uri = api.GetLoginPageUri(out randomState);
			browser.Show();
			browser.Navigate(uri);
		}

		private void BrowserNavigated(object sender, WebBrowserNavigatedEventArgs e) {
			// ProcessUri(e.Url);
		}

		private void BrowserNavigateError(object sender, Controls.WebBrowserNavigateErrorEventArgs e) {
			Uri uri;
			if(Uri.TryCreate(e.Url, UriKind.Absolute, out uri))
				ProcessUri(uri);
		}

		private void BrowserNavigating(object sender, WebBrowserNavigatingEventArgs e) {
			ProcessUri(e.Url);
		}

		private void ProcessUri(Uri uri) {
			string code = null;
			string confirmState = null;
			string error = null;
			string errorDesc = null;

			// TODO: Check for "error" GET variable
			foreach (var term in uri.Query.Split('&', '?')) {
				var parts = term.Split('=');
				if (parts.Length < 2)
					continue;

				if (parts[0] == "error")
					error = parts[1];

				if (parts[0] == "error_description")
					errorDesc = parts[1];

				if (parts[0] == "code")
					code = parts[1];

				if (parts[0] == "state")
					confirmState = parts[1];
			}

			if (confirmState != null && code != null) {
				if (randomState == confirmState) {
					browser.Hide();
					progressUI.Show();

					cts = new CancellationTokenSource();
					progressUI.Cancelled += AuthCancelled;
					var task = api.Authenticate(code, cts.Token, this);
					task.ContinueWith(t =>
						SynchronizationContext.Current.Post(delegate {
							if (t.Exception != null) {
								MessageBox.Show(t.Exception.GetBaseException().Message, Properties.Resources.ErrorLoggingIn, MessageBoxButtons.OK, MessageBoxIcon.Error);
							} else if (t.IsCanceled) {
								progressUI.Hide();
							} else {
								Configuration.AccessToken = t.Result.ApiToken;
								Configuration.AccessTokenExpiry = t.Result.ValidTo;
								Configuration.UserName = t.Result.UserName;
								Configuration.Save();

								progressUI.Hide();
								userName.Text = t.Result.UserName;
								logInOut.Text = Properties.Resources.LogOut;
							}
						}, null)
					);
				} else {
					MessageBox.Show(Properties.Resources.InvalidAuthResponse, Properties.Resources.ErrorLoggingIn, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			} else if (error != null) {
				MessageBox.Show(
					errorDesc ?? Properties.Resources.UnknownErrorLoggingIn,
					Properties.Resources.ErrorLoggingIn, 
					MessageBoxButtons.OK, 
					MessageBoxIcon.Error);
				StartLogin();
			} 
		}

		private void AuthCancelled(object sender, EventArgs e) {
			if (cts == null)
				return;
			
			cts.Cancel();
			cts.Dispose(); 
			cts = null;
			progressUI.Cancelled -= AuthCancelled;
		}

		void IProgress<ProgressChangedEventArgs>.Report(ProgressChangedEventArgs progress) {
			progressUI.Percent = progress.ProgressPercentage;
		}
	}
}
