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
	}

	public class TypeStringTable : StringTable {
		string prefix;

		public TypeStringTable(Type type)
			: base("TypeDescriptions")
		{
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

		public StringTable(string name) {
			resourceManager = new ResourceManager("Szotar.WindowsForms.Resources.Strings." + name, System.Reflection.Assembly.GetExecutingAssembly());
		}

		public virtual string this[string stringName] {
			get { return resourceManager.GetString(stringName); }
		}
	}
}
