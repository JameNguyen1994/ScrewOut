using UnityEngine;

/// <summary>
/// ScriptableObject that stores pre-computed convex meshes generated with VHACD.
/// This allows reusing collider meshes without recalculating them each time.
/// </summary>
public class ConvexColliderData : ScriptableObject
{
    /// <summary>
    /// Array of convex meshes computed for this object.
    /// </summary>
    public Mesh[] ComputedMeshes = new Mesh[0];

#if UNITY_EDITOR
    /// <summary>
    /// Creates a new ConvexColliderData asset and attaches meshes as sub-assets.
    /// </summary>
    public static ConvexColliderData CreateAsset(string path, Mesh[] meshes)
    {
        var data = CreateInstance<ConvexColliderData>();

        UnityEditor.AssetDatabase.CreateAsset(data, path);
        AddMeshesToAsset(data, meshes);
        data.ComputedMeshes = meshes;

        return data;
    }

    /// <summary>
    /// Updates an existing asset by removing old meshes and adding new ones.
    /// </summary>
    public void UpdateAsset(Mesh[] meshes)
    {
        foreach (var mesh in ComputedMeshes)
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(mesh);

        ComputedMeshes = null;
        AddMeshesToAsset(this, meshes);
        ComputedMeshes = meshes;
    }

    /// <summary>
    /// Adds meshes as sub-assets of the given parent ScriptableObject.
    /// </summary>
    private static void AddMeshesToAsset(Object parent, Mesh[] meshes)
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].name = $"Computed Mesh {i}";
            UnityEditor.AssetDatabase.AddObjectToAsset(meshes[i], parent);
        }
    }
#endif
}