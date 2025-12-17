using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ScrewOverlapCheckEditor : EditorWindow
{
    private GameObject targetObject;
    private float groupDistance = 1f;

    [MenuItem("Tools/Screw Overlap Checker")]
    public static void ShowWindow()
    {
        GetWindow<ScrewOverlapCheckEditor>("Screw Overlap Checker");
    }

    void OnGUI()
    {
        GUILayout.Label("Group Screws by Distance", EditorStyles.boldLabel);

        targetObject = (GameObject)EditorGUILayout.ObjectField("LevelMap Object", targetObject, typeof(GameObject), true);
        groupDistance = EditorGUILayout.FloatField("Group Distance", groupDistance);

        if (targetObject != null && GUILayout.Button("Group Screws"))
        {
            GroupScrews();
        }
    }

    void GroupScrews()
    {
        LevelMap levelMap = targetObject.GetComponent<LevelMap>();
        if (levelMap == null)
        {
            Debug.LogError("GameObject không chứa script LevelMap.");
            return;
        }

        var screws = levelMap.LstScrew;
        if (screws == null || screws.Count == 0)
        {
            Debug.LogWarning("Không tìm thấy lstScrew hoặc danh sách rỗng.");
            return;
        }

        List<List<Screw>> groups = new List<List<Screw>>();
        HashSet<Screw> visited = new HashSet<Screw>();

        foreach (var screw in screws)
        {
            if (screw == null || visited.Contains(screw)) continue;

            List<Screw> group = new List<Screw>();
            Queue<Screw> queue = new Queue<Screw>();
            queue.Enqueue(screw);
            visited.Add(screw);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                group.Add(current);

                foreach (var other in screws)
                {
                    if (other == null || visited.Contains(other)) continue;

                    if (Vector3.Distance(current.transform.position, other.transform.position) <= groupDistance)
                    {
                        visited.Add(other);
                        queue.Enqueue(other);
                    }
                }
            }

            // ✅ Chỉ thêm nhóm nếu có từ 2 screw trở lên
            if (group.Count >= 2)
            {
                groups.Add(group);
            }
        }

        // Log theo từng nhóm
        for (int i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            HashSet<string> parentNames = new HashSet<string>();

            Debug.Log($"Nhóm {i + 1}:");
            foreach (var screw in group)
            {
                if (screw.transform.parent != null)
                {
                    //parentNames.Add(screw.transform.parent.name);
                    Debug.Log($" - Parent: {screw.transform.parent.name} --- ID: {screw.transform.parent.GetSiblingIndex()}");
                }
                    
            }
            
            // foreach (var name in parentNames)
            // {
            //     Debug.Log($" - Parent: {name}");
            // }
        }

        Debug.Log($"Đã tìm thấy {groups.Count} nhóm có từ 2 screw trở lên.");
    }
}
