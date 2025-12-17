using Cysharp.Threading.Tasks;
using MainMenuBar;
using PS.Ad;
using PS.Analytic;
using PS.Analytic.RemoteConfig;
using PS.Utils;
using Storage;
using Storage.Model;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AdsController : Singleton<AdsController>
{
    private bool isShowBanner = false;

    private int gameplayCount = 0;
    private int backHomeCount = 0;
    private int pauseCount = 0;

    private float nextTimeToShowInterAfterInter = 0;
    private float nextTimeToShowInterAfterReward = 0;

    public bool IsShowBanner { get => isShowBanner; }

    [SerializeField] private PopupFirstAds popupFirstAds;
    [SerializeField] private ScreenBeforeAds screenBeforeAds;
    private void Start()
    {
        //StartAdByLevel(_userInforController.GetValueByType(UserInfoType.Level)).Forget();
        /* StartCoroutine(StartAdByLevel(Db.storage.USER_INFO.level));
         CheckPlayTimeStartInter();*/
    }

    public IEnumerator StartAdByLevel(long level)
    {
        Debug.Log("StartAdByLevel - WaitUntil IsReadyRemote");
        var _remote = GameAnalyticController.Instance.Remote();
        yield return new WaitUntil(() => _remote.IsReadyRemote);
        Debug.Log("StartAdByLevel - IsReadyRemote");
        StartFocusAd(level);
        Debug.Log($"StartAdByLevel - Level:{level} - IsAdOn:{_remote.IsAdOn} - LevelStartFocus:{_remote.LevelStartFocus} - BannerLvl:{_remote.LevelStartAd.bannerLvl}");
        ApplovinMaxController.Instance.SetTurnOnAd(_remote.IsAdOn);
        if (_remote.IsAdOn)
        {
            // ApplovinMaxController.Instance.SetFocusAd(_remote.LevelStartFocus <= level && _remote.LevelStartFocus >= 0);
            //ApplovinMaxController.Instance.SetFocusAd(false);
            bool isShowBannerByRemote = level >= _remote.LevelStartAd.bannerLvl;
            Debug.Log($"ShowBanner  bannerLevel{_remote.LevelStartAd.bannerLvl}/{level}");

            if (isShowBannerByRemote)
            {
                ApplovinMaxController.Instance.ShowBanner();
                ApplovinMaxController.Instance.EnableCoverBanner(true);
                isShowBanner = true;
            }
            else
            {
                Debug.Log("HideBanner");

                ApplovinMaxController.Instance.HideBanner();
                ApplovinMaxController.Instance.EnableCoverBanner(false);
                isShowBanner = false;
            }
            LevelController.Instance.ChangeHolderPosByBanner(isShowBanner);

            EventDispatcher.Push(EventId.OnToggleBanner);

        }
        else
        {
            ApplovinMaxController.Instance.HideBanner();
            ApplovinMaxController.Instance.EnableCoverBanner(false);
            isShowBanner = false;
            LevelController.Instance.ChangeHolderPosByBanner(isShowBanner);

        }


        /*#if UNITY_EDITOR
                ApplovinMaxController.Instance.SetFocusAd(false);
        #endif*/
    }
    public bool IsShowPopupBuyNoAdsLoop(bool isWin)
    {
        // 15 -> win -> 16 -> check ads 16- 9 =7
        // 15 -> lose -> 15 +1 -> check ads 16 -9 =7



        int levelFistNoAds = GameAnalyticController.Instance.Remote().LevelStartAd.interFirstStart;
        int level = Db.storage.USER_INFO.level;
        int numLoop = GameAnalyticController.Instance.Remote().NumToLoopShowAdsPopup;
        if (numLoop <= 0)
        {
            return false;
        }
        if (!isWin)
        {
            level += 1;
        }
        bool isShow = levelFistNoAds != 0 && (level - levelFistNoAds >= numLoop) && (level - levelFistNoAds) % numLoop == 0;
        Debug.Log($"ShowPopupBuyNoAdsLoop: {isShow} - levelFistNoAds: {levelFistNoAds} - level: {level}");

        bool isNoAds = CheckNoAds.Instance.CheckIsNoAds();

        return isShow && !isNoAds;
    }

    private void ShowAdsBreak(Func<GameAnalyticRemoteConfig, bool> canShow, UnityAction<ShowAdResult> onComplete = null, string placement = null)
    {
        if (canShow(GameAnalyticController.Instance.Remote()))
        {
            ApplovinMaxController.Instance.ShowInterstitialWithScreenText((Result) =>
            {
                nextTimeToShowInterAfterInter = Time.time + GameAnalyticController.Instance.Remote().InterCd;
                onComplete?.Invoke(Result);
            }, placement);
        }
        else
        {
            onComplete?.Invoke(ShowAdResult.Unknown);
        }
    }
    public async UniTask ShowInterFailed(int level, int gamePlayTime, UnityAction<ShowAdResult> onComplete)
    {
        bool isBoughtNoADs = CheckNoAds.Instance.CheckIsNoAds();
        if (isBoughtNoADs)
        {
            Debug.Log("AdsController ShowInterFailed: isBoughtNoADs true");
            onComplete(ShowAdResult.BuyNoAd);
            return;
        }
        var user = Db.storage.USER_INFO;
        var timeShowAdsBefore = user.playTimeShowAds;
        var remote = GameAnalyticController.Instance.Remote();
        Debug.Log($"LEVEL_END FAIL{gamePlayTime}: ");
        bool isEnableInterForFirst =
            remote.LevelStartAd.interFirstStart <= level;
        bool isEnableInterTime = remote.LevelStartAd.interMinTime <= gamePlayTime - timeShowAdsBefore;

        bool isShowInterFailed = isEnableInterForFirst && isEnableInterTime;
        Debug.Log($"IsShowInterFailed: {isShowInterFailed} - isEnableInterForFirst: {isEnableInterForFirst} - isEnableInterTime: {isEnableInterTime} - level: {level} - gamePlayTime: {gamePlayTime} - timeShowAdsBefore: {timeShowAdsBefore}");
        if (!isShowInterFailed)
        {
            //Fix Lose Game
            onComplete(ShowAdResult.Unknown);
            return;
        }
        user.playTimeShowAds = gamePlayTime;
        Db.storage.USER_INFO = user;
        bool isShowLoop = IsShowPopupBuyNoAdsLoop(false);
        if ((Db.storage.USER_INFO.countInterAds == 0 && IngameData.IsCheckFirstAds) || isShowLoop)
        {
            Debug.Log("Show First Ads Popup 1");
            IngameData.IsCheckFirstAds = false;
            /*            if (Db.storage.USER_INFO.countInterAds == 0)
                        {
                           Db.storage.LEVEL_FIRST_ADS = Db.storage.USER_INFO.level;
                        }*/
            bool isNoAds = CheckNoAds.Instance.CheckIsNoAds();
            if (!isNoAds)
            {
                popupFirstAds.Show(async () =>
                {
                    ForceShowInterFailed(onComplete);

                });
            }
            else
            {
                onComplete?.Invoke(ShowAdResult.BuyNoAd);
            }
        }
        else
        {
            await screenBeforeAds.Show();
            ApplovinMaxController.Instance.ShowInterstitial(onComplete, "inter_lose");
            await screenBeforeAds.Hide();

        }
    }

    public async UniTask ForceShowInterFailed(UnityAction<ShowAdResult> onComplete)
    {
        await screenBeforeAds.Show();

        ApplovinMaxController.Instance.ShowInterstitial(onComplete, "inter_lose");
        await screenBeforeAds.Hide();

    }
    public void OnBuyNoAds()
    {
        popupFirstAds.OnBuyNoAds();
    }

    private async UniTask CheckShowInter(Func<GameAnalyticRemoteConfig, bool> canShow, UnityAction<ShowAdResult> onComplete = null, string placement = null)
    {
        if (canShow(GameAnalyticController.Instance.Remote()))
        {
            var isShowNoAdsLoop = IsShowPopupBuyNoAdsLoop(true);
            if ((Db.storage.USER_INFO.countInterAds == 0 && IngameData.IsCheckFirstAds) || isShowNoAdsLoop)
            {
                Debug.Log("Show First Ads Popup 1");
                IngameData.IsCheckFirstAds = false;
                await popupFirstAds.Show(async () =>
                {
                    bool isNoAds = CheckNoAds.Instance.CheckIsNoAds();
                    if (!isNoAds)
                    {

                        await screenBeforeAds.Show();
                        ApplovinMaxController.Instance.ShowInterstitial((Result) =>
                        {
                            Db.storage.USER_INFO.countInterAds++;
                            Db.storage.USER_INFO = Db.storage.USER_INFO;
                            nextTimeToShowInterAfterInter = Time.time + GameAnalyticController.Instance.Remote().InterCd;
                            onComplete?.Invoke(Result);
                            screenBeforeAds.Hide();
                        }, placement);
                    }
                    else
                    {
                        onComplete?.Invoke(ShowAdResult.BuyNoAd);
                    }

                });
            }
            else
            {
                Debug.Log("Show First Ads Popup 2");

                await screenBeforeAds.Show();
                ApplovinMaxController.Instance.ShowInterstitial((Result) =>
                {
                    Db.storage.USER_INFO.countInterAds++;
                    Db.storage.USER_INFO = Db.storage.USER_INFO;
                    nextTimeToShowInterAfterInter = Time.time + GameAnalyticController.Instance.Remote().InterCd;
                    onComplete?.Invoke(Result);
                    screenBeforeAds.Hide();
                }, placement);
            }

        }
        else
        {
            onComplete?.Invoke(ShowAdResult.Unknown);
        }
    }

    public void ShowAdBreakUnScrew(UnityAction<ShowAdResult> onComplete = null)
    {
        ShowAdsBreak(CanShowAdBreak, onComplete, $"{AdsPlacement.inter_break}");
    }

    public void ShowInterEnd(UnityAction<ShowAdResult> onComplete = null)
    {
        CheckShowInter(CanShowInterEnd, onComplete, $"{AdsPlacement.inter_end}");
    }

    public void CheckShowInterHome(UnityAction<ShowAdResult> onComplete = null)
    {
        CheckShowInter(CanShowInterHome, onComplete, $"{AdsPlacement.inter_end}");
    }

    public void CheckShowInterPause(UnityAction<ShowAdResult> onComplete = null)
    {
        CheckShowInter(CanShowInterPause, onComplete, $"{AdsPlacement.inter_end}");
    }


    private bool CanShowAdBreak(GameAnalyticRemoteConfig remote)
    {
        return false;
        bool result = remote.SegmentFlow.breakCount > 0 && IngameData.BREAK_SCREW_COUNT >= remote.SegmentFlow.breakCount;
        bool interEnable = IsStartInterBreak() && !HasShowedInterRecently() && !HasShowedRewardRecently();
        result = result && interEnable;
        if (result)
        {
            IngameData.BREAK_SCREW_COUNT = 0;
        }
        //AmplitudeController.Instance.SendExposureEvent(FlagKey.inter_end);

        return result;
    }
    private bool CanShowInterEnd(GameAnalyticRemoteConfig remote)
    {
        bool isBoughtNoADs = CheckNoAds.Instance.CheckIsNoAds();
        if (isBoughtNoADs)
        {
            Debug.Log("AdsController CanShowInterEnd: isBoughtNoADs true");

            return false;
        }
        int level = Db.storage.USER_INFO.level;

        bool isEnableInterForFirst =
            remote.LevelStartAd.interFirstStart <= level && level < remote.LevelStartAd.interSecondStart;
        bool isEnableInterFrequency = remote.LevelStartAd.interFirstFrequency > 0 &&
            (level - remote.LevelStartAd.interFirstStart) % remote.LevelStartAd.interFirstFrequency == 0;

        bool isFirstInterEnable = isEnableInterFrequency && isEnableInterForFirst;

        bool isEnableInterForSecond =
            remote.LevelStartAd.interSecondStart <= level;
        bool isEnableInterFrequencySecond = remote.LevelStartAd.interSecondFrequency > 0 &&
                                            (level - remote.LevelStartAd.interSecondStart) % remote.LevelStartAd.interSecondFrequency == 0;

        bool isSecondInterEnable = isEnableInterFrequencySecond && isEnableInterForSecond;

        bool isShowInterEnd = isFirstInterEnable || isSecondInterEnable;

        return isShowInterEnd;
    }

    private bool CanShowInterHome(GameAnalyticRemoteConfig remote)
    {
        return false;
        int interHome = remote.InterHome;
        backHomeCount++;
        bool result = interHome > 0 && backHomeCount >= interHome;
        bool interEnable = IsStartInterEnd() && !HasShowedInterRecently() && !HasShowedRewardRecently();
        result = result && interEnable;
        //AmplitudeController.Instance.SendExposureEvent(FlagKey.inter_home);

        if (result)
        {
            backHomeCount = 0;
        }

        return result;
    }

    public void StartFocusAd(long level)
    {
        var remote = GameAnalyticController.Instance.Remote();

        if (remote == null)
        {
            return;
        }

        bool isFocusAdOn = remote.LevelStartAd.focusLvl > 0 && level >= remote.LevelStartAd.focusLvl;
        // AdController.Instance?.AppFocusing.SetFocusing(isFocusAdOn);
        // ApplovinMaxController.Instance.SetFocusAd(isFocusAdOn);
    }
    private bool CanShowInterPause(GameAnalyticRemoteConfig remote)
    {
        return false;
        int numShowInterAtPause = remote.InterPause;
        pauseCount++;
        bool result = numShowInterAtPause > 0 && pauseCount >= numShowInterAtPause;
        bool interEnable = IsStartInterEnd() && !HasShowedInterRecently() && !HasShowedRewardRecently();
        result = result && interEnable;
        if (result)
        {
            pauseCount = 0;
        }
        //AmplitudeController.Instance.SendExposureEvent(FlagKey.inter_pause);
        return result;
    }


    private bool IsStartInterEnd()
    {
        return false;
        var remote = GameAnalyticController.Instance.Remote();
        var isStart = Db.storage.USER_INFO.level > remote.SegmentFlow.interLvl && remote.SegmentFlow.interLvl >= 0;
        return isStart;
    }

    private bool IsStartInterBreak()
    {
        return false;
        var remote = GameAnalyticController.Instance.Remote();
        var isStart = Db.storage.USER_INFO.level > remote.SegmentFlow.breakLvl && remote.SegmentFlow.breakLvl >= 0;
        return isStart;
    }


    private bool HasShowedInterRecently()
    {
        bool isShow = Time.time < nextTimeToShowInterAfterInter;

        //AmplitudeController.Instance.SendExposureEvent(FlagKey.inter_cd);

        //return isShow;
        return false;
    }

    private bool HasShowedRewardRecently()
    {
        bool isShow = Time.time < nextTimeToShowInterAfterReward;

        //AmplitudeController.Instance.SendExposureEvent(FlagKey.rw_skipinter);

        //return isShow;
        return false;
    }

    public void ShowRewardAds(RewardAdsPos rewardAdsPos, UnityAction onComplete,
            UnityAction notComplete = null,
            UnityAction onAdNotReady = null, string placement = "none")
    {
        ApplovinMaxController.Instance.ShowRewardedAd(() =>
        {
            //  AmplitudeController.Instance.Tracking().TrackingRewardAdComplete(rewardAdsPos);

            nextTimeToShowInterAfterReward = Time.time + GameAnalyticController.Instance.Remote().RwSkipInter;
            onComplete?.Invoke();
        }, () =>
                {
                    notComplete?.Invoke();
                    ToastMessage.Instance.ShowToast("No Reward Ads Available");
                }, () =>
        {
            GameAnalyticController.Instance.Tracking().TrackingRewardAdFailed(rewardAdsPos);
            onAdNotReady?.Invoke();
            ToastMessage.Instance.ShowToast("No Reward Ads Available");
        }, placement);
    }

    async UniTask CountDownStartInter()
    {
        await UniTask.Delay(1000);
        //Db.storage.PLAY_TIME_START_INTER++;
    }

    async void CheckPlayTimeStartInter()
    {
        var _remote = GameAnalyticController.Instance.Remote();

        while (true/*Db.storage.PLAY_TIME_START_INTER != -1 && Db.storage.PLAY_TIME_START_INTER < _remote.LevelStartAd.interStartTime*/)
        {
            await CountDownStartInter();
        }
        //Db.storage.PLAY_TIME_START_INTER = -1;
    }

    public void OnGoHome()
    {
        ApplovinMaxController.Instance.HideBanner();
        ApplovinMaxController.Instance.EnableCoverBanner(false);
    }
}
