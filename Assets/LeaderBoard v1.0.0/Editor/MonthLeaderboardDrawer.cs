// Editor/MonthLeaderboardDrawer.cs
using UnityEditor;
using UnityEngine;
using System.Collections.Concurrent;
using ps.modules.leaderboard;

public class MonthLeaderboardDrawer
{
    private readonly ConcurrentDictionary<MonthDataSO, bool> _foldoutMonth = new();
    private readonly System.Func<LDGenerateData> _getSettings;   // lấy settings từ window
    private readonly System.Action<MonthDataSO> _onGenerateOne;  // callback generate

    public MonthLeaderboardDrawer(System.Func<LDGenerateData> getSettings, System.Action<MonthDataSO> onGenerateOne)
    {
        _getSettings = getSettings;
        _onGenerateOne = onGenerateOne;
    }

    public void DrawMonth(MonthDataSO monthData)
    {
        if (monthData == null) return;

        _foldoutMonth.TryGetValue(monthData, out bool open);
        bool newOpen = EditorGUILayout.Foldout(open, $"🗓 Tháng {monthData.month}", true);
        _foldoutMonth[monthData] = newOpen;

        if (!newOpen) return;

        EditorGUI.indentLevel++;
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Month Asset", monthData, typeof(MonthDataSO), false);
            EditorGUILayout.LabelField("Tháng", monthData.month.ToString());
            if (monthData.data != null)
                EditorGUILayout.ObjectField("Leaderboard Asset", monthData.data, typeof(LeaderboardDataSO), false);
            else
                EditorGUILayout.HelpBox("⚠️ Tháng này chưa gán LeaderboardDataSO.", MessageType.Info);
        }

        // nút Generate riêng cho tháng
        using (new EditorGUI.DisabledScope(_getSettings?.Invoke() == null))
        {
            if (GUILayout.Button("⚙️ Generate từ Settings (tháng này)", GUILayout.Width(280)))
                _onGenerateOne?.Invoke(monthData);
        }

        EditorGUI.indentLevel--;
    }
}
