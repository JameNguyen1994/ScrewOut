using Cysharp.Threading.Tasks;
using MainMenuBar;
using PS.Ad;
using ResourceIAP;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyBundleNoAdsHandler : IPurchaseHandler
{
    Transform tfmCoin;
    public void OnPurchaseCancel(string productID, object data)
    {

    }

    public void OnPurchaseError(string productID, string error)
    {
    }

    public void OnPurchaseSuccess(string productID, object data)
    {
        var coinData = (IAPItemData)data;
        ActionAfterBuy(productID, coinData).Forget();
    }
    async UniTask ActionAfterBuy(string productID, IAPItemData data)
    {
        ApplovinMaxController.Instance.SetIsNoAd(true);
        ShopIAPController.Instance.RecheckUI();
        IngameData.BUY_NO_ADS = true;
        var lstResource = new List<ResourceValue>();
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.NoADs,
            value = 1
        });
        if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
        {
            ShopIAPController.Instance.HideNoAds();
            await ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, null);
            MainMenuRecieveRewardsHelper.Instance.OnGetReward();
            MainMenuBarController.Instance.SetNoAds();
        }
        else
        {
            ShopIAPController.Instance.HideNoAds();

            await ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, null);
            ShopIAPController.Instance.OnClickClose();
        }
        if (AdsController.Instance!=null)
        {
            AdsController.Instance.OnBuyNoAds();
        }
    }
    public void SetCoinDestination(Transform transform)
    {
        this.tfmCoin = transform;
    }
}
