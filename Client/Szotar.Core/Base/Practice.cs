using System;
using System.Collections.Generic;
using System.Linq;

namespace Szotar {
	[System.Diagnostics.DebuggerDisplay("{Phrase}: {History.DebugText}")]
	public class PracticeItem {
		public long SetID { get; protected set; }
		public string Phrase { get; protected set; }
		public string Translation { get; protected set; }
		public PracticeHistory History { get; protected set; }

		public PracticeItem(long setID, string phrase, string translation, PracticeHistory history = null) {
			Phrase = phrase;
			Translation = translation;
			SetID = setID;
			History = history ?? new PracticeHistory();
		}
	}

	[System.Diagnostics.DebuggerDisplay("{DebugText}")]
	public class PracticeHistory {
		public List<KeyValuePair<DateTime, bool>> History { get; protected set; }

		double? importance;
		public double Importance {
			get {
				if (importance == null)
					importance = Need();
				return importance.Value;
			}
		}

		public PracticeHistory() {
			History = new List<KeyValuePair<DateTime, bool>>();
		}

		public void Add(DateTime created, bool correct) {
			importance = null;
			History.Add(new KeyValuePair<DateTime, bool>(created, correct));
		}

		// Value between 0 and 1 indicating the need to practice this item.
		double Need() {
			double need = 0.9; // Starts very high.
			const int manyDays = 28;

			foreach (var kvp in History) {
				// Correct results reduce the need to practice.
				// Incorrect results increase the need to practice.
				int days = DaysAgo(kvp.Key);

				// Factor is between 0.5 (for old results) and 1 (for recent results). 
				double factor = 1.0 / (1.0 + Math.Max(Math.Min(days / (double)manyDays, 0.0), 1.0));
				//timeWeight = Math.Pow(timeWeight, 0.5);

				need = kvp.Value ? Lerp(need, 0, factor / 2) : Lerp(need, 1, factor);
			}

			// Not having any recent results increases need to practice (to a point).
			if (History.Count == 0) {
			} else {
				int days = DaysAgo(History[History.Count - 1].Key);
				double factor = Math.Min(Math.Min(days, 1) / (double)manyDays, 0.5);
				need = Lerp(need, 0.5, factor);
			}

			return need;
		}

		static int DaysAgo(DateTime d) {
			return DateTime.Now.Subtract(d).Days;
		}

		static double Lerp(double x, double y, double factor) {
			return x + (y - x) * factor;
		}

		internal string DebugText {
			get {
				return string.Format("importance {5}: {0} attempts, {1}/{2} = {3}%, most recent {4} days ago",
					History.Count,
					History.Count(kvp => kvp.Value),
					History.Count,
					History.Count == 0 ? 100 : 100 * History.Count(kvp => kvp.Value) / History.Count,
					History.Count == 0 ? 0 : DaysAgo(History[History.Count - 1].Key),
					Importance);
			}
		}
	}

	/// <summary>A circular queue of PracticeItems, which shuffles itself every time it repeats.</summary>
	public class PracticeQueue {
		readonly PracticeItem[] items;
		readonly Random random = new Random();

		public PracticeQueue(List<PracticeItem> items) {
			if (items.Count == 0)
				throw new ArgumentException("items");

			this.items = items.ToArray();
			Index = 0;
			Laps = 0;
			items.Shuffle(random);
		}

		public IList<PracticeItem> AllItems {
			get {
				return items;
			}
		}

		public PracticeItem TakeOne() {
			var item = items[Index++];
			if (Index >= items.Length) {
				items.Shuffle(random);
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