using System;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

public interface IOwDb
{
    T GetDataCustom<T>(string key, T defaultValue);
    T GetData<T>(string key, T defaultValue);
    void SetDataCustom<T>(string key, T value);
    void SetData<T>(string key, T value);
}

/// <summary>
/// Offerwall Database class
/// </summary>

public class OwDb: IOwDb
{
    public OwDb()
    {
        if (!ObscuredPrefs.HasKey("VC"))
        {
            SetData<float>("VC", 0);
        }

        if (!ObscuredPrefs.HasKey("OW_USER_INFO"))
        {
            SetDataCustom<OfferwallUserInfo>("OW_USER_INFO", new OfferwallUserInfo(){token = "", uuid = ""});
        }

        if (!ObscuredPrefs.HasKey("OW_PRODEGE_DATA"))
        {
            SetDataCustom<OfferwallData>("OW_PRODEGE_DATA", new OfferwallData(){TotalRevenue = 0, TotalVirtualCurrency = 0});
        }
    }
    
    public T GetDataCustom<T>(string key, T defaultValue)
    {
        if (ObscuredPrefs.HasKey(key))
        {
            var value = ObscuredPrefs.Get<string>(key);
            return JsonUtility.FromJson<T>(value);
        }
        
        return defaultValue;
    }

    public T GetData<T>(string key, T defaultValue)
    {
        if (ObscuredPrefs.HasKey(key))
        {
            return ObscuredPrefs.Get<T>(key);
        }
        
        return defaultValue;
    }

    public void SetDataCustom<T>(string key, T value)
    {
        var json = JsonUtility.ToJson(value);
        ObscuredPrefs.Set(key, json);
    }

    public void SetData<T>(string key, T value)
    {
        ObscuredPrefs.Set<T>(key, value);
    }
}

[Serializable]
public class OfferwallData
{
    public double TotalVirtualCurrency;
    public double TotalRevenue;
}
    
[Serializable]
public class OfferwallUserInfo
{
    public string uuid;
    public string token;
}
