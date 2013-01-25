using System.Xml.Serialization;
using System.Xml;
using System;

namespace Szotar {
	[Serializable]
	public class DictionaryInfo
		: IKeyCompare<DictionaryInfo>
		, IXmlSerializable
		, IJsonConvertible 
	{
		public string Name { get; set; }
		public string Author { get; set; }
		public string Path { get; set; }
		public string Url { get; set; }

		public string[] SectionNames { get; set; }
		public string[] Languages { get; set; }
		public int[] SectionSizes { get; set; }

		public DictionaryInfo() {
			// This needs a default: for example, JSON-serialized DictionaryInfo objects might not have it.
			// Maybe it shouldn't be a delegate after all. LookupForm makes sure the dictionary is deallocated
			// after closing anyway, so it's not like the weak reference does anything.
			GetFullInstance = delegate() {
				if (Path.EndsWith(".dictx"))
					return SqliteDictionary.FromPath(Path);
				else
					return new SimpleDictionary(Path);
			};
		}

		int IKeyCompare<DictionaryInfo>.CompareKeyWith(DictionaryInfo b) {
			return Path.CompareTo(b.Path);
		}

		public Func<IBilingualDictionary> GetFullInstance { get; set; }

		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() {
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader) {
			if (reader.Read()) {
				while (reader.NodeType == XmlNodeType.Element) {
					var name = reader.Name;
					var value = reader.ReadElementContentAsString();
					switch (name) {
						case "Name": Name = value; break;
						case "Author": Author = value; break;
						case "Path": Path = value; break;
						case "Url": Url = value; break;
					}
				}

				while (reader.NodeType != XmlNodeType.EndElement)
					reader.Read();
				reader.Read();
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer) {
			writer.WriteElementString("Name", Name);
			writer.WriteElementString("Author", Author);
			writer.WriteElementString("Path", Path);
			writer.WriteElementString("Url", Url);
		}

		protected DictionaryInfo(JsonValue value, IJsonContext context)
			: this() 
		{
			var dict = value as JsonDictionary;
			if (dict == null)
				throw new JsonConvertException("Expected a JSON dictionary");

			foreach (var k in dict.Items) {
				if (k.Value != null) {
					var s = k.Value as JsonString;
					if (s == null && (k.Key == "Name" || k.Key == "Author" || k.Key == "Path" || k.Key == "Url"))
						throw new JsonConvertException("DictionaryInfo." + k.Key + ": expected a string");

					switch (k.Key) {
						case "Name": Name = s.Value; break;
						case "Author": Author = s.Value; break;
						case "Path": Path = s.Value; break;
						case "Url": Url = s.Value; break;
					}
				}
			}
		}

		JsonValue IJsonConvertible.ToJson(IJsonContext context) {
			var dict = new JsonDictionary();

			if (Name != null)
				dict.Items.Add("Name", new JsonString(Name));
			if (Author != null)
				dict.Items.Add("Author", new JsonString(Author));
			if (Path != null)
				dict.Items.Add("Path", new JsonString(Path));
			if (Url != null)
				dict.Items.Add("Url", new JsonString(Url));

			return dict;
		}
	}
}