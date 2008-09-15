using System;
using System.Collections.Generic;
using System.IO;

namespace Szotar {
	public abstract class DictionaryInfo {
		public string Name { get; set; }
		public string Author { get; set; }
		public string Path { get; set; }
		public string Url { get; set; }
		
		public string[] SectionNames { get; set; }
		public string[] Languages { get; set; }
		public int[] SectionSizes { get; set; }
		
		public abstract IBilingualDictionary GetFullInstance();
	}
	
	/// <summary>
	/// Represents a read-write dictionary.
	/// </summary>
	public interface IBilingualDictionary : IDisposable {
		string Name { get; set; }
		string Author { get; set; }
		string Path { get; set; }
		
		IDictionarySection ForwardsSection { get; }
		IDictionarySection ReverseSection { get; }

		string FirstLanguage { get; set; }
		string SecondLanguage { get; set; }
		string FirstLanguageReverse { get; set; }
		string SecondLanguageReverse { get; set; }
		string FirstLanguageCode { get; set; }
		string SecondLanguageCode { get; set; }

		void Save();
	}
	
	public static class Dictionary {
		public static IEnumerable<DictionaryInfo> GetAll() {			
			foreach (FileInfo file in DataStore.CombinedDataStore.GetFiles
			         ("Dictionaries", new System.Text.RegularExpressions.Regex(@"\.dict$"), true)) 
			{
				//Only load the info section of the dictionary
				yield return new SimpleDictionary.Info(file.FullName);
			}

			yield break;
		}
	}
	
	public interface IDictionarySection : ISearchDataSource {
		int HeadWords { get; }
		string Name { get; set; }

		Entry GetFullEntry(Entry entry);
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