using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Szotar {
	[Serializable]
	public class ImportException : Exception {
		public ImportException(string message)
			: base(message)
		{ }

		public ImportException() { }
	}

	#region Attributes
    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ImporterAttribute : Attribute {
        // See the attribute guidelines at 
        //  http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconusingattributeclasses.asp
        readonly string name;
        readonly Type type;

        public ImporterAttribute(string name, Type type) {
            this.name = name;
            this.type = type;
        }

        public string Name {
            get { return name; }
        }

        public Type Type {
            get { return type; }
        }
    }

    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ImporterUIAttribute : Attribute {
        // See the attribute guidelines at 
        //  http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconusingattributeclasses.asp
        readonly Type importerType;

        public ImporterUIAttribute(Type importerType) {
            this.importerType = importerType;
        }

        public Type ImporterType {
            get { return importerType; }
        }
    }

	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ImporterDescriptionAttribute : Attribute {
		// See the attribute guidelines at 
		//  http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconusingattributeclasses.asp
		readonly string description;
		readonly string resourceIdentifier;

		public ImporterDescriptionAttribute(string description) {
			this.description = description;
		}

		public ImporterDescriptionAttribute(string description, string resourceIdentifier) {
			this.description = description;
			this.resourceIdentifier = resourceIdentifier;
		}

		public string Description {
			get { return description; }
		}

		public string ResourceIdentifier {
			get { return resourceIdentifier; }
		}
	}
	#endregion

	public interface INotifyProgress {
		void SetProgressMessage(string message, int? percent);
	}

    public class ProgressMessageEventArgs : EventArgs {
        readonly string message;
        readonly int? percentage;

        public ProgressMessageEventArgs(string message, int? percentage) {
            this.message = message;
            this.percentage = percentage;
        }

        public string Message {
            get { return message; }
        }

        public int? Percentage {
            get { return percentage; }
        }
    }

	public class ImportCompletedEventArgs<T> : AsyncCompletedEventArgs {
		T importedObject;

		public ImportCompletedEventArgs(T importedObject, Exception e, bool cancelled, object state)
			: base(e, cancelled, state) {
			this.importedObject = importedObject;
		}

		public T ImportedObject {
			get {
				RaiseExceptionIfNecessary();
				return importedObject;
			}
		}
	}

	public interface IImporter<T> : IDisposable {
		IImporterUI<T> CreateUI();

		void BeginImport();
		void Cancel();

		event EventHandler<ImportCompletedEventArgs<T>> Completed;
		event EventHandler<ProgressMessageEventArgs> ProgressChanged;
	}

	public interface IImporterUI<T> {
		void Apply();
		event EventHandler Finished;

		IImporter<T> Importer { get; }
	}
}
