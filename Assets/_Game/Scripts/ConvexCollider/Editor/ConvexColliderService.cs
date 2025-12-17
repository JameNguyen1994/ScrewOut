using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Utility methods for convex collider generation and management.
/// </summary>
public static class ConvexColliderService
{
    /// <summary>
    /// Extracts a single submesh from a mesh and creates a new Mesh object.
    /// </summary>
    public static Mesh ExtractSubmesh(Mesh mesh, int submeshIndex)
    {
        if (mesh == null) throw new ArgumentNullException(nameof(mesh));

        var descriptor = mesh.GetSubMesh(submeshIndex);
        var newMesh = new Mesh
        {
            vertices = RangeSubset(mesh.vertices, descriptor.firstVertex, descriptor.vertexCount)
        };

        // Copy triangles with correct indices
        var triangles = RangeSubset(mesh.triangles, descriptor.indexStart, descriptor.indexCount);
        for (int i = 0; i < triangles.Length; i++)
            triangles[i] -= descriptor.firstVertex;

        newMesh.triangles = triangles;

        // Copy normals or recalculate if missing
        if (mesh.normals != null && mesh.normals.Length == mesh.vertexCount)
            newMesh.normals = RangeSubset(mesh.normals, descriptor.firstVertex, descriptor.vertexCount);
        else
            newMesh.RecalculateNormals();

        newMesh.OptimizeIndexBuffers();
        newMesh.OptimizeReorderVertexBuffer();
        newMesh.RecalculateBounds();

        newMesh.name = $"{mesh.name} Submesh {submeshIndex}";
        return newMesh;
    }

    /// <summary>
    /// Helper method to copy a subarray from an array.
    /// </summary>
    private static T[] RangeSubset<T>(T[] array, int startIndex, int length)
    {
        var subset = new T[length];
        Array.Copy(array, startIndex, subset, 0, length);
        return subset;
    }

    /// <summary>
    /// Shows an error dialog when mesh filters exist but no meshes are assigned.
    /// </summary>
    public static void ShowMissingMeshes()
    {
        EditorUtility.DisplayDialog(
            "Error",
            "The object you are trying to calculate colliders for has mesh filters but no associated meshes. Calculation aborted.",
            "Continue");
    }

    /// <summary>
    /// Shows an error dialog when no MeshFilter is attached to the GameObject.
    /// </summary>
    public static void ShowMissingMeshFilter()
    {
        EditorUtility.DisplayDialog(
            "Error",
            "The object you are trying to calculate colliders for has no MeshFilter. Calculation aborted.",
            "Continue");
    }

    /// <summary>
    /// Returns the voxel resolution for VHACD based on quality preset.
    /// </summary>
    public static uint Resolution(int quality) => quality switch
    {
        0 => 10_000,
        1 => 200_000,
        2 => 1_000_000,
        3 => 4_000_000,
        _ => 4_000_000
    };

    /// <summary>
    /// Returns the max number of convex hulls for VHACD based on quality preset.
    /// </summary>
    public static uint ConvexHulls(int quality) => quality switch
    {
        0 => 32,
        1 => 64,
        2 => 128,
        3 => 256,
        _ => 256
    };

    /// <summary>
    /// Generates or clears colliders on a GameObject depending on whether data is provided.
    /// </summary>
    public static void GenOrClearColliders(ConvexColliderData data, GameObject baseGameObject)
    {
        if (baseGameObject == null) return;

        ClearColliders(baseGameObject);

        if (data != null)
            GenerateColliders(data, baseGameObject);
    }

    /// <summary>
    /// Instantiates MeshColliders on a GameObject from a ConvexColliderData asset.
    /// </summary>
    private static void GenerateColliders(ConvexColliderData data, GameObject baseGameObject)
    {
        if (data.ComputedMeshes == null || data.ComputedMeshes.Length == 0) return;

        foreach (var mesh in data.ComputedMeshes)
        {
            var collider = baseGameObject.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.isTrigger = false;
            collider.sharedMesh = mesh;
        }
    }

    /// <summary>
    /// Removes all MeshColliders from a GameObject.
    /// </summary>
    private static void ClearColliders(GameObject baseGameObject)
    {
        var colliders = baseGameObject.GetComponents<MeshCollider>();
        if (colliders == null || colliders.Length == 0) return;

        foreach (var collider in colliders)
            UnityEngine.Object.DestroyImmediate(collider);
    }
}