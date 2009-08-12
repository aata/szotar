using System;
using System.Windows.Forms;

namespace Szotar.WindowsForms {
	public static class Errors {
		static void ShowError(string caption, string message, params object[] inserts) {
			MessageBox.Show(string.Format(message, inserts), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static void CouldNotLoadDictionary(DictionaryInfo dict, Exception e) {
			ShowError(
				Resources.Errors.CouldNotLoadDictionaryCaption,
				Resources.Errors.CouldNotLoadDictionary,
				dict.Name);
			ProgramLog.Default.AddMessage(LogType.Error, "Recent dictionary {0} was not available: {1}", dict.Name, e.Message);
		}

		public static void NewerDatabaseVersion(Szotar.Sqlite.DatabaseVersionException e) {
			ShowError(
				Application.ProductName,
				Resources.Errors.NewerDatabaseVersion,
				Application.ProductName);
		}
	}
}