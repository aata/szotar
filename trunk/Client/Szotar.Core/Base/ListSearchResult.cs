namespace Szotar {
	/// <summary>
	/// Represents a word list or an item within a word list (basically, a search result),
	/// depending on whether or not a Phrase and Translation are listed.
	/// </summary>
	public class ListSearchResult {
		public long SetID { get; set; }

		public string Phrase { get; set; }
		public string Translation { get; set; }

		public int? PositionHint { get; set; }

		public bool HasItem { get { return Phrase != null; } } 

		public ListSearchResult(long setID) {
			SetID = setID;
		}

		public ListSearchResult(long setID, string phrase, string translation, int? positionHint = null) {
			SetID = setID;
			Phrase = phrase;
			Translation = translation;
			PositionHint = positionHint;
		}
	}
}