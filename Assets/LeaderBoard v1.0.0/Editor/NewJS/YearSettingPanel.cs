#if UNITY_EDITOR
using ps.modules.leaderboard;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class YearSettingPanel
{
    public int Year;
    public Y YearData;

    public int SelectedMonth = 1;
    public LDGenerateData Generator;

    // Trả danh sách user cho tháng được chọn
    public List<U> CurrentUsers =>
        (YearData != null && SelectedMonth >= 1 && SelectedMonth <= 12)
        ? YearData.m[SelectedMonth - 1].u
        : null;


    // ===================================================================
    public void Load()
    {
        YearData = LeaderboardDataService.LoadYear(Year);
    }

    // ===================================================================
    public void Draw()
    {
        GUILayout.Label("YEAR SETTINGS", EditorStyles.boldLabel);

        Year = EditorGUILayout.IntField("Year", Year);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Year JSON")) Load();
        if (GUILayout.Button("Save Year JSON"))
        {
            if (YearData != null)
                LeaderboardDataService.SaveYear(YearData);
        }
        GUILayout.EndHorizontal();

        // --------------------------------------------------------------
        if (GUILayout.Button("Open Data Folder"))
            LeaderboardEditorUtils.OpenDataFolder();

        if (YearData == null)
        {
            EditorGUILayout.HelpBox("Year JSON not loaded.", MessageType.Warning);
            return;
        }

        GUILayout.Space(10);

        SelectedMonth = EditorGUILayout.IntSlider("Month", SelectedMonth, 1, 12);

        GUILayout.Space(10);
        DrawGeneratorSection();
    }


    // ===================================================================
    private void DrawGeneratorSection()
    {
        GUILayout.Label("MOCK GENERATOR", EditorStyles.boldLabel);

        Generator = (LDGenerateData)EditorGUILayout.ObjectField(
            "Data Source",
            Generator,
            typeof(LDGenerateData),
            false
        );

        if (Generator == null)
        {
            EditorGUILayout.HelpBox("Assign LDGenerateData to generate mock data.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Generate This Month"))
        {
            Generator.GenerateForJson(YearData.m[SelectedMonth - 1]);
            LeaderboardDataService.SaveYear(YearData);
        }

        if (GUILayout.Button("Generate All 12 Months"))
        {
            foreach (var month in YearData.m)
                Generator.GenerateForJson(month);

            LeaderboardDataService.SaveYear(YearData);
        }
    }
}
#endif
