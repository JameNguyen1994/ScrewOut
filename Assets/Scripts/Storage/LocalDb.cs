using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.ObscuredTypes.Converters;
using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;
using ps.modules.journey;
using Storage.Model;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Storage
{
    public partial class LocalDb
    {
        public LocalDb()
        {
            ObscuredPrefs.DeviceLockSettings.Level = DeviceLockLevel.Soft;
            ObscuredPrefs.DeviceLockSettings.Sensitivity = DeviceLockTamperingSensitivity.Low;

            ObscuredPrefs.NotGenuineDataDetected += () => { IS_CHEATER = true; };
            ObscuredPrefs.DataFromAnotherDeviceDetected += () => { IS_CHEATER = true; };

            if (!ObscuredPrefs.HasKey(DbKey.CHEAT_INT))
            {
                CHEAT_INT = 10;
            }

            if (!ObscuredPrefs.HasKey(DbKey.IS_CHEATER))
            {
                IS_CHEATER = false;
            }
            if (!ObscuredPrefs.HasKey(DbKey.IS_SOUND))
            {
                IS_SOUND = true;
            }

            if (!ObscuredPrefs.HasKey(DbKey.USER_INFO))
            {

                var userInfo = new UserInfo()
                {
                    coin = 0,
                    level = 1,
                    score = 0,
                    star = 0,
                    totalSession = 0,
                    maxLevel = 0,
                    total_ad_revenue = 0,
                    beginSaleAdsValidUntil = DateTime.MinValue,
                    beginSaleAdsNextAvailable = DateTime.MinValue,
                    countInterAds = 0
                };
                ObscuredPrefs.Set(DbKey.H_USER_PROFILE, userInfo.GetH());
                SetFromTData(DbKey.USER_INFO, userInfo);
            }
            if (!ObscuredPrefs.HasKey(DbKey.SESSION_COUNT))
            {
                SESSION_COUNT = 0;
            }

            if (!ObscuredPrefs.HasKey(DbKey.TUTORIAL))
            {
                IS_TUTORIAL = true;
            }
            if (!ObscuredPrefs.HasKey(DbKey.IS_SHOW_RATING))
            {
                IS_SHOW_RATING = true;
            }
            if (!ObscuredPrefs.HasKey(DbKey.REJECT_REVIEW_COUNT))
            {
                REJECT_REVIEW_COUNT = 0;
            }
            if (!ObscuredPrefs.HasKey(DbKey.BOOSTER_DATAS))
            {
                var bstData = new BoosterDatas(true);
                Debug.Log($"[DB] {bstData}");
                ObscuredPrefs.Set(DbKey.H_BOOSTER, bstData.GetH());
                SetFromTData(DbKey.BOOSTER_DATAS, bstData);
            }
            if (!PlayerPrefs.HasKey(DbKey.IS_COLOR_MODE))
            {
                IS_COLOR_MODE = true;
            }


            if (!ObscuredPrefs.HasKey(DbKey.SETTING_DATAS))
            {
                SETTING_DATAS = new SettingData(true);
            }

            if (!ObscuredPrefs.HasKey(DbKey.USER_EXP))
            {
                var userExp = new UserExp() { level = 1, exp = 0 };

                ObscuredPrefs.Set(DbKey.H_USER_EXP, userExp.GetH());

                SetFromTData(DbKey.USER_EXP, userExp);
            }
            if (!ObscuredPrefs.HasKey(DbKey.ADS_COIN_AMOUNT))
            {
                ADS_COIN_AMOUNT = -10;
            }
            if (!ObscuredPrefs.HasKey(DbKey.ADS_COIN_MARK))
            {
                ADS_COIN_MARK = 0;
            }
            if (!ObscuredPrefs.HasKey(DbKey.FREE_COIN_MARK))
            {
                FREE_COIN_MARK = true;
            }
            if (!ObscuredPrefs.HasKey(DbKey.DAY_RESET))
            {
                var tick = DateTime.Now.Date.Ticks;
                DAY_RESET = (long)new TimeSpan(tick).TotalDays;
            }

            if (!ObscuredPrefs.HasKey(DbKey.TRK_DATA))
            {
                var trkDb = new TrackingData();
                trkDb.Reset();

                ObscuredPrefs.Set(DbKey.H_TRK_DATA, trkDb.GetH());

                SetFromTData(DbKey.TRK_DATA, trkDb);
            }

            if (!ObscuredPrefs.HasKey(DbKey.TRK_MONET_DATA))
            {
                var trkDb = new TrackingAdData();
                trkDb.Reset();
                ObscuredPrefs.Set(DbKey.H_MONET_DATA, trkDb.GetH());

                SetFromTData(DbKey.TRK_MONET_DATA, trkDb);
            }

            if (!ObscuredPrefs.HasKey(DbKey.LEVEL_LENGTH_EXP))
            {
                LVL_LENGTH_EXP = 0;
            }
            if (!ObscuredPrefs.HasKey(DbKey.REWARD_DATA))
            {
                this.rewardData = new RewardData();
                ObscuredPrefs.Set(DbKey.H_R_D, rewardData.GetH());
                RewardData = rewardData;
            }

            if (!ObscuredPrefs.HasKey(DbKey.REWARD_BOOSTER_HAMMER_COUNT))
            {
                REWARD_BOOSTER_HAMMER_COUNT = 0;
            }

            if (!ObscuredPrefs.HasKey(DbKey.REWARD_BOOSTER_BLOOM_COUNT))
            {
                REWARD_BOOSTER_BLOOM_COUNT = 0;
            }

            if (!ObscuredPrefs.HasKey(DbKey.REWARD_BOOSTER_ADD_HOLE_COUNT))
            {
                REWARD_BOOSTER_ADD_HOLE_COUNT = 0;
            }
            if (!ObscuredPrefs.HasKey(DbKey.REWARD_BOOSTER_UNLOCK_BOX_COUNT))
            {
                REWARD_BOOSTER_UNLOCK_BOX_COUNT = 0;
            }

            if (!ObscuredPrefs.HasKey(DbKey.REWARD_LIFE_COUNT))
            {
                REWARD_LIFE_COUNT = 0;
            }

            if (!ObscuredPrefs.HasKey(DbKey.REWARD_COIN_FREE_COUNT))
            {
                REWARD_COIN_FREE_COUNT = 0;
            }

            if (!ObscuredPrefs.HasKey(DbKey.PREV_DATE_TIME))
            {
                PREV_DATE_TIME = new DateTime(2001, 1, 1);
            }

            if (!ObscuredPrefs.HasKey(DbKey.REWARD_SHOP_COIN_IAP_COUNT))
            {
                REWARD_SHOP_COIN_IAP_COUNT = 0;
            }

            if (!ObscuredPrefs.HasKey(DbKey.OW_USER_INFO))
            {
                OW_USER_INFO = new OfferwallUserInfo()
                {
                    token = "",
                    uuid = ""
                };
            }

            if (!ObscuredPrefs.HasKey(DbKey.OW_PRODEGE_DATA))
            {
                OW_PRODEGE_DATA = new OfferwallData()
                {
                    TotalRevenue = 0,
                    TotalVirtualCurrency = 0
                };
            }
            if (!ObscuredPrefs.HasKey(DbKey.PREBOOSTER_DATA))
            {
                var preBoosterData = new PreBoosterData(true);


                ObscuredPrefs.Set(DbKey.H_PRE_BOOSTER, preBoosterData.GetH());
                SetFromTData(DbKey.PREBOOSTER_DATA, preBoosterData);

                Debug.Log($"====== PreBoosterData Hash 0: {preBoosterData.ToString()}");
            }

            if (!ObscuredPrefs.HasKey(DbKey.LUCKY_SPIN_DATA))
            {
                LuckySpinData = new LuckySpinData();
            }
            if (!ObscuredPrefs.HasKey(DbKey.SCREW_BLOCKED_DATA))
            {
                var screwBlockedData = new ScrewBlockedRealTimeData();
                ScrewBlockedRealTimeData = screwBlockedData;
            }
            if (!ObscuredPrefs.HasKey(DbKey.WRENCH_COLLECTION))
            {
                WrenchCollectionData = new WrenchCollectionData();
            }
            if (!ObscuredPrefs.HasKey(DbKey.JOURNEY_DB))
            {
                var journeyDB = new JourneyDB();
                journeyDB.lstLevelClaim = new System.Collections.Generic.List<int>();
                JOURNEY_DB = journeyDB;
            }
            if (!ObscuredPrefs.HasKey(DbKey.REVIVE_DATA))
            {
                var reviveData = new ReviveData();
                ObscuredPrefs.Set(DbKey.H_REVIVE_DATA, reviveData.GetH());
                SetFromTData(DbKey.REVIVE_DATA, reviveData);
            }
            
            if (!ObscuredPrefs.HasKey(DbKey.LEVEL_BONUS_DATA))
            {
                var levelData = new LevelBonusData();
                levelData.lstLevelBonusUsed = new System.Collections.Generic.List<int>();
                levelData.lstLevelBonusStart = new System.Collections.Generic.List<int>();
                for (int i = 0; i < 10; i++)
                {
                    levelData.lstLevelBonusStart.Add(i);
                }
                levelData.lstLevelBonusStart.Shuffle();
                LEVEL_BONUS_DATA = levelData;
            }
            if (!ObscuredPrefs.HasKey(DbKey.LEVEL_FIRST_ADS))
            {
                LEVEL_FIRST_ADS = 0;
            }

            if (!ObscuredPrefs.HasKey(DbKey.SUPPER_OFFER_DATA))
            {
                SupperOfferData = new SupperOfferData();
            }

            if (!ObscuredPrefs.HasKey(DbKey.REWARD_IAP_DATA))
            {
                RewardIAPData = new RewardData();
            }

            if (!ObscuredPrefs.HasKey(DbKey.BEGINER_PACK_DATA))
            {
                BeginerPackData = new BeginerPackData();
            }

            Load();
        }

        private void Load()
        {
            cheatInt = ObscuredPrefs.Get(DbKey.CHEAT_INT, 0);
            sessionCount = ObscuredPrefs.Get(DbKey.SESSION_COUNT, 0);
            isCheater = ObscuredPrefs.Get(DbKey.IS_CHEATER, false);
            isSound = ObscuredPrefs.Get(DbKey.IS_SOUND, false);
            userInfo = GetFromJson<UserInfo>(DbKey.USER_INFO);
            isTutorial = ObscuredPrefs.Get(DbKey.TUTORIAL, true);
            isShowRating = ObscuredPrefs.Get(DbKey.IS_SHOW_RATING, true);
            rejectReviewCount = ObscuredPrefs.Get(DbKey.REJECT_REVIEW_COUNT, 0);

            boosterDatas = GetFromJson<BoosterDatas>(DbKey.BOOSTER_DATAS);
            preBoosterData = GetFromJson<PreBoosterData>(DbKey.PREBOOSTER_DATA);

            isColorMode = ObscuredPrefs.Get(DbKey.IS_COLOR_MODE, true);
            settingDatas = GetFromJson<SettingData>(DbKey.SETTING_DATAS);
            userExp = GetFromJson<UserExp>(DbKey.USER_EXP);

            _freeCoinMark = ObscuredPrefs.Get<bool>(DbKey.FREE_COIN_MARK, false);
            _adsCoinMark = ObscuredPrefs.Get<long>(DbKey.ADS_COIN_MARK, 0);
            _adsCoinAmount = ObscuredPrefs.Get<int>(DbKey.ADS_COIN_AMOUNT, 0);
            _dayReset = ObscuredPrefs.Get<long>(DbKey.DAY_RESET, 0);

            trkData = GetFromJson<TrackingData>(DbKey.TRK_DATA);
            lvlLengthExp = ObscuredPrefs.Get<int>(DbKey.LEVEL_LENGTH_EXP, 0);
            rewardData = GetFromJson<RewardData>(DbKey.REWARD_DATA);

            offerwallUserInfo = GetFromJson<OfferwallUserInfo>(DbKey.OW_USER_INFO);
            offerwallProdegeData = GetFromJson<OfferwallData>(DbKey.OW_PRODEGE_DATA);

            if (!ObscuredPrefs.HasKey(DbKey.H_USER_PROFILE))
            {
                ObscuredPrefs.Set(DbKey.H_USER_PROFILE, this.userInfo.GetH());
            }

            if (!ObscuredPrefs.HasKey(DbKey.H_BOOSTER))
            {
                ObscuredPrefs.Set(DbKey.H_BOOSTER, this.boosterDatas.GetH());
            }

            if (!ObscuredPrefs.HasKey(DbKey.H_PRE_BOOSTER))
            {
                ObscuredPrefs.Set(DbKey.H_PRE_BOOSTER, this.preBoosterData.GetH());
            }



            rewardBoosterHammerCount = ObscuredPrefs.Get(DbKey.REWARD_BOOSTER_HAMMER_COUNT, 0);
            rewardBoosterAddHoleCount = ObscuredPrefs.Get(DbKey.REWARD_BOOSTER_ADD_HOLE_COUNT, 0);
            rewardBoosterBloomCount = ObscuredPrefs.Get(DbKey.REWARD_BOOSTER_BLOOM_COUNT, 0);
            rewardBoosterUnlockBoxCount = ObscuredPrefs.Get(DbKey.REWARD_BOOSTER_UNLOCK_BOX_COUNT, 0);
            rewardShopCoinIAPCount = ObscuredPrefs.Get(DbKey.REWARD_SHOP_COIN_IAP_COUNT, 0);
            rewardLifeCount = ObscuredPrefs.Get(DbKey.REWARD_LIFE_COUNT, 0);
            rewardCoinFreeCount = ObscuredPrefs.Get(DbKey.REWARD_COIN_FREE_COUNT, 0);
            prevDateTime = ObscuredPrefs.Get(DbKey.PREV_DATE_TIME, new DateTime(2001, 1, 1));

            if (!ObscuredPrefs.HasKey(DbKey.H_R_D))
            {
                ObscuredPrefs.Set(DbKey.H_R_D, this.rewardData.GetH());
            }
            _luckySpinData = GetFromJson<LuckySpinData>(DbKey.LUCKY_SPIN_DATA);
            // Debug.Log($"Load PreBoosterData  preBoosterData==null:{preBoosterData==null}");

            _screwBlockedRealTimeData = GetFromJson<ScrewBlockedRealTimeData>(DbKey.SCREW_BLOCKED_DATA);
            _wrenchCollectionData = GetFromJson<WrenchCollectionData>(DbKey.WRENCH_COLLECTION);

            journeyDB = GetFromJson<JourneyDB>(DbKey.JOURNEY_DB);
            _reviveData = GetFromJson<ReviveData>(DbKey.REVIVE_DATA);
            levelBonusData = GetFromJson<LevelBonusData>(DbKey.LEVEL_BONUS_DATA);

            _levelFirstAds = ObscuredPrefs.Get<int>(DbKey.LEVEL_FIRST_ADS);
            _supperOfferData = GetFromJson<SupperOfferData>(DbKey.SUPPER_OFFER_DATA);
            _rewardIAPData = GetFromJson<RewardData>(DbKey.REWARD_IAP_DATA);
            _beginerPackData = GetFromJson<BeginerPackData>(DbKey.BEGINER_PACK_DATA);
        }
    }

    public static class DbKey
    {
        public static readonly string CHEAT_INT = "CHEAT_INT";
        public static readonly string IS_CHEATER = "IS_CHEATER";
        public static readonly string USER_INFO = "USER_INFO";
        public static readonly string IS_SOUND = "IS_SOUND";
        public static readonly string SESSION_COUNT = "SESSION_COUNT";
        public static readonly string TUTORIAL = "TUTORIAL";
        public static readonly string IS_SHOW_RATING = "IS_SHOW_RATING";
        public static readonly string REJECT_REVIEW_COUNT = "REJECT_REVIEW_COUNT";
        public static readonly string BOOSTER_DATAS = "BOOSTER_DATA";
        public static readonly string IS_COLOR_MODE = "IS_COLOR_MODE";
        public static readonly string SETTING_DATAS = "SETTING_DATAS";
        public static readonly string FREE_COIN_MARK = "FREE_TIME_MARK";
        public static readonly string USER_EXP = "USER_EXP";
        public static readonly string ADS_COIN_MARK = "ADS_COIN_MARK";
        public static readonly string ADS_COIN_AMOUNT = "ADS_COIN_AMOUNT";
        public static readonly string DAY_RESET = "DAY_RESET";

        public static readonly string LEVEL_LENGTH_EXP = "LEVEL_LENGTH_EXP";
        public static readonly string OW_VC_DATA = "OW_VC_DATA"; // offerwall virtual currency data
        public static readonly string OW_PRODEGE_DATA = "OW_PRODEGE_DATA"; // offerwall info of prodege
        public static readonly string OW_USER_INFO = "OW_USER_INFO"; // offerwall user info
        public static readonly string REWARD_DATA = "REWARD_DATA"; // offerwall virtual currency hash
        public static readonly string PREBOOSTER_DATA = "PREBOOSTER_DATA";
        public static readonly string LUCKY_SPIN_DATA = "LUCKY_SPIN_DATA";

        public static readonly string SCREW_BLOCKED_DATA = "SCREW_BLOCKED_DATA";
        public static readonly string WRENCH_COLLECTION = "WRENCH_COLLECTION";
        public static readonly string SUPPER_OFFER_DATA = "SUPPER_OFFER_DATA";
        public static readonly string REWARD_IAP_DATA = "REWARD_IAP_DATA";
        public static readonly string BEGINER_PACK_DATA = "BEGINER_PACK_DATA";

        //=============REMOTE REWARD CONTROL================
        public static readonly string REWARD_BOOSTER_HAMMER_COUNT = "REWARD_BOOSTER_HAMMER_COUNT";
        public static readonly string REWARD_BOOSTER_ADD_HOLE_COUNT = "REWARD_BOOSTER_ADD_HOLE_COUNT";
        public static readonly string REWARD_BOOSTER_BLOOM_COUNT = "REWARD_BOOSTER_BLOOM_COUNT";
        public static readonly string REWARD_BOOSTER_UNLOCK_BOX_COUNT = "REWARD_BOOSTER_UNLOCK_BOX_COUNT";
        public static readonly string REWARD_SHOP_COIN_IAP_COUNT = "REWARD_SHOP_COIN_IAP_COUNT";
        public static readonly string REWARD_LIFE_COUNT = "REWARD_LIFE_COUNT";
        public static readonly string REWARD_COIN_FREE_COUNT = "REWARD_COIN_FREE_COUNT";
        public static readonly string PREV_DATE_TIME = "PREV_DATE_TIME";
        //==================================================

        public static readonly string JOURNEY_DB = "JOURNEY_DB";
        public static readonly string REVIVE_DATA = "REVIVE_DATA";
        public static readonly string LEVEL_BONUS_DATA = "LEVEL_BONUS_DATA_NEW";

        //============= Anti Cheat ================
        public static readonly string H_USER_PROFILE = "HASH_U_P";
        public static readonly string H_USER_EXP = "HASH_U_E";
        public static readonly string H_W_I = "H_W_I";
        public static readonly string H_R_D = "H_R_D"; // reward data hash
        public static readonly string H_BOOSTER = "HASH_BST";
        public static readonly string H_PRE_BOOSTER = "HASH_PRE_BST";
        public static readonly string H_MONET_DATA = "H_AD_DATA";
        public static readonly string H_TRK_DATA = "H_UA_DATA";
        public static readonly string TRK_DATA = "SECRET_TRK_DATA";
        public static readonly string TRK_MONET_DATA = "SECRET_TRK_AD_DATA";
        public static readonly string H_REVIVE_DATA = "SECRET_REVIVE_DATA";
        public static readonly string LEVEL_FIRST_ADS = "LEVEL_FIRST_ADS";
        public static readonly string IS_BUYED_BEGINNER = "IS_BUYED_BEGINNER";
    }
}
