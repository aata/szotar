using System;
using System.Collections.Generic;
using System.Text;

namespace Szotar {
	public class NullWeakReference<T> {
		private WeakReference weak;

		public NullWeakReference(T @object) {
			weak = new WeakReference(@object);
		}

		public bool IsAlive {
			get { return weak.IsAlive; }
		}

		public override int GetHashCode() {
			return weak.GetHashCode();
		}

		public bool TrackResurrection {
			get { return weak.TrackResurrection; }
		}

		public T Target {
			get {
				try {
					return (T)weak.Target;
				} catch (InvalidOperationException) {
					return default(T);
				}
			}
		}
	}
}
