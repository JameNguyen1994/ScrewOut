using UnityEditor;
using UnityEngine;

public static class BuildConfigApplier
{
    private const string DevConfigPath = "Assets/Editor/BuildConfig/BuildConfigSODev.asset";
    private const string ProdConfigPath = "Assets/Editor/BuildConfig/BuildConfigSOProduction.asset";
    
    [MenuItem("Tools/Build Config/Open Dev Config File")]
    public static void OpenDevConfigFile()
    {
        SelectConfigFile(DevConfigPath);
    }

    [MenuItem("Tools/Build Config/Open Production Config File")]
    public static void OpenProdConfigFile()
    {
        SelectConfigFile(ProdConfigPath);
    }
    
    private static void SelectConfigFile(string path)
    {
        var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (asset == null)
        {
            Debug.LogError($"❌ Cannot find config at path: {path}");
            return;
        }

        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset); // Optional: highlight in Project
        Debug.Log($"📂 Selected config: {asset.name}");
    }

    [MenuItem("Tools/Build Config/Use Dev Config")]
    public static void UseDevConfig()
    {
        ApplyBuildConfig(AssetDatabase.LoadAssetAtPath<BuildConfig>(DevConfigPath));
    }

    [MenuItem("Tools/Build Config/Use Production Config")]
    public static void UseProductionConfig()
    {
        ApplyBuildConfig(AssetDatabase.LoadAssetAtPath<BuildConfig>(ProdConfigPath));
    }

    private static void ApplyBuildConfig(BuildConfig config)
    {
        if (config == null)
        {
            Debug.LogError("❌ BuildConfig not found!");
            return;
        }

        Debug.Log("✅ Applying Build Config: " + config.name);

#if UNITY_ANDROID
        // Android
        PlayerSettings.bundleVersion = config.buildVersionAOS;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, config.packageNameAOS);
        PlayerSettings.Android.bundleVersionCode = config.bundleVersionCodeAOS;
        PlayerSettings.companyName = config.companyNameAOS;
        PlayerSettings.productName = config.productNameAOS;
#endif

#if UNITY_IOS
        // iOS
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, config.bundleIdentifierIOS);
        PlayerSettings.iOS.buildNumber = config.buildIOS.ToString();
        PlayerSettings.iOS.appleDeveloperTeamID = config.signingTeamIDIOS;
        PlayerSettings.bundleVersion = config.buildVersionIOS;

        // Optional: Switch company/product for iOS if needed (override)
        if (!string.IsNullOrEmpty(config.companyNameIOS)) PlayerSettings.companyName = config.companyNameIOS;
        if (!string.IsNullOrEmpty(config.productNameIOS)) PlayerSettings.productName = config.productNameIOS;
#endif

        Debug.Log("🎯 BuildConfig applied successfully.");
        
        RepaintSettingsWindow();
    }
    
    private static void RepaintSettingsWindow()
    {
        var windowType = typeof(Editor).Assembly.GetType("UnityEditor.PlayerSettingsEditor");
        if (windowType == null) return;

        var windows = Resources.FindObjectsOfTypeAll(windowType);
        foreach (var win in windows)
        {
            var editorWindow = win as EditorWindow;
            editorWindow?.Repaint();
        }
    }
}