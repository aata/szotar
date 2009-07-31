using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace Szotar {
	public enum LogType {
		Metrics,
		Debug,
		Warning,
		Error
	}

	public class LogMessage {
		public LogType Type { get; set; }
		public string Text { get; set; }
		public DateTime Time { get; set; }
	}

	public class LogEventArgs : EventArgs {
		public LogMessage Message { get; set; }

		public LogEventArgs(LogMessage m) { Message = m; }
	}

	public class ProgramLog {
		public List<LogMessage> Messages { get; set; }
		public int MaxLength { get; set; }
		object syncObject = new object();

		public static ProgramLog Default { get; set; }

		static ProgramLog() {
			Default = new ProgramLog();
		}

		public ProgramLog() {
			Messages = new List<LogMessage>();
			MaxLength = 8192;
		}

		/// <remarks>Do not call this from another thread. Yet.</remarks>
		public void AddMessage(LogType type, string message, params object[] inserts) {
			var m = new LogMessage {
				Type = type,
				Text = string.Format(CultureInfo.InvariantCulture, message, inserts),
				Time = DateTime.Now
			};

			lock (syncObject) {
				Messages.Add(m);

				if (Messages.Count > MaxLength)
					Messages.RemoveRange(0, Messages.Count / 2);
			}

			RaiseMessageAdded(m);
		}

		public event EventHandler<LogEventArgs> MessageAdded;

		protected void RaiseMessageAdded(LogMessage m) {
			var f = MessageAdded;
			if (f != null)
				f(this, new LogEventArgs(m));
		}
	}

	public static class Metrics {
		public static void Measure(string name, Action action) {
			var timer = new Stopwatch();
			timer.Start();
			action();
			LogMeasurement(name, timer.Elapsed);
		}

		public static void LogMeasurement(string name, TimeSpan time) {
			ProgramLog.Default.AddMessage(LogType.Metrics, "{0}: took {1}ms", name, time.TotalMilliseconds);
		}
	}
}