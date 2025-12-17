using CodeStage.AntiCheat.ObscuredTypes.Converters;
using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;

public class SerializationService
{
    public static void SerializeObject<T>(string id, T data) where T : GameData
    {
        string json = JsonConvert.SerializeObject(data, new ObscuredTypesNewtonsoftConverter());
        ObscuredPrefs.Set(id, json);
    }

    public static T DeserializeObject<T>(string id) where T : GameData
    {
        string json = ObscuredPrefs.Get(id, string.Empty);

        if (string.IsNullOrEmpty(json))
        {
            return default(T);
        }

        return JsonConvert.DeserializeObject<T>(json, new ObscuredTypesNewtonsoftConverter());
    }

    public static string GetIdByName(string name)
    {
        for (int i = 0; i < Define.IDENTIFIERS.Length; i++)
        {
            if (Define.IDENTIFIERS[i].Name == name)
            {
                return Define.IDENTIFIERS[i].Value;
            }
        }

        return string.Empty;
    }

    public static void ClearData(string id)
    {
        if (ObscuredPrefs.HasKey(id))
        {
            ObscuredPrefs.DeleteKey(id);
        }
    }

    public static bool IsHasData(string id)
    {
        return ObscuredPrefs.HasKey(id);
    }

    public static bool IsNoneData(string id)
    {
        return !IsHasData(id);
    }
}

public class Identifier
{
    public string Name;
    public string Value;

    public Identifier(string name, string value)
    {
        Name = name;
        Value = value;
    }
}