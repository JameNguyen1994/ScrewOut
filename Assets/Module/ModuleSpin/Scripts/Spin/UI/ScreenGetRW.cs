using DG.Tweening;
using Spin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGetRW : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private TMPro.TextMeshProUGUI txtValue;

    [SerializeField] private GameObject gobjRW;
    [SerializeField] private GameObject gobjCover;
    [SerializeField] private GameObject btnDoubleByAds;
    [SerializeField] private GameObject btnClose;

    [SerializeField] private SpinController spinController;

    public void OnShowScreen(string value, Sprite sprIcon)
    {
        gobjCover.SetActive(true);
        gobjRW.SetActive(true);
        btnDoubleByAds.SetActive(false);
        btnClose.SetActive(false);
        gobjRW.transform.localScale = Vector3.zero;
        imgIcon.sprite = sprIcon;
        txtValue.text = string.IsNullOrEmpty(value) ? string.Empty : $"<size=80>x</size>{value}";

        AudioController.Instance.PlaySound(SoundName.IAP_Complete);

        gobjRW.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            btnDoubleByAds.SetActive(true);
            btnClose.SetActive(true);
        });
    }

    public void OnHideScreen()
    {
        gobjRW.SetActive(false);
        gobjCover.SetActive(false);
        btnDoubleByAds.SetActive(false);
        btnClose.SetActive(false);
    }

    public void OnCloseClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        OnHideScreen();
        spinController.OnClaimRewardComplete(false);
    }

    public void OnDoubleRewardClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        AdsController.Instance.ShowRewardAds(RewardAdsPos.lucky_spin, DoubleRewardHandler, null, null, "double_spin_reward");
    }

    private void DoubleRewardHandler()
    {
        OnHideScreen();
        spinController.OnClaimRewardComplete(true);
    }
}
