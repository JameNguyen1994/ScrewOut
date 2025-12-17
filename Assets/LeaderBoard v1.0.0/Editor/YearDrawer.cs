using UnityEditor;
using UnityEngine;
using System.Collections.Concurrent;
using ps.modules.leaderboard;


public class YearDrawer
{
    private readonly ConcurrentDictionary<YearDataSO, bool> _foldoutYear = new();
    private readonly System.Action<YearDataSO> _onCreateMonthsAndLeaderboards;

    public YearDrawer(System.Action<YearDataSO> onCreateMonthsAndLeaderboards)
    {
        _onCreateMonthsAndLeaderboards = onCreateMonthsAndLeaderboards;
    }

    /// <summary>
    /// Vẽ 1 YearData và danh sách tháng bên trong (dùng MonthLeaderboardDrawer để vẽ tháng)
    /// </summary>
    public void DrawYear(YearDataSO yearData, MonthLeaderboardDrawer monthDrawer)
    {
        if (yearData == null) return;

        _foldoutYear.TryGetValue(yearData, out bool isOpenYear);
        bool newOpen = EditorGUILayout.Foldout(isOpenYear, $"📅 Year {yearData.year}", true);
        _foldoutYear[yearData] = newOpen;

        if (!newOpen) return;

        EditorGUI.indentLevel++;
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Year Asset", yearData, typeof(YearDataSO), false);
            EditorGUILayout.LabelField("Months", (yearData.lstMonthData?.Count ?? 0).ToString());
        }

        // Nút tạo nhanh 12 tháng + leaderboard
        if (GUILayout.Button($"📦 Tạo 12 tháng + Leaderboard cho {yearData.year}", GUILayout.Width(340)))
        {
            _onCreateMonthsAndLeaderboards?.Invoke(yearData);
        }

        if (yearData.lstMonthData == null || yearData.lstMonthData.Count == 0)
        {
            EditorGUILayout.HelpBox("⚠️ Năm này chưa có dữ liệu tháng.", MessageType.None);
            EditorGUI.indentLevel--;
            return;
        }

        // Duyệt & vẽ từng tháng bằng MonthLeaderboardDrawer
        foreach (var month in yearData.lstMonthData)
        {
            monthDrawer.DrawMonth(month);
        }

        EditorGUI.indentLevel--;
    }
}
