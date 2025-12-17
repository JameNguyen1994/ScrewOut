using UnityEngine;
using VHACD.Unity;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine.Rendering;
using System;

[CustomEditor(typeof(ConvexCollider))]
public class ConvexColliderEditor : Editor
{
    private ConvexCollider _base;

    private SerializedProperty _script;
    private SerializedProperty _quality;

    private SerializedProperty _parameters;
    private SerializedProperty _paramResolution;
    private SerializedProperty _paramConcavity;
    private SerializedProperty _paramPlaneDownsample;
    private SerializedProperty _paramConvexHullDownsample;
    private SerializedProperty _paramAlpha;
    private SerializedProperty _paramBeta;
    private SerializedProperty _paramPCA;
    private SerializedProperty _paramMode;
    private SerializedProperty _paramMaxVertices;
    private SerializedProperty _paramMinVolume;
    private SerializedProperty _paramConvexHullApprox;
    private SerializedProperty _paramOCLAccel;
    private SerializedProperty _paramMaxConvexHull;
    private SerializedProperty _paramProjectHullVertices;

    private string[] _qualityNames = { "Low", "Medium", "High", "Insane", "Custom" };

    public override void OnInspectorGUI()
    {
        Properties();

        ScriptField();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        QualityButtons();

        CalculateColliderButtons();
        EditorGUILayout.EndVertical();

        if (_base != null && serializedObject != null)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void Properties()
    {
        _base = (ConvexCollider)target;

        _script = serializedObject.FindProperty("m_Script");

        _quality = serializedObject.FindProperty("_quality");

        _parameters = serializedObject.FindProperty("_parameters");
        _paramResolution = _parameters.FindPropertyRelative("m_resolution");
        _paramConcavity = _parameters.FindPropertyRelative("m_concavity");
        _paramPlaneDownsample = _parameters.FindPropertyRelative("m_planeDownsampling");
        _paramConvexHullDownsample = _parameters.FindPropertyRelative("m_convexhullDownsampling");
        _paramAlpha = _parameters.FindPropertyRelative("m_alpha");
        _paramBeta = _parameters.FindPropertyRelative("m_beta");
        _paramPCA = _parameters.FindPropertyRelative("m_pca");
        _paramMode = _parameters.FindPropertyRelative("m_mode");
        _paramMaxVertices = _parameters.FindPropertyRelative("m_maxNumVerticesPerCH");
        _paramMinVolume = _parameters.FindPropertyRelative("m_minVolumePerCH");
        _paramConvexHullApprox = _parameters.FindPropertyRelative("m_convexhullApproximation");
        _paramOCLAccel = _parameters.FindPropertyRelative("m_oclAcceleration");
        _paramMaxConvexHull = _parameters.FindPropertyRelative("m_maxConvexHulls");
        _paramProjectHullVertices = _parameters.FindPropertyRelative("m_projectHullVertices");
    }

    private void ScriptField()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(_script);
        EditorGUI.EndDisabledGroup();
    }

    #region Quality

    private void QualityButtons()
    {
        EditorGUILayout.LabelField("Collider Quality", EditorStyles.boldLabel);

        if (_quality.intValue == -1)
        {
            _quality.intValue = 2;
            ApplyDefaultParams(2);
        }

        if (!IsQualityEqual() || !IsConvexEqual())
        {
            _quality.intValue = 4;
        }

        int val = GUILayout.Toolbar(_quality.intValue, _qualityNames);
        if (val != _quality.intValue)
        {
            _quality.intValue = val;
            if (val < 4)
            {
                ApplyDefaultParams(val);
            }
        }

        EditorGUILayout.PropertyField(_paramResolution);
        EditorGUILayout.PropertyField(_paramMaxConvexHull);
        if (val == 4)
        {
            EditorGUILayout.PropertyField(_paramConcavity);
            EditorGUILayout.PropertyField(_paramPlaneDownsample);
            EditorGUILayout.PropertyField(_paramConvexHullDownsample);
            EditorGUILayout.PropertyField(_paramAlpha);
            EditorGUILayout.PropertyField(_paramBeta);
            EditorGUILayout.PropertyField(_paramPCA);
            EditorGUILayout.PropertyField(_paramMode);
            EditorGUILayout.PropertyField(_paramMaxVertices);
            EditorGUILayout.PropertyField(_paramMinVolume);
            EditorGUILayout.PropertyField(_paramConvexHullApprox);
            EditorGUILayout.PropertyField(_paramOCLAccel);
            EditorGUILayout.PropertyField(_paramProjectHullVertices);
        }
    }

    private bool IsQualityEqual()
    {
        if (_quality.intValue >= 4)
        {
            return false;
        }
        else
        {
            return _paramResolution.intValue == Resolution(_quality.intValue);
        }
    }

    private bool IsConvexEqual()
    {
        if (_quality.intValue >= 4)
        {
            return false;
        }
        else
        {
            return _paramMaxConvexHull.intValue == ConvexHulls(_quality.intValue);
        }
    }

    private int Resolution(int quality)
    {
        switch (quality)
        {
            case 0:
                return 10000;
            case 1:
                return 200000;
            case 2:
                return 1000000;
            case 3:
                return 4000000;
        }
        return -1;
    }

    private int ConvexHulls(int quality)
    {
        switch (quality)
        {
            case 0:
                return 32;
            case 1:
                return 64;
            case 2:
                return 128;
            case 3:
                return 256;
        }
        return -1;
    }

    private void ApplyDefaultParams(int quality)
    {
        _paramResolution.intValue = Resolution(quality);
        _paramMaxConvexHull.intValue = ConvexHulls(quality);
        _paramConcavity.doubleValue = 0.001;
        _paramPlaneDownsample.intValue = 4;
        _paramConvexHullDownsample.intValue = 4;
        _paramAlpha.doubleValue = 0.05;
        _paramBeta.doubleValue = 0.05;
        _paramPCA.intValue = 0;
        _paramMode.intValue = 0;
        _paramMaxVertices.intValue = 64;
        _paramMinVolume.doubleValue = 0.0001;
        _paramConvexHullApprox.intValue = 1;
        _paramOCLAccel.intValue = 0;
        _paramProjectHullVertices.boolValue = true;
    }

    #endregion

    #region Collider Logic

    private void CalculateColliderButtons()
    {
        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        EditorGUI.BeginDisabledGroup(!_base.TryGetComponent<MeshFilter>(out var filter));

        if (GUILayout.Button("Calculate Colliders From Current Mesh Filter"))
        {
            CalculateColliders(_base.Parameters, false);
        }

        if (GUILayout.Button("Done - Remove"))
        {
            DestroyImmediate(_base);
        }
    }

    private void CalculateColliders(Parameters parameters, bool combine)
    {
        Mesh mesh = null;
        string path = "";
        EditorUtility.DisplayProgressBar("Calculating Colliders", "Discovering meshes...", 0.1f);
        List<Mesh> originalMeshes = new List<Mesh>();

        List<Mesh> meshesToCalc = new List<Mesh>();
        List<Matrix4x4> transformsToCalc = new List<Matrix4x4>();

        if (combine)
        {
            MeshFilter[] filters = _base.GetComponentsInChildren<MeshFilter>(true);
            List<MeshFilter> foundMeshes = new List<MeshFilter>();
            foreach (var item in filters)
            {
                if (item.sharedMesh != null)
                {
                    foundMeshes.Add(item);
                }
            }
            if (foundMeshes.Count == 0)
            {
                EditorUtility.ClearProgressBar();
                ShowMissingMeshes();
                return;
            }
            else
            {
                for (int i = 0; i < foundMeshes.Count; i++)
                {
                    if (path == "")
                    {
                        path = AssetDatabase.GetAssetPath(foundMeshes[i].sharedMesh);
                    }
                    for (int j = 0; j < foundMeshes[i].sharedMesh.subMeshCount; j++)
                    {
                        meshesToCalc.Add(ExtractSubmesh(foundMeshes[i].sharedMesh, j));
                        transformsToCalc.Add(_base.transform.worldToLocalMatrix * foundMeshes[i].transform.localToWorldMatrix);
                    }
                    originalMeshes.Add(foundMeshes[i].sharedMesh);
                }
            }
        }
        else
        {
            if (_base.TryGetComponent<MeshFilter>(out var filter))
            {
                if (filter.sharedMesh == null)
                {
                    EditorUtility.ClearProgressBar();
                    ShowMissingMeshes();
                    return;
                }
                else
                {
                    for (int j = 0; j < filter.sharedMesh.subMeshCount; j++)
                    {
                        meshesToCalc.Add(ExtractSubmesh(filter.sharedMesh, j));
                        transformsToCalc.Add(_base.transform.worldToLocalMatrix * filter.transform.localToWorldMatrix);
                    }
                    originalMeshes.Add(filter.sharedMesh);
                    path = AssetDatabase.GetAssetPath(mesh);
                }
            }
            else
            {
                EditorUtility.ClearProgressBar();
                ShowMissingMeshFilter();
                return;
            }
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayProgressBar("Calculating Colliders", "Combining meshes...", 0.3f);

        List<Mesh> meshes = new List<Mesh>();

        mesh = CombineMeshes(meshesToCalc, transformsToCalc);
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayProgressBar("Calculating Colliders", $"Processing mesh... (this can take a while)", 0.5f);
        VHACDProcessor.GenerateConvexMeshes(mesh, parameters, out meshes);

        EditorUtility.ClearProgressBar();

        if (meshes.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", $"The object you are trying to calculate colliders for did not compute any submeshes.\nTry modifying your quality parameters and try again", "Ok");
            return;
        }

        EditorUtility.DisplayProgressBar("Calculating Colliders", "Storing submeshes...", 0.8f);

        // Save results into a ConvexColliderData asset
        var colliderData = ConvexColliderGenerator.SaveConvexColliderData(_base.gameObject, meshes, _base.transform.parent.name);

        // Attach MeshColliders to the GameObject
        ConvexColliderService.GenOrClearColliders(colliderData, _base.gameObject);

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
        EditorGUIUtility.PingObject(colliderData);
    }

    private Mesh CombineMeshes(List<Mesh> meshes, List<Matrix4x4> transform)
    {
        if (meshes.Count == 1)
        {
            return meshes[0];
        }
        CombineInstance[] combineMesh = new CombineInstance[meshes.Count];
        for (int i = 0; i < meshes.Count; i++)
        {
            combineMesh[i].mesh = meshes[i];
            combineMesh[i].transform = transform[i];
        }
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combineMesh);
        return mesh;
    }

    private Mesh ExtractSubmesh(Mesh mesh, int submesh)
    {
        Mesh newMesh = new Mesh();
        SubMeshDescriptor descriptor = mesh.GetSubMesh(submesh);
        newMesh.vertices = RangeSubset(mesh.vertices, descriptor.firstVertex, descriptor.vertexCount);

        var triangles = RangeSubset(mesh.triangles, descriptor.indexStart, descriptor.indexCount);
        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] -= descriptor.firstVertex;
        }

        newMesh.triangles = triangles;

        if (mesh.normals != null && mesh.normals.Length == mesh.vertices.Length)
        {
            newMesh.normals = RangeSubset(mesh.normals, descriptor.firstVertex, descriptor.vertexCount);
        }
        else
        {
            newMesh.RecalculateNormals();
        }

        newMesh.Optimize();
        newMesh.OptimizeIndexBuffers();
        newMesh.RecalculateBounds();
        newMesh.name = mesh.name + $" Submesh {submesh}";
        return newMesh;
    }

    private T[] RangeSubset<T>(T[] array, int startIndex, int length)
    {
        T[] subset = new T[length];
        Array.Copy(array, startIndex, subset, 0, length);
        return subset;
    }

    #endregion

    private void ShowMissingMeshFilter()
    {
        EditorUtility.DisplayDialog("Error", $"The object you are trying to calculate colliders for has no mesh filter. Calculation has been aborted.", "Continue");
    }

    private void ShowMissingMeshes()
    {
        EditorUtility.DisplayDialog("Error", $"The object you are trying to calculate colliders for has mesh filters but no associated meshes. Calculation has been aborted.", "Continue");
    }

}
