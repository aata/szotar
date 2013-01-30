namespace Szotar.WindowsForms.Preferences {
	partial class Quizlet {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
				DisposeCts();
			}
			base.Dispose(disposing);
		}

		partial void DisposeCts();

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Quizlet));
			this.userNameLabel = new System.Windows.Forms.Label();
			this.userName = new System.Windows.Forms.TextBox();
			this.logInOut = new System.Windows.Forms.Button();
			this.browser = new Szotar.WindowsForms.Controls.WebBrowserWithErrors();
			this.progressUI = new Szotar.WindowsForms.Controls.ProgressUI();
			this.SuspendLayout();
			// 
			// userNameLabel
			// 
			resources.ApplyResources(this.userNameLabel, "userNameLabel");
			this.userNameLabel.Name = "userNameLabel";
			// 
			// userName
			// 
			resources.ApplyResources(this.userName, "userName");
			this.userName.Name = "userName";
			// 
			// logInOut
			// 
			resources.ApplyResources(this.logInOut, "logInOut");
			this.logInOut.Name = "logInOut";
			this.logInOut.UseVisualStyleBackColor = true;
			this.logInOut.Click += new System.EventHandler(this.LogInOut);
			// 
			// browser
			// 
			resources.ApplyResources(this.browser, "browser");
			this.browser.Name = "browser";
			this.browser.NavigateError += new Szotar.WindowsForms.Controls.WebBrowserNavigateErrorEventHandler(this.BrowserNavigateError);
			this.browser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.BrowserNavigated);
			this.browser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.BrowserNavigating);
			// 
			// progressUI
			// 
			resources.ApplyResources(this.progressUI, "progressUI");
			this.progressUI.Message = "Authenticating...";
			this.progressUI.Name = "progressUI";
			this.progressUI.Percent = null;
			this.progressUI.Cancelled += new System.EventHandler(this.AuthCancelled);
			// 
			// Quizlet
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.progressUI);
			this.Controls.Add(this.browser);
			this.Controls.Add(this.logInOut);
			this.Controls.Add(this.userName);
			this.Controls.Add(this.userNameLabel);
			this.Name = "Quizlet";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label userNameLabel;
		private System.Windows.Forms.TextBox userName;
		private System.Windows.Forms.Button logInOut;
		private Szotar.WindowsForms.Controls.WebBrowserWithErrors browser;
		private Controls.ProgressUI progressUI;

	}
}
