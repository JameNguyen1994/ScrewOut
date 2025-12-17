using UnityEngine;

/// <summary>
/// Controls temporary speed boost when user taps rapidly.
/// Example use case: speeding up DOTween animations or gameplay tempo.
/// </summary>
public class SpeedGameManager : Singleton<SpeedGameManager>
{
    [Header("Tap Detection Settings")]
    [SerializeField] private float tapInterval = 0.15f;   // Max time (in seconds) between taps to be considered a combo
    [SerializeField] private int tapThreshold = 3;        // Number of rapid taps required to trigger a boost
    [SerializeField] private float boostedSpeed = 2f;     // Boosted timescale multiplier

    [Header("References")]
    [SerializeField] private SpeedGameHandler speedGameHandler;

    private int tapCount = 0;
    private float lastTapTime;
    private bool isBoosted;
    private bool isActive;

    /// <summary>
    /// Checks if the player is tapping rapidly enough to trigger a boost.
    /// Should be called every frame (e.g., in Update).
    /// </summary>
    public void CheckUserActivity()
    {
        if (!isActive || isBoosted)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            float now = Time.time;

            if (now - lastTapTime <= tapInterval)
                tapCount++;
            else
                tapCount = 1;

            lastTapTime = now;

            if (tapCount >= tapThreshold)
                ActivateBoost();
        }
    }

    /// <summary>
    /// Activates temporary boost by increasing game timescale.
    /// </summary>
    private void ActivateBoost()
    {
        isBoosted = true;

        //DOTween.timeScale = boostedSpeed;
        Time.timeScale = boostedSpeed;

        EditorLogger.Log($"🔥 [SpeedGame] Speed boost activated ×{boostedSpeed}");
    }

    /// <summary>
    /// Resets game speed and clears tap combo data.
    /// </summary>
    private void ResetSpeed()
    {
        isBoosted = false;
        tapCount = 0;

        //DOTween.timeScale = 1f;
        Time.timeScale = 1f;

        EditorLogger.Log("⚡[SpeedGame] Speed boost reset");
    }

    /// <summary>
    /// Enables or disables tap detection and speed logic.
    /// </summary>
    public void Active(bool active)
    {
        isActive = active;
        speedGameHandler.Active(active);

        if (!active)
            ResetSpeed();

        EditorLogger.Log($"[SpeedGame] Active: {active}, Boost Multiplier: {boostedSpeed}");
    }
}