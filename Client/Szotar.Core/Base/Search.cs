using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Szotar {
	public interface ISearchDataSource : IEnumerable<Entry> {
		IEnumerable<SearchResult> Search(string search, bool ignoreAccents, bool ignoreCase);
	}

	public enum MatchType : byte {
		NormalMatch,
		StartMatch,
		PerfectMatch
	}

	/// <summary>
	/// Represents a result of a search. The <cref>Translation</cref> member is not generated until
	/// it is requested, and is discarded once the caller discards it. This should improve memory usage,
	/// at a slight (but ultimately unimportant) cost of execution time (say, when scrolling the list on 
	/// the lookup form) and an improvement when generating the list of search results.
	/// </summary>
	public class SearchResult {
		public SearchResult(Entry entry, MatchType matchType) {
			Entry = entry;
			MatchType = matchType;
		}

		public string Phrase {
			get {
				return Entry.Phrase;
			}
		}

		public string Translation {
			get {
				if (Entry.Translations == null)
					Entry.FullyLoad();
				return Searcher.Join(Entry.Translations);
			}
		}

		/// <remarks>This property is marked as not browsable because that is how the
		/// DataGridView builds its column list (on Windows .NET, at least).</remarks>
		[Browsable(false)]
		public MatchType MatchType {
			get;
			private set;
		}

		/// <remarks>This property is marked as not browsable because that is how the
		/// DataGridView builds its column list (on Windows .NET, at least).</remarks>
		[Browsable(false)]
		public Entry Entry { get; private set; }
	}

	struct SearchQuery {
		public string text;
		public string[] terms;
	};

	public static class Searcher {
		public static IEnumerable<SearchResult> Search(ISearchDataSource source, string search, bool ignoreAccents, bool ignoreCase) {
			if (ignoreAccents)
				search = RemoveAccents(search);

			//Split the expression into sub-queries.
			string[] queryStrings = search.Split(new[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
			if (queryStrings.Length == 0) {
				foreach (var e in source) {
					//yield return new SearchResult(p.Phrase, Join(p.Translations), MatchType.NormalMatch);
					yield return new SearchResult(e, MatchType.NormalMatch);
				}
				yield break;
			}

			var queries = new SearchQuery[queryStrings.Length];
			for (int i = 0; i < queries.Length; ++i)
				queries[i] = new SearchQuery {text = queryStrings[i]};

			//Find the terms for each query. Exit the search if any query has no terms.
			for (int i = 0; i < queries.Length; ++i) {
				queries[i].terms = queries[i].text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				if (queries[i].terms.Length == 0)
					yield break;
				for (int j = 0; j < queries[i].terms.Length; j++)
					queries[i].terms[j] = queries[i].terms[j].Trim();
			}

			//If none of the queries fail, we can yield the pair to the owner.
			foreach (var entry in source) {
				var matchType = MatchType.NormalMatch;
				string phrase = ignoreAccents ? entry.PhraseNoAccents : entry.Phrase;

				if (queries.All(q => q.terms.Any(term => Filter(term, phrase, ignoreCase, ref matchType))))
					yield return new SearchResult(entry, matchType);
			}
		}

		public static string Join(IList<Translation> list) {
			if (list == null || list.Count == 0)
				return "";

			return string.Join(", ", from tr in list select tr.Value);
		}

		/// <summary>
		/// Checks if the search term (<paramref name="term"/>) was found in the phrase (<paramref name="phrase"/>).
		/// </summary>
		/// <param name="term">The search term, to be found in <paramref name="phrase"/>.</param>
		/// <param name="phrase">The phrase or word which may contain the search term, <paramref name="term"/>.</param>
		/// <param name="ignoreCase">Specifies whether or not case should be ignored during comparisons.</param>
		/// <param name="matchType">Set to <c>MatchType.StartMatch</c> if the <paramref name="term"/> is found at 
		/// the start of <paramref name="phrase"/>, to <c>MatchType.PerfectMatch</c> for an exact match, 
		/// otherwise left untouched.</param>
		/// <returns>True if the <paramref name="term"/> was found in the phrase.</returns>
		private static bool Filter(string term, string phrase, bool ignoreCase, ref MatchType matchType) {
			// This definitely seems to improve things with large search terms
			if (term.Length > phrase.Length)
				return false;

			int index = CultureInfo.InvariantCulture.CompareInfo.IndexOf(
					phrase, term, 
					0, phrase.Length, 
					ignoreCase 
						? CompareOptions.OrdinalIgnoreCase
						: CompareOptions.Ordinal);

			if (index == 0)
				matchType = term.Length == phrase.Length ? MatchType.PerfectMatch :  MatchType.StartMatch;

			return index > -1;
		}

		/// <summary>
		/// Removes all combining diacritics belonging to the “Mark, Nonspacing” Unicode Character Category.
		/// This includes all code points from U+0300 to U+0360, and others.
		/// If no diacritics are removed, the exact same string is returned, in the interest of using less memory.
		/// </summary>
		/// <param name="str">The string from which to remove the accents.</param>
		/// <returns>The string without accents.</returns>
		public static string RemoveAccents(string str) {
			if (str == null)
				return null;

			var sb = new StringBuilder();
			bool changed = false;

			//Normalize the text into form D: (decomposed)
			foreach (char c in str.Normalize(NormalizationForm.FormD))
				if (char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
					sb.Append(c);
				else
					changed = true;

			//I can't see normalization being *needed*. But it should be done.
			return changed ? sb.ToString().Normalize() : str;
		}

		static readonly AccentTable accentTable = new AccentTable();

		// Determine if x contains y, irrespective of accented characters and standalone accents.
		public static bool Contains(string x, string y, bool ignoreCase) {
			return GetMatch(x, y, ignoreCase) != null;
		}

		public static MatchType? GetMatch(string x, string y, bool ignoreCase) {
			// return x.IndexOf(y, StringComparison.Ordinal); 

			if (string.IsNullOrEmpty(y))
				return MatchType.NormalMatch;

			// Find the first non-accent char in y
			int firstY = -1;
			for (int i = 0; i < y.Length; ++i) {
				if (!accentTable.IsAccent(y[i])) {
					firstY = i;
					break;
				}
			}

			// No actual characters in y
			if (firstY == -1)
				return MatchType.NormalMatch;

			// The first thing to do is to look for the first character of y in x.
			char searchFor = accentTable[y[firstY]];
			if (ignoreCase)
				searchFor = char.ToLower(searchFor);

			for (int i = 0; i < x.Length; ++i) {
				// Skip combining diacritics (some accented characters are made of a base character and a combining diacritic character.
				// Unicode calls it a non-spacing mark.)
				if (accentTable.IsAccent(x[i]))
					continue;

				char cx = accentTable[x[i]];
				if (ignoreCase)
					cx = char.ToLower(cx);

				if (cx == searchFor) {
					int foundAt = i;

					// Once we find it, iterate through x and y at the same time.
					// If we reach the end of y without either running off the end of x
					// or finding a non-match, x contains y.
					for (int j = firstY;;) {
						while (i < x.Length && accentTable.IsAccent(x[i]))
							++i;
						bool hasX = i < x.Length;

						while (j < y.Length && accentTable.IsAccent(y[j]))
							++j;
						if (j >= y.Length) {
							if (foundAt > 0)
								return MatchType.NormalMatch;
							return hasX ? MatchType.PerfectMatch : MatchType.StartMatch;
						}

						if (!hasX)
							return null;

						cx = x[i];
						char cy = accentTable[y[j]];
						if (ignoreCase) {
							cy = char.ToLower(cy);
							cx = char.ToLower(cx);
						}

						if (cx != cy)
							break;

						if (++i >= x.Length)
							hasX = false;
						if (++j >= y.Length) {
							if (foundAt > 0)
								return MatchType.NormalMatch;
							return !hasX ? MatchType.PerfectMatch : MatchType.StartMatch;
						}

						if (!hasX)
							return null;
					}
				}
			}

			return null;
		}

		// Speeds up converting accented characters to non-accented characteres by way of
		// a pre-computed table for the whole Basic Multilingual Plane. Characters not in
		// the BMP are not currently supported.
		class AccentTable {
			char[] bmp;
			bool[] isAccent;

			public bool IsAccent(char c) {
				if (isAccent == null)
					InitTable();

				return isAccent[c];
			}

			public char this[char index] {
				get {
					if (bmp == null)
						InitTable();

					var c = (int)index;
					return c < 65536 ? bmp[c] : index;
				}
			}

			private void InitTable() {
				Metrics.Measure("Initializing accent removal table", delegate {
					bmp = new char[65536];
					isAccent = new bool[65536];

					for (int i = 0; i < 65536; ++i) {
						var c = (char)i;

						var category = char.GetUnicodeCategory(c);
						if (category == UnicodeCategory.NonSpacingMark) {
							isAccent[i] = true;
							continue;
						}

						if (category == UnicodeCategory.Surrogate)
							continue;

						// Normalize() throws exceptions on some characters because they aren't valid on their own.
						try {
							string denorm = new string(c, 1).Normalize(NormalizationForm.FormD);

							foreach (char x in denorm) {
								if (char.GetUnicodeCategory(x) != UnicodeCategory.NonSpacingMark) {
									bmp[i] = x;
									break;
								}
							}
						} catch (ArgumentException) {
							// This character isn't a valid code point on its own
						}
					}
				});

				Debug.Assert(bmp != null);
				Debug.Assert(isAccent != null);
			}
		}
	}
}