using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PS.Analytic;
using PS.IAP;
using Storage;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemIAPNoAdsWithCombo : ItemIAPBase
{
    [SerializeField] private Text txtCoin;
    [SerializeField] private int valueCoin;
    [SerializeField] private Text txtClear;
    [SerializeField] private int valueClear;
    [SerializeField] private Text txtHammer;
    [SerializeField] private int valueHammer;
    [SerializeField] private Text txtAddHole;
    [SerializeField] private int valueAddHole;
    [SerializeField] private Image imgCoinPack;
    [SerializeField] private int valueUnlockBox;
    [SerializeField] private Text txtUnlockBox;
    // Start is called before the first frame update
    private async void OnEnable()
    {/*
        if (CheckNoAds.Instance.CheckIsNoAds())
        {
            this.gameObject.SetActive(false);
            return;
        }*/
/*        await UniTask.WaitUntil(() => GameAnalyticController.Instance.Remote().IsReadyRemote);
        var indexNoadsCombo = GameAnalyticController.Instance.Remote().NoAdsWithCombo.noAdsWithCombo;
        var isShowNoAdsWithCombo = IsShowNoAdsWithCombo();
        this.gameObject.SetActive(false);
        if (isShowNoAdsWithCombo && indexNoadsCombo != 0)
        {
            this.gameObject.SetActive(true);
        }*/
    }
    private bool IsShowNoAdsWithCombo()
    {
        return Db.storage.USER_INFO.countInterAds >= 2;
    }
    public override async UniTask Init(object data, IPurchaseHandler purchaseHandler)
    {
        await base.Init(data, purchaseHandler);
        this.purchaseHandler = purchaseHandler;
        this.data = data;
        var coinData = (IAPItemData)data;

        productID = coinData.iapKey;
        valueCoin = coinData.data.Find(x => x.resourceType == ResourceType.Coin).value;

        valueAddHole = coinData.data.Find(x => x.resourceType == ResourceType.ADD_HOLE).value;
        valueHammer = coinData.data.Find(x => x.resourceType == ResourceType.HAMMER).value;
        valueClear = coinData.data.Find(x => x.resourceType == ResourceType.CLEAR).value;
        valueUnlockBox = coinData.data.Find(x => x.resourceType == ResourceType.UNLOCK_BOX).value;
        saleOffPercent = coinData.saleOffPercent;
        InitUI();
    }

    public override void InitUI()
    {
        base.InitUI();
        txtCoin.text = $"{valueCoin}";
        txtAddHole.text = $"x{valueAddHole}";
        txtHammer.text = $"x{valueHammer}";
        txtClear.text = $"x{valueClear}";
        txtUnlockBox.text = $"x{valueUnlockBox}";
    }
    public void RecheckUI()
    {
        bool bought = InAppPurchase.Instance.HasReceipt(productID) || CheckNoAds.Instance.CheckIsNoAds();
        if (bought)
            txtPrice.text = "Purchased";
    }
    public void SetImageCoinPack(Sprite sprite)
    {
        imgCoinPack.sprite = sprite;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
    public Transform TfmImagCoin() => imgCoinPack.transform;
}
