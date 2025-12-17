using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using System.Collections.Generic;
using UnityEngine;

public class PopupPrevWin: PopupBase
{
    [SerializeField] private GameObject objWellDone;
    [SerializeField] private GameObject gobjWin;
    [SerializeField] private GameObject fxSparkle;
    [SerializeField] private List<GameObject> lstFxExplosion;
    [SerializeField] private List<Animator> lstAniWin;


    [EasyButtons.Button]
    public override async UniTask Show()
    {
        Setup();
        DOShow().Forget();
        IngameData.LoseCount = 0;
    }
    
    void Setup()
    {
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);
        
        objWellDone.SetActive(false);
        for (int i = 0; i < lstFxExplosion.Count; i++)
        {
            lstFxExplosion[i].SetActive(false);
        }
        fxSparkle.SetActive(false);
    }

    async UniTask DOShow()
    {
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.9f, 0.5f);
        await UniTask.Delay(400);
        VibrationController.Instance.Vibrate(VibrationType.Medium);

        AudioController.Instance.PlaySound(SoundName.WIN_WITH_TRUMPET);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            AudioController.Instance.PlaySound(SoundName.WIN_WITH_FIREWORK);
        });
       // await UniTask.Delay(200);
        gobjWin.SetActive(true);
       // lstAniWin.ForEach(ani => ani.Play("Open"));

        objWellDone.SetActive(true);
        fxSparkle.SetActive(true);
        await UniTask.Delay(2500);

        VibrationController.Instance.Vibrate(VibrationType.Medium);
        for (int i = 0; i < lstFxExplosion.Count; i++)
        {
            lstFxExplosion[i].SetActive(true);
        }
        await UniTask.Delay(1700);
        AnimTrumpetClose();
        Hide();
    }
    [Button]
    private void AnimTrumpetClose()
    {
        lstAniWin.ForEach(ani => ani.Play("Close"));

    }
    public override void Hide()
    {
        DOHide().Forget();
    }

    async UniTask DOHide()
    {
        objWellDone.SetActive(false);
        fxSparkle.SetActive(false);
        for (int i = 0; i < lstFxExplosion.Count; i++)
        {
            lstFxExplosion[i].SetActive(false);
        }
        await PopupController.Instance.ShowWin();
        await imgFade.DOFade(0f, 0.5f);
        imgFade.gameObject.SetActive(false);
    }
}
