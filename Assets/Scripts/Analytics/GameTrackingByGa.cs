using Beebyte.Obfuscator;
using GameAnalyticsSDK;
using PS.Analytic.Event;
using PS.Utils;
using Storage;
using Storage.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Purchasing;

[Skip]
public class GameTrackingByGa : MonoBehaviour
{

    private float sessionStartTime = 0f;
    private void Start()
    {
        sessionStartTime = Time.time;
    }
    /// <summary>
    /// Những event default game nào cũng cần tracking thì để ở đây
    /// </summary>
    /// <param name="level"></param>
    #region DEFAULT_EVENT
    public void Tracking_BUSINESS(string pack_name, shop_placement shop_Placement, int price, string currency, ItemIAPType itemIAPType, string receipt, string signature)
    {
        {
            Dictionary<string, object> eventProps = new Dictionary<string, object>();
            eventProps.Add("receipt", receipt);
            eventProps.Add("signature", signature);


            GameAnalytics.NewBusinessEvent(currency,price,itemIAPType.ToString(),pack_name, shop_Placement.ToString(), eventProps);
        }
    }
    public void TrackingStartLevelProgression(int level)
    {
        GameAnalyticEvent.ProgressionEvent(GAProgressionStatus.Start, $"Level{level}");
    }

    public void TrackingFailLevelProgression(int level)
    {
        GameAnalyticEvent.ProgressionEvent(GAProgressionStatus.Fail, $"Level{level}");
    }

    public void TrackingCompleteLevelProgression(int level)
    {
        GameAnalyticEvent.ProgressionEvent(GAProgressionStatus.Complete, $"Level{level}");
    }
    // public void SendAdRevenueToAmplitude(string adUnitId, double revenue, string networkName, string precision, string placementName,
    //     string currency, string adType, string platform, string adAction)
    // {
    //     GameAnalytics.NewAdEvent();
    //     return;
    //     AmplitudeAdData adData = new AmplitudeAdData
    //     {
    //         adUnitId = adUnitId,
    //         revenue = revenue,
    //         networkName = networkName,
    //         precision = precision,
    //         placementName = placementName,
    //         currency = currency,
    //         adType = adType,
    //         platform = platform,
    //         adAction = adAction
    //     };
    //     TrackingAdRevenue(adData);
    //     var user = Db.storage.USER_INFO;
    //     user.total_ad_revenue += revenue;
    //     Db.storage.USER_INFO = user;
    // }
    // public void TrackingAdRevenue(AmplitudeAdData data)
    // {
    //     return;
    //     Dictionary<string, object> eventProps = new Dictionary<string, object>();
    //     eventProps.Add("ad_unit_id", data.adUnitId);
    //     eventProps.Add("$ad_revenue", $"{data.revenue}");
    //     eventProps.Add("ad_network_name", data.networkName);
    //     eventProps.Add("ad_precision", data.precision);
    //     eventProps.Add("ad_placement", data.placementName);
    //     eventProps.Add("ad_currency", data.currency);
    //     eventProps.Add("ad_type", data.adType);
    //     eventProps.Add("ad_platform", data.platform);
    //     amplitude.logEvent(data.adAction, eventProps);
    //
    //     var userProps = new Dictionary<string, object>()
    //     {
    //         ["total_revenue"] = Db.storage.USER_INFO.total_ad_revenue,
    //     };
    //
    //     amplitude.setUserProperties(userProps);
    //
    //
    // }

    //public void TrackingIAPRevenue(ProductID productID)
    //{
    //    var price = InAppPurchase.Instance.GetProductPrice($"{productID}");
    //    var currentcy = InAppPurchase.Instance.GetProductCurrencyCode($"{productID}");
    //    amplitude.logRevenue($"{productID}", 1, price);
    //    FirebaseAnalyticController.Tracking_IAP_CLICK($"{productID}", $"{price}", currentcy);
    //}


    #region DESIGN_EVENT
    public void Tracking_TUTORIAL(TUTORIAL_TYPE tutorialType)
    {
        int level = Db.storage.USER_INFO.level;
        Dictionary<string, object> eventProps = new Dictionary<string, object>();
        eventProps.Add("level_id", level);
        eventProps.Add("tutorial_type", tutorialType.ToString());
        GameAnalyticEvent.Event("TUTORIAL", eventProps);
    }
    public void Tracking_LEVEL_START(long level)
    {
        Dictionary<string, object> eventProps = new Dictionary<string, object>();
        eventProps.Add("level_id", level);
        eventProps.Add("mode_name", "");
        GameAnalyticEvent.Event("LEVEL_START", eventProps);
    }
    public void Tracking_LEVEL_END(long level, float playtime, LEVEL_STATUS status)
    {
        Dictionary<string, object> eventProps = new Dictionary<string, object>();
        eventProps.Add("level_id", level);
        eventProps.Add("level_duration", playtime);
        eventProps.Add("status", status);
Debug.Log($"LEVEL_END {playtime}: {status}");
        GameAnalyticEvent.Event("LEVEL_END", eventProps);
    }
    public void Tracking_RETRY(long level)
    {
        level = Db.storage.USER_INFO.level;
        var percentage = (float)IngameData.TRACKING_UN_SCREW_COUNT / (float)LevelController.Instance.Level.LstScrew.Count;
        Dictionary<string, object> eventProps = new Dictionary<string, object>();
        eventProps.Add("level_id", level);
        eventProps.Add("mode_name", "");

        GameAnalyticEvent.Event("RETRY", eventProps);
    }
    public void Tracking_BOOSTERS(BoosterType boosterType)
    {
        var level = Db.storage.USER_INFO.level;
        float percentage = GetPercentageSolved();
        Dictionary<string, object> eventProps = new Dictionary<string, object>();

        string boosterName = "";
        switch (boosterType)
        {
            case BoosterType.AddHole:
                boosterName = "Add Hole";
                break;
            case BoosterType.Clears:
                boosterName = "Clear";
                break;
            case BoosterType.Hammer:
                boosterName = "Hammer";
                break;
            case BoosterType.Magnet:
                boosterName = "Magnet";
                break;
            case BoosterType.UnlockBox:
                boosterName = "Unlock Box";
                break;
        }
        eventProps.Add("flow_type", boosterName);
        eventProps.Add("booster_type", boosterName);
        eventProps.Add("level_id", level);
        eventProps.Add("percentage_solved", percentage);

        GameAnalyticEvent.Event("BOOSTERS", eventProps);
    }
    public void Tracking_POWERUP(PreBoosterType preBoosterType, PreBoosterPlace preBoosterPlace)
    {
        var level = Db.storage.USER_INFO.level;
        float percentage = GetPercentageSolved();
        Dictionary<string, object> eventProps = new Dictionary<string, object>();

        string boosterName = "";
        switch (preBoosterType)
        {
            case PreBoosterType.Rocket:
                boosterName = "Rocket";
                break;
            case PreBoosterType.Glass:
                boosterName = "Glass";
                break;
        }
        var place = "";
        switch (preBoosterPlace)
        {
            case PreBoosterPlace.Home:
                place = "home";
                break;
            case PreBoosterPlace.Popup_end:
                place = "popup_fail";
                break;
        }
        eventProps.Add("flow_type", place);
        eventProps.Add("booster_type", boosterName);
        eventProps.Add("level_id", level);
        eventProps.Add("percentage_solved", percentage);

        GameAnalyticEvent.Event("POWERUP", eventProps);
    }
    public void Tracking_REVIVE(ReviveType_Tracking reviveType)
    {
        var level = Db.storage.USER_INFO.level;
        float percentage = GetPercentageSolved();
        Dictionary<string, object> eventProps = new Dictionary<string, object>();

        string revivetype = "";
        switch (reviveType)
        {
            case ReviveType_Tracking.ads:
                revivetype = "Ads";
                break;
            case ReviveType_Tracking.coin:
                revivetype = "Coin";
                break;
        }
        eventProps.Add("level_id", level);
        eventProps.Add("type", revivetype);
        eventProps.Add("percentage_solved", percentage);

        GameAnalyticEvent.Event("REVIVE", eventProps);
    }
    public void Tracking_SHOP(ReviveType_Tracking reviveType)
    {
        var level = Db.storage.USER_INFO.level;
        Dictionary<string, object> eventProps = new Dictionary<string, object>();
        eventProps.Add("cart_type", "");
        eventProps.Add("level_id", level);
        GameAnalyticEvent.Event("SHOP", eventProps);
    }

    public void Tracking_IAP_CLICK(string pack_name, shop_placement shop_Placement, string price, string currency)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>()
        {
            ["cart_type"] = IngameData.SHOP_PLACEMENT,
            ["item_name"] = pack_name,
            ["amount"] = price,
            ["currency"] = currency,
        };

        GameAnalyticEvent.Event("IAP_CLICKED", dict);
    }
    public void Tracking_IAP(string pack_name, shop_placement shop_Placement, string price, string currency, ItemIAPType itemIAPType)
    {
        Dictionary<string, object> eventProps = new Dictionary<string, object>();
        eventProps.Add("currency", currency);
        eventProps.Add("amount", price);
        eventProps.Add("item_type", itemIAPType);
        eventProps.Add("item_name", pack_name);
        eventProps.Add("cart_type", shop_Placement);

        GameAnalyticEvent.Event("IAP", eventProps);
    }
    public void Tracking_WATCH_AD_SPEND_TIME()
    {
    }
    public void Tracking_USER_LEVEL_COMPLETED()
    {
        var level = Db.storage.USER_INFO.level;
        var expLevel = Db.storage.USER_EXP.level;
        Dictionary<string, object> dict = new Dictionary<string, object>()
        {
            ["user_level_id"] = expLevel,
            ["level_id"] = level
        };

        GameAnalyticEvent.Event("USER_LEVEL_COMPLETED", dict);
    }
    public void Tracking_USER_PROFILE()
    {
        var expLevel = Db.storage.USER_EXP.level;
        Dictionary<string, object> dict = new Dictionary<string, object>()
        {
            ["user_level_id"] = expLevel,
        };

        GameAnalyticEvent.Event("USER_PROFILE", dict);
    }

    public void Tracking_USER_EDIT_PROFILE(EditProfileType editProfileType, string value)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>()
        {
            ["edit_type"] = editProfileType.ToString(),
            ["edit_value"] = value
        };
        GameAnalyticEvent.Event("USER_EDIT_PROFILE", dict);
    }
    public void Tracking_LUCKY_SPIN(ResourceIAP.ResourceType resourceType, int amount)
    {
        var user = Db.storage.USER_INFO;
        user.spinCount += 1;
        Db.storage.USER_INFO = user;
        Dictionary<string, object> dict = new Dictionary<string, object>()
        {
            ["spin_count"] = user.spinCount.ToString(),
            ["type"] = resourceType.ToString(),
            ["amout"] = amount
        };
        GameAnalyticEvent.Event("LUCKY_SPIN_REWARD", dict);
    }
    #endregion




    #endregion


    public void TrackingAdClicked(string placement, string adType, string adUnitId)
    {
        // var dict = new Dictionary<string, object>()
        // {
        //     ["ad_placement"] = placement,
        //     ["ad_unit_id"] = adUnitId,
        //     ["ad_platform"] = "AppLovin Max",
        //     ["ad_type"] = adType,
        // };
        // GameAnalyticEvent.Event("Ad Clicked", dict);
    }


    public void TrackingStartRewardAds(bool status, string adNetwork, string adId, string placement)
    {
        // var dict = new Dictionary<string, object>();
        // dict.Add("ad_status", $"{status}");
        // dict.Add("ad_unit_id", $"{adId}");
        // dict.Add("ad_placement", placement);
        // GameAnalyticEvent.Event("Ad Reward Started", dict);

    }

    public void TrackingRewardAdComplete(RewardAdsPos rewardAdsPos)
    {
        var dict = new Dictionary<string, object>();
        dict.Add("rw_position", $"{rewardAdsPos}");
        dict.Add("ad_placement", $"{rewardAdsPos}");
        GameAnalyticEvent.Event("reward ad completed", dict);
    }
    public void TrackingRewardAdFailed(RewardAdsPos rewardAdsPos)
    {
        // var dict = new Dictionary<string, object>();
        // dict.Add("rw_position", $"{rewardAdsPos}");
        // GameAnalyticEvent.Event("reward ad failed", dict);
    }

    public void TrackingLoadAdFailed(string id, string adType)
    {
        // var dict = new Dictionary<string, object>();
        // dict.Add("ad_unit_id", $"{id}");
        // dict.Add("ad_type", $"{adType}");
        // //dict.Add("ad_network_name", $"{network}");
        // dict.Add("ad_platform", $"Applovin Max");
        // //dict.Add("ad_placement", $"{placement}");
        // GameAnalyticEvent.Event("Ad Load Failed", dict);
    }


    public void TrackingOfferwallClick(shop_placement shop_Placement, int level)
    {
        Dictionary<string, object> eventProps = new Dictionary<string, object>()
        {
            ["placement"] = $"{shop_Placement}",
            ["level_id"] = $"{level}"
        };

        GameAnalyticEvent.Event("OFFERWALL_CLICKED", eventProps);
    }

    public void TrackingOfferwallRewardReceived(int level, int coinReward)
    {
        Dictionary<string, object> eventProps = new Dictionary<string, object>()
        {
            ["level_id"] = $"{level}",
            ["coin_amount"] = $"{coinReward}"
        };

        GameAnalyticEvent.Event("OFFERWALL_REWARD_RECEIVED", eventProps);
    }

    private float GetPercentageSolved()
    {
        return (int)(ScreenGamePlayUI.Instance.LevelProgressBar.GetProgress() * 100);
    }

}
public enum ReviveType_Tracking
{
    coin = 0,
    ads = 1
}
public enum SettingType
{
    exit = 0,
    sound = 1,
    music = 2
}
public enum shop_placement
{
    Home = 0,
    BoosterUnlockBox = 1,
    BoosterAddHole = 2,
    BoosterHammer = 3,
    BoosterClear = 4,
    booster_magnet = 5,
    Revive = 6,
    Heart = 7,
    IconCoin = 8,
    IconStarterPack = 9,
    IconNoAds = 10,
    PopupFirstAds =11,
    PopupBuyRocket =12,
    PopupBuyGlass = 13
}
public enum TUTORIAL_TYPE
{
    OnBoarding = 0,
    Booster = 1
}
public enum ItemIAPType
{
    Coin = 0,
    NoAds = 1,
    Bundle = 2
}
public enum EditProfileType
{
    Name = 0,
    Avatar = 1,
    Border = 2
}
public class AmplitudeAdData
{
    public string adUnitId;
    public double revenue;
    public string networkName;
    public string precision;
    public string placementName;
    public string currency;
    public string adType;
    public string platform; //Applovin, Admob 
    public string adAction;
}