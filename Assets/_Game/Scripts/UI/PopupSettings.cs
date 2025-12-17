using Life;
using Storage;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PopupSettings : PopupBase
{
    [SerializeField] private SwitchButton btnSound;
    [SerializeField] private SwitchButton btnVibra;
    [SerializeField] private SwitchButton btnMusic;

    [SerializeField] private Transform content;
    [SerializeField] private Transform tfmBtnClose, tfmBtnMusic, tfmBtnSound, tfmBtnHaptic;

    private bool isContinue = true;
    
    [EasyButtons.Button]
    public override async UniTask Show()
    {
        Setup();
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
    }

    async UniTask DOShow()
    {
        UITopController.Instance?.OnShowSetting();
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        await UniTask.Delay(200);
        content.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(200);
        tfmBtnHaptic.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        tfmBtnSound.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        tfmBtnMusic.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        await UniTask.Delay(200);
            tfmBtnClose.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
    
    [EasyButtons.Button]
    public override void Hide()
    {
        UITopController.Instance?.OnShowMainMenu();
        DOHide().Forget();
    }
    
    async UniTask DOHide()
    {
        tfmBtnClose.DOScale(0, 0.3f).SetEase(Ease.InBack);
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
      //  TrackingController.Instance.TrackingSettingInGame(Db.storage.SETTING_DATAS.sound? 2: 1);

    }
    public void OnHapticBtnClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        Db.storage.SETTING_DATAS.ChangeValueVibra(!Db.storage.SETTING_DATAS.vibra);
        btnVibra.SetUIButton(Db.storage.SETTING_DATAS.vibra);
      //  TrackingController.Instance.TrackingSettingInGame(Db.storage.SETTING_DATAS.vibra? 4: 3);

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
        
       // TrackingController.Instance.TrackingSettingInGame(Db.storage.SETTING_DATAS.music? 6: 5);

    }

    public void OnHomeBtnClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        isContinue = false;
        Hide();
    }

    public void OnContinueBtnClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        isContinue = true;
        Hide();
    }
}
