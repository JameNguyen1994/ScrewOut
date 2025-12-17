using UnityEngine;
using UnityEngine.UI;

public class PerformanceToggleUI : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            toggle.isOn = PerformanceService.IsPerformanceMode;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnChanged(bool value)
    {
        PerformanceService.SetPerformanceMode(value);
    }
}