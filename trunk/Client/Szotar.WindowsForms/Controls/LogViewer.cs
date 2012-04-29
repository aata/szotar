using System;
using System.Windows.Forms;

namespace Szotar.WindowsForms.Controls {
	public partial class LogViewer : UserControl {
		bool showMetrics, showDebug, showWarnings, showErrors;
		ProgramLog log;

		public LogViewer() {
			InitializeComponent();

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

			UpdateView();
		}

		public void AddMessage(LogMessage message) {
			if (InvokeRequired) {
				Invoke(new Action(delegate { AddMessage(message); }));
				return;
			}

			if (Filter(message)) {
				list.Items.Add(MakeItem(message));
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

			ScrollToEnd();
		}

		ListViewItem MakeItem(LogMessage m) {
			string type = m.Type.ToString();
			switch (m.Type) {
				case LogType.Debug: type = debug.Text; break;
				case LogType.Error: type = error.Text; break;
				case LogType.Metrics: type = metrics.Text; break;
				case LogType.Warning: type = warning.Text; break;
			}

			var item = new ListViewItem(new[] { type, m.Time.ToString("t", System.Threading.Thread.CurrentThread.CurrentUICulture), m.Text });
			item.Tag = m;
			return item;
		}
	}
}
