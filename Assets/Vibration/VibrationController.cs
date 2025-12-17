using Cysharp.Threading.Tasks;
using PS.Utils;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationController : Singleton<VibrationController>
{
    private void Start()
    {
        Vibration.Init();
    }

    //public void Vibrate()
    //{
    //    //if (!DBController.Instance.VIBRATE) return;
    //    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
    //    {
    //        if (SystemInfo.supportsVibration)
    //        {
    //            Handheld.Vibrate();
    //        }
    //    }
    //}

    public void Vibrate(VibrationType type)
    {
        if (!Db.storage.SETTING_DATAS.vibra) return;
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer) return;
        if (!SystemInfo.supportsVibration) return;

        switch (type)
        {
            case VibrationType.VerySmall:
                Vibration.VibrateVerySmall();
                break;
            case VibrationType.Small:
                Vibration.VibratePop();
                break;
            case VibrationType.Medium:
                Vibration.VibratePeek();
                break;
            case VibrationType.Big:
                Vibration.VibrateNope();
                break;
            case VibrationType.Bigbang:
                Vibration.VibrateBigbang();
                break;

        }
        Debug.Log($"Vibrate: {type}");
    }

    public async UniTaskVoid DoubleVibrate(VibrationType type)
    {
        Vibrate(type);
        await UniTask.Delay(500);
        Vibrate(type);
    }

    public void SetVibrate(bool isOn)
    {
        //DBController.Instance.IS_VIBRATE = isOn;
    }
}

public enum VibrationType
{
    Small = 0,
    Medium = 1,
    Big = 2, //Kimochiii
    VerySmall = 3,
    Bigbang=4
}
