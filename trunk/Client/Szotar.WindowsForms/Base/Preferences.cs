using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;

namespace Szotar.WindowsForms.Preferences {
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	sealed class PreferencePageAttribute : Attribute {
		static readonly ResourceManager defaultResourceManager = Resources.PreferencePageAttribute.ResourceManager;

		public PreferencePageAttribute(string name) {
			this.Name = name;
			Importance = 0;
		}

		public Type Parent { get; set; }
		public string Name { get; private set; }
		public int Importance { get; set; }

		public bool IsRoot {
			get {
				return Parent == null;
			}
		}

		public IEnumerable<string> Path {
			get {
				var components = new List<string>();

				Type t = Parent;
				while (t != null) {
					var attr = GetCustomAttribute(t, typeof(PreferencePageAttribute)) as PreferencePageAttribute;
					Debug.Assert(attr != null);

					components.Add(attr.Name);
					t = attr.Parent;
				}

				components.Reverse();
				return components;
			}
		}

		public IEnumerable<string> LocalisedPath {
			get {
				return Path.Select(s => LocalizePathComponent(s) ?? s);
			}
		}

		public string LocalisedName {
			get { return LocalizePathComponent(Name) ?? Name; }
		}

		private static string LocalizePathComponent(string name) {
			string key = "PrefPath_" + name;

			try {
				return defaultResourceManager.GetString(key);
			} catch (MissingManifestResourceException) {
				return name;
			}
		}
	}

	namespace Categories {
		[PreferencePage("General", Importance = 100)]
		public static class General { }

		[PreferencePage("Advanced", Importance = 30)]
		public static class Advanced { }
	}

	public class PreferencePage : System.Windows.Forms.UserControl {
		public virtual void Commit() {
		}

		public Forms.Preferences Owner { get; set; }
	}
}