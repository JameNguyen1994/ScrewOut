using UnityEditor;
using UnityEngine;

public class FixMissingMeshColliders : MonoBehaviour
{
#if (UNITY_EDITOR)
    [MenuItem("Tools/Fix Missing MeshColliders")]
    public static void FixMissingMeshCollidersInSelection()
    {
        // Get the currently selected GameObjects in the editor
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No GameObjects selected. Please select at least one GameObject.");
            return;
        }

        int fixedCount = 0;

        foreach (GameObject rootObject in selectedObjects)
        {
            // Get all child objects, including the root object
            MeshCollider[] meshColliders = rootObject.GetComponentsInChildren<MeshCollider>(true);

            foreach (MeshCollider meshCollider in meshColliders)
            {
                if (meshCollider.sharedMesh == null)
                {
                    MeshFilter meshFilter = meshCollider.GetComponent<MeshFilter>();

                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        Undo.RecordObject(meshCollider, "Fix Missing MeshCollider Mesh");
                        meshCollider.sharedMesh = meshFilter.sharedMesh;
                        EditorUtility.SetDirty(meshCollider);
                        fixedCount++;
                        Debug.Log($"Fixed missing mesh on: {meshCollider.gameObject.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"Missing MeshFilter or Mesh on: {meshCollider.gameObject.name}");
                    }
                }
            }
        }

        Debug.Log($"Fixing complete. {fixedCount} MeshColliders updated.");
    }

    [ContextMenu("Fix Missing MeshColliders")]
    private void FixMissingMeshCollidersInContextMenu()
    {
        GameObject rootObject = gameObject;

        if (rootObject == null)
        {
            Debug.LogWarning("No GameObject found. Please attach this script to a GameObject.");
            return;
        }

        int fixedCount = 0;

        // Get all child objects, including the root object
        MeshCollider[] meshColliders = rootObject.GetComponentsInChildren<MeshCollider>(true);

        foreach (MeshCollider meshCollider in meshColliders)
        {
            if (meshCollider.sharedMesh == null)
            {
                MeshFilter meshFilter = meshCollider.GetComponent<MeshFilter>();

                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    Undo.RecordObject(meshCollider, "Fix Missing MeshCollider Mesh");
                    meshCollider.sharedMesh = meshFilter.sharedMesh;
                    EditorUtility.SetDirty(meshCollider);
                    fixedCount++;
                    Debug.Log($"Fixed missing mesh on: {meshCollider.gameObject.name}");
                }
                else
                {
                    Debug.LogWarning($"Missing MeshFilter or Mesh on: {meshCollider.gameObject.name}");
                }
            }
        }

        Debug.Log($"Fixing complete. {fixedCount} MeshColliders updated.");
    }
#endif

}
