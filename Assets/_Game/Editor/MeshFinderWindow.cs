using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using VHACD.Unity;

public class MeshFinderWindow : EditorWindow
{
    private string targetString = "";
    private GameObject rootObject;

    [MenuItem("Tools/Mesh Finder")]
    public static void ShowWindow()
    {
        GetWindow<MeshFinderWindow>("Mesh Finder");
    }

    private void OnGUI()
    {
        targetString = EditorGUILayout.TextField("Target String", targetString);
        rootObject = (GameObject)EditorGUILayout.ObjectField("Root GameObject", rootObject, typeof(GameObject), true);

        if (GUILayout.Button("Find & Process"))
        {
            if (!string.IsNullOrEmpty(targetString) && rootObject != null)
            {
                Debug.Log($"Start Find: \"{targetString}\" in GameObject: {rootObject.name}");
                ProcessChildren(rootObject.transform);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign both string and GameObject.", "OK");
            }
        }
    }

    private void ProcessChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(targetString))
            {
                Debug.Log($"Found: {child.name}", child.gameObject);
                // Add Component
                var completex = child.AddComponent<ComplexCollider>();
                
            }
            ProcessChildren(child);
        }
    }
}