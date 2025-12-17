using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotifyUI : UIBase
{
    [SerializeField] private Sprite sprRewardOnWay;
    [SerializeField] private Sprite sprRewardStillVerify;
    [SerializeField] private Image imgNotify;
    [SerializeField] private RectTransform tfmNotify;
    [SerializeField] private float topY, targetY;
    private UnityAction<Dictionary<string, object>> callback;
    
    
    public override void Show(Dictionary<string, object> data, UnityAction<Dictionary<string, object>> callback)
    {
        this.callback = callback;
        int spriteType = (int)data["spr_index"];
        imgNotify.sprite = spriteType == 0 ? sprRewardOnWay : sprRewardStillVerify;
        gameObject.SetActive(true);
        tfmNotify.DOAnchorPosY(targetY, 0.3f).SetEase(Ease.OutBack);
        
        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), 5f);
    }

    public override void Hide()
    {
        tfmNotify.DOAnchorPosY(topY, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            callback?.Invoke(null);
        });
    }
}
