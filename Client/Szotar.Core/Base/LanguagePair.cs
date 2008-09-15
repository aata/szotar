using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Szotar {
	//[ResourceTableName("Asztal.Szótár.Resources.LanguagePair")]
	//[TypeDescriptionProvider(typeof(LocalisedTypeDescriptorProvider<LanguagePair>))]
	public class LanguagePair {
		NullWeakReference<IBilingualDictionary> dictionary;
		private string name;
		private string author;

		public string FirstLanguageCode { get; set; }
		public string SecondLanguageCode { get; set; }

		public string FirstLanguageReverse { get; set; }
		public string SecondLanguageReverse { get; set; }

		[Browsable(false)]
		public string DefinitionPath { get; set; }

		public string FirstLanguage { get; set; }
		public string SecondLanguage { get; set; }

		[Browsable(false)]
		public string DictionaryPath { get; set; }

		/// <summary>
		/// Gets the absolute path of the dictionary. If the dictionary path is relative, the path of the 
		/// *.languagepair.xml file is used as the base.
		/// </summary>
		[Browsable(false)]
		public string AbsoluteDictionaryPath {
			get {
				if (DictionaryPath == null)
					return null;

				if (Path.IsPathRooted(DictionaryPath))
					return DictionaryPath;
				else
					return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(this.DefinitionPath), DictionaryPath));
			}
		}

		public string Author {
			get {
				if (string.IsNullOrEmpty(author) && dictionary != null) {
					IBilingualDictionary dict = dictionary.Target;
					if(dict != null && !string.IsNullOrEmpty(dict.Author)) {
						return dict.Author;
					}
				}
				return author;
			}
			set {
				author = value;
				if (!string.IsNullOrEmpty(value) && dictionary != null) {
					IBilingualDictionary dict = dictionary.Target;
					if(dict != null && string.IsNullOrEmpty(dict.Name)) {
						dict.Author = value;
					}
				}
			}
		}

		public string Name {
			get {
				if (!string.IsNullOrEmpty(name))
					return name;

				if (dictionary != null) {
					IBilingualDictionary dict = dictionary.Target;
					if (dict != null && !string.IsNullOrEmpty(dict.Name))
						return dict.Name;
				}
				
				if(!string.IsNullOrEmpty(FirstLanguage) && !string.IsNullOrEmpty(SecondLanguage))
					return FirstLanguage + "-" + SecondLanguage;
				return string.Empty;
			}
			set {
				name = value;
				if (!string.IsNullOrEmpty(value) && dictionary != null) {
					IBilingualDictionary dict = dictionary.Target;
					if (dict != null && string.IsNullOrEmpty(dict.Name)) {
						dict.Name = value;
					}
				}
			}
		}

		public LanguagePair(IBilingualDictionary dict) {
			Author = dict.Author;
			Name = dict.Name;
		}

		public LanguagePair(string path) {
			using (Stream stream = File.OpenRead(path)) {
				XPathDocument document = new XPathDocument(stream);
				XPathNavigator navigator = document.CreateNavigator();

				DefinitionPath = path;
				Name = (string)navigator.Evaluate("string(/language-pair/name)");
				if (string.IsNullOrEmpty(Name))
					Name = null;

				FirstLanguage = (string)navigator.Evaluate("string(/language-pair/first/@name)");
				SecondLanguage = (string)navigator.Evaluate("string(/language-pair/second/@name)");

				FirstLanguageCode = (string)navigator.Evaluate("string(/language-pair/first/@code)");
				SecondLanguageCode = (string)navigator.Evaluate("string(/language-pair/second/@code)");

				FirstLanguageReverse = (string)navigator.Evaluate("string(/language-pair/first/@reverse-name)");
				SecondLanguageReverse = (string)navigator.Evaluate("string(/language-pair/second/@reverse-name)");

				Author = (string)navigator.Evaluate("string(/language-pair/author)");

				string dictPath = (string)navigator.Evaluate("string(/language-pair/dictionary)");

				if (!string.IsNullOrEmpty(dictPath))
					DictionaryPath = dictPath;
			}
		}

		public static IEnumerable<LanguagePair> GetAllLanguagePairs() {			
			foreach (FileInfo file in DataStore.CombinedDataStore.GetFiles("LangPairs", new Regex(@"\.languagepair\.xml$"), false)) {
				yield return new LanguagePair(file.FullName);
			}

			yield break;
		}

		public IBilingualDictionary GetDictionary() {
			if (dictionary != null && dictionary.Target != null)
				return dictionary.Target as IBilingualDictionary;

			if (AbsoluteDictionaryPath == null)
				throw new InvalidOperationException();

			IBilingualDictionary instance = new SimpleDictionary(AbsoluteDictionaryPath);
			dictionary = new NullWeakReference<IBilingualDictionary>(instance);
			return instance;
		}

		public void SetDictionary(IBilingualDictionary dict) {
			dictionary = new NullWeakReference<IBilingualDictionary>(dict);
		}

		#region Save
		/// <summary>
		/// Saves the language pair to the file referenced by DefinitionPath. If DefinitionPath is null,
		/// InvalidOperationException is thrown.
		/// </summary>
		public void Save() {
			string path = DefinitionPath;
			if (string.IsNullOrEmpty(path))
				throw new InvalidOperationException();

			XmlDocument doc = new XmlDocument();
			XmlNode root = doc.AppendChild(doc.CreateElement("language-pair"));
			XmlNode node;
			node = root.AppendChild(doc.CreateElement("name"));
			node.InnerText = this.Name;
			node = root.AppendChild(doc.CreateElement("author"));
			node.InnerText = this.Author;

			node = root.AppendChild(doc.CreateElement("first"));
			{
				XmlAttribute attr;
				attr = doc.CreateAttribute("name");
				attr.Value = this.FirstLanguage;
				node.Attributes.Append(attr);
				attr = doc.CreateAttribute("reverse-name");
				attr.Value = this.FirstLanguageReverse;
				node.Attributes.Append(attr);
				attr = doc.CreateAttribute("code");
				attr.Value = this.FirstLanguageCode;
				node.Attributes.Append(attr);
			}

			node = root.AppendChild(doc.CreateElement("second"));
			{
				XmlAttribute attr;
				attr = doc.CreateAttribute("name");
				attr.Value = this.SecondLanguage;
				node.Attributes.Append(attr);
				attr = doc.CreateAttribute("reverse-name");
				attr.Value = this.SecondLanguageReverse;
				node.Attributes.Append(attr);
				attr = doc.CreateAttribute("code");
				attr.Value = this.SecondLanguageCode;
				node.Attributes.Append(attr);
			}

			node = root.AppendChild(doc.CreateElement("dictionary"));
			node.InnerText = DictionaryPath;

			doc.Save(path);
		}
		#endregion
	}
}
