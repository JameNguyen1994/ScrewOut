using UnityEditor;
using UnityEngine;

public static class ScrewSelectionHelper
{
    public static Screw GetSelectedScrew()
    {
        var go = Selection.activeGameObject;
        if (go == null) return null;

        // Kiểm tra có component Screw không
        var screw = go.GetComponent<Screw>();
        return screw;
    }
}
