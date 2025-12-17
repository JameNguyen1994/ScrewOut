using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using PS.Utils;
using Storage;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static UniWebViewLogger;

namespace MainMenuBar
{
    public class DBMainMenuBarController : Singleton<DBMainMenuBarController>
    {
        [SerializeField] private MainMenuBarDataSO mainMenuBarDataSO;
        private DBBarsItem _mainMenuBarItems;

        public DBBarsItem DB_MAIN_MENU_ITEMS
        {
            get => _mainMenuBarItems;
            set
            {
                _mainMenuBarItems = value;
                Save(DBKey.DB_MAIN_MENU_ITEMS, value);
            }
        }
        protected override void CustomAwake()
        {
            base.CustomAwake();
            Initializing();
        }
        async UniTask Initializing()
        {
            await UniTask.WaitUntil(() => Db.storage.Inited);
            CheckDependency(DBKey.DB_MAIN_MENU_ITEMS, key =>
              {
                  DBBarsItem dBLands = new DBBarsItem(mainMenuBarDataSO.data);
                  DB_MAIN_MENU_ITEMS = dBLands;
              });

            Load();

            if (mainMenuBarDataSO.data.Count == DB_MAIN_MENU_ITEMS.lstDBBarItem.Count)
            {

                ResetDBSameCount();
            }



            Debug.Log($"mainMenuBarDataSO.data.Count {mainMenuBarDataSO.data.Count}");
            Debug.Log($"DB_MAIN_MENU_ITEMS.lstDBBarItem.Count {DB_MAIN_MENU_ITEMS.lstDBBarItem.Count}");
            if (mainMenuBarDataSO.data.Count != DB_MAIN_MENU_ITEMS.lstDBBarItem.Count)
                ResetDBDiffCount();
        }
        public void ResetDBSameCount()
        {

            var data = DB_MAIN_MENU_ITEMS;
            var level = Db.storage.USER_INFO.level;
            var lstIndexDiff = new List<int>();
            for (int i = 0; i < mainMenuBarDataSO.data.Count; i++)
            {
                if (mainMenuBarDataSO.data[i].levelUnlock != data.lstDBBarItem[i].levelUnlock)
                {
                    lstIndexDiff.Add(i);
                    data.lstDBBarItem[i].levelUnlock = mainMenuBarDataSO.data[i].levelUnlock;
                }
            }


            for (int i = 0; i < lstIndexDiff.Count; i++)
            {
                int index = lstIndexDiff[i];
                if (data.lstDBBarItem[i].levelUnlock < level)
                    data.lstIndexCompleted.Add(data.lstDBBarItem[i].levelUnlock);
            }
            DB_MAIN_MENU_ITEMS = data;
        }
        public void ResetDBDiffCount()
        {
            DBBarsItem data = new DBBarsItem(mainMenuBarDataSO.data);
            var lstIndexDiff = new List<int>();
            var level = Db.storage.USER_INFO.level;
            for (int i = 0; i < mainMenuBarDataSO.data.Count; i++)
            {
                lstIndexDiff.Add(i);
            }


            for (int i = 0; i < lstIndexDiff.Count; i++)
            {
                int index = lstIndexDiff[i];
                if (data.lstDBBarItem[i].levelUnlock < level)
                    data.lstIndexCompleted.Add(data.lstDBBarItem[i].levelUnlock);
            }
            DB_MAIN_MENU_ITEMS = data;
        }
        void CheckDependency(string key, UnityAction<string> onComplete)
        {
            if (!ObscuredPrefs.HasKey(key))
            {
                onComplete?.Invoke(key);
            }
        }
        void Load()
        {
            _mainMenuBarItems = LoadDataByKey<DBBarsItem>(DBKey.DB_MAIN_MENU_ITEMS);
        }
        public void Save<T>(string key, T values)
        {

            if (typeof(T) == typeof(int) ||
                typeof(T) == typeof(bool) ||
                typeof(T) == typeof(string) ||
                typeof(T) == typeof(float) ||
                typeof(T) == typeof(long) ||
                typeof(T) == typeof(Quaternion) ||
                typeof(T) == typeof(Vector2) ||
                typeof(T) == typeof(Vector3) ||
                typeof(T) == typeof(Vector2Int) ||
                typeof(T) == typeof(Vector3Int))
            {
                ObscuredPrefs.Set(key, values);
            }
            else
            {
                try
                {
                    ObscuredString json = JsonUtility.ToJson(values);
                    ObscuredPrefs.Set(key, json);
                }
                catch (UnityException e)
                {
                    throw new UnityException(e.Message);
                }
            }
        }
        public T LoadDataByKey<T>(string key)
        {
            if (typeof(T) == typeof(int) ||
                 typeof(T) == typeof(bool) ||
                 typeof(T) == typeof(string) ||
                 typeof(T) == typeof(float) ||
                 typeof(T) == typeof(long) ||
                 typeof(T) == typeof(Quaternion) ||
                 typeof(T) == typeof(Vector2) ||
                 typeof(T) == typeof(Vector3) ||
                 typeof(T) == typeof(Vector2Int) ||
                 typeof(T) == typeof(Vector3Int))
            {
                var value = ObscuredPrefs.Get<T>(key);
                return value;
            }
            else
            {
                string json = ObscuredPrefs.Get<string>(key);
                return JsonUtility.FromJson<T>(json);
            }
        }

    }
    public class DBKey
    {
        public readonly static string DB_MAIN_MENU_ITEMS = "DB_LAND";
    }
}
