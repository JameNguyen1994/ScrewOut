using Life;
using Storage;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupPause : PopupBase
{
    [SerializeField] private SwitchButton btnSound;
    [SerializeField] private SwitchButton btnVibra;
    [SerializeField] private SwitchButton btnMusic;

    [SerializeField] private Transform content;
    [SerializeField] private Transform tfmBtnClose, tfmBtnMusic, tfmBtnSound, tfmBtnHaptic, tfmBtnHome, tfmBtnContinue;

    [Header("ComfirmQuit")]
    [SerializeField] private Transform tfmConfirmQuit;
    [SerializeField] private Transform tfmButtonQuit;
    [SerializeField] private Transform tfmButtonCloseQuit;
    [SerializeField] private Image imgFadeConfirmQuit;

    [SerializeField] GameObject btnHome;
    [SerializeField] GameObject btnHomeLock;
    private bool isContinue = true;
    
    [EasyButtons.Button]
    public override async UniTask Show()
    {
        btnHomeLock.SetActive(Db.storage.USER_INFO.level <= 3);

        Setup();
        UITopController.Instance?.OnPauseGame();
        DOShow().Forget();
    }

    void Setup()
    {
        InitButton();
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);
        content.localScale = Vector3.zero;
        tfmBtnClose.localScale = Vector3.zero;
        tfmBtnMusic.localScale = Vector3.zero;
        tfmBtnSound.localScale = Vector3.zero;
        tfmBtnHaptic.localScale = Vector3.zero;
        if (tfmBtnContinue != null)
            tfmBtnContinue.localScale = Vector3.zero;
        if (tfmBtnHome != null)
            tfmBtnHome.localScale = Vector3.zero;
    }

    async UniTask DOShow()
    {
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        await UniTask.Delay(200);
        content.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(200);
        tfmBtnHaptic.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        tfmBtnSound.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        tfmBtnMusic.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        await UniTask.Delay(200);
        tfmBtnHome.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        if (tfmBtnContinue != null)
            await tfmBtnContinue.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        if (tfmBtnHome != null)
            tfmBtnClose.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
    
    [EasyButtons.Button]
    public override void Hide()
    {
        UITopController.Instance?.OnStartGameplay();
        DOHide().Forget();
    }
    
    async UniTask DOHide()
    {
        tfmBtnClose.DOScale(0, 0.3f).SetEase(Ease.InBack);
        tfmBtnHome.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await tfmBtnContinue.DOScale(0, 0.3f).SetEase(Ease.InBack);
        
        await content.DOScale(0, 0.3f).SetEase(Ease.InBack);
        imgFade.DOFade(0, 0.5f);
        await UniTask.Delay(500);
        imgFade.gameObject.SetActive(false);

        if (!isContinue)
        {
            SceneController.Instance.ChangeScene(SceneType.MainMenu);
        }
    }
    
    public void InitButton()
    {
        btnSound.SetUIButton(Db.storage.SETTING_DATAS.sound);
        btnVibra.SetUIButton(Db.storage.SETTING_DATAS.vibra);
        btnMusic.SetUIButton(Db.storage.SETTING_DATAS.music);
    }
    public void OnSoundBtnClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        Db.storage.SETTING_DATAS.ChangeValueSound(!Db.storage.SETTING_DATAS.sound);
        btnSound.SetUIButton(Db.storage.SETTING_DATAS.sound);

    }
    public void OnHapticBtnClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        Db.storage.SETTING_DATAS.ChangeValueVibra(!Db.storage.SETTING_DATAS.vibra);
        btnVibra.SetUIButton(Db.storage.SETTING_DATAS.vibra);

    }
    public void OnMusicBtnClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        Db.storage.SETTING_DATAS.ChangeValueMusic(!Db.storage.SETTING_DATAS.music);
        btnMusic.SetUIButton(Db.storage.SETTING_DATAS.music);
        if (!Db.storage.SETTING_DATAS.music)
            AudioController.Instance.StopMusic(SoundName.Music);
        else
            AudioController.Instance.PlayMusic(SoundName.Music,true,0.5f);

    }

    public void OnHomeBtnClick()
    {
        if (Db.storage.USER_INFO.level <= 3)
        {
            return;
        }

        AudioController.Instance.PlaySound(SoundName.Click);

        ShowConfirmQuit();
    }
    public void OnCloseButtonComfirmClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        HideConfirmQuit();
    }
    private async UniTask ShowConfirmQuit()
    {
        tfmConfirmQuit.gameObject.SetActive(true);
        tfmButtonQuit.localScale = Vector3.zero;
        tfmConfirmQuit.localScale = Vector3.zero;

        tfmButtonCloseQuit.localScale = Vector3.zero;
        imgFadeConfirmQuit.gameObject.SetActive(true);
        imgFadeConfirmQuit.DOFade(0.98f, 0.3f).From(0);
        await tfmConfirmQuit.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await tfmButtonQuit.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await tfmButtonCloseQuit.DOScale(1, 0.3f).SetEase(Ease.OutBack);


    }

    private async UniTask HideConfirmQuit()
    {
        tfmButtonQuit.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await tfmButtonCloseQuit.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await tfmConfirmQuit.DOScale(0, 0.3f).SetEase(Ease.InBack);
        imgFadeConfirmQuit.gameObject.SetActive(false);

        tfmConfirmQuit.gameObject.SetActive(false);
        imgFadeConfirmQuit.DOFade(0, 0.3f);

    }
    public void OnClickComfirmQuit()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
       // LoadingFade.Instance.ShowLoadingFade();
        isContinue = false;
        LifeController.Instance.UseLife();
        int level = 0;
        float percentage = 0;
        if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
        {
            SerializationManager.IsQuitGame = true;
            level = Db.storage.USER_INFO.level;
            percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
            SerializationManager.ClearAllDataInGamePlay();
        }
        GoHome().Forget();
        TrackingController.Instance.TrackingInventory(level, percentage);
        TrackingController.Instance.TrackingGamePlay(LEVEL_STATUS.FAILED);
       // TrackingController.Instance.TrackingLevelEnd("HOME_DROP", IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count);
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Life", 1, "life", "QuitToHome");

    }   
    private async UniTask GoHome()
    {

        var user = Db.storage.USER_INFO;
        await AdsController.Instance.ShowInterFailed(user.level, user.playTime, (result) =>
        {
            SceneController.Instance.ChangeScene(SceneType.MainMenu);

        });


    }
    public void OnContinueBtnClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        isContinue = true;
        Hide();
    }
}
