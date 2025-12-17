using System.Collections.Generic;
using UnityEngine;

public class RatioService
{
    public const float TOLERANCE = 0.01f;

    public static T GetValue<T>(List<RatioData<T>> configs, T defaultValue)
    {
        Vector2 screenResolution = GetScreenResolution();
        float currentRatio = CalculateAspectRatio(screenResolution.x, screenResolution.y);

        EditorLogger.Log($">>>Current ratio: {screenResolution.x}x{screenResolution.y}");

        foreach (var setting in configs)
        {
            if (Mathf.Abs(setting.Ratio - currentRatio) <= TOLERANCE)
            {
                EditorLogger.Log($"\">>>Applied ratio: {setting.Width}x{setting.Height} - {setting.Ratio:F2}");
                return setting.Value;
            }
        }

        return defaultValue;
    }

    public static Vector2 GetScreenResolution()
    {
#if UNITY_EDITOR
        return UnityEditor.Handles.GetMainGameViewSize();
#else
        return new Vector2(Screen.width , Screen.height);
#endif
    }

    public static float CalculateAspectRatio(float screenWidth, float screenHeight)
    {
        return Mathf.Round((screenWidth / screenHeight) * 100f) / 100f;
    }
}