using System.Collections.Generic;
using PS.IAP;
using TMPro;
using UnityEngine;

public class BoosterBestValueBanner : IBillingBanner
{
    [SerializeField] protected TextMeshProUGUI txtCoinAmount, txtBoosterAmount, txtPrice;
    [SerializeField] protected IAPItemData itemData;
    protected IPurchaseHandler purchaseHandler;

    public override void Init()
    {
        productId = itemData.iapKey;
        txtPrice.text = InAppPurchase.Instance.GetProductPriceString(productId);
    }

    public virtual void OnBuyBtnClicked()
    {
        CoverBuyIAP.Instance?.OnEnableCover(true);
        InAppPurchase.Instance.OnPurchaseSuccess(OnPurchaseSuccess);
        InAppPurchase.Instance.BuyProduction(productId, "Booster" ,OnPurchaseError);
    }

    private void OnPurchaseSuccess(bool success)
    {
        CoverBuyIAP.Instance?.OnEnableCover(false);
        if (!success)
        {
            purchaseHandler.OnPurchaseCancel(productId, itemData);
            return;
        }
        
        purchaseHandler.OnPurchaseSuccess(productId, itemData);
    }

    void OnPurchaseError(string error)
    {
        CoverBuyIAP.Instance?.OnEnableCover(false);
        purchaseHandler.OnPurchaseError(productId, error);
    }
}
