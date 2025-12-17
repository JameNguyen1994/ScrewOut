#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class YearLeaderboardWindow : EditorWindow
{
    private enum DisplayMode { Year, Daily30 }

    private DisplayMode Mode = DisplayMode.Year;

    private YearSettingPanel yearPanel = new YearSettingPanel();
    private Daily30SettingPanel dailyPanel = new Daily30SettingPanel();

    private UserListSettings listSettings = new UserListSettings();
    private Vector2 scroll;


    [MenuItem("Tools/Leaderboard JSON")]
    public static void Open()
    {
        GetWindow<YearLeaderboardWindow>("Leaderboard JSON");
    }


    private void OnEnable()
    {
        yearPanel.Year = 2025;
        yearPanel.Load();
        dailyPanel.Init();
    }


    private void OnGUI()
    {
        GUILayout.Label("LEADERBOARD TOOL", EditorStyles.boldLabel);

        Mode = (DisplayMode)EditorGUILayout.EnumPopup("View Mode", Mode);

        GUILayout.Space(10);

        if (Mode == DisplayMode.Year)
        {
            yearPanel.Draw();
            GUILayout.Space(15);

            DrawUserList(yearPanel.CurrentUsers);
        }
        else
        {
            dailyPanel.Draw();
            GUILayout.Space(15);

            DrawUserList(dailyPanel.Data.m.u);
        }
    }


    private void DrawUserList(List<U> users)
    {
        listSettings.DrawSettingsGUI();

        UserListDrawer.DrawUserList(
            users,
            ref scroll,
            listSettings,
            (Mode == DisplayMode.Year && users != null)
                ? (System.Action<int>)(i => users.RemoveAt(i))
                : null
        );
    }
}
#endif
