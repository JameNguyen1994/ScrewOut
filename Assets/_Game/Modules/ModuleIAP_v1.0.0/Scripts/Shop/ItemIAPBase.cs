using Cysharp.Threading.Tasks;
using PS.IAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemIAPBase : MonoBehaviour
{
    [Header("IAP")]
    [SerializeField] protected string productID;
    [SerializeField] protected object data;
    [SerializeField] protected Text txtPrice;
/*    [SerializeField] protected Text txtPriceFake;
    [SerializeField] protected GameObject gobjFakeValue;
    [SerializeField] protected Text txtSale;
    [SerializeField] protected GameObject gobjSale;*/
    [SerializeField] protected int saleOffPercent;
    [SerializeField] private string itemCategory;
    

    protected IPurchaseHandler purchaseHandler;

    public virtual async UniTask Init(object data, IPurchaseHandler purchaseHandler)
    {
        await UniTask.WaitUntil(() => InAppPurchase.Instance.IsInitialized() == true);
    }

    public virtual void InitUI()
    {
        var price = InAppPurchase.Instance.GetProductPriceString($"{productID}");
        if (price != null && price != string.Empty)
        {

            txtPrice.text = $"{price}";
          /*  if (saleOffPercent > 0)
            {
                gobjFakeValue?.SetActive(true);
                gobjSale?.SetActive(true);
                txtSale.text = $"{saleOffPercent}%\nSALE";
                //var priceFake = InAppPurchase.Instance.GetProductPriceString($"{fakeKey}");
                var priceFake = InAppPurchase.Instance.GetProductPrice(productID);
                var isoCurrency = InAppPurchase.Instance.GetProductCurrencyCode(productID);

                priceFake = (100 * priceFake) / (100 - this.saleOffPercent);

                txtPriceFake.text = $"{priceFake.GetCurrencyFromPriceAtCurrentCulture(isoCurrency)}";
            }
            else
            {
                gobjFakeValue?.SetActive(false);
                gobjSale?.SetActive(false);

            }*/
        }
        else
        {
         /*   gobjFakeValue?.SetActive(false);
            gobjSale?.SetActive(false);*/
            txtPrice.text = "SOLD OUT";
        }
    }

    public void OnItemClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        CoverBuyIAP.Instance?.OnEnableCover(true);

        Debug.Log("Click Item");
        InAppPurchase.Instance.OnPurchaseSuccess(OnPurchaseSuccess);
        InAppPurchase.Instance.BuyProduction($"{productID}", itemCategory, OnError);
        
        var name = InAppPurchase.Instance.GetProductName($"{productID}");
        name = productID;
        var price = InAppPurchase.Instance.GetProductPrice($"{productID}");
        var currency = InAppPurchase.Instance.GetProductCurrencyCode($"{productID}");
       
        // TrackingController.Instance.Tracking_IAP_CLICK(name,IngameData.SHOP_PLACEMENT,price, currency);
    }

    private void OnPurchaseSuccess(bool success)
    {
        CoverBuyIAP.Instance?.OnEnableCover(false);

        if (success)
        {
            AudioController.Instance.PlaySound(SoundName.IAP_Complete);
            var name = InAppPurchase.Instance.GetProductName($"{productID}");
            name = productID;

            var floatPrice =(float) InAppPurchase.Instance.GetProductPrice($"{productID}");
            var price = InAppPurchase.Instance.GetProductPrice($"{productID}").ToString();

            var currency = InAppPurchase.Instance.GetProductCurrencyCode($"{productID}");
           /* double amountInVND = CurrencyConverter.Instance.Convert(currency, "VND", floatPrice); // Quy đổi 100 USD sang VND
            Debug.Log($"100 USD = {amountInVND} VND");*/
            // TrackingController.Instance.Tracking_IAP(name, IngameData.SHOP_PLACEMENT, price, currency);
            purchaseHandler?.OnPurchaseSuccess(productID, data);
        }
        else
        {
            purchaseHandler?.OnPurchaseCancel(productID, data);
        }
    }

    private void OnError(string error)
    {
        CoverBuyIAP.Instance?.OnEnableCover(false);

        purchaseHandler?.OnPurchaseError(productID, error);
    }
}
