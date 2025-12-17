using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class StreamingSpriteLoader
{
    private static readonly Dictionary<string, Sprite> spriteCache = new();
    private static readonly Dictionary<string, Action<Sprite, string>> loadingRequests = new();

    /// <summary>
    /// Loads a sprite asynchronously from StreamingAssets using callback.
    /// Multiple requests for the same sprite will wait for the same load.
    /// </summary>
    public static void LoadSpriteAsync(string relativePath, Action<Sprite, string> onLoaded, bool isCache = true)
    {
        // Cached → return immediately
        if (spriteCache.ContainsKey(relativePath))
        {
            onLoaded?.Invoke(spriteCache[relativePath], relativePath);
            return;
        }

        // Already loading → queue callback
        if (loadingRequests.ContainsKey(relativePath))
        {
            loadingRequests[relativePath] += onLoaded;
            return;
        }

        //Add new loading
        if (!loadingRequests.ContainsKey(relativePath))
        {
            loadingRequests[relativePath] = onLoaded;
        }

        AsyncRunner.RunCoroutine(LoadSpriteCoroutine(relativePath, isCache));
    }

    private static IEnumerator LoadSpriteCoroutine(string relativePath, bool isCache)
    {
        string fullPath;

#if UNITY_ANDROID && !UNITY_EDITOR
        fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
#elif UNITY_IOS && !UNITY_EDITOR
        fullPath = "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
#else
        fullPath = "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
#endif
        Debug.Log($"[LoadSprite] URL = {fullPath}");

        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(fullPath);
        yield return request.SendWebRequest();

        Sprite sprite = null;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[StreamingSpriteLoader] Failed to load: {fullPath}\n{request.error}");
        }
        else
        {
            byte[] data = request.downloadHandler.data;

            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (tex.LoadImage(data, false)) // ✅ Safe decode from PNG/JPG bytes
            {
                tex.filterMode = FilterMode.Point;
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.Apply();

                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);

                if (isCache)
                {
                    spriteCache[relativePath] = sprite;
                }
            }
            else
            {
                Debug.LogError($"[StreamingSpriteLoader] LoadImage() failed: {relativePath}");
            }
        }

        if (loadingRequests.ContainsKey(relativePath))
        {
            loadingRequests[relativePath].Invoke(sprite, relativePath);
            loadingRequests.Remove(relativePath);
        }
    }

    public static void ClearCacheAll()
    {
        spriteCache.Clear();
        loadingRequests.Clear();
    }

    public static void ClearCacheByPath(string relativePath)
    {
        if (spriteCache.ContainsKey(relativePath))
        {
            spriteCache.Remove(relativePath);
        }

        if (loadingRequests.ContainsKey(relativePath))
        {
            loadingRequests.Remove(relativePath);
        }
    }
}