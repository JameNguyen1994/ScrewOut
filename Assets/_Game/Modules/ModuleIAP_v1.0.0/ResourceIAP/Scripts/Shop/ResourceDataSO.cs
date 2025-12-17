using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceDataSO", menuName = "SO/ResourceDataSO", order = 1)]
public class ResourceDataSO : ScriptableObject
{
    public List<IAPItemData> data = new List<IAPItemData>();

    [Header("CreateItem")]
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private List<int> lstValue;
    [SerializeField] private List<string> lstKey;
    [SerializeField] private List<int> lstSale;

    [ContextMenu("CreateItem")]
    public void InitItem()
    {
        data.Clear();
        for (int i = 0; i < lstKey.Count; i++)
        {
            string key = lstKey[i];
            var value = lstValue[i];
            IAPItemData newItem = new IAPItemData
            {
                iapKey = key,
                data = new List<ResourceData>(),
                saleOffPercent = lstSale[i]

            };
            ResourceData resourceData = new ResourceData
            {
                value = value,
                resourceType = resourceType
            };

            newItem.data.Add(resourceData);
            data.Add(newItem);
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
    [ContextMenu("AddItem")]
    public void AddItem()
    {
        for (int i = 0; i < lstKey.Count; i++)
        {
            string key = lstKey[i];
            var value = lstValue[i];
            IAPItemData newItem = new IAPItemData
            {
                iapKey = key,
                data = new List<ResourceData>(),
                saleOffPercent = lstSale[i]
            };
            ResourceData resourceData = new ResourceData
            {
                value = value,
                resourceType = resourceType
            };
            newItem.data.Add(resourceData);
            data.Add(newItem);
        }

        // Add the newItem to the data list
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

[System.Serializable]
public class IAPItemData
{
    public string iapKey;
    public int saleOffPercent;
    public List<ResourceData> data;
}

[System.Serializable]
public class ResourceData
{
    public int value;
    public ResourceType resourceType;
}

public enum ResourceType
{
    ADD_HOLE = 0,
    HAMMER = 1,
    CLEAR = 2,
    Coin = 3,
    MAGNET = 4,
    TIME_HEART = 5, //(milisecond)
    UNLOCK_BOX = 6,
    ROCKET = 7,
    GLASS = 8,
    FREE_REVIVE = 9,
}

