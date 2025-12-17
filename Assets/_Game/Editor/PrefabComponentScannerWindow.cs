using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabScannerWindow : EditorWindow
{
    // ScriptableObject nội bộ để serialize list prefab
    public class PrefabListSO : ScriptableObject
    {
        public List<GameObject> prefabs = new();
    }

    private PrefabListSO dataSO;
    private SerializedObject so;
    private SerializedProperty spPrefabs;

    private string componentName = "";
    private Type targetType;
    private Vector2 scroll;

    // Lưu danh sách child có chứa component
    private Dictionary<GameObject, HashSet<Transform>> foundMap = new();

    // Lưu trạng thái foldout cha
    private Dictionary<Transform, bool> foldoutState = new();

    [MenuItem("Tools/Prefab Scanner Tree Foldout")]
    public static void ShowWindow()
    {
        GetWindow<PrefabScannerWindow>("Prefab Scanner");
    }

    private void OnEnable()
    {
        if (dataSO == null)
            dataSO = ScriptableObject.CreateInstance<PrefabListSO>();

        so = new SerializedObject(dataSO);
        spPrefabs = so.FindProperty("prefabs");
    }

    private void OnGUI()
    {
        so.Update();
        GUILayout.Label("=== PREFAB LIST ===", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(spPrefabs, true);
        so.ApplyModifiedProperties();

        GUILayout.Space(10);

        componentName = EditorGUILayout.TextField("Component Name", componentName);

        if (GUILayout.Button("SCAN", GUILayout.Height(30)))
        {
            ScanPrefabs();
        }

        GUILayout.Space(10);
        GUILayout.Label("=== RESULTS ===", EditorStyles.boldLabel);

        scroll = GUILayout.BeginScrollView(scroll);

        foreach (var kv in foundMap)
        {
            var prefab = kv.Key;
            var found = kv.Value;

            EditorGUILayout.Space(5);

            // Foldout root
            Transform root = prefab.transform;
            if (!foldoutState.ContainsKey(root))
                foldoutState[root] = true;

            foldoutState[root] = EditorGUILayout.Foldout(foldoutState[root], prefab.name, true);

            if (foldoutState[root])
            {
                EditorGUI.indentLevel++;
                DrawFilteredTree(root, found);
                EditorGUI.indentLevel--;
            }
        }

        GUILayout.EndScrollView();
    }

    // =======================================================================
    // DRAW TREE (ONLY BRANCHES THAT CONTAIN MATCHING CHILDREN)
    // =======================================================================
    private bool DrawFilteredTree(Transform node, HashSet<Transform> matches)
    {
        bool hasMatchInSubtree = matches.Contains(node);

        // Check children
        List<Transform> matchedChildren = new();
        foreach (Transform child in node)
        {
            if (SubtreeContainsMatch(child, matches))
                matchedChildren.Add(child);
        }

        // Nếu node không match và không có con match → đừng hiện
        if (!hasMatchInSubtree && matchedChildren.Count == 0)
            return false;

        // Nếu đây là node match → highlight
        if (matches.Contains(node))
        {
            GUI.color = Color.green;
            EditorGUILayout.LabelField("✔ " + node.name);
            GUI.color = Color.white;
        }
        else if (node != node.root) // node gốc đã foldout riêng
        {
            EditorGUILayout.LabelField(node.name);
        }

        // Vẽ con
        EditorGUI.indentLevel++;
        foreach (var child in matchedChildren)
        {
            DrawFilteredTree(child, matches);
        }
        EditorGUI.indentLevel--;

        return true;
    }

    private bool SubtreeContainsMatch(Transform node, HashSet<Transform> matches)
    {
        if (matches.Contains(node))
            return true;

        foreach (Transform child in node)
        {
            if (SubtreeContainsMatch(child, matches))
                return true;
        }
        return false;
    }

    // =======================================================================
    // SCAN PREFAB
    // =======================================================================
    private void ScanPrefabs()
    {
        foundMap.Clear();
        foldoutState.Clear();

        if (string.IsNullOrEmpty(componentName))
        {
            Debug.LogError("Chưa nhập component!");
            return;
        }

        targetType = GetTypeByName(componentName);
        if (targetType == null)
        {
            Debug.LogError($"Không tìm thấy class: {componentName}");
            return;
        }

        foreach (var prefab in dataSO.prefabs)
        {
            if (prefab == null) continue;

            HashSet<Transform> matches = new();

            FindMatches(prefab.transform, matches);

            if (matches.Count > 0)
                foundMap[prefab] = matches;
        }
    }

    private void FindMatches(Transform node, HashSet<Transform> result)
    {
        if (node.GetComponent(targetType) != null)
            result.Add(node);

        foreach (Transform child in node)
            FindMatches(child, result);
    }

    private Type GetTypeByName(string name)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in asm.GetTypes())
            {
                if (t.Name == name || t.FullName == name)
                    return t;
            }
        }
        return null;
    }
}
