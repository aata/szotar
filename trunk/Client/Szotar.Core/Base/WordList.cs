using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace Szotar {
	public interface IWordListEditor {
		void BringToFront();
		void Close();
	}
	
	//TODO: Ascertain which members are necessary at this level.
	/// <summary>
	/// Supports all metadata pertaining to a wordlist, but demands no specific	storage policy.
	/// Hence, there is no 'Path' member of IWordList, because that is specific to file-based storage.
	/// </summary>
	public interface IWordList : INotifyPropertyChanged {
		string Author { get; set; }
		string Name { get; set; }
		string Url { get; set; }
		IWordListEditor CurrentEditor { get; set; }
	}
	
	public abstract class BaseWordList : List<TranslationPair>, IWordList {
		string name, author, url;
		IList<string> tags;
		NullWeakReference<IWordListEditor> currentEditor;
		
		//TODO: Constructors accepting IEnumerable<TranslationPair>?
		public BaseWordList() {
			name = "Untitled List"; //;Properties.Resources.DefaultListName;
			author = "User Real Name"; //Properties.Settings.Default.UserRealName;
			if (string.IsNullOrEmpty(author))
				author = "User Nickname"; //Properties.Settings.Default.UserNickname;
			if (string.IsNullOrEmpty(author))
				author = Environment.UserName;
		}
		
		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string property) {
			PropertyChangedEventHandler temp = PropertyChanged;
			if (temp != null) {
				temp(this, new PropertyChangedEventArgs(property));
			}
		}
		
		public void Add(string phrase, string translation) {
			base.Add(new TranslationPair(phrase, translation, true));
		}

		public string Name {
			get { return name; }
			set {
				name = value;
				RaisePropertyChanged("Name");
			}
		}

		public string Author {
			get { return author; }
			set {
				author = value;
				RaisePropertyChanged("Author");
			}
		}

		public string Url {
			get { return url; }
			set {
				url = value;
				RaisePropertyChanged("Url");
			}
		}

		public IList<string> Tags {
			get { return tags; }
			set {
				tags = value;
				RaisePropertyChanged("Tags");
			}
		}

		public IWordListEditor CurrentEditor {
			get {
				return currentEditor.Target;
			}
			set {
				currentEditor = new NullWeakReference<IWordListEditor>(value);
				RaisePropertyChanged("CurrentEditor");
			}
		}

		public IWordListEditor OpenEditor() {
			var lb = currentEditor.Target;
			if (lb != null) {
				lb.BringToFront();
				return lb;
			} else {
				//TODO: some way of obtaining an IWordListEditor for this instance
				return null;
			}
		}
	}
	
	/// <summary>
	/// The default word list, using a file-based storage mechanism.
	/// </summary>
	public class WordList : BaseWordList, INotifyPropertyChanged {
		private string path;

		public WordList(string path)
			: this() {
			Path = path;
			using (FileStream fs = File.OpenRead(path)) {
				Load(fs);
			}
		}

		public WordList(Stream stream)
			: this() {
			Load(stream);
		}

		public WordList() {
		}

		//Is this method necessary?
		//Seems wrong to have UI code at this level.
		public void Save() {
			if (Path != null) {
				Save(Path);
			} else {
				//TODO: Move this part elsewhere, throw an exception here.
				
				throw new NotImplementedException();
				/*System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					Path = dialog.FileName;
					Save(Path);
				}*/
			}
		}

		protected void Save(Stream stream) {
			using (var writer = new StreamWriter(stream)) {
				writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "info {0} {1}", Uri.EscapeDataString(this.Name), this.Count));
				if (!string.IsNullOrEmpty(Author))
					writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "author {0}", Uri.EscapeDataString(this.Author)));
				if (!string.IsNullOrEmpty(Url))
					writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "url {0}", Uri.EscapeDataString(this.Url)));

				foreach (TranslationPair pair in this) {
					writer.Write("e ");
					writer.Write(Uri.EscapeDataString(pair.Phrase));
					writer.Write(' ');
					writer.Write(Uri.EscapeDataString(pair.Translation));
					writer.WriteLine();
				}
			}
		}

		protected void Load(Stream stream) {
			using (var reader = new StreamReader(stream)) {
				while (!reader.EndOfStream) {
					string[] bits = reader.ReadLine().Split(' ');
					for (int i = 0; i < bits.Length; ++i)
						bits[i] = Uri.UnescapeDataString(bits[i]);

					if (bits.Length > 0) {
						switch (bits[0]) {
							case "e":
								Add(bits[1], bits[2]);
								break;

							case "info":
								Name = bits[1];
								//TODO: support info section
								break;

							case "meta":
							case "author":
								Author = bits[1];
								break;

							case "url":
								Url = bits[1];
								break;

							default:
								//Unknown or comment; ignore
								break;
						}
					}
				}
			}
		}

		public void Save(string path) {
			using (Stream stream = File.Open(path, FileMode.Create, FileAccess.Write)) {
				Save(stream);
			}
		}

		//It's not the end of the world if the original List<T> version of this
		//gets called, but I'd prefer this one did.
		public new void Add(TranslationPair pair) {
			if (pair == null)
				throw new ArgumentNullException("pair");
			if (pair.Mutable == false)
				throw new ArgumentException("TranslationPairs in a WordList must be mutable.", "pair");
			base.Add(pair);
		}

		public string Path {
			get { return path; }
			set {
				bool hasPathChanged = (path != null && value == null) || (path == null && value != null);
				path = value;
				RaisePropertyChanged("Path");
				if (hasPathChanged)
					RaisePropertyChanged("HasPath");
			}
		}

		public bool HasPath {
			get { return Path != null; }
		}

		public ListInfo GetInfo() {
			return new ListInfo(Path, Name);
		}
	}
}