using System;
using System.ComponentModel;

namespace Szotar.WindowsForms {
	class DisposableComponent : IComponent {
		private IDisposable thing;
		public event EventHandler Disposed;
		public ISite Site { get; set; }

		public DisposableComponent(IDisposable thing) {
			this.thing = thing;
			if (thing == null)
				ProgramLog.Default.AddMessage(LogType.Error, "DisposableComponent: passed a null IDisposable");
		}

		public void Dispose() {
			EventHandler ev = Disposed;
			if (ev != null)
				ev(this, new EventArgs());
			if(thing != null)
				thing.Dispose();
		}
	}
}