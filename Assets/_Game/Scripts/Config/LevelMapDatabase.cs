using UnityEngine;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(fileName = "LevelMapDatabase ", menuName = "Configs/Level Map Database")]
public class LevelMapDatabase : ScriptableObject
{
    public int MinLevel = 10;
    public int MaxLevel = 200;

    public List<LevelDifficulty> LevelDifficulties;
    public List<int> LevelsTotalScrew;
    public List<string> LevelsName;
    public List<string> LevelsIcon;

    public int GetMinLevel()
    {
        return MinLevel;
    }

    public int GetMaxLevel()
    {
        return MaxLevel;
    }

#if UNITY_EDITOR

    public GameObject Hole;
    public Material MaterialWhite;
    public PhysicsMaterial PhysicMaterial;
    public Material OverlayBlink;

    [SerializeField]
    private List<MapModel> configModels = new List<MapModel>();

    public IReadOnlyList<MapModel> ConfigModels => configModels;


    public MapModel GetMapModelByMeshName(string meshName)
    {
        for (int i = 0; i < ConfigModels.Count; i++)
        {
            if (meshName.Contains(ConfigModels[i].replacementName))
            {
                return ConfigModels[i];
            }
        }

        return null;
    }

    [EasyButtons.Button]
    public void GetInfo()
    {
        var objects = UnityEditor.Selection.objects;
        string content = "";

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null && objects[i] is GameObject select)
            {
                LevelMap levelMap = select.GetComponent<LevelMap>();
                content += $"{levelMap.name} - Total Screw: {levelMap.LstScrew.Count} - Total Box: {levelMap.LstScrew.Count / 3} - Color: {levelMap.GetAmountColor()} - Shape: {levelMap.LstShape.Count}\n";
            }
        }

        Debug.Log(content);
    }

    [EasyButtons.Button]
    public void GetAllLevelDifficulties()
    {
        var objects = UnityEditor.Selection.objects;

        LevelDifficulties = new List<LevelDifficulty>();
        LevelsTotalScrew = new List<int>();
        //LevelsName = new List<string>();

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null && objects[i] is GameObject select)
            {
                LevelMap levelMap = select.GetComponent<LevelMap>();
                LevelDifficulties.Add(levelMap.LevelDifficulty);
                //LevelsName.Add(levelMap.LevelName);
                LevelsTotalScrew.Add(levelMap.GeTotalScrewAmount());
            }
        }

        MaxLevel = LevelDifficulties.Count;

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    public string dataPath = "D:/DataThumbnail.txt";

    [EasyButtons.Button]
    public void GetThumbnail()
    {
        string[] lines = System.IO.File.ReadAllLines(dataPath);
        LevelsIcon = new List<string>();

        foreach (string line in lines)
        {
            LevelsIcon.Add(line);
        }

        Debug.Log("DONE - Get Thumbnail");

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    public string dataReportPath = "D:/Report.txt";

    [EasyButtons.Button]
    public void CheckThumbnail()
    {
        string content = "Missing\n";

        for (int i = 0; i < LevelsIcon.Count; i++)
        {
            string fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, $"LevelMap/{LevelsIcon[i]}.png");

            if (!File.Exists(fullPath))
            {
                content += LevelsIcon[i] + "\n";
            }
        }

        System.IO.File.WriteAllText(dataReportPath, content);

        Debug.Log(content);

        Debug.Log("DONE - Check Thumbnail");
    }

    public string dataNamePath = "D:/Data.txt";

    [EasyButtons.Button]
    public void GetNames()
    {
        string[] lines = System.IO.File.ReadAllLines(dataNamePath);
        LevelsName = new List<string>();

        foreach (string line in lines)
        {
            LevelsName.Add(line);
        }

        Debug.Log("DONE - Get Names");

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

#endif
}

public enum MapModelType
{
    Cube,
    LinkCircle,
    LinkI,
    LinkL,
    LinkSquare,
    LinkY,
    LinkT,
}

[System.Serializable]
public class MapModel
{
    public MapModelType modelType;
    public GameObject prefab;
    public Material material;
    public string replacementName;
}