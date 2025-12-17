using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OfferwallListUI : UIBase
{
    [SerializeField] private Transform tfmMain;
    [SerializeField] private Image imgFade;
    [SerializeField] private GameObject objMc, objBitlabs;
    
    
    public override void Show(Dictionary<string, object> data, UnityAction<Dictionary<string, object>> callback)
    {
        
        bool isEnableMc = data.ContainsKey("mc_enable") 
            ? (bool)data["mc_enable"] 
            : true;
        
        bool isEnableBitlab = data.ContainsKey("bl_enable") 
            ? (bool)data["bl_enable"] 
            : true;
        
        objMc.SetActive(isEnableMc);
        objBitlabs.SetActive(isEnableBitlab);
        
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            tfmMain.gameObject.SetActive(true);
            tfmMain.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        });
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
