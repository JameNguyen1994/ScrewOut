using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UserProfile
{
    [CreateAssetMenu(fileName = "ItemFrameDataSO", menuName = "SO/UserProfile/ItemFrameDataSO")]
    public class ItemFrameDataSO : ScriptableObject
    {
        [SerializeField] List<Sprite> lstSprite;
        public List<ItemFrameData> data;

        public ItemFrameData GetItemFrameDataByID(int id)
        {
            return data.Find(x => x.id == id);
        }
        [ContextMenu("Create Data")]
        public void InitData()
        {
            for (int i = 0; i < lstSprite.Count; i++)
            {
                var item = new ItemFrameData();
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
    public class ItemFrameData
    {
        public int id;
        public Sprite sprite;
        public Color color;
        public ConditionUnlock conditionUnlock;
    }
    [System.Serializable]
    public class DBItemFrame
    {
        public List<DBItemUserProfile> data = new List<DBItemUserProfile>();
    }
}

