using System.Windows.Forms;

namespace Szotar.WindowsForms.Forms {
	public partial class LogViewerForm : Form {
		public LogViewerForm() {
			InitializeComponent();
		}

		public static void Open() {
			LogViewerForm form = null;

			foreach (Form f in Application.OpenForms)
				if (f is LogViewerForm)
					form = (LogViewerForm)f;

			form = form ?? new Forms.LogViewerForm();
			form.Show();
		}
	}
}
