using System;
using System.Text;
using System.Collections.Generic;

namespace Szotar.WindowsForms.Importing.WordListImporting {
	using System.Net;
	using System.Globalization;
	using System.IO;
	using System.Xml;
	using System.ComponentModel;
	using System.Xml.XPath;

	[ImporterAttribute("Quizlet", typeof(WordList))]
	public class QuizletImporter : IImporter<WordList> {
		int? setID;
		System.Net.WebRequest request;
		IAsyncResult requestAsyncResult;
		AsyncOperation operation;
		bool disposed;

		public QuizletImporter()
			: this(null) {
		}

		public QuizletImporter(int? setID) {
			this.setID = setID;
		}

		public int? Set {
			get { return setID; }
			set { setID = value; }
		}

		public IImporterUI<WordList> CreateUI() {
			return new Controls.QuizletSetSelector(this);
		}

		public void BeginImport() {
			if (disposed)
				throw new ObjectDisposedException("quizletImporter");
			if (setID == null) {
			}

			operation = AsyncOperationManager.CreateOperation(null);

			OnProgressChanged(string.Format(CultureInfo.CurrentUICulture, Resources.Quizlet.ContactingServer, "quizlet.com"), null);
			request = System.Net.WebRequest.Create(
				new Uri(String.Format(CultureInfo.InvariantCulture, "http://quizlet.com/set/{0}/", setID)));

			requestAsyncResult = request.BeginGetResponse(new AsyncCallback(this.GotResponse), null);
		}

		public void Cancel() {
			request.Abort();
		}

		private WordList GetResult(IAsyncResult result) {
			WordList wordList = new WordList();

			try {
				WebResponse response = request.EndGetResponse(result);
				OnProgressChanged(Resources.Quizlet.ReceivedResponse, null);
				using (Stream stream = response.GetResponseStream()) {
					const string xhtmlNamespace = "http://www.w3.org/1999/xhtml";
					string xml;

					using (StreamReader sr = new StreamReader(stream))
						xml = sr.ReadToEnd();

					XmlNamespaceManager nsm = new XmlNamespaceManager(new NameTable());
					nsm.AddNamespace("html", xhtmlNamespace);
					XmlDocument doc = new XmlDocument(nsm.NameTable);
					doc.LoadXml(xml);
					XPathNavigator nav = doc.CreateNavigator();

					//Acquire the name, author and entries of the set.

					{
						object eval = nav.Evaluate("string(//html:h2[1])", nsm);
						if (eval is string) {
							object eval2 = nav.Evaluate("string(//html:h2[1]/html:span[1])", nsm);
							if (eval2 is string) {
								wordList.Name = ((string)eval).Substring(Math.Min(((string)eval).Length - 1, ((string)eval2).Length));
							} else {
								wordList.Name = (string)eval;
							}
						}
					}

					{
						//This is almost as bad as that bloody regex.
						object eval = nav.Evaluate("string(//html:h3[1]/../html:table[1]/html:tr[string(html:td[1])='Creator']/html:td[2]/html:a)", nsm);
						if (eval is string)
							wordList.Author = (string)eval;
					}

					TranslationPair pair = null;
					XPathNodeIterator iterator = nav.Select("//html:div[@id='words-normal']//html:tr/html:td", nsm);
					foreach (XPathNavigator td in iterator) {
						string value = (string)td.Evaluate("string(.)");
						if (pair == null) {
							//First TD: Search button
							pair = new TranslationPair(null, null, true);
						} else if (pair.Phrase == null) {
							//Second TD: Phrase
							pair.Phrase = value;
						} else if (pair.Translation == null) {
							//Third TD: Translation
							pair.Translation = value;
							wordList.Add(pair);
							pair = null;
						}
					}
				}
			} catch (WebException ex) {
				throw new ImportException(Resources.Quizlet.WebRequestFailed + ":\n\n" + ex.Message);
			} catch (XmlException ex) {
				throw new ImportException(Resources.Quizlet.XmlParseFailed + ":\n\n" + ex.Message);
			}

			if (setID.HasValue)
				wordList.Url = string.Format(CultureInfo.InvariantCulture, "http://quizlet.com/set/{0}/", setID.Value);

			return wordList;
		}

		private void GotResponse(IAsyncResult result) {
			try {
				OnCompleted(GetResult(result), null, false, null);
			} catch (WebException e) {
				OnCompleted(null, e, true, null);
			}
			request = null;
		}

		public event EventHandler<ImportCompletedEventArgs<WordList>> Completed;

		private void OnCompleted(WordList result, Exception exception, bool cancelled, object state) {
			operation.PostOperationCompleted(new System.Threading.SendOrPostCallback(delegate(object postState) {
				EventHandler<ImportCompletedEventArgs<WordList>> h = Completed;
				if (h != null)
					h(this, new ImportCompletedEventArgs<WordList>(result, exception, cancelled, state));
			}), null);
		}

		public event EventHandler<ProgressMessageEventArgs> ProgressChanged;

		private void OnProgressChanged(string message, int? percent) {
			operation.Post(new System.Threading.SendOrPostCallback(delegate(object postState) {
				EventHandler<ProgressMessageEventArgs> h = ProgressChanged;
				if (h != null)
					h(this, new ProgressMessageEventArgs(message, percent));
			}), null);
		}

		#region Dispose
		public void Dispose() {
			disposed = true;

			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (request != null) {
					request.Abort();
					request = null;
				}
			}
		}

		~QuizletImporter() {
			Dispose(false);
		}
		#endregion
	}
}
