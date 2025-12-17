using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using ps.modules.leaderboard;

public class LeaderBoardDataWindow : EditorWindow
{
    private AllYearData allYearData;
    private Vector2 scrollPos;
    private int inputYear = System.DateTime.Now.Year;

    // Settings generate
    private LDGenerateData generateSettings;
    private LeaderboardDataSO targetLeaderboard;   // 🎯 asset chỉ định để generate trực tiếp

    // Drawers
    private YearDrawer yearDrawer;
    private MonthLeaderboardDrawer monthDrawer;

    [MenuItem("Tools/Leaderboard/Leaderboard Data Window")]
    public static void ShowWindow()
    {
        var w = GetWindow<LeaderBoardDataWindow>("Leaderboard Data Manager");
        w.minSize = new Vector2(720, 560);
        w.InitDrawers();
        w.FindAllYearDataAuto();
        w.Show();
    }

    private void OnEnable() => InitDrawers();

    private void InitDrawers()
    {
        if (yearDrawer == null)
            yearDrawer = new YearDrawer(CreateMonthAndLeaderboard); // callback tạo 12 tháng + LB
        if (monthDrawer == null)
            monthDrawer = new MonthLeaderboardDrawer(() => generateSettings, OnGenerateOneMonth);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🏆 Leaderboard Data Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // --- Load AllYearData ---
        if (allYearData == null)
        {
            if (GUILayout.Button("🔍 Tự động tìm AllYearData"))
                FindAllYearDataAuto();

            EditorGUILayout.HelpBox("Không tìm thấy AllYearData. Hãy nhấn nút trên để tự tìm hoặc kéo thả thủ công.", MessageType.Warning);
            allYearData = (AllYearData)EditorGUILayout.ObjectField("All Year Data", allYearData, typeof(AllYearData), false);
            return;
        }

        using (new EditorGUI.DisabledScope(true))
        {
            allYearData = (AllYearData)EditorGUILayout.ObjectField("All Year Data", allYearData, typeof(AllYearData), false);
        }

        // --- Generate settings ---
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("⚙️ Generate Settings", EditorStyles.boldLabel);
        generateSettings = (LDGenerateData)EditorGUILayout.ObjectField("LDGenerateData", generateSettings, typeof(LDGenerateData), false);

        // 🎯 --- Generate trực tiếp vào 1 asset cụ thể ---
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("🎯 Generate vào Leaderboard cụ thể", EditorStyles.boldLabel);
        targetLeaderboard = (LeaderboardDataSO)EditorGUILayout.ObjectField("Target Leaderboard", targetLeaderboard, typeof(LeaderboardDataSO), false);

        using (new EditorGUI.DisabledScope(generateSettings == null || targetLeaderboard == null))
        {
            if (GUILayout.Button("⚙️ Generate vào asset này", GUILayout.Width(240)))
            {
                generateSettings.ApplyTo(targetLeaderboard);
                EditorUtility.SetDirty(targetLeaderboard);
                AssetDatabase.SaveAssets();
                Debug.Log($"✅ Generated mock data into: {targetLeaderboard.name}");
                EditorGUIUtility.PingObject(targetLeaderboard);
            }
        }

        // --- Generate toàn bộ / theo năm ---
        using (new EditorGUI.DisabledScope(generateSettings == null))
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("⚙️ Generate TẤT CẢ (AllYear)", GUILayout.Width(220)))
                LeaderboardMockGenerator.GenerateForAllYears(generateSettings, allYearData);

            if (GUILayout.Button("⚙️ Generate TẤT CẢ NĂM (each Year)", GUILayout.Width(260)))
            {
                foreach (var y in allYearData.lstYearData)
                    LeaderboardMockGenerator.GenerateForYear(generateSettings, y);
            }
            EditorGUILayout.EndHorizontal();
        }

        // --- Create new Year ---
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("🆕 Tạo dữ liệu năm mới", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal("box");
        inputYear = EditorGUILayout.IntField("Nhập năm:", inputYear);
        if (GUILayout.Button("➕ Tạo Năm Mới", GUILayout.Width(150)))
        {
            var created = LeaderboardDataFactory.CreateYear(allYearData, inputYear);
            if (created != null) EditorGUIUtility.PingObject(created);
        }
        EditorGUILayout.EndHorizontal();

        // --- Hiển thị dữ liệu ---
        EditorGUILayout.Space(8);
        DrawAllYearData();
    }

    // 🔍 Tự động tìm AllYearData trong project
    private void FindAllYearDataAuto()
    {
        var guids = AssetDatabase.FindAssets("t:AllYearData");
        if (guids.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            allYearData = AssetDatabase.LoadAssetAtPath<AllYearData>(path);
            Debug.Log($"✅ Đã tự động tìm thấy AllYearData: {path}");
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy asset nào có type AllYearData trong project.");
        }
    }

    // 🧱 Callback: tạo 12 tháng + leaderboard
    private void CreateMonthAndLeaderboard(YearDataSO yearData)
    {
        LeaderboardDataFactory.CreateMonthsAndLeaderboards(yearData, overwrite: false);
        EditorGUIUtility.PingObject(yearData);
    }

    // 🧩 Callback: generate 1 tháng
    private void OnGenerateOneMonth(MonthDataSO month)
    {
        if (generateSettings == null) return;
        LeaderboardMockGenerator.GenerateForMonth(generateSettings, month);
        if (month.data != null) EditorGUIUtility.PingObject(month.data);
    }

    // --- Hiển thị toàn bộ cây dữ liệu ---
    private void DrawAllYearData()
    {
        if (allYearData.lstYearData == null || allYearData.lstYearData.Count == 0)
        {
            EditorGUILayout.HelpBox("Chưa có dữ liệu năm nào.", MessageType.Info);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var year in allYearData.lstYearData)
        {
            if (year == null) continue;
            EditorGUILayout.BeginVertical("box");
            yearDrawer.DrawYear(year, monthDrawer);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }
}
