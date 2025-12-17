using UnityEngine;

/// <summary>
/// Handles continuous checking of user activity for speed boost feature.
/// </summary>
public class SpeedGameHandler : MonoBehaviour
{
    private void Update()
    {
        SpeedGameManager.Instance.CheckUserActivity();
    }

    /// <summary>
    /// Toggles active state of this handler.
    /// </summary>
    public void Active(bool active)
    {
        gameObject.SetActive(active);
    }
}