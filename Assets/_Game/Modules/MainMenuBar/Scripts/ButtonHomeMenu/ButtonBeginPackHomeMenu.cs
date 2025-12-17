using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PS.Analytic;
using Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBeginPackHomeMenu : MonoBehaviour
{
    [SerializeField] private Image imgBg;
    [SerializeField] private TextMeshProUGUI txtCountDown;
    [SerializeField] private Sprite sprIconBeginNormal;
    [SerializeField] private Sprite sprIconBeginSale;
    private bool isRunning = false;
    private const int levelUnlock = 5;
    // Start is called before the first frame update
    void OnEnable()
    {
       /* if (Db.storage.USER_INFO.level < levelUnlock + 1)
        {
            this.gameObject.SetActive(false);
            return;
        }*/
        InitUI();
    }

    private void OnDisable()
    {
        isRunning = false;
    }

    private void InitUI()
    {
       /* if (Db.storage.USER_INFO.level < levelUnlock + 1)
        {
            this.gameObject.SetActive(false);
            return;
        }*/
        this.gameObject.SetActive(true);
        imgBg.sprite = sprIconBeginNormal;
        txtCountDown.gameObject.SetActive(false);
        bool isShowSalePack = IsBeginSalePackExist();
        if (isShowSalePack)
        {
            var   indexKey = GameAnalyticController.Instance.Remote().BeginAds.beginAds;
            if (indexKey >= 1)
            {
                imgBg.sprite = sprIconBeginSale;
                txtCountDown.gameObject.SetActive(true);
                StartCountdown().Forget();
            }
        }
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
            txtCountDown.text = Utilities.ConvertSecondToString(remaining);

            await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.DeltaTime, PlayerLoopTiming.Update);
        }
    }
    private bool IsBeginSalePackExist()
    {
        var time = Db.storage.USER_INFO.beginSaleAdsValidUntil;
        return DateTime.UtcNow <= time;
    }
}
