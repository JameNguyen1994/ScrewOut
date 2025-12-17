using System;
using Storage;
using UnityEngine;

public class GameEventListener: MonoBehaviour
{
    private void OnEnable()
    {
        EventDispatcher.Register(EventId.OnBoxCompleted, OnBoxCompletedEvent);
        LevelController.OnGameWinExpAddEvent += OnGameWinExpAddEvent;
    }

    [System.Reflection.Obfuscation(Exclude = false)]
    private void OnGameWinExpAddEvent(int exp, int level)
    {
        var trkData = Db.storage.TRK_DATA.DeepClone();

        if (Db.storage.USER_INFO.level % 2 == 0)
        {
            trkData.mulScoreExp += exp;
        }
        else
        {
            trkData.singleScoreExp += exp;
        }

        trkData.expOfSection += exp;
        Db.storage.TRK_DATA = trkData;
        
#if UNITY_EDITOR
        IEconomicTracking tracking = new EconomicTrackingUnity();
#elif UNITY_ANDROID
            IEconomicTracking tracking = new EconomicTrackingAndroid();
#else
            IEconomicTracking tracking = new EconomicTrackingIos();
#endif

        print($"Game Event Listener - Level Finish: {level}");
        tracking.SendLevelFinish(level);
    }

    private void OnBoxCompletedEvent(object arg0)
    {
        int boxCount = (int)arg0;
        boxCount = ((boxCount - 1) % 6) + 1;
        var trkData = Db.storage.TRK_DATA.DeepClone();
        
        EditorLogger.Log($"box count: {boxCount} : {arg0}");
        
        switch (boxCount)
        {
            case 1:
                trkData.numX1OfLine++;
                break;
            case 2:
                trkData.numX2OfLine++;
                break;
            case 3:
                trkData.numX3OfLine++;
                break;
            case 4:
                trkData.numX4OfLine++;
                break;
            case 5:
                trkData.numX5OfLine++;
                break;
            case 6:
                trkData.numX6OfLine++;
                break;
        }
        Db.storage.TRK_DATA = trkData;
    }

    private void OnDestroy()
    {
        EventDispatcher.RemoveCallback(EventId.OnBoxCompleted, OnBoxCompletedEvent);
        LevelController.OnGameWinExpAddEvent -= OnGameWinExpAddEvent;
    }
}
