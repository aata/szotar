using System;
using System.Collections.Generic;

namespace Szotar {
	public static class ShuffleExtension {
		public static void Shuffle<T>(this IList<T> items) {
			Shuffle(items, new Random());
		}

		public static void Shuffle<T>(this IList<T> items, Random random, int start, int count) {
			// Fisher-Yates shuffle
			int n = count;
			while (n > 1) {
				n--;
				int k = random.Next(n + 1);
				var t = items[start + k];
				items[start + k] = items[start + n];
				items[start + n] = t;
			}
		}

		public static void Shuffle<T>(this IList<T> items, Random random) {
			Shuffle(items, random, 0, items.Count);
		}
	}
}
