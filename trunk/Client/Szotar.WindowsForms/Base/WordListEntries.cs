using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace Szotar.WindowsForms {
	// The data object for dragging a set of rows from one list to another.
	// It must be serializable to be placed onto the clipboard.
	[Serializable]
	public class WordListEntries : ISerializable {
		public WordList WordList { get; private set; }
		public IList<WordListEntry> Items { get; private set; }
		int count;

		public IEnumerable<int> Indices { 
			get {
				if (WordList == null)
					throw new InvalidOperationException();

				foreach (var e in Items) {
					int i = WordList.IndexOf(e);
					if (i != -1)
						yield return i;
				}
			} 
		}

		public WordListEntries(WordList wordList, IList<WordListEntry> entries) {
			WordList = wordList;
			Items = entries;
			count = entries.Count;
		}

		public DataObject MakeDataObject() {
			var data = new DataObject();
			data.SetData(typeof(WordListEntries), this);

			var sb = new StringBuilder();
			foreach (var item in Items) {
				CsvUtilities.AppendCSVValue(sb, item.Phrase);
				sb.Append(',');
				CsvUtilities.AppendCSVValue(sb, item.Translation);
				sb.AppendLine();
			}
			data.SetText(sb.ToString(), TextDataFormat.CommaSeparatedValue);

			sb.Length = 0;
			foreach (var item in Items)
				sb.Append(item.Phrase).Append(" -- ").AppendLine(item.Translation);
			data.SetText(sb.ToString());

			// TODO: More formats
			return data;
		}

		public static WordListEntries FromDataObject(IDataObject data) {
			var wle = data.GetData(typeof(WordListEntries)) as WordListEntries;
			if (wle != null)
				return wle;

			var searchResults = data.GetData(typeof(TranslationPair[])) as TranslationPair[];
			if (searchResults != null) {
				var entries = new List<WordListEntry>();
				foreach (var sr in searchResults)
					entries.Add(new WordListEntry(null, sr.Phrase, sr.Translation));
				return new WordListEntries(null, entries);
			}

			// Try to parse as CSV.
			string text = Clipboard.GetText();

			if (text != null) {
				int validCSV, validTSV;
				var csv = CsvUtilities.ParseCSV(',', text, out validCSV);
				var tsv = CsvUtilities.ParseCSV('\t', text, out validTSV);
				var lines = validTSV >= validCSV ? tsv : csv;
				var entries = new List<WordListEntry>();

				foreach (var line in lines)
					if (line.Count >= 2)
						entries.Add(new WordListEntry(null, line[0], line[1]));

				return new WordListEntries(null, entries);
			}

			return null;
		}

		// Serialize by converting into a KeyValuePair<string, string>[].
		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			var list = new KeyValuePair<string, string>[Items.Count];

			for (int i = 0; i < Items.Count; ++i)
				list[i] = new KeyValuePair<string, string>(Items[i].Phrase, Items[i].Translation);

			info.AddValue("Entries", list);
		}

		protected WordListEntries(SerializationInfo info, StreamingContext context) {
			var entries = info.GetValue("Entries", typeof(KeyValuePair<string, string>[])) as KeyValuePair<string, string>[];

			if (entries != null) {
				var list = new List<WordListEntry>();
				foreach (var entry in entries)
					list.Add(new WordListEntry(null, entry.Key, entry.Value));
				Items = list;
			} else {
				Items = new WordListEntry[] { };
			}
		}
	}
}
