using UnityEditor;
using UnityEngine;
using System.IO;

public class RenameLevels
{
    [MenuItem("Tools/Rename Level Files")]
    public static void Rename()
    {
        string folderPath = "Assets/_Game/MCPE/LevelPrefab"; // 👉 đổi thành folder chứa level của bạn

        string[] guids = AssetDatabase.FindAssets("Level_", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            // Kiểm tra format Level_X_Y
            string[] parts = fileName.Split('_');
            if (parts.Length == 3)
            {
                string newName = $"Level_{parts[2]}";
                string newPath = assetPath.Replace(fileName, newName);

                if (fileName != newName)
                {
                    AssetDatabase.RenameAsset(assetPath, newName);
                    Debug.Log($"Renamed: {fileName} ➜ {newName}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
