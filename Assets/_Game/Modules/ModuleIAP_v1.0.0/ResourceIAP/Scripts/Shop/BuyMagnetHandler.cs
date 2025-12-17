using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyMagnetHandler : IPurchaseHandler
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
        var coin = coinData.data.Find(x => x.resourceType == ResourceType.Coin).value;
        var addHold = coinData.data.Find(x => x.resourceType == ResourceType.MAGNET).value;
        var user = Db.storage.USER_INFO;
        user.coin += coin;
        Db.storage.USER_INFO = user;

        EventDispatcher.Push(EventId.UpdateCoinUI
            , coin);
        EventDispatcher.Push(EventId.MakeCoinFly, tfmCoin.transform.position);

        Debug.Log($"Get {coin} coin  productID: {productID}");

        Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Magnet, addHold);
        PopupController.Instance.OnDoneBuyBooster();

    }
    public void SetCoinDestination(Transform transform)
    {
        this.tfmCoin = transform;
    }
}
