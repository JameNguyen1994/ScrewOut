using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

[CustomEditor(typeof(LevelMap)), CanEditMultipleObjects]
public class LevelMapEditor : Editor
{
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector(); // Vẽ Inspector mặc định
        return;
        LevelMap levelMap = (LevelMap)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Custom Actions", EditorStyles.boldLabel);

        //if (GUILayout.Button("Get All Screw On Map"))
        //{
        //    levelMap.GetAllScrewOnMap();
        //}
        if (GUILayout.Button("Set Up Link Obstacle"))
        {
            levelMap.GetScrewOnLinkObstacle();
        }

        // if (GUILayout.Button("Set Up Shape"))
        // {
        //     levelMap.GetAllScrewOnCustomsMap();
        // }
        // if (GUILayout.Button("Set Up Data Color"))
        // {
        //     levelMap.SetUpDataColorAsync();
        // }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Set Up Level"))
        {
            levelMap.GetAllScrewOnCustomsMap();
            levelMap.SetUpDataColorAsync();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Test Level"))
        {
            levelMap.TestLevel();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save as Prefab"))
        {
            SaveAsPrefab(levelMap.gameObject, levelMap.LevelId);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Update Shape"))
        {
            levelMap.UpdateShape();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();        
        if (GUILayout.Button("Update Link Obstacle"))
        {
            levelMap.UpdateLinkObstacle();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate All Id"))
        {
            levelMap.GenerateAllId();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void SaveAsPrefab(GameObject levelMap, int levelId)
    {
        string folderPath = "Assets/_Game/MCPE/SofPrefabs/"; // Assets/_Game/MCPE/LevelPrefab/Level_119.prefab
        string prefabName = $"Level_{levelId}.prefab";
        string fullPath = Path.Combine(folderPath, prefabName);

        // Ensure folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        // Confirm overwrite if prefab exists
        if (File.Exists(fullPath))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "Prefab Exists",
                $"A prefab named '{prefabName}' already exists. Overwrite it?",
                "Yes", "Cancel");

            if (!overwrite)
                return;
        }

        // Save prefab (create or overwrite)
/*#if UNITY_2023_1_OR_NEWER
        PrefabUtility.SaveAsPrefabAssetAndConnect(levelMap, fullPath, InteractionMode.UserAction);
#else
        PrefabUtility.SaveAsPrefabAsset(levelMap, fullPath);
#endif*/
        PrefabUtility.SaveAsPrefabAsset(levelMap, fullPath);

        Debug.Log($"Saved prefab: {fullPath}");

        // Ensure it's marked as Addressable
        EnsureAddressable(fullPath, fullPath);
    }

    private void EnsureAddressable(string assetPath, string address)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.FindAssetEntry(guid);

        if (entry == null)
        {
            entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
            entry.address = address;
            Debug.Log($"Addressable assigned: {address}");
        }
    }
}

