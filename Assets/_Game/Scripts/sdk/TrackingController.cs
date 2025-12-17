using Beebyte.Obfuscator;
using Cysharp.Threading.Tasks;
using GameAnalyticsSDK;
using Life;
using Newtonsoft.Json;
using ps.modules.leaderboard;
using PS.Analytic;
// using Firebase.Analytics;
using PS.Analytic.Event;
using PS.Utils;
using ResourceIAP;
using Storage;
using Storage.Model;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;

public class TrackingController : Singleton<TrackingController>
{
    private int _playTime;
    public int PlayTime => _playTime;

    Coroutine countGameplay;

    public async UniTask TrackingStartSession()
    {
        await UniTask.WaitUntil(() => Db.storage != null);

        var user = Db.storage.USER_INFO;
        user.totalSession++;
        Db.storage.USER_INFO = user;
        //   GameAnalyticController.Instance.Tracking().TrackingSession(user.totalSession);
    }

    private void TrackingStartLevel()
    {
        //FirebaseAnalyticController.TrackingLevelStart($"{_level}", "");
    }

    public void StartSessionGamePlay()
    {
        var user = Db.storage.USER_INFO;
        user.playTime = 0;
        user.playTimeShowAds = 0;
        Db.storage.USER_INFO = user;
        _playTime = 0;
        GameAnalyticController.Instance.Tracking().TrackingStartLevelProgression(Db.storage.USER_INFO.level);
        GameAnalyticController.Instance.Tracking().Tracking_LEVEL_START(user.level);
        if (countGameplay != null)
            StopCoroutine(countGameplay);
        countGameplay = StartCoroutine(CountTimeGameplay());
    }

/*    public void OnQuitGameSave()
    {
        StopCoroutine(CountTimeGameplay());
        var user = Db.storage.USER_INFO;
        user.playTime += _playTime;
        Db.storage.USER_INFO = user;
    }*/

    IEnumerator CountTimeGameplay()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            var user = Db.storage.USER_INFO;
            user.playTime += 1;
#if UNITY_EDITOR
            Debug.Log($"Playtime: {user.playTime}");
#endif
            Db.storage.USER_INFO = user;
        }
    }

    public void TrackingBooster(BoosterType boosterType)
    {
        GameAnalyticController.Instance.Tracking().Tracking_BOOSTERS(boosterType);
    }

    public void TrackingPowerUP(PreBoosterType preBoosterType, PreBoosterPlace preBoosterPlace)
    {
        GameAnalyticController.Instance.Tracking().Tracking_POWERUP(preBoosterType, preBoosterPlace);
    }

    private void StopSessionGamePlay()
    {
        var user = Db.storage.USER_INFO;
        _playTime = user.playTime;
        user.playTime = 0;
        user.playTimeShowAds = 0;

        Db.storage.USER_INFO = user;
    }

    public void TrackingBooster(int level, float percentageSolve, string flowType, BoosterType boosterType)
    {
        //GameAnalyticController.Instance.Tracking().Tracking_BOOSTERS(preBoosterType);
    }

    public void TrackingInventory(int level, float percentageSolve)
    {
        return;
        Dictionary<string, object> inventoryDict = new Dictionary<string, object>()
        {
            ["coin"] = Db.storage.USER_INFO.coin.GetDecrypted(),
            ["life"] = DBLifeController.Instance.LIFE_INFO.lifeAmount.GetDecrypted(),
            ["hammer"] = Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.Hammer),
            ["add_hole"] = Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.AddHole),
            ["clear"] = Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.Clears),
            ["unlock_box"] = Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.UnlockBox),
        };

        string inventoryJson = JsonConvert.SerializeObject(inventoryDict, Formatting.None);

        Dictionary<string, object> dict = new Dictionary<string, object>()
        {
            ["level_id"] = level,
            ["percentage_solve"] = percentageSolve,
            ["inventory"] = inventoryJson
        };

        GameAnalyticEvent.Event("INVENTORY", dict);
    }

    public void TrackingRevive(ReviveType_Tracking reviveType_Tracking)
    {
        GameAnalyticController.Instance.Tracking().Tracking_REVIVE(reviveType_Tracking);
    }

    public void TrackingShop(int level)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>()
        {
            ["level_id"] = level,
            ["cart_type"] = $"{IngameData.SHOP_PLACEMENT}"
        };

        GameAnalyticEvent.Event("SHOP", dict);
    }

    public void TrackingGamePlay(LEVEL_STATUS levelStatus)
    {
        StopSessionGamePlay();
        var user = Db.storage.USER_INFO;
        user.totalSession++;

        int _level = user.level;
        if (levelStatus == LEVEL_STATUS.WIN)
        {
            TrackingCompleteProgress();
            TrackingGASingularFinishLevel(_playTime);
            GameAnalyticController.Instance.Tracking().TrackingCompleteLevelProgression(_level);

            GameAnalyticController.Instance.Tracking().Tracking_LEVEL_END(_level, _playTime, LEVEL_STATUS.WIN);

        }

        if (levelStatus == LEVEL_STATUS.FAILED)
        {
            GameAnalyticController.Instance.Tracking().Tracking_LEVEL_END(_level, _playTime, LEVEL_STATUS.FAILED);
            GameAnalyticController.Instance.Tracking().TrackingFailLevelProgression(_level);
        } else

        if (levelStatus == LEVEL_STATUS.out_of_slot)
        {
            GameAnalyticController.Instance.Tracking().Tracking_LEVEL_END(_level, _playTime, LEVEL_STATUS.FAILED);
            GameAnalyticController.Instance.Tracking().TrackingFailLevelProgression(_level);
        }

        FirebaseAnalyticController.TrackingLevelEnd($"{_level}", "", "");
        Db.storage.USER_INFO = user;
        Debug.Log($"Tracking: LEVEL_LENGTH ");
        ResetData();
    }

    void ResetData()
    {
        IngameData.GAME_PLAY_TIME = 0;
        IngameData.TRACKING_UNLOCK_BOX_COUNT = 0;
        IngameData.REVIVE_REWARD_PER_LEVEL_COUNT = 0;
    }


    public void TrackingRetry()
    {
        Debug.Log($"AmplitudeController.Instance null: {GameAnalyticController.Instance == null}");

        GameAnalyticController.Instance.Tracking().Tracking_RETRY(_playTime);
    }


    private void TrackingGASingularFinishLevel(int levelLength)
    {
        var _level = Db.storage.USER_INFO.level;
        switch (_level)
        {
            case 1:
            case 3:
            case 5:
            case 10:
            case 15:
            case 20:
            case 25:
            case 30:
            case 40:
            case 50:
            case 60:
            case 70:
            case 80:
            case 90:
            case 100:
            case 120:
            case 140:
            case 150:
            case 175:
            case 200:
            case 250:
            case 300:
            case 350:
            case 400:
            case 450:
            case 500:
                SingularSDK.Event($"FINISH_LEVEL_{_level}");
                //GameAnalyticEvent.Event($"FINISH_LEVEL_{_level}");

                Debug.Log($"Tracking: FINISH_LEVEL_{_level}");
                break;
        }
        
        var dict = new Dictionary<string, object>()
        {
            ["is_revenue_event"] = false,
            ["level"] = _level.GetDecrypted(),
            ["level_length"] = levelLength
        };

        SingularSDK.Event(dict, $"SOF_LEVEL_FINISH");
        Debug.Log($"Tracking: SOF_LEVEL_FINISH with level: {_level.GetDecrypted()} - length: {levelLength}");
        TrackingFirebaseFinishLevel(_level);
    }

    private void TrackingFirebaseFinishLevel(long level)
    {
        // switch (level)
        // {
        //     case 1:
        //     case 2:
        //     case 3:
        //     case 4:
        //     case 5:
        //     case 10:
        //     case 20:
        //         FirebaseAnalytics.LogEvent($"FINISH_LEVEL_{level}");
        //         break;
        // }
    }

    private void TrackingCompleteProgress()
    {
        var _level = Db.storage.USER_INFO.level;
        //FirebaseAnalyticController.TrackingLevelEnd($"{_level}", "", "true");
    }

    public void TrackingOfferwallClick(shop_placement shop_Placement, int level)
    {
        GameAnalyticController.Instance.Tracking().TrackingOfferwallClick(shop_Placement, level);
    }

    public void TrackingOfferwallRewardReceived(int level, int coinReward)
    {
        GameAnalyticController.Instance.Tracking().TrackingOfferwallRewardReceived(level, coinReward);
    }

    public void TrackingOpenShop(shop_placement shop_Placement)
    {
        // GameAnalyticController.Instance.Tracking().TrackingOpenShop(shop_Placement);
        TrackingShop(Db.storage.USER_INFO.level);
    }


    public void TrackingTutorial(TUTORIAL_TYPE type)
    {
        GameAnalyticController.Instance.Tracking().Tracking_TUTORIAL(type);
        SingularSDK.Event("TUTORIAL_COMPLETE");

    }

    public void TrackingIAPClicked(string productId, decimal amount, string currency)
    {
        GameAnalyticController.Instance.Tracking()
            .Tracking_IAP_CLICK(productId, IngameData.SHOP_PLACEMENT, $"{amount}", currency);
        // FirebaseAnalyticController.TrackingIAP(productId, $"{amount}", currency);
        SingularSDK.Event("IAP_CLICKED");
    }

    public void Tracking_IAP(string productId, decimal amount, string currency, ItemIAPType itemIAPType)
    {
        GameAnalyticController.Instance.Tracking()
            .Tracking_IAP(productId, IngameData.SHOP_PLACEMENT, $"{amount}", currency, itemIAPType);
        SingularSDK.Event($"{productId.ToUpper()}");
    }

    public void TrackingUserLevelCompleted()
    {
        GameAnalyticController.Instance.Tracking().Tracking_USER_LEVEL_COMPLETED();
    }

    public void TrackingProfile()
    {
        GameAnalyticController.Instance.Tracking().Tracking_USER_PROFILE();
    }

    public void TrackingEditProfile(EditProfileType editProfileType, string value)
    {
        GameAnalyticController.Instance.Tracking().Tracking_USER_EDIT_PROFILE(editProfileType, value);
    }

    public void TrackingSpin(ResourceIAP.ResourceType resourceType, int amount)
    {
        GameAnalyticController.Instance.Tracking().Tracking_LUCKY_SPIN(resourceType, amount);
    }

    public void Tracking_BUSINESS(string pack_name, decimal price, string currency, ItemIAPType itemIAPType,
        string receipt, string signature)
    {
        GameAnalyticController.Instance.Tracking().Tracking_BUSINESS(pack_name, IngameData.SHOP_PLACEMENT, (int)price,
            currency, itemIAPType, receipt, signature);
    }
}

public enum RewardAdsPos
{
    NONE = 0,
    unlock_box = 1,
    revive = 2,
    booster = 3,
    shop = 4,
    win = 5,
    lucky_spin = 6,
}

public enum LEVEL_STATUS
{
    [SkipRename] WIN = 0,
    [SkipRename] FAILED = 1,
    [SkipRename] RESTART = 2,
    [SkipRename] out_of_move = 3,
    [SkipRename] out_of_slot = 4,
}