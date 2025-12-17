using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SavePositionObjectsWindow : EditorWindow
{
    [System.Serializable]
    public class ObjectPosition
    {
        public string name;
        public Vector3 position;
    }

    private List<ObjectPosition> savedPositions = new List<ObjectPosition>();
    private Vector2 scrollPosRight;
    private GameObject rootObject;

    [MenuItem("Tools/Save Position Objects")]
    public static void ShowWindow()
    {
        GetWindow<SavePositionObjectsWindow>("Save Position Objects");
    }

    private void OnGUI()
    {
        GUILayout.Space(5);
        GUILayout.Label("🎯 Save & Paste Positions", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "1️⃣ Chọn GameObject gốc → 📂 Get From Root để lưu vị trí và xóa con.\n" +
            "2️⃣ Khi có danh sách mới trong root → 📥 Paste To Root để gán lại vị trí theo thứ tự.",
            MessageType.Info);

        GUILayout.Space(5);
        rootObject = (GameObject)EditorGUILayout.ObjectField("Root Object:", rootObject, typeof(GameObject), true);

        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📂 Get From Root (Save & Delete)", GUILayout.Height(30)))
            GetFromRootAndDelete();
        if (GUILayout.Button("📥 Paste To Root (Apply Saved Positions)", GUILayout.Height(30)))
            PastePositionsToRoot();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("📋 Saved Positions", EditorStyles.boldLabel);

        scrollPosRight = EditorGUILayout.BeginScrollView(scrollPosRight, GUILayout.Height(300));
        if (savedPositions.Count == 0)
        {
            EditorGUILayout.HelpBox("Chưa có dữ liệu. Nhấn 'Get From Root' để lưu vị trí.", MessageType.Info);
        }
        else
        {
            for (int i = 0; i < savedPositions.Count; i++)
            {
                var data = savedPositions[i];
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"{i + 1}. {data.name}", GUILayout.Width(150));
                EditorGUILayout.Vector3Field(GUIContent.none, data.position);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("🧹 Clear Saved Positions", GUILayout.Height(25)))
        {
            savedPositions.Clear();
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("⚠️ Cảnh báo: GameObject bị xóa vĩnh viễn khỏi Scene (không Undo được).", MessageType.Warning);
    }

    // =====================================================================
    // 1️⃣ GET FROM ROOT + DELETE
    // =====================================================================
    private void GetFromRootAndDelete()
    {
        if (rootObject == null)
        {
            EditorUtility.DisplayDialog("⚠️ Lỗi", "Chưa chọn GameObject gốc!", "OK");
            return;
        }

        var children = new List<GameObject>();
        foreach (Transform child in rootObject.GetComponentsInChildren<Transform>(true))
        {
            if (child.gameObject != rootObject && child.gameObject.GetComponent<Shape>()==null)
                children.Add(child.gameObject);
        }

        if (children.Count == 0)
        {
            EditorUtility.DisplayDialog("❌ Không có con", "GameObject gốc không có con nào để xử lý.", "OK");
            return;
        }

        savedPositions.Clear();
        foreach (var go in children)
        {
            savedPositions.Add(new ObjectPosition
            {
                name = go.name,
                position = go.transform.position
            });
        }


        Debug.Log($"✅ Đã lưu {savedPositions.Count} vị trí và xóa {children.Count} GameObject con của '{rootObject.name}'.");
    }

    // =====================================================================
    // 2️⃣ PASTE TO ROOT
    // =====================================================================
    private void PastePositionsToRoot()
    {
        if (rootObject == null)
        {
            EditorUtility.DisplayDialog("⚠️ Lỗi", "Chưa chọn GameObject gốc!", "OK");
            return;
        }

        if (savedPositions == null || savedPositions.Count == 0)
        {
            EditorUtility.DisplayDialog("❌ Không có dữ liệu", "Chưa có danh sách vị trí để paste.", "OK");
            return;
        }

        var children = new List<Transform>();
        foreach (Transform child in rootObject.GetComponentsInChildren<Transform>(true))
        {
            if (child.gameObject != rootObject && child.gameObject.GetComponent<Shape>() == null)
                children.Add(child);
        }

        if (children.Count == 0)
        {
            EditorUtility.DisplayDialog("❌ Không có con", "GameObject gốc chưa có object con để paste.", "OK");
            return;
        }

        int count = Mathf.Min(savedPositions.Count, children.Count);
        for (int i = 0; i < count; i++)
        {
            children[i].position = savedPositions[i].position;
        }

        Debug.Log($"📥 Gán lại {count} vị trí cho {rootObject.name}. " +
                  $"(Saved: {savedPositions.Count}, Children: {children.Count})");

        if (savedPositions.Count != children.Count)
        {
            Debug.LogWarning("⚠️ Số lượng savedPositions và con không khớp — chỉ áp dụng theo thứ tự chung nhỏ nhất.");
        }
        EditorUtility.SetDirty(rootObject);
    }

}
