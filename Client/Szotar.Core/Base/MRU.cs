﻿using System;
using System.Collections.Generic;

namespace Szotar {
	[Serializable]
	public class MruEntry {
		public string Path { get; set; }
		public string Title { get; set; }
	}

	[Serializable]
	public class MruList {
		public List<MruEntry> Entries { get; set; }

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
			Entries = new List<MruEntry>();
			MaximumSize = size;
		}

		void Clip() {
			if (Entries.Count > MaximumSize)
				Entries.RemoveRange(MaximumSize, Entries.Count - MaximumSize);
		}

		public void Update(string path, string title) {
			var entry = new MruEntry { Path = path, Title = title };
			int index = Entries.FindIndex(e => e.Path == path);
			if (index != -1)
				Entries.RemoveAt(index);
			Entries.Insert(0, entry);

			Clip();
		}
	}
}