using System;
using System.Collections.Generic;

namespace Szotar {
    public static class ShuffleExtension {
        public static void Shuffle<T>(this IList<T> items) {
            Shuffle(items, new Random());
        }

        public static void Shuffle<T>(this IList<T> items, Random random) {
            // Fisher-Yates shuffle
            int n = items.Count;
            while (n > 1) {
                n--;
                int k = random.Next(n + 1);
                var t = items[k];
                items[k] = items[n];
                items[n] = t;
            }
        }
    }
}
