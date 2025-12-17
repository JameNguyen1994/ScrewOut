using System;
using UnityEngine;

public static class PlayerPrefsSaveTime
{
    private const string SaveTimeKey = "LAST_SAVETIME_UTC";

    public static void SaveNow()
    {
        string utcTime = DateTime.UtcNow.ToString("o"); // ISO 8601
        PlayerPrefs.SetString(SaveTimeKey, utcTime);
        PlayerPrefs.Save();

        EditorLogger.Log($"[SaveTime] Saved at {utcTime}");
    }

    public static double GetMinutesSinceLastSave()
    {
        if (!PlayerPrefs.HasKey(SaveTimeKey))
        {
            EditorLogger.LogWarning("[SaveTime] No previous save found.");
            return -1;
        }

        try
        {
            string savedTime = PlayerPrefs.GetString(SaveTimeKey);
            DateTime lastSave = DateTime.Parse(savedTime, null, System.Globalization.DateTimeStyles.RoundtripKind);

            TimeSpan elapsed = DateTime.UtcNow - lastSave;
            EditorLogger.Log($"[SaveTime] Minutes since last save: {elapsed.TotalMinutes:F1}");

            return elapsed.TotalMinutes;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveTime] {e.Message}");
        }

        return -1;
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(SaveTimeKey);
        PlayerPrefs.Save();

        EditorLogger.Log("[SaveTime] Clear");
    }

    public static bool IsNoneData()
    {
        return GetMinutesSinceLastSave() == -1;
    }
}