using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OfferwalCompletedUI : UIBase
{
    [SerializeField] private Transform tfmMain;
    [SerializeField] private Image imgFade;
    [SerializeField] private Text txtCoin;

    private UnityAction<Dictionary<string, object>> callback;
    private int vc = 0;
    
    public override void Show(Dictionary<string, object> data, UnityAction<Dictionary<string, object>> callback)
    {
        this.callback = callback;
        vc = (int)data["vc"];
        
        txtCoin.text = $"+{vc}";
        
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            tfmMain.gameObject.SetActive(true);
            tfmMain.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        });
    }

    public void OnClaimClick()
    {
        callback?.Invoke(new Dictionary<string, object>()
        {
            ["vc"] = vc
        });
        Hide();
    }

    public override void Hide()
    {
        tfmMain.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            tfmMain.gameObject.SetActive(false);
        });
        
        imgFade.DOFade(0, 0.4f).SetEase(Ease.Linear).OnComplete(() =>
        {
            imgFade.gameObject.SetActive(false);
        });
    }
}
