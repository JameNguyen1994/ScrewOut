using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StreamingImageBinder : MonoBehaviour
{
    [Header("Sprite path inside StreamingAssets")]
    [SerializeField] private string spritePathInStreamingAssets;
    [SerializeField] private bool autoLoad = true;
    [SerializeField] private Image image;
    [SerializeField] private bool isCache = true;

    private string _currentPath;
    private bool isLoading = false;

    private void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }

        if (autoLoad && !string.IsNullOrEmpty(spritePathInStreamingAssets))
        {
            LoadFromStreaming(spritePathInStreamingAssets).Forget();
        }
    }

    public async UniTask LoadFromStreaming(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            Debug.LogWarning($"[StreamingImageBinder] Invalid path on {name}");
            return;
        }

        if (_currentPath == relativePath && image.sprite != null)
            return;

        _currentPath = relativePath;

        isLoading = true;
        StreamingSpriteLoader.LoadSpriteAsync(relativePath, OnSpriteLoaded, isCache);
        await UniTask.WaitUntil(() => !isLoading);
    }

    private void OnSpriteLoaded(Sprite sprite, string relativePath)
    {
        if (_currentPath == relativePath)
        {
            isLoading = false;

            if (sprite == null)
            {
                Debug.LogError($"[StreamingImageBinder] Failed to load sprite: {_currentPath}");
                return;
            }

            image.sprite = sprite;
        }
    }
}