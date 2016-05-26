using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Com.Pinz.Client.Module.Login.Infrastructure
{
    public class IsolatedStorageSettings
    {
        private const string FileName = "settings.cfg";
        private readonly BinaryFormatter formatter = new BinaryFormatter();
        private Dictionary<string, object> settings;

        private IsolatedStorageFile Store => Thread.GetDomain().ActivationContext != null
            ? IsolatedStorageFile.GetUserStoreForApplication()
            : IsolatedStorageFile.GetUserStoreForAssembly();

        /// <summary>
        /// Save data to storage.
        /// </summary>
        public void Save()
        {
            using (var store = Store)
            {
                using (var stream = store.OpenFile(FileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    formatter.Serialize(stream, settings);
                }
            }
        }

        /// <summary>
        /// Load data from storage or create new one.
        /// </summary>
        private Dictionary<string, object> Settings
        {
            get
            {
                if (settings != null) return settings;

                using (var store = Store)
                {
                    if (store.FileExists(FileName))
                    {
                        using (var stream = store.OpenFile(FileName, FileMode.Open, FileAccess.Read))
                        {
                            if (stream.Length == 0)
                                settings = new Dictionary<string, object>();
                            else
                                settings = (Dictionary<string, object>) formatter.Deserialize(stream);
                        }
                    }
                    else
                    {
                        settings = new Dictionary<string, object>();
                    }
                }
                return settings;
            }
        }

        /// <summary>
        /// Get string value from storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return GetValue<string>(key);
        }

        /// <summary>
        /// Get value from the storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            if (Settings.ContainsKey(key))
                return (T) settings[key];
            return defaultValue;
        }

        /// <summary>
        /// Set value to the storage - operation save needs to be called explicitly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue<T>(string key, T value)
        {
            Settings[key] = value;
        }
    }
}