using PS.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleWarning : Singleton<HoleWarning>
{
    [SerializeField] private GameObject gobjRedWarning;
    [SerializeField] private IconHoldWarningController iconHoldWarningController;

    public void ShowWarning()
    {
     //   gobjRedWarning.SetActive(true);
        VibrationController.Instance.DoubleVibrate(VibrationType.Medium).Forget();
        iconHoldWarningController.EnableEffect(true);
    }
    public void HideWarning()
    {
        gobjRedWarning.SetActive(false);
        iconHoldWarningController.EnableEffect(false);

    }
}
