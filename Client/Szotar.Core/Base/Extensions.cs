using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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

    public static class LinqExtensions {
        public static void Remove<T>(this IList<T> list, Predicate<T> predicate) {
            for (int i = 0; i < list.Count; i++)
                if (predicate(list[i]))
                    list.RemoveAt(i);
        }

        public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate) {
            int i = 0;
            foreach (var item in source) {
                if (predicate(item))
                    return i;
                i++;
            }
            return -1;
        }
    }

    public static class CollectionExtensions {
        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue)) {
            TValue result;
            return dictionary.TryGetValue(key, out result) ? result : defaultValue;
        }
    }

    public static class AsyncExtensions {
        public static Task<Stream> OpenReadAsTask(this WebClient wc, Uri uri, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null) {
            var tcs = new TaskCompletionSource<Stream>(uri);
            var ctr = cancel.Register(wc.CancelAsync);

            if (progress != null)
                wc.DownloadProgressChanged += (s, e) => progress.Report(e);

            wc.OpenReadCompleted += (s, e) => {
                ctr.Dispose();
                if (e.Error != null)
                    tcs.TrySetException(e.Error);
                else if (e.Cancelled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(e.Result);
            };

            wc.OpenReadAsync(uri);
            return tcs.Task;
        }

        public static Task<string> DownloadStringAsTask(this WebClient wc, Uri uri, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null) {
            var tcs = new TaskCompletionSource<string>(uri);
            var ctr = cancel.Register(wc.CancelAsync);

            if (progress != null)
                wc.DownloadProgressChanged += (s, e) => progress.Report(e);

            wc.DownloadStringCompleted += (s, e) => {
                ctr.Dispose();
                if (e.Error != null)
                    tcs.TrySetException(e.Error);
                else if (e.Cancelled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(e.Result);
            };

            wc.DownloadStringAsync(uri, tcs);
            return tcs.Task;
        }

        public static Task<string> UploadStringAsTask(this WebClient wc, Uri uri, string method, string postData, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null) {
            var tcs = new TaskCompletionSource<string>(uri);
            var ctr = cancel.Register(wc.CancelAsync);

            if (progress != null)
                wc.UploadProgressChanged += (s, e) => progress.Report(e);

            wc.UploadStringCompleted += (s, e) => {
                ctr.Dispose();
                if (e.Error != null) tcs.TrySetException(e.Error);
                else if (e.Cancelled) tcs.TrySetCanceled();
                else tcs.TrySetResult(e.Result);
            };

            wc.UploadStringAsync(uri, method, postData, tcs);
            return tcs.Task;
        }

        public static Task StartOnUIThread(this Task task) {
            task.Start(TaskScheduler.FromCurrentSynchronizationContext());
            return task;
        }

        public static void DoOnException(this Task task, Action<Exception> action) {
            task.ContinueWith(t => action(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }
    }

    public static class DateTimeExtensions {
        private static DateTime epoch = new DateTime(1970, 1, 1);

        public static long ToUnixTime(this DateTime dt) {
            return (long)dt.Subtract(epoch).TotalSeconds;
        }

        public static DateTime DateTimeFromUnixTime(this long unixTime) {
            return epoch.AddSeconds(unixTime);
        }
    }

    public static class StringExtensions {
        public static string Format(this string str, IFormatProvider provider, params object[] inserts) {
            return string.Format(provider, str, inserts);
        }

        public static string Format(this string str, params object[] inserts) {
            return string.Format(str, inserts);
        }

        public static string FormatCurrent(this string str, params object[] inserts) {
            return string.Format(CultureInfo.CurrentCulture, str, inserts);
        }

        public static string FormatInvariant(this string str, params object[] inserts) {
            return string.Format(CultureInfo.InvariantCulture, str, inserts);
        }

        public static string FormatUI(this string str, params object[] inserts) {
            return string.Format(CultureInfo.CurrentUICulture, str, inserts);
        }
    }
}
