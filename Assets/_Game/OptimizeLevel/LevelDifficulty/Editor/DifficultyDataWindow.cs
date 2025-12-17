using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class DifficultyDataWindow : EditorWindow
{
    private List<BoxDifficultyData> dataList = new List<BoxDifficultyData>();
    private LevelDifficultyData levelData = new LevelDifficultyData() { level = 1 };

    private Vector2 scrollPos1;
    private Vector2 scrollPos2;

    private BoxDifficultyData copyBuffer; // để copy-paste

    private string defaultPosBox = "Assets/_Game/OptimizeLevel/LevelDifficulty/Datas/DifficultyData/";
    private string defaultPosLevel = "Assets/_Game/OptimizeLevel/LevelDifficulty/Datas/DifficultyData/";
    private int startLevel = 1;
    private int endLevel = 150;
    [MenuItem("Tools/Optimize Level/Difficulty Data Window")]
    public static void ShowWindow()
    {
        GetWindow<DifficultyDataWindow>("Difficulty Data");
    }

    private void OnGUI()
    {
        GUILayout.Label("=== Box Difficulty Data ===", EditorStyles.boldLabel);
        DrawBoxDifficultyData();

        GUILayout.Space(20);
        GUILayout.Label("=== Level Difficulty Data ===", EditorStyles.boldLabel);
        DrawLevelDifficultyData();
        DrawQuitSaveLevel();
    }
    private void DrawQuitSaveLevel()
    {
        GUILayout.Label("=== Quick Save Levels ===", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        startLevel = EditorGUILayout.IntField("Start Level", startLevel, GUILayout.Width(200));
        endLevel = EditorGUILayout.IntField("End Level", endLevel, GUILayout.Width(200));

        if (GUILayout.Button("Save Range Levels", GUILayout.Width(200)))
        {
            QuickSaveRangeLevels(startLevel, endLevel);
        }
        GUILayout.EndHorizontal();
    }
    private void QuickSaveAllLevels()
    {
        // Giả sử bạn có danh sách levels từ 1 đến 150
        List<LevelDifficultyData> allLevels = new List<LevelDifficultyData>();

        for (int i = 1; i <= 150; i++)
        {
            LevelDifficultyData data = new LevelDifficultyData();
            data.level = i;
            data.lstProcessData = new List<ProcessData>();

            // Nếu muốn có sẵn 1 process mặc định cho mỗi level
            data.lstProcessData.Add(new ProcessData()
            {
                process = 1,
                boxDifficulty = new BoxDifficultyData()
                {
                    name = "Default",
                    easePercent = 100,
                    normalPercent = 0,
                    hardPercent = 0
                }
            });

            allLevels.Add(data);
        }

        string path = "Assets/_Game/OptimizeLevel/LevelDifficulty/Editor/AllLevels.json";

        WrapperAllLevels wrapper = new WrapperAllLevels() { list = allLevels };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);

        Debug.Log($"✅ Quick saved 150 levels to {path}");
        AssetDatabase.Refresh();
    }

    #region Box Difficulty Table
    private void DrawBoxDifficultyData()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Entry", GUILayout.Width(150)))
        {
            dataList.Add(new BoxDifficultyData());
        }

        if (GUILayout.Button("Save"))
        {
            SaveBoxData();
        }

        if (GUILayout.Button("Load"))
        {
            LoadBoxData();
        }
        GUILayout.EndHorizontal();

        scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1, GUILayout.Height(200));

        if (dataList.Count > 0)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Name", GUILayout.Width(150));
            GUILayout.Label("Ease %", GUILayout.Width(60));
            GUILayout.Label("Normal %", GUILayout.Width(60));
            GUILayout.Label("Hard %", GUILayout.Width(60));
            GUILayout.Label("Total", GUILayout.Width(60));
            GUILayout.Label("Copy", GUILayout.Width(50));
            GUILayout.Label("Actions", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < dataList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                dataList[i].name = EditorGUILayout.TextField(dataList[i].name, GUILayout.Width(150));
                dataList[i].easePercent = EditorGUILayout.IntField(dataList[i].easePercent, GUILayout.Width(60));
                dataList[i].normalPercent = EditorGUILayout.IntField(dataList[i].normalPercent, GUILayout.Width(60));
                dataList[i].hardPercent = EditorGUILayout.IntField(dataList[i].hardPercent, GUILayout.Width(60));

                int total = dataList[i].easePercent + dataList[i].normalPercent + dataList[i].hardPercent;
                Color prevColor = GUI.color;
                GUI.color = (total == 100) ? Color.green : Color.red;
                GUILayout.Label(total.ToString(), GUILayout.Width(60));
                GUI.color = prevColor;

                if (GUILayout.Button("Copy", GUILayout.Width(50)))
                {
                    copyBuffer = new BoxDifficultyData()
                    {
                        name = dataList[i].name,
                        easePercent = dataList[i].easePercent,
                        normalPercent = dataList[i].normalPercent,
                        hardPercent = dataList[i].hardPercent
                    };
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    dataList.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
    }
    #endregion

    #region Single Level Difficulty
    private void DrawLevelDifficultyData()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level:", GUILayout.Width(50));
        levelData.level = EditorGUILayout.IntField(levelData.level, GUILayout.Width(60));

        if (GUILayout.Button("Add Process", GUILayout.Width(120)))
        {
            levelData.lstProcessData.Add(new ProcessData());
        }

        if (GUILayout.Button("Save"))
        {
            SaveLevelData();
        }

        if (GUILayout.Button("Load"))
        {
            LoadLevelData();
        }
        GUILayout.EndHorizontal();

        scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2);

        for (int j = 0; j < levelData.lstProcessData.Count; j++)
        {
            var process = levelData.lstProcessData[j];
            EditorGUILayout.BeginHorizontal("box");

            GUILayout.Label("Prcess", GUILayout.Width(60));
            process.process = EditorGUILayout.IntField(process.process, GUILayout.Width(60));

            // BoxDifficultyData hiển thị
            process.boxDifficulty.name = EditorGUILayout.TextField(process.boxDifficulty.name, GUILayout.Width(100));
            process.boxDifficulty.easePercent = EditorGUILayout.IntField(process.boxDifficulty.easePercent, GUILayout.Width(50));
            process.boxDifficulty.normalPercent = EditorGUILayout.IntField(process.boxDifficulty.normalPercent, GUILayout.Width(50));
            process.boxDifficulty.hardPercent = EditorGUILayout.IntField(process.boxDifficulty.hardPercent, GUILayout.Width(50));

            int total = process.boxDifficulty.easePercent +
                        process.boxDifficulty.normalPercent +
                        process.boxDifficulty.hardPercent;

            Color prevColor = GUI.color;
            GUI.color = (total == 100) ? Color.green : Color.red;
            GUILayout.Label(total.ToString(), GUILayout.Width(50));
            GUI.color = prevColor;

            if (GUILayout.Button("Paste", GUILayout.Width(60)) && copyBuffer != null)
            {
                process.boxDifficulty.name = copyBuffer.name;
                process.boxDifficulty.easePercent = copyBuffer.easePercent;
                process.boxDifficulty.normalPercent = copyBuffer.normalPercent;
                process.boxDifficulty.hardPercent = copyBuffer.hardPercent;
            }

            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                levelData.lstProcessData.RemoveAt(j);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
    #endregion

    #region Save/Load
    private void SaveBoxData()
    {
        var path = defaultPosBox + $"BoxDifficultyData.json";
        if (!string.IsNullOrEmpty(path))
        {
            string json = JsonUtility.ToJson(new WrapperBox() { list = dataList }, true);
            File.WriteAllText(path, json);
            Debug.Log($"Saved Box data to: {path}");
        }
    }

    private void LoadBoxData()
    {
        string path = EditorUtility.OpenFilePanel("Load Box Difficulty Data", defaultPosBox, "json");
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var wrapper = JsonUtility.FromJson<WrapperBox>(json);
            if (wrapper != null && wrapper.list != null)
            {
                dataList = wrapper.list;
                Debug.Log($"Loaded Box data from: {path}");
            }
        }
    }

    private void SaveLevelData()
    {
      //  string path = EditorUtility.SaveFilePanel("Save Level Difficulty Data", defaultPosLevel, $"LevelDifficultyData_{levelData.level}", "json");
        var path = defaultPosBox+$"LevelData_{levelData.level}" + ".json";
        if (!string.IsNullOrEmpty(path))
        {
            string json = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(path, json);
            Debug.Log($"Saved Level data to: {path}");
            AssetDatabase.Refresh();
        }
    }

    private void LoadLevelData()
    {
        string path = EditorUtility.OpenFilePanel("Load Level Difficulty Data", defaultPosLevel, "json");
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var obj = JsonUtility.FromJson<LevelDifficultyData>(json);
            if (obj != null)
            {
                levelData = obj;
                Debug.Log($"Loaded Level data from: {path}");
            }
        }
    }
    #endregion

    [System.Serializable]
    private class WrapperBox
    {
        public List<BoxDifficultyData> list;
    }
    private void QuickSaveRangeLevels(int start, int end)
    {
        if (start <= 0 || end < start)
        {
            Debug.LogError("❌ Range không hợp lệ!");
            return;
        }

        string folder = defaultPosBox;
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        for (int i = start; i <= end; i++)
        {
            LevelDifficultyData data = new LevelDifficultyData();
            data.level = i;
            data.lstProcessData = new List<ProcessData>();

            // 👉 ở đây bạn có thể copy dữ liệu từ editor (levelData) 
            // hoặc tạo mặc định
            data.lstProcessData.Add(new ProcessData()
            {
                process = 1,
                boxDifficulty = new BoxDifficultyData()
                {
                    name = "Default",
                    easePercent = 100,
                    normalPercent = 0,
                    hardPercent = 0
                }
            });

            string path = Path.Combine(folder, $"LevelData_{i}.json");

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log($"✅ Saved Level {i} → {path}");
        }

        AssetDatabase.Refresh();
    }

}
[System.Serializable]
public class WrapperAllLevels
{
    public List<LevelDifficultyData> list;
}

