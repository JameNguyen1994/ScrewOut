using Cysharp.Threading.Tasks;
using Life;
using ResourceIAP;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyBundleHeartHandler : IPurchaseHandler
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
        if (BoosterController.Instance != null)
            BoosterController.Instance.Init();

        var coinData = (IAPItemData)data;
        ActionAfterBuy(productID, coinData);

    }
    async UniTask ActionAfterBuy(string productID, IAPItemData data)
    {
        var coin = data.data.Find(x => x.resourceType == ResourceType.Coin).value;
        var addHold = data.data.Find(x => x.resourceType == ResourceType.ADD_HOLE).value;
        var hammer = data.data.Find(x => x.resourceType == ResourceType.HAMMER).value;
        var clear = data.data.Find(x => x.resourceType == ResourceType.CLEAR).value;
        var magnet = data.data.Find(x => x.resourceType == ResourceType.MAGNET).value;
        var heart = data.data.Find(x => x.resourceType == ResourceType.TIME_HEART).value;
        var unlockBox = data.data.Find(x => x.resourceType == ResourceType.UNLOCK_BOX).value;



        Debug.Log($"Get {coin} coin  productID: {productID}");

        var lstResource = new List<ResourceValue>();
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.InfiniteLives,
            value = heart
        });
        if (coin > 0)
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
            var storageRewardData = Db.storage.RewardData.DeepClone();
            if (coin>0)
            storageRewardData.AddCoinValue(coin);
            storageRewardData.BoosterValue(BoosterType.AddHole, addHold);
            storageRewardData.BoosterValue(BoosterType.Hammer, hammer);
            storageRewardData.BoosterValue(BoosterType.Clears, clear);
            //  Db.storage.RewardData.BoosterValue(BoosterType.Magnet, magnet);
            storageRewardData.BoosterValue(BoosterType.UnlockBox, unlockBox);
            storageRewardData.AddHeartTimeValue(heart);
            Db.storage.RewardData = storageRewardData;

            await ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, null);



            LifeController.Instance.HidePopup();
            MainMenuRecieveRewardsHelper.Instance.OnGetReward();
        }
        else
        {
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


            DBLifeController.Instance.LIFE_INFO.AddTimeInfinity(heart);
            LifeController.Instance.UpdateInfo();


        }

    }


    public void SetCoinDestination(Transform transform)
    {
        this.tfmCoin = transform;
    }
}
