using System;
using System.Text;

namespace Szotar {
	public static class CsvUtilities {
		public static void AppendCSVValue(StringBuilder sb, string value) {
			sb.Append('"');
			foreach (char c in value) {
				if (c == '"')
					sb.Append("\"\"");
				else
					sb.Append(c);
			}
			sb.Append('"');
		}
	}
}