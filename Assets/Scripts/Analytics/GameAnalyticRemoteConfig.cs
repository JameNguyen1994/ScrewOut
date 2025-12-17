using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using GameAnalyticsSDK;
using Newtonsoft.Json.Linq;
using PS.Ad;
using UnityEngine;

namespace PS.Analytic.RemoteConfig
{
    public class GameAnalyticRemoteConfig
    {
        public bool IsReadyRemote
        {
            get;
            private set;
        } = false;

        public int InterEnd
        {
            get;
            private set;
        } = 1;

        public bool FocusAd
        {
            get;
            private set;
        } = true;


        public int NumToShowReview
        {
            get;
            private set;
        } = 8;
        public int NumToLoopShowReview
        {
            get;
            private set;
        } = 5;
        public int NumToLoopShowAdsPopup
        {
            get;
            private set;
        } = 5;
        public bool IsAdOn
        {
            get;
            private set;
        } = true;

        public int InterPause
        {
            get;
            private set;
        } = 3;

        public int InterHome
        {
            get;
            private set;
        } = 3;

        public int CbsNoAd
        {
            get;
            private set;
        } = 1;

        public int ModeGamePlayControl // 1: game play mode cũ, 2: game play mode mới
        {
            get;
            private set;
        } = 2;

        public int InterCd
        {
            get;
            private set;
        } = 0;

        public int RwSkipInter
        {
            get;
            private set;
        } = 0;

        public bool IsShowStartLevelPopup
        {
            get;
            private set;
        } = true;

        public int LifeFull
        {
            get;
            private set;
        } = 5;

        public int LevelBackHome
        {
            get;
            private set;
        } = 5;

        public bool IsNewUI
        {
            get;
            private set;
        } = true;

        public int LevelPack
        {
            get;
            private set;
        } = 2;

        public int ExpCompleteBox
        {
            get;
            private set;
        } = 4;

        public bool OfferwallOn
        {
            get;
            private set;
        } = true;

        public string DefaultEventFir
        {
            get;
            private set;
        } = "{\"LV5\":1,\"LV8\":1,\"LV10\":1,\"LV13\":1,\"LV15\":1,\"LV18\":1,\"LV20\":1,\"LV23\":1,\"LV25\":1,\"LV30\":1,\"LV35\":1,\"LV40\":1,\"LV45\":1,\"LV50\":1,\"LV55\":1,\"LV60\":1}";

        public int LevelStartFocus
        {
            get;
            private set;
        } = -1;

        public SegmentFlow SegmentFlow
        {
            get;
            private set;
        } = new SegmentFlow()
        {
            interLvl = 5,
            bannerLvl = 1,
            breakLvl = -1,
            breakCount = 60,
            boosterEnable = true,
            noads = 1,
            noadsWithCombo = 0,
            beginAds = 0,
            enableExpBar = false,
        };

        public BeginAds BeginAds
        {
            get;
            private set;
        } = new BeginAds()
        {
            beginAds = 0
        };

        public NoAdsWithCombo NoAdsWithCombo
        {
            get;
            private set;
        } = new NoAdsWithCombo()
        {
            noAdsWithCombo = 0
        };
        public ShopRemoteConfig RewardFreeCoin
        {
            get;
            private set;
        } = new ShopRemoteConfig()
        {
            CoinTime = 30,
            CoinAmount = 100
        };
        string iapShopConfig = "{\"rw_coin_time\":  300,\"coin_amount\":50}"; // ct: reward coin time, ca: coin amount

        public RewardControlConfig RewardControl
        {
            get;
            private set;
        } = new RewardControlConfig
        {
            rewardBoosterAmountPerDay = 3,
            rewardReviveAmountPerLevel = 1,
            rewardLifeAmountPerDay = 100,
            isEnableRewardButtonXCoinWinPopup = true,
            rewardShopCoinAmountPerDay = 100
        };
        string rewardControlJson = "{\"booster\": 3,\"revive\":1, \"life\": 100,\"xcoin\":true,\"shopcoin\":100}";

        private string levelStartAdsJson = "{\"banner\": 100,\"inter_level_fst\": 9," +
                                         "\"inter_win_fst\": 2,\"inter_level_snd\": 10," +
                                         "\"inter_win_snd\": 1,\"inter_lose_time\": 30," +
                                         "\"break_percent\": -1,\"lose_focus\": -1,\"inter_rw\": 5}";
        private LevelStartAd levelStartAds = new LevelStartAd
        {
            interFirstStart = 9,
            interFirstFrequency = 2,
            interSecondStart = 15,
            interSecondFrequency = 1,
            interMinTime = 30,
            bannerLvl = 5,
            focusLvl = -1,
            breakPercent = -1,
            inter_rw = 5
        };

        public LevelStartAd LevelStartAd
        {
            get
            {
                return levelStartAds;
            }
        }


        private string costInGame = "{\"rv\": 100,\"li\": 100," +
                                         "\"ah\": 2,\"hm\": 10," +
                                         "\"rk\": 2,\"gl\": 10," + "\"cl\": 1,\"ub\": 30}";
        private CostInGame costIngame = new CostInGame
        {
            coinRevive = 200,
            coinLife = 500,
            coinBoosterAddHold = 100,
            coinBoosterHammer = 101,
            coinBoosterClear = 102,
            coinBoosterUnlockBox = 103,
            coinBoosterRocket = 104,
            coinBoosterGlass = 105,
        };

        public CostInGame CostInGame
        {
            get
            {
                return costIngame;
            }
        }

        public string OfferWallRemote
        {
            get;
            private set;
        } = "{\"enable\":true,\"mc_enable\":true,\"bl_enable\":true}";

        private int revive_free;
        public int Revive_Free
        {
            get;

            private set;
        } = 5;

        public int Requried_Screw
        {
            get;
            private set;
        } = 75;
        public int Daily_Spin
        {
            get;
            private set;
        } = 2;
        public int SORepeat
        {
            get;
            private set;
        } = 10;
        public int SPRepeat
        {
            get;
            private set;
        } = 4;
        public int BonusTime
        {
            get;
            private set;
        } = 5;
        public GameAnalyticRemoteConfig()
        {
            IngameData.MODE_CONTROL = ModeControl.ControlV2;
            GameAnalytics.OnRemoteConfigsUpdatedEvent += MyOnRemoteConfigsUpdateFunction;

#if UNITY_EDITOR
            IsReadyRemote = true;
            GameConfig.OLD_VERSION = !IsNewUI;
#endif
        }

        private void MyOnRemoteConfigsUpdateFunction()
        {
            string sessionShowInter = GameAnalytics.GetRemoteConfigsValueAsString("inter_end", "1");
            string focus = GameAnalytics.GetRemoteConfigsValueAsString("focus_ad", "false");
            string adOn = GameAnalytics.GetRemoteConfigsValueAsString("turn_on_ads", "false");
            string showReview = GameAnalytics.GetRemoteConfigsValueAsString("show_review", "8");
            string showReviewLoop = GameAnalytics.GetRemoteConfigsValueAsString("show_review_loop", "5");
            string showAdsPopupLoop = GameAnalytics.GetRemoteConfigsValueAsString("show_p_ads", "0");
            string interPause = GameAnalytics.GetRemoteConfigsValueAsString("inter_pause", "3");
            string interHomeStr = GameAnalytics.GetRemoteConfigsValueAsString("inter_home", "3");
            string interCdStr = GameAnalytics.GetRemoteConfigsValueAsString("inter_cd", "0");
            string rwSkipInterStr = GameAnalytics.GetRemoteConfigsValueAsString("rw_skipinter", "0");
            string lifeFullStr = GameAnalytics.GetRemoteConfigsValueAsString("life_full", "5");
            string levelBackHomeStr = GameAnalytics.GetRemoteConfigsValueAsString("lvl_back_home", "5");
            string levelPackStr = GameAnalytics.GetRemoteConfigsValueAsString("level_pack", "2");
            string expCompletedBoxStr = GameAnalytics.GetRemoteConfigsValueAsString("exp_box", "4");
            string cbsNoAdsStr = GameAnalytics.GetRemoteConfigsValueAsString("cbs_no_ads", "1");
            string levelStartFocusAdStr = GameAnalytics.GetRemoteConfigsValueAsString("lvl_start_focus", "1");
            string isNewUIStr = GameAnalytics.GetRemoteConfigsValueAsString("new_ui", "true");
            string segmentFlowStr = GameAnalytics.GetRemoteConfigsValueAsString("segment_flow", "{\"break\":-1,\"inter\":5,\"banner\":1,\"no_ads\":1,\"booster\":true,\"exp_enable\":false,\"screw_break\":60}");


            string levelStartAds = GameAnalytics.GetRemoteConfigsValueAsString("ad_flow", this.levelStartAdsJson);

            DefaultEventFir = GameAnalytics.GetRemoteConfigsValueAsString("event_fir", DefaultEventFir);
            iapShopConfig = GameAnalytics.GetRemoteConfigsValueAsString("reward_freecoin", iapShopConfig);
            rewardControlJson = GameAnalytics.GetRemoteConfigsValueAsString("reward_control", iapShopConfig);
            string offerwallStr = GameAnalytics.GetRemoteConfigsValueAsString("offer_wall", "true");

            string costInGameStr = GameAnalytics.GetRemoteConfigsValueAsString("cost_in_game", this.costInGame);

            string offerWallRemote = GameAnalytics.GetRemoteConfigsValueAsString("offer_control", "{\"enable\":true,\"mc_enable\":true,\"bl_enable\":true}");
            string revive_free = GameAnalytics.GetRemoteConfigsValueAsString("revive_free", "5");
            string requried_screw = GameAnalytics.GetRemoteConfigsValueAsString("requried_screw", "75");
            string bonusTime = GameAnalytics.GetRemoteConfigsValueAsString("bonus_time", "65");
            string so_repeat = GameAnalytics.GetRemoteConfigsValueAsString("so_repeat", "10");
            string sp_repeat = GameAnalytics.GetRemoteConfigsValueAsString("sp_repeat", "4");
            string daily_spin = GameAnalytics.GetRemoteConfigsValueAsString("daily_spin", "2");

            try
            {
                InterEnd = CastVale<int>(sessionShowInter);
                FocusAd = CastVale<bool>(focus);
                IsAdOn = CastVale<bool>(adOn);
                NumToShowReview = CastVale<int>(showReview);
                NumToLoopShowReview = CastVale<int>(showReviewLoop);
                InterPause = CastVale<int>(interPause);
                InterHome = CastVale<int>(interHomeStr);
                InterCd = CastVale<int>(interCdStr);
                RwSkipInter = CastVale<int>(rwSkipInterStr);
                LifeFull = CastVale<int>(lifeFullStr);
                LevelBackHome = CastVale<int>(levelBackHomeStr);
                LevelPack = CastVale<int>(levelPackStr);
                ExpCompleteBox = CastVale<int>(expCompletedBoxStr);
                CbsNoAd = CastVale<int>(cbsNoAdsStr);
                IsNewUI = CastVale<bool>(isNewUIStr);
                LevelStartFocus = CastVale<int>(levelStartFocusAdStr);
                Revive_Free = CastVale<int>(revive_free);
                Requried_Screw = CastVale<int>(requried_screw);
                BonusTime = CastVale<int>(bonusTime);
                SORepeat = CastVale<int>(so_repeat);
                SPRepeat = CastVale<int>(sp_repeat);
                Daily_Spin = CastVale<int>(daily_spin);
                JObject segmentFlowJObject = JObject.Parse(segmentFlowStr);
                SegmentFlow = new SegmentFlow()
                {
                    interLvl = (segmentFlowJObject["inter"] ?? 5).Value<int>(),
                    bannerLvl = (segmentFlowJObject["banner"] ?? 1).Value<int>(),
                    breakLvl = (segmentFlowJObject["break"] ?? -1).Value<int>(),
                    breakCount = (segmentFlowJObject["screw_break"] ?? 60).Value<int>(),
                    boosterEnable = (segmentFlowJObject["booster"] ?? true).Value<bool>(),
                    noads = (segmentFlowJObject["no_ads"] ?? 1).Value<int>(),
                    noadsWithCombo = (segmentFlowJObject["no_ads_with_combo"] ?? 0).Value<int>(),
                    beginAds = (segmentFlowJObject["begin_ads"] ?? 0).Value<int>(),
                    enableExpBar = (segmentFlowJObject["exp_enable"] ?? false).Value<bool>()
                };
                NumToLoopShowAdsPopup = CastVale<int>(showAdsPopupLoop);
                this.OfferWallRemote = CastVale<string>(offerWallRemote);

            }
            catch (Exception e)
            {
                throw new UnityException($"Can not cast string to int \n Detail: {e}"); this.levelStartAdsJson = CastVale<string>(levelStartAds);
                OfferwallOn = CastVale<bool>(offerwallStr);
            }
            this.levelStartAdsJson = CastVale<string>(levelStartAds);
            OfferwallOn = CastVale<bool>(offerwallStr);
            JObject shopConfig = JObject.Parse(iapShopConfig);
            this.RewardFreeCoin = new ShopRemoteConfig()
            {
                CoinTime = (shopConfig[$"rw_coin_time"] ?? 300).Value<int>(),
                CoinAmount = (shopConfig[$"coin_amount"] ?? 50).Value<int>()
            };
            JObject rewardControlJO = JObject.Parse(this.rewardControlJson);
            this.RewardControl = new RewardControlConfig
            {
                rewardBoosterAmountPerDay = (rewardControlJO[$"booster"] ?? 3).Value<int>(),
                rewardReviveAmountPerLevel = (rewardControlJO[$"revive"] ?? 1).Value<int>(),
                rewardLifeAmountPerDay = (rewardControlJO[$"life"] ?? 100).Value<int>(),
                isEnableRewardButtonXCoinWinPopup = (rewardControlJO[$"xcoin"] ?? true).Value<bool>(),
                rewardShopCoinAmountPerDay = (rewardControlJO[$"shopcoin"] ?? 100).Value<int>()
            };
            JObject rewardAddFlow = JObject.Parse(this.levelStartAdsJson);
            this.levelStartAds = new LevelStartAd()
            {
                interFirstStart = (rewardAddFlow[$"inter_level_fst"] ?? 9).Value<int>(),
                interSecondStart = (rewardAddFlow[$"inter_level_snd"] ?? 10).Value<int>(),
                interFirstFrequency = (rewardAddFlow[$"inter_win_fst"] ?? 2).Value<int>(),
                interSecondFrequency = (rewardAddFlow[$"inter_win_snd"] ?? 1).Value<int>(),
                bannerLvl = (rewardAddFlow[$"banner"] ?? 5).Value<int>(),
                focusLvl = (rewardAddFlow[$"lose_focus"] ?? -1).Value<int>(),
                interMinTime = (rewardAddFlow[$"inter_lose_time"] ?? 120).Value<int>(),
                breakPercent = (rewardAddFlow[$"break_percent"] ?? -1).Value<int>(),
                inter_rw = (rewardAddFlow[$"inter_rw"] ?? 0).Value<int>(),
            };
            JObject costInGameJObject = JObject.Parse(costInGameStr);
            this.costIngame = new CostInGame()
            {
                coinRevive = (costInGameJObject["rv"] ?? 200).Value<int>(),
                coinLife = (costInGameJObject["li"] ?? 500).Value<int>(),
                coinBoosterAddHold = (costInGameJObject["ah"] ?? 100).Value<int>(),
                coinBoosterHammer = (costInGameJObject["hm"] ?? 101).Value<int>(),
                coinBoosterClear = (costInGameJObject["cl"] ?? 102).Value<int>(),
                coinBoosterUnlockBox = (costInGameJObject["ub"] ?? 103).Value<int>(),
                coinBoosterRocket = (costInGameJObject["rk"] ?? 104).Value<int>(),
                coinBoosterGlass = (costInGameJObject["gl"] ?? 105).Value<int>(),
            };

            Debug.Log($"[RemoteConfig] InterEnd: {InterEnd}, FocusAd: {FocusAd}, IsAdOn: {IsAdOn}, " +
                      $"ShowReview: {NumToShowReview}, InterPause: {InterPause}, InterHome: {InterHome}, InterCd: {InterCd}, " +
                      $"RwSkipInter: {RwSkipInter}, LifeFull: {LifeFull}, LevelBackHome: {LevelBackHome}, LevelPack: {LevelPack}, " +
                      $"ExpCompleteBox: {ExpCompleteBox}, CbsNoAd: {CbsNoAd}, IsNewUI: {IsNewUI}, LevelStartFocus: {LevelStartFocus}\n" +
                      $"SegmentFlow: {SegmentFlow}\n" +
                      $"LevelStartAds: {LevelStartAd}\n" +
                      $"RewardFreeCoin: {RewardFreeCoin}\n" +
                      $"RewardControl: {RewardControl}\n" +
                      $"CostInGame: {CostInGame}"
                      );


            GameConfig.OLD_VERSION = !IsNewUI;

            IsReadyRemote = true;
            // ApplovinMaxController.Instance.SetFocusAd(LevelStartAd.focusLvl>=0);

            // chý ý: khi cast từ string qua double hay float, giá trị sẽ có thay đổi nhẹ gần đúng với giá trị trong chuỗi chứ không chính xác hoàn toàn.
            // Cast value chỉ thực hiện với các dữ liệu cơ bản: int, long, double, float, bool ...
            // với bool thì string value phải là chữ true hoặc false. Tuy nhiên vẫn có thể nhập: True, tRue, ...
        }

        public static T CastVale<T>(string value) where T : IConvertible
        {
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        public List<UnitTestItem> UnitTest()
        {
            List<UnitTestItem> itemList = new List<UnitTestItem>();
            var list = GameAnalyticRemoteTest.GetListVariable(this);

            foreach (var item in list)
            {
                //Debug.Log($"{item.Name} : {GameAnalyticRemoteTest.GetValue(this, item)}");
                itemList.Add(new UnitTestItem()
                {
                    name = item.Name,
                    value = GameAnalyticRemoteTest.GetValue(this, item)
                });
            }

            return itemList;
        }
    }

    public class UnitTestItem
    {
        public string name;
        public string value;
    }
    [Serializable]
    public struct ShopRemoteConfig
    {
        public int CoinTime;
        public int CoinAmount;

        public override string ToString()
        {
            return $"ct: {CoinTime}, ca: {CoinAmount}";
        }
    }
    [Serializable]
    public struct RewardControlConfig
    {
        public int rewardBoosterAmountPerDay;
        public int rewardReviveAmountPerLevel;
        public int rewardLifeAmountPerDay;
        public bool isEnableRewardButtonXCoinWinPopup;
        public int rewardShopCoinAmountPerDay;

        public override string ToString()
        {
            return $"booster: {rewardBoosterAmountPerDay}, revive: {rewardReviveAmountPerLevel},\n" +
                   $"life: {rewardLifeAmountPerDay}, xcoin: {isEnableRewardButtonXCoinWinPopup}, shopcoin: {rewardShopCoinAmountPerDay}";
        }
    }
    [Serializable]
    public class LevelStartAd
    {
        public int interFirstStart;
        public int interFirstFrequency;
        public int interSecondStart;
        public int interSecondFrequency;
        public int interMinTime;
        public int bannerLvl;
        public int focusLvl;
        public int breakPercent;
        public int inter_rw;

        public override string ToString()
        {
            return $"first inter: {interFirstStart}, firstf: {interFirstFrequency}\n" +
                   $"sec inter: {interSecondStart}, secf: {interSecondFrequency}\n" +
                   $"inter time: {interMinTime} banner: {bannerLvl},\n" +
                   $"focus: {focusLvl}, bpercent: {breakPercent}" +
                   $"inter_rw: {inter_rw}";


        }
    }
    [Serializable]
    public class CostInGame
    {
        public int coinRevive;
        public int coinLife;
        public int coinBoosterAddHold;
        public int coinBoosterHammer;
        public int coinBoosterClear;
        public int coinBoosterUnlockBox;
        public int coinBoosterRocket;
        public int coinBoosterGlass;

        public override string ToString()
        {
            return $"coinRevive: {coinRevive}, coinLife: {coinLife}, " +
                   $"coinBoosterAddHold: {coinBoosterAddHold}, coinBoosterHammer: {coinBoosterHammer}, " +
                   $"coinBoosterClear: {coinBoosterClear}, coinBoosterUnlockBox: {coinBoosterUnlockBox}";
        }
    }
}