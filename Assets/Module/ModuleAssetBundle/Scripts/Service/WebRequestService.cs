using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// Handles downloading files from a URL to local disk asynchronously.
/// Uses UnityWebRequest with DownloadHandlerFile for efficient large file downloads.
/// </summary>
public class WebRequestService
{
    public static bool IsDownloadFile = false;

    /// <summary>
    /// Downloads a file from the specified URL and saves it directly to disk.
    /// </summary>
    /// <param name="url">The URL of the file to download.</param>
    /// <param name="savePath">The full local path where the file should be saved.</param>
    /// <param name="timeoutSeconds">Timeout in seconds before the request fails.</param>
    /// <returns>True if download succeeded, false if failed.</returns>
    public static async UniTask<bool> DownloadFileAsync(string url, string savePath, int timeoutSeconds = 3)
    {
        await UniTask.WaitUntil(() => !IsDownloadFile);
        IsDownloadFile = true;
        AssetBundleService.Logger($"[DownloadFileAsync] Downloaded file url {url}");

        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Set the download handler to write the file directly to disk
                // This avoids loading the entire file into memory, safe for large files
                request.downloadHandler = new DownloadHandlerFile(savePath);

                // Set the timeout for the request in seconds
                request.timeout = timeoutSeconds;

                // Send the request asynchronously and wait until completion
                await request.SendWebRequest().ToUniTask();

                // Check for request success
                if (request.result != UnityWebRequest.Result.Success)
                {
                    // Log error if download failed
                    AssetBundleService.LoggerError($"[DownloadFileAsync] Download failed: {request.error}");
                    IsDownloadFile = false;
                    return false;
                }

                // Log success and return true
                AssetBundleService.Logger($"[DownloadFileAsync] Downloaded file saved to {savePath}");
                IsDownloadFile = false;
                return true;
            }
        }
        catch (Exception e)
        {
            IsDownloadFile = false;
            AssetBundleService.LoggerError($"[DownloadFileAsync] Exception: {e.Message}");
        }

        IsDownloadFile = false;
        return false;
    }
}