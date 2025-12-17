using Storage;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes.Converters;
using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;
using Storage.Model;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using GameAnalyticsSDK;
using PS.Analytic;
using PS.Analytic.RemoteConfig;

public class ExpTracking : MonoBehaviour
{
    [System.Reflection.Obfuscation(Exclude = false)]
    public void TrackingLevelUp()
    {
        var remote = GameAnalyticController.Instance.Remote();
        var level = Db.storage.USER_EXP.level;
        
        TrackingController.Instance.TrackingUserLevelCompleted();

#if UNITY_ANDROID
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
/*            ["android_id"] = SystemInfo.deviceUniqueIdentifier,
            ["install_source"] = deviceInfo["installSource"],
            ["api_level"] = deviceInfo["apiLevel"],
            ["first_install_time"] = deviceInfo["firstInstallTime"],
            ["last_update_time"] = deviceInfo["lastUpdateTime"],*/
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
            ["country"] = deviceInfo["country"],
            ["is_finish_event"] = 0
            //["app_version"] = deviceInfo["appVersion"],
            //["app_version_accept"] = "1.1.0"
        };
        
#endif


        switch (level)
        {
            case 2:
            case 3:
            case 5:
            case 8:
            case 10:
            case 13:
            case 15:
            case 18:
            case 20:
            case 23:
            case 25:
            case 30:
            case 35:
            case 40:
            case 45:
            case 50:
            case 55:
            case 60:
                ObscuredString defaultEventFir =
                    "{\"LV5\":1,\"LV8\":1,\"LV10\":1,\"LV13\":1,\"LV15\":1,\"LV18\":1,\"LV20\":1,\"LV23\":1,\"LV25\":1,\"LV30\":1,\"LV35\":1,\"LV40\":1,\"LV45\":1,\"LV50\":1,\"LV55\":1,\"LV60\":1}";
                ObscuredString eventFirStr = remote.DefaultEventFir;


                Dictionary<string, int> singularEventData = new Dictionary<string, int>();

                try
                {
                    singularEventData = JsonConvert.DeserializeObject<Dictionary<string, int>>(eventFirStr);
                }
                catch (UnityException e)
                {
                    print($"data convert error: {e}");

                    singularEventData = JsonConvert.DeserializeObject<Dictionary<string, int>>(defaultEventFir);
                }

                string key = $"LV{level}";

                if (!singularEventData.ContainsKey(key) || singularEventData[key] == 1)
                {
                    string sterValue = GameAnalytics.GetRemoteConfigsValueAsString("ster", "true");
                    var ster = GameAnalyticRemoteConfig.CastVale<bool>(sterValue);

                    if (ster)
                    {
                        #if UNITY_ANDROID
                        dict["en"] = "VlBPQ1pJTkpB";
                        SingularSDK.EventS2S($"SOF_EXP_{level}", new Dictionary<string, object>()
                        {
                            ["user_data"] = dict
                        });
                        #endif
                        
                        #if UNITY_IOS
                        SingularSDK.Event($"EXP_LEVEL_{level}");
                        #endif
                    }
                    else
                    {
                        #if UNITY_ANDROID
                        SingularSDK.Event(new Dictionary<string, object>()
                            {
                                ["user_data"] = dict
                            }, $"SOF_EXP_{level}");
                        #endif
                        
#if UNITY_IOS
                        SingularSDK.Event($"EXP_LEVEL_{level}");
#endif
                    }
                }
                break;
            default:
                break;
        }
        
#if UNITY_ANDROID
        var levelMinus = level - 1;
        dict["level"] = levelMinus;
        dict["is_finish_event"] = 1;

        dict["en"] = "VlBPQ1pJTkpB";
        SingularSDK.EventS2S("SOF_EXP_FINISH", new Dictionary<string, object>()
        {
            ["user_data"] = dict
        });
#endif
        
#if UNITY_IOS
        SingularSDK.Event("EXP_LEVEL_FINISH");
#endif
    }
}