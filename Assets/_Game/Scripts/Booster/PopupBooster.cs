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

public class PopupBooster : PopupBase
{
    [SerializeField] private Image imgBooster;
    [SerializeField] private Image imgTitle;
    [SerializeField] private TextMeshProUGUI txtContent;
    [SerializeField] private TextMeshProUGUI txtPrice;
    [SerializeField] private TextMeshProUGUI txtBoosterAmount;

    [SerializeField] private BoosterType boosterType;
    [SerializeField] private int price;
    [SerializeField] private bool bought;
    [SerializeField] private Transform tfmBtnClose, tfmContent, tfmImgBooster, tfmTxtContent;
    [SerializeField] private Transform tfmBtnCoin, tfmBtnAds;


    [InspectorName("IAP")]
    [SerializeField] private List<ResourceDataSO> lstDataBooster;
    [SerializeField] private List<Sprite> lstSprBooster;
    [SerializeField] private Image imgPack;
    [SerializeField] private Button btnCoin;
    [SerializeField] private List<BoosterBannerData> lstBoosterBanner;
    [SerializeField] private GameObject btnRewardAd;
    private IBillingBanner currentBanner;


    private bool isBought = false;

    public void ShowBoosterBanner()
    {
        var booster = lstBoosterBanner.Find(x => x.BoosterType == boosterType);

        if (booster == null)
        {
            return;
        }

        currentBanner = booster.BillingBanner;

        booster.BillingBanner.Init();
        booster.BillingBanner.gameObject.SetActive(true);
        booster.BillingBanner.GetComponent<Transform>().DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    public override void InitData(object data)
    {
        if (data is BoosterType boosterType)
        {
            this.boosterType = boosterType;
        }
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
        switch (boosterType)
        {
            case BoosterType.Hammer:
                return costInGame.coinBoosterHammer;
            case BoosterType.AddHole:
                return costInGame.coinBoosterAddHold;
            case BoosterType.Clears:
                return costInGame.coinBoosterClear;
            case BoosterType.UnlockBox:
                return costInGame.coinBoosterUnlockBox;
            default:
                return 0;
        }
    }
    void InitUI()
    {
        var remote = GameAnalyticController.Instance.Remote();
        if (boosterType == BoosterType.Hammer)
        {
            btnRewardAd.SetActive(remote.RewardControl.rewardBoosterAmountPerDay > Db.storage.REWARD_BOOSTER_HAMMER_COUNT);
        }

        else if (boosterType == BoosterType.AddHole)
        {
            btnRewardAd.SetActive(remote.RewardControl.rewardBoosterAmountPerDay > Db.storage.REWARD_BOOSTER_ADD_HOLE_COUNT);
        }
        else if (boosterType == BoosterType.Clears)
        {
            btnRewardAd.SetActive(remote.RewardControl.rewardBoosterAmountPerDay > Db.storage.REWARD_BOOSTER_BLOOM_COUNT);
        }
        else if (boosterType == BoosterType.UnlockBox)
        {
            btnRewardAd.SetActive(remote.RewardControl.rewardBoosterAmountPerDay > Db.storage.REWARD_BOOSTER_UNLOCK_BOX_COUNT);
        }
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

        var boosterData = BoosterDataHelper.Instance.GetBoosterData(boosterType);
        imgBooster.sprite = boosterData.sprBooster;
        imgTitle.sprite = boosterData.sprTitle;
        txtContent.text = boosterData.content;

        var price = Db.storage.BOOSTER_DATAS.PriceBooster(boosterType);
        price = GetCost();
        this.price = price;
        txtPrice.text = $"{price}";
        txtBoosterAmount.text = $"X3";

        for (int i = 0; i < lstBoosterBanner.Count; i++)
        {
            var billingBanner = lstBoosterBanner[i].BillingBanner;
            billingBanner.GetComponent<Transform>().localScale = Vector3.zero;
            billingBanner.gameObject.SetActive(false);

            if (lstBoosterBanner[i].BoosterType == boosterType)
            {
                imgBooster.sprite = boosterData.sprBooster;
            }
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
        tfmBtnClose.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        ShowBoosterBanner();

        UITopController.Instance.OnShowPopupBooster();

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

        if (currentBanner)
        {
            currentBanner.GetComponent<Transform>().DOScale(0, 0.3f).SetEase(Ease.InBack);
        }

        tfmBtnClose.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await UniTask.Delay(150);
        tfmBtnAds.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await tfmBtnCoin.DOScale(0, 0.3f).SetEase(Ease.InBack);
        tfmContent.DOScale(0, 0.3f).SetEase(Ease.InBack);
        tfmContent.gameObject.SetActive(false);
        await imgFade.DOFade(0f, 0.5f);
        imgFade.gameObject.SetActive(false);
        PopupController.Instance.PopupCount--;

        if (isBought)
        {
            BoosterController.Instance.HighLightBooster(boosterType);
        }
    }

    public void ShowBooster(BoosterType boosterType)
    {
        UITopController.Instance.OnShowPopupBooster();

        AudioController.Instance.PlaySound(SoundName.Popup);


        imgFade.gameObject.SetActive(true);
        bought = false;
        this.boosterType = boosterType;


        if (GameConfig.OLD_VERSION)
        {
            btnCoin.gameObject.SetActive(false);
            return;
        }
        switch (boosterType)
        {
            case BoosterType.Hammer:
                Db.storage.REWARD_BOOSTER_HAMMER_COUNT++;
                break;
            case BoosterType.AddHole:
                Db.storage.REWARD_BOOSTER_ADD_HOLE_COUNT++;
                break;
            case BoosterType.Clears:
                Db.storage.REWARD_BOOSTER_BLOOM_COUNT++;
                break;
            case BoosterType.UnlockBox:
                Db.storage.REWARD_BOOSTER_UNLOCK_BOX_COUNT++;
                break;
        }




        // switch (preBoosterType)
        // {
        //     case BoosterType.UnlockBox:
        //         //itemIAPAddHold.gameObject.SetActive(true);
        //         IngameData.SHOP_PLACEMENT = shop_placement.booster_unlock_box;
        //
        //         // imgPack.sprite = lstSprBooster[0];
        //         break;
        //     case BoosterType.AddHole:
        //         itemIAPAddHold.gameObject.SetActive(true);
        //         IngameData.SHOP_PLACEMENT = shop_placement.booster_add_hole;
        //
        //         // imgPack.sprite = lstSprBooster[0];
        //         break;
        //     case BoosterType.Hammer:
        //         itemIAPBundleHammer.gameObject.SetActive(true);
        //         IngameData.SHOP_PLACEMENT = shop_placement.booster_hammer;
        //
        //         // imgPack.sprite = lstSprBooster[1];
        //
        //
        //         break;
        //     case BoosterType.Clears:
        //         itemIAPBundleClear.gameObject.SetActive(true);
        //         IngameData.SHOP_PLACEMENT = shop_placement.booster_clear;
        //
        //         // imgPack.sprite = lstSprBooster[2];
        //
        //         break;
        //     case BoosterType.Magnet:
        //         itemIAPBundleMagnet.gameObject.SetActive(true);
        //         IngameData.SHOP_PLACEMENT = shop_placement.booster_magnet;
        //
        //         //imgPack.sprite = lstSprBooster[3];
        //
        //         break;
        // }

    }

    private bool isBusy = false;

    public void OnClickBuyCoinBooster()
    {
        if (isBusy) return;

        int price = Db.storage.BOOSTER_DATAS.PriceBooster(boosterType);
        price = GetCost();
        var user = Db.storage.USER_INFO;
        if (Db.storage.USER_INFO.coin >= price)
        {
            isBusy = true;
            isBought = true;
            Db.storage.BOOSTER_DATAS.AddBooster(boosterType, 3);
            user.coin -= price;
            SingularSDK.Event("COIN_SPEND");
            WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.UseCoins, price);

            Db.storage.USER_INFO = user;
            bought = true;
            EventDispatcher.Push(EventId.UpdateCoinUI, -price);
            BoosterController.Instance.Init();
            /*   TrackingController.Instance.TrackingBooster(Db.storage.USER_INFO.level, IngameData.TRACKING_UN_SCREW_COUNT * 1.0f/
                   LevelController.Instance.Level.LstScrew.Count, "source", preBoosterType);*/

            int level = 0;
            float percentage = 0;

            if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
            {
                level = Db.storage.USER_INFO.level;
                percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
            }

            TrackingController.Instance.TrackingInventory(level, percentage);

            Hide();


            string boosterName = GetBoosterTrackingName();
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, boosterName, 3, "coin", "BoosterPopup");
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Coin", price, "boosters", "BoosterPopup");
        }
        else
        {
            if (UITopController.Instance != null)
            {

                switch (boosterType)
                {
                    case BoosterType.AddHole:
                        IngameData.SHOP_PLACEMENT = shop_placement.BoosterAddHole;
                        break;
                    case BoosterType.Hammer:
                        IngameData.SHOP_PLACEMENT = shop_placement.BoosterHammer;
                        break;
                    case BoosterType.Clears:
                        IngameData.SHOP_PLACEMENT = shop_placement.BoosterClear;
                        break;
                    case BoosterType.UnlockBox:
                        IngameData.SHOP_PLACEMENT = shop_placement.BoosterUnlockBox;
                        break;
                }

                UITopController.Instance.ShowShop();
            }
        }
    }

    string GetBoosterTrackingName()
    {
        string boosterName = "";

        switch (boosterType)
        {
            case BoosterType.AddHole:
                boosterName = "AddHole";
                break;
            case BoosterType.Hammer:
                boosterName = "Hammer";
                break;
            case BoosterType.Clears:
                boosterName = "Clear";
                break;
            case BoosterType.UnlockBox:
                boosterName = "UnlockBox";
                break;
        }

        return boosterName;
    }

    public void OnClickBuyAdsBooster()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        AdsController.Instance.ShowRewardAds(RewardAdsPos.booster, () =>
        {
            isBought = true;
            Db.storage.BOOSTER_DATAS.AddBooster(boosterType, 1);
            BoosterController.Instance.Init();
            bought = true;
            /*   TrackingController.Instance.TrackingBooster(Db.storage.USER_INFO.level, IngameData.TRACKING_UN_SCREW_COUNT * 1.0f/
                   LevelController.Instance.Level.LstScrew.Count, "source", preBoosterType);*/
            Hide();

            string boosterName = GetBoosterTrackingName();
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, boosterName, 1, "reward_ad", "BoosterPopup");
            UpdateRewardByADSBooster();
        }, null, null, PlacementByBooster());
    }

    private void UpdateRewardByADSBooster()
    {
        switch (boosterType)
        {
            case BoosterType.Hammer:
                Db.storage.REWARD_BOOSTER_HAMMER_COUNT++;
                break;
            case BoosterType.AddHole:
                Db.storage.REWARD_BOOSTER_ADD_HOLE_COUNT++;
                break;
            case BoosterType.Clears:
                Db.storage.REWARD_BOOSTER_BLOOM_COUNT++;
                break;
            case BoosterType.UnlockBox:
                Db.storage.REWARD_BOOSTER_UNLOCK_BOX_COUNT++;
                break;
        }
    }

    public string PlacementByBooster()
    {
        string placement = "";
        switch (boosterType)
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
        }
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
        AudioController.Instance.PlaySound(SoundName.Close_Popup_Booster);
        PopupController.Instance.PopupCount--;
        imgFade.gameObject.SetActive(false);
        if (bought)
        {
            BoosterController.Instance.HighLightBooster(boosterType);
            UITopController.Instance.OnStartGameplay();

        }
        else
        {
            ShopIAPController.Instance.ShowBeginner();
        }
        Hide();
    }
    public void OnDoneBuyBooster()
    {
        BoosterController.Instance.Init();
        bought = true;
        Close();
    }
}

[Serializable]
public class BoosterBannerData
{
    public BoosterType BoosterType;
    public Sprite sprBooster;
    public IBillingBanner BillingBanner;
}
