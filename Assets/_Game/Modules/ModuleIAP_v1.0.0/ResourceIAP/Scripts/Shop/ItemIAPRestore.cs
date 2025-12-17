using Cysharp.Threading.Tasks;
using PS.IAP;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemIAPRestore : ItemIAPBase
{
    [SerializeField] private Text txtAmount;
    // [SerializeField] private int value;
    // [SerializeField] private Image imgCoinPack;

    public override async UniTask Init(object data, IPurchaseHandler purchaseHandler)
    {
        await base.Init(data, purchaseHandler);
        this.purchaseHandler = purchaseHandler;
        this.data = data;
        var coinData = (IAPItemData)data;

        productID = coinData.iapKey;
        saleOffPercent = coinData.saleOffPercent;
        InitUI();
    }

    public override void InitUI()
    {
        base.InitUI();
    }
    public void SetImageCoinPack(Sprite sprite)
    {

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

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
