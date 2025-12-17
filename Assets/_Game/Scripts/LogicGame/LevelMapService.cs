using CodeStage.AntiCheat.ObscuredTypes.Converters;
using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;
using Storage;
using System.Collections.Generic;
using UnityEngine;

public class LevelMapService
{
    private const string Key_Shuffled = "LevelManager_Shuffled";
    private const int Default_Level = 10;

    private static List<int> _shuffledLevels;
    private static List<int> shuffledLevels
    {
        get
        {
            if (_shuffledLevels == null || _shuffledLevels.Count == 0)
            {
                _shuffledLevels = GetShuffledLevels();
            }

            return _shuffledLevels;
        }
    }

    private static LevelMapDatabase _mapDatabase;
    private static LevelMapDatabase MapDatabase
    {
        get
        {
            if (_mapDatabase == null)
            {
                _mapDatabase = Resources.Load<LevelMapDatabase>(Define.CONFIG_LEVEL_MAP);
            }

            return _mapDatabase;
        }
    }

    private static List<int> GenerateShuffledLevels()
    {
        EditorLogger.Log(">>>> Generate Shuffled Levels");

        List<int> levels = new List<int>();

        for (int i = MapDatabase.GetMinLevel(); i <= MapDatabase.GetMaxLevel(); i++)
        {
            levels.Add(i);
        }

        SaveProgress(levels);

        return levels;
    }

    public static void UserLevelMapUp(int userLevel)
    {
        if (userLevel > MapDatabase.GetMaxLevel())
        {
            if (shuffledLevels.Count > 0)
            {
                shuffledLevels.RemoveAt(0);
            }

            if (shuffledLevels.Count == 0)
            {
                GenerateShuffledLevels();
            }
            else
            {
                SaveProgress(shuffledLevels);
            }
        }
    }

    private static List<int> GetShuffledLevels()
    {
        string shuffledJson = ObscuredPrefs.Get(Key_Shuffled, string.Empty);

        if (!string.IsNullOrEmpty(shuffledJson))
        {
            EditorLogger.Log(">>>>Get ShuffledLevels: " + shuffledJson);
            ShuffledData data = JsonConvert.DeserializeObject<ShuffledData>(shuffledJson, new ObscuredTypesNewtonsoftConverter());

            int totalRandom = MapDatabase.GetMaxLevel() - MapDatabase.GetMinLevel() + 1;

            if (data.levels.Count == totalRandom)
            {
                return data.levels;
            }
        }

        return GenerateShuffledLevels();
    }

    private static void SaveProgress(List<int> shuffledLevels)
    {
        ShuffledData data = new ShuffledData { levels = shuffledLevels };
        string json = JsonConvert.SerializeObject(data, new ObscuredTypesNewtonsoftConverter());
        ObscuredPrefs.Set(Key_Shuffled, json);
        EditorLogger.Log(">>>>Save ShuffledLevels: " + json);
    }

    [System.Serializable]
    private class ShuffledData
    {
        public List<int> levels;
    }

    public static int GetLevelMap(int userLevel)
    {
        if (userLevel <= MapDatabase.GetMaxLevel())
        {
            return userLevel;
        }

        int index = (userLevel - MapDatabase.GetMaxLevel() - 1) % shuffledLevels.Count;
        return shuffledLevels[index];
    }

    public static LevelDifficulty GetLevelDifficulty(int userLevel)
    {
        int realLevelMap = GetLevelMap(userLevel);

        if (MapDatabase.LevelDifficulties != null && MapDatabase.LevelDifficulties.Count >= realLevelMap)
        {
            return MapDatabase.LevelDifficulties[realLevelMap - 1];
        }

        return LevelDifficulty.Easy;
    }

    public static string GetLevelName(int userLevel)
    {
        int realLevelMap = GetLevelMap(userLevel);

        if (MapDatabase.LevelsName != null && MapDatabase.LevelsName.Count >= realLevelMap)
        {
            if (!string.IsNullOrEmpty(MapDatabase.LevelsName[realLevelMap - 1]))
            {
                return MapDatabase.LevelsName[realLevelMap - 1];
            }
        }

        return "LEVEL " + realLevelMap;
    }

    public static int GetLevelTotalScrew(int userLevel)
    {
        int realLevelMap = GetLevelMap(userLevel);

        if (MapDatabase.LevelsTotalScrew != null && MapDatabase.LevelsTotalScrew.Count >= realLevelMap)
        {
            return MapDatabase.LevelsTotalScrew[realLevelMap - 1];
        }

        return 100;
    }

    public static string GetLevelThumbnail(int userLevel)
    {
        int realLevelMap = GetLevelMap(userLevel);

        if (MapDatabase.LevelsIcon != null && MapDatabase.LevelsIcon.Count >= realLevelMap)
        {
            if (!string.IsNullOrEmpty(MapDatabase.LevelsIcon[realLevelMap - 1]))
            {
                return MapDatabase.LevelsIcon[realLevelMap - 1];
            }
        }

        return string.Empty;
    }

    public static int GetDefaultLevel()
    {
        return Default_Level;
    }
}