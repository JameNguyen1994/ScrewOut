using Cysharp.Threading.Tasks;
using GameAnalyticsSDK;
using Life;
using ResourceIAP;
using Storage;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;

public class BuyItemCoinHandler : IPurchaseHandler
{
    private Transform transformDestination;
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
        var coin = data.data[0].value;

        if (coin > 0)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coin", coin, "iap", productID);
        }

        var lstResource = new List<ResourceValue>();
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.Coin,
            value = coin
        });


        if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
        {
            var storageRewardData = Db.storage.RewardData.DeepClone();
            storageRewardData.AddCoinValue(coin);
            Db.storage.RewardData = storageRewardData;
            await ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, null);

            await MainMenuRecieveRewardsHelper.Instance.OnGetReward();
        }
        else
        {
            var user = Db.storage.USER_INFO;
            user.coin += coin;
            Db.storage.USER_INFO = user;
            await ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, null);

            EventDispatcher.Push(EventId.UpdateCoinUI
           , new UpdateCoinData()
           {
               coin = coin,
               coinMode = CoinMode.Plus
           });
            ShopIAPController.Instance.OnClickClose();
            // EventDispatcher.Push(EventId.MakeCoinFly, transformDestination.position);
        }

        Debug.Log($"Get {coin} coin  productID: {productID}");
    }
    public void SetTransform(Transform transformDestination)
    {
        this.transformDestination = transformDestination;
    }
}
