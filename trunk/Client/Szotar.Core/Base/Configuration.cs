using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Szotar {
	/// <summary>
	/// A configuration store containing settings with unique names. Example implementations are
	/// configuration files, GConf settings, Windows registry keys, etc.
	/// </summary>
	public interface IConfiguration {
		/// <summary> Gets the setting with the given name. If the setting is not found, the default value for <typeparamref name="T"/> is used.</summary>
		/// <typeparam name="T">The type to which to convert using <c>Convert.ChangeType</c> if necessary.</typeparam>
		/// <param name="setting">The name of the setting to retrieve.</param>
		/// <returns>The value of the setting, or the default value of <typeparamref name="T"/> if the setting is not found.</returns>
		T Get<T>(string setting);

		/// <summary> Gets the setting with the given name. If the setting is not found, the given default value is used.</summary>
		/// <typeparam name="T">The type to which to convert using <c>Convert.ChangeType</c> if necessary.</typeparam>
		/// <param name="setting">The name of the setting to retrieve.</param>
		/// <param name="defaultValue">The default value to be returned if the setting is not found.</param>
		/// <returns>The value of the setting, or the default value of <typeparamref name="T"/> if the setting is not found.</returns>
		T Get<T>(string setting, T defaultValue);

		/// <summary> Set the setting with the given name.</summary>
		/// <typeparam name="T">The type of the value to set.</typeparam>
		/// <param name="setting">The name of the setting which is to be set.</param>
		/// <param name="value">The value to set the setting to.</param>
		void Set<T>(string setting, T value);

		/// <summary>Removes a setting from the configuration store entirely.</summary>
		/// <param name="setting">The setting to remove.</param>
		void Delete(string setting);
		
		/// <summary><value>True</value> if the configuration has been changed since it was last synchronized with its external representation. This may never be set if the configuration is always synchronized with its external representation (such as a database-backed configuration or a file-based configuration which always saves itself)</summary>
		bool NeedsSaving { get; }

		/// <summary>Saves the configuration to some external place. The details of this are specific to the implementation.</summary>
		void Save();

		/// <summary>Removes all settings from the configuration.</summary>
		void Reset();

		/// <summary>Fired when a setting is changed.</summary>
		/// <remarks>Currently there's no way to cancel a setting being changed. If there is 
		/// a need for that, SettingChanging can be added without much difficulty.</remarks>
		event EventHandler<SettingChangedEventArgs> SettingChanged;
	}
	
	/// <summary>
	/// Specifies which setting was changed.
	/// </summary>
	public class SettingChangedEventArgs : EventArgs {
		public string SettingName { get; protected set; }
		
		public SettingChangedEventArgs(string settingName) {
			SettingName = settingName;
		}
	}
	
	public static class Configuration {
		public static IConfiguration Default {
			get; set; 
		}
		
		/// <summary>
		/// Initialises the default configuration with a configuration file located 
		/// in ~/.config/Szotar/config.xml.
		/// </summary>
		static Configuration() {
			if(Default == null) {
				string path = Path.Combine(DefaultConfigurationFolder(), "config.xml");
				Default = new FileConfiguration(path);
			} else {
				//Someone already set the default configuration.
			}
		}
		
		public static string DefaultConfigurationFolder() {
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			return Path.Combine(path, "Szotar");
		}
		
		public static void Save() {
			Default.Save();
		}

		public static void Reset() {
			Default.Reset();
		}

		public static int RecentListsSize {
			get { return Default.Get("RecentListsSize", 10); }
			set { Default.Set("RecentListsSize", value); }
		}

		public static string UserDataStore {
			get { return Default.Get("UserDataStore", DefaultConfigurationFolder()); }
			set { Default.Set("UserDataStore", value); }
		}

		public static string UserListsStore {
			get { return Default.Get("UserListsStore", Path.Combine(UserDataStore, "Lists")); }
			set { Default.Set("UserListsStore", value); }
		}

		public static ListInfo[] RecentLists {
			get { return Default.Get<ListInfo[]>("RecentLists"); }
			set { Default.Set("RecentLists", value); }
		}
	}
	
	public class FileConfiguration : IConfiguration {
		string path;
		Dictionary<string, object> settings;
		
		//TODO: raise event when necessary
		//FUTURE: watch file for changes?
		public event EventHandler<SettingChangedEventArgs> SettingChanged;
		
		protected void RaiseSettingChanged(string setting) {
			var handler = SettingChanged;
			if(handler != null)
				handler(this, new SettingChangedEventArgs(setting));
		}
		
		public FileConfiguration(string path) {
			this.path = path;
			
			Load();
		}
		
		~FileConfiguration() {
			if(NeedsSaving)
				Save();
		}
		
		public bool NeedsSaving { get; protected set; }
		
		public T Get<T>(string setting, T defaultValue) {
			object value;
			lock(settings) {
				if(settings.TryGetValue(setting, out value)) {
					return (T)Convert.ChangeType(value, typeof(T));
				}
			}
			return defaultValue;
		}
		
		public T Get<T>(string setting) {
			return Get<T>(setting, default(T));
		}
		
		public void Set<T>(string setting, T value) {
			lock(settings) {
				settings[setting] = value;
			}
			
			NeedsSaving = true;
			RaiseSettingChanged(setting);
		}
		
		public void Delete(string setting) {
			lock(settings) {
				if(settings.ContainsKey(setting)) {
					settings.Remove(setting);
				}
			}
			
			NeedsSaving = true;
			RaiseSettingChanged(setting);
		}
		
		//Annoyingly, XmlSerializer refuses to serialize anything deriving from IDictionary.
		//Thus, the settings are serialized as a List<Entry> (since KeyValuePair's Key/Value don't have a setter,
		//the Key and Value properties aren't actually serialized... how useless).
		protected XmlSerializer CreateSerializer() {
			return new System.Xml.Serialization.XmlSerializer(typeof(List<Entry>),
				new Type[] { typeof(List<MruEntry>), typeof(MruEntry), typeof(MruList) }
				);
		}
		
		public void Save() {
			XmlSerializer serializer = CreateSerializer();
			
			var pairs = new List<Entry>();
			lock(settings) {
				foreach(var kv in settings) {
					pairs.Add(new Entry { Key = kv.Key, Value = kv.Value });
				}
			}

			string tempPath = Path.GetTempFileName();

			using(StreamWriter sw = new StreamWriter(tempPath))
				serializer.Serialize(sw, pairs);

			//Ensure that the target directory exists.
			Directory.CreateDirectory(Path.GetDirectoryName(path));

			try {
				File.Copy(path, path + ".backup", true);
			} catch (FileNotFoundException) { }
			File.Delete(path);
			File.Move(tempPath, path);
			
			NeedsSaving = false;
		}
		
		protected void Load() {
			XmlSerializer serializer = CreateSerializer();
			
			settings = new Dictionary<string,object>();
			
			if(File.Exists(path)) {
				List<Entry> pairs;
				using(StreamReader sr = new StreamReader(path)) {
					pairs = (List<Entry>)serializer.Deserialize(sr);
				}
				
				foreach(var kvp in pairs) {
					if(kvp.Key != null) {
						settings.Add(kvp.Key, kvp.Value);
					}
				}
			}
			
			NeedsSaving = false;
		}

		public void Reset() {
			settings.Clear();
			Save();
		}

		[Serializable]
		public class Entry {
			public string Key { get; set; } 
			public object Value { get; set; }
		}
	}
}