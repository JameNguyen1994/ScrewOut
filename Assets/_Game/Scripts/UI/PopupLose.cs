using DG.Tweening;
using Life;
using ScriptsEffect;
using Storage;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopupLose : PopupBase
{
    private static readonly int Break = Animator.StringToHash("Break");
    [SerializeField] private Transform tfmContent;
    [SerializeField] private Transform tfmHeart;
    [SerializeField] private Transform tfmHome, tfmRetry;
    [SerializeField] private Animator animator;
    [SerializeField] private List<PreBooster> lstPreBooster;


    private bool isRetry;


    [EasyButtons.Button]
    public override async UniTask Show()
    {
        var user = Db.storage.USER_INFO;
       await  AdsController.Instance.ShowInterFailed(user.level, user.playTime, (result) =>
        {
            /*            if (GameConfig.OLD_VERSION)
                        {
                            ShowReviveOld();
                        }
                        else
                        {
                        }*/
        });
        Setup();
        IngameData.preBoosterPlace = PreBoosterPlace.Popup_end;
        TrackingController.Instance.TrackingGamePlay(LEVEL_STATUS.out_of_slot);
        UITopController.Instance?.OnShowLose();
        AudioController.Instance?.PlaySound(SoundName.Lose_New);
        VibrationController.Instance?.DoubleVibrate(VibrationType.Bigbang).Forget();
        IngameData.LoseCount++;
        DOShow().Forget();
    }

    async UniTask DOShow()
    {
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        await UniTask.Delay(200);
        tfmContent.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(200);
        await tfmHeart.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        animator.SetTrigger(Break);
        // do heart animation here
        await UniTask.Delay(1000);

        foreach (var pre in lstPreBooster)
        {
            pre.Show().Forget();
        }
        await UniTask.Delay(300);
        if (IngameData.LoseCount % 5 == 0)
            foreach (var pre in lstPreBooster)
            {
                pre.UseIfHaveCount();
            }

        tfmHome.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await tfmRetry.DOScale(1, 0.3f).SetEase(Ease.OutBack);

    }

    void Setup()
    {
        foreach (var pre in lstPreBooster)
        {
            //PreBoosterController.Instance.SetSellectPreBooster(pre.PreBoosterType, false);
            pre.Reset();

        }
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);
        tfmContent.localScale = Vector3.zero;
        tfmHeart.localScale = Vector3.zero;
        tfmHome.localScale = Vector3.zero;
        tfmRetry.localScale = Vector3.zero;
        animator.Rebind();
    }

    public void OnRetryClick()
    {
        AudioController.Instance?.PlaySound(SoundName.Click);
        if (DBLifeController.Instance.LIFE_INFO.lifeAmount > 0 || DBLifeController.Instance.LIFE_INFO.timeInfinity > 0)
        {
            isRetry = true;
            TrackingController.Instance.TrackingRetry();
            DOHide().Forget();
        }
        else
        {
            LifeController.Instance.ShowPopupLife();
        }
    }

    [EasyButtons.Button]
    void TestTryAgain()
    {
        isRetry = true;
        DOHide().Forget();
    }

    public void OnHomeClick()
    {
        AudioController.Instance?.PlaySound(SoundName.Click);
        isRetry = false;
        //  LoadingFade.Instance.ShowLoadingFade();

        DOHide().Forget();
    }

    async UniTask DOHide()
    {

        tfmRetry.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await tfmHome.DOScale(0, 0.3f).SetEase(Ease.InBack);

        if (isRetry)
        {
            foreach (var pre in lstPreBooster)
            {
                PreBoosterController.Instance.SetSellectPreBooster(pre.PreBoosterType, pre.Select);
                pre.Hide();

            }
            await UniTask.Delay(200);
            //animator.SetTrigger(Break);
            await UniTask.Delay(900);
        }

        tfmContent.DOScale(0, 0.3f).SetEase(Ease.InBack);
        // await UniTask.Delay(200);
        // await imgFade.DOFade(0f, 0.5f);
        // imgFade.gameObject.SetActive(false);

        if (isRetry)
        {
            var scene = IngameData.MODE_CONTROL == ModeControl.ControlV2 ? SceneType.GamePlayNewControl : SceneType.Gameplay;
            SceneController.Instance.ChangeScene(scene);
            UITopController.Instance.OnStartGameplay();
        }
        else
        {
            SceneController.Instance.ChangeScene(SceneType.MainMenu);
        }

    }
}
