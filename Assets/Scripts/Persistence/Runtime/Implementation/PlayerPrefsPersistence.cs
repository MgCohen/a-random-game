using System;
using Newtonsoft.Json;
using UnityEngine;

namespace CardMatch.Persistence
{
    public class PlayerPrefsPersistence : IPersistence
    {
        private const string KeyPrefix = "CardMatch.Persistence.";

        public void Save<T>(T value, string key = default)
        {
            string resolvedKey = ResolveKey(key, typeof(T));
            string json = JsonConvert.SerializeObject(value);
            string prefKey = KeyPrefix + resolvedKey;
            PlayerPrefs.SetString(prefKey, json);
            PlayerPrefs.Save();
        }

        public T Load<T>(string key = default)
        {
            string resolvedKey = ResolveKey(key, typeof(T));
            string prefKey = KeyPrefix + resolvedKey;
            if (!PlayerPrefs.HasKey(prefKey))
            {
                return default;
            }
            string json = PlayerPrefs.GetString(prefKey);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default;
            }
        }

        public void ClearAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        private static string ResolveKey(string key, Type type)
        {
            if (!string.IsNullOrEmpty(key))
            {
                return key;
            }
            return type.FullName ?? type.Name;
        }
    }
}
