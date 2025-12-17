using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LevelPrefabReplacer : EditorWindow
{
    private string jsonFilePath = "";
    private string prefabFolderPath = "";
    private string outputFolderPath = "";

    [System.Serializable]
    public class LevelMapping
    {
        public string oldId;
        public string newId;
    }

    [System.Serializable]
    public class LevelMappingList
    {
        public List<LevelMapping> mappings;
    }

    [MenuItem("Tools/Clone & Replace Prefabs (JSON)")]
    public static void ShowWindow()
    {
        GetWindow<LevelPrefabReplacer>("Level Prefab Cloner (JSON)");
    }

    void OnGUI()
    {
        GUILayout.Label("Clone & Replace Level Prefabs from JSON", EditorStyles.boldLabel);

        // JSON File
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("JSON File", jsonFilePath);
        if (GUILayout.Button("Chọn JSON File", GUILayout.Width(150)))
        {
            string selected = EditorUtility.OpenFilePanel("Chọn JSON File", "Assets", "json");
            if (!string.IsNullOrEmpty(selected))
            {
                jsonFilePath = GetRelativeAssetPath(selected);
            }
        }
        EditorGUILayout.EndHorizontal();

        // Prefab Folder
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Prefab Source Folder", prefabFolderPath);
        if (GUILayout.Button("Chọn Folder", GUILayout.Width(150)))
        {
            string selected = EditorUtility.OpenFolderPanel("Chọn folder chứa Prefabs", "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
                prefabFolderPath = GetRelativeAssetPath(selected);
            }
        }
        EditorGUILayout.EndHorizontal();

        // Output Folder
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Output Folder", outputFolderPath);
        if (GUILayout.Button("Chọn Folder", GUILayout.Width(150)))
        {
            string selected = EditorUtility.OpenFolderPanel("Chọn folder lưu Prefabs mới", "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
                outputFolderPath = GetRelativeAssetPath(selected);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        if (GUILayout.Button("Tiến hành Clone và Thay thế"))
        {
            ReplacePrefabDataFromJson();
        }
    }

    private string GetRelativeAssetPath(string absolutePath)
    {
        string projectPath = Application.dataPath;
        if (absolutePath.StartsWith(projectPath))
        {
            return "Assets" + absolutePath.Substring(projectPath.Length);
        }
        return absolutePath;
    }

    private void ReplacePrefabDataFromJson()
    {
        if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath))
        {
            Debug.LogError("JSON file không hợp lệ!");
            return;
        }

        if (!AssetDatabase.IsValidFolder(prefabFolderPath))
        {
            Debug.LogError("Prefab source folder không hợp lệ!");
            return;
        }

        if (!AssetDatabase.IsValidFolder(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
            AssetDatabase.Refresh();
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        List<LevelMapping> mappingList;

        try
        {
            mappingList = JsonUtility.FromJson<LevelMappingWrapper>($"{{\"mappings\":{jsonContent}}}").mappings;
        }
        catch
        {
            Debug.LogError("Lỗi khi đọc JSON. Đảm bảo định dạng đúng.");
            return;
        }

        foreach (var mapping in mappingList)
        {
            if (mapping.oldId == mapping.newId)
                continue;

            string oldPrefabPath = FindPrefabById(mapping.oldId);
            string newPrefabPath = FindPrefabById(mapping.newId);

            if (string.IsNullOrEmpty(oldPrefabPath) || string.IsNullOrEmpty(newPrefabPath))
            {
                Debug.LogWarning($"Không tìm thấy prefab cho oldId: {mapping.oldId} hoặc newId: {mapping.newId}");
                continue;
            }

            GameObject oldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(oldPrefabPath);
            GameObject newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath);

            if (oldPrefab == null || newPrefab == null)
            {
                Debug.LogWarning($"Lỗi khi load prefab cho: {mapping.oldId} hoặc {mapping.newId}");
                continue;
            }

            Debug.Log($"Clone từ ID mới {mapping.newId} thành ID cũ {mapping.oldId}");

            GameObject tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);
            ReplaceData(tempInstance, oldPrefab);

            string savePath = $"{outputFolderPath}/Level_{mapping.oldId}.prefab";
            PrefabUtility.SaveAsPrefabAsset(tempInstance, savePath);
            DestroyImmediate(tempInstance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Hoàn tất clone và thay thế.");
    }

    private string FindPrefabById(string id)
    {
        string[] guids = AssetDatabase.FindAssets($"t:Prefab Level_{id}", new[] { prefabFolderPath });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path) == $"Level_{id}")
            {
                return path;
            }
        }
        return null;
    }

    private void ReplaceData(GameObject target, GameObject source)
    {
        var sourceMap = source.GetComponent<LevelMap>();
        if (sourceMap == null)
        {
            Debug.LogWarning($"Prefab {source.name} không có component LevelMap.");
            return;
        }
        var targetMap = target.GetComponent<LevelMap>();
        if (targetMap == null)
        {
            Debug.LogWarning($"Prefab {target.name} không có component LevelMap.");
            return;
        }

        targetMap.LevelId = sourceMap.LevelId;
    }
    
    [System.Serializable]
    private class LevelMappingWrapper
    {
        public List<LevelMapping> mappings;
    }
}
