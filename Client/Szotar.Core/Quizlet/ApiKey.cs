namespace Szotar.Quizlet {
	public partial class QuizletApi {
		/// <summary>
		/// Used to identify the client for non-authenticated API endpoints (such as search, etc.)
		/// </summary>
		private const string clientID = "2zwdE6TQt9";

		/// <summary>
		/// Used only once when authenticating; it is necessary in order to obtain an API token.
		/// </summary>
		private const string secretKey = "Th47XLcjLDZqenNgcGagYQ";
    }
}