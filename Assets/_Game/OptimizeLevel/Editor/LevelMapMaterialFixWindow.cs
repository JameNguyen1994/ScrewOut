using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class LevelMapMaterialFixWindow : EditorWindow
{
    private LevelDataJS levelData = new LevelDataJS();
    private string savePath = "LevelData.json";


    // 🔥 danh sách LevelMap để export nhiều level
    [SerializeField] private List<LevelMap> lstLevelMaps = new List<LevelMap>();
    [SerializeField] private List<PairMaterial> lstPairMaterial = new List<PairMaterial>();
    private Vector2 scrollPos;
    private SerializedObject so;
    private SerializedProperty spList;
    private SerializedProperty spListMate;
    [MenuItem("Tools/LevelMapMaterialFixWindow")]
    public static void ShowWindow()
    {
        GetWindow<LevelMapMaterialFixWindow>("Level Map Material Fix Windoww");
    }
    private void OnEnable()
    {
        so = new SerializedObject(this);
        spList = so.FindProperty("lstLevelMaps");
        spListMate = so.FindProperty("lstPairMaterial");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Data Fix Materials", EditorStyles.boldLabel);

        so.Update();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.PropertyField(spListMate, new GUIContent("List Pair Mate"), true);
        EditorGUILayout.PropertyField(spList, new GUIContent("Level Maps"), true);
        EditorGUILayout.EndScrollView();
        so.ApplyModifiedProperties();
        if (GUILayout.Button("Check Missing Material trans Levels"))
        {
            ExportAllLevels();
        }
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

        var levelMap = levelMapRoot.GetComponent<LevelMap>();
        if (levelMap == null)
        {
            Debug.LogWarning("Root không có LevelMap!");
            return;
        }
/*      //  string nameNorMaterial = levelMap.MatNor != null ? levelMap.MatNor.name : "null";
        bool found = false;
        for (int i = 0; i < lstPairMaterial.Count; i++)
        {
            if (levelMap.MatNor == lstPairMaterial[i].norMaterial)
            {
                levelMap.MatTrans = lstPairMaterial[i].transMaterial;
                Debug.Log($"Fixed Level {levelMap.LevelId} material to {levelMap.MatTrans.name}");
                //return;
                found = true;
                break;
            }
        }
        if (!found)
        {
            Debug.LogError($"Level {levelMap.LevelId} material {nameNorMaterial} not found in list pair material");
            return;
        }*/
        SaveAsPrefab(levelMap.gameObject, levelMap.LevelId);
    }
    public void SaveAsPrefab(GameObject levelMap, int levelId)
    {
        string folderPath = "Assets/_Game/MCPE/SofPrefabs"; // Assets/_Game/MCPE/LevelPrefab/Level_119.prefab
        string prefabName = $"Level_{levelId}.prefab";
        string fullPath = Path.Combine(folderPath, prefabName);

        // Ensure folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }
        PrefabUtility.SaveAsPrefabAsset(levelMap, fullPath);
        Debug.Log($"Saved prefab: {fullPath}");

        // Ensure it's marked as Addressable
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
    /// <summary>
    /// Xuất tất cả level trong list lstLevelMaps
    /// </summary>
    private void ExportAllLevels()
    {
        // string basePath = EditorUtility.OpenFolderPanel("Chọn thư mục lưu tất cả LevelData", "Assets/_Game/OptimizeLevel/SofLevels", "");
        // if (string.IsNullOrEmpty(basePath)) return;

        bool canceled = false;

        for (int i = 0; i < lstLevelMaps.Count; i++)
        {
            var levelMap = lstLevelMaps[i];
            if (levelMap == null) continue;

            float progress = (float)i / lstLevelMaps.Count;
            // 🔥 Sử dụng CancelableProgressBar
            if (EditorUtility.DisplayCancelableProgressBar(
                "Exporting Levels",
                $"Processing Level {levelMap.LevelId} ({i + 1}/{lstLevelMaps.Count})",
                progress))
            {
                canceled = true;
                break;
            }

            // Scan dữ liệu
            ScanShapesAndScrews(levelMap.gameObject);

            /*            // Save JSON
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

                        Debug.Log($"Saved Level {levelMap.LevelId} data to {filePath}");*/
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (canceled)
        {
            EditorUtility.DisplayDialog("Export Canceled", "Quá trình export đã bị hủy.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Export Done", $"Đã export {lstLevelMaps.Count} levels", "OK");
        }
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

[System.Serializable]
public class PairMaterial
{
    public Material norMaterial;
    public Material transMaterial;
}