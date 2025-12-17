using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using UnityEngine;
using UnityEngine.UI;

public class OfferwallNotificationSystem : MonoBehaviour
{
    [SerializeField] private Text txtBody, txtTitle;
    [SerializeField] private RectTransform tfmNotification;
    [SerializeField] private float outPosY, inPosY;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private int presentDuration = 3000;
    

    [Button]
    public void Show(string title, string body)
    {
        txtBody.text = body;
        txtTitle.text = title;
        gameObject.SetActive(true);
        DoAnimationShow().Forget();
    }

    async UniTask DoAnimationShow()
    {
        await tfmNotification.DOAnchorPosY(inPosY, duration);
        await UniTask.Delay(presentDuration);
        Hide().Forget();
    }

    async UniTask DoAnimationHide()
    {
        await tfmNotification.DOAnchorPosY(outPosY, duration);
    }

    [Button]
    public void OnNotificationClick()
    {
        //ShopIAPController.Instance.OfferwallRewardPopup.Show();
        Hide().Forget();
    }

    public async UniTask Hide()
    {
        await DoAnimationHide();
        gameObject.SetActive(false);
    }
}
