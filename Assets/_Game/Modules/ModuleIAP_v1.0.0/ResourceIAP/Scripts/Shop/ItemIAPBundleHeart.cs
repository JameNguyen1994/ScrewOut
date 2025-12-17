using Cysharp.Threading.Tasks;
using PS.IAP;
using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemIAPBundleHeart :  ItemIAPBase
{
    [SerializeField] private TextMeshProUGUI txtAmountCoin;
    [SerializeField] private int valueCoin;
    [SerializeField] private Image imgCoinPack;
    [SerializeField] private Image imgBundle;


    [SerializeField] private TextMeshProUGUI txtAddHoleAmount;
    [SerializeField] private int valueAddHole;
    [SerializeField] private TextMeshProUGUI txtHammerAmount;
    [SerializeField] private int valueHammer;
    [SerializeField] private TextMeshProUGUI txtClearAmount;
    [SerializeField] private int valueClear;
    [SerializeField] private TextMeshProUGUI txtUnlockBox;
    [SerializeField] private int valueUnlockBox;
    [SerializeField] private TextMeshProUGUI txtTimeHeartAmount;
    [SerializeField] private int valueTimeHeart;
    [SerializeField] private GameObject goRoot;
   
    private const int levelUnlock = 1;
    private void OnEnable()
    {
      /*  if (Db.storage.USER_INFO.level < levelUnlock + 1)
        {
            goRoot.SetActive(false);
            return;
        }*/
    }
    public override async UniTask Init(object data, IPurchaseHandler purchaseHandler)
    {
     /*   if (Db.storage.USER_INFO.level < levelUnlock + 1 )
        {
            goRoot.SetActive(false);
            return;
        }*/
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
        valueUnlockBox = starterData.data.Find(x => x.resourceType == ResourceType.UNLOCK_BOX).value;
        valueTimeHeart = starterData.data.Find(x => x.resourceType == ResourceType.TIME_HEART).value;
        saleOffPercent = starterData.saleOffPercent;
        InitUI();
    }

    public override void InitUI()
    {
        base.InitUI();
        txtAddHoleAmount.text = $"x{valueAddHole}";
        txtHammerAmount.text = $"x{valueHammer}";
        txtClearAmount.text = $"x{valueClear}";
        txtUnlockBox.text = $"x{valueUnlockBox}";
        txtAmountCoin.text = $"{valueCoin}";

        var timeSpan = TimeSpan.FromMilliseconds(valueTimeHeart);
        string detail;
        if (timeSpan.TotalHours < 10)
            detail = $"{(int)timeSpan.TotalHours:D1}h";
        else
            detail = $"{(int)timeSpan.TotalHours:D2}h";

        /* if (timeSpan.TotalHours < 100)
         {
             // Hiển thị tổng số giờ, phút và giây
             detail = $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
         }
         else
         {
             // Hiển thị ngày, giờ, phút và giây
             detail = $"{timeSpan.Days:D2}:{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
         }
 */
        txtTimeHeartAmount.text = $"{detail}";
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
