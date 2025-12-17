#if UNITY_EDITOR 
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TagWindow : EditorWindow
{
    private Object targetObject;
    private List<string> tags;
    private System.Action saveCallback;
    static private List<string> fixedTags = new List<string>();// { "Important", "ToDo", "Reviewed", "Pending" }; // Fixed list of tags

    public static void ShowWindow(Object obj, List<string> currentTags, System.Action onSave)
    {
        TagWindow window = GetWindow<TagWindow>("Edit Tags");
        window.targetObject = obj;
        window.tags = new List<string>(currentTags);
        window.saveCallback = onSave;
        Debug.Log("Test");
    }
    public void InitListTag(List<string> fixedTag)
    {
        fixedTags = fixedTag;
        Debug.Log(fixedTags.Count);
    }
    private void OnGUI()
    {
        GUILayout.Label($"Editing Tags for: {targetObject.name}", EditorStyles.boldLabel);

        foreach (string tag in fixedTags)
        {
            bool isTagged = tags.Contains(tag);
            bool newIsTagged = EditorGUILayout.ToggleLeft(tag, isTagged);
            if (newIsTagged != isTagged)
            {
                if (newIsTagged)
                {
                    tags.Add(tag);
                }
                else
                {
                    tags.Remove(tag);
                }
            }
        }

        if (GUILayout.Button("Save", GUILayout.Width(100)))
        {
            string path = AssetDatabase.GetAssetPath(targetObject);
            BookMarkWindowEditor.objectTags[path] = new List<string>(tags);
            saveCallback.Invoke();
            Close();
        }
    }
}

#endif

