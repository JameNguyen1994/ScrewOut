using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class RoundedProgressBar : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform fillRect;

    [Header("Settings")]
    [Range(0f, 1f)] public float progress = 0f; // 0..1

    private float bgWidth = -1;
    private TweenerCore<float, float, FloatOptions> task;

    private float GetBGWidth()
    {
        if (bgWidth == -1)
        {
            bgWidth = fillRect.sizeDelta.x;
        }

        return bgWidth;
    }

    public async UniTask SetProgress(float value, float tweenDuration)
    {
        if (task != null)
        {
            task.Kill();
        }

        if (fillRect.sizeDelta.x == GetBGWidth())
        {
            Vector2 s = fillRect.sizeDelta;
            s.x = 0;
            fillRect.sizeDelta = s;
        }

        value = Mathf.Clamp01(value);
        progress = value;

        // Calculate target width in pixels
        float targetWidth = GetBGWidth() * progress;

        if (tweenDuration <= 0f)
        {
            // immediate set
            Vector2 s = fillRect.sizeDelta;
            s.x = targetWidth;
            fillRect.sizeDelta = s;
        }
        else
        {
            float start = fillRect.sizeDelta.x;
            task = DOTween.To(() => start, x =>
            {
                Vector2 s = fillRect.sizeDelta;
                s.x = x;
                fillRect.sizeDelta = s;
            }, targetWidth, tweenDuration).SetEase(Ease.OutCubic);

            await UniTask.Delay((int)(tweenDuration * 1000));
        }
    }

    public void SetProgress(float value)
    {
        value = Mathf.Clamp01(value);
        progress = value;
        float targetWidth = GetBGWidth() * progress;
        Vector2 s = fillRect.sizeDelta;
        s.x = targetWidth;
        fillRect.sizeDelta = s;
    }
}