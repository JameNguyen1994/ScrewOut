using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PS.Analytic;
using Storage;
using UnityEngine;
using UnityEngine.UI;

public class ButtonNoAdsPackHomeMenu : MonoBehaviour
{
    [SerializeField] private Image imgBg;

    [SerializeField] private Sprite sprNoAdsNormal;
    [SerializeField] private Sprite sprNoAdsWithCombo;
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        InitUI().Forget();
    }

    private async UniTask InitUI()
    {
        if (CheckNoAds.Instance.CheckIsNoAds())
        {
            this.gameObject.SetActive(false);
            return;
        }
        this.gameObject.SetActive(true);
        await UniTask.WaitUntil(() => GameAnalyticController.Instance.Remote().IsReadyRemote);
        var indexNoadsCombo = GameAnalyticController.Instance.Remote().NoAdsWithCombo.noAdsWithCombo;
        imgBg.sprite = sprNoAdsNormal;
        if (IsShowNoAdsWithCombo() && indexNoadsCombo != 0)
        {
            if (indexNoadsCombo >= 1)
            {
                imgBg.sprite = sprNoAdsWithCombo;
            }

        }
    }

    private bool IsShowNoAdsWithCombo()
    {
        return Db.storage.USER_INFO.countInterAds >= 2;
    }
}
