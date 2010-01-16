using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class LogViewerForm : Form {
		public LogViewerForm() {
			InitializeComponent();

			ProgramLog.Default.MessageAdded += new System.EventHandler<LogEventArgs>(LogMessageAdded);
			FormClosed += delegate {
				ProgramLog.Default.MessageAdded -= new System.EventHandler<LogEventArgs>(LogMessageAdded);
			};
		}

		void LogMessageAdded(object sender, LogEventArgs e) {
			viewer.AddMessage(e.Message);
		}
	}
}
