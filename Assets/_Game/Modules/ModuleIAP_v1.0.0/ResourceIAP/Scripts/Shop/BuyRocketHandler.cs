using Cysharp.Threading.Tasks;
using GameAnalyticsSDK;
using ResourceIAP;
using Storage;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyRocketHandler : IPurchaseHandler
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
        var coin = data.data.Find(x => x.resourceType == ResourceType.Coin).value;
        var rocketValue = data.data.Find(x => x.resourceType == ResourceType.ROCKET).value;
        var user = Db.storage.USER_INFO;
        user.coin += coin;
        Db.storage.USER_INFO = user;

        Debug.Log($"Get {coin} coin  productID: {productID}");
        Debug.Log($"Get {rocketValue} unlock box  productID: {productID}");

        Db.storage.PreBoosterData.AddValue(PreBoosterType.Rocket, rocketValue);
        var lstResource = new List<ResourceValue>();

        if (coin > 0)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coin", coin, "iap", productID);
            lstResource.Add(new ResourceIAP.ResourceValue()
            {
                type = ResourceIAP.ResourceType.Coin,
                value = coin
            });
        }

        if (rocketValue > 0)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Rocket", rocketValue, "iap", productID);
            lstResource.Add(new ResourceIAP.ResourceValue()
            {
                type = ResourceIAP.ResourceType.Rocket,
                value = rocketValue
            });
        }


        int level = 0;
        float percentage = 0;

        if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
        {
            level = Db.storage.USER_INFO.level;
            percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
        }

        TrackingController.Instance.TrackingInventory(level, percentage);
        await ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, null);
        PreBoosterController.Instance.OnBuy();

        EventDispatcher.Push(EventId.UpdateCoinUI
     , coin);
       // PopupController.Instance.OnDoneBuyBooster();
    }
    public void SetCoinDestination(Transform transform)
    {
        this.tfmCoin = transform;
    }
}
