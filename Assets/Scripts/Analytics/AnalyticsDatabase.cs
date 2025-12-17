using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PS.Analytic.Database
{
    public class AnalyticsDatabase
    {
        private bool isFirstOpen;

        public bool IS_FIRST_OPEN
        {
            get => isFirstOpen;
            set
            {
                isFirstOpen = value;
                Save(Key.IS_FIRST_OPEN, value);
            }
        }
        
        public AnalyticsDatabase()
        {
            Init();
        }

        void Init()
        {
            CheckExistKey(Key.IS_FIRST_OPEN, () => IS_FIRST_OPEN = true);
            Load();
        }

        void CheckExistKey(string key, UnityAction onCompleted)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                onCompleted?.Invoke();
            }
        }
        
        public void Save<T>(string key, T values)
        {
            if (typeof(T) == typeof(int))
            {
                var parserInt = int.Parse(values.ToString());
                PlayerPrefs.SetInt(key, parserInt);
            }
            else if (typeof(T) == typeof(string))
            {
                PlayerPrefs.SetString(key, values.ToString());
            }
            else if (typeof(T) == typeof(bool))
            {
                var parserBool = bool.Parse(values.ToString());
                PlayerPrefs.SetInt(key, parserBool ? 1 : 0);
            }
            else if (typeof(T) == typeof(float))
            {
                var parserFloat = float.Parse(values.ToString());
                PlayerPrefs.SetFloat(key, parserFloat);
            }
            else
            {
                string json = JsonUtility.ToJson(values);
                PlayerPrefs.SetString(key, json);
            }
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        void Load()
        {
            isFirstOpen = PlayerPrefs.GetInt(Key.IS_FIRST_OPEN) == 1;
        }
    }

    public class Key
    {
        public static readonly string IS_FIRST_OPEN = "IS_FIRST_OPEN";
    }
}
