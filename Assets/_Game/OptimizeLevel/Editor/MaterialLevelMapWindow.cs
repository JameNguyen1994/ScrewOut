using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class MaterialLevelMapWindow : EditorWindow
{
    private LevelDataJS levelData = new LevelDataJS();
    private string savePath = "LevelData.json";


    // 🔥 danh sách LevelMap để export nhiều level
    [SerializeField] private List<LevelMap> lstLevelMaps = new List<LevelMap>();
    [SerializeField] private List<LevelMaterial> lstPairMaterial = new List<LevelMaterial>();
    [SerializeField] private List<Material> lstMaterial = new List<Material>();
    private Vector2 scrollPosLevelMaterial;
    private Vector2 scrollPosLevel;
    private Vector2 scrollPosMaterial;
    private SerializedObject so;
    private SerializedProperty spList;
    private SerializedProperty spListMate;
    private SerializedProperty spListMateDistince;
    [MenuItem("Tools/MaterialLevelMapWindow")]
    public static void ShowWindow()
    {
        GetWindow<MaterialLevelMapWindow>("Level Map Material Fix Windoww");
    }
    private void OnEnable()
    {
        so = new SerializedObject(this);
        spList = so.FindProperty("lstLevelMaps");
        spListMate = so.FindProperty("lstPairMaterial");
        spListMateDistince = so.FindProperty("lstMaterial");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Data Fix Materials", EditorStyles.boldLabel);

        so.Update();
        scrollPosLevelMaterial = EditorGUILayout.BeginScrollView(scrollPosLevelMaterial);
        EditorGUILayout.PropertyField(spListMate, new GUIContent("List Pair Mate"), true);
        EditorGUILayout.EndScrollView();

        scrollPosLevel = EditorGUILayout.BeginScrollView(scrollPosLevel);
        EditorGUILayout.PropertyField(spList, new GUIContent("Level Maps"), true);

        EditorGUILayout.EndScrollView();
        scrollPosMaterial = EditorGUILayout.BeginScrollView(scrollPosMaterial);
        EditorGUILayout.PropertyField(spListMateDistince, new GUIContent("Lst Material Distince"), true);
        EditorGUILayout.EndScrollView();

        so.ApplyModifiedProperties();
        if (GUILayout.Button("Check Missing Material trans Levels"))
        {
            FindAllMaterial();
        }
        if (GUILayout.Button("Change Material folder"))
        {
            ExportDistinctMaterials();
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
      /*  lstPairMaterial.Add(new LevelMaterial()
        {
            LevelId = levelMap.LevelId,
            transMaterial = levelMap.MatNor,
        });
        if (lstMaterial.Contains(levelMap.MatNor) == false)
        {
            lstMaterial.Add(levelMap.MatNor);
        }*/
        // SaveAsPrefab(levelMap.gameObject, levelMap.LevelId);
    }
    private void FindAllMaterial()
    {
        lstMaterial.Clear();
        lstPairMaterial.Clear();
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
                "Finding Levels",
                $"Processing Level {levelMap.LevelId} ({i + 1}/{lstLevelMaps.Count})",
                progress))
            {
                canceled = true;
                break;
            }

            // Scan dữ liệu
            ScanShapesAndScrews(levelMap.gameObject);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (canceled)
        {
            EditorUtility.DisplayDialog("Find Canceled", "Quá trình Find đã bị hủy.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Find Done", $"Đã export {lstLevelMaps.Count} levels", "OK");
        }
    }
    /// <summary>
    /// Move tất cả material distinct vào thư mục chỉ định (không clone)
    /// </summary>
    private void ExportDistinctMaterials()
    {
        string path = EditorUtility.OpenFolderPanel("Chọn thư mục lưu Material", "Assets", "");
        if (string.IsNullOrEmpty(path)) return;

        // Convert system path -> relative path (Assets/…)
        if (path.StartsWith(Application.dataPath))
        {
            path = "Assets" + path.Substring(Application.dataPath.Length);
        }
        else
        {
            EditorUtility.DisplayDialog("Invalid Path", "Hãy chọn thư mục nằm trong Assets!", "OK");
            return;
        }

        foreach (var mat in lstMaterial)
        {
            if (mat == null) continue;

            string srcPath = AssetDatabase.GetAssetPath(mat);
            if (string.IsNullOrEmpty(srcPath)) continue;

            string destPath = Path.Combine(path, Path.GetFileName(srcPath));
            destPath = AssetDatabase.GenerateUniqueAssetPath(destPath);

            string error = AssetDatabase.MoveAsset(srcPath, destPath);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"❌ Move {srcPath} -> {destPath} failed: {error}");
            }
            else
            {
                Debug.Log($"✅ Moved {mat.name} -> {destPath}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Export Done", $"Đã move {lstMaterial.Count} materials vào {path}", "OK");
    }

}

[System.Serializable]
public class LevelMaterial
{
    public int LevelId;
    public Material transMaterial;
}