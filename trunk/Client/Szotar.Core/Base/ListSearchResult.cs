namespace Szotar {
	/// <summary>
	/// Represents a word list or an item within a word list (basically, a search result),
	/// depending on whether or not the Position field is defined.
	/// </summary>
	public class ListSearchResult {
		public long SetID { get; set; }
		public int? Position { get; set; }

		public ListSearchResult(long setID) {
			SetID = setID;
		}

		public ListSearchResult(long setID, int? position) {
			SetID = setID;
			Position = position;
		}
	}
}