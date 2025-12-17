using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PopupSlowNetwork : PopupBase
{
    [SerializeField] private Transform content;
    [SerializeField] private Transform buttonCancel;
    [SerializeField] private Transform buttonOk;

    public static PopupSlowNetwork Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override async UniTask Show()
    {
        Setup();
        DOShow().Forget();
    }

    private void Setup()
    {
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);
        content.localScale = Vector3.zero;
        buttonCancel.localScale = Vector3.zero;
        buttonOk.localScale = Vector3.zero;
    }

    private async UniTask DOShow()
    {
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        await UniTask.Delay(200);
        content.gameObject.SetActive(true);
        content.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(200);
        buttonCancel.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        buttonOk.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
    }

    public override void Hide()
    {
        DOHide().Forget();
    }

    private async UniTask DOHide()
    {
        buttonCancel.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await buttonOk.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await content.DOScale(0, 0.3f).SetEase(Ease.InBack);
        imgFade.DOFade(0, 0.5f);
        await UniTask.Delay(500);
        imgFade.gameObject.SetActive(false);
    }

    public void OnClickOk()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        Hide();
    }
}