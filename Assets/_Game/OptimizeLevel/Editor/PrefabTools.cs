using UnityEngine;
using UnityEditor;

public class PrefabTools
{
    [MenuItem("Tools/Convert To Original Prefab")]
    public static void ConvertSelectedPrefab()
    {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null) continue;

            // Tạo bản sao prefab gốc
            string newPath = path.Replace(".prefab", "_Original.prefab");
            var newPrefab = PrefabUtility.SaveAsPrefabAsset(go, newPath);
            Debug.Log($"Converted {obj.name} -> {newPrefab.name} (Original Prefab)");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
