using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace UserProfile
{
    [CreateAssetMenu(fileName = "ItemAvatarDataSO", menuName = "SO/UserProfile/ItemAvatarDataSO")]
    public class ItemAvatarDataSO : ScriptableObject
    {
        [SerializeField] List<Sprite> lstSprite;

        public List<ItemAvatarData> data;

        public ItemAvatarData GetItemAvatarDataByID(int id)
        {
            return data.Find(x => x.id == id);
        }
        [ContextMenu("Create Data")]
        public void InitData()
        {
            for (int i = 0; i < lstSprite.Count; i++)
            {
                var item = new ItemAvatarData();
                item.id = i;
                item.sprite = lstSprite[i];
                data.Add(item);
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
    [System.Serializable]
    public class ItemAvatarData
    {
        public int id;
        public Sprite sprite;
        public ConditionUnlock conditionUnlock;
    }
    [System.Serializable]
    public class DBItemAvatar
    {
        public List<DBItemUserProfile> data = new List<DBItemUserProfile>();
    }


    [System.Serializable]
    public class DBItemUserProfile
    {
        public int id;
        public ItemState itemState;

        public DBItemUserProfile(int id, ItemState itemState)
        {
            this.id = id;
            this.itemState = itemState;
        }
    }

    public enum ItemState
    {
        Lock = 0,
        Unlock = 1,
        Select = 2,
    }
}
