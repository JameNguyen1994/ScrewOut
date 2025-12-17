using System;

public enum Platform
{
    android,
    ios,
    pc,
}

[Serializable]
public class AssetBundleData
{
    public string Id;
    public string PrefabName;
    public string BundleName;

    public string GetId()
    {
        return Id;
    }

    public string GetPrefabName()
    {
        return PrefabName;
    }

    public string GetBundleName()
    {
        return $"{BundleName}_{AssetBundleService.GetPlatform()}.ab";
    }

    public string GetBundleURL()
    {
        return $"{AssetBundleService.GetPlatform()}/{GetBundleName()}";
    }
}