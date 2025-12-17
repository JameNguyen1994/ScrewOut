using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes.Converters;
using CodeStage.AntiCheat.Storage;
using GameAnalyticsSDK;
using Newtonsoft.Json;
using PS.Analytic.RemoteConfig;
using Storage;
using UnityEngine;
using UserProfile;
using static UniWebViewLogger;

public class EconomicTrackingAndroid : IEconomicTracking
{
    private readonly HashSet<int> levelReachMap = new HashSet<int>()
    {
        2, 3, 5, 8, 10, 13, 15, 18, 20, 23, 25, 30, 35, 40, 45, 50, 55, 60
    };

    private readonly HashSet<int> levelFinishMap = new HashSet<int>()
    {
        1, 3, 5, 10, 15, 20, 25, 30, 40, 50, 60, 70, 80,
        90, 100, 120, 140, 150, 175, 200, 250, 300, 350, 400, 450, 500
    };

    public void SendLevelFinish(int level)
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        if (!levelFinishMap.Contains(level))
        {
            return;
        }

        var dict = GetInfo();

        SingularSDK.Event(new Dictionary<string, object>()
        {
            ["user_data"] = dict
        }, $"FINISH_LEVEL_{level}");

        dict["en"] = "TkpBVlBPQ1pJ";
        dict["in_game_level"] = level;
        dict["is_finish_event"] = 1;
        SingularSDK.EventS2S($"FINISH_LEVEL_{level}", new Dictionary<string, object>()
        {
            ["user_data"] = dict
        });
#endif
    }

    public void SendReachLevel()
    {
        var level = Db.storage.USER_EXP.level.GetDecrypted();

#if UNITY_ANDROID && !UNITY_EDITOR
        var dict = GetInfo();

        SendExpLevelFinish(new Dictionary<string, object>(dict), level);

        if (!levelReachMap.Contains(level))
        {
            return;
        }

        string sterValue = GameAnalytics.GetRemoteConfigsValueAsString("ster", "true");
        var ster = GameAnalyticRemoteConfig.CastVale<bool>(sterValue);

        if (ster)
        {
            dict["en"] = "TkpBVlBPQ1pJ";
            SingularSDK.EventS2S($"EXP_LEVEL_{level}", new Dictionary<string, object>()
            {
                ["user_data"] = dict
            });
        }
        else
        {
            SingularSDK.Event(new Dictionary<string, object>()
            {
                ["user_data"] = dict
            }, $"SOF_EXP_{level}");
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    Dictionary<string, object> GetInfo()
    {
        var level = Db.storage.USER_EXP.level;

        var trkData = Db.storage.TRK_DATA;
        var json = ObscuredPrefs.Get(DbKey.TRK_MONET_DATA, "{}");
        var adData =
            JsonConvert.DeserializeObject<TrackingAdData>(json, new ObscuredTypesNewtonsoftConverter());

        string deviceInfoJson = SingularSDK.GetDeviceInfo();

        Dictionary<string, object> deviceInfo =
            JsonConvert.DeserializeObject<Dictionary<string, object>>(deviceInfoJson);

        deviceInfo.TryAdd("installSource", "null");
        deviceInfo.TryAdd("country", "null");

        var dict = new Dictionary<string, object>()
        {
            ["line_x1"] = trkData.numX1OfLine,
            ["line_x2"] = trkData.numX2OfLine,
            ["line_x3"] = trkData.numX3OfLine,
            ["line_x4"] = trkData.numX4OfLine,
            ["line_x5"] = trkData.numX5OfLine,
            ["line_x6"] = trkData.numX6OfLine,
            ["block_count"] = trkData.numOfBlock,
            ["mul_block_count"] = trkData.mulScoreBlockCount,
            ["mul_exp"] = trkData.mulScoreExp,
            ["single_block_count"] = trkData.singleScoreBlockCount,
            ["single_exp"] = trkData.singleScoreExp,
            ["drop_block_count"] = trkData.numDropBlock,
            ["exp_lucky"] = trkData.expOfLuckyWheel,
            ["length"] = Db.storage.LVL_LENGTH_EXP,
            ["section_exp"] = trkData.expOfSection,
            ["total_exp"] = Db.storage.USER_EXP.totalExp.GetDecrypted(),
            ["level"] = level.GetDecrypted(),
            ["banner_rev"] = adData.bannerRev,
            ["inter_rev"] = adData.interRev,
            ["reward_rev"] = adData.rewardRev,
            ["open_ad_rev"] = adData.openRev,
            ["mrec_rev"] = adData.mrecRev,
            ["banner_count"] = adData.bannerCount,
            ["inter_count"] = adData.interCount,
            ["reward_count"] = adData.rewardCount,
            ["open_ad_count"] = adData.openCount,
            ["mrec_count"] = adData.mrecCount,
            ["install_source"] = deviceInfo["installSource"],
            ["api_level"] = deviceInfo["apiLevel"],
            ["first_install_time"] = deviceInfo["firstInstallTime"],
            ["last_update_time"] = deviceInfo["lastUpdateTime"],
            ["country"] = deviceInfo["country"],
            ["is_finish_event"] = 0
            //["app_version"] = deviceInfo["appVersion"],
            //["app_version_accept"] = "1.1.0"
        };
        return dict;
    }
#endif

    void SendExpLevelFinish(Dictionary<string, object> dict, int level)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var levelMinus = level - 1;
        dict["level"] = levelMinus;
        dict["is_finish_event"] = 1;

        dict["en"] = "TkpBVlBPQ1pJ";
        SingularSDK.EventS2S("EXP_LEVEL_FINISH", new Dictionary<string, object>()
        {
            ["user_data"] = dict
        });
#endif
    }
}
