using System;
using System.Collections.Generic;
using System.Resources;

namespace Szotar.WindowsForms.Preferences {
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	sealed class PreferencePageAttribute : Attribute {
		static ResourceManager defaultResourceManager = new ResourceManager(typeof(PreferencePageAttribute));

		// This is a positional argument
		public PreferencePageAttribute(string name) {
			this.Name = name;
			Importance = 0;
		}

		public Type Parent { get; set; }
		public string Name { get; private set; }
		public int Importance { get; set; }

		bool IsRoot { 
			get {
				return Parent == null;
			}
		}

		public IEnumerable<string> Path {
			get {
				var components = new List<string>();
				
				Type t = Parent;
				while (t != null) {
					PreferencePageAttribute attr = Attribute.GetCustomAttribute(t, typeof(PreferencePageAttribute)) as PreferencePageAttribute;
					System.Diagnostics.Debug.Assert(attr != null);

					components.Add(attr.Name);
					t = attr.Parent;
				}

				components.Reverse();
				return components;
			}
		}

		public IEnumerable<string> LocalisedPath {
			get {
				foreach (string s in Path) {
					yield return LocalizePathComponent(s) ?? s;
				}
			}
		}

		private string LocalizePathComponent(string name) {
			//Hmmm...
			//ResourceTable property maybe, or ResourceType
			ResourceManager specificResourceManager = null; 

			string key = "PrefPathString." + name;

			if (specificResourceManager != null) {
				try {
					string str = specificResourceManager.GetString(key);
					if (!string.IsNullOrEmpty(str))
						return str;
				} catch (MissingManifestResourceException) {
				}
			}

			try {
				return defaultResourceManager.GetString(key);
			} catch (MissingManifestResourceException) {
				return name;
			}
		}
	}

	namespace Categories {
		[PreferencePage("General", Importance=100)]
		public static class General { }

		[PreferencePage("Advanced", Importance=30)]
		public static class Advanced { }
	}
	
	public class PreferencePage : System.Windows.Forms.UserControl {
		public virtual void Commit() {
		}

		public Forms.Preferences Owner { get; set; }
	}
}