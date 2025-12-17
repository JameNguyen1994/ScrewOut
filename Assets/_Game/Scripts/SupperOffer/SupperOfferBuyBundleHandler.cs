using Cysharp.Threading.Tasks;
using GameAnalyticsSDK;
using Storage;
using System.Collections.Generic;

public class SupperOfferBuyBundleHandler : IPurchaseHandler
{
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

    private async UniTask ActionAfterBuy(string productID, IAPItemData data)
    {
        var coin = data.data.Find(x => x.resourceType == ResourceType.Coin).value;
        var addHold = data.data.Find(x => x.resourceType == ResourceType.ADD_HOLE).value;
        var hammer = data.data.Find(x => x.resourceType == ResourceType.HAMMER).value;
        var clear = data.data.Find(x => x.resourceType == ResourceType.CLEAR).value;
        var revive = data.data.Find(x => x.resourceType == ResourceType.FREE_REVIVE).value;
        var unlockBox = data.data.Find(x => x.resourceType == ResourceType.UNLOCK_BOX).value;

        if (revive > 0)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "FreeRevive", revive, "iap", productID);
        }

        if (coin > 0)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coin", coin, "iap", productID);
        }

        if (addHold > 0)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "AddHole", addHold, "iap", productID);
        }

        if (hammer > 0)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Hammer", hammer, "iap", productID);
        }

        if (clear > 0)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Clear", clear, "iap", productID);
        }

        if (unlockBox > 0)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "UnlockBox", unlockBox, "iap", productID);
        }

        var lstResource = new List<ResourceIAP.ResourceValue>();

        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.Coin,
            value = coin
        });
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.BoosterHammer,
            value = hammer
        });
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.BoosterAddHold,
            value = addHold
        });
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.BoosterBloom,
            value = clear
        });
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.BoosterUnlockBox,
            value = unlockBox
        });
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.FreeRevive,
            value = revive
        });

        SupperOfferService.SetBuy();

        if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
        {
            var storageRewardData = Db.storage.RewardIAPData.DeepClone();

            storageRewardData.AddCoinValue(coin);
            storageRewardData.BoosterValue(BoosterType.AddHole, addHold);
            storageRewardData.BoosterValue(BoosterType.Hammer, hammer);
            storageRewardData.BoosterValue(BoosterType.Clears, clear);
            storageRewardData.BoosterValue(BoosterType.UnlockBox, unlockBox);
            storageRewardData.BoosterValue(BoosterType.FreeRevive, revive);

            Db.storage.RewardIAPData = storageRewardData;

            await ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, null);
            PopupSupperOffer.Instance.Hide();
            await MainMenuRecieveRewardsHelper.Instance.OnGetRewardIAP();

            PopupSupperOffer.Instance.CheckShowButtonHappyShop();
        }
    }
}