using Cysharp.Threading.Tasks;
using PS.IAP;
using System;
using System.Collections;
using System.Collections.Generic;
using PS.Analytic;
using Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemIAPBundleBeginner : ItemIAPBase
{
    [SerializeField] private Text txtAmountCoin;
    [SerializeField] private int valueCoin;
    [SerializeField] private Image imgCoinPack;

    [SerializeField] private Text txtAddHoleAmount;
    [SerializeField] private int valueAddHole;
    [SerializeField] private Text txtHammerAmount;
    [SerializeField] private int valueHammer;
    [SerializeField] private Text txtClearAmount;
    [SerializeField] private int valueClear;
    [SerializeField] private Text txtMagnetAmount;
    [SerializeField] private int valueMagnet;
    [SerializeField] private Text txtUnlockAmount;
    [SerializeField] private int valueUnlockBox;

    [SerializeField] private GameObject goHighLight;
    [SerializeField] private TextMeshProUGUI txtCountDown;
    [SerializeField] private GameObject goRoot;
    [SerializeField] private bool isPopup = false;
    private bool isRunning = false;
    private const int levelUnlock = 5;

    [SerializeField] private Text txtFakePrice;

    private void OnEnable()
    {
     /*   if (Db.storage.USER_INFO.level < levelUnlock + 1 && !isPopup)
        {
            goRoot.SetActive(false);
            return;
        }*/
    }
    public override async UniTask Init(object data, IPurchaseHandler purchaseHandler)
    {
/*        if (Db.storage.USER_INFO.level < levelUnlock + 1 && !isPopup)
        {
            goRoot.SetActive(false);
            return;
        }*/
        goRoot.SetActive(true);
        await base.Init(data, purchaseHandler);
        this.purchaseHandler = purchaseHandler;
        this.data = data;

        var starterData = (IAPItemData)data;
        productID = starterData.iapKey;

        valueCoin = starterData.data.Find(x => x.resourceType == ResourceType.Coin).value;

        valueAddHole = starterData.data.Find(x => x.resourceType == ResourceType.ADD_HOLE).value;
        valueHammer = starterData.data.Find(x => x.resourceType == ResourceType.HAMMER).value;
        valueClear = starterData.data.Find(x => x.resourceType == ResourceType.CLEAR).value;
        valueUnlockBox = starterData.data.Find(x => x.resourceType == ResourceType.UNLOCK_BOX).value;
        //valueMagnet = starterData.data.Find(x => x.resourceType == ResourceType.MAGNET).value;
        saleOffPercent = starterData.saleOffPercent;
        InitUI();
        
    }
    private void OnDisable()
    {
        isRunning = false;
    }
    public override void InitUI()
    {
        base.InitUI();
        txtAddHoleAmount.text = $"x{valueAddHole}";
        txtHammerAmount.text = $"x{valueHammer}";
        txtClearAmount.text = $"x{valueClear}";

        if (txtMagnetAmount != null)
        {
            txtMagnetAmount.text = $"x{valueMagnet}";
        }

        txtAmountCoin.text = $"{valueCoin}";
        txtUnlockAmount.text = $"x{valueUnlockBox}";

        if (txtFakePrice != null)
        {
            var priceFake = InAppPurchase.Instance.GetProductPrice(productID);
            var isoCurrency = InAppPurchase.Instance.GetProductCurrencyCode(productID);
            priceFake = (100 * priceFake) / (100 - 80);
            txtFakePrice.text = $"{priceFake.GetCurrencyFromPriceAtCurrentCulture(isoCurrency)}";
        }

        if (IsBeginSalePackExist())
        {
            var   indexKey = GameAnalyticController.Instance.Remote().BeginAds.beginAds;
            if (indexKey == 0)
            {
                txtCountDown.gameObject.SetActive(false);
                goHighLight.SetActive(false);
            }
            else if (indexKey == 1 || indexKey == 2)
            {
                txtCountDown.gameObject.SetActive(true);
                goHighLight.SetActive(true);
                StartCountdown().Forget();
            }
        }
        else
        {
           // txtCountDown.gameObject.SetActive(false);
           // goHighLight.SetActive(false);
        }
        /*        var timeSpan = TimeSpan.FromMilliseconds(valueMagnet);
                string detail;

                if (timeSpan.TotalHours < 100)
                {
                    // Hiển thị tổng số giờ, phút và giây
                    detail = $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
                else
                {
                    // Hiển thị ngày, giờ, phút và giây
                    detail = $"{timeSpan.Days:D2}:{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }

                txtMagnetAmount.text = $"{detail}";*/
    }
    private async UniTask StartCountdown()
    {
        isRunning = true;

        DateTime endTime = Db.storage.USER_INFO.beginSaleAdsValidUntil;

        while (isRunning)
        {
            int remaining = Mathf.Max(0, (int)(endTime - DateTime.UtcNow).TotalSeconds);

            if (remaining <= 0f)
            {
                this.gameObject.SetActive(false);
                break;
            }

            // Cập nhật UI nếu cần
            txtCountDown.text = Utilities.ConvertSecondToString(remaining);

            await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.DeltaTime, PlayerLoopTiming.Update);
        }
    }
    public Transform TfmImagCoin() => imgCoinPack.transform;
    private bool IsBeginSalePackExist()
    {
        var time = Db.storage.USER_INFO.beginSaleAdsValidUntil;
        return DateTime.UtcNow <= time;
    }
}
