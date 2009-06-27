using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;

namespace Szotar {
	[Serializable]
	public class DictionaryInfo 
		: IKeyCompare<DictionaryInfo>
		, IXmlSerializable
	{
		public string Name { get; set; }
		public string Author { get; set; }
		public string Path { get; set; }
		public string Url { get; set; }
		
		public string[] SectionNames { get; set; }
		public string[] Languages { get; set; }
		public int[] SectionSizes { get; set; }

		int IKeyCompare<DictionaryInfo>.CompareKeyWith(DictionaryInfo b) {
			return Path.CompareTo(b.Path);
		}
		
		public Func<IBilingualDictionary> GetFullInstance { get; set; }

		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() {
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader) {
			if (reader.Read()) {
				while(reader.NodeType == XmlNodeType.Element) {
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

		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
			writer.WriteElementString("Name", Name);
			writer.WriteElementString("Author", Author);
			writer.WriteElementString("Path", Path);
			writer.WriteElementString("Url", Url);
		}
	}
	
	/// <summary>
	/// Represents a read-write dictionary.
	/// </summary>
	public interface IBilingualDictionary : IDisposable {
		string Name { get; set; }
		string Author { get; set; }
		string Path { get; set; }
		string Url { get; set; }

		DictionaryInfo Info { get; }
		
		IDictionarySection ForwardsSection { get; }
		IDictionarySection ReverseSection { get; }

		string FirstLanguage { get; set; }
		string SecondLanguage { get; set; }
		string FirstLanguageCode { get; set; }
		string SecondLanguageCode { get; set; }

		void Save();
	}
	
	public static class Dictionary {
		public static IEnumerable<DictionaryInfo> GetAll() {			
			foreach (FileInfo file in DataStore.CombinedDataStore.GetFiles
			         (Configuration.DictionariesFolderName, new System.Text.RegularExpressions.Regex(@"\.dict$"), true)) 
			{
				//Only load the info section of the dictionary
				yield return new SimpleDictionary.Info(file.FullName);
			}

			yield break;
		}
	}
	
	public interface IDictionarySection : ISearchDataSource {
		int HeadWords { get; }

		void GetFullEntry(Entry entry);
	}

	[Serializable]
	public class DictionaryLoadException : Exception {
		public DictionaryLoadException() { }
		public DictionaryLoadException(string message) : base(message) { }
		public DictionaryLoadException(string message, Exception inner) : base(message, inner) { }
		protected DictionaryLoadException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}