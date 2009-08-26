using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Runtime.Serialization;

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
			string[] queryStrings = search.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
			if (queryStrings.Length == 0) {
				foreach (Entry e in source) {
					//yield return new SearchResult(p.Phrase, Join(p.Translations), MatchType.NormalMatch);
					yield return new SearchResult(e, MatchType.NormalMatch);
				}
				yield break;
			}

			SearchQuery[] queries = new SearchQuery[queryStrings.Length];
			for (int i = 0; i < queries.Length; ++i) {
				queries[i] = new SearchQuery();
				queries[i].text = queryStrings[i];
			}

			//Find the terms for each query. Exit the search if any query has no terms.
			for (int i = 0; i < queries.Length; ++i) {
				queries[i].terms = queries[i].text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				if (queries[i].terms.Length == 0)
					yield break;
				for (int j = 0; j < queries[i].terms.Length; j++)
					queries[i].terms[j] = queries[i].terms[j].Trim();
			}

			//If none of the queries fail, we can yield the pair to the owner.
			foreach (Entry entry in source) {
				bool aQueryFailed = false;
				MatchType matchType = MatchType.NormalMatch;

				string phrase = ignoreAccents ? entry.PhraseNoAccents : entry.Phrase;

				foreach (SearchQuery query in queries) {
					bool matched = false;

					//If it matches any of the terms in this query, it has passed this query.
					//Should the final query should determine whether the match is desirable?
					//Currently this is not the behaviour exhibited.
					foreach (string term in query.terms) {
						if (Filter(term, phrase, ignoreCase, ref matchType)) {
							matched = true;
							break;
						}
					}

					if (!matched) {
						aQueryFailed = true;
						break;
					}
				}

				if (!aQueryFailed)
					yield return new SearchResult(entry, matchType);
			}
		}

		public static string Join(IList<Translation> list) {
			if (list == null || list.Count == 0)
				return string.Empty;

			var sb = new StringBuilder();
			foreach (Translation tr in list) {
				if (sb.Length > 0)
					sb.Append(", ");
				sb.Append(tr.Value);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Checks if the search term (<paramref name="term"/>) was found in the phrase (<paramref name="phrase"/>).
		/// </summary>
		/// <param name="term">The search term, to be found in <paramref name="phrase"/>.</param>
		/// <param name="str">The phrase or word which may contain the search term, <paramref name="term"/>.</param>
		/// <param name="ignoreAccents">Specifies whether or not accents should be ignored during comparisons.</param>
		/// <param name="ignoreCase">Specifies whether or not case should be ignored during comparisons.</param>
		/// <param name="stringStartedWithTerm">Set to true if the <paramref name="term"/> is found at the start of <paramref name="phrase"/>, otherwise left untouched.</param>
		/// <returns>True if the <paramref name="term"/> was found in the phrase.</returns>
		private static bool Filter(string term, string phrase, bool ignoreCase, ref MatchType matchType) {
			//This definitely seems to improve things with large search terms
			if (term.Length > phrase.Length)
				return false;

			int index = System.Globalization.CultureInfo.InvariantCulture.CompareInfo.IndexOf(phrase, term, 0, phrase.Length, ignoreCase ? System.Globalization.CompareOptions.OrdinalIgnoreCase : System.Globalization.CompareOptions.Ordinal);
			if (index == 0)
				matchType = MatchType.StartMatch;

			if (index == 0 && term.Length == phrase.Length)
				matchType = MatchType.PerfectMatch;

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

			StringBuilder sb = new StringBuilder();
			bool changed = false;

			//Normalize the text into form D: (decomposed)
			foreach (char c in str.Normalize(NormalizationForm.FormD))
				if (char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
					sb.Append(c);
				else
					changed = true;

			if (changed)
				return sb.ToString().Normalize(); //I can't see normalization being *needed*. But it should be done.

			return str;
		}
	}
}