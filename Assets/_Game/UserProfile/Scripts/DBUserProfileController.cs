using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CodeStage.AntiCheat.Storage;
using CodeStage.AntiCheat.ObscuredTypes;

namespace UserProfile
{
    public class DBUserProfileController : Singleton<DBUserProfileController>
    {
        [SerializeField] DataUserProfileManager dataUserProfileManager;
        private UserInfoData _userInfoData;

        public UserInfoData USER_INFO_DATA
        {
            get => _userInfoData;
            set
            {
                _userInfoData = value;
                Save(DBKey.USER_INFO_DATA, value);
            }
        }
        private DBItemAvatar _itemAvatars;

        public DBItemAvatar ITEM_AVATARS
        {
            get => _itemAvatars;
            set
            {
                _itemAvatars = value;
                Save(DBKey.ITEM_AVATARS, value);
            }
        }
        private DBItemFrame _itemFrames;

        public DBItemFrame ITEM_FRAMES
        {
            get => _itemFrames;
            set
            {
                _itemFrames = value;
                Save(DBKey.ITEM_FRAMES, value);
            }
        }
        protected override void CustomAwake()
        {
            base.CustomAwake();
            Initializing();
        }
        void Initializing()
        {
            CheckDependency(DBKey.USER_INFO_DATA, key =>
            {
                UserInfoData userInfoData = new UserInfoData("2304", "Player 2304", 0, 0);
                USER_INFO_DATA = userInfoData;
            });
            CheckDependency(DBKey.ITEM_AVATARS, key =>
            {
                DBItemAvatar itemAvatars = new DBItemAvatar();
                ItemAvatarDataSO itemAvatarDataSO = dataUserProfileManager.ItemAvatarDataSO;
                if (dataUserProfileManager == null)
                {
                    Debug.Log("dataUserProfileManager == null");
                }
                if (itemAvatarDataSO == null)
                {
                    Debug.Log("itemAvatarDataSO == null");
                }
                var data = itemAvatarDataSO.data;
                if (data.Count == 0)
                {
                    Debug.Log("itemAvatarDataSO.data.Count == 0");
                }
                DBItemUserProfile itemUserProfile = new DBItemUserProfile(0, ItemState.Select);
                itemAvatars.data.Add(itemUserProfile);
                for (int i = 1; i < data.Count; i++)
                {
                    var state = ItemState.Unlock;
                    if (data[i].conditionUnlock.conditionType != ConditionType.None)
                        state = ItemState.Lock;
                    DBItemUserProfile itemUserProfilee = new DBItemUserProfile(i, state);
                    itemAvatars.data.Add(itemUserProfilee);
                }
                ITEM_AVATARS = itemAvatars;
            });
            CheckDependency(DBKey.ITEM_FRAMES, key =>
            {
                DBItemFrame itemFrames = new DBItemFrame();
                ItemFrameDataSO itemFrameDataSO = dataUserProfileManager.ItemFrameDataSO;
                if (dataUserProfileManager == null)
                {
                    Debug.Log("dataUserProfileManager == null");
                }
                if (itemFrameDataSO == null)
                {
                    Debug.Log("itemAvatarDataSO == null");
                }
                var data = itemFrameDataSO.data;
                if (data.Count == 0)
                {
                    Debug.Log("itemFrameDataSO.data.Count == 0");
                }
                DBItemUserProfile itemUserProfile = new DBItemUserProfile(0, ItemState.Select);
                itemFrames.data.Add(itemUserProfile);
                for (int i = 1; i < data.Count; i++)
                {
                    var state = ItemState.Unlock;
                    if (data[i].conditionUnlock.conditionType != ConditionType.None)
                        state = ItemState.Lock;
                    DBItemUserProfile itemUserProfilee = new DBItemUserProfile(i, state);
                    itemFrames.data.Add(itemUserProfilee);
                }
                ITEM_FRAMES = itemFrames;
            });

            Load();
            CheckDBAvatars();
            CheckDBFrames();
        }
        private void CheckDBAvatars()
        {
            DBItemAvatar itemAvatars = ITEM_AVATARS;
            ItemAvatarDataSO itemAvatarDataSO = dataUserProfileManager.ItemAvatarDataSO;
            var data = itemAvatarDataSO.data;
            if (data.Count > itemAvatars.data.Count)
            {
                int reFillCount = data.Count - ITEM_AVATARS.data.Count;
                for (int i = 0; i < reFillCount; i++)
                {
                    var state = ItemState.Unlock;
                    if (data[i].conditionUnlock.conditionType != ConditionType.None)
                        state = ItemState.Lock;
                    DBItemUserProfile itemUserProfilee = new DBItemUserProfile(i, state);
                    itemAvatars.data.Add(itemUserProfilee);
                }
                ITEM_AVATARS = itemAvatars;
            }
        }
        private void CheckDBFrames()
        {
            DBItemFrame itemFrames = ITEM_FRAMES;
            ItemFrameDataSO itemFrameDataSO = dataUserProfileManager.ItemFrameDataSO;
            var data = itemFrameDataSO.data;
            if (data.Count > itemFrames.data.Count)
            {
                int reFillCount = data.Count - ITEM_AVATARS.data.Count;
                for (int i = 0; i < reFillCount; i++)
                {
                    var state = ItemState.Unlock;
                    if (data[i].conditionUnlock.conditionType != ConditionType.None)
                        state = ItemState.Lock;
                    DBItemUserProfile itemUserProfilee = new DBItemUserProfile(i, state);
                    itemFrames.data.Add(itemUserProfilee);
                }
                ITEM_FRAMES = itemFrames;
            }
        }
        void Load()
        {
            _userInfoData = LoadDataByKey<UserInfoData>(DBKey.USER_INFO_DATA);
            _itemAvatars = LoadDataByKey<DBItemAvatar>(DBKey.ITEM_AVATARS);
            _itemFrames = LoadDataByKey<DBItemFrame>(DBKey.ITEM_FRAMES);
        }
        void CheckDependency(string key, UnityAction<string> onComplete)
        {
            if (!ObscuredPrefs.HasKey(key))
            {
                onComplete?.Invoke(key);
            }
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
        public readonly static string USER_INFO_DATA = "USER_INFO_DATA";
        public readonly static string ITEM_AVATARS = "ITEMS_AVATAR";
        public readonly static string ITEM_FRAMES = "ITEMS_FRAME";
    }
}