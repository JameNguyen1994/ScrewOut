using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class YService
{
    private static string PathFor(int year)
        => System.IO.Path.Combine(Application.persistentDataPath, $"ylb_{year}.json");

    public static void Save(Y data)
    {
        string json = JsonUtility.ToJson(data, false);
        File.WriteAllText(PathFor(data.y), json);
    }

    public static Y Load(int year)
    {
        string path = PathFor(year);

        if (!File.Exists(path))
            return CreateEmptyYear(year);

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<Y>(json);
    }

    private static Y CreateEmptyYear(int year)
    {
        var y = new Y { y = year, m = new List<M>() };

        for (int i = 1; i <= 12; i++)
            y.m.Add(new M { m = i, u = new List<U>() });

        return y;
    }
}
