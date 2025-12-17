using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class SpriteOptimizer : EditorWindow
{

    private string folderPath = "Assets";

    [MenuItem("CustomTools/Optimize Sprites")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SpriteOptimizer));
    }

    private void OnGUI()
    {
        GUILayout.Label("Folder Path:");
        EditorGUILayout.BeginHorizontal();
        folderPath = EditorGUILayout.TextField(folderPath);

        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", folderPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                int assetsGameIndex = selectedPath.IndexOf("Assets");

                if (assetsGameIndex != -1)
                {
                    folderPath = selectedPath.Substring(assetsGameIndex);
                }
                else
                {
                    folderPath = selectedPath;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Optimize Sprites"))
        {
            OptimizeSprites();
        }
    }

    [System.Obsolete]
    private void OptimizeSprites()
    {
        string[] spritePaths = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });

        foreach (string path in spritePaths)
        {
            string spritePath = AssetDatabase.GUIDToAssetPath(path);
            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(spritePath);
            if (textureImporter != null)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.textureCompression = TextureImporterCompression.Compressed;
                textureImporter.mipmapEnabled = false;
                textureImporter.alphaIsTransparency = true;

                textureImporter.SetPlatformTextureSettings("Android", MaxSizeTexture(textureImporter, spritePath), TextureImporterFormat.ASTC_4x4);
                textureImporter.SetPlatformTextureSettings("iOS", MaxSizeTexture(textureImporter, spritePath), TextureImporterFormat.ASTC_4x4);

                textureImporter.SaveAndReimport();
            }
        }
    }

    private int MaxSizeTexture(TextureImporter textureImporter, string spritePath)
    {
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePath);
        int width = texture.width;
        int height = texture.height;

        int maxSize = Mathf.Max(width, height);
        Debug.Log(maxSize);

        if (maxSize <= 64)
        {
            return 64;
        }
        else if (maxSize <= 128)
        {
            return 128;
        }
        else if (maxSize <= 256)
        {
            return 256;
        }
        else if (maxSize <= 512)
        {
            return 512;
        }
        else if (maxSize <= 1024)
        {
            return 1024;
        }
        else if (maxSize <= 2048)
        {
            return 2048;
        }
        else if (maxSize <= 4096)
        {
            return 4096;
        }
        else if (maxSize <= 8192)
        {
            return 8192;
        }
        else
        {
            return 16384;
        }
    }
}
#endif