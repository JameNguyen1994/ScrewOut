using UnityEngine;
using System.Collections.Generic;

public static class ConvexDecomposerService
{
    // =================== Reset Colliders ===================
    public static void ResetColliders(GameObject go)
    {
        foreach (var col in go.GetComponents<MeshCollider>())
            Object.DestroyImmediate(col);
    }

    // =================== Deconvex Colliders ===================
    public static void DeconvexColliders(GameObject go)
    {
        foreach (var col in go.GetComponents<MeshCollider>())
        {
            col.convex = false;
        }
    }

    // =================== Generate Preview Bounds ===================
    public static List<Bounds> GeneratePreviewBounds(GameObject go, bool combineMeshes = true, int maxTris = 255)
    {
        List<Bounds> previewBounds = new List<Bounds>();
        List<Mesh> meshes = PrepareMeshes(go, out List<Matrix4x4> transforms);

        if (combineMeshes)
        {
            Mesh combined = CombineMeshes(meshes, transforms);
            List<List<int>> groups = ClusterTrianglesByVolume(combined, maxTris);
            AddBoundsForGroups(combined, groups, go, previewBounds);
        }
        else
        {
            foreach (var mesh in meshes)
            {
                List<List<int>> groups = ClusterTrianglesByVolume(mesh, maxTris);
                AddBoundsForGroups(mesh, groups, go, previewBounds);
            }
        }

        return previewBounds;
    }

    // =================== Generate Convex Colliders ===================
    public static void GenerateConvexColliders(GameObject go, bool combineMeshes = true, int maxTris = 255)
    {
        ResetColliders(go);

        List<Mesh> meshes = PrepareMeshes(go, out List<Matrix4x4> transforms);
        List<Mesh> convexMeshes = new List<Mesh>();

        if (combineMeshes)
        {
            Mesh combined = CombineMeshes(meshes, transforms);
            List<List<int>> groups = ClusterTrianglesByVolume(combined, maxTris);
            foreach (var g in groups)
            {
                Mesh subMesh = CreateSubMesh(combined, g);
                convexMeshes.Add(subMesh);
            }
        }
        else
        {
            foreach (var mesh in meshes)
            {
                List<List<int>> groups = ClusterTrianglesByVolume(mesh, maxTris);
                foreach (var g in groups)
                {
                    Mesh subMesh = CreateSubMesh(mesh, g);
                    convexMeshes.Add(subMesh);
                }
            }
        }

        foreach (var convex in convexMeshes)
        {
            MeshCollider mc = go.AddComponent<MeshCollider>();
            mc.sharedMesh = convex;
            mc.convex = true;
        }
    }

    // =================== Mesh Utilities ===================
    private static List<Mesh> PrepareMeshes(GameObject go, out List<Matrix4x4> transforms)
    {
        List<Mesh> meshesToCalc = new List<Mesh>();
        transforms = new List<Matrix4x4>();
        MeshFilter[] filters = go.GetComponentsInChildren<MeshFilter>(true);

        foreach (var mf in filters)
        {
            if (mf.sharedMesh == null) continue;
            for (int j = 0; j < mf.sharedMesh.subMeshCount; j++)
            {
                meshesToCalc.Add(ExtractSubmesh(mf.sharedMesh, j));
                transforms.Add(go.transform.worldToLocalMatrix * mf.transform.localToWorldMatrix);
            }
        }
        return meshesToCalc;
    }

    private static Mesh CombineMeshes(List<Mesh> meshes, List<Matrix4x4> transforms)
    {
        List<CombineInstance> combines = new List<CombineInstance>();
        for (int i = 0; i < meshes.Count; i++)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = meshes[i];
            ci.transform = transforms[i];
            combines.Add(ci);
        }
        Mesh combined = new Mesh();
        combined.CombineMeshes(combines.ToArray(), true, true);
        return combined;
    }

    private static Mesh ExtractSubmesh(Mesh mesh, int subIndex)
    {
        int[] tris = mesh.GetTriangles(subIndex);
        Vector3[] verts = mesh.vertices;

        Dictionary<int, int> vertMap = new Dictionary<int, int>();
        List<Vector3> newVerts = new List<Vector3>();
        List<int> newTris = new List<int>();

        for (int i = 0; i < tris.Length; i++)
        {
            int oldIndex = tris[i];
            if (!vertMap.ContainsKey(oldIndex))
            {
                vertMap[oldIndex] = newVerts.Count;
                newVerts.Add(verts[oldIndex]);
            }
            newTris.Add(vertMap[oldIndex]);
        }

        Mesh subMesh = new Mesh();
        subMesh.vertices = newVerts.ToArray();
        subMesh.triangles = newTris.ToArray();
        subMesh.RecalculateNormals();
        subMesh.RecalculateBounds();
        return subMesh;
    }

    private static void AddBoundsForGroups(Mesh mesh, List<List<int>> groups, GameObject go, List<Bounds> previewBounds)
    {
        Vector3[] vertices = mesh.vertices;
        foreach (var group in groups)
        {
            Bounds b = new Bounds(vertices[group[0]], Vector3.zero);
            foreach (var idx in group)
                b.Encapsulate(vertices[idx]);
            b.center = go.transform.TransformPoint(b.center);
            b.size = Vector3.Scale(b.size, go.transform.lossyScale);
            previewBounds.Add(b);
        }
    }

    private static Mesh CreateSubMesh(Mesh mesh, List<int> group)
    {
        Vector3[] verts = mesh.vertices;
        List<Vector3> newVerts = new List<Vector3>();
        Dictionary<int, int> vertMap = new Dictionary<int, int>();
        List<int> newTris = new List<int>();

        foreach (int idx in group)
        {
            if (!vertMap.ContainsKey(idx))
            {
                vertMap[idx] = newVerts.Count;
                newVerts.Add(verts[idx]);
            }
            newTris.Add(vertMap[idx]);
        }

        Mesh subMesh = new Mesh();
        subMesh.vertices = newVerts.ToArray();
        subMesh.triangles = newTris.ToArray();
        subMesh.RecalculateNormals();
        subMesh.RecalculateBounds();
        return subMesh;
    }

    // =================== Volume-based clustering ===================
    private static List<List<int>> ClusterTrianglesByVolume(Mesh mesh, int maxTris)
    {
        List<int> allTris = new List<int>(mesh.triangles);
        return SplitTrianglesRecursive(mesh, allTris, maxTris);
    }

    private static List<List<int>> SplitTrianglesRecursive(Mesh mesh, List<int> tris, int maxTris)
    {
        List<List<int>> result = new List<List<int>>();
        if (tris.Count <= maxTris * 3)
        {
            result.Add(tris);
            return result;
        }

        Vector3[] verts = mesh.vertices;
        Vector3 min = verts[tris[0]];
        Vector3 max = verts[tris[0]];
        for (int i = 0; i < tris.Count; i++)
        {
            Vector3 v = verts[tris[i]];
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        Vector3 size = max - min;
        int axis = 0;
        if (size.y > size.x && size.y > size.z) axis = 1;
        else if (size.z > size.x && size.z > size.y) axis = 2;

        float mid = (min[axis] + max[axis]) * 0.5f;
        List<int> left = new List<int>();
        List<int> right = new List<int>();

        for (int i = 0; i < tris.Count; i += 3)
        {
            Vector3 center = (verts[tris[i]] + verts[tris[i + 1]] + verts[tris[i + 2]]) / 3f;
            if (center[axis] <= mid)
            {
                left.Add(tris[i]); left.Add(tris[i + 1]); left.Add(tris[i + 2]);
            }
            else
            {
                right.Add(tris[i]); right.Add(tris[i + 1]); right.Add(tris[i + 2]);
            }
        }

        result.AddRange(SplitTrianglesRecursive(mesh, left, maxTris));
        result.AddRange(SplitTrianglesRecursive(mesh, right, maxTris));
        return result;
    }
}