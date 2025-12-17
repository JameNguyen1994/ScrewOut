using Cysharp.Threading.Tasks;
using DG.Tweening;
using ScriptsEffect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoosterHandlerHammer : BoosterHandlerBase
{
    [SerializeField] private Image imgHammer;
    [SerializeField] private Image imgHammerDefalt;
    [SerializeField] private Shape shape;
    [SerializeField] private RectTransform canvasRectTransform; // RectTransform của Canvas (phải được tham chiếu)
    [SerializeField] private SimpleAnimationCanvas simpleAnimationCanvas;
    [SerializeField] private ParticleSystem parSmoke;
    [SerializeField] private float animTime = 0.02f;

    public override void ActiveBooster(UnityAction actionCompleteBooster)
    {
        base.ActiveBooster(actionCompleteBooster);
        //GameManager.Instance.ChangeGameState(GameState.Booster_Hammer);

        BoosterController.Instance.ChangeToHammerScrewState();
        /* {
             box = LevelController.Instance.GetBoxToFill();
             Debug.Log("Magnet");
             imgMagnet.DOFade(1, 0.5f);

             SetDoneBooster();
             // Lấy world position của tray
             Vector3 trayWorldPosition = box.transform.position;

             // Lấy screen point
             Vector3 trayScreenPoint = Camera.main.WorldToScreenPoint(trayWorldPosition);

             // Chuyển screen point sang local point trong canvas
             RectTransformUtility.ScreenPointToLocalPointInRectangle(
                 canvasRectTransform,
                 trayScreenPoint,
                 null,
                 out Vector2 trayCanvasLocalPoint
             );

             imgMagnet.rectTransform.DOAnchorPos(trayCanvasLocalPoint, 0.5f).OnComplete(() =>
             {
                 imgMagnet.gameObject.SetActive(true);

                 imgMagnet.DOFade(0, 0);


                 LevelController.Instance.FillBox(box);
             });
         }*/
    }
    public override void SetDoneBooster()
    {
        Action();

        GameManager.Instance.ChangeGameState(GameState.Play);
        //shape.gameObject.SetActive(false);
        //  actionCompleteBooster?.Invoke();
    }

    public static System.DateTime TargetTimeActionHammer;

    public static bool IsOver5Seconds()
    {
        return (System.DateTime.UtcNow - TargetTimeActionHammer).TotalSeconds > 5;
    }

    public override async UniTask Action()
    {
        BoosterController.Instance.StartAnimation(BoosterType.Hammer);

        imgHammer.DOFade(1, 0);
        imgHammer.gameObject.SetActive(true);


        base.SetDoneBooster();
        shape = BoosterController.Instance.ShapeHammer;
        Vector3 trayWorldPosition = shape.transform.position;

        // Lấy screen point
        Vector3 trayScreenPoint = Camera.main.WorldToScreenPoint(trayWorldPosition);

        // Chuyển screen point sang local point trong canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            trayScreenPoint,
            null,
            out Vector2 trayCanvasLocalPoint
        );
        imgHammer.transform.position = imgHammerDefalt.transform.position;
        imgHammer.rectTransform.DOAnchorPos(trayCanvasLocalPoint, 0.5f);
        await simpleAnimationCanvas.StartAnimation(1, animTime);
        AudioController.Instance.PlaySound(SoundName.Booster_Hammer);
        Camera.main.DOShakePosition(0.2f);
      //  Camera.main.DOShakeRotation(0.2f,45,5,45);

        // parSmoke.transform.position = new Vector3(shape.transform.position.x, shape.transform.position.y+2.5f, shape.transform.position.z-3);
        parSmoke.transform.position = shape.transform.position;
        parSmoke.Play();
        imgHammer.gameObject.SetActive(false);

        //parSmoke.Play();
        shape.RemoveAllScrew();
        LevelController.Instance.CheckAllBox();
        BoosterController.Instance.EndAnimation();
        TargetTimeActionHammer = System.DateTime.UtcNow.AddSeconds(5);
    }
}
