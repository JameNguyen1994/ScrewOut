#if UNITY_EDITOR
using ps.modules.leaderboard;
using UnityEditor;
using UnityEngine;

public class Daily30SettingPanel
{
    public D30 Data;
    public LDGenerateData Generator;

    public void Init()
    {
        Data = LeaderboardDataService.LoadDaily30();
    }

    public void Draw()
    {
        GUILayout.Label("DAILY (ONLY 1 DAY)", EditorStyles.boldLabel);

        if (GUILayout.Button("Open Data Folder"))
            LeaderboardEditorUtils.OpenDataFolder();

        GUILayout.Space(10);
        DrawGenerator();
        GUILayout.Space(10);
        DrawCurrent();
    }

    // ===================================================================
    private void DrawGenerator()
    {
        Generator = (LDGenerateData)EditorGUILayout.ObjectField(
            "LDGenerateData",
            Generator,
            typeof(LDGenerateData),
            false
        );

        if (Generator == null)
        {
            EditorGUILayout.HelpBox("Assign LDGenerateData", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Generate New Random Day"))
        {
            int day = D30Logic.RandomDay();

            M monthData = new M { m = day, u = new System.Collections.Generic.List<U>() };

            Generator.GenerateForJson(monthData);

            // Ghi đè dữ liệu cũ
            Data.d = day;
            Data.m = monthData;

            LeaderboardDataService.SaveDaily30(Data);
        }
    }

    // ===================================================================
    private void DrawCurrent()
    {
        GUILayout.Label("CURRENT DAY DATA", EditorStyles.boldLabel);

        if (Data.m == null)
        {
            GUILayout.Label("No data generated.");
            return;
        }

        GUILayout.Label($"Day: {Data.d}");

        // Trả về để window hiển thị user
        if (GUILayout.Button("Show User List"))
        {
            // Có thể set flag hoặc callback cho Window chính
        }
    }
}
#endif
