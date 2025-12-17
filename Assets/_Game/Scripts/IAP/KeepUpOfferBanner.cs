using System.Collections.Generic;
using PS.IAP;
using TMPro;
using UnityEngine;

public class KeepUpOfferBanner : IBillingBanner
{
    [SerializeField] private TextMeshProUGUI txtCoin, txtHammer, txtDrill, txtUnlockBox, txtClear;
    [SerializeField] private TextMeshProUGUI txtPrice;
    [SerializeField]  private IAPItemData itemData;
    private IPurchaseHandler purchaseHandler;

    public override void Init()
    {
        string priceTxt = InAppPurchase.Instance.GetProductPriceString(productId);
        txtPrice.text = priceTxt;

        purchaseHandler = new KeepUpOfferPurchaseHandler();
        itemData = new IAPItemData()
        {
            iapKey = productId,
            data = new List<ResourceData>
            {
                new ResourceData { value = 1500, resourceType = ResourceType.Coin },
                new ResourceData { value = 1, resourceType = ResourceType.HAMMER },
                new ResourceData { value = 4*60*60*1000, resourceType = ResourceType.TIME_HEART },
                new ResourceData { value = 1, resourceType = ResourceType.ADD_HOLE },
                new ResourceData { value = 1, resourceType = ResourceType.UNLOCK_BOX },
                new ResourceData { value = 1, resourceType = ResourceType.CLEAR }
            },
            saleOffPercent = 0
        };

        var hammer = itemData.data.Find(x => x.resourceType == ResourceType.HAMMER);

        if (hammer != null)
        {
            txtHammer.text = $"X{hammer.value}";
        }
        
        var drill = itemData.data.Find(x => x.resourceType == ResourceType.ADD_HOLE);
        if (drill != null)
        {
            txtDrill.text = $"X{drill.value}";
        }
        
        var unlockBox = itemData.data.Find(x => x.resourceType == ResourceType.UNLOCK_BOX);
        if (unlockBox != null)
        {
            txtUnlockBox.text = $"X{unlockBox.value}";
        }
        
        var clear = itemData.data.Find(x => x.resourceType == ResourceType.CLEAR);
        if (clear != null)
        {
            txtClear.text = $"X{clear.value}";
        }
        
        var coin = itemData.data.Find(x => x.resourceType == ResourceType.Coin);
        if (coin != null)
        {
            txtCoin.text = $"{coin.value}";
        }
    }
    
    public void OnBuyBtnClicked()
    {
        CoverBuyIAP.Instance?.OnEnableCover(true);
        InAppPurchase.Instance.OnPurchaseSuccess(OnPurchaseSuccess);
        InAppPurchase.Instance.BuyProduction(productId, "KeepUpOffer" ,OnPurchaseError);
    }

    private void OnPurchaseSuccess(bool success)
    {
        CoverBuyIAP.Instance?.OnEnableCover(false);
        if (!success)
        {
            return;
        }
        
        purchaseHandler.OnPurchaseSuccess(productId, itemData);
    }

    void OnPurchaseError(string error)
    {
        CoverBuyIAP.Instance?.OnEnableCover(false);
    }
}
