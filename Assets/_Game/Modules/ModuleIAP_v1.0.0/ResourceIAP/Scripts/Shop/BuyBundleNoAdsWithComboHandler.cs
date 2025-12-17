using Cysharp.Threading.Tasks;
using GameAnalyticsSDK;
using Life;
using MainMenuBar;
using PS.Ad;
using ResourceIAP;
using Storage;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyBundleNoAdsWithComboHandler : IPurchaseHandler
{
    Transform tfmCoin;
    public void OnPurchaseCancel(string productID, object data)
    {
        throw new System.NotImplementedException();
    }

    public void OnPurchaseError(string productID, string error)
    {
        
    }
    public void OnPurchaseSuccess(string productID, object data)
    {
        if (BoosterController.Instance != null)
            BoosterController.Instance.Init();



        var coinData = (IAPItemData)data;
        ActionAfterBuy(productID, coinData).Forget();

    }
    async UniTask ActionAfterBuy(string productID, IAPItemData data)
    {
        var coin = data.data.Find(x => x.resourceType == ResourceType.Coin).value;
        var addHold = data.data.Find(x => x.resourceType == ResourceType.ADD_HOLE).value;
        var hammer = data.data.Find(x => x.resourceType == ResourceType.HAMMER).value;
        var clear = data.data.Find(x => x.resourceType == ResourceType.CLEAR).value;
        // var magnet = coinData.data.Find(x => x.resourceType == ResourceType.MAGNET).value;
        var unlockBox = data.data.Find(x => x.resourceType == ResourceType.UNLOCK_BOX).value;

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

        ApplovinMaxController.Instance.SetIsNoAd(true);
        var lstResource = new List<ResourceValue>();
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.NoADs,
            value = 1
        });
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

        if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
        {
            ShopIAPController.Instance.HideNoAds();

            var storageRewardData = Db.storage.RewardData.DeepClone();
            storageRewardData.AddCoinValue(coin);
            storageRewardData.BoosterValue(BoosterType.AddHole, addHold);
            storageRewardData.BoosterValue(BoosterType.Hammer, hammer);
            storageRewardData.BoosterValue(BoosterType.Clears, clear);
            //  Db.storage.RewardData.BoosterValue(BoosterType.Magnet, magnet);
            storageRewardData.BoosterValue(BoosterType.UnlockBox, unlockBox);
            Db.storage.RewardData = storageRewardData;

            await ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, null);

            MainMenuRecieveRewardsHelper.Instance.OnGetReward();
        }
        else
        {
            ShopIAPController.Instance.HideNoAds();

            Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.AddHole, addHold);
            Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Hammer, hammer);
            Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Clears, clear);
            //  Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Magnet, magnet);
            Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.UnlockBox, unlockBox);


            var user = Db.storage.USER_INFO;
            user.coin += coin;
            Db.storage.USER_INFO = user;
            EventDispatcher.Push(EventId.UpdateCoinUI
           , coin);
            //EventDispatcher.Push(EventId.MakeCoinFly, tfmCoin.transform.position);
            await ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, null);
            ShopIAPController.Instance.OnClickClose();

            int level = 0;
            float percentage = 0;

            if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
            {
                level = Db.storage.USER_INFO.level;
                percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
            }

            TrackingController.Instance.TrackingInventory(level, percentage);
        }
        ShopIAPController.Instance.RecheckUI();
        ShopIAPController.Instance.HideNoAds();
        IngameData.BUY_NO_ADS = true;

        MainMenuBarController.Instance?.SetNoAds();
        if (AdsController.Instance != null)
        {
            AdsController.Instance.OnBuyNoAds();
        }
    }
    public void SetCoinDestination(Transform transform)
    {
        this.tfmCoin = transform;
    }
}
