using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.ComponentModel;

namespace Szotar.WindowsForms {
	public class LocalizationProvider : Szotar.LocalizationProvider {
		public override IStringTable GetTypeDescriptionStringTable(Type type) {
			return new TypeStringTable(type);
		}

		public override IStringTable ErrorStringTable {
			get { return new StringTable("Errors"); }
		}

		public override IStringTable Strings {
			get { return new StringTable(Properties.Resources.ResourceManager); }
		}

        public override IStringTable GetStringTable(string tableName) {
            return new StringTable(tableName);
        }
	}

	public class TypeStringTable : StringTable {
		string prefix;

		public TypeStringTable(Type type)
			: base("TypeDescriptions") {
			prefix = type.Name + "$";
		}

		public override string this[string stringName] {
			get {
				return base[prefix + stringName];
			}
		}
	}

	public class StringTable : IStringTable {
		ResourceManager resourceManager;

		public StringTable(ResourceManager resourceManager) {
			this.resourceManager = resourceManager;
		}

		public StringTable(string name) {
			resourceManager = new ResourceManager("Szotar.WindowsForms.Resources." + name, System.Reflection.Assembly.GetExecutingAssembly());
		}

		public virtual string this[string stringName] {
			get { return resourceManager.GetString(stringName); }
		}
	}
}
