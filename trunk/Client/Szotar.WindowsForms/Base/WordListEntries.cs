using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Szotar.WindowsForms {
	// The data object for dragging a set of rows from one list to another.
	public class WordListEntries {
		public WordList WordList { get; private set; }
		public IList<int> Indices { get; private set; }

		public WordListEntries(WordList wordList, IList<int> indices) {
			WordList = wordList;
			Indices = indices;
		}

		public void SetData(DataObject data) {
			data.SetData(typeof(WordListEntries), this);

			var sb = new StringBuilder();
			foreach (var item in Items) {
				AppendCSV(sb, item.Phrase);
				sb.Append(',');
				AppendCSV(sb, item.Translation);
				sb.AppendLine();
			}
			data.SetText(sb.ToString(), TextDataFormat.CommaSeparatedValue);

			sb.Length = 0;
			foreach (var item in Items)
				sb.Append(item.Phrase).Append(" -- ").AppendLine(item.Translation);
			data.SetText(sb.ToString());

			// TODO: More formats
		}

		protected void AppendCSV(StringBuilder sb, string s) {
			sb.Append('"');
			foreach (char c in s) {
				if (c == '"')
					sb.Append("\"\"");
				else
					sb.Append(c);
			}
			sb.Append('"');
		}

		protected IEnumerable<WordListEntry> Items {
			get {
				foreach (int i in Indices)
					yield return WordList[i];
			}
		}
	};
}
