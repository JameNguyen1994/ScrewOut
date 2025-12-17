using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AssetBundleExporter
{
    private const string ASSET_BUNDLE_TEMP_DIR = "AssetBundlesTemp";
    private const string AB_EXT = ".ab";

    public static string[] ignoreEndPatterns = new string[] { "_SkeletonData" };

    public static string IgnoreEndPatterns(string text)
    {
        foreach (string pattern in ignoreEndPatterns)
        {
            if (text.EndsWith(pattern))
            {
                return text.Remove(text.Length - pattern.Length);
            }
        }
        return text;
    }

    [MenuItem("AssetBundle/Build AssetBundles - iOS")]
    private static void ExportAssetBundleIOSMenu()
    {
        ExportAssetBundle(Selection.objects, BuildTarget.iOS, true);
    }

    [MenuItem("AssetBundle/Build AssetBundles - Android")]
    private static void ExportAssetBundleAdrMenu()
    {
        ExportAssetBundle(Selection.objects, BuildTarget.Android, true);
    }

    [MenuItem("Assets/Build AssetBundles - iOS")]
    private static void ExportAssetBundleIOS()
    {
        ExportAssetBundle(Selection.objects, BuildTarget.iOS, true);
    }

    [MenuItem("Assets/Build AssetBundles - Android")]
    private static void ExportAssetBundleAdr()
    {
        ExportAssetBundle(Selection.objects, BuildTarget.Android, true);
    }

    private static void ExportAssetBundle(Object[] objects, BuildTarget buildTarget, bool openOutputFolder = false)
    {
        // Determine platform folder
        string platformName = buildTarget.ToString();
        string folder = Path.Combine(ASSET_BUNDLE_TEMP_DIR, platformName);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        foreach (var obj in objects)
        {
            string fileName = IgnoreEndPatterns(obj.name);
            string targetPath = Path.Combine(folder, $"{fileName}_{platformName}{AB_EXT}");

            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning($"⚠️ Cannot find path for {obj.name}");
                continue;
            }

            // ✅ Lấy toàn bộ dependencies (tránh lỗi khi load)
            string[] dependencies = AssetDatabase.GetDependencies(assetPath, true).Where(p => !p.EndsWith(".cs")).ToArray();

            // ✅ Tạo cấu trúc build
            AssetBundleBuild build = new AssetBundleBuild
            {
                assetBundleName = Path.GetFileName(targetPath),
                assetNames = dependencies
            };

            // ✅ Build 1 file duy nhất
            BuildPipeline.BuildAssetBundles(folder, new[] { build }, BuildAssetBundleOptions.None, buildTarget);
            Debug.Log($"✅ Built: {targetPath}");
        }

        if (openOutputFolder)
        {
            EditorUtility.RevealInFinder(folder);
        }
    }
}