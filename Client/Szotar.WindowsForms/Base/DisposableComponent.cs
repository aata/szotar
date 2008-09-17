using System;
using System.ComponentModel;

namespace Szotar.WindowsForms {
	class DisposableComponent : IComponent {
		private IDisposable thing;
		public event EventHandler Disposed;
		public ISite Site { get; set; }

		public DisposableComponent(IDisposable thing) {
			this.thing = thing;
		}

		public void Dispose() {
			EventHandler ev = Disposed;
			if (ev != null)
				ev(this, new EventArgs());
			thing.Dispose();
		}
	}
}