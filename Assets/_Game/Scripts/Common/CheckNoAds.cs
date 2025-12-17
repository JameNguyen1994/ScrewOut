using Cysharp.Threading.Tasks;
using PS.Ad;
using PS.IAP;
using PS.Utils;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckNoAds : Singleton<CheckNoAds>
{
    [SerializeField] private ResourceDataSO shopNoAdsData;
    [SerializeField] private ResourceDataSO shopNoAdsWithComboData;
    [SerializeField] private ResourceDataSO shopBeginner;

    private void Start()
    {
        CheckIsNoAds();
    }
    public bool CheckIsNoAds()
    {
        Debug.Log("Check set noads");
        bool boughtNoAds = InAppPurchase.Instance.HasReceipt(shopNoAdsData.data[0].iapKey) ||
                           InAppPurchase.Instance.HasReceipt(shopNoAdsWithComboData.data[0].iapKey);
        //iconNoAds.SetActive(!boughtNoAds);
        if (boughtNoAds)
            ApplovinMaxController.Instance.SetIsNoAd(boughtNoAds);
        return boughtNoAds;
    }
}
