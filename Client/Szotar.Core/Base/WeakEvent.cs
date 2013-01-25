using System.Collections.Generic;
using System;

namespace Szotar {
	public interface IWeakEventListener<in TSender, in TArgs> 
		where TSender : class 
		where TArgs : EventArgs 
	{
		void ReceiveWeakEvent(TSender sender, TArgs args);
	}

	public abstract class WeakEventManager<TSender, TArgs> 
		where TSender : class 
		where TArgs : EventArgs 
	{
		readonly TSender eventSource;
		readonly List<NullWeakReference<IWeakEventListener<TSender, TArgs>>> listeners;

		protected WeakEventManager(TSender eventSource) {
			this.eventSource = eventSource;
			listeners = new List<NullWeakReference<IWeakEventListener<TSender, TArgs>>>();
		}

		public void AddListener(IWeakEventListener<TSender, TArgs> listener) {
			listeners.Add(new NullWeakReference<IWeakEventListener<TSender, TArgs>>(listener));
		}

		public void RemoveListener(IWeakEventListener<TSender, TArgs> listener) {
			listeners.Remove(wr => {
				var target = wr.Target;
				return target == listener || target == null;
			});
		}

		protected void RaiseEvent(TArgs args) {
			for (int i = 0; i < listeners.Count; i++) {
				var wr = listeners[i];
				var listener = wr.Target;
				if (listener != null)
					listener.ReceiveWeakEvent(eventSource, args);
				else
					listeners.RemoveAt(i);
			}
		}
	}
}