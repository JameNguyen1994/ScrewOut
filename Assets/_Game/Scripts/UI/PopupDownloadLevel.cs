using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using TMPro;
using System;
using Storage.Model;
using Storage;

public class PopupDownloadLevel : PopupBase
{
    [SerializeField] private Transform content;
    [SerializeField] private Transform buttonCancel;
    [SerializeField] private Transform buttonConfirm;

    [SerializeField] private GameObject lockControl;
    [SerializeField] private GameObject downloadProgress;

    public TextMeshProUGUI textDownloadProgress;
    public RoundedProgressBar progressBar;

    [SerializeField] private GameObject btnUpdate;
    [SerializeField] private GameObject btnRetry;

    [SerializeField] private GameObject lblUpdateLevel;
    [SerializeField] private GameObject lblDownloadFailed;

    public static PopupDownloadLevel Instance { get; private set; }

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
        buttonConfirm.localScale = Vector3.zero;

        btnUpdate.SetActive(true);
        btnRetry.SetActive(false);

        lblUpdateLevel.SetActive(true);
        lblDownloadFailed.SetActive(false);

        textDownloadProgress.text = "0%";
        lockControl.SetActive(false);
        downloadProgress.SetActive(false);
        progressBar.SetProgress(0);
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
        buttonConfirm.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
    }

    public override void Hide()
    {
        DOHide().Forget();
    }

    private async UniTask DOHide()
    {
        buttonCancel.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await buttonConfirm.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await content.DOScale(0, 0.3f).SetEase(Ease.InBack);
        imgFade.DOFade(0, 0.5f);
        await UniTask.Delay(500);
        imgFade.gameObject.SetActive(false);
    }

    public void OnClickCancel()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        Hide();
    }

    public void OnClickConfirm()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        ProgressDownload();
    }

    private async void ProgressDownload()
    {
        btnUpdate.SetActive(true);
        btnRetry.SetActive(false);

        lblUpdateLevel.SetActive(true);
        lblDownloadFailed.SetActive(false);

        lockControl.SetActive(true);
        textDownloadProgress.text = "0%";
        progressBar.SetProgress(0);
        downloadProgress.SetActive(true);

        UserInfo user = Db.storage.USER_INFO;

        float currentValue = 0f;
        float maxValue = UnityEngine.Random.Range(20f, 76f);
        float randomFakeTime = UnityEngine.Random.Range(3f, 5f);

        var task = DOTween.To(() => currentValue, x =>
        {
            currentValue = x;
            textDownloadProgress.text = $"{(int)currentValue}%";
        }, maxValue, randomFakeTime);

        progressBar.SetProgress(maxValue / 100, randomFakeTime).Forget();

        await AssetBundleService.CacheLevel(user.level);
        bool isHaveCacheLevel = await AssetBundleService.IsHaveCachLevel(user.level);

        if (isHaveCacheLevel)
        {
            task.Kill();

            DOTween.To(() => currentValue, x =>
            {
                currentValue = x;
                textDownloadProgress.text = $"{(int)currentValue}%";
            }, 100f, 0.25f).ToUniTask().Forget();

            await progressBar.SetProgress(1, 0.25f);
            await UniTask.Delay(500);

            Hide();

            MainMenuRecieveRewardsHelper.Instance.UpdateUIForReward();
            SceneController.Instance.ChangeScene(SceneType.GamePlayNewControl);
            UITopController.Instance.OnStartGameplay();
        }
        else
        {
            await task;
            await UniTask.Delay(2000);

            lockControl.SetActive(false);

            btnUpdate.SetActive(false);
            btnRetry.SetActive(true);

            lblUpdateLevel.SetActive(false);
            lblDownloadFailed.SetActive(true);

            //Show popup low internet
            PopupSlowNetwork.Instance.Show().Forget();
        }
    }
}