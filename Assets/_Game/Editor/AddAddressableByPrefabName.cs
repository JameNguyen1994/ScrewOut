#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public static class AddLevelPrefabsToAddressables
{
    [MenuItem("Assets/Add to Addressables", true)]
    private static bool ValidateLevelPrefabs()
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (path.EndsWith(".prefab") && Regex.IsMatch(Path.GetFileNameWithoutExtension(path), @"^Level_\d+$"))
                return true;
        }
        return false;
    }

    private const string LevelPrefabFolder = "Assets/_Game/MCPE/SofPrefabs/";

    [MenuItem("Assets/Add to Addressables")]
    private static void MarkSelectedLevelPrefabs()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found.");
            return;
        }

        var validPrefabs = new List<(string path, string filename, int levelID, string address, string label)>();

        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (!path.EndsWith(".prefab")) continue;
            if (!path.StartsWith(LevelPrefabFolder)) continue; // Only allow prefabs in the target folder

            string filename = Path.GetFileNameWithoutExtension(path);
            var match = Regex.Match(filename, @"^Level_(\d+)$");
            if (!match.Success) continue;

            int levelID = int.Parse(match.Groups[1].Value);
            string address = $"Level_{levelID}";
            string label = $"L{Mathf.CeilToInt(levelID / 5f)}";

            validPrefabs.Add((path, filename, levelID, address, label));
        }

        if (validPrefabs.Count == 0)
        {
            EditorUtility.DisplayDialog("No Valid Prefabs", $"No prefabs matched the 'Level_{{ID}}' pattern in:\n{LevelPrefabFolder}", "OK");
            return;
        }

        bool batchConfirm = true;
        if (validPrefabs.Count > 1)
        {
            string summary = string.Join(", ",
                validPrefabs.ConvertAll(p => $"[{p.filename} ({p.address}) → {p.label}]"));

            batchConfirm = EditorUtility.DisplayDialog(
                "Batch Add to Addressables?",
                $"Detected {validPrefabs.Count} valid prefabs.\n\n{summary}\n\nApply all without confirmation?",
                "Yes", "No");
        }

        foreach (var (path, filename, levelID, address, label) in validPrefabs)
        {
            if (!batchConfirm)
            {
                bool confirm = EditorUtility.DisplayDialog(
                    "Confirm Addressable",
                    $"Prefab: {filename}\nPath: {path}\nAddress: {address}\nLabel: {label}\n\nAdd to Addressables?",
                    "Yes", "No");

                if (!confirm) continue;
            }

            string guid = AssetDatabase.AssetPathToGUID(path);
            var entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                Debug.Log($"Created addressable entry for {filename} ({path})");
            }

            // Clear all old labels
            foreach (string existingLabel in new List<string>(entry.labels))
            {
                entry.SetLabel(existingLabel, false);
            }

            // entry.address = address;
            entry.SetLabel(label, true);

            Debug.Log($"✅ {filename} ({path}) → Address: {address}, Label: {label}");
        }

        AssetDatabase.SaveAssets();

        if (batchConfirm)
        {
            string batchLog = string.Join("\n", validPrefabs.ConvertAll(p => $"{p.filename} ({p.path}) → {p.label}"));
            EditorUtility.DisplayDialog("Batch Add Complete", $"Addressables added:\n\n{batchLog}", "OK");
        }
    }
}
#endif
