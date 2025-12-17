using Life;
using ResourceIAP;
using Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepUpOfferPurchaseHandler : IPurchaseHandler
{
    public void OnPurchaseSuccess(string productID, object data)
    {
        var starterData = (IAPItemData)data;


        int coinValue = 0;
        var addHoldValue = 0;
        var hammerValue = 0;
        var clearValue = 0;
        var unlockBoxValue = 0;
        var heartValue = 0;




        var drill = starterData.data.Find(x => x.resourceType == ResourceType.ADD_HOLE);

        if (drill != null)
        {
            Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.AddHole, drill.value);
            addHoldValue = drill.value;
        }

        var hammer = starterData.data.Find(x => x.resourceType == ResourceType.HAMMER);
        if (hammer != null)
        {
            Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Hammer, hammer.value);
            hammerValue = hammer.value;
        }

        var clear = starterData.data.Find(x => x.resourceType == ResourceType.CLEAR);
        if (clear != null)
        {
            Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Clears, clear.value);
            clearValue = clear.value;
        }

        var unlockBox = starterData.data.Find(x => x.resourceType == ResourceType.UNLOCK_BOX);
        if (unlockBox != null)
        {
            Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.UnlockBox, unlockBox.value);
            unlockBoxValue = unlockBox.value;
        }

        var coin = starterData.data.Find(x => x.resourceType == ResourceType.Coin);
        if (coin != null)
        {
            var userInfo = Db.storage.USER_INFO;
            userInfo.coin += coin.value;
            Db.storage.USER_INFO = userInfo;
            coinValue = coin.value;
            EventDispatcher.Push(EventId.UpdateCoinUI);
        }
        var heart = starterData.data.Find(x => x.resourceType == ResourceType.TIME_HEART);
        if (heart != null)
        {
            /*  var time = DBLifeController.Instance.LIFE_INFO;
              time.AddTimeInfinity(heart.value);*/
            heartValue = heart.value;
            LifeController.Instance.AddInfinityTime(heart.value);
        }

        var lstResource = new List<ResourceValue>();
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.Coin,
            value = coinValue
        });

        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.InfiniteLives,
            value = heartValue
        });
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.BoosterHammer,
            value = hammerValue
        });
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.BoosterAddHold,
            value = addHoldValue
        });
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.BoosterBloom,
            value = addHoldValue
        });
        lstResource.Add(new ResourceIAP.ResourceValue()
        {
            type = ResourceIAP.ResourceType.BoosterUnlockBox,
            value = unlockBoxValue
        });

        ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, null);


        int level = 0;
        float percentage = 0;

        if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
        {
            level = Db.storage.USER_INFO.level;
            percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
            BoosterController.Instance.Init();
        }

        TrackingController.Instance.TrackingInventory(level, percentage);
    }

    public void OnPurchaseCancel(string productID, object data)
    {

    }

    public void OnPurchaseError(string productID, string error)
    {

    }
}
