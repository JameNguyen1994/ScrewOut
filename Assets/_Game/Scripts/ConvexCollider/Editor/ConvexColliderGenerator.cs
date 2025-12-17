using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using VHACD.Unity;

/// <summary>
/// Handles the generation of convex colliders using VHACD and saving them as assets.
/// Provides editor-side logic to generate colliders for a GameObject.
/// </summary>
public class ConvexColliderGenerator
{
    /*
    // Example menu integration (optional):
    [MenuItem("Tools/Convex Colliders/Generate Selected")]
    public static void GenerateSelectedColliders()
    {
        if (Selection.objects != null
            && Selection.objects.Length > 0
            && Selection.objects[0] is GameObject selectedGameObject)
        {
            CreateColliders(selectedGameObject, 2, 0); // quality=2 (high), groupId=0
        }
    }
    */

    /// <summary>
    /// Creates convex colliders for a GameObject, saves them into an asset,
    /// and attaches MeshColliders to the GameObject.
    /// </summary>
    /// <param name="baseGameObject">The GameObject to process.</param>
    /// <param name="quality">Quality preset (0=Low, 1=Medium, 2=High, 3=Insane).</param>
    /// <param name="groupName">Folder grouping</param>
    public static void CreateColliders(GameObject baseGameObject, int quality, string groupName)
    {
        // VHACD parameters
        var parameters = new Parameters();
        parameters.Init();
        parameters.m_resolution = ConvexColliderService.Resolution(quality);
        parameters.m_maxConvexHulls = ConvexColliderService.ConvexHulls(quality);

        var generatedMeshes = new List<Mesh>();
        var meshesToProcess = new List<Mesh>();
        var transformsToProcess = new List<Matrix4x4>();

        // Clear existing colliders before generating new ones
        ConvexColliderService.GenOrClearColliders(null, baseGameObject);

        // Extract meshes from the MeshFilter
        if (!TryExtractMeshesFromFilter(baseGameObject, meshesToProcess, transformsToProcess))
            return;

        // Run VHACD for each mesh
        var tempMeshes = new List<Mesh>();
        int progressIndex = 1;

        foreach (var mesh in meshesToProcess)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayProgressBar(
                "Calculating Colliders",
                $"Processing mesh... ({progressIndex++}/{meshesToProcess.Count}) (this can take a while)",
                Mathf.Lerp(0.4f, 0.7f, Mathf.InverseLerp(1, meshesToProcess.Count + 1, progressIndex))
            );

            VHACDProcessor.GenerateConvexMeshes(mesh, parameters, out tempMeshes);
            generatedMeshes.AddRange(tempMeshes);
        }

        if (generatedMeshes.Count == 0)
        {
            EditorUtility.DisplayDialog("Error",
                "No convex meshes were generated. Try adjusting quality parameters and try again.",
                "Ok");
            return;
        }

        // Save results into a ConvexColliderData asset
        var colliderData = SaveConvexColliderData(baseGameObject, generatedMeshes, groupName);

        // Attach MeshColliders to the GameObject
        ConvexColliderService.GenOrClearColliders(colliderData, baseGameObject);

        // Cleanup
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorGUIUtility.PingObject(colliderData);
    }

    /// <summary>
    /// Attempts to extract submeshes from the GameObject's MeshFilter.
    /// Each submesh is separated into an individual Mesh for VHACD processing.
    /// </summary>
    private static bool TryExtractMeshesFromFilter(GameObject baseGameObject,
        List<Mesh> meshesToProcess, List<Matrix4x4> transformsToProcess)
    {
        if (!baseGameObject.TryGetComponent<MeshFilter>(out var filter))
        {
            EditorUtility.ClearProgressBar();
            ConvexColliderService.ShowMissingMeshFilter();
            return false;
        }

        if (filter.sharedMesh == null)
        {
            EditorUtility.ClearProgressBar();
            ConvexColliderService.ShowMissingMeshes();
            return false;
        }

        for (int i = 0; i < filter.sharedMesh.subMeshCount; i++)
        {
            meshesToProcess.Add(ConvexColliderService.ExtractSubmesh(filter.sharedMesh, i));
            transformsToProcess.Add(baseGameObject.transform.worldToLocalMatrix * filter.transform.localToWorldMatrix);
        }

        return true;
    }

    /// <summary>
    /// Saves generated meshes as a ConvexColliderData asset in Assets/ConvexColliders/Group_X.
    /// If an asset already exists, it will be updated instead.
    /// </summary>
    public static ConvexColliderData SaveConvexColliderData(GameObject baseGameObject, List<Mesh> meshes, string groupName)
    {
        string rootFolder = EnsureFolder("Assets", "ConvexColliders");
        string parentFolder = EnsureFolder(rootFolder, groupName);
        string assetPath = $"{parentFolder}/{baseGameObject.name}.asset";

        ConvexColliderData colliderData = AssetDatabase.LoadAssetAtPath<ConvexColliderData>(assetPath);

        if (colliderData == null)
        {
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            colliderData = ConvexColliderData.CreateAsset(assetPath, meshes.ToArray());
        }
        else
        {
            colliderData.UpdateAsset(meshes.ToArray());
        }

        return colliderData;
    }

    /// <summary>
    /// Ensures that a folder exists in the Unity project (Assets/).
    /// If it doesn’t exist, it will be created.
    /// </summary>
    public static string EnsureFolder(string parent, string folderName)
    {
        string path = $"{parent}/{folderName}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, folderName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        return path;
    }
}