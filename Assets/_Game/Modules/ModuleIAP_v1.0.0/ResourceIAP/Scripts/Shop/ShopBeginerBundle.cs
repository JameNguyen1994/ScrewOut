using System;
using System.Collections;
using System.Collections.Generic;
using PS.Analytic;
using Storage;
using UnityEngine;

public class ShopBeginerBundle : MonoBehaviour
{
    [SerializeField] private ItemIAPBundleBeginner itemIAPBundleBeginner;
    [SerializeField] private ResourceDataSO shopCoinData;
    [SerializeField] private ResourceDataSO shopBeginSaleData1;
    [SerializeField] private ResourceDataSO shopBeginSaleData2;
    // Start is called before the first frame update
    public void OnEnable()
    {
        Init();
    }
    public void Init()
    {
        Debug.Log("InitUI");
        
        List<IAPItemData> data = new List<IAPItemData>();
        var buyItemCoinHandler = new BuyBundleHandlerBeginner();
        buyItemCoinHandler.SetCoinDestination(itemIAPBundleBeginner.TfmImagCoin());

        bool isShowSalePack = IsBeginSalePackExist();
        if (isShowSalePack)
        {
            var   indexKey = GameAnalyticController.Instance.Remote().BeginAds.beginAds;
            if (indexKey == 0)
            {
                data = shopCoinData.data;
            }
            else if (indexKey== 1)
            {
                data = shopBeginSaleData1.data;
            } 
            else if(indexKey==2)
            {
                data = shopBeginSaleData2.data;
            }
        }
        else
        {
            data = shopCoinData.data;
        }
        itemIAPBundleBeginner.Init(data[0], buyItemCoinHandler);

        CheckUI();
    }
    private bool IsBeginSalePackExist()
    {
        if(Db.storage == null) return false;
        var time = Db.storage.USER_INFO.beginSaleAdsValidUntil;
        return DateTime.UtcNow <= time;
    }

    public void CheckUI()
    {
        itemIAPBundleBeginner.gameObject.SetActive(BeginerPackService.IsActive());
    }
}
