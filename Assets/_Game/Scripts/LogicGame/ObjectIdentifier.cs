using UnityEngine;
using System;

[DisallowMultipleComponent]
public class ObjectIdentifier : MonoBehaviour, ISerialization
{
    [SerializeField]
    protected string uniqueId;

    public virtual string UniqueId => uniqueId;

    public void SetId(string id)
    {
        uniqueId = id;
    }

    public virtual void InitData()
    {
        if (string.IsNullOrEmpty(uniqueId))
        {
            uniqueId = Guid.NewGuid().ToString();
        }
    }

#if UNITY_EDITOR

    [EasyButtons.Button]
    public void GenerateId()
    {
        InitData();
    }

    [EasyButtons.Button]
    public void SetData()
    {
        Serialize();
        EditorLogger.Log("Set Data: " + PlayerPrefs.GetString(UniqueId));
    }

    [EasyButtons.Button]
    public void GetData()
    {
        EditorLogger.Log("Get Data: " + PlayerPrefs.GetString(UniqueId));
    }

    [EasyButtons.Button]
    public void TestInitializeFromSave()
    {
        InitializeFromSave();
    }

#endif

    public virtual void Serialize()
    {
        EditorLogger.Log(">>>> Serialize: " + name + " Id: " + UniqueId);
    }

    public virtual void InitializeFromSave()
    {
        EditorLogger.Log(">>>> Reload: " + name + " Id: " + UniqueId);
    }

    public virtual void Serialize<T>(T data) where T : GameData
    {
        SerializationService.SerializeObject(UniqueId, data);
    }

    public virtual T Deserialize<T>() where T : GameData
    {
        return SerializationService.DeserializeObject<T>(UniqueId);
    }

    public virtual void ClearData()
    {
        SerializationService.ClearData(UniqueId);
    }
}