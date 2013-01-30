using System;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
	public partial class ProgressUI : UserControl {
		public ProgressUI() {
			InitializeComponent();

			progressBar.Style = ProgressBarStyle.Marquee;
			progressBar.MarqueeAnimationSpeed = 4;
		}

		public string Message {
			get { return progressLabel.Text; }
			set { progressLabel.Text = value; }
		}

		public int? Percent {
			get {
				if (progressBar.Style == ProgressBarStyle.Marquee)
					return null;
				return progressBar.Value;
			}
			set {
				if (value == null) {
					progressBar.Style = ProgressBarStyle.Marquee;
				} else {
					progressBar.Style = ProgressBarStyle.Continuous;
					progressBar.Value = value.Value;
				}
			}
		}

		private void CancelClick(object sender, EventArgs e) {
			OnCancelled();
			cancel.Enabled = false;
		}

		public void Cancel() {
			OnCancelled();
			cancel.Enabled = false;
		}

		public event EventHandler Cancelled;
		private void OnCancelled() {
			EventHandler handler = Cancelled;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public void CancelFailed() {
			cancel.Enabled = false;
		}
	}
}
