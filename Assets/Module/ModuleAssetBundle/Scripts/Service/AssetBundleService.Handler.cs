using UnityEngine;
using Cysharp.Threading.Tasks;

public static partial class AssetBundleService
{
    /// <summary>
    /// Checks if a level has an associated AssetBundle configuration.
    /// </summary>
    public static bool HasAssetBundle(int level)
    {
        return GetBundleDataByLevel(level) != null;
    }

    /// <summary>
    /// Retrieves AssetBundleData for a given level.
    /// </summary>
    public static AssetBundleData GetBundleDataByLevel(int level)
    {
        return GetBundleDataById(GetBundleIdByLevel(level));
    }

    /// <summary>
    /// Generates a bundle ID string based on level.
    /// </summary>
    private static string GetBundleIdByLevel(int level)
    {
        return $"Level_{level}";
    }

    /// <summary>
    /// Loads and instantiates a prefab for a given level asynchronously.
    /// Handles retries on failure.
    /// </summary>
    public static async UniTask<GameObject> LoadPrefabAsync(int level)
    {
        Logger($"[AssetBundle] LoadPrefabAsync Level={level}, Retry={GetRetry()}");
        return await LoadPrefabAsync(GetBundleIdByLevel(level), GetRetry());
    }

    /// <summary>
    /// Loads and instantiates a prefab for a given bundleId asynchronously.
    /// Handles retries on failure.
    /// </summary>
    public static async UniTask<GameObject> LoadPrefabAsync(string bundleId)
    {
        Logger($"[AssetBundle] LoadPrefabAsync Bundle Id={bundleId}, Retry={GetRetry()}");
        return await LoadPrefabAsync(bundleId, GetRetry());
    }

    public static void OnWinLevel(int level)
    {
        Logger("[AssetBundle] OnWinLevel");

        //Clear Old Level
        Logger("[AssetBundle] Clear Old Level");
        int realLevel = LevelMapService.GetLevelMap(level);
        AssetBundleData data = GetBundleDataByLevel(realLevel);
        if (data != null)
        {
            UnloadBundle(data);
        }

        AssetReferenceController.Instance.UnloadAsset(realLevel);

        //Load Next Level
        Logger("[AssetBundle] Load Next Level");
        CacheLevel(level + 1).Forget();
        DownloadMap(level + 2).Forget();
        DownloadMap(level + 3).Forget();
    }

    public static async UniTask CacheLevel(int level, bool isCaheAssetBundle = false)
    {
        int realLevel = LevelMapService.GetLevelMap(level);
        AssetBundleData dataNext = GetBundleDataByLevel(realLevel);
        if (dataNext != null)
        {
            await LoadPrefabAsync(realLevel);
        }
        else if (!isCaheAssetBundle)
        {
            try
            {
                Logger("[AssetBundle] Have No Data Load Level Local");
                await AssetReferenceController.Instance.LoadLevelPrefabAsync<LevelMap>(realLevel);
            }
            catch (System.Exception e)
            {
                Debug.LogError("[AssetReferenceController] ERROR " + e.Message);
            }
        }
    }

    public static async UniTask<bool> IsHaveCachLevel(int level)
    {
        int realLevel = LevelMapService.GetLevelMap(level);
        AssetBundleData bundleData = GetBundleDataByLevel(realLevel);

        if (bundleData != null)
        {
            if (_prefabCache.ContainsKey(bundleData.GetBundleName()))
            {
                return true;
            }

            return false;
        }

        return await AssetReferenceController.Instance.IsHaveCachLevel(realLevel);
    }

    public static async UniTask DownloadMap(int level)
    {
        var bundleData = GetBundleDataByLevel(level);
        if (bundleData == null)
        {
            return;
        }

        string fullURL = Config.GetFullURL(bundleData.GetBundleURL());
        await EnsureLocalBundleAsync(fullURL, bundleData.GetBundleName());
    }
    public static async UniTask DownloadMapLevelBonus(string bundleId)
    {
        AssetBundleData bundleData = GetBundleDataById(bundleId);
        if (bundleData == null)
        {
            return;
        }
        string fullURL = Config.GetFullURL(bundleData.GetBundleURL());
        await EnsureLocalBundleAsync(fullURL, bundleData.GetBundleName());
    }
}