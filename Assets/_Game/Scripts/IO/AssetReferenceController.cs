using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PS.Analytic;
using Storage;
using Storage.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetReferenceController : Singleton<AssetReferenceController>
{
    public bool IsCompleted;
    public bool IsLoading;

    private int timeLoad = 3;
    private readonly Dictionary<string, AsyncOperationHandle> _loadedAssets = new();

    public async void Start()
    {
        await UniTask.Delay(10);
        var task1 = UniTask.WaitForSeconds(timeLoad);
        var task2 = UniTask.WaitUntil(() => GameAnalyticController.Instance.Remote().IsReadyRemote);

        await UniTask.WhenAny(task1, task2);

        //Load level cache before play game
        UserInfo user = Db.storage.USER_INFO;
        await AssetBundleService.CacheLevel(user.level);

        IsCompleted = true;
    }

    public string GetAssetKeyByLevel(int level)
    {
        return string.Format(Define.LEVEL_ASSET_KEY, level);
    }
    public string GetAssetKeyByLevelBonus(int level)
    {
        return string.Format(Define.LEVEL_BONUS_ASSET_KEY, level);
    }

    public async UniTask<T> LoadLevelPrefabAsync<T>(int level)
    {
        return await LoadComponentAsync<T>(GetAssetKeyByLevel(level));
    }
    public async UniTask<T> LoadLevelBonusPrefabAsync<T>(int level)
    {
        return await LoadComponentAsync<T>(GetAssetKeyByLevelBonus(level));
    }

    public async UniTask<T> LoadComponentAsync<T>(string assetKey)
    {
        await UniTask.WaitUntil(() => !IsLoading);
        IsLoading = true;

        if (_loadedAssets.TryGetValue(assetKey, out var existingHandle))
        {
            IsLoading = false;
            return ((GameObject)existingHandle.Result).GetComponent<T>();
        }

        try
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(assetKey);
            GameObject prefab = await handle.ToUniTask();

            if (prefab == null)
            {
                Debug.LogWarning($"[LoadComponentAsync] Failed to load asset: {assetKey}");
                IsLoading = false;
                return default;
            }

            _loadedAssets[assetKey] = handle;
            IsLoading = false;
            return prefab.GetComponent<T>();
        }
        catch (System.Exception e)
        {
            Debug.LogError("[LoadComponentAsync] Failed ERROR " + e.Message);
        }

        IsLoading = false;
        return default;
    }

    public void UnloadAsset(int level)
    {
        UnloadAsset(GetAssetKeyByLevel(level));
    }

    public void UnloadAsset(string assetKey)
    {
        if (_loadedAssets.TryGetValue(assetKey, out var handle))
        {
            Addressables.Release(handle);
            _loadedAssets.Remove(assetKey);
        }
    }

    public void UnloadAllAssets()
    {
        foreach (var handle in _loadedAssets.Values)
        {
            Addressables.Release(handle);
        }

        _loadedAssets.Clear();
    }

    public async UniTask<bool> IsHaveCachLevel(int level)
    {
        await UniTask.WaitUntil(() => !IsLoading);

        if (_loadedAssets.ContainsKey(GetAssetKeyByLevel(level)))
        {
            return true;
        }

        return false;
    }
}
