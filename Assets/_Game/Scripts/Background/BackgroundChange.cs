using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Bson;
using Storage;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundChange : MonoBehaviour
{
    [SerializeField] private StreamingImageBinder backgroundRenderer;

    private void OnEnable()
    {
        EventDispatcher.Register(EventId.ChangeBackground, ChangeBackground);
    }
    private void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.ChangeBackground, ChangeBackground);
    }

    public void ChangeBackground(object data = null)
    {
        ChangeBackgroundAsync(data).Forget();
    }

    public async UniTask ChangeBackgroundAsync(object data = null)
    {
        var level = Db.storage.USER_INFO.level;
        var backgroundName = BackgroundDataHelper.Instance.BackgroundDataSO.GetCurrentBackground(level);

        if (string.IsNullOrEmpty(backgroundName))
        {
            return;
        }

        string path = $"Background/{backgroundName}.jpg";
        await backgroundRenderer.LoadFromStreaming(path);
    }
}