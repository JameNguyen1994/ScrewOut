#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class SAMeshColliderBuilderCleaner
{
    // Add to GameObject right-click context menu
    [MenuItem("GameObject/Cleanup/Clean SAMeshColliderBuilder & Master_New", false, 49)]
    public static void CleanSelected()
    {
        foreach (var obj in Selection.gameObjects)
        {
            CleanSAMeshColliderBuilderAndMasterNew(obj);
        }
    }

    // Validation: Only show if at least one selected object has SAMeshColliderBuilder
    [MenuItem("GameObject/Cleanup/Clean SAMeshColliderBuilder & Master_New", true)]
    public static bool CleanSelected_Validate()
    {
        foreach (var obj in Selection.gameObjects)
        {
            if (obj.GetComponentInChildren<SAMeshColliderBuilder>(true) != null)
                return true;
        }
        return false;
    }

    public static void CleanSAMeshColliderBuilderAndMasterNew(GameObject root)
{
    var allBuilders = root.GetComponentsInChildren<SAMeshColliderBuilder>(true);

    foreach (var builder in allBuilders)
    {
        var parent = builder.gameObject.transform;
        var toRemove = new System.Collections.Generic.List<GameObject>();

        for (int i = 0; i < parent.childCount; ++i)
        {
            var child = parent.GetChild(i).gameObject;
            if (child.name == "Master_New" || child.name.StartsWith("Master_New"))
            {
                toRemove.Add(child);
            }
        }

        foreach (var child in toRemove)
        {
            string childName = child.name;
            string parentName = parent.name;
            Object.DestroyImmediate(child);
            Debug.Log($"Removed child '{childName}' from '{parentName}'");
        }

        foreach (var col in builder.GetComponents<MeshCollider>())
            Object.DestroyImmediate(col, true);

        Object.DestroyImmediate(builder, true);

        EditorUtility.SetDirty(parent.gameObject);
    }
}

}
#endif
