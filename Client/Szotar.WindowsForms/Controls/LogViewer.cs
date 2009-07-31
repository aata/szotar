using System;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
	public partial class LogViewer : UserControl {
		bool showMetrics, showDebug, showWarnings, showErrors;
		ProgramLog log;

		public LogViewer() {
			InitializeComponent();

			// TODO: Load from configuration
			showMetrics = metrics.Checked = GuiConfiguration.LogViewerShowMetrics;
			showDebug = debug.Checked = GuiConfiguration.LogViewerShowDebug;
			showWarnings = warning.Checked = GuiConfiguration.LogViewerShowWarnings;
			showErrors = error.Checked = GuiConfiguration.LogViewerShowErrors;

			metrics.CheckedChanged += delegate {
				showMetrics = GuiConfiguration.LogViewerShowMetrics = metrics.Checked;
				UpdateView();
			};

			debug.CheckedChanged += delegate {
				showDebug = GuiConfiguration.LogViewerShowDebug = debug.Checked;
				UpdateView();
			};

			warning.CheckedChanged += delegate {
				showWarnings = GuiConfiguration.LogViewerShowWarnings = warning.Checked;
				UpdateView();
			};

			error.CheckedChanged += delegate {
				showErrors = GuiConfiguration.LogViewerShowErrors = error.Checked;
				UpdateView();
			};

			log = ProgramLog.Default;
			log.MessageAdded += new EventHandler<LogEventArgs>(log_MessageAdded);

			UpdateView();
		}

		void log_MessageAdded(object sender, LogEventArgs e) {
			if (InvokeRequired) {
				Invoke(new Action(delegate { log_MessageAdded(sender, e); }));
				return;
			}

			if (Filter(e.Message)) {
				list.Items.Add(MakeItem(e.Message));
				ScrollToEnd();
			}
		}

		void ScrollToEnd() {
			if (list.Items.Count > 0)
				list.EnsureVisible(list.Items.Count - 1);
		}

		bool Filter(LogMessage m) {
			if ((m.Type == LogType.Debug && showDebug)
				|| (m.Type == LogType.Error && showErrors)
				|| (m.Type == LogType.Metrics && showMetrics)
				|| (m.Type == LogType.Warning && showWarnings))
				return true;

			return false;
		}

		void UpdateView() {
			if (InvokeRequired) {
				Invoke(new Action(delegate { UpdateView(); }));
				return;
			}

			list.BeginUpdate();
			list.Items.Clear();
			foreach (LogMessage m in log.Messages) {
				if (Filter(m))
					list.Items.Add(MakeItem(m));
			}
			list.EndUpdate();

			list.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
			ScrollToEnd();
		}

		ListViewItem MakeItem(LogMessage m) {
			var item = new ListViewItem(new[] { m.Type.ToString(), m.Time.ToShortTimeString(), m.Text });
			item.Tag = m;
			return item;
		}
	}
}
