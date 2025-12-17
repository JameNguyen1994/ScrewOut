#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class UserListSettings
{
    public int maxPerRow = 3;
    public int topCount = 10;
    public int bottomCount = 10;

    // Toggle hiển thị
    public bool showName = true;
    public bool showPoint = true;
    public bool showAvatar = false;
    public bool showBorder = false;

    public void DrawSettingsGUI()
    {
        GUILayout.Label("USER LIST VIEW SETTINGS", EditorStyles.boldLabel);

        maxPerRow = Mathf.Max(1, EditorGUILayout.IntField("Max Per Row", maxPerRow));
        topCount = Mathf.Max(0, EditorGUILayout.IntField("Top Count", topCount));
        bottomCount = Mathf.Max(0, EditorGUILayout.IntField("Bottom Count", bottomCount));

        GUILayout.Space(5);
        GUILayout.Label("FIELDS", EditorStyles.boldLabel);

        showName = EditorGUILayout.Toggle("Show Name", showName);
        showPoint = EditorGUILayout.Toggle("Show Points", showPoint);
        showAvatar = EditorGUILayout.Toggle("Show Avatar Index", showAvatar);
        showBorder = EditorGUILayout.Toggle("Show Border Index", showBorder);

        GUILayout.Space(10);
    }
}
#endif
