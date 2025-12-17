// Editor/LeaderboardMockGenerator.cs
using UnityEditor;
using UnityEngine;
using System.IO;
using ps.modules.leaderboard;

public static class LeaderboardMockGenerator
{
    /// Generate cho 1 Month (tự tạo LeaderboardDataSO nếu thiếu)
    public static void GenerateForMonth(LDGenerateData settings, MonthDataSO month)
    {
        if (settings == null || month == null) return;

        if (month.data == null)
        {
            // nếu thiếu, tạo LeaderboardDataSO cạnh MonthData
            var monthPath = AssetDatabase.GetAssetPath(month);
            var dir = Path.GetDirectoryName(monthPath)?.Replace("\\", "/");
            var lbPath = Path.Combine(dir, $"Leaderboard_{month.month}.asset").Replace("\\", "/");

            var lb = ScriptableObject.CreateInstance<LeaderboardDataSO>();
            AssetDatabase.CreateAsset(lb, lbPath);
            month.data = lb;
            EditorUtility.SetDirty(month);
        }

        Undo.RecordObject(month.data, "Generate Leaderboard");
        settings.ApplyTo(month.data);
        EditorUtility.SetDirty(month.data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// Generate cho 12 tháng của 1 năm (bỏ qua tháng null)
    public static void GenerateForYear(LDGenerateData settings, YearDataSO year)
    {
        if (settings == null || year == null || year.lstMonthData == null) return;

        foreach (var m in year.lstMonthData)
            GenerateForMonth(settings, m);
    }

    /// Generate cho toàn bộ AllYear
    public static void GenerateForAllYears(LDGenerateData settings, AllYearData all)
    {
        if (settings == null || all == null || all.lstYearData == null) return;

        foreach (var y in all.lstYearData)
            GenerateForYear(settings, y);
    }
}
