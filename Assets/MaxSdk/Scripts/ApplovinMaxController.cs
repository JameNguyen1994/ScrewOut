using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Ump.Api;
using PS.Ad.Consent;
using PS.Ad.Utils;
using PS.Database;
#if PS_UNITY_ANALYTICS
using GameAnalyticsSDK;
using PS.Analytic.Event;
#endif
using UnityEngine;
using UnityEngine.Events;
using Beebyte.Obfuscator;
using CodeStage.AntiCheat.ObscuredTypes.Converters;
using CodeStage.AntiCheat.Storage;
using GameAnalyticsSDK;
using static MaxSdkBase;
using Newtonsoft.Json;
using PS.Analytic;
using Storage;
using Storage.Model;

namespace PS.Ad
{
    [Skip]
    public class 
        
        ApplovinMaxController : PS.Utils.Singleton<ApplovinMaxController>
    {
        [SerializeField]
        string apiKey =
            "UGU5a3Gg0fbad6dFJei93m5_s1UrDJOeTtNPIMMdZnS18pacZ8Saq6fQlgacPbo7BFo-_dmCHe5cZdpBIrGPzZ";

        [SerializeField] string bannerAdUnitId;
        [SerializeField] private string interstitialAdUnitId;
        [SerializeField] string rewardAdUnitId;
        [SerializeField] private string openAdUnitId;
        [SerializeField] string mrecAdUnitId;

        [SerializeField] private bool useBannerAdaptive;
        [SerializeField] private MaxSdkBase.BannerPosition bannerPosition;
        [SerializeField] private Color bannerBackgroundColor = Color.black;

        [SerializeField] private MaxSdkBase.AdViewPosition mrecPosition;
        [SerializeField] private string bannerPlacement;
        [SerializeField] private string mrecPlacement;
        [SerializeField] private bool isAgeRestrictedUser;
        [SerializeField] private bool doNotSell;

        [SerializeField] private bool asyncWithRemote = true;

        [SerializeField] private List<string> lstDeviceTest;
        [SerializeField] private DebugGeography debugGeography;
        [SerializeField] private bool underAgeOfConsent;

        private int interstitialRetryAttempt;

        private int rewardedRetryAttempt;

        private UnityAction<ShowAdResult> onInterstitialScreenTextCompleted;
        //        private int rewardedInterstitialRetryAttempt;

        private bool isRewardAdCompleted;

        private bool exceptFocusAd;
        private bool enableFocusAd;
        private bool isNoAd;
        private bool isTurnOnAd;
        private bool isMrecReady;

        public static UnityAction OnInitializedEvent;
        public static bool IsInitialized { get; private set; }
        public bool IsNoAd { get => isNoAd;}

        // REWARD ADS
        private UnityAction OnRewardAdCompleted = null;
        private UnityAction OnRewardedAdNotEarn = null;
        private UnityAction<ShowAdResult> OnOpenAdShowComplete;

        // INTER ADS
        private UnityAction<ShowAdResult> OnInterAdCompleted = null;

        private MonetDatabase db;

        private Inform inform;

        private float _spentWatchAdTime = 0f;

        private ConsentDialog _consentDialog;

        private string rewardPlacement;
        string interstitialPlacement;
        private string openAdPlacement;
        private string mrecAdPlacement;

        protected override void CustomAwake()
        {
            IsInitialized = false;
            db = new MonetDatabase();

            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                // AppLovin SDK is initialized, configure and start loading ads.
                Debug.Log("MAX SDK Initialized");

                MaxSdk.SetUserId(db.UUID);
                print($"===================> uuid: {db.UUID}");

#if UNITY_IOS
                if (sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized ||
                    sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Unavailable)
                {
                    //AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
                    _consentDialog = new ConsentDialog(underAgeOfConsent, OnInitSdk, lstDeviceTest, debugGeography);
                }
                else
                {
                    //AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(false);
                    OnInitSdk(false);
                }
#endif
#if UNITY_ANDROID
                _consentDialog = new ConsentDialog(underAgeOfConsent, OnInitSdk, lstDeviceTest, debugGeography);
#endif
            };

            MaxSdk.SetDoNotSell(doNotSell);
            MaxSdk.SetSdkKey(apiKey);
            MaxSdk.InitializeSdk();

            LoadInform();

            isTurnOnAd = !asyncWithRemote;
          //  isTurnOnAd = false;
          SetFocusAd(false);
        }

        private void OnInitSdk(bool hasConsent)
        {
            if (IsInitialized) return;

            MaxSdk.SetHasUserConsent(hasConsent);
            InitAdsBeforeSdkInit();
        }

        void LoadInform()
        {
            var informPrefab = Resources.Load<GameObject>("InformCanvas");
            var informSpawn = Instantiate(informPrefab);
            informSpawn.name = "[InformMax]";
            inform = informSpawn.GetComponent<Inform>();
            //EnableCoverBanner(false);
        }

        void InitAdsBeforeSdkInit()
        {
            InitializeOpenAd();
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeBannerAds();
            InitializeMRecAds();
            IsInitialized = true;
            OnInitializedEvent?.Invoke();
        }

        private void Start()
        {
#if PS_UNITY_ANALYTICS
            GameAnalyticsILRD.SubscribeMaxImpressions();
#endif
        }

        #region Open Ad Methods

        void InitializeOpenAd()
        {
            if (openAdUnitId == "")
            {
                return;
            }

            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnOpenAdClickedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnOpenAdDisplayedEvent;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnOpenAdCloseEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnOpenAdLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnOpenAdDisplayFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnOpenAdLoadFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnOpenAdRevenuePaidEvent;

            MaxSdk.LoadAppOpenAd(openAdUnitId);
        }

        public void ShowOpenAdIfAvailable(UnityAction<ShowAdResult> result, string placement = null)
        {
            openAdPlacement = placement;
            if (isNoAd || !isTurnOnAd || !IsInitialized)
            {
                ShowAdResult reason = ShowAdResult.Unknown;

                if (isNoAd)
                {
                    reason = reason & ShowAdResult.BuyNoAd;
                }

                if (!isTurnOnAd)
                {
                    reason = reason & ShowAdResult.SetTurnOnAdFalse;
                }

                if (!IsInitialized)
                {
                    reason = reason & ShowAdResult.NotInitialized;
                }

                reason = reason ^ ShowAdResult.Unknown;

                result?.Invoke(reason);

                return;
            }
            
            if (MaxSdk.IsAppOpenAdReady(openAdUnitId))
            {
                OnOpenAdShowComplete = result;

                if (placement == null)
                {
                    MaxSdk.ShowAppOpenAd(openAdUnitId);
                }
                else
                {
                    MaxSdk.ShowAppOpenAd(openAdUnitId, placement);
                }
            }
            else
            {
                result?.Invoke(ShowAdResult.AdNotAvailable);
                MaxSdk.LoadAppOpenAd(openAdUnitId);
            }
        }

        public bool IsOpenAdAvailable()
        {
            if (!IsInitialized) return false;

            return MaxSdk.IsAppOpenAdReady(openAdUnitId);
        }

        void OnOpenAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("open ad click event.");
            GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.AppOpen, "applovin max", openAdPlacement);
        }

        void OnOpenAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("open ad displayed event.");
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.AppOpen, "applovin max", openAdPlacement);
        }

        void OnOpenAdCloseEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("open ad close event.");
            OnOpenAdShowComplete?.Invoke(ShowAdResult.Completed);
            OnOpenAdShowComplete = null;
            MaxSdk.LoadAppOpenAd(openAdUnitId);
        }

        void OnOpenAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("open ad loaded event.");
            MobileAnalytic.SendLatencyLoadedData(adInfo);
            GameAnalytics.NewAdEvent(GAAdAction.Loaded, GAAdType.AppOpen, "applovin max", "");
        }

        void OnOpenAdDisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error, MaxSdkBase.AdInfo adInfo)
        {
            print("open ad display failed event.");
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.AppOpen, "applovin max", openAdPlacement);
        }

        void OnOpenAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error)
        {
            print($"open ad load failed event with code {error.Code}: {error.Message}");

            MobileAnalytic.SendLatencyLoadFailedData(adUnitId, "AppOpen", error);
        }

        void OnOpenAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("open ad revenue paid event.");
            MobileAnalytic.SendAdRevenueToSingular(adInfo.Revenue, adInfo.AdUnitIdentifier, adInfo.AdFormat,
                adInfo.RevenuePrecision, adInfo.NetworkName,
                adInfo.Placement);
            
            var json = ObscuredPrefs.Get(DbKey.TRK_MONET_DATA, "{}");
            var trkData = JsonConvert.DeserializeObject<TrackingAdData>(json, new ObscuredTypesNewtonsoftConverter());

            if (ObscuredPrefs.Get(DbKey.H_MONET_DATA, "") != trkData.GetH())
            {
                return;
            }

            trkData.openCount++;
            trkData.openRev += adInfo.Revenue;
            ObscuredPrefs.Set(DbKey.H_MONET_DATA, trkData.GetH());
            var jsonOP = JsonConvert.SerializeObject(trkData, new ObscuredTypesNewtonsoftConverter());
            ObscuredPrefs.Set(DbKey.TRK_MONET_DATA, jsonOP);
        }

        #endregion


        #region Interstitial Ad Methods

        private void InitializeInterstitialAds()
        {
            if (interstitialAdUnitId == "")
            {
                return;
            }

            // Attach callbacks
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

            // Load the first interstitial
            LoadInterstitial();
        }

        void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(interstitialAdUnitId);
        }

        public void ShowInterstitial(UnityAction<ShowAdResult> result, string placement = null)
        {
            interstitialPlacement = placement;
            Debug.Log("ShowInterstitial 1");
            if (isNoAd || !isTurnOnAd || !IsInitialized)
            {
                ShowAdResult reason = ShowAdResult.Unknown;

                if (isNoAd)
                {
                    reason = reason | ShowAdResult.BuyNoAd;
                }

                if (!isTurnOnAd)
                {
                    reason = reason | ShowAdResult.SetTurnOnAdFalse;
                }

                if (!IsInitialized)
                {
                    reason = reason | ShowAdResult.NotInitialized;
                }

                reason = reason ^ ShowAdResult.Unknown;

                result?.Invoke(reason);
                return;
            }
            Debug.Log("ShowInterstitial 2");

            if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
            {
                exceptFocusAd = true;
                this.OnInterAdCompleted = result;
                _spentWatchAdTime = Time.realtimeSinceStartup;

                if (placement == null)
                {
                    MaxSdk.ShowInterstitial(interstitialAdUnitId);
                }
                else
                {
                    MaxSdk.ShowInterstitial(interstitialAdUnitId, placement);
                }
            }
            else
            {
                result?.Invoke(ShowAdResult.AdNotAvailable);
            }
        }

        public void ShowInterstitialWithScreenText(UnityAction<ShowAdResult> result, string placement = null)
        {
            if (isNoAd || !isTurnOnAd || !IsInitialized)
            {
                ShowAdResult reason = ShowAdResult.Unknown;

                if (isNoAd)
                {
                    reason |= ShowAdResult.BuyNoAd;
                }

                if (!isTurnOnAd)
                {
                    reason |= ShowAdResult.SetTurnOnAdFalse;
                }

                if (!IsInitialized)
                {
                    reason |= ShowAdResult.NotInitialized;
                }

                reason ^= ShowAdResult.Unknown;

                result?.Invoke(reason);
                return;
            }

            if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
            {
                exceptFocusAd = true;
                onInterstitialScreenTextCompleted = result;
                inform.Open(() => { StartCoroutine(WaitToShowInterAd(placement)); });
            }
            else
            {
                result?.Invoke(ShowAdResult.AdNotAvailable);
            }
        }

        IEnumerator WaitToShowInterAd(string placement)
        {
            yield return new WaitForSecondsRealtime(0);
            ShowInterstitial(CloseInform, placement);
        }

        void CloseInform(ShowAdResult result)
        {
            inform.Close(() =>
            {
                onInterstitialScreenTextCompleted?.Invoke(result);
                onInterstitialScreenTextCompleted = null;
            });
        }

        public bool IsInterstitialAvailable()
        {
            if (!IsInitialized) return false;

            return MaxSdk.IsInterstitialReady(interstitialAdUnitId);
        }

        void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("inter clicked event.");
            GameAnalyticController.Instance.Tracking().TrackingAdClicked("", "interstitial", adUnitId);

#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Interstitial, "applovin max", adUnitId);
#endif
        }

        void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("inter revenue paid event.");
            MobileAnalytic.SendAdRevenueToSingular(adInfo.Revenue, adInfo.AdUnitIdentifier, adInfo.AdFormat,
                adInfo.RevenuePrecision, adInfo.NetworkName,
                adInfo.Placement);
            
            var json = ObscuredPrefs.Get(DbKey.TRK_MONET_DATA, "{}");
            var trkData = JsonConvert.DeserializeObject<TrackingAdData>(json, new ObscuredTypesNewtonsoftConverter());

            if (ObscuredPrefs.Get(DbKey.H_MONET_DATA, "") != trkData.GetH())
            {
                return;
            }

            trkData.interCount++;
            trkData.interRev += adInfo.Revenue;
            ObscuredPrefs.Set(DbKey.H_MONET_DATA, trkData.GetH());
            var jsonOP = JsonConvert.SerializeObject(trkData, new ObscuredTypesNewtonsoftConverter());
            ObscuredPrefs.Set(DbKey.TRK_MONET_DATA, jsonOP);
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
            Debug.Log("Interstitial loaded");
#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.Loaded, GAAdType.Interstitial, "applovin max", interstitialPlacement);
#endif
            MobileAnalytic.SendLatencyLoadedData(adInfo);
            // Reset retry attempt
            interstitialRetryAttempt = 0;
        }

        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorCode)
        {
            // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

            Debug.Log("Interstitial failed to load with error code: " + errorCode.Code);
#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, "applovin max", interstitialPlacement);
#endif

            MobileAnalytic.SendLatencyLoadFailedData(adUnitId, "Interstitials", errorCode);

            Invoke("LoadInterstitial", (float)retryDelay);
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorCode,
            MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            Debug.Log("Interstitial failed to display with error code: " + errorCode.Code);

#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, "applovin max", interstitialPlacement);
#endif

            LoadInterstitial();
        }

        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            Debug.Log("Interstitial dismissed");
            OnInterAdCompleted?.Invoke(ShowAdResult.Completed);
            int _spentTime = Mathf.CeilToInt(Time.realtimeSinceStartup - _spentWatchAdTime);
            MobileAnalytic.SendSpentWatchAdTime(adInfo, _spentTime);

            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, "applovin max", interstitialPlacement, _spentTime);
            
            LoadInterstitial();
        }

        void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // GameAnalytics.StartTimer(interstitialPlacement);
// #if PS_UNITY_ANALYTICS
//             GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, "applovin max", interstitialPlacement);
// #endif
            db.WATCH_AD_NUM_ALL_GAME_LIFE++;
            MobileAnalytic.SendWatchAd(db.WATCH_AD_NUM_ALL_GAME_LIFE);
        }

        #endregion

        #region Rewarded Ad Methods

        private void InitializeRewardedAds()
        {
            if (rewardAdUnitId == "")
            {
                return;
            }

            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

            // Load the first RewardedAd
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(rewardAdUnitId);
        }

        public void ShowRewardedAd(UnityAction onReward,
            UnityAction onNotReward = null,
            UnityAction onAdNotReady = null, string placement = null)
        {
            rewardPlacement = placement;
            if (!IsInitialized)
            {
                //Debug.Log("Ads ");
                onAdNotReady?.Invoke();
                return;
            }

            if (MaxSdk.IsRewardedAdReady(rewardAdUnitId))
            {
                exceptFocusAd = true;
                this.OnRewardAdCompleted = onReward;
                this.OnRewardedAdNotEarn = onNotReward;
                _spentWatchAdTime = Time.realtimeSinceStartup;

                if (placement == null)
                {
                    MaxSdk.ShowRewardedAd(rewardAdUnitId);
                }
                else
                {
                    MaxSdk.ShowRewardedAd(rewardAdUnitId, placement);
                }
                GameAnalyticController.Instance.Tracking().TrackingStartRewardAds(true, "", rewardAdUnitId, placement);
            }
            else
            {
                onAdNotReady?.Invoke();
            }
        }
        public bool IsRewardAdAvailable()
        {
            if (!IsInitialized) return false;

            return MaxSdk.IsRewardedAdReady(rewardAdUnitId);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
            Debug.Log("Rewarded ad loaded");
#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.Loaded, GAAdType.RewardedVideo, "applovin max", rewardPlacement);
#endif
            MobileAnalytic.SendLatencyLoadedData(adInfo);
            // Reset retry attempt
            rewardedRetryAttempt = 0;


        }

        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorCode)
        {
            GameAnalyticController.Instance.Tracking().TrackingStartRewardAds(false, "", adUnitId, "");

            // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));

            Debug.Log("Rewarded ad failed to load with error code: " + errorCode.Code);
            MobileAnalytic.SendLatencyLoadFailedData(adUnitId, "Rewarded", errorCode);

#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "applovin max", rewardPlacement);
#endif

            Invoke("LoadRewardedAd", (float)retryDelay);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorCode,
            MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            Debug.Log("Rewarded ad failed to display with error code: " + errorCode.Code);
#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "applovin max", rewardPlacement);
#endif
            LoadRewardedAd();
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad displayed");
            // GameAnalytics.StartTimer(rewardPlacement);
// #if PS_UNITY_ANALYTICS
//             GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, "applovin max", rewardPlacement);
// #endif
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad clicked");
            GameAnalyticController.Instance.Tracking().TrackingAdClicked(rewardPlacement, "reward", rewardPlacement);

#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.RewardedVideo, "applovin max", rewardPlacement);
#endif
        }

        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            Debug.Log("Rewarded ad dismissed");
            LoadRewardedAd();

            if (!isRewardAdCompleted)
            {
                //AmplitudeController.Instance.Tracking().TrackingRewardAdFailed();
                OnRewardedAdNotEarn?.Invoke();
            }

            int _spentTime = Mathf.CeilToInt(Time.realtimeSinceStartup - _spentWatchAdTime);
            MobileAnalytic.SendSpentWatchAdTime(adInfo, _spentTime);
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, "applovin max", rewardPlacement, _spentTime);
            Debug.Log($"(GAAdAction.Show  GAAdType.RewardedVideo {rewardPlacement} rewarded ad time: {_spentTime} seconds");
            isRewardAdCompleted = false;
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            isRewardAdCompleted = true;
            // Rewarded ad was displayed and user should receive the reward
            Debug.Log("Rewarded ad received reward");
            OnRewardAdCompleted?.Invoke();

#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "applovin max", rewardPlacement);
#endif

            db.WATCH_AD_NUM_ALL_GAME_LIFE++;
            MobileAnalytic.SendWatchAd(db.WATCH_AD_NUM_ALL_GAME_LIFE);
            //AmplitudeController.Instance.Tracking().TrackingRewardAdComplete();
        }

        void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("reward revenue paid event");
            MobileAnalytic.SendAdRevenueToSingular(adInfo.Revenue, adInfo.AdUnitIdentifier, adInfo.AdFormat,
                adInfo.RevenuePrecision, adInfo.NetworkName,
                adInfo.Placement);
            
            var json = ObscuredPrefs.Get(DbKey.TRK_MONET_DATA, "{}");
            var trkData = JsonConvert.DeserializeObject<TrackingAdData>(json, new ObscuredTypesNewtonsoftConverter());

            if (ObscuredPrefs.Get(DbKey.H_MONET_DATA, "") != trkData.GetH())
            {
                return;
            }

            trkData.rewardCount++;
            trkData.rewardRev += adInfo.Revenue;
            ObscuredPrefs.Set(DbKey.H_MONET_DATA, trkData.GetH());
            var jsonOP = JsonConvert.SerializeObject(trkData, new ObscuredTypesNewtonsoftConverter());
            ObscuredPrefs.Set(DbKey.TRK_MONET_DATA, jsonOP);
        }

        #endregion

        #region Rewarded Interstitial Ad Methods

        /* private void InitializeRewardedInterstitialAds()
         {
             // Attach callbacks
             MaxSdkCallbacks.OnRewardedInterstitialAdLoadedEvent += OnRewardedInterstitialAdLoadedEvent;
             MaxSdkCallbacks.OnRewardedInterstitialAdLoadFailedEvent += OnRewardedInterstitialAdFailedEvent;
             MaxSdkCallbacks.OnRewardedInterstitialAdFailedToDisplayEvent += OnRewardedInterstitialAdFailedToDisplayEvent;
             MaxSdkCallbacks.OnRewardedInterstitialAdDisplayedEvent += OnRewardedInterstitialAdDisplayedEvent;
             MaxSdkCallbacks.OnRewardedInterstitialAdClickedEvent += OnRewardedInterstitialAdClickedEvent;
             MaxSdkCallbacks.OnRewardedInterstitialAdHiddenEvent += OnRewardedInterstitialAdDismissedEvent;
             MaxSdkCallbacks.OnRewardedInterstitialAdReceivedRewardEvent += OnRewardedInterstitialAdReceivedRewardEvent;
     
             // Load the first RewardedInterstitialAd
             LoadRewardedInterstitialAd();
         }*/

        #endregion

        #region Banner Ad Methods

        public void InitializeBannerAds()
        {
            if (bannerAdUnitId == "")
            {
                return;
            }

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
            // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
            MaxSdk.CreateBanner(bannerAdUnitId, bannerPosition);
            if (bannerPlacement != "")
            {
                MaxSdk.SetBannerPlacement(bannerAdUnitId, bannerPlacement);
            }

            if (useBannerAdaptive)
            {
                MaxSdk.SetBannerExtraParameter(bannerAdUnitId, "adaptive_banner", "true");
            }

            Color color = Color.black;
            color.a = 0;

            // Set background or background color for banners to be fully functional.
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, color);
        }

        private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error)
        {
            print("banner load failed");
            MobileAnalytic.SendLatencyLoadFailedData(adUnitId, "Banners", error);

#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Banner, "applovin max", adUnitId);
#endif
        }

        void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("banner loaded event");
            MobileAnalytic.SendLatencyLoadedData(adInfo);
#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.Loaded, GAAdType.Banner, "applovin max", adUnitId);
#endif
        }

        void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("banner clicked event");
            GameAnalyticController.Instance.Tracking().TrackingAdClicked("", "banner", adUnitId);

#if PS_UNITY_ANALYTICS
            GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Banner, "applovin max", adUnitId);
#endif
        }

        void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            print("banner revenue paid event");
            MobileAnalytic.SendAdRevenueToSingular(adInfo.Revenue, adInfo.AdUnitIdentifier, adInfo.AdFormat,
                adInfo.RevenuePrecision, adInfo.NetworkName,
                adInfo.Placement);
            
            var json = ObscuredPrefs.Get(DbKey.TRK_MONET_DATA, "{}");
            var trkData = JsonConvert.DeserializeObject<TrackingAdData>(json, new ObscuredTypesNewtonsoftConverter());

            if (ObscuredPrefs.Get(DbKey.H_MONET_DATA, "") != trkData.GetH())
            {
                return;
            }

            trkData.bannerCount++;
            trkData.bannerRev += adInfo.Revenue;
            ObscuredPrefs.Set(DbKey.H_MONET_DATA, trkData.GetH());
            var jsonOP = JsonConvert.SerializeObject(trkData, new ObscuredTypesNewtonsoftConverter());
            ObscuredPrefs.Set(DbKey.TRK_MONET_DATA, jsonOP);
        }

        public void ShowBanner()
        {
            if (isNoAd || !isTurnOnAd || !IsInitialized) return;
            MaxSdk.ShowBanner(bannerAdUnitId);
        }

        public void SetBannerPlacement(string placement)
        {
            MaxSdk.SetBannerPlacement(bannerAdUnitId, placement);
        }

        public void HideBanner()
        {
            if (!IsInitialized)
            {
                return;
            }
            EnableCoverBanner(false);
            MaxSdk.HideBanner(bannerAdUnitId);

        }

        public void DestroyBanner()
        {
            MaxSdk.DestroyBanner(bannerAdUnitId);

        }
        public Rect LayoutOfBanner()
        {
            return MaxSdk.GetBannerLayout(bannerAdUnitId);
        }

        #endregion

        #region MREC Ad Methods

        private void InitializeMRecAds()
        {
            if (mrecAdUnitId == "")
            {
                return;
            }

            MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMrecAdLoadedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMrecAdLoadFailedEvent;
            MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMrecAdClickedEvent;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMrecAdRevenuePaidEvent;
            // MRECs are automatically sized to 300x250.
            MaxSdk.CreateMRec(mrecAdUnitId, mrecPosition);

            if (mrecPlacement != "")
            {
                MaxSdk.SetMRecPlacement(mrecAdUnitId, mrecPlacement);
            }
        }

        private void OnMrecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error)
        {
            print("mrec load failed event");
            isMrecReady = false;
            MobileAnalytic.SendLatencyLoadFailedData(adUnitId, "MRECs", error);
        }

        public Rect LayoutOfMrec()
        {
            return MaxSdk.GetMRecLayout(mrecAdUnitId);
        }

        void OnMrecAdLoadedEvent(string sender, MaxSdkBase.AdInfo adInfo)
        {
            print("mrec loaded event");
            isMrecReady = true;
            MobileAnalytic.SendLatencyLoadedData(adInfo);
            GameAnalytics.NewAdEvent(GAAdAction.Loaded, GAAdType.Undefined, "applovin max", mrecAdPlacement);
        }

        void OnMrecAdClickedEvent(string sender, MaxSdkBase.AdInfo adInfo)
        {
            print("mrec clicked event");
            GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Undefined, "applovin max", mrecAdPlacement);

        }

        void OnMrecAdRevenuePaidEvent(string sender, MaxSdkBase.AdInfo adInfo)
        {
            print("mrec revenue paid event");
            MobileAnalytic.SendAdRevenueToSingular(adInfo.Revenue, adInfo.AdUnitIdentifier, adInfo.AdFormat,
                adInfo.RevenuePrecision, adInfo.NetworkName,
                adInfo.Placement);
            
            var json = ObscuredPrefs.Get(DbKey.TRK_MONET_DATA, "{}");
            var trkData = JsonConvert.DeserializeObject<TrackingAdData>(json, new ObscuredTypesNewtonsoftConverter());

            if (ObscuredPrefs.Get(DbKey.H_MONET_DATA, "") != trkData.GetH())
            {
                return;
            }

            trkData.mrecCount++;
            trkData.mrecRev += adInfo.Revenue;
            ObscuredPrefs.Set(DbKey.H_MONET_DATA, trkData.GetH());
            var jsonOP = JsonConvert.SerializeObject(trkData, new ObscuredTypesNewtonsoftConverter());
            ObscuredPrefs.Set(DbKey.TRK_MONET_DATA, jsonOP);
        }

        public void ShowMREC(string placement = null)
        {
            mrecAdPlacement = placement;
            if (isNoAd || !isTurnOnAd || !IsInitialized) return;

            if (placement != null)
            {
                MaxSdk.SetMRecPlacement(mrecAdUnitId, placement);
            }

            MaxSdk.ShowMRec(mrecAdUnitId);
        }

        public void HideMREC()
        {
            if (!IsInitialized) return;

            MaxSdk.HideMRec(mrecAdUnitId);
        }

        public bool IsMrecAvailable()
        {
            if (!IsInitialized) return false;

            return isMrecReady;
        }

        #endregion

        public void SetFocusAd(bool isEnable)
        {
            enableFocusAd = isEnable;
        }

        public void SetIsNoAd(bool isNoAd)
        {
            this.isNoAd = isNoAd;

            if (isNoAd)
            {
                Debug.Log("Hide banner");
                HideBanner();
                HideMREC();
                EnableCoverBanner(false);
            }
            else
            {
                ShowBanner();
                EnableCoverBanner(true);
            }
        }

        public void SetTurnOnAd(bool isTurnOnAd)
        {
            this.isTurnOnAd = isTurnOnAd;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                if (!exceptFocusAd)
                {
                    if (enableFocusAd && !isNoAd && isTurnOnAd )
                    {
                        if (IsOpenAdAvailable())
                        {
                            inform.Open(() => { StartCoroutine(WaitToShowAppOpenAd()); });
                        }
                    }
                }
                else
                {
                    exceptFocusAd = false;
                }
            }
        }
        
        // private void OnApplicationPause(bool pauseStatus)
        // {
        //     if (pauseStatus)
        //     {
        //         if (rewardPlacement != null)
        //         {
        //             GameAnalytics.PauseTimer(rewardPlacement);
        //         }
        //
        //         if (interstitialPlacement != null)
        //         {
        //             GameAnalytics.PauseTimer(interstitialPlacement);
        //         }
        //     }
        //     else
        //     {
        //         if (rewardPlacement != null)
        //         {
        //             GameAnalytics.ResumeTimer(rewardPlacement);
        //         }
        //         
        //         if (interstitialPlacement != null)
        //         {
        //             GameAnalytics.ResumeTimer(interstitialPlacement);
        //         }
        //     }
        // }

        IEnumerator WaitToShowAppOpenAd()
        {
            yield return new WaitForSecondsRealtime(0);
            ShowOpenAdIfAvailable(CloseInform, "focus_ad");
        }

        public void EnableExceptFocusAd()
        {
            exceptFocusAd = true;
        }

        public static void ShowTestSuit()
        {
            if (!IsInitialized) return;

            MaxSdk.ShowMediationDebugger();
        }

        public void EnableCoverBanner(bool enable)
        {
            inform?.SetBannerCoverActive(enable);
        }
    }

    public class MobileAnalytic
    {
        public static void SendWatchAd(int watchAd)
        {
#if PS_UNITY_ANALYTICS
            switch (watchAd)
            {
                case 3:
                    SingularSDK.Event("WATCH_3_ADS");
                    break;
                case 5:
                    SingularSDK.Event("WATCH_5_ADS");
                    break;
                case 10:
                    SingularSDK.Event("WATCH_10_ADS");
                    break;
                case 20:
                    SingularSDK.Event("WATCH_20_ADS");
                    break;
                case 30:
                    SingularSDK.Event("WATCH_30_ADS");
                    break;
                case 50:
                    SingularSDK.Event("WATCH_50_ADS");
                    break;
            }
#endif

#if PS_UNITY_SINGULAR
            switch (watchAd)
            {
                case 3:
                    SingularSDK.Event("WATCH_3_ADS");
                    break;
                case 5:
                    SingularSDK.Event("WATCH_5_ADS");
                    break;
                case 10:
                    SingularSDK.Event("WATCH_10_ADS");
                    break;
                case 20:
                    SingularSDK.Event("WATCH_20_ADS");
                    break;
                case 30:
                    SingularSDK.Event("WATCH_30_ADS");
                    break;
                case 50:
                    SingularSDK.Event("WATCH_50_ADS");
                    break;
                    //case 100:
                    //    SingularSDK.Event("WATCH_100_ADS");
                    //    break;
            }
#endif

            // switch (watchAd)
            // {
            //     //case 3:
            //     //    FirebaseAnalytics.LogEvent("WATCH_3_ADS");
            //     //    break;
            //     case 5:
            //         FirebaseAnalytics.LogEvent("WATCH_5_ADS");
            //         break;
            //     case 10:
            //         FirebaseAnalytics.LogEvent("WATCH_10_ADS");
            //         break;
            //     case 20:
            //         FirebaseAnalytics.LogEvent("WATCH_20_ADS");
            //         break;
            //         //case 30:
            //         //    GameAnalyticEvent.Event("WATCH_30_ADS");
            //         //    break;
            //         //case 50:
            //         //    GameAnalyticEvent.Event("WATCH_50_ADS");
            //         //    break;
            //         //case 100:
            //         //    GameAnalyticEvent.Event("WATCH_100_ADS");
            //         //    break;
            // }
        }

        public static void SendAdRevenueToSingular(double revenue, string adUnitId, string adType, string precision,
            string networkName, string placementName)
        {
#if PS_UNITY_SINGULAR
            SingularAdData data = new SingularAdData("AppLovinMax", "USD", revenue);
            data.WithAdUnitId(adUnitId);
            data.WithAdType(adType);
            data.WithPrecision(precision);
            data.WithNetworkName(networkName);
            data.WithAdPlacmentName(placementName);

            SingularSDK.AdRevenue(data);
            // GameAnalyticController.Instance.Tracking().SendAdRevenueToAmplitude(adUnitId, revenue, networkName, precision, placementName, "USD", adType, "ApplovinMax", "Ad Revenue");
            FirebaseAnalyticController.TrackingAdRevenue(revenue, adUnitId, adType, precision, networkName, placementName);
#endif
        }
        public static void SendAdmobAdRevenueToSingular(double revenue, string adUnitId, string adType, string precision,
       string networkName, string placementName)
        {
#if PS_UNITY_SINGULAR
            SingularAdData data = new SingularAdData("Admob", "USD", revenue);
            data.WithAdUnitId(adUnitId);
            data.WithAdType(adType);
            data.WithPrecision(precision);
            data.WithNetworkName(networkName);
            data.WithAdPlacmentName(placementName);

            SingularSDK.AdRevenue(data);
            // GameAnalyticController.Instance.Tracking().SendAdRevenueToAmplitude(adUnitId, revenue, networkName, precision, placementName, "USD", adType, "Admob", "Ad Revenue");
            FirebaseAnalyticController.TrackingAdRevenue(revenue, adUnitId, adType, precision, networkName, placementName);

#endif
        }

        public static void SendSpentWatchAdTime(MaxSdkBase.AdInfo adInfo, int spentTime)
        {
            Debug.Log($"spent watch ad time: {spentTime}s");
#if PS_UNITY_ANALYTICS
            Dictionary<string, object> _data = new Dictionary<string, object>()
            {
                ["ad_unit_id"] = adInfo.AdUnitIdentifier,
                ["ad_type"] = adInfo.AdFormat,
                ["network_name"] = adInfo.NetworkName,
                ["placement"] = adInfo.Placement,
                ["network_placement"] = adInfo.NetworkPlacement,
                ["watch_time"] = $"{spentTime}"
            };
            
            GameAnalyticEvent.Event("WATCH_AD_SPEND_TIME", _data);
#endif
        }

        public static void SendLatencyLoadedData(MaxSdkBase.AdInfo adInfo)
        {
#if PS_UNITY_SINGULAR
            //Dictionary<string, object> _data = new Dictionary<string, object>()
            //{
            //    ["ad_unit_id"] = adInfo.AdUnitIdentifier,
            //    ["ad_type"] = adInfo.AdFormat,
            //    ["network_name"] = adInfo.NetworkName,
            //    ["placement"] = adInfo.Placement,
            //    ["network_placement"] = adInfo.NetworkPlacement,
            //    ["latency"] = adInfo.WaterfallInfo.LatencyMillis
            //};

            //SingularSDK.Event(_data, "loaded_latency");
#endif
        }

        public static void SendLatencyLoadFailedData(string adUnitId, string adFormat, MaxSdkBase.ErrorInfo error)
        {
#if PS_UNITY_SINGULAR

            //var data = new WaterfallResponse();
            //foreach (var response in error.WaterfallInfo.NetworkResponses)
            //{
            //    data.data.Add(response.ToString());
            //}

            //string json = JsonUtility.ToJson(data);

            //Dictionary<string, object> _data = new Dictionary<string, object>()
            //{
            //    ["ad_unit_id"] = adUnitId,
            //    ["ad_type"] = adFormat,
            //    ["latency"] = error.WaterfallInfo.LatencyMillis,
            //    ["error_massage"] = error.Message,
            //    ["error_code"] = error.Code,
            //    ["error_detail"] = json
            //};

            //SingularSDK.Event(_data, "load_failed_latency");
#endif

            GameAnalyticController.Instance.Tracking().TrackingLoadAdFailed(adUnitId, adFormat);
        }
    }

    [Flags]
    public enum ShowAdResult
    {
        Unknown = 0,
        Completed = 1,
        BuyNoAd = 2,
        NotInitialized = 4,
        SetTurnOnAdFalse = 8,
        AdNotAvailable = 16,
    }

    public enum AdAction
    {
        Undefined = 0,
        Clicked = 1,
        Show = 2,
        FailedShow = 3,
        RewardReceived = 4,
        Request = 5,
        Loaded = 6
    }

    public enum AdType
    {
        Undefined = 0,
        Video = 1,
        RewardedVideo = 2,
        Playable = 3,
        Interstitial = 4,
        OfferWall = 5,
        Banner = 6
    }

    [Serializable]
    public class WaterfallResponse
    {
        public List<string> data;

        public WaterfallResponse()
        {
            data = new List<string>();
        }
    }
}