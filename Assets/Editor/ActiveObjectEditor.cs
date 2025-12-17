using UnityEngine;
using UnityEditor;

public static class ToggleObjectActiveEditor
{
    [MenuItem("GameObject/Toggle Active _b")] // Hoặc %h cho Ctrl+H
    private static void ToggleSelectedObjectActive()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject selectedObject = Selection.activeGameObject;
            selectedObject.SetActive(!selectedObject.activeSelf);
            EditorUtility.SetDirty(selectedObject);
            Debug.Log($"[Editor] Object '{selectedObject.name}' active state toggled to: {selectedObject.activeSelf}");
        }
        else
        {
            Debug.Log("[Editor] No object selected to toggle active state.");
        }
    }
}