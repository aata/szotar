using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Szotar.Json;
using System.Reflection;

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
		bool NeedsSaving { get; set; }

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
		/// in ~/.config/Szotar/config.txt.
		/// </summary>
		static Configuration() {
			if(Default == null) {
				string path = Path.Combine(DefaultConfigurationFolder(), "config.txt");
				Default = new JsonConfiguration(path);
			} else {
				// Someone already set the default configuration.
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

		public static List<ListInfo> RecentLists {
			get { return Default.Get<List<ListInfo>>("RecentLists", null); }
			set { Default.Set("RecentLists", value); }
		}

		public static int RecentListsSize {
			get { return Default.Get<int>("RecentListsSize", 10); }
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
		
		public static string DictionariesFolderName {
			get { return "Dictionaries"; } 
		}
	}
	
	public class FileConfiguration : IConfiguration {
		string path;
		Dictionary<string, object> settings;
		bool dirty = false;
		
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

		public bool NeedsSaving {
			get {
				return dirty;
			}
			set {
				dirty |= value;
			}
		}
		
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
			return new System.Xml.Serialization.XmlSerializer(
				typeof(List<Entry>),
				new Type[] { 
					typeof(DictionaryInfo),
					typeof(List<DictionaryInfo>),
					typeof(MruList<DictionaryInfo>),
					typeof(ListInfo),
					typeof(List<ListInfo>) }
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

	/// <summary>
	/// An object which can be converted to JSON or constructed from a JSON value.
	/// </summary>
	/// <remarks>
	/// The construction from a JSON value is implemented by a constructor, which C#'s
	/// interfaces cannot express. However, that constructor must be present, or the 
	/// system will fail.
	/// </remarks>
	public interface IJsonConvertible {
		/// <summary>
		/// Converts the object to a JSON value.
		/// </summary>
		/// <param name="context">The context in which to convert (the set of system converters).</param>
		/// <remarks>This should not throw exceptions, but it may happen.</remarks>
		JsonValue ToJson(IJsonContext context);
	}

	/// <summary>
	/// Capable of converting everything into or from a JSON object, provided it knows
	/// what type of object it is dealing with.
	/// </summary>
	/// <remarks>
	/// The JSON context is an abstraction of the set of system converters employed by the 
	/// JsonConfiguration class to implement IJsonConvertible for system types.
	/// </remarks>
	public interface IJsonContext {
		/// <summary>
		/// Convert some .NET object to JSON.
		/// </summary>
		/// <exception cref="JsonConvertException">The object could not be converted to JSON, perhaps because no converter was found.</exception>
		/// <exception cref="InvalidCastException">The passed object was of the wrong type for the converter which was found (probably an internal error).</exception>
		JsonValue ToJson(object value);

		/// <summary>
		/// Convert a JSON value to a specific type of .NET object.
		/// </summary>
		/// <exception cref="JsonConvertException">The object could not be converted to JSON, perhaps because no converter was found.</exception>
		/// <exception cref="InvalidCastException">The passed object was of the wrong type for the converter which was found.</exception>
		T FromJson<T>(JsonValue json);
	}

	/// <summary>
	/// A specific converter than only converts to or from one .NET type.
	/// </summary>
	public interface IJsonConverter {
		/// <summary>
		/// Converts an object to a JSON value.
		/// </summary>
		/// <param name="context">The context in which to convert (the set of system converters).</param>
		/// <exception cref="JsonConvertException">The object could not be converted to JSON, perhaps because it was of the wrong type.</exception>
		/// <exception cref="InvalidCastException">The passed object was of the wrong type.</exception>
		JsonValue ToJson(object value, IJsonContext context);

		/// <summary>
		/// Converts a JSON value to a specific type of JSON object.
		/// </summary>
		/// <param name="context">The context in which to convert (the set of system converters).</param>
		/// <exception cref="JsonConvertException">The object could not be converted to JSON, perhaps because it was the wrong type of JSON primitive.</exception>
		/// <exception cref="InvalidCastException">The passed object was of the wrong type.</exception>
		object FromJson(JsonValue json, IJsonContext context);
	}

	[Serializable]
	public class JsonConvertException : Exception {
		public JsonConvertException() { }
		public JsonConvertException(string message) : base(message) { }
		public JsonConvertException(string message, Exception inner) : base(message, inner) { }
		protected JsonConvertException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// Saves the configuration to disk as a JSON-formatted file, using IJsonConverter and IJsonConvertible
	/// to convert from .NET objects to JSON objects.
	/// </summary>
	/// <remarks>
	/// Converters for system types (which are not modifiable by the program) cannot be created by
	/// implementing IJsonConvertible, so we must implement a system of handlers for system types.
	/// It's a shame C# doesn't have type classes.
	/// 
	/// There are converters for: bool, int, double, float, long, short and string. There is also
	/// a special case for List&lt;T&gt;. It would be nice to have instances for all IList&lt;T&gt;-
	/// and IDictionary&lt;T&gt;-derived classes (obviously, in these cases, the class would have to 
	/// be default-constructible).
	/// 
	/// Setting values are kept in memory as JSON values, and are thus only converted to .NET objects when 
	/// it is necessary. If a JsonConvertException or InvalidCastException exception is throw during the
	/// conversion, the setting's value is treated as null. The same goes for when writing the configuration 
	/// file.
	/// </remarks>
	public class JsonConfiguration 
		: IConfiguration 
		, IJsonContext
	{
		Dictionary<Type, IJsonConverter> converters;
		Dictionary<string, object> values;
		string path;
		bool dirty = false;

		public JsonConfiguration(string path) {
			converters = new Dictionary<Type, IJsonConverter>();
			values = new Dictionary<string, object>();
			this.path = path;

			// The JsonIntegralConverter uses the LongValue, the JsonFloatingConverter uses
			// the DoubleValue.
			converters.Add(typeof(string), new JsonStringConverter());
			converters.Add(typeof(double), new JsonFloatingConverter<double>());
			converters.Add(typeof(float), new JsonFloatingConverter<float>());
			converters.Add(typeof(int), new JsonIntegralConverter<int>());
			converters.Add(typeof(long), new JsonIntegralConverter<long>());
			converters.Add(typeof(short), new JsonIntegralConverter<short>());
			converters.Add(typeof(byte), new JsonIntegralConverter<byte>());
			converters.Add(typeof(bool), new JsonBoolConverter());

			// NB. List<T> is handled specially.
			// Perhaps Nullable<T> should be handled specially too, but there is already
			// a defaulting mechanism in Configuration anyway.

			Load();
		}

		private void Load() {
			if (File.Exists(path)) {
				JsonDictionary dict;

				using (var sr = new StreamReader(path))
					dict = JsonValue.Parse(sr) as JsonDictionary;

				if (dict == null) {
					Reset();
					return;
				}

				// See notes: no conversion is done here; it is done only when necessary.
				foreach (var kvp in dict.Items)
					values[kvp.Key] = kvp.Value;
			}
		}

		public void Save() {
			if (!dirty)
				return;

			var dict = new JsonDictionary();
			foreach (var k in values) {
				// NB. null is a valid JsonValue too.
				var json = k.Value as JsonValue;
				if (json == null && k.Value != null) {
					try {
						json = GetConverter(k.Value.GetType()).ToJson(k.Value, this);

						dict.Items.Add(k.Key, json);
					} catch (JsonConvertException) {
						// TODO: Log exception
					} catch (InvalidCastException) {
						// TODO: Log exception
					}
				}
			}

			using (var sw = new StreamWriter(path)) {
				JsonValue.Write(dict, sw);
				sw.WriteLine();
			}

			dirty = false;
		}

		JsonValue IJsonContext.ToJson(object value) {
			return GetConverter(value.GetType()).ToJson(value, this);
		}

		T IJsonContext.FromJson<T>(JsonValue json) {
		    return (T)GetConverter<T>().FromJson(json, this);
		}

		public T Get<T>(string setting) {
			return Get(setting, default(T));
		}

		public T Get<T>(string setting, T defaultValue) {
			object value;
			lock (values) {
				// For now, values that are set, but are set to null, are treated as unset.
				// This shouldn't pose a problem, I think, even with nullable types, because
				// I don't see a scenario where they would mean anything different.
				if (values.TryGetValue(setting, out value) && value != null) {
					if(value is JsonValue) {
						T real;
						try {
							real = (T)GetConverter<T>().FromJson((JsonValue)value, this);
						} catch (JsonConvertException) {
							// TODO: Log exception
							real = default(T);
						} catch (InvalidCastException) {
							// TODO: Log exception
							real = default(T);
						}
						values[setting] = real;
						return real;
					} else {
						return (T)Convert.ChangeType(value, typeof(T));
					}
				} else {
					return defaultValue;
				}
			}			
		}

		protected IJsonConverter GetConverter<T>() {
			return GetConverter(typeof(T));
		}

		// Converts an object to and from JSON by using its IJsonConvertible instance.
		class ReflectionConverter : IJsonConverter {
			Type type;

			public ReflectionConverter(Type type) {
				// It is not known yet if the type even implements IJsonConvertible.
				// If it does not, a conversion exception will be thrown.
				this.type = type;
			}

			public JsonValue ToJson(object value, IJsonContext context) {
				if (value == null)
					return null;

				var c = value as IJsonConvertible;
				if(c != null)
					return c.ToJson(context);

				throw new JsonConvertException("There is no converter registered for the type " + type.Namespace + "." + type.Name + " and it does not implement IJsonConvertible");
			}

			public object FromJson(JsonValue json, IJsonContext context) {
				var c = type.GetConstructor(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
					null,
					new[] { typeof(JsonValue), typeof(IJsonContext) },
					null
					);

				if (c != null)
					return c.Invoke(new object[] { json, context });

				throw new JsonConvertException("There is no converter registered for the type " + type.Namespace + "." + type.Name + " and it does not have a constructor taking a JsonValue and an IJsonContext");
			}
		}

		protected IJsonConverter GetConverter(Type type) {
			IJsonConverter converter;
			if (converters.TryGetValue(type, out converter))
				return converter;

			// A special case for List<T>.
			if (type.IsGenericType) {
				if (type.GetGenericTypeDefinition() == typeof(List<>)) {
					var elemT = type.GetGenericArguments()[0];
					var convT = typeof(JsonListConverter<>).MakeGenericType(elemT);
					return (IJsonConverter)Activator.CreateInstance(convT);
				}
			}

			return new ReflectionConverter(type);
		}

		public void Set<T>(string setting, T value) {
			lock (values) {
				values[setting] = value;
				NeedsSaving = true;
			}

			RaiseSettingChanged(setting);
		}

		public void Delete(string setting) {
			lock (values) {
				values.Remove(setting);
				NeedsSaving = true;
			}
		}

		public bool NeedsSaving {
			get { return dirty;	}
			set { dirty |= value; }
		}

		public void Reset() {
			lock (values) {
				values = new Dictionary<string, object>();
				NeedsSaving = true;
			}
		}

		public event EventHandler<SettingChangedEventArgs> SettingChanged;

		protected void RaiseSettingChanged(string setting) {
			var handler = SettingChanged;
			if (handler != null)
				handler(this, new SettingChangedEventArgs(setting));
		}

		public class JsonBoolConverter : IJsonConverter {
			public JsonValue ToJson(object value, IJsonContext context) {
				return new JsonBool(Convert.ToBoolean(value));
			}

			public object FromJson(JsonValue value, IJsonContext context) {
				var b = value as JsonBool;
				if (b != null)
					return b.Value;

				throw new JsonConvertException("Value was not a boolean"); 
			}
		}

		public class JsonFloatingConverter<T> : IJsonConverter {
			public JsonValue ToJson(object value, IJsonContext context) {
				return new JsonNumber(Convert.ToDouble(value));
			}

			public object FromJson(JsonValue value, IJsonContext context) {
				var n = value as JsonNumber;
				if (n != null)
					return Convert.ChangeType(n.DoubleValue, typeof(T));
				else
					throw new JsonConvertException("Value was not a number");
			}
		}

		public class JsonIntegralConverter<T> : IJsonConverter {
			public JsonValue ToJson(object value, IJsonContext context) {
				return new JsonNumber(Convert.ToDouble(value));
			}

			public object FromJson(JsonValue value, IJsonContext context) {
				var n = value as JsonNumber;
				if (n != null)
					return Convert.ChangeType(n.LongValue, typeof(T));
				else
					throw new JsonConvertException("Value was not a number");
			}
		}

		public class JsonStringConverter : IJsonConverter {
			public JsonValue ToJson(object value, IJsonContext context) {
				return new JsonString(Convert.ToString(value));
			}

			public object FromJson(JsonValue json, IJsonContext context) {
				var s = json as JsonString;
				if (s != null)
					return s.Value;
				else
					throw new JsonConvertException("Value was not a string");
			}
		}

		public class JsonListConverter<T> : IJsonConverter {
			public JsonValue ToJson(object value, IJsonContext context) {
				var list = value as List<T>;
				if (list == null)
					throw new JsonConvertException("Value was not a List<" + typeof(T).ToString() + ">");

				var array = new JsonArray();
				foreach (var item in list)
					array.Items.Add(context.ToJson(item));

				return array;
			}

			public object FromJson(JsonValue json, IJsonContext context) {
				var array = json as JsonArray;
				if (array == null)
					throw new JsonConvertException("Value was not an array");

				var list = new List<T>();
				foreach (var item in array.Items)
					list.Add(context.FromJson<T>(item));

				return list;
			}
		}
	}
}