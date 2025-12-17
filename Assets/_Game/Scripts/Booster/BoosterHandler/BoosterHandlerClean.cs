using Cysharp.Threading.Tasks;
using DG.Tweening;
using ScriptsEffect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoosterHandlerClean : BoosterHandlerBase
{
    [SerializeField] private Image imgClean;
    [SerializeField] private Image imgCleanFail;
    [SerializeField] private Tray tray;
    [SerializeField] private Tray tray2;
    [SerializeField] private RectTransform canvasRectTransform; // RectTransform của Canvas (phải được tham chiếu)
    [SerializeField] private SimpleAnimationCanvas simpleAnimationCanvas;
    [SerializeField] private ParticleSystem parSmoke;
    [SerializeField] private ParticleSystem parSmoke2;
    [SerializeField] private Vector3 defaultPos;
    private void Start()
    {
        defaultPos = imgClean.rectTransform.anchoredPosition;
    }
    public override void ActiveBooster(UnityAction actionCompleteBooster)
    {
        base.ActiveBooster(actionCompleteBooster);
        BoosterController.Instance.StartAnimation(BoosterType.Clears);
        var trayFill = LevelController.Instance.GetTrayFill();
        if (trayFill == null)
        {

            imgCleanFail.DOKill();
            imgCleanFail.gameObject.SetActive(true);
            imgCleanFail.DOFade(1, 0f);
            imgCleanFail.DOFade(0, 1f).SetDelay(0.5f).OnComplete(() =>
               {
                   imgCleanFail.gameObject.SetActive(false);

               });
            Debug.Log("No tray fill");
            BoosterController.Instance.EndAnimation();
        }
        else
        {
            Action();
            HoleWarning.Instance.HideWarning();
        }
    }
    public async UniTask Action()
    {
        tray2 = LevelController.Instance.LstTray[0];
        tray = LevelController.Instance.LstTray[LevelController.Instance.LstTray.Count-1];

        imgClean.rectTransform.anchoredPosition = defaultPos;
        Debug.Log("Clean");
        SetDoneBooster();
        // Lấy world position của tray
        Vector3 trayWorldPosition = tray.transform.position;
        Vector3 trayWorldPosition2 = tray2.transform.position;

        // Lấy screen point
        Vector3 trayScreenPoint = Camera.main.WorldToScreenPoint(trayWorldPosition);

        // Chuyển screen point sang local point trong canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            trayScreenPoint,
            null,
            out Vector2 trayCanvasLocalPoint
        );
        Vector3 trayScreenPoint2 = Camera.main.WorldToScreenPoint(trayWorldPosition2);

        // Chuyển screen point sang local point trong canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            trayScreenPoint2,
            null,
            out Vector2 trayCanvasLocalPoint2
        );
        imgClean.gameObject.SetActive(true);

        AudioController.Instance.PlaySound(SoundName.Effectt_Appear);
        await imgClean.rectTransform.DOAnchorPos(trayCanvasLocalPoint,1f);
        parSmoke.gameObject.SetActive(true);
        //parSmoke2.gameObject.SetActive(true);

        parSmoke.Play();
        AudioController.Instance.PlaySound(SoundName.Booster_Clear);

        LevelController.Instance.MoveScrewOnTrayToSecretBox();

        simpleAnimationCanvas.StartAnimation(10, 0.02f);

        await imgClean.rectTransform.DOAnchorPos(trayCanvasLocalPoint2,1.5f);
        parSmoke.Stop();
        //parSmoke2.Stop();
        parSmoke.gameObject.SetActive(false);
        //parSmoke2.gameObject.SetActive(false);
        AudioController.Instance.StopSound(SoundName.Booster_Clear);
        await  imgClean.rectTransform.DOAnchorPosX(defaultPos.x*-1.5f,1f);

        imgClean.gameObject.SetActive(false);


        BoosterController.Instance.EndAnimation();
    }
    public override void SetDoneBooster()
    {
        base.SetDoneBooster();
        //  actionCompleteBooster?.Invoke();
    }
}
