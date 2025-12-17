using System;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using ps.modules.journey;
using Storage.Model;
using UnityEngine;

namespace Storage
{

    public partial class LocalDb
    {
        private OfferwallData offerwallProdegeData;

        public OfferwallData OW_PRODEGE_DATA
        {
            get => offerwallProdegeData;
            set
            {
                offerwallProdegeData = value;
                SetFromTData(DbKey.OW_PRODEGE_DATA, offerwallProdegeData);
            }
        }

        private OfferwallUserInfo offerwallUserInfo;

        public OfferwallUserInfo OW_USER_INFO
        {
            get => offerwallUserInfo;
            set
            {
                offerwallUserInfo = value;
                SetFromTData(DbKey.OW_USER_INFO, offerwallUserInfo);
            }
        }

        private UserInfo userInfo;

        public UserInfo USER_INFO
        {
            get => userInfo;
            set
            {
                userInfo = value;
                userInfo.RandomizeCryptoKey();
                SetFromTData(DbKey.USER_INFO, userInfo);
            }
        }

        private ObscuredInt cheatInt;

        public ObscuredInt CHEAT_INT
        {
            get => cheatInt;
            set
            {
                cheatInt = value;
                cheatInt.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.CHEAT_INT, cheatInt);
            }
        }

        private ObscuredBool isCheater;

        public ObscuredBool IS_CHEATER
        {
            get => isCheater;
            set
            {
                isCheater = value;
                isCheater.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.IS_CHEATER, isCheater);
            }
        }

        private ObscuredBool isSound;

        public ObscuredBool IS_SOUND
        {
            get => isSound;
            set
            {
                isSound = value;
                isSound.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.IS_SOUND, isSound);
            }
        }
        private ObscuredInt sessionCount;

        public ObscuredInt SESSION_COUNT
        {
            get => sessionCount;
            set
            {
                sessionCount = value;
                sessionCount.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.SESSION_COUNT, sessionCount);
            }
        }

        private bool isTutorial;
        public bool IS_TUTORIAL
        {
            get => isTutorial;
            set
            {
                isTutorial = value;
                ObscuredPrefs.Set(DbKey.TUTORIAL, isTutorial);
            }
        }
        private ObscuredInt rejectReviewCount;
        public ObscuredInt REJECT_REVIEW_COUNT
        {
            get => rejectReviewCount;
            set
            {
                rejectReviewCount = value;
                rejectReviewCount.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.REJECT_REVIEW_COUNT, rejectReviewCount);
            }
        }
        private ObscuredBool isShowRating;
        public ObscuredBool IS_SHOW_RATING
        {
            get => isShowRating;
            set
            {
                isShowRating = value;
                isShowRating.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.IS_SHOW_RATING, isShowRating);
            }
        }
        private BoosterDatas boosterDatas;

        public BoosterDatas BOOSTER_DATAS
        {
            get => ObscuredPrefs.Get(DbKey.H_BOOSTER, "") != boosterDatas.GetH() ? new BoosterDatas() : boosterDatas;
            set
            {
                //boosterDatas = ObscuredPrefs.Get(DbKey.H_BOOSTER, "") != boosterDatas.GetH() ? new BoosterDatas() : value;
                //SetFromTData(DbKey.BOOSTER_DATAS, boosterDatas);
                //ObscuredPrefs.Set(DbKey.H_BOOSTER, boosterDatas.GetH());

                boosterDatas = value;
                boosterDatas.RandomizeCryptoKey();
                SetFromTData(DbKey.BOOSTER_DATAS, boosterDatas);
            }
        }

        private bool isColorMode;

        public bool IS_COLOR_MODE
        {
            get => isColorMode;
            set
            {
                isColorMode = value;
                ObscuredPrefs.Set(DbKey.IS_COLOR_MODE, value);

            }
        }

        private SettingData settingDatas;

        public SettingData SETTING_DATAS
        {
            get => settingDatas;
            set
            {
                settingDatas = value;
                settingDatas.RandomizeCryptoKey();
                SetFromTData(DbKey.SETTING_DATAS, settingDatas);
            }
        }

        private ObscuredBool _freeCoinMark;
        public ObscuredBool FREE_COIN_MARK
        {
            get => _freeCoinMark;
            set
            {
                _freeCoinMark = value;
                _freeCoinMark.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.FREE_COIN_MARK, _freeCoinMark);
            }
        }
        private ObscuredLong _adsCoinMark;

        public ObscuredLong ADS_COIN_MARK
        {
            get => _adsCoinMark;
            set
            {
                _adsCoinMark = value;
                _adsCoinMark.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.ADS_COIN_MARK, _adsCoinMark);
            }
        }
        private ObscuredInt _adsCoinAmount;

        public ObscuredInt ADS_COIN_AMOUNT
        {
            get => _adsCoinAmount;
            set
            {
                _adsCoinAmount = value;
                _adsCoinAmount.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.ADS_COIN_AMOUNT, _adsCoinAmount);
            }
        }
        private ObscuredLong _dayReset;

        public ObscuredLong DAY_RESET
        {
            get => _dayReset;
            set
            {
                _dayReset = value;
                _dayReset.RandomizeCryptoKey();
                ObscuredPrefs.Set(DbKey.DAY_RESET, _dayReset);
            }
        }

        private UserExp userExp;
        public UserExp USER_EXP
        {
            get => ObscuredPrefs.Get(DbKey.H_USER_EXP, "") != userExp.GetH() ? new UserExp() { exp = 0, level = 1, totalExp = 0 } : userExp;
            set
            {
                userExp = ObscuredPrefs.Get(DbKey.H_USER_EXP, "") != userExp.GetH() ? new UserExp() { exp = 0, level = 1, totalExp = 0 } : value;

                userExp.RandomizeCryptoKey();
                SetFromTData(DbKey.USER_EXP, userExp);
                ObscuredPrefs.Set(DbKey.H_USER_EXP, userExp.GetH());
            }
        }

        private TrackingData trkData;

        public TrackingData TRK_DATA
        {
            get => ObscuredPrefs.Get(DbKey.H_TRK_DATA, "") != trkData.GetH() ? new TrackingData() : trkData;
            set
            {
                trkData = ObscuredPrefs.Get(DbKey.H_TRK_DATA, "") != trkData.GetH() ? new TrackingData() : value;
                SetFromTData(DbKey.TRK_DATA, trkData);
                ObscuredPrefs.Set(DbKey.H_TRK_DATA, trkData.GetH());
            }
        }

        private int lvlLengthExp;

        public int LVL_LENGTH_EXP
        {
            get => lvlLengthExp;
            set
            {
                lvlLengthExp = value;
                ObscuredPrefs.Set(DbKey.LEVEL_LENGTH_EXP, lvlLengthExp);
            }
        }

        private double owVcData;

        public double OW_VC_DATA
        {
            get => owVcData;
            set
            {
                owVcData = value;
                ObscuredPrefs.Set(DbKey.OW_VC_DATA, owVcData);
            }
        }
        private RewardData rewardData;
        public RewardData RewardData
        {
            get
            {
                // if (ObscuredPrefs.Get(DbKey.H_R_D, "") != rewardData.GetH())
                // {
                //     Debug.LogError($"Reward Data error: {ObscuredPrefs.Get(DbKey.H_R_D, "")} != {rewardData.GetH()}");
                // }

                return ObscuredPrefs.Get(DbKey.H_R_D, "") != rewardData.GetH() ? new RewardData() : rewardData;
            }
            set
            {
                // if (ObscuredPrefs.Get(DbKey.H_R_D, "") != rewardData.GetH())
                // {
                //     Debug.LogError($"Reward Data error: {ObscuredPrefs.Get(DbKey.H_R_D, "")} != {rewardData.GetH()}");
                // }
                rewardData = ObscuredPrefs.Get(DbKey.H_R_D, "") != rewardData.GetH() ? new RewardData() : value;
                SetFromTData(DbKey.REWARD_DATA, rewardData);
                ObscuredPrefs.Set(DbKey.H_R_D, rewardData.GetH());
            }

        }


        private int rewardBoosterHammerCount;
        public int REWARD_BOOSTER_HAMMER_COUNT
        {
            get => rewardBoosterHammerCount;
            set
            {
                rewardBoosterHammerCount = value;
                ObscuredPrefs.Set(DbKey.REWARD_BOOSTER_HAMMER_COUNT, rewardBoosterHammerCount);
            }
        }

        int rewardBoosterAddHoleCount;
        public int REWARD_BOOSTER_ADD_HOLE_COUNT
        {
            get => rewardBoosterAddHoleCount;
            set
            {
                rewardBoosterAddHoleCount = value;
                ObscuredPrefs.Set(DbKey.REWARD_BOOSTER_ADD_HOLE_COUNT, rewardBoosterAddHoleCount);
            }
        }

        int rewardBoosterBloomCount;

        public int REWARD_BOOSTER_BLOOM_COUNT
        {
            get => rewardBoosterBloomCount;
            set
            {
                rewardBoosterBloomCount = value;
                ObscuredPrefs.Set(DbKey.REWARD_BOOSTER_BLOOM_COUNT, rewardBoosterBloomCount);
            }
        }

        int rewardBoosterUnlockBoxCount;

        public int REWARD_BOOSTER_UNLOCK_BOX_COUNT
        {
            get => rewardBoosterUnlockBoxCount;
            set
            {
                rewardBoosterUnlockBoxCount = value;
                ObscuredPrefs.Set(DbKey.REWARD_BOOSTER_UNLOCK_BOX_COUNT, rewardBoosterUnlockBoxCount);
            }
        }
        private int rewardLifeCount;
        public int REWARD_LIFE_COUNT
        {
            get => rewardLifeCount;
            set
            {
                rewardLifeCount = value;
                ObscuredPrefs.Set(DbKey.REWARD_LIFE_COUNT, rewardLifeCount);
            }
        }

        private int rewardCoinFreeCount;
        public int REWARD_COIN_FREE_COUNT
        {
            get => rewardCoinFreeCount;
            set
            {
                rewardCoinFreeCount = value;
                ObscuredPrefs.Set(DbKey.REWARD_COIN_FREE_COUNT, rewardCoinFreeCount);
            }
        }

        private DateTime prevDateTime;

        public DateTime PREV_DATE_TIME
        {
            get => prevDateTime;
            set
            {
                prevDateTime = value;
                ObscuredPrefs.Set(DbKey.PREV_DATE_TIME, prevDateTime);
            }
        }

        int rewardShopCoinIAPCount;

        public int REWARD_SHOP_COIN_IAP_COUNT
        {
            get => rewardShopCoinIAPCount;
            set
            {
                rewardShopCoinIAPCount = value;
                ObscuredPrefs.Set(DbKey.REWARD_SHOP_COIN_IAP_COUNT, rewardShopCoinIAPCount);
            }
        }
        private PreBoosterData preBoosterData;
        public PreBoosterData PreBoosterData
        {
            get
            {
                Debug.Log($"====== PreBoosterData Hash 1: {ObscuredPrefs.Get(DbKey.H_PRE_BOOSTER, "")} != {preBoosterData.ToString()}");
                return ObscuredPrefs.Get(DbKey.H_PRE_BOOSTER, "") != preBoosterData.GetH() ? new PreBoosterData(false) : preBoosterData;
            }
            set
            {
                Debug.Log($"====== PreBoosterData Hash 2: {ObscuredPrefs.Get(DbKey.H_PRE_BOOSTER, "")} != {preBoosterData.GetH()}");

                preBoosterData = ObscuredPrefs.Get(DbKey.H_PRE_BOOSTER, "") != preBoosterData.GetH() ? new PreBoosterData(false) : value;
                SetFromTData(DbKey.PREBOOSTER_DATA, preBoosterData);
                ObscuredPrefs.Set(DbKey.H_PRE_BOOSTER, preBoosterData.GetH());

                Debug.Log($"====== PreBoosterData Hash 3: {ObscuredPrefs.Get(DbKey.H_PRE_BOOSTER, "")} != {preBoosterData.GetH()}");

                //preBoosterData = value;
                //ObscuredPrefs.Set(DbKey.PREBOOSTER_DATA, preBoosterData);

            }
        }

        private LuckySpinData _luckySpinData;
        public LuckySpinData LuckySpinData
        {
            get => _luckySpinData;
            set
            {
                _luckySpinData = value;
                SetFromTData(DbKey.LUCKY_SPIN_DATA, _luckySpinData);
            }
        }
        private ScrewBlockedRealTimeData _screwBlockedRealTimeData;

        public ScrewBlockedRealTimeData ScrewBlockedRealTimeData
        {

            get => _screwBlockedRealTimeData;
            set
            {
                _screwBlockedRealTimeData = value;
                SetFromTData(DbKey.SCREW_BLOCKED_DATA, _screwBlockedRealTimeData);
            }
        }

        private WrenchCollectionData _wrenchCollectionData;
        public WrenchCollectionData WrenchCollectionData
        {
            get => _wrenchCollectionData;
            set
            {
                _wrenchCollectionData = value;
                SetFromTData(DbKey.WRENCH_COLLECTION, _wrenchCollectionData);
            }
        }

        private JourneyDB journeyDB;
        public JourneyDB JOURNEY_DB
        {
            get => journeyDB;
            set
            {
                journeyDB = value;
                SetFromTData(DbKey.JOURNEY_DB, journeyDB);
            }
        }

        private ReviveData _reviveData;
        public ReviveData ReviveData
        {
            get
            {
                return ObscuredPrefs.Get(DbKey.H_REVIVE_DATA, "") != _reviveData.GetH() ? new ReviveData() : _reviveData;
            }
            set
            {
                _reviveData = ObscuredPrefs.Get(DbKey.H_REVIVE_DATA, "") != _reviveData.GetH() ? new ReviveData() : value;
                ObscuredPrefs.Set(DbKey.H_REVIVE_DATA, _reviveData.GetH());
                SetFromTData(DbKey.REVIVE_DATA, _reviveData);
            }
        }
        private LevelBonusData levelBonusData;

        public LevelBonusData LEVEL_BONUS_DATA
        {
            get => levelBonusData;
            set
            {
                levelBonusData = value;
                SetFromTData(DbKey.LEVEL_BONUS_DATA, levelBonusData);
            }
        }

        int _levelFirstAds;

        public int LEVEL_FIRST_ADS
        {
            get => _levelFirstAds;
            set
            {
                _levelFirstAds = value;
                ObscuredPrefs.Set(DbKey.LEVEL_FIRST_ADS, _levelFirstAds);
            }
        }

        private SupperOfferData _supperOfferData;
        public SupperOfferData SupperOfferData
        {
            get => _supperOfferData;
            set
            {
                _supperOfferData = value;
                SetFromTData(DbKey.SUPPER_OFFER_DATA, _supperOfferData);
            }
        }

        private RewardData _rewardIAPData;
        public RewardData RewardIAPData
        {
            get
            {
                return _rewardIAPData;
            }
            set
            {
                _rewardIAPData = value;
                SetFromTData(DbKey.REWARD_IAP_DATA, _rewardIAPData);
            }
        }

        private BeginerPackData _beginerPackData;
        public BeginerPackData BeginerPackData
        {
            get => _beginerPackData;
            set
            {
                _beginerPackData = value;
                SetFromTData(DbKey.BEGINER_PACK_DATA, _beginerPackData);
            }
        }
    }
}