using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PS.Analytic;
using Storage;
using UnityEngine;

public class ShopNoAdsBundle : MonoBehaviour
{
    [SerializeField] private ResourceDataSO shopNoAdsNormalData;
    [SerializeField] private ResourceDataSO shopNoAdsWithComboData1;
/*    [SerializeField] private ResourceDataSO shopNoAdsWithComboData2;
    [SerializeField] private ResourceDataSO shopNoAdsWithComboData3;*/

    [SerializeField] private ItemIAPNoAds itemBuyNoAds;
    [SerializeField] private ItemIAPNoAdsWithCombo itemBuyNoAdsWithCombo;
    [SerializeField] private GameObject noadsPanel;
    // Start is called before the first frame update
    void OnEnable()
    {
        InitUI().Forget();
    }

    private async UniTask InitUI()
    {
        Debug.Log("InitUI No Ads Bundle UI 1");
        if (CheckNoAds.Instance.CheckIsNoAds() || !MainMenuService.IsUnlockNoADS())
        {
            Debug.Log("No Ads already purchased, hiding no ads items.");
            itemBuyNoAdsWithCombo.gameObject.SetActive(false);
            itemBuyNoAds.gameObject.SetActive(false);
            noadsPanel.SetActive(false);
            return;
        }
       // await UniTask.WaitUntil(() => GameAnalyticController.Instance.Remote().IsReadyRemote);
        Debug.Log("InitUI No Ads Bundle UI 2");

        var indexNoadsCombo = GameAnalyticController.Instance.Remote().CbsNoAd;
        itemBuyNoAdsWithCombo.gameObject.SetActive(false);
        itemBuyNoAds.gameObject.SetActive(false);
        noadsPanel.SetActive(false);
        if (indexNoadsCombo != 0)
        {

            //if (indexNoadsCombo == 1)
            {
                var buyBundleNoAdshandler = new BuyBundleNoAdsHandler();
                buyBundleNoAdshandler.SetCoinDestination(itemBuyNoAds.TfmImagCoin());
                itemBuyNoAds.Init(shopNoAdsNormalData.data[0], buyBundleNoAdshandler);
                itemBuyNoAds.gameObject.SetActive(true);
            }
           // else
            {
                var buyBundleNoAdsWithComboHandler = new BuyBundleNoAdsWithComboHandler();
                buyBundleNoAdsWithComboHandler.SetCoinDestination(itemBuyNoAdsWithCombo.TfmImagCoin());
                itemBuyNoAdsWithCombo.Init(shopNoAdsWithComboData1.data[0], buyBundleNoAdsWithComboHandler);
                itemBuyNoAdsWithCombo.gameObject.SetActive(true);
            }    
            noadsPanel.SetActive(true);
        }
    }

    private bool IsShowNoAdsWithCombo()
    {
        return Db.storage.USER_INFO.countInterAds >= 2;
    }
    public void RecheckUI()
    {
        itemBuyNoAds.RecheckUI();
        itemBuyNoAdsWithCombo.RecheckUI();
    }
}
