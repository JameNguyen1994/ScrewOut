//using Beebyte.Obfuscator;

using Beebyte.Obfuscator;
using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace PS.IAP
{
    //[Skip]
    public class InAppPurchase : MonoBehaviour, IDetailedStoreListener
    {
        private static InAppPurchase _instance;
        public static InAppPurchase Instance => _instance;

        private static IStoreController storeController;
        private static IExtensionProvider storeExtensionProvider;
        private UnityAction<bool> onPurchaseComplete;

        [SerializeField] private bool dontDestroyOnLoad;
        [SerializeField] private ProductCatalog[] products;
        string itemCategory = "";

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }

                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public bool IsInitialized()
        {
            return storeController != null && storeExtensionProvider != null;
        }

        public ItemIAPType GetItemIAPType(string productId)
        {
            for (int i = 0; i < products.Length; i++)
            {
                if (products[i].productId == productId)
                {
                    return products[i].itemType;
                }
            }

            return ItemIAPType.Coin;
        }

        void Init()
        {
            if (IsInitialized())
            {
                print("Your IAP has initialized!");
                return;
            }

            print("IAP init again");
            var _builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            for (int i = 0; i < products.Length; i++)
            {
                var _product = products[i];
                _builder.AddProduct(_product.productId, _product.productType, new IDs()
                {
                    { _product.productId, GooglePlay.Name },
                    { _product.productId, AppleAppStore.Name }
                });
            }

            UnityPurchasing.Initialize(this, _builder);
        }

        public string GetProductPriceString(string productId)
        {
            if (!IsInitialized())
            {
                return "";
            }

            var products = storeController.products;
            return products.WithID(productId).metadata.localizedPriceString;
        }

        public decimal GetProductPrice(string productId)
        {
            if (!IsInitialized())
            {
                print("IAP is not initialized yet.");
                return 0;
            }

            return (decimal)storeController.products.WithID(productId).metadata.localizedPrice;
        }

        public string GetProductName(string productId)
        {
            if (!IsInitialized())
            {
                print("IAP is not initialized yet.");
                return "";
            }

            return storeController.products.WithID(productId).metadata.localizedTitle;
        }

        public string GetProductDescription(string productId)
        {
            if (!IsInitialized())
            {
                print("IAP is not initialized yet.");
                return "";
            }

            return storeController.products.WithID(productId).metadata.localizedDescription;
        }

        public string GetProductCurrencyCode(string productId)
        {
            if (!IsInitialized())
            {
                print("IAP is not initialized yet.");
                return "";
            }

            return storeController.products.WithID(productId).metadata.isoCurrencyCode;
        }

        Product FindProduct(string productId)
        {
            if (!IsInitialized())
            {
                print("IAP is not initialized yet.");
                return null;
            }

            return storeController.products.WithID(productId);
        }

        public InAppPurchase BuyProduction(string productId, string itemCategory,
            UnityAction<string> errorCallback = null)
        {
            this.itemCategory = itemCategory;

            if (!IsInitialized())
            {
                print("IAP is not initialized yet.");
                errorCallback?.Invoke("IAP is not initialized yet.");
                return this;
            }

            try
            {
                Product product = FindProduct(productId);

                if (product == null)
                {
                    print($"Can not find the product id: {productId}");
                    errorCallback?.Invoke($"Can not find the product id: {productId}");
                    return this;
                }

                if (product.availableToPurchase)
                {
                    print("start purchasing");

                    TrackingController.Instance.TrackingIAPClicked(productId, GetProductPrice(productId),
                        GetProductCurrencyCode(productId));
                    storeController.InitiatePurchase(product);
                }
                else
                {
                    print(
                        "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    errorCallback?.Invoke(
                        $"Product Id: {productId} either is not found or is not available for purchase");
                }
            }
            catch (Exception e)
            {
                print($"BuyProductID: FAIL. Exception during purchase:\n  {e}");
                errorCallback?.Invoke($"Exception error: {e}");
            }

            return this;
        }

        public void RestorePurchase(UnityAction onCompleted)
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                print("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                print("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = storeExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result, error) =>
                {
                    onCompleted?.Invoke();
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
                    //Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            else
            {
                print($"RestorePurchases FAIL. Not supported on this platform. Current = {Application.platform}");
            }
        }

        public bool HasReceipt(string productId)
        {
            if (!IsInitialized()) return false;
            var product = FindProduct(productId);

            if (product == null)
            {
                return false;
            }

            return product.hasReceipt;
        }

        public void OnPurchaseSuccess(UnityAction<bool> onComplete)
        {
            onPurchaseComplete = onComplete;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            print($"init IAP fail: {error}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log("ProcessPurchase");
            var product = purchaseEvent.purchasedProduct;

            bool validPurchase = true; // Presume valid for platforms with no R.V.

#if UNITY_ANDROID && !UNITY_EDITOR
            // Prepare the validator with the secrets we prepared in the Editor
            // obfuscation window.
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);

            try
            {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
                
                print($"===> number count: {result.Length}");
                
                foreach (IPurchaseReceipt productReceipt in result) {
                    if (productReceipt is GooglePlayReceipt google) {
                        // This is Google's Order ID.
                        // Note that it is null when testing in the sandbox
                        // because Google's sandbox does not provide Order IDs.
                        validPurchase =
 google.productID == product.definition.id && product.transactionID == google.purchaseToken;
                    }

                    if (productReceipt is AppleInAppPurchaseReceipt apple) {
                        validPurchase = apple.originalTransactionIdentifier == product.appleOriginalTransactionID && 
                                        apple.productID == product.definition.id;
                    }
                }
            }
            catch (IAPSecurityException)
            {
                Debug.Log("Invalid receipt, not unlocking content");
                validPurchase = false;
            }
#endif

            print($"======> is validPurchase: {validPurchase}");
            if (!validPurchase)
            {
                onPurchaseComplete?.Invoke(false);
                onPurchaseComplete = null;
                return PurchaseProcessingResult.Complete;
            }

            print($"====> onPurchaseEvent: {onPurchaseComplete?.Target}");

            onPurchaseComplete?.Invoke(true);
            onPurchaseComplete = null;
            var productId = product.definition.id;

            TrackingController.Instance.Tracking_IAP(productId, GetProductPrice(productId),
                GetProductCurrencyCode(productId), GetItemIAPType(productId));
            string receipt = product.receipt;
            string signature = "";
            //  TrackingController.Instance.Tracking_BUSINESS(productId, GetProductPrice(productId), GetProductCurrencyCode(productId),GetItemIAPType(productId), receipt, signature);

            print($"===========> Purchase Into Here!!!!!!");
#if UNITY_IOS
            if (product.appleProductIsRestored)
            {
                Debug.Log($"Product is restored {product.definition.id}");
                return PurchaseProcessingResult.Complete;
            }
#endif


#if UNITY_ANDROID && !UNITY_EDITOR
                        Receipt receiptAndroid = JsonUtility.FromJson<Receipt>(product.receipt);
                        PayloadAndroid receiptPayload = JsonUtility.FromJson<PayloadAndroid>(receiptAndroid.Payload);
                        GameAnalytics.NewBusinessEventGooglePlay(product.metadata.isoCurrencyCode, decimal.ToInt32(product.metadata.localizedPrice * 100), GetCategoryItem(product.definition.id), product.definition.id, $"{IngameData.SHOP_PLACEMENT}", receiptPayload.json, receiptPayload.signature);
#endif

#if UNITY_IOS && !UNITY_EDITOR
                        Receipt receiptiOS = JsonUtility.FromJson<Receipt>(product.receipt);
                        string receiptPayload = receiptiOS.Payload;
                        Debug.Log($"BUSINESS iOS :{product.metadata.isoCurrencyCode}, {decimal.ToInt32(product.metadata.localizedPrice * 100)}, { GetCategoryItem(product.definition.id)}, {product.definition.id}, {IngameData.SHOP_PLACEMENT}, {receiptPayload}");
                        GameAnalytics.NewBusinessEventIOS($"{product.metadata.isoCurrencyCode}", decimal.ToInt32(product.metadata.localizedPrice * 100), GetCategoryItem(product.definition.id), product.definition.id, $"{IngameData.SHOP_PLACEMENT}", receiptPayload);
#endif

#if UNITY_PURCHASING
            SingularSDK.InAppPurchase(product, null);
            FirebaseAnalyticController.TrackingIAP(product.definition.id, product.metadata.localizedPrice.ToString(), product.metadata.isoCurrencyCode);
#endif

            return PurchaseProcessingResult.Complete;
        }

        private string GetCategoryItem(string productId)
        {
            for (int i = 0; i < products.Length; i++)
            {
                if (products[i].productId == productId)
                {
                    return products[i].itemType.ToString();
                }
            }

            return ItemIAPType.Coin.ToString();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            print($"Purchase {product.definition.id} FAILED. Reason: \n {failureReason}");
            onPurchaseComplete?.Invoke(false);
            onPurchaseComplete = null;
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            storeExtensionProvider = extensions;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            print($"Purchase {product.definition.id} FAILED. Reason: \n {failureDescription.message}");
            onPurchaseComplete?.Invoke(false);
            onPurchaseComplete = null;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            print($"init IAP fail: {error}, messgae: {message}");
        }
    }

    [Serializable]
    public class ProductCatalog
    {
        public string productId;
        public ProductType productType;
        public ItemIAPType itemType;
    }

    [Serializable]
    public class Receipt
    {
        public string Store;
        public string TransactionID;
        public string Payload;

        public Receipt()
        {
            Store = TransactionID = Payload = "";
        }

        public Receipt(string store, string transactionID, string payload)
        {
            Store = store;
            TransactionID = transactionID;
            Payload = payload;
        }
    }

    [Serializable]
    public class PayloadAndroid
    {
        public string json;
        public string signature;

        public PayloadAndroid()
        {
            json = signature = "";
        }

        public PayloadAndroid(string _json, string _signature)
        {
            json = _json;
            signature = _signature;
        }
    }
}