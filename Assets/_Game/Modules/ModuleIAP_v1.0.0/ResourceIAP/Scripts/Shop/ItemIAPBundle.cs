using Cysharp.Threading.Tasks;
using PS.IAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Storage;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemIAPBundle : ItemIAPBase
{
    [SerializeField] private Text txtAmountCoin;
    [SerializeField] private int valueCoin;
    [SerializeField] private Image imgCoinPack;
    [SerializeField] private Image imgBundle;

    [SerializeField] private Text txtAddHoleAmount;
    [SerializeField] private int valueAddHole;
    [SerializeField] private Text txtHammerAmount;
    [SerializeField] private int valueHammer;
    [SerializeField] private Text txtClearAmount;
    [SerializeField] private int valueClear;
    [SerializeField] private Text txtMagnetAmount;
    [SerializeField] private int valueMagnet;
    [SerializeField] private GameObject goRoot;
    private const int levelUnlock = 20;
    private void OnEnable()
    {
        if (Db.storage.USER_INFO.level < levelUnlock + 1)
        {
            goRoot.SetActive(false);
            return;
        }
    }
    public override async UniTask Init(object data, IPurchaseHandler purchaseHandler)
    {
        if (Db.storage.USER_INFO.level < levelUnlock + 1)
        {
            goRoot.SetActive(false);
            return;
        }
        goRoot.SetActive(true);
        await base.Init(data, purchaseHandler);
        this.purchaseHandler = purchaseHandler;
        this.data = data;

        var starterData = (IAPItemData)data;
        productID = starterData.iapKey;

        valueCoin = starterData.data.Find(x => x.resourceType == ResourceType.Coin).value;

        valueAddHole = starterData.data.Find(x => x.resourceType == ResourceType.ADD_HOLE).value;
        valueHammer = starterData.data.Find(x => x.resourceType == ResourceType.HAMMER).value;
        valueClear = starterData.data.Find(x => x.resourceType == ResourceType.CLEAR).value;
        //valueMagnet = starterData.data.Find(x => x.resourceType == ResourceType.MAGNET).value;
        saleOffPercent = starterData.saleOffPercent;
        InitUI();
    }

    public override void InitUI()
    {
        base.InitUI();
        txtAddHoleAmount.text = $"x{valueAddHole}";
        txtHammerAmount.text = $"x{valueHammer}";
        txtClearAmount.text = $"x{valueClear}";
        txtMagnetAmount.text = $"x{valueMagnet}";
        txtAmountCoin.text = $"x{valueCoin}";

        /*        var timeSpan = TimeSpan.FromMilliseconds(valueMagnet);
                string detail;

                if (timeSpan.TotalHours < 100)
                {
                    // Hiển thị tổng số giờ, phút và giây
                    detail = $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
                else
                {
                    // Hiển thị ngày, giờ, phút và giây
                    detail = $"{timeSpan.Days:D2}:{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }

                txtMagnetAmount.text = $"{detail}";*/
    }
    public Transform TfmImagCoin() => imgCoinPack.transform;
    public void SetImageCoinPack(Sprite sprite)
    {
        imgCoinPack.sprite = sprite;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
    public void SetImageBundle(Sprite sprite)
    {
        imgBundle.sprite = sprite;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}
