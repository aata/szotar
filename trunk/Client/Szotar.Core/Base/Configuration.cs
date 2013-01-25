using System;
using System.Collections.Generic;
using System.IO;

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
        /// <typeparam name="T">The type of the value to group.</typeparam>
        /// <param name="setting">The name of the setting which is to be group.</param>
        /// <param name="value">The value to group the setting to.</param>
        void Set<T>(string setting, T value);

        /// <summary>Removes a setting from the configuration store entirely.</summary>
        /// <param name="setting">The setting to remove.</param>
        void Delete(string setting);

        /// <summary><value>True</value> if the configuration has been changed since it was last synchronized with its external representation. This may never be group if the configuration is always synchronized with its external representation (such as a database-backed configuration or a file-based configuration which always saves itself)</summary>
        bool NeedsSaving { get; set; }

        /// <summary>Saves the configuration to some external place. The details of this are specific to the implementation.</summary>
        void Save();

        /// <summary>Removes all settings from the configuration.</summary>
        void Reset();

        /// <summary>Fired when a setting is changed.</summary>
        /// <remarks>Currently there's no way to cancel a setting being changed. If there is 
        /// a need for that, SettingChanging can be added without much difficulty.</remarks>
        event EventHandler<SettingChangedEventArgs> SettingChanged;
        SettingChangedWeakEventManager SettingChangedEventManager { get; }
    }

    public class SettingChangedWeakEventManager : WeakEventManager<IConfiguration, SettingChangedEventArgs> {
        public SettingChangedWeakEventManager(IConfiguration eventSource)
            : base(eventSource) {
            eventSource.SettingChanged += SettingChanged;
        }

        void SettingChanged(object _, SettingChangedEventArgs args) {
            RaiseEvent(args);
        }
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

    public class NullConfiguration : IConfiguration {
        public NullConfiguration() {
            SettingChangedEventManager = new SettingChangedWeakEventManager(this);
        }

        T IConfiguration.Get<T>(string setting) {
            return default(T);
        }

        T IConfiguration.Get<T>(string setting, T defaultValue) {
            return defaultValue;
        }

        void IConfiguration.Set<T>(string setting, T value) { }
        void IConfiguration.Delete(string setting) { }

        bool IConfiguration.NeedsSaving {
            get { return false; }
            set { }
        }

        void IConfiguration.Save() { }
        void IConfiguration.Reset() { }

        event EventHandler<SettingChangedEventArgs> IConfiguration.SettingChanged {
            add { }
            remove { }
        }

        public SettingChangedWeakEventManager SettingChangedEventManager { get; private set; }
    }

    public static class Configuration {
        public static IConfiguration Default { get; set; }

        /// <summary>
        /// Initialises the default configuration with a configuration file located 
        /// in ~/.config/Szotar/config.txt.
        /// </summary>
        static Configuration() {
            string path = Path.Combine(DefaultConfigurationFolder(), "config.txt");
            Default = new JsonConfiguration(path);
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

        public static string AccessToken {
            get { return Default.Get<string>("AccessToken"); }
            set { Default.Set("AccessToken", value); }
        }

        public static string UserName {
            get { return Default.Get<string>("UserName"); }
            set { Default.Set("UserName", value); }
        }

        public static DateTime AccessTokenExpiry {
            get { return Default.Get<DateTime>("AccessTokenExpiry"); }
            set { Default.Set("AccessTokenExpiry", value); }
        }

        public static string LoginState {
            get { return Default.Get<string>("LoginState"); }
            set { Default.Set("LoginState", value); }
        }

        public static bool SyncOnOpen {
            get { return Default.Get("SyncOnOpen", true); }
            set { Default.Set("SyncOnOpen", value); }
        }

        public static List<String> SearchHistory {
            get { return Default.Get<List<string>>("SearchHistory"); }
            set { Default.Set("SearchHistory", value); }
        }

        public static string DefaultTermLanguageCode {
            get { return Default.Get<string>("DefaultTermLanguageCode"); }
            set { Default.Set("DefaultTermLanguageCode", value); }
        }

        public static string DefaultDefinitionLanguageCode {
            get { return Default.Get<string>("DefaultDefinitionLanguageCode"); }
            set { Default.Set("DefaultDefinitionLanguageCode", value); }
        }

        public static bool ResumeSessionOnOpen {
            get { return Default.Get("ResumeSessionOnOpen", true); }
            set { Default.Set("ResumeSessionOnOpen", value); }
        }

        public static bool VerifyCertificates {
            get { return Default.Get("VerifyCertificates", true); }
            set { Default.Set("VerifyCertificates", value); }
        }

        public static string PromptWithLanguage {
            get { return Default.Get<string>("PromptWithLanguage"); }
            set { Default.Set("PromptWithLanguage", value); }
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

        public static string DictionariesFolderName {
            get { return "Dictionaries"; }
        }
    }

    /// <summary>
    /// Saves the configuration to disk as a JSON-formatted file, using IJsonConverter and IJsonConvertible
    /// to convert from .NET objects to JSON objects.
    /// </summary>
    /// <remarks>
    /// Setting values are kept in memory as JSON values, and are thus only converted to .NET objects when 
    /// it is necessary. If a JsonConvertException or InvalidCastException exception is throw during the
    /// conversion, the setting's value is treated as null. The same goes for when writing the configuration 
    /// file.
    /// </remarks>
    public class JsonConfiguration
        : IConfiguration
        , IJsonContext 
    {
        readonly JsonContext context;
        Dictionary<string, object> values;
        readonly string path;
        bool dirty;

        public JsonConfiguration(string path) {
            if (path == null)
                throw new ArgumentNullException("path");
            
            SettingChangedEventManager = new SettingChangedWeakEventManager(this);
            context = new JsonContext();
            values = new Dictionary<string, object>();
            this.path = path;

            // NB. List<T> is handled specially.
            // Perhaps Nullable<T> should be handled specially too, but there is already
            // a defaulting mechanism in Configuration anyway.

            Load();
        }

        void Load() {
            JsonDictionary dict;

            try {
                using (var sr = new StreamReader(path))
                    dict = JsonValue.Parse(sr) as JsonDictionary;
            } catch (ParseException e) {
                ProgramLog.Default.AddMessage(LogType.Error, "JSON parsing exception in configuration: {0}", e.Message);
                return;
            } catch (IOException e) {
                ProgramLog.Default.AddMessage(LogType.Warning, "Could not load configuration: {0}", e.Message);
                return;
            }

            // Why the file would contain something other than a dictionary, I don't know.
            if (dict == null) {
                ProgramLog.Default.AddMessage(LogType.Error, "JSON configuration file was not a JSON hash object!");
                Reset();
                return;
            }

            // See notes: no conversion is done here; it is done only when necessary.
            foreach (var kvp in dict.Items)
                values[kvp.Key] = kvp.Value;
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
                        json = context.ToJson(k.Value);

                        dict.Items.Add(k.Key, json);
                    } catch (JsonConvertException) {
                        //ProgramLog.Default.AddMessage(LogType.Error, "JSON conversion exception while saving {0}: {1}", k.Key, e.Message);
                    } catch (InvalidCastException) {
                        //ProgramLog.Default.AddMessage(LogType.Error, "JSON conversion exception while saving {0}: {1}", k.Key, e.Message);
                    }
                } else {
                    // The setting was never converted from JSON, so it's still in JSON form.
                    dict.Items.Add(k.Key, json);
                }
            }

            try {
                using (var sw = new StreamWriter(path)) {
                    JsonValue.Write(dict, sw);
                    sw.WriteLine();
                }
            } catch(IOException e) {
                ProgramLog.Default.AddMessage(LogType.Error, "Could not save configuration (JSON): {0}", e.Message);
            }
            dirty = false;
        }


        public T Get<T>(string setting) {
            return Get(setting, default(T));
        }

        public T Get<T>(string setting, T defaultValue) {
            lock (values) {
                // For now, values that are set, but are set to null, are treated as unset.
                // This shouldn't pose a problem, I think, even with nullable types, because
                // I don't see a scenario where they would mean anything different.
                object value;
                if (!values.TryGetValue(setting, out value) || value == null)
                    return defaultValue;

                if (!(value is JsonValue))
                    return (T)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);

                T real;
                try {
                    real = context.FromJson<T>((JsonValue)value);
                } catch (JsonConvertException) {
                    //ProgramLog.Default.AddMessage(LogType.Error, "JSON conversion exception while retrieving {0}: {1}", setting, e.Message);
                    real = default(T);
                } catch (InvalidCastException) {
                    //ProgramLog.Default.AddMessage(LogType.Error, "JSON conversion exception while retrieving {0}: {1}", setting, e.Message);
                    real = default(T);
                }

                values[setting] = real;
                return real;
            }
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
            get { return dirty; }
            set { dirty |= value; }
        }

        public void Reset() {
            lock (values) {
                values = new Dictionary<string, object>();
                NeedsSaving = true;
            }
        }

        public event EventHandler<SettingChangedEventArgs> SettingChanged;
        public SettingChangedWeakEventManager SettingChangedEventManager { get; private set; }

        protected void RaiseSettingChanged(string setting) {
            var handler = SettingChanged;
            if (handler != null)
                handler(this, new SettingChangedEventArgs(setting));
        }

        JsonValue IJsonContext.ToJson(object value) {
            return context.ToJson(value);
        }

        T IJsonContext.FromJson<T>(JsonValue json) {
            return context.FromJson<T>(json);
        }

        bool IJsonContext.RelaxedNumericConversion { get; set; }
    }
}
