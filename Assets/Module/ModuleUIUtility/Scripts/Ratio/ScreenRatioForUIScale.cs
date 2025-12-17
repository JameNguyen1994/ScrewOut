using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRatioForUIScale : MonoBehaviour
{
    [Serializable]
    public class RatioScale : RatioData<float>
    {
    }

    [SerializeField] public RectTransform target;
    [SerializeField] public List<RatioScale> ratioConfigs;

    private void Awake()
    {
        if (target == null)
        {
            target = GetComponent<RectTransform>();
        }

        Vector2 screenResolution = RatioService.GetScreenResolution();
        ApplyAspectRatioSettings(screenResolution.x, screenResolution.y);
    }

    private void ApplyAspectRatioSettings(float width, float height)
    {
        float currentRatio = RatioService.CalculateAspectRatio(width, height);
        const float tolerance = 0.01f;

        EditorLogger.Log($">>>Current ratio: {width}x{height} ~ {currentRatio}");

        foreach (var setting in ratioConfigs)
        {
            if (Mathf.Abs(setting.Ratio - currentRatio) <= tolerance)
            {
                EditorLogger.Log($"\">>>Applied ratio: {setting.Width}x{setting.Height} - {setting.Ratio:F2}");
                target.localScale = new Vector3(setting.Value, setting.Value, setting.Value);
                return;
            }
        }

        EditorLogger.LogWarning("\">>>No matching ratio found in settings!");
    }
}