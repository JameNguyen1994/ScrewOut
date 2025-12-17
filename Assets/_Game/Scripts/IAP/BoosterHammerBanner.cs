using System.Collections.Generic;
using UnityEngine;

public class BoosterHammerBanner : BoosterBestValueBanner
{
    public override void Init()
    {
        base.Init();
        purchaseHandler = new BuyHammerHandler();
        txtBoosterAmount.text = $"X{itemData.data.Find(x => x.resourceType == ResourceType.HAMMER).value}";
        txtCoinAmount.text = $"{itemData.data.Find(x => x.resourceType == ResourceType.Coin).value}";
    }
    public override void OnBuyBtnClicked()
    {
        IngameData.SHOP_PLACEMENT = shop_placement.BoosterHammer;
        base.OnBuyBtnClicked();
    }
}
