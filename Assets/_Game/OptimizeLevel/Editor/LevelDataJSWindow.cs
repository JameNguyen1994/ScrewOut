using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class LevelDataJSWindow : EditorWindow
{
    private LevelDataJS levelData = new LevelDataJS();
    private string savePath = "LevelData.json";

    private GameObject rootObject;

    // 🔥 danh sách LevelMap để export nhiều level
    [SerializeField] private List<LevelMap> lstLevelMaps = new List<LevelMap>();
    private Vector2 scrollPos;
    private SerializedObject so;
    private SerializedProperty spList;

    // 👇 Biến bool để kiểm soát việc xóa
    [SerializeField] private bool wishDelete = false;

    [MenuItem("Tools/Optimize Level/Level Data Window")]
    public static void ShowWindow()
    {
        GetWindow<LevelDataJSWindow>("Level Data Window");
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        spList = so.FindProperty("lstLevelMaps");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Data Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Chọn root object (single)
        rootObject = (GameObject)EditorGUILayout.ObjectField("Root Object", rootObject, typeof(GameObject), true);

        if (GUILayout.Button("Scan Shapes & Screws (Single)"))
        {
            ScanShapesAndScrews(rootObject);
        }

        // 👇 Toggle để bật/tắt xóa
        wishDelete = EditorGUILayout.Toggle("Wish Delete", wishDelete);

        EditorGUILayout.Space();
        GUILayout.Label("Batch Export Levels", EditorStyles.boldLabel);

        // Scroll để chứa list levelMap
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
        for (int i = 0; i < lstLevelMaps.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            lstLevelMaps[i] = (LevelMap)EditorGUILayout.ObjectField($"Level {i}", lstLevelMaps[i], typeof(LevelMap), true);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                lstLevelMaps.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        so.Update();
        EditorGUILayout.PropertyField(spList, new GUIContent("Level Maps"), true);
        so.ApplyModifiedProperties();

        if (GUILayout.Button("Export All Levels"))
        {
            ExportAllLevels();
        }
        if (GUILayout.Button("DeleteAllLevel"))
        {
            DeleteAllLevel();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Save/Load JSON (Single)", EditorStyles.boldLabel);

        // Đường dẫn file
        EditorGUILayout.BeginHorizontal();
        int levelId = (rootObject != null && rootObject.GetComponent<LevelMap>() != null)
            ? rootObject.GetComponent<LevelMap>().LevelId : 0;
        savePath = EditorGUILayout.TextField("Save Path", savePath);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string path = EditorUtility.SaveFilePanel(
                "Save Level Data",
                "Assets/_Game/OptimizeLevel/LevelDatas",
                $"LevelData_{levelId}",
                "json"
            );
            if (!string.IsNullOrEmpty(path))
            {
                savePath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Save JSON")) SaveJson();
        if (GUILayout.Button("Load JSON")) LoadJson();
    }

    /// <summary>
    /// Tìm Shapes & Screws trong 1 levelMapRoot
    /// </summary>
    private void ScanShapesAndScrews(GameObject levelMapRoot)
    {
        if (levelMapRoot == null)
        {
            Debug.LogWarning("Root Object chưa được chọn!");
            return;
        }

        levelData = new LevelDataJS();
        var levelMap = levelMapRoot.GetComponent<LevelMap>();
        if (levelMap == null)
        {
            Debug.LogWarning("Root không có LevelMap!");
            return;
        }

        Shape[] allShapes = levelMap.LstShape.ToArray();
        LinkObstacle[] allLinks = levelMap.LstLinkObstacle.ToArray();

        foreach (var shapeCmp in allShapes)
        {
            ShapeData shape = new ShapeData();
            Screw[] screws = shapeCmp.LstScrew.ToArray();
            foreach (var s in screws)
            {
                var tf = s.transform;
                shape.screws.Add(new ScrewTransformData()
                {
                    position = tf.position,
                    rotation = tf.rotation,
                    scale = tf.localScale
                });
            }
            levelData.shapes.Add(shape);
        }

        foreach (var link in allLinks)
        {
            LinkData linkData = new LinkData();
            levelData.links.Add(linkData);
        }

        // 👇 Nếu wishDelete = true thì xóa sau khi đã lấy dữ liệu
        if (wishDelete)
        {
            for (int i = 0; i < allShapes.Length; i++)
            {
                var shapeCmp = allShapes[i];
                Screw[] screws = shapeCmp.LstScrew.ToArray();
                for (int k = 0; k < screws.Length; k++)
                {
                    var s = screws[k];
                    if (s != null)
                        DestroyImmediate(s.gameObject, true);
                }
            }

            foreach (var shapeCmp in allShapes)
            {
                foreach (var hole in shapeCmp.LstHoldRigi)
                    if (hole != null)
                        DestroyImmediate(hole.gameObject, true);
            }
        }

        Debug.Log($"Level {levelMap.LevelId} -> Shapes: {levelData.shapes.Count}, Links: {levelData.links.Count}");
    }

    public void DeleteScrewAndHole(LevelMap levelMapRoot)
    {
        var levelMap = levelMapRoot.GetComponent<LevelMap>();
        if (levelMap == null)
        {
            Debug.LogWarning("Root không có LevelMap!");
            return;
        }

        Shape[] allShapes = levelMap.LstShape.ToArray();
        for (int i = 0; i < allShapes.Length; i++)
        {
            var shapeCmp = allShapes[i];
            Screw[] screws = shapeCmp.LstScrew.ToArray();
            for (int k = 0; k < screws.Length; k++)
            {
                var s = screws[k];
                if (s != null)
                    DestroyImmediate(s.gameObject, true);
            }
        }

        foreach (var shapeCmp in allShapes)
        {
            foreach (var hole in shapeCmp.LstHoldRigi)
                if (hole != null)
                    DestroyImmediate(hole.gameObject, true);
        }
    }

    public void SaveAsPrefab(GameObject levelMap, int levelId)
    {
        string folderPath = "Assets/_Game/MCPE/LevelPrefab";
        string prefabName = $"Level_{levelId}.prefab";
        string fullPath = Path.Combine(folderPath, prefabName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        PrefabUtility.SaveAsPrefabAsset(levelMap, fullPath);
        Debug.Log($"Saved prefab: {fullPath}");
        EnsureAddressable(fullPath, $"Level_{levelId}");
    }

    private void EnsureAddressable(string assetPath, string address)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.FindAssetEntry(guid);

        if (entry == null)
        {
            entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
            entry.address = address;
            Debug.Log($"Addressable assigned: {address}");
        }
    }

    private void ExportAllLevels()
    {
        string basePath = EditorUtility.OpenFolderPanel(
            "Chọn thư mục lưu tất cả LevelData",
            "Assets/_Game/OptimizeLevel/LevelDatas",
            ""
        );
        if (string.IsNullOrEmpty(basePath)) return;

        bool canceled = false;

        for (int i = 0; i < lstLevelMaps.Count; i++)
        {
            var levelMap = lstLevelMaps[i];
            if (levelMap == null) continue;

            float progress = (float)i / lstLevelMaps.Count;
            if (EditorUtility.DisplayCancelableProgressBar(
                "Exporting Levels",
                $"Processing Level {levelMap.LevelId} ({i + 1}/{lstLevelMaps.Count})",
                progress))
            {
                canceled = true;
                break;
            }

            // Scan dữ liệu (có thể xóa nếu wishDelete = true)
            ScanShapesAndScrews(levelMap.gameObject);

            // Save JSON
            string filePath = Path.Combine(basePath, $"LevelData_{levelMap.LevelId}.json");
            string json = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(filePath, json);

            // Save prefab
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(levelMap.gameObject);
            if (prefab != null)
            {
                PrefabUtility.SavePrefabAsset(prefab);
                Debug.Log($"Updated prefab for Level {levelMap.LevelId}");
            }
            else
            {
                Debug.LogWarning($"Level {levelMap.LevelId} is not a prefab instance, skipping prefab save.");
            }

            Debug.Log($"Saved Level {levelMap.LevelId} data to {filePath}");
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (canceled)
            EditorUtility.DisplayDialog("Export Canceled", "Quá trình export đã bị hủy.", "OK");
        else
            EditorUtility.DisplayDialog("Export Done", $"Đã export {lstLevelMaps.Count} levels", "OK");
    }

    private void DeleteAllLevel()
    {
        for (int i = 0; i < lstLevelMaps.Count; i++)
        {
            var levelMap = lstLevelMaps[i];
            if (levelMap == null) continue;

            if (wishDelete)
            {
                DeleteScrewAndHole(levelMap);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", $"Đã xử lý {lstLevelMaps.Count} levels", "OK");
    }

    private void SaveJson()
    {
        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Saved LevelDataJS to " + savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void LoadJson()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("File not found: " + savePath);
            return;
        }

        string json = File.ReadAllText(savePath);
        levelData = JsonUtility.FromJson<LevelDataJS>(json);
        Debug.Log("Loaded LevelDataJS from " + savePath);
    }
}
