using System;
using System.Windows.Forms;

namespace Szotar.WindowsForms {
	public static class Errors {
		static void ShowError(string caption, string message, params object[] inserts) {
			MessageBox.Show(string.Format(message, inserts), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// Displays a message indicating that a dictionary could not be loaded.
		/// </summary>
		/// <param name="name">The name of the dictionary, if known. May be null, in which case the path will be shown instead.</param>
		/// <param name="path">The path of the dictionary.</param>
		/// <param name="e">The exception thrown.</param>
		public static void CouldNotLoadDictionary(string name, string path, Exception e) {
			ShowError(
				Resources.Errors.CouldNotLoadDictionaryCaption,
				Resources.Errors.CouldNotLoadDictionary,
				name ?? path);
			ProgramLog.Default.AddMessage(LogType.Error, "Dictionary {0} was not available: {1}", name ?? path, e.Message);
		}

		/// <summary>
		/// Displays a message indicating that the program database was created by a 
		/// newer version of the application.
		/// </summary>
		/// <param name="e">The version exception thrown.</param>
		public static void NewerDatabaseVersion(Szotar.Sqlite.DatabaseVersionException e) {
			ShowError(
				Application.ProductName,
				Resources.Errors.NewerDatabaseVersion,
				Application.ProductName);
		}
	}
}