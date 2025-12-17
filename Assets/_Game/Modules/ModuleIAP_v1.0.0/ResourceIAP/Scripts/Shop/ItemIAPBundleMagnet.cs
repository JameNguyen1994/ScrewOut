using Cysharp.Threading.Tasks;
using PS.IAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIAPBundleMagnet : ItemIAPBase
{
    [SerializeField] private Text txtAmountCoin;
    [SerializeField] private int valueCoin;
    [SerializeField] private Image imgCoinPack;

    [SerializeField] private Text txtAmount;
    [SerializeField] private int value;
    public override async UniTask Init(object data, IPurchaseHandler purchaseHandler)
    {
        await base.Init(data, purchaseHandler);
        this.purchaseHandler = purchaseHandler;
        this.data = data;

        var starterData = (IAPItemData)data;
        productID = starterData.iapKey;

        valueCoin = starterData.data.Find(x => x.resourceType == ResourceType.Coin).value;

        value = starterData.data.Find(x => x.resourceType == ResourceType.MAGNET).value;
        saleOffPercent = starterData.saleOffPercent;
        InitUI();
    }

    public override void InitUI()
    {
        base.InitUI();
        txtAmount.text = $"x{value}";
        txtAmountCoin.text = $"x{valueCoin}";

    }
    public Transform TfmImagCoin() => imgCoinPack.transform;

}
