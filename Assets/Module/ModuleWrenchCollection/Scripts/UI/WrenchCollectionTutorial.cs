using Cysharp.Threading.Tasks;
using DG.Tweening;
using Storage;
using TMPro;
using UnityEngine;

public class WrenchCollectionTutorial : PopupBase
{
    [SerializeField] private Transform content;
    [SerializeField] private Transform buttonCancel;
    [SerializeField] private Transform buttonConfirm;

    [SerializeField] private TextMeshProUGUI txtTutorial;

    public bool IsShow;

    public override async UniTask Show()
    {
        IsShow = true;

        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        WrenchCollectionRewardData reward = WrenchCollectionManager.Instance.Config.GetConfigByIndex(data.level, data.rewardGroup);

        if (reward != null)
        {
            txtTutorial.text = string.Format(WrenchCollectionDefine.CONTENT_FORMAT, reward.WrenchAmount - data.collectedWrench);
        }

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
        buttonConfirm.localScale = Vector3.zero;
    }

    private async UniTask DOShow()
    {
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        content.gameObject.SetActive(true);
        await content.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        buttonCancel.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        buttonConfirm.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
    }

    public override void Hide()
    {
        IsShow = false;

        DOHide().Forget();
    }

    private async UniTask DOHide()
    {
        buttonCancel.DOScale(0, 0.3f).SetEase(Ease.InBack);
        buttonConfirm.DOScale(0, 0.3f).SetEase(Ease.InBack);
        content.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await imgFade.DOFade(0, 0.5f);
        imgFade.gameObject.SetActive(false);
    }

    public void OnClickCancel()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        Hide();
/*        if (!WrenchCollectionController.Instance.IsShow)
        {
            WrenchCollectionController.Instance.Show();
        }*/
    }
}