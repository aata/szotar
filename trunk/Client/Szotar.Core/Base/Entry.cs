using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Szotar {
	/// <summary>
	/// Represents a translation of a phrase, supporting property binding and cloning interfaces.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{Phrase} -> {Translation}")]
	[Serializable]
	public class TranslationPair
		: INotifyPropertyChanged
		, IComparable<TranslationPair>
		, ISerializable
	{
		string phrase, translation;
		string phraseNA;

		public string Phrase {
			get { return phrase; }
			set {
				phrase = value;
				phraseNA = Searcher.RemoveAccents(phrase);
				OnPropertyChanged("Phrase");
				OnPropertyChanged("PhraseNoAccents");
			}
		}

		public string Translation {
			get { return translation; }
			set {
				translation = value;
				OnPropertyChanged("Translation");
			}
		}

		/// <summary>The phrase without any accents. This is used for speeding up the search.</summary>
		/// <remarks>Note that The translation without accents is not yet needed.</remarks>
		[Browsable(false)]
		public string PhraseNoAccents {
			get { return phraseNA; }
		}

		public TranslationPair(string phrase, string translation) {
			this.phrase = phrase;
			this.translation = translation;
			phraseNA = Searcher.RemoveAccents(phrase);
		}

		/// <summary>A default constructor for the purposes of data binding lists and such.
		/// Not really intended to be used from code which knows better.</summary>
		public TranslationPair() {
			phrase = string.Empty;
			translation = string.Empty;
			phraseNA = string.Empty;
		}

		public override string ToString() {
			return string.Format("{{\"{0}\" => \"{1}}\"}", phrase, translation);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName) {
			PropertyChangedEventHandler h = PropertyChanged;
			if (h != null)
				h(this, new PropertyChangedEventArgs(propertyName));
		}

		public int CompareTo(TranslationPair other) {
			return phrase.CompareTo(other.phrase);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("Phrase", phrase);
			info.AddValue("Translation", translation);
		}

		protected TranslationPair(SerializationInfo info, StreamingContext context) {
			phrase = info.GetValue("Phrase", typeof(string)) as string ?? string.Empty;
			translation = info.GetValue("Translation", typeof(string)) as string ?? string.Empty;
		}
	}
	
	[System.Diagnostics.DebuggerDisplay("{Value} ({PartOfSpeech})")]
	public class Translation {
		public string Value { get; set; }
		// public string PartOfSpeech { get; set; }

		public Translation(string value) {
			Value = value;
		}
	}

	public class EntryTag {
		public IDictionarySection DictionarySection { get; protected set; }
		public object Data { get; protected set; }

		public EntryTag(IDictionarySection section, object data) {
			DictionarySection = section;
			Data = data;
		}
	}

	[System.Diagnostics.DebuggerDisplay("{Phrase} -> [{Translations.Count} translations]")]
	public class Entry : IComparable<Entry>, INotifyPropertyChanged {
		string phrase;
		string phraseNA;
		IList<Translation> translations;
		EntryTag fullEntryHandle;

		public string Phrase {
			set {
				phraseNA = null;
				phrase = value;
				RaisePropertyChanged("Phrase");
				RaisePropertyChanged("PhraseNoAccents");
			}
			get {
				return phrase;
			}
		}

		protected void RaisePropertyChanged(string property) {
			PropertyChangedEventHandler temp = PropertyChanged;
			if (temp != null) {
				temp(this, new PropertyChangedEventArgs(property));
			}
		}

		[Browsable(false)]
		public EntryTag Tag {
			get { return fullEntryHandle; }
			set { 
				fullEntryHandle = value;
				RaisePropertyChanged("Tag");
			}
		}

		/// <summary>
		/// A copy of the Phrase without any accents (combining diacritic marks).
		/// </summary>
		/// <remarks>Crude performance tests indicate that this optimisation is indeed necessary. 
		/// However, it could easily increase memory usage. Searcher.RemoveAccents was changed to
		/// return the exact same string instance if no diacritics were found, so that should
		/// reduce memory usage somewhat, as many entries don't contain such marks.</remarks>
		public string PhraseNoAccents {
			get {
				return phraseNA ?? (phraseNA = Searcher.RemoveAccents(Phrase));
			}
		}

		public Entry(string phrase, IList<Translation> translations) {
			Translations = translations;
			this.phrase = phrase;
		}

		public void FullyLoad() {
			if(Tag != null)
				Tag.DictionarySection.GetFullEntry(this);
		}

		public IList<Translation> Translations {
			get {
				return translations;
			}
			set {
				translations = value;
				RaisePropertyChanged("Translations");
			}
		}

		public int CompareTo(Entry other) {
			return phrase.CompareTo(other.phrase);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public Entry Clone() {
			var clone = new Entry(Phrase, null);
			
			if(Translations != null) {
				var translations = new List<Translation>(Translations.Count);
				foreach(var t in Translations)
					translations.Add(t);
				clone.Translations = translations;
			}

			if(Tag != null) {
				var tag = new EntryTag(Tag.DictionarySection, Tag.Data);
				clone.Tag = tag;
			}

			return clone;
		}
	}	
}