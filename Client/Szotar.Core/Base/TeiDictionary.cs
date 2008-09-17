using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Xml.XPath;

//This code is rather outdated. Its only real purpose now is for importing, although it wasn't always that way.
//This should be changed to a TEI importer.
namespace Szotar {
	public class TeiSection : IDictionarySection {
		ICollection<Entry> entries;

		public TeiSection(ICollection<Entry> entries) {
			this.entries = entries;
			Name = "TEI Dictionary";
		}

		public ICollection<Entry> Entries {
			get { return entries; }
		}

		public int HeadWords {
			get { return entries.Count; }
		}

		public string Name {
			get;
			set;
		}

		public IEnumerator<Entry> GetEnumerator() {
			return entries.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return entries.GetEnumerator();
		}

		public IEnumerable<SearchResult> Search(string search, bool ignoreAccents, bool ignoreCase) {
			return Searcher.Search(this, search, ignoreAccents, ignoreCase);
		}

		void IDictionarySection.GetFullEntry(Entry stub) {
		}
	}

	public class TeiDictionary : IBilingualDictionary {
		FileStream stream;
		TeiReader reader;

		TeiSection forwardSection, backwardSection;

		bool disposed = false;

		public TeiDictionary(string fileName) {
			Path = fileName;

			stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
			reader = new TeiReader(stream);

			var forwardEntries = new List<Entry>();
			var backwardEntries = new List<Entry>();

			this.reader.GetItemHeaders(forwardEntries, backwardEntries);

			forwardSection = new TeiSection(forwardEntries);
			backwardSection = new TeiSection(backwardEntries);

			Name = "TEI Dictionary";
			Author = "TEI Author";
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!this.disposed) {
				if (disposing) {
					// Dispose managed resources.
					stream.Dispose();
					reader.Dispose();
				}
				// Release unmanaged resources.
			}
			this.disposed = true;
		}

		~TeiDictionary() {
			Dispose(false);
		}

		public void Save() {
			throw new Exception("The method or operation is not implemented.");
		}

		public IDictionarySection ForwardsSection {
			get { return forwardSection; }
		}

		public IDictionarySection ReverseSection {
			get { return backwardSection; }
		}

		public string Name {
			get;
			set;
		}

		public string Author {
			get;
			set;
		}

		public string Path {
			get;
			set;
		}

		public string FirstLanguage { get; set; }
		public string SecondLanguage { get; set; }
		public string FirstLanguageReverse { get; set; }
		public string SecondLanguageReverse { get; set; }
		public string FirstLanguageCode { get; set; }
		public string SecondLanguageCode { get; set; }
	}

	//Uh, do TEI dictionaries have two sections or one?
	internal class TeiReader : IDisposable {
		Stream stream;
		bool disposed;

		public enum FileType {
			OneWay, TwoWay
		}

		public FileType GetFileType() {
			CheckDisposed();

			using (XmlReader reader = this.CreateXmlReader(0)) {
				reader.MoveToContent();
				reader.ReadToFollowing("div0");
				if (reader.EOF)
					return FileType.OneWay;
				return FileType.TwoWay;
			}
		}

		/// <summary>
		/// Reads the item headers from the TEI markup.
		/// </summary>
		/// <param name="forwardEntries">An empty collection, which will be filled with the list of entries in the forwards section of the dictionary.</param>
		/// <param name="backwardEntries">An empty collection, which will be filled with the list of entries in the backwards section of the dictionary.</param>
		public void GetItemHeaders(ICollection<Entry> forwardEntries, ICollection<Entry> backwardEntries) {
			CheckDisposed();

			//Might not work with single-sided TEI dictionaries
			LineInfo secondSectionPosition;

			using (XmlReader reader = this.CreateXmlReader(0)) {
				reader.MoveToContent();
				IXmlLineInfo lineInfo = (IXmlLineInfo)reader;

				//Find out the line info
				reader.ReadToFollowing("div0");
				reader.ReadToFollowing("div0");
				secondSectionPosition = new LineInfo(lineInfo);
			}

			using (XmlReader reader = this.CreateXmlReader(0)) {
				reader.MoveToContent();

				while (!reader.EOF) {
					reader.ReadToFollowing("entry");

					//Determine what section we're in by the position in the file (so so hacky)
					bool forward = new LineInfo((IXmlLineInfo)reader).CompareTo(secondSectionPosition) < 0;

					var collection = forward ? forwardEntries : backwardEntries;

					Entry entryStub = ReadEntryStub(reader);
					if (entryStub != null)
						collection.Add(entryStub);
				}
			}
		}

		public TeiReader(Stream stream) {
			this.stream = stream;
		}

		private XmlReader CreateXmlReader(long readPosition) {
			CheckDisposed();

			stream.Position = 0;
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.None;
			settings.IgnoreComments = true;
			settings.IgnoreProcessingInstructions = true;
			settings.IgnoreWhitespace = true;
			settings.ProhibitDtd = false;
			settings.CloseInput = false;
			settings.CheckCharacters = false;
			settings.XmlResolver = new TeiResolver();
			return XmlReader.Create(stream, settings);
		}

		public Encoding GetEncoding() {
			using (XmlTextReader reader = new XmlTextReader(stream)) {
				reader.Read();
				return reader.Encoding;
			}
		}

		/// <remarks>
		/// Uses XmlReader.ReadSubtree to determine where the Xml fragment ends.
		/// </remarks>
		private Entry ReadEntryStub(XmlReader reader) {
			CheckDisposed();

			if (reader.NodeType == XmlNodeType.None)
				return null;

			//LineInfo lineInfo = null;
			//IXmlLineInfo lineInfoSource = reader as IXmlLineInfo;
			//if (lineInfoSource != null)
			//	lineInfo = new LineInfo(lineInfoSource.LineNumber, lineInfoSource.LinePosition);

			XmlDocument fragment = new XmlDocument();
			using (XmlReader entryReader = reader.ReadSubtree()) {
				fragment.Load(entryReader);
				while (!entryReader.EOF)
					fragment.ReadNode(entryReader);
			}

			XPathNavigator nav = fragment.CreateNavigator();

			XPathNodeIterator phrases = (XPathNodeIterator)nav.Evaluate("//orth");
			XPathNodeIterator translations = (XPathNodeIterator)nav.Evaluate("//trans//tr");

			StringBuilder phrase = new StringBuilder();

			//Possibly write JoinNodeListValues or something
			//Note: multiple <orth> forms is kinda strange
			//      I don't expect to see it but it wouldn't be a good thing for this
			foreach (XPathItem node in phrases) {
				if (phrase.Length > 0)
					phrase.Append(", ");
				phrase.Append(Sanitize(node.Value));
			}

			var list = new List<Translation>();
			foreach (XPathItem node in translations)
				list.Add(new Translation(Sanitize(node.Value)));

			Entry p = new Entry(phrase.ToString(), list);
			//p.Tag = lineInfo;
			return p;
		}

		//HACK This is totally specific to Hungarian dictionaries which are badly encoded...
		private string Sanitize(string value) {
			return value.Replace("û", "ű").Replace("Û", "Ű")
				.Replace("ô", "ő").Replace("Ô", "Ő");
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!this.disposed) {
				if (disposing) {
					stream.Dispose();
				}
				// Release unmanaged resources.
			}
			this.disposed = true;
		}

		protected void CheckDisposed() {
			if (this.disposed)
				throw new ObjectDisposedException("this");
		}

		~TeiReader() {
			Dispose(false);
		}
	}

	/// <summary>
	/// Stores a position in a file in a line number/column number format, and a position from the beginning
	/// of the file. Used to remember where a dictionary entry started.
	/// </summary>
	[DebuggerDisplay("Line {LineNumber}, Column {LinePosition}")]
	internal class LineInfo : IComparable<LineInfo> {
		int lineNumber, linePosition;
		long position;

		public long Position {
			get { return position; }
			set { position = value; }
		}

		public int LinePosition {
			get { return linePosition; }
			set { linePosition = value; }
		}

		public int LineNumber {
			get { return lineNumber; }
			set { lineNumber = value; }
		}

		public LineInfo(int line, int column) {
			lineNumber = line;
			linePosition = column;
		}

		public LineInfo(IXmlLineInfo other) {
			lineNumber = other.LineNumber;
			linePosition = other.LinePosition;
		}

		public int CompareTo(LineInfo other) {
			int i = lineNumber.CompareTo(other.LineNumber);
			if (i != 0)
				return i;
			return linePosition.CompareTo(other.LinePosition);
		}

		public override string ToString() {
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1}", lineNumber, linePosition);
		}

		public static LineInfo Parse(string str) {
			string[] parts = str.Split(',');
			if (parts.Length != 2)
				throw new FormatException("TEI offset descriptor does not contain exactly 2 parts");

			return new LineInfo(int.Parse(parts[0]), int.Parse(parts[1]));
		}
	}

	/// <summary>
	/// Pretends to fetch external entities from an XML document. Actually just returns 
	/// an empty stream.
	/// </summary>
	internal class TeiResolver : XmlResolver {
		public override System.Net.ICredentials Credentials {
			set {
			}
		}

		//Return an empty stream. This seems to work when you're not validating.
		//For now, we can't validate TEI documents at all.
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn) {
			return new MemoryStream();
		}

		public TeiResolver() {
		}
	}
}


