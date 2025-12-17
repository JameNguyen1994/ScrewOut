using UnityEngine;
using System.IO;

public static class D30Service
{
    private static string Path => System.IO.Path.Combine(Application.persistentDataPath, "daily30.json");

    public static D30 Load()
    {
        if (!File.Exists(Path))
        {
            return new D30();
        }

        string json = File.ReadAllText(Path);
        return JsonUtility.FromJson<D30>(json);
    }

    public static void Save(D30 data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path, json);
    }
}
