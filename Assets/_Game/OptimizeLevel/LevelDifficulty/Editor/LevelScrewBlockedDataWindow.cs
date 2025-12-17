using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using OptimizeLevel.LevelDifficulty.Editor;
using Storage;

public class LevelScrewBlockedDataWindow : EditorWindow
{
    public static LevelScrewBlockedDataWindow Instance;
    private CancellationTokenSource cancelSource;

    private LevelScrewBlockedData levelData = new LevelScrewBlockedData();
    private string savePath = "LevelData.json";

    private GameObject rootObject;

    private Vector2 scrollGrid;
    private Vector2 scrollDetail;

    int startLevelIndex = 1;
    int endLevelIndex = 150;
    bool isExporting = false;
    private int level;
    private bool isDetailMode = false;
    private bool isShowData = false;
    private LevelScrewBlockedData selectedLevel;
    private ItemDetailLevel selectedItemLevelMap;
    // 🔥 danh sách load từ file JSON
    private List<LevelScrewBlockedData> loadedLevels = new List<LevelScrewBlockedData>();
    private string folderPath = "";

    [MenuItem("Tools/Optimize Level/Level Screw BlockedData Window")]
    public static void ShowWindow()
    {
        GetWindow<LevelScrewBlockedDataWindow>("Level Screw BlockedData Window");
    }

    private void OnEnable()
    {
        Instance = this;
    }

    void OnGUI()
    {
        GUILayout.Label("Level Data Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        startLevelIndex = EditorGUILayout.IntField("Start Level ID", startLevelIndex);
        endLevelIndex = EditorGUILayout.IntField("End Level ID", endLevelIndex);

        if (GUILayout.Button(isExporting ? "Cancel" : "Start"))
        {
            if (isExporting)
            {
                isExporting = false;
                if (cancelSource != null)
                    cancelSource.Cancel(); // báo hủy
            }
            else
                ExportAllLevels();
        }
        GUILayout.Label("-----------------------------------------------", EditorStyles.boldLabel);

        level = EditorGUILayout.IntField("Target Level", level);
        if (GUILayout.Button("Export Target Level"))
        {
            ExportTargetLevelMap();
        }

        isShowData = EditorGUILayout.Foldout(isShowData, "Show Current Level Data");
        GUILayout.Space(20);
        if (!isShowData) return;
        GUILayout.Label("Load & View Exported JSON", EditorStyles.boldLabel);

        if (GUILayout.Button("Chọn Folder JSON"))
        {
            folderPath = EditorUtility.OpenFolderPanel("Chọn thư mục chứa LevelData JSON", "Assets/_Game/OptimizeLevel/LevelDatas", "");
            if (!string.IsNullOrEmpty(folderPath))
                LoadAllJsonFiles();
        }
        if (loadedLevels.Count > 0)
        {
            if (isDetailMode && selectedLevel != null && selectedItemLevelMap != null)
            {
                selectedItemLevelMap.Draw();

            }
            else
            {
                // chế độ grid
                scrollGrid = EditorGUILayout.BeginScrollView(scrollGrid, GUILayout.Height(400));
                int countInRow = 5;
                for (int i = 0; i < loadedLevels.Count; i++)
                {
                    if (i % countInRow == 0) EditorGUILayout.BeginHorizontal();

                    var item = new ItemLevel(loadedLevels[i], (lv) =>
                    {
                        isDetailMode = true;
                        selectedLevel = lv;
                        selectedItemLevelMap = new ItemDetailLevel(selectedLevel, () =>
                        {
                            isDetailMode = false;
                            selectedLevel = null;
                        });
                    });
                    item.Draw();

                    if (i % countInRow == countInRow - 1 || i == loadedLevels.Count - 1)
                        EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
        }


    }
    public void BeginScroll()
    {
        scrollGrid = EditorGUILayout.BeginScrollView(scrollGrid, GUILayout.Height(400));

    }
    public void EndScroll()
    {
        EditorGUILayout.EndScrollView();

    }
    private void LoadAllJsonFiles()
    {
        loadedLevels.Clear();
        string[] files = Directory.GetFiles(folderPath, "LevelData_*.json", SearchOption.TopDirectoryOnly);

        // 🔥 Sắp xếp theo số level (trích từ tên file)
        var sortedFiles = files
            .Select(f => new { Path = f, Id = GetLevelIdFromFileName(f) })
            .OrderBy(f => f.Id)
            .ToList();

        foreach (var file in sortedFiles)
        {
            string json = File.ReadAllText(file.Path);
            var data = JsonUtility.FromJson<LevelScrewBlockedData>(json);
            if (data != null)
                loadedLevels.Add(data);
        }

        Debug.Log($"Đã load {loadedLevels.Count} file LevelData từ {folderPath}");
    }

    // Hàm tách số từ tên file (ví dụ: LevelData_123.json → 123)
    private int GetLevelIdFromFileName(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath); // LevelData_123
        string[] parts = fileName.Split('_');
        if (parts.Length > 1 && int.TryParse(parts[1], out int levelId))
            return levelId;
        return -1;
    }


    private void ScanShapesAndScrews(LevelMap levelMap)
    {
        levelData = new LevelScrewBlockedData();

        levelMap.TestDetectAllScrew();

        levelData.level = levelMap.LevelId;
        levelData.totalScrew = levelMap.LstScrew.Count;
        levelData.lstScrewBlockedData = new List<ScrewBlockedData>();
        levelData.lstScrewBlockedData = levelMap.LstBlockedData;

        var countItemHasShape = levelData.lstScrewBlockedData.Count(d => d.lstIndexShapeBlock != null && d.lstIndexShapeBlock.Count > 0);
        Debug.Log($"Scanned Level {levelData.level}: {levelData.lstScrewBlockedData.Count} Count: {countItemHasShape} ScrewBlockedData entries");
    }

    private async UniTask ExportAllLevels()
    {
        IngameData.IS_GEN_SCREW_BLOCK = true;
        isExporting = true;
        string basePath = EditorUtility.OpenFolderPanel("Chọn thư mục lưu tất cả LevelData", "Assets/_Game/OptimizeLevel/LevelDifficulty/Datas/ScrewBlockData", "");
        if (string.IsNullOrEmpty(basePath)) return;

        cancelSource = new CancellationTokenSource();
        var token = cancelSource.Token;

        for (int i = startLevelIndex; i <= endLevelIndex; i++)
        {
            if (i + 1 == Db.storage.USER_INFO.level)
            {

            }
            else
                await LevelController.Instance.ForceLoadLevel(i);

            var levelMap = LevelController.Instance.Level;
            if (levelMap == null || levelMap.gameObject == null)
            {
                Debug.LogWarning($"Level {i} không có LevelMap, bỏ qua.");
                continue;
            }
            levelMap.SetShapesIndex();

            float progress = (float)i / endLevelIndex;
            if (EditorUtility.DisplayCancelableProgressBar(
                "Exporting Levels",
                $"Processing Level {levelMap.LevelId} ({i + 1}/{endLevelIndex})",
                progress))
            {
                cancelSource.Cancel(); // báo hủy
                break;
            }
            await UniTask.Delay(500); // đợi một chút để tránh lag UI
            // Scan dữ liệu (có check cancel bên trong)
            ScanShapesAndScrews(levelMap);
            //   await UniTask.Delay(500); // đợi một chút để tránh lag UI

            if (token.IsCancellationRequested) break;

            string filePath = Path.Combine(basePath, $"LevelData_{levelMap.LevelId}.json");
            string json = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(filePath, json);

            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(levelMap.gameObject);
            if (prefab != null) PrefabUtility.SavePrefabAsset(prefab);
            //LoadAllJsonFiles();
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (token.IsCancellationRequested)
        {
            EditorUtility.DisplayDialog("Export Canceled", "Quá trình export đã bị hủy.", "OK");
            isExporting = false;
        }
        else
        {
            isExporting = false;
            EditorUtility.DisplayDialog("Export Done", $"Đã export {endLevelIndex} levels", "OK");
        }
        IngameData.IS_GEN_SCREW_BLOCK = false;
    }

    private async UniTask ExportTargetLevelMap()
    {
        IngameData.IS_GEN_SCREW_BLOCK = true;
        await LevelController.Instance.ForceLoadLevel(level);

        var levelMap = LevelController.Instance.Level;
        if (levelMap == null || levelMap.gameObject == null)
        {
            Debug.LogWarning($"Level {level} không có LevelMap, bỏ qua.");
            ShowNotification(new GUIContent($"Level {level} không có LevelMap, bỏ qua."));
        }
        ExportTargetLevelMap(levelMap);
        IngameData.IS_GEN_SCREW_BLOCK = false;


    }
    private async UniTask ExportTargetLevelMap(LevelMap levelMap)
    {
        IngameData.IS_GEN_SCREW_BLOCK = true;

        string basePath = "Assets/_Game/OptimizeLevel/LevelDifficulty/Datas/ScrewBlockData";
        if (string.IsNullOrEmpty(basePath)) return;
        if (levelMap == null || levelMap.gameObject == null)
        {
            Debug.LogWarning($"Level {level} không có LevelMap, bỏ qua.");
            ShowNotification(new GUIContent($"Level {level} không có LevelMap, bỏ qua."));
        }
        levelMap.SetShapesIndex();
        ScanShapesAndScrews(levelMap);

        string filePath = Path.Combine(basePath, $"LevelData_{levelMap.LevelId}.json");
        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(filePath, json);

        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(levelMap.gameObject);
        if (prefab != null) PrefabUtility.SavePrefabAsset(prefab);

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        IngameData.IS_GEN_SCREW_BLOCK = false;
        ShowNotification(new GUIContent($"Đã export Level {levelMap.LevelId} thành công!"));

    }
}
