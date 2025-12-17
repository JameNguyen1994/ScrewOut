using System;
using Cysharp.Threading.Tasks;
using PS.Ad;
using PS.IAP;
using System.Collections;
using System.Collections.Generic;
using PS.Analytic;
using Storage;
using UnityEngine;

public class PopupNoAds : MonoBehaviour
{
    [SerializeField] private ResourceDataSO shopNoAdsData1;
    [SerializeField] private ResourceDataSO shopNoAdsWithComboData1;
    [SerializeField] private ItemIAPNoAds itemBuyNoAds;
    [SerializeField] private ItemIAPNoAdsWithCombo itemBuyNoAdsWithCombo;
    [SerializeField] private int indexKey;
    [SerializeField] private GameObject goNoAdsNormal;
    [SerializeField] private GameObject goNoAdsWithCombo;

    [SerializeField] private string strURLTerms;
    [SerializeField] private string strURLPolicy;

    [SerializeField] string strURLTermsAndroid;
    [SerializeField] private string strURLPolicyAndroid;

    private async void Start()
    {
        //InitUI();
        //  await UniTask.WaitUntil(() => AmplitudeController.Instance.Remote().IsReadyRemote);
        //GetKeyByRemote();
    }
    public void GetKeyByRemote()
    {
        //indexKey = AmplitudeController.Instance.Remote().SegmentFlow.noads;
        indexKey = 0; // mac dinh lay goi shopNoAdsData1
    }

    private void OnEnable()
    {
        Init();
    }
    public void OnClickBuyItemNoADs()
    {
        if (IsShowNoAdsWithCombo())
        {
            itemBuyNoAdsWithCombo.OnItemClick();
        }
        else
        {
            itemBuyNoAds.OnItemClick();
        }
    }
    public async UniTask Init()
    {
        if (CheckNoAds.Instance.CheckIsNoAds())
        {
            this.gameObject.SetActive(false);
            return;
        }
        await UniTask.WaitUntil(() => GameAnalyticController.Instance.Remote().IsReadyRemote);
        GetKeyByRemote();
        Debug.Log("InitUI");
        var data = shopNoAdsData1.data;
        //indexNoadsCombo = 0: show no ads normal, indexNoadsCombo = 1: show no ads normal + variant A, indexNoadsCombo = 2: show no ads normal + variant B, indexNoadsCombo = 3: show no ads normal + variant C 
        Debug.Log($"Noads: {indexKey} - {data[0].iapKey}");

        goNoAdsNormal.gameObject.SetActive(false);
        goNoAdsWithCombo.gameObject.SetActive(false);
        Debug.Log("-----  IsShowNoAdsWithCombo :  " + IsShowNoAdsWithCombo() + " indexNoadsCombo:  ");
        if (IsShowNoAdsWithCombo())
        {
            var buyBundleNoAdsWithComboHandler = new BuyBundleNoAdsWithComboHandler();
            buyBundleNoAdsWithComboHandler.SetCoinDestination(itemBuyNoAdsWithCombo.TfmImagCoin());
            itemBuyNoAdsWithCombo.Init(shopNoAdsWithComboData1.data[0], buyBundleNoAdsWithComboHandler);
            goNoAdsWithCombo.gameObject.SetActive(true);
        }
        else
        {
            var buyItemCoinHandler = new BuyBundleNoAdsHandler();
            buyItemCoinHandler.SetCoinDestination(itemBuyNoAds.TfmImagCoin());
            itemBuyNoAds.Init(data[0], buyItemCoinHandler);
            goNoAdsNormal.gameObject.SetActive(true);
        }

    }
    public void RecheckUI()
    {
        itemBuyNoAds.RecheckUI();
        itemBuyNoAdsWithCombo.RecheckUI();
    }

    private bool IsShowNoAdsWithCombo()
    {
        var indexNoadsCombo = GameAnalyticController.Instance.Remote().CbsNoAd;
        return indexNoadsCombo == 2;
    }
    public void OnTermsClick()
    {
#if UNITY_IOS
        Application.OpenURL(strURLTerms);
#endif

#if UNITY_ANDROID
        Application.OpenURL(strURLTermsAndroid);
#endif
    }
    public void OnPolicyClick()
    {
#if UNITY_IOS
        Application.OpenURL(strURLPolicy);
#endif

#if UNITY_ANDROID
        Application.OpenURL(strURLPolicyAndroid);
#endif
    }
    public void OnRestorePurchaseClick()
    {
        InAppPurchase.Instance.RestorePurchase(CheckToShowAds);
    }
    public void CheckToShowAds()
    {
        RecheckUI();
    }
}
