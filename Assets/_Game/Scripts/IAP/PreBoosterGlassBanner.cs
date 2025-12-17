using System.Collections.Generic;
using UnityEngine;

public class PreBoosterGlassBanner : BoosterBestValueBanner
{
    public override void Init()
    {
        base.Init();
        purchaseHandler = new BuyGlassHandler();
        txtBoosterAmount.text = $"X{itemData.data.Find(x => x.resourceType == ResourceType.GLASS).value}";
        txtCoinAmount.text = $"{itemData.data.Find(x => x.resourceType == ResourceType.Coin).value}";
    }
    public override void OnBuyBtnClicked()
    {
        IngameData.SHOP_PLACEMENT = shop_placement.BoosterUnlockBox;
        base.OnBuyBtnClicked();
    }
}
