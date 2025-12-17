using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PS.Database
{
    public class MonetDatabase
    {
        private int watchAdNumAllGameLife;

        public int WATCH_AD_NUM_ALL_GAME_LIFE
        {
            get => watchAdNumAllGameLife;
            set
            {
                watchAdNumAllGameLife = value;
                Save(MonetDbKey.WATCH_AD_NUM_ALL_GAME_LIFE, value);
            }
        }

        private string uuid;

        public string UUID
        {
            get => uuid;
            set
            {
                uuid = value;
                Save(MonetDbKey.UUID, value);
            }
        }


        private bool hasShowConsentDialog;

        public bool HAS_SHOW_CONSENT_DIALOG
        {
            get => hasShowConsentDialog;
            set
            {
                hasShowConsentDialog = value;
                Save(MonetDbKey.HAS_SHOW_CONSENT_DIALOG, value);
            }
        }
        
        public MonetDatabase()
        {
            Debug.Log("Monet database initialized.");
            Init();
        }

        void Init()
        {
            ExistKey(MonetDbKey.WATCH_AD_NUM_ALL_GAME_LIFE, key => WATCH_AD_NUM_ALL_GAME_LIFE = 0);
            ExistKey(MonetDbKey.HAS_SHOW_CONSENT_DIALOG, key => HAS_SHOW_CONSENT_DIALOG = false);
            ExistKey(MonetDbKey.UUID, key =>
            {
                Guid _uuid = Guid.NewGuid();
                UUID = _uuid.ToString();
            });
            Load();
        }


        void ExistKey(string key, UnityAction<string> onComplete)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                onComplete?.Invoke(key);
            }
        }

        public void SaveAll()
        {
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

        /*T GetValue<T>(string key) where T: typeof(int)
        {
            if (typeof(T) is string)
            {
                return PlayerPrefs.GetString(key) as T ;
            }else if (typeof(T) is int)
            {
                return PlayerPrefs.GetInt(key).ToString() as T;
            }
    
            return default;
        }*/

        void Load()
        {
            watchAdNumAllGameLife = PlayerPrefs.GetInt(MonetDbKey.WATCH_AD_NUM_ALL_GAME_LIFE);
            hasShowConsentDialog = PlayerPrefs.GetInt(MonetDbKey.HAS_SHOW_CONSENT_DIALOG) == 1;
            uuid = PlayerPrefs.GetString(MonetDbKey.UUID);
        }
    }

    public class MonetDbKey
    {
        public static readonly string WATCH_AD_NUM_ALL_GAME_LIFE = "WATCH_AD_NUM_ALL_GAME_LIFE";
        public static readonly string HAS_SHOW_CONSENT_DIALOG = "HAS_SHOW_CONSENT_DIALOG";
        public static readonly string UUID = "UUID";
    }
}