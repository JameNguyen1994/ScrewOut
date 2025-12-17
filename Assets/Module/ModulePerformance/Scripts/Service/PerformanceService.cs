using UnityEngine;

public static class PerformanceService
{
    private const string KEY = "android_perf_mode";

    public static bool IsPerformanceMode { get; private set; }

    public static void Initialize()
    {
        // Only run on real IOS devices
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GoodDevice();
            return;
        }

        // Only run on real Android devices
        if (Application.platform != RuntimePlatform.Android)
        {
            // Other platforms will always use max settings
            IsPerformanceMode = false;
            return;
        }

        // If already detected before, load saved value
        if (PlayerPrefs.HasKey(KEY))
        {
            IsPerformanceMode = PlayerPrefs.GetInt(KEY) == 1;
            Apply(IsPerformanceMode);
            return;
        }

        // Detect device strength based on RAM
        int ramMB = SystemInfo.systemMemorySize; // Returns RAM in MB

        // Devices with RAM <= 6GB are considered low/mid-range
        bool isLowDevice = ramMB <= 6144;

        // Low device → performance mode ON
        IsPerformanceMode = !isLowDevice;
        Apply(IsPerformanceMode);

        PlayerPrefs.SetInt(KEY, IsPerformanceMode ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void SetPerformanceMode(bool value)
    {
        IsPerformanceMode = value;
        Apply(IsPerformanceMode);

        PlayerPrefs.SetInt(KEY, IsPerformanceMode ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void Apply(bool performanceMode)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }

        if (!performanceMode)
        {
            LowDevice();
        }
        else
        {
            GoodDevice();
        }
    }

    private static void LowDevice()
    {
        // Set quality level về thấp nhất (index 0)
        QualitySettings.SetQualityLevel(0, true);
        QualitySettings.shadows = ShadowQuality.Disable;      // Tắt shadows
        QualitySettings.antiAliasing = 0;                     // Tắt AA
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable; // Tắt Aniso
        QualitySettings.globalTextureMipmapLimit = 5;        // Giảm chất lượng texture
        QualitySettings.softParticles = false;               // Tắt soft particles
        QualitySettings.realtimeReflectionProbes = false;    // Tắt reflection probes
        QualitySettings.billboardsFaceCameraPosition = false;
        QualitySettings.vSyncCount = 0;                      // Tắt VSync
        QualitySettings.maxQueuedFrames = 1; // Giảm lag frame
        Application.targetFrameRate = 30; // Giới hạn FPS = 30
    }

    private static void GoodDevice()
    {
        // Set quality level về thấp nhất (index 0)
        QualitySettings.SetQualityLevel(0, true);
        QualitySettings.shadows = ShadowQuality.Disable;      // Tắt shadows
        QualitySettings.antiAliasing = 0;                     // Tắt AA
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable; // Tắt Aniso
        QualitySettings.globalTextureMipmapLimit = 1;        // Giảm chất lượng texture
        QualitySettings.softParticles = false;               // Tắt soft particles
        QualitySettings.realtimeReflectionProbes = false;    // Tắt reflection probes
        QualitySettings.billboardsFaceCameraPosition = false;
        QualitySettings.vSyncCount = 0;                      // Tắt VSync
        QualitySettings.maxQueuedFrames = 1;                 // Giảm lag frame
        Application.targetFrameRate = 60;   // Giới hạn FPS = 60
    }
}