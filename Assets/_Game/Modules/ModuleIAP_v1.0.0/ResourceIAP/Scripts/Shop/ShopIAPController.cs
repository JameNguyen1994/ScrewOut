using DG.Tweening;
using PS.Utils;
using Storage;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PS.Analytic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ResourceIAP;
using MainMenuBar;
using Newtonsoft.Json;
using ps.modules.journey;

public class ShopIAPController : Singleton<ShopIAPController>
{
    [Header("IMAGE FIELD")]
    [SerializeField] private Image imgBGPanel;

    [Header("TRANSFORM FIELD")]
    [SerializeField] private Transform tfmBGPanel;
    [SerializeField] private Transform tfmPopup;
    [SerializeField] private RectTransform rtfmShop;

    [SerializeField] private Button btnClose;
    //  [SerializeField] private Button btnMoreOffer;
    [SerializeField] private PopupBeginnerPack popupBeginnerPack;
    [SerializeField] private PopupNoAds popupNoAds;
    [SerializeField] private ShopNoAdsBundle shopNoAdsBundle;
    private UnityAction actionOnClose;

    [Header("Item Management")]
    [SerializeField] private Transform tfmParent;
    [SerializeField] private List<GameObject> lstItem;
    [SerializeField] private List<int> lstIndexFullHome = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
    [SerializeField] private List<int> lstIndexAbbreviatedGamePlay = new List<int> { 8, 0, 1, 7 };
    [SerializeField] private List<int> lstIndexAbbreviatedHeartGamePlay = new List<int> { 0, 3, 7, 8 };
    [SerializeField] private List<int> lstIndexFullGamePlay = new List<int> { 6, 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12 };

    [SerializeField] private ItemCoinFreeTime itemcoin;
    [SerializeField] private ItemCoinAds itemAds;
    [SerializeField] private bool showing;
    [Header("Scroll Management")]
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private OfferwallBanner offerwallPage;
    [SerializeField] private PopupCompletedPurchase popupCompletedPurchase;
    [SerializeField] private ResourceDataIAPSO resourceDataIAPSO;
    [SerializeField] private GameObject content;


    public bool Showing { get => showing; }

    private Tween navigationTween;

    private async void Start()
    {
        showing = false;
        GetCurrentTimeZoneOffset();
        /*   bool success = await CurrencyConverter.Instance.FetchExchangeRates();
           if (success)
           {
               Debug.Log("Tỷ giá đã được cập nhật!");
           }
           else
           {
               Debug.LogError("Không thể cập nhật tỷ giá!");
           }*/
    }

    private async void OnEnable()
    {
        await UniTask.WaitUntil(() => GameAnalyticController.Instance.Remote().IsReadyRemote);
        Debug.Log("+++++ beginAds :  " + GameAnalyticController.Instance.Remote().BeginAds.beginAds);
        Debug.Log("+++++ noadsWithCombo :  " + GameAnalyticController.Instance.Remote().NoAdsWithCombo.noAdsWithCombo);
        Debug.Log("+++++ noads :  " + GameAnalyticController.Instance.Remote().SegmentFlow.noads);
    }

    private void ShowItems(List<int> lstIndex)
    {
        var remote = GameAnalyticController.Instance.Remote();
        string offerwallData = remote.OfferWallRemote;

        var owDataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(offerwallData);
        if (owDataDict != null && owDataDict.ContainsKey("enable") && Convert.ToBoolean(owDataDict["enable"]))
        {
            offerwallPage.Init(true);
        }
        else
        {
            offerwallPage.Init(false);
        }

        /* // Deactivate all items first
         foreach (var item in lstItem)
         {
             item.SetActive(false);
         }

         // Activate and set the sibling id for items in lstIndex
         for (int targetIndex = 0; targetIndex < lstIndex.Count; targetIndex++)
         {
             int itemIndex = lstIndex[targetIndex];

             // Ensure itemIndex is within bounds
             if (itemIndex >= 0 && itemIndex < lstItem.Count)
             {
                 var item = lstItem[itemIndex];
                 item.SetActive(true);
                 item.transform.SetSiblingIndex(targetIndex);
             }
         }*/
    }


    public async void ScrollFromEndToStart(float duration)
    {
        if (scrollRect == null)
        {
            Debug.LogWarning("ScrollRect is not assigned!");
            return;
        }

        // Dừng mọi tween trước đó trên scrollRect
        DOTween.Kill(scrollRect, true);

        // Đặt Scroll đến cuối
        scrollRect.verticalNormalizedPosition = 0;

        // Tạo tween để scroll về đầu
        await DOTween.To(
            () => scrollRect.verticalNormalizedPosition,
            value => scrollRect.verticalNormalizedPosition = value,
            1.0f, // Vị trí đầu (0: cuối, 1: đầu)
            duration
        ).SetEase(Ease.OutQuad) // Thêm easing nếu cần
        .SetId(scrollRect)     // Gán ID để dễ quản lý
        .AsyncWaitForCompletion();
    }


    public void DoKillNavigationAtHome()
    {
        rtfmShop.DOKill();
        navigationTween?.Kill();
    }

    public void ShowAbbreviated()
    {
        //   btnMoreOffer.gameObject.SetActive(true);
        ShowItems(lstIndexAbbreviatedGamePlay);
    }
    public void ShowAbbreviatedHeart()
    {
        // btnMoreOffer.gameObject.SetActive(true);

        ShowItems(lstIndexAbbreviatedHeartGamePlay);
    }

    public void ShowFullGameplay()
    {
        //   btnMoreOffer.gameObject.SetActive(false);
        ShowItems(lstIndexFullGamePlay);
    }
    public void OnClickShowPopup(UnityAction actionOnClose, bool canClose)
    {
        if (showing) return;
        UITopController.Instance.OnShowTabShop();
        ShopTabNavigation.Instance.ForceShow();
        TrackingController.Instance.TrackingOpenShop(IngameData.SHOP_PLACEMENT);
        btnClose.gameObject.SetActive(canClose);
        this.actionOnClose = actionOnClose;
        showing = true;
        ExpandBottom(canClose);
        tfmPopup.gameObject.SetActive(true);
        imgBGPanel.DOKill();

        imgBGPanel.DOFade(0.9f, 0.5f).OnComplete(() =>
        {
            tfmPopup.gameObject.SetActive(true);
            //  imgBGPanel.gameObject.SetActive(true);
        }
        );
    }

    public void ExpandBottom(bool isExpand)
    {
        if (isExpand)
        {
            imgBGPanel.gameObject.SetActive(true);
            rtfmShop.offsetMax = new Vector2(rtfmShop.offsetMax.x, -400);
            rtfmShop.offsetMin = new Vector2(rtfmShop.offsetMin.x, 0); // Set bottom position to 0
        }
        else
        {
            rtfmShop.offsetMax = new Vector2(rtfmShop.offsetMax.x, -400);
            rtfmShop.offsetMin = new Vector2(rtfmShop.offsetMin.x, 200); // Set bottom position to 250
        }
    }
    public void ShowAtHome(float width)
    {
        //ScrollFromEndToStart(0f);
        tfmPopup.gameObject.SetActive(true);
        showing = true;

        // imgBGPanel.gameObject.SetActive(true);
        rtfmShop.offsetMin = new Vector2(-width, rtfmShop.offsetMin.y);
        rtfmShop.offsetMax = new Vector2(-width, rtfmShop.offsetMax.y);
        navigationTween = DOVirtual.Float(-width, 0, 0.5f, value =>
        {
            rtfmShop.offsetMin = new Vector2(value, rtfmShop.offsetMin.y);
            rtfmShop.offsetMax = new Vector2(value, rtfmShop.offsetMax.y);
        });
        //rtfmShop.anchoredPosition = new Vector2(-width, 0);

        //rtfmShop.DOAnchorPosX(0, 0.5f);

        //    btnMoreOffer.gameObject.SetActive(false);
        ShowItems(lstIndexFullHome);
        navigationTween.OnComplete(() =>
        {
            navigationTween = null;
            showing = false;
        });
    }
    public async UniTaskVoid OnClickCloseAtHome(float width)
    {
        print("on click close shop tab at home");
        navigationTween?.Kill();
        showing = true;
        navigationTween = DOVirtual.Float(0, -width, 0.5f, value =>
        {
            rtfmShop.offsetMin = new Vector2(value, rtfmShop.offsetMin.y);
            rtfmShop.offsetMax = new Vector2(value, rtfmShop.offsetMax.y);
        }).OnComplete(() =>
        {
            // tfmPopup.gameObject.SetActive(false);
            // imgBGPanel.gameObject.SetActive(false);
            showing = false;

        });
        await navigationTween;
        //await rtfmShop.DOAnchorPosX(-width, 0.5f);

    }
    public async UniTaskVoid OnClickClosePopupAtHome()
    {
        OnClickClose();
    }

    public void OnClickClose()
    {
        showing = false;
        UITopController.Instance.OnClosePopupShop();
        if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
        {
            if (MainMenuBarController.Instance.CurrentIndex == 2)
                UITopController.Instance.OnShowTabHome();
        }

        imgBGPanel.DOFade(0f, 0.1f).OnComplete(() =>
        {
            tfmPopup.gameObject.SetActive(false);
            imgBGPanel.gameObject.SetActive(false);
            rtfmShop.offsetMin = new Vector2(0, rtfmShop.offsetMin.y);
            rtfmShop.offsetMax = new Vector2(0, rtfmShop.offsetMax.y);
            actionOnClose?.Invoke();
        });

        if (SceneController.Instance.CurrentScene == SceneType.GamePlayNewControl)
        {
            UITopController.Instance.OnShowPopupBooster();
        }
    }

    public static void GetCurrentTimeZoneOffset()
    {
        // Lấy múi giờ hiện tại
        TimeZoneInfo localZone = TimeZoneInfo.Local;

        // Lấy UTC offset của múi giờ hiện tại
        TimeSpan offset = localZone.GetUtcOffset(DateTime.Now);

        // Hiển thị thông tin múi giờ
        Debug.Log($"Tên múi giờ hiện tại: {localZone.DisplayName}");
        Debug.Log($"UTC offset hiện tại: {(offset >= TimeSpan.Zero ? "+" : "-")}{offset:hh\\:mm}");
    }

    private void Update()
    {
        var time = TimeGetter.Instance.CurrentTime;

        TimeZoneInfo localZone = TimeZoneInfo.Local;

        TimeSpan offset = localZone.GetUtcOffset(DateTime.Now);

        time += offset.Ticks;

        TimeSpan currentTimeUTC = new TimeSpan(time);
        if (currentTimeUTC.TotalDays > Db.storage.DAY_RESET)
        {
            Debug.Log("Reset");
            itemcoin.NextDay();
            itemAds.NextDay();
            Db.storage.DAY_RESET = currentTimeUTC.Ticks;
        }
    }

    public void ShowBeginner()
    {
        showing = true;

        CheckShowBundleBeginerPack();

        IngameData.SHOP_PLACEMENT = shop_placement.IconStarterPack;
        if (PopupController.Instance != null)
            PopupController.Instance.PopupCount++;
        //imgBGPanel.gameObject.SetActive(true);
        //imgBGPanel.DOFade(0.9f, 0.3f).OnComplete(() =>
        //{
        //    popupBeginnerPack.gameObject.SetActive(true);
        //});
        popupBeginnerPack.gameObject.SetActive(true);
        UITopController.Instance.OnShowPopupNoAds();
    }

    public void OnClickHideBeginner()
    {
        HideBeginner();
    }

    public void HideBeginner()
    {
        showing = false;

        popupBeginnerPack.gameObject.SetActive(false);

        //imgBGPanel.DOFade(0, 0.3f).OnComplete(() =>
        //{
        //    imgBGPanel.gameObject.SetActive(false);
        //    if (PopupController.Instance != null)
        //        PopupController.Instance.PopupCount--;
        //});
        if (PopupController.Instance != null)
            PopupController.Instance.PopupCount--;
        if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
        {
            UITopController.Instance.OnShowMainMenu();
        }
        else
        {
            UITopController.Instance.OnStartGameplay();
        }

        if (MainMenuEventManager.Instance != null)
        {
            if (BeginerPackService.IsActive() && !MainMenuEventManager.Instance.ButtonBeginner.activeSelf)
            {
                MainMenuEventManager.Instance.ButtonBeginner.SetActive(true);
                MainMenuEventManager.Instance.particleBeginner.Play();
            }

            MainMenuEventManager.Instance.CheckUnlockEvent();
        }
    }

    public void ShowNoAds()
    {
        showing = true;
        if (PopupController.Instance != null)
            PopupController.Instance.PopupCount++;
        imgBGPanel.gameObject.SetActive(true);
        imgBGPanel.DOFade(0.9f, 0.3f).OnComplete(() =>
        {
            popupNoAds.gameObject.SetActive(true);
        });
        IngameData.SHOP_PLACEMENT = shop_placement.IconNoAds;
        UITopController.Instance.OnShowPopupNoAds();
    }
    public void HideNoAds()
    {
        showing = false;

        popupNoAds.gameObject.SetActive(false);

        imgBGPanel.DOFade(0, 0.3f).OnComplete(() =>
        {
            if (PopupController.Instance != null)
                PopupController.Instance.PopupCount--;
            imgBGPanel.gameObject.SetActive(false);
        });
        if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
        {
            UITopController.Instance.OnShowMainMenu();
        }
    }
    public void RecheckUI()
    {
        popupNoAds.RecheckUI();
        shopNoAdsBundle.RecheckUI();
    }




    public async UniTask ShowCompletedPurchasePopup(List<ResourceIAP.ResourceValue> lstResource, UnityAction ActionAfterGetPurchase)
    {
        await popupCompletedPurchase.Show(lstResource);
        ActionAfterGetPurchase?.Invoke();
    }
    public Sprite GetSpriteResource(ResourceIAP.ResourceType type)
    {
        return resourceDataIAPSO.GetResourceData(type).icon;
    }

    public ItemIAPBundleBeginner bundleBeginner;
    public TMPro.TextMeshProUGUI txtCountDownBeginer;
    public TMPro.TextMeshProUGUI txtPopupCountDownBeginer;
    public GameObject countDownPopupBeginer;

    public void CheckShowBundleBeginerPack()
    {
        CheckShowBundleBeginner(BeginerPackService.IsActive());
        UpdateCountdown();
    }

    private void CheckShowBundleBeginner(bool isActive)
    {
        if (MainMenuEventManager.Instance != null)
        {
            MainMenuEventManager.Instance.ButtonBeginner.SetActive(isActive);
        }

        bundleBeginner.gameObject.SetActive(isActive);
    }

    private void UpdateCountdown()
    {
        if (!BeginerPackService.IsActive())
        {
            CheckShowBundleBeginner(false);
            return;
        }

        var data = Db.storage.BeginerPackData;
        string countDown = Utility.ClockDownTimeToString(data.endYear, data.endMonth, data.endDay, data.endHour, data.endMinute, 0);

        if (MainMenuEventManager.Instance != null)
        {
            MainMenuEventManager.Instance.txtCountDownBeginer.text = Utility.CountDownTimeToString(data.endYear, data.endMonth, data.endDay, data.endHour, data.endMinute, 0);
        }

        txtCountDownBeginer.text = countDown;
        txtPopupCountDownBeginer.text = countDown;

        if (string.IsNullOrEmpty(countDown))
        {
            CheckShowBundleBeginner(false);
            CancelInvoke(nameof(UpdateCountdown));
        }
        else
        {
            Invoke(nameof(UpdateCountdown), 1);
        }
    }
}
