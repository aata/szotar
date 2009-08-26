using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

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

		// The 'valid' count is the count of rows which produced two columns when parsed.
		public static List<List<string>> ParseCSV(char delim, string text, out int validCount) {
			validCount = 0;
			var lines = new List<List<string>>();
			var line = new List<string>();

			using (var reader = new StringReader(text)) {
				char last = '\0', c;
				int read;
				bool escaped = false;
				var cur = new StringBuilder();
				while ((read = reader.Read()) != -1) {
					c = (char)read;
					if (c == '"') {
						if (last == '"')
							cur.Append(c);
						else if (last == delim)
							escaped = !escaped;
						else
							cur.Append(c);
					} else if (c == '\r') {
					} else if (c == '\n') {
						if (escaped) {
							// Newlines aren't useful to us. Replace them with spaces instead.
							cur.Append(' ');
						} else {
							line.Add(cur.ToString());
							cur.Length = 0;
							lines.Add(line);
							line = new List<string>();
						}
					} else if (c == delim) {
						if (escaped) {
							cur.Append(c);
						} else {
							line.Add(cur.ToString());
							cur.Length = 0;
						}
					} else {
						cur.Append(c);
					}

					last = c;
				}

				if (cur.Length > 0)
					line.Add(cur.ToString());
			}

			if (line.Count > 0)
				lines.Add(line);

			foreach (var x in lines)
				if (x.Count >= 2)
					validCount++;

			return lines;
		}

	}
}