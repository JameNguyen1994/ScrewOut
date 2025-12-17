using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupPrevLose : PopupBase
{
    [SerializeField] private Transform tfmCover;
    [SerializeField] private Transform txtLose;

    [EasyButtons.Button]
    public override async UniTask Show()
    {
        Setup();

        await DOShow();
    }

    void Setup()
    {
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);
        tfmCover.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        txtLose.localScale = Vector3.zero;
    }

    async UniTask DOShow()
    {
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        tfmCover.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(200);
        txtLose.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(2000);
        DoHide().Forget();
    }
    
    async UniTask DoHide()
    {
        tfmCover.DOScale(0f, 0.2f).SetEase(Ease.InBack);
        await UniTask.Delay(100); 
        await imgFade.DOFade(0, 0.3f);
        imgFade.gameObject.SetActive(false);

        PopupController.Instance?.ShowRevive();
    }
}
