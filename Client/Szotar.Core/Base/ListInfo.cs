using System;
using Szotar.Json;

namespace Szotar {
	[Serializable]
	public class ListInfo : IJsonConvertible {
		public long? ID { get; set; }
		public string Name { get; set; }
		public string Author { get; set; }
		public string Language { get; set; }
		public string Url { get; set; }
		public DateTime? Date { get; set; }
		public long? TermCount { get; set; }

		public ListInfo() { }

		protected ListInfo(JsonValue value, IJsonContext context) {
			var dict = value as JsonDictionary;
			if (dict == null)
				throw new JsonConvertException("Value was not a dictionary");

			foreach (var k in dict.Items) {
				switch (k.Key) {
					case "ID":
						if (k.Value != null) {
							var n = k.Value as JsonNumber;
							if (n != null)
								ID = (long)n.LongValue;
							else
								throw new JsonConvertException("ListInfo.ID was not a number");
						}
						break;

					case "Name":
					case "Author":
					case "Language":
					case "Url":
						if (k.Value != null) {
							var s = k.Value as JsonString;
							if (s != null)
								SetStringProperty(k.Key, s.Value);
							else
								throw new JsonConvertException("ListInfo." + k.Key + " was not a number");
						}
						break;

					case "Date":
						if (k.Value != null) {
							var s = k.Value as JsonString;
							if (s != null)
								Date = DateTime.Parse(s.Value);
							else
								throw new JsonConvertException("ListInfo.Date was not a string");
						}
						break;

					case "TermCount":
						if (k.Value != null) {
							var n = k.Value as JsonNumber;
							if (n != null)
								TermCount = (long)n.LongValue;
							else
								throw new JsonConvertException("ListInfo.TermCount was not a number");
						}
						break;
				}
			}
		}

		protected void SetStringProperty(string property, string value) {
			switch (property) {
				case "Name": Name = value; break;
				case "Author": Author = value; break;
				case "Language": Language = value; break;
				case "Url": Url = value; break;
			}
		}

		JsonValue IJsonConvertible.ToJson(IJsonContext context) {
			var dict = new JsonDictionary();

			if (ID.HasValue)
				dict.Items.Add("ID", new JsonNumber(ID.Value));
			if (Name != null)
				dict.Items.Add("Name", new JsonString(Name));
			if (Author != null)
				dict.Items.Add("Author", new JsonString(Author));
			if (Language != null)
				dict.Items.Add("Language", new JsonString(Language));
			if (Url != null)
				dict.Items.Add("Url", new JsonString(Url));
			if (Date.HasValue)
				dict.Items.Add("Date", new JsonString(Date.Value.ToString("s")));
			if (TermCount.HasValue)
				dict.Items.Add("TermCount", new JsonNumber(TermCount.Value));

			return dict;
		}
	}
}