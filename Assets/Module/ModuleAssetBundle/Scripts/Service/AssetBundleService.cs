using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// Handles loading, caching, and cleanup of AssetBundles at runtime.
/// Supports prefab caching, shader reassignment, and retry logic for network resilience.
/// </summary>
public static partial class AssetBundleService
{
    // Cache for instantiated prefabs to avoid reloading from AssetBundle
    private static readonly Dictionary<string, GameObject> _prefabCache = new();

    // Cache for loaded AssetBundles to prevent repeated disk/network loading
    private static readonly Dictionary<string, AssetBundle> _bundleCache = new();

    public static bool IsLoadAssetBundle = false;

    // Cached AssetBundleConfig for lookup
    private static AssetBundleConfig _config;
    public static AssetBundleConfig Config
    {
        get
        {
            // Load configuration from Resources if not loaded yet
            if (_config == null)
            {
                _config = Resources.Load<AssetBundleConfig>(AssetBundleDefine.ASSET_BUNDLE_CONFIG);
                if (_config == null)
                {
                    LoggerError("[AssetBundle] Missing AssetBundleConfig resource.");
                }
            }
            return _config;
        }
    }

    /// <summary>
    /// Retrieves AssetBundleData by its bundle ID.
    /// </summary>
    public static AssetBundleData GetBundleDataById(string id)
    {
        if (Config == null || Config.BundleDatas == null)
        {
            return null;
        }

        for (int i = 0; i < Config.BundleDatas.Count; i++)
        {
            if (Config.BundleDatas[i].GetId().Equals(id))
            {
                return Config.BundleDatas[i];
            }
        }

        for (int i = 0; i < Config.BundleBonusDatas.Count; i++)
        {
            if (Config.BundleBonusDatas[i].GetId().Equals(id))
            {
                return Config.BundleBonusDatas[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Loads and instantiates a prefab by bundle ID asynchronously.
    /// Automatically retries if loading fails and deletes corrupted local cache.
    /// </summary>
    public static async UniTask<GameObject> LoadPrefabAsync(string bundleId, int retryCount)
    {
        await UniTask.WaitUntil(() => !IsLoadAssetBundle);

        Logger($"[AssetBundle] LoadPrefabAsync bundleId={bundleId}, Retry={retryCount}");
        GameObject prefabInstance = null;

        AssetBundleData bundleData = GetBundleDataById(bundleId);
        if (bundleData == null)
        {
            LoggerError($"[AssetBundle] No AssetBundle data found for bundleId {bundleId}");
            return null;
        }

        // Attempt to load prefab from local cache or download if missing
        IsLoadAssetBundle = true;
        prefabInstance = await LoadPrefabAsync(bundleData);
        IsLoadAssetBundle = false;

        // Retry logic for corrupted downloads or failed loads
        if (prefabInstance == null)
        {
            string localPath = GetLocalPath(bundleData.GetBundleName());

            if (File.Exists(localPath))
            {
                File.Delete(localPath); // Delete corrupted cache
            }

            if (retryCount > 0)
            {
                Logger($"[AssetBundle] Retry LoadPrefabAsync Retry={retryCount - 1}");
                return await LoadPrefabAsync(bundleId, retryCount - 1);
            }
        }

        return prefabInstance;
    }

    /// <summary>
    /// Core logic for loading a prefab from AssetBundle asynchronously.
    /// Handles caching, disk storage, and shader reassignment.
    /// </summary>
    private static async UniTask<GameObject> LoadPrefabAsync(AssetBundleData bundleData)
    {
        Logger($"[AssetBundle] LoadPrefabAsync BundleUrl={bundleData.GetBundleURL()}, BundleName={bundleData.GetBundleName()}");
        GameObject prefabInstance = null;

        string fullURL = Config.GetFullURL(bundleData.GetBundleURL());

        // Return cached prefab if already loaded
        if (_prefabCache.TryGetValue(bundleData.GetBundleName(), out GameObject cachedPrefab) && cachedPrefab != null)
        {
            return cachedPrefab;
        }

        Logger($"[AssetBundle] Ensure the AssetBundle exists locally; download if missing");
        // Ensure the AssetBundle exists locally; download if missing
        string localPath = await EnsureLocalBundleAsync(fullURL, bundleData.GetBundleName());

        if (string.IsNullOrEmpty(localPath))
        {
            LoggerError($"[AssetBundle] Invalid local path for {bundleData.GetBundleName()}");
            return null;
        }

        AssetBundle bundle = null;

        // Load AssetBundle from memory cache or disk
        if (!_bundleCache.TryGetValue(bundleData.GetBundleName(), out bundle))
        {
            try
            {
                var bundleLoading = AssetBundle.LoadFromFileAsync(localPath);
                await bundleLoading.ToUniTask();

                bundle = bundleLoading.assetBundle;

                if (bundle == null)
                {
                    LoggerError($"[AssetBundle] Failed to load AssetBundle from {localPath}");
                    return null;
                }

                if (_bundleCache != null)
                {
                    _bundleCache[bundleData.GetBundleName()] = bundle;
                }
            }
            catch (Exception e)
            {
                LoggerError($"[AssetBundle] Exception {localPath} " + e.Message);
                return null;
            }
        }

        // Load prefab from AssetBundle
        var assetRequest = bundle.LoadAssetAsync<GameObject>(bundleData.GetPrefabName());
        await assetRequest.ToUniTask();

        if (assetRequest.asset != null && assetRequest.asset is GameObject prefab)
        {
            ReassignShaders(prefab);// Fix shaders if missing
            _prefabCache[bundleData.GetBundleName()] = prefab;// Cache prefab
            prefabInstance = prefab;
        }
        else
        {
            LoggerError($"[AssetBundle] Prefab '{bundleData.GetPrefabName()}' not found in bundle {bundleData.GetBundleName()}");
        }

        return prefabInstance;
    }

    /// <summary>
    /// Fixes missing shaders in the prefab after loading from AssetBundle.
    /// </summary>
    public static void ReassignShaders(GameObject root)
    {
        if (root == null) return;

        foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null || renderer.sharedMaterials == null)
                continue;

            var materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
                if (material == null) continue;

                string shaderName = material.shader?.name;
                if (string.IsNullOrEmpty(shaderName)) continue;

                Shader foundShader = Shader.Find(shaderName);
                if (foundShader != null && material.shader != foundShader)
                {
                    material.shader = foundShader; // Reassign shader if missing
                    Logger($"[ShaderFix] Material '{material.name}' reassigned to '{foundShader.name}'");
                }
                else if (foundShader == null)
                {
                    LoggerError($"[ShaderFix] Shader '{shaderName}' not found in build.");
                }
            }
        }
    }

    /// <summary>
    /// Unloads a specific AssetBundle from memory.
    /// </summary>
    public static void UnloadBundle(AssetBundleData bundleData)
    {
        if (_bundleCache.TryGetValue(bundleData.GetBundleName(), out AssetBundle bundle))
        {
            bundle.Unload(false); // Do not destroy instantiated prefabs
            _bundleCache.Remove(bundleData.GetBundleName());
            Logger($"[AssetBundle] Unloaded: {bundleData.GetBundleName()}");
        }

        if (_prefabCache.ContainsKey(bundleData.GetBundleName()))
        {
            _prefabCache.Remove(bundleData.GetBundleName());
        }
    }

    /// <summary>
    /// Clears all cached AssetBundles and prefabs from memory.
    /// </summary>
    public static void ClearCache()
    {
        foreach (var bundle in _bundleCache.Values)
        {
            bundle.Unload(false); // Keep instantiated objects alive
        }

        _bundleCache.Clear();
        _prefabCache.Clear();

        Logger("[AssetBundle] Cleared all caches.");
    }

    /// <summary>
    /// Ensures the AssetBundle exists locally; downloads it if missing.
    /// Returns the local file path if successful.
    /// </summary>
    public static async UniTask<string> EnsureLocalBundleAsync(string bundleFullUrl, string bundleName)
    {
        string localPath = GetLocalPath(bundleName);

        if (!File.Exists(localPath))
        {
            EnsureLocalBundleDirectory();                       // Make sure folder exists
            bool success = await WebRequestService.DownloadFileAsync(bundleFullUrl, localPath); // Download file

            if (!success)
            {
                Logger("[AssetBundle] Download failed");
                return string.Empty;
            }
        }

        return localPath;
    }

    /// <summary>
    /// Creates the folder for local AssetBundles if it does not exist.
    /// </summary>
    private static void EnsureLocalBundleDirectory()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, GetProjectCode());

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    /// <summary>
    /// Deletes the local cached AssetBundle for a specific level.
    /// </summary>
    public static void RemoveOldLevelBundleAsset(int level)
    {
        AssetBundleData bundleData = GetBundleDataByLevel(level);

        if (bundleData != null)
        {
            string localPath = GetLocalPath(bundleData.GetBundleName());

            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }
        }
    }

    /// <summary>
    /// Returns the full local path to the cached AssetBundle.
    /// </summary>
    public static string GetLocalPath(string bundleName)
    {
        string localPath = Path.Combine(Application.persistentDataPath, GetProjectCode(), bundleName);
        return localPath;
    }

    // Logging helpers
    public static void Logger(string log) => Debug.Log(log);
    public static void LoggerError(string log) => Debug.LogWarning(log);

    /// <summary>
    /// Returns the current runtime platform.
    /// </summary>
    public static Platform GetPlatform()
    {
#if UNITY_ANDROID
        return Platform.android;
#elif UNITY_IOS
        return Platform.ios;
#else
        return Platform.pc;
#endif
    }

    /// <summary>
    /// Returns retry time
    /// </summary>
    public static int GetRetry()
    {
        return Config.Retry;
    }

    /// <summary>
    /// Returns project code
    /// </summary>
    public static string GetProjectCode()
    {
        return AssetBundleDefine.PROJECT_CODE;
    }
}