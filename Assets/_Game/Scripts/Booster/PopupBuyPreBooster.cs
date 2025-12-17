using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameAnalyticsSDK;
using PS.Analytic;
using Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WeeklyQuest;

public class PopupBuyPreBooster : PopupBase
{
    [SerializeField] private Image imgBooster;
    [SerializeField] private Image imgTitle;
    [SerializeField] private TextMeshProUGUI txtContent;
    [SerializeField] private TextMeshProUGUI txtPrice;
    [SerializeField] private TextMeshProUGUI txtBoosterAmount;

    [SerializeField] private PreBoosterType preBoosterType;
    [SerializeField] private int price;
    [SerializeField] private bool bought;
    [SerializeField] private Transform tfmBtnClose, tfmContent, tfmImgBooster, tfmTxtContent;
    [SerializeField] private Transform tfmBtnCoin, tfmBtnAds;


    [InspectorName("IAP")]
    [SerializeField] private List<ResourceDataSO> lstDataBooster;
    [SerializeField] private Button btnCoin;
    [SerializeField] private List<PreBoosterBannerData> lstBoosterBanner;
    [SerializeField] private List<PreBoosterDataSO> lstPreBoosterData;
    [SerializeField] private GameObject btnRewardAd;
    private PreBoosterBannerData currentBanner;


    private bool isBought = false;

    public void ShowBoosterBanner()
    {
        var booster = lstBoosterBanner.Find(x => x.BoosterType == preBoosterType);

        Debug.Log("ShowBoosterBanner: " + booster);
        if (booster == null)
        {
            return;
        }
        Debug.Log("ShowBoosterBanner Found: " + booster.BoosterType);


        currentBanner = booster;

        booster.BillingBanner.Init();
        booster.rtfmBanner.gameObject.SetActive(true);
        booster.rtfmBanner.localScale = Vector3.zero;
        booster.rtfmBanner.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    [EasyButtons.Button]
    public override async UniTask Show()
    {
        Setup();
        InitUI();
        DOShow().Forget();
    }
    private int GetCost()
    {
        var remote = GameAnalyticController.Instance.Remote();
        var costInGame = remote.CostInGame;
        switch (preBoosterType)
        {
            case PreBoosterType.Rocket:
                return costInGame.coinBoosterRocket;
            case PreBoosterType.Glass:
                return costInGame.coinBoosterGlass;
            default:
                return 0;
        }
    }
    void InitUI()
    {
    }
    void Setup()
    {
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);

        tfmBtnClose.localScale = Vector3.zero;
        tfmContent.localScale = Vector3.zero;
        tfmContent.gameObject.SetActive(false);
        tfmImgBooster.localScale = Vector3.zero;
        tfmTxtContent.localScale = Vector3.zero;
        tfmBtnAds.localScale = Vector3.zero;
        tfmBtnCoin.localScale = Vector3.zero;

        var boosterData = lstPreBoosterData.Find(x => x.PreBoosterType == preBoosterType);
        imgBooster.sprite = boosterData.sprBooster;
        imgTitle.sprite = boosterData.title;
        txtContent.text = boosterData.content;

        var price = 900;
        if (GetCost() > 0)
            price = GetCost();
        this.price = price;
        txtPrice.text = $"{price}";
        txtBoosterAmount.text = $"X3";

        for (int i = 0; i < lstBoosterBanner.Count; i++)
        {
            var billingBanner = lstBoosterBanner[i].BillingBanner;
            var rtfm = lstBoosterBanner[i].rtfmBanner;
            rtfm.localScale = Vector3.zero;

           /* if (lstBoosterBanner[i].BoosterType == preBoosterType)
            {
                rtfm.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            }*/
        }
    }

    async UniTask DOShow()
    {
        tfmContent.gameObject.SetActive(true);
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.9f, 0.5f);
        await UniTask.Delay(200);
        await tfmContent.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        tfmImgBooster.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        tfmTxtContent.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(200);
        tfmBtnAds.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await tfmBtnCoin.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        tfmBtnAds.DOScale(1 ,0.3f).SetEase(Ease.OutBack);
        btnCoin.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        ShowBoosterBanner();

        UITopController.Instance.OnShowPopupBooster();

       await tfmBtnClose.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        isBusy = false;
    }

    public override void Hide()
    {
        DOHide().Forget();
    }

    async UniTask DOHide()
    {
        UITopController.Instance.OnHideCoin();
        UITopController.Instance.OnStartGameplay();

        if (currentBanner!=null)
        {
           currentBanner.rtfmBanner.DOScale(0, 0.3f).SetEase(Ease.InBack);
        }

        tfmBtnClose.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await UniTask.Delay(150);
        tfmBtnAds.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await tfmBtnCoin.DOScale(0, 0.3f).SetEase(Ease.InBack);
        tfmContent.DOScale(0, 0.3f).SetEase(Ease.InBack);
        tfmContent.gameObject.SetActive(false);
        await imgFade.DOFade(0f, 0.5f);
        imgFade.gameObject.SetActive(false);

        if (isBought)
        {
           // BoosterController.Instance.HighLightBooster(preBoosterType);
        }
    }

    public void ShowBooster(PreBoosterType preBoosterType)
    {
        this.preBoosterType = preBoosterType;
        Show();
        UITopController.Instance.OnShowPopupBooster();

        AudioController.Instance.PlaySound(SoundName.Popup);


/*        imgFade.gameObject.SetActive(true);
        imgContent.gameObject.SetActive(true);
        imgContent.transform.localScale = Vector3.zero;
        imgContent.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);*/
        bought = false;
        switch (preBoosterType)
        {
            case PreBoosterType.Rocket:
                imgBooster.sprite = lstPreBoosterData.Find(x => x.PreBoosterType == PreBoosterType.Rocket).sprBooster;
                break;
            case PreBoosterType.Glass:
                imgBooster.sprite = lstPreBoosterData.Find(x => x.PreBoosterType == PreBoosterType.Glass).sprBooster;
                break;
            default:
                break;
        }
    }

    private bool isBusy = false;
    public void OnClickBuyCoinBooster()
    {
        if (isBusy) return;

        int price = GetCost();
        var user = Db.storage.USER_INFO;
        if (Db.storage.USER_INFO.coin >= price)
        {
            isBusy = true;
            isBought = true;
            Db.storage.PreBoosterData.AddValue(preBoosterType, 3);
            user.coin -= price;
            SingularSDK.Event("COIN_SPEND");
            WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.UseCoins, price);

            Db.storage.USER_INFO = user;
            bought = true;
            EventDispatcher.Push(EventId.UpdateCoinUI, -price);
           // BoosterController.Instance.Init();
            /*   TrackingController.Instance.TrackingBooster(Db.storage.USER_INFO.level, IngameData.TRACKING_UN_SCREW_COUNT * 1.0f/
                   LevelController.Instance.Level.LstScrew.Count, "source", preBoosterType);*/

            int level = 0;
            float percentage = 0;
            PreBoosterController.Instance.OnBuy();
        }
        else
        {
            if (UITopController.Instance != null)
            {

                switch (preBoosterType)
                {
                    case PreBoosterType.Rocket:
                        IngameData.SHOP_PLACEMENT = shop_placement.PopupBuyRocket;
                        break;
                    case PreBoosterType.Glass:
                        IngameData.SHOP_PLACEMENT = shop_placement.PopupBuyGlass;
                        break;
                }

                UITopController.Instance.ShowShop();
            }
        }
    }

    public void OnClickBuyAdsBooster()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        AdsController.Instance.ShowRewardAds(RewardAdsPos.booster, () =>
        {
            isBought = true;
            Db.storage.PreBoosterData.AddValue(preBoosterType, 1);
            //BoosterController.Instance.Init();
            bought = true;
            PreBoosterController.Instance.OnBuy();

            /*
                        string boosterName = GetBoosterTrackingName();
                        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, boosterName, 1, "reward_ad", "PreBoosterPopup");*/
        }, null, null, PlacementByBooster());
    }
    public string PlacementByBooster()
    {
        string placement = "";
        /*switch (preBoosterType)
        {
            case BoosterType.AddHole:
                placement = AdsPlacement.reward_add_hole.ToString();
                break;
            case BoosterType.Hammer:
                placement = AdsPlacement.reward_hammer.ToString();

                break;
            case BoosterType.Clears:
                placement = AdsPlacement.reward_clear.ToString();

                break;
            case BoosterType.Magnet:
                placement = AdsPlacement.reward_magnet.ToString();

                break;
            case BoosterType.UnlockBox:
                placement = AdsPlacement.reward_unlock_box.ToString();

                break;
        }*/
        return placement;
    }
    public void OnClickClose()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        UITopController.Instance.OnHideCoin();
        Hide();


    }
    void Close()
    {
        /*     AudioController.Instance.PlaySound(SoundName.Close_Popup_Booster);
             PopupController.Instance.PopupCount--;
             imgFade.gameObject.SetActive(false);
             if (bought)
             {
                 BoosterController.Instance.HighLightBooster(preBoosterType);
                 UITopController.Instance.OnStartGameplay();

             }
             else
             {
                 ShopIAPController.Instance.ShowBeginner();
             }
             Hide();*/
    }
}

[Serializable]
public class PreBoosterBannerData
{
    public PreBoosterType BoosterType;
    public Sprite sprBooster;
    public IBillingBanner BillingBanner;
    public RectTransform rtfmBanner;
}

[System.Serializable]
public class PreBoosterDataSO
{
    public PreBoosterType PreBoosterType;
    public Sprite sprBooster;
    public Sprite title;
    public string content;
}