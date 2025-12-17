using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR

public class TagManagementWindow : EditorWindow
{
    private static List<string> fixedTags = new List<string>();// { "Important", "ToDo", "Reviewed", "Pending" }; // Fixed list of tags
    private System.Action saveCallback;

    // [MenuItem("Tam's Window/Bookmarks/TagManagementWindow")]
    public static void ShowWindow(System.Action onSave)
    {
        TagManagementWindow window = GetWindow<TagManagementWindow>("Tag Management");
        window.minSize = new Vector2(200, 300); // Set window size
        window.saveCallback = onSave;
    }
    public static void SetCallBackSave(System.Action onSave)
    {
        TagManagementWindow window = GetWindow<TagManagementWindow>("Tag Management");
        window.saveCallback = onSave;
    }

    private void OnGUI()
    {
        GUILayout.Label("Fixed Tags", EditorStyles.boldLabel);

        foreach (var tag in fixedTags)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(tag);
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                if (EditorUtility.DisplayDialog("Delete Tag", $"Are you sure you want to delete tag '{tag}'?", "Delete", "Cancel"))
                {
                    fixedTags.Remove(tag);
                    SaveFixedTags();
                    Repaint();
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("New Tag:");
        newTag = EditorGUILayout.TextField(newTag);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Tag", GUILayout.Width(100)))
        {
            if (!string.IsNullOrEmpty(newTag) && !fixedTags.Contains(newTag))
            {
                fixedTags.Add(newTag);
                newTag = "";
                SaveFixedTags();
                Repaint();
            }
        }
        GUILayout.EndHorizontal();
    }

    private void SaveFixedTags()
    {
        saveCallback?.Invoke();
    }

    public static void InitListTag(List<string> fixedTag)
    {
        fixedTags = fixedTag;
    }

    private string newTag = "";
}

#endif
