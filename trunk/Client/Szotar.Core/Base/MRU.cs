using System;
using System.Collections.Generic;

namespace Szotar {
	public interface IKeyCompare<T> {
		int CompareKeyWith(T b);
	}

	[Serializable]
	public class MruList<T>
		where T : IKeyCompare<T>
	{
		public List<T> Entries { get; set; }
		int size;

		public int MaximumSize {
			get {
				return size;
			}
			set {
				size = value;
				Clip();
			}
		}

		public MruList() : this(10) { }

		public MruList(int size) {
			Entries = new List<T>();
			MaximumSize = size;
		}

		void Clip() {
			if (Entries.Count > MaximumSize)
				Entries.RemoveRange(MaximumSize, Entries.Count - MaximumSize);
		}

		public void Update(T newItem) {
			int index = Entries.FindIndex(e => e.CompareKeyWith(newItem) == 0);
			if (index != -1)
				Entries.RemoveAt(index);
			Entries.Insert(0, newItem);

			Clip();
		}
	}
}