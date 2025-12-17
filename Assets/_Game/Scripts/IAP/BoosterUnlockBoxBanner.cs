using System.Collections.Generic;
using UnityEngine;

public class BoosterUnlockBoxBanner : BoosterBestValueBanner
{
    public override void Init()
    {
        base.Init();
        purchaseHandler = new BuyUnlockBoxHandler();
        txtBoosterAmount.text = $"X{itemData.data.Find(x => x.resourceType == ResourceType.UNLOCK_BOX).value}";
        txtCoinAmount.text = $"{itemData.data.Find(x => x.resourceType == ResourceType.Coin).value}";
    }
    public override void OnBuyBtnClicked()
    {
        IngameData.SHOP_PLACEMENT = shop_placement.BoosterUnlockBox;
        base.OnBuyBtnClicked();
    }
}
