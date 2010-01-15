using System;
using System.Collections.Generic;

namespace Szotar {
	public class PracticeItem {
		public long SetID { get; protected set; }
		public string Phrase { get; protected set; }
		public string Translation { get; protected set; }

		public PracticeItem(long setID, string phrase, string translation) {
			Phrase = phrase;
			Translation = translation;
			SetID = setID;
		}
	}

	/// <summary>A circular queue of PracticeItems, which shuffles itself every time it repeats.</summary>
	public class PracticeQueue {
		PracticeItem[] items;
		Random random = new Random();

		public PracticeQueue(List<PracticeItem> items) {
			if (items.Count == 0)
				throw new ArgumentException("items");

			this.items = items.ToArray();
			Index = 0;
			Laps = 0;
			Shuffle();
		}

		public IList<PracticeItem> AllItems {
			get {
				return items;
			}
		}

		void Shuffle() {
			// Fisher-Yates shuffle
			int n = items.Length;
			while (n > 1) {
				n--;
				int k = random.Next(n + 1);
				var t = items[k];
				items[k] = items[n];
				items[n] = t;
			}
		}

		public PracticeItem TakeOne() {
			var item = items[Index++];
			if (Index >= items.Length) {
				Shuffle();
				Index = 0;
				Laps++;
			}

			return item;
		}

		public int Laps { get; set; }
		public int Index { get; private set; }
		public int Length { get { return items.Length; } }
	}
}