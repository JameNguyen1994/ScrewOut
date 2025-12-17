using Cysharp.Threading.Tasks;
using PS.IAP;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemIAPNoAds : ItemIAPBase
{
    //[SerializeField] private Text txtAmount;
    //[SerializeField] private int value;
    [SerializeField] private Image imgCoinPack;

    public override async UniTask Init(object data, IPurchaseHandler purchaseHandler)
    {
        await base.Init(data, purchaseHandler);
        this.purchaseHandler = purchaseHandler;
        this.data = data;
        var coinData = (IAPItemData)data;

        productID = coinData.iapKey;
        //value = coinData.data[0].value;
        saleOffPercent = coinData.saleOffPercent;
        InitUI();
    }

    public override void InitUI()
    {
        base.InitUI();
        //txtAmount.text = $"x{value}";
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

    //    string GetCurrencyString(decimal value)
    //    {
    //        if (value % 1 == 0)
    //        {
    //            return value.ToString("C0", CultureInfo.CurrentCulture);
    //        }
    //
    //        if ((value * 10) % 1 == 0)
    //        {
    //            return value.ToString("C1", CultureInfo.CurrentCulture);
    //        }
    //        
    //        return value.ToString("C2", CultureInfo.CurrentCulture);
    //    }
}
