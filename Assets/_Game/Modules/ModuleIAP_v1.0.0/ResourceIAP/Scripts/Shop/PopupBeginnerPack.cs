using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameAnalyticsSDK.Setup;
using PS.Analytic;
using Storage;
using TMPro;
using UnityEngine;

public class PopupBeginnerPack : MonoBehaviour
{
    [SerializeField] private ResourceDataSO shopCoinData;
    [SerializeField] private ItemIAPBundleBeginner itemIAPBundleBeginner;
    [SerializeField] private GameObject goTagSale;
    [SerializeField] private GameObject goTagLimited;
   // [SerializeField] private TextMeshProUGUI txtCountDown;
    [SerializeField] private Transform middleCoin;
    [SerializeField] private Transform itemCoin;

    private Vector3 initCoinPos;
    private bool isRunning = false;
    private void Awake()
    {
        //InitUI();
        initCoinPos = itemCoin.transform.localPosition;
    }
    public void OnEnable()
    {
        Init();
    }
    private void OnDisable()
    {
        isRunning = false;
    }
    public void Init()
    {
        Debug.Log("InitUI");
        goTagSale.SetActive(false);
        goTagLimited.SetActive(false);
       // txtCountDown.gameObject.SetActive(false);
        itemCoin.transform.localPosition = initCoinPos;

        List<IAPItemData> data = new List<IAPItemData>();
        var buyItemCoinHandler = new BuyBundleHandlerBeginner();
        buyItemCoinHandler.SetCoinDestination(itemIAPBundleBeginner.TfmImagCoin());

        bool isShowSalePack = IsBeginSalePackExist();
        if (isShowSalePack)
        {
            data = shopCoinData.data;
            itemCoin.transform.localPosition = middleCoin.transform.localPosition;
        }
        else
        {
            data = shopCoinData.data;
            itemCoin.transform.localPosition = middleCoin.transform.localPosition;
        }
        itemIAPBundleBeginner.Init(data[0], buyItemCoinHandler);
    }
    private async UniTask StartCountdown()
    {
        isRunning = true;

        DateTime endTime = Db.storage.USER_INFO.beginSaleAdsValidUntil;

        while (isRunning)
        {
            int remaining = Mathf.Max(0, (int)(endTime - DateTime.UtcNow).TotalSeconds);

            if (remaining <= 0f)
            {
                this.gameObject.SetActive(false);
                break;
            }

            // Cập nhật UI nếu cần
            //txtCountDown.text = Utilities.ConvertSecondToString(remaining);

            await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.DeltaTime, PlayerLoopTiming.Update);
        }
    }

    private bool IsBeginSalePackExist()
    {
        return true;
        var time = Db.storage.USER_INFO.beginSaleAdsValidUntil;
        return DateTime.UtcNow <= time;
    }

}
