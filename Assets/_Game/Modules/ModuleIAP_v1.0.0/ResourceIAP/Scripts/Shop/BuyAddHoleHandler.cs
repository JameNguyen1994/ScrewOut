using Cysharp.Threading.Tasks;
using GameAnalyticsSDK;
using ResourceIAP;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyAddHoleHandler : IPurchaseHandler
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
        var addHole = data.data.Find(x => x.resourceType == ResourceType.ADD_HOLE).value;
        var user = Db.storage.USER_INFO;
        user.coin += coin;
        Db.storage.USER_INFO = user;

 
       // EventDispatcher.Push(EventId.MakeCoinFly, tfmCoin.transform.position);

        Debug.Log($"Get {coin} coin  productID: {productID}");
        Debug.Log($"Get {addHole} unlock box  productID: {productID}");

        Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.AddHole, addHole);
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

        if (addHole > 0)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "AddHole", addHole, "iap", productID);
            lstResource.Add(new ResourceIAP.ResourceValue()
            {
                type = ResourceIAP.ResourceType.BoosterAddHold,
                value = addHole
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
        EventDispatcher.Push(EventId.UpdateCoinUI
     , coin);
        PopupController.Instance.OnDoneBuyBooster();
    }
    public void SetCoinDestination(Transform transform)
    {
        this.tfmCoin = transform;
    }
}
