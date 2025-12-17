using System.Collections.Generic;
using UnityEngine;

public class BoosterDrillBanner : BoosterBestValueBanner
{
    public override void Init()
    {
        base.Init();
        purchaseHandler = new BuyAddHoleHandler();
        txtBoosterAmount.text = $"X{itemData.data.Find(x => x.resourceType == ResourceType.ADD_HOLE).value}";
        txtCoinAmount.text = $"{itemData.data.Find(x => x.resourceType == ResourceType.Coin).value}";
    }

    public override void OnBuyBtnClicked()
    {
        IngameData.SHOP_PLACEMENT = shop_placement.BoosterAddHole;
        base.OnBuyBtnClicked();
    }

}
