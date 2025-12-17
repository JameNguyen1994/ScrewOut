using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SmartConvexDecomposerTool : EditorWindow
{
    private GameObject targetObject;
    private bool combineMeshes = true;
    private int maxTrisPerConvex = 255;
    private Vector2 scrollPos;
    private List<Bounds> previewBounds = new List<Bounds>();

    [MenuItem("Tools/Collider/Smart Convex Decomposer Tool")]
    private static void ShowWindow()
    {
        var window = GetWindow<SmartConvexDecomposerTool>();
        window.titleContent = new GUIContent("Smart Convex Decomposer");
        window.minSize = new Vector2(380, 260);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Smart Convex Collider Generator", EditorStyles.boldLabel);

        targetObject = (GameObject)EditorGUILayout.ObjectField("Target GameObject", targetObject, typeof(GameObject), true);
        combineMeshes = EditorGUILayout.Toggle("Combine Meshes Before Compute", combineMeshes);
        maxTrisPerConvex = EditorGUILayout.IntField("Max Triangles per Convex", maxTrisPerConvex);

        EditorGUILayout.Space();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(50));
        GUILayout.Label("Preview bounding boxes will appear in Scene View.");
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Preview Colliders"))
        {
            if (targetObject != null)
            {
                previewBounds = ConvexDecomposerService.GeneratePreviewBounds(targetObject, combineMeshes, maxTrisPerConvex);
                SceneView.RepaintAll();
            }
            else
                EditorUtility.DisplayDialog("Error", "Please select a GameObject with MeshFilter!", "OK");
        }

        if (GUILayout.Button("Generate Convex Colliders"))
        {
            if (targetObject != null)
            {
                ConvexDecomposerService.GenerateConvexColliders(targetObject, combineMeshes, maxTrisPerConvex);
                previewBounds.Clear();
                SceneView.RepaintAll();
            }
            else
                EditorUtility.DisplayDialog("Error", "Please select a GameObject with MeshFilter!", "OK");
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset"))
        {
            if (targetObject != null)
            {
                ConvexDecomposerService.ResetColliders(targetObject);
                previewBounds.Clear();
                SceneView.RepaintAll();
                Debug.Log("⚡ Reset completed for " + targetObject.name);
            }
        }

        if (GUILayout.Button("Deconvex"))
        {
            if (targetObject != null)
            {
                ConvexDecomposerService.DeconvexColliders(targetObject);
                previewBounds.Clear();
                SceneView.RepaintAll();
                Debug.Log("⚡ Deconvex completed for " + targetObject.name);
            }
        }
    }

    // =================== Scene Preview ===================
    private void OnSceneGUI(SceneView sceneView)
    {
        if (previewBounds.Count == 0) return;

        Handles.color = Color.green;
        foreach (var b in previewBounds)
        {
            Handles.DrawWireCube(b.center, b.size);
        }
    }

    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;
}