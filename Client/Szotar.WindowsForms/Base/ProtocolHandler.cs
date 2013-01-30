using System.Security.AccessControl;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Szotar.WindowsForms {
	public class ProtocolHandler {
		public static void Register(string scheme, string description) {
			string regKey = @"Software\Classes\" + scheme;
			var key = Registry.CurrentUser.OpenSubKey(regKey, true) ?? Registry.CurrentUser.CreateSubKey(regKey);
			if (key == null)
				return;

			using (key) {
				key.SetValue("", "URL:" + description, RegistryValueKind.String);
				key.SetValue("URL Protocol", "", RegistryValueKind.String);

				regKey = @"Shell\Open\Command";
				var cmd = key.OpenSubKey(regKey, true) ?? key.CreateSubKey(regKey);
				if (cmd == null)
					return;

				using (cmd)
					cmd.SetValue("", GetCommandPath(), RegistryValueKind.String);
			}
		}

		private static string GetCommandPath() {
			return "\"" + Application.ExecutablePath.Replace("\"", "\"\"") + "\" \"%1\"";
		}
	}
}