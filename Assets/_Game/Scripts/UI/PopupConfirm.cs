using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using TMPro;
using System;

public class PopupConfirm : PopupBase
{
    [SerializeField] private Transform content;
    [SerializeField] private Transform buttonCancel;
    [SerializeField] private Transform buttonConfirm;

    public TextMeshProUGUI text;
    public TextMeshProUGUI textBuildVersion;

    private bool isConfirm;
    private Action<bool> callback;

    public static PopupConfirm Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

#if UNITY_EDITOR
        textBuildVersion.gameObject.SetActive(true);
#endif
    }

    public void ShowBuild()
    {
        textBuildVersion.gameObject.SetActive(Debug.unityLogger.logEnabled);
    }

    public override void InitData(params object[] data)
    {
        base.InitData(data);
        callback = data[0] as Action<bool>;
        text.text = data[1].ToString();
        Show();
    }

    public override async UniTask Show()
    {
        Setup();
        DOShow().Forget();
    }

    void Setup()
    {
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);
        content.localScale = Vector3.zero;
        buttonCancel.localScale = Vector3.zero;
        buttonConfirm.localScale = Vector3.zero;
    }

    async UniTask DOShow()
    {
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        await UniTask.Delay(200);
        content.gameObject.SetActive(true);
        content.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(200);
        buttonCancel.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        buttonConfirm.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
    }

    public override void Hide()
    {
        DOHide().Forget();
    }

    async UniTask DOHide()
    {
        buttonCancel.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await buttonConfirm.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await content.DOScale(0, 0.3f).SetEase(Ease.InBack);
        imgFade.DOFade(0, 0.5f);
        await UniTask.Delay(500);
        imgFade.gameObject.SetActive(false);

        callback(isConfirm);
    }

    public void OnClickCancel()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        isConfirm = false;
        Hide();
    }

    public void OnClickConfirm()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        isConfirm = true;
        Hide();
    }
}