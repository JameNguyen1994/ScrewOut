using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using ScriptsEffect;

public class BoosterHandlerAddHole : BoosterHandlerBase
{
    [SerializeField] private Image imgDrill;
    [SerializeField] private SimpleAnimationCanvas simpleAnimationCanvas;
    [SerializeField] private SimpleAnimationCanvas prepareAnimationCanvas;
    
    [SerializeField] private GameObject parSmoke;
    [SerializeField] private Image imgNoHoldToAdd;
    [SerializeField] private List<Tray> lstTray;
    [SerializeField] private Transform tfmHolderHole;
    [SerializeField] private RectTransform canvasRectTransform; // RectTransform của Canvas (phải được tham chiếu)
    [SerializeField] private Vector3 defaultPos;



    private void Start()
    {
        defaultPos = imgDrill.rectTransform.anchoredPosition;
    }
    public override void ActiveBooster(UnityAction actionCompleteBooster)
    {
        base.ActiveBooster(actionCompleteBooster);
        BoosterController.Instance.StartAnimation(BoosterType.AddHole);
        if (lstTray.Count == 0)
        {

            imgNoHoldToAdd.DOKill();
            imgNoHoldToAdd.gameObject.SetActive(true);
            imgNoHoldToAdd.DOFade(1, 0f);
            imgNoHoldToAdd.DOFade(0, 1f).SetDelay(0.5f).OnComplete(() =>
            {
                imgNoHoldToAdd.gameObject.SetActive(false);

            });
            BoosterController.Instance.EndAnimation();
           // HoleWarning.Instance.HideWarning();
            return;
        }
        else
        {
            if (lstTray.Count == 0) return;
            Action();
            HoleWarning.Instance.HideWarning();

        }
    }


    public override async UniTask Action()
    {

        // return base.Action();
        imgDrill.rectTransform.anchoredPosition = defaultPos;
        imgDrill.DOFade(1, 0);
        imgDrill.gameObject.SetActive(true);
        // Lấy world position của tray
        Vector3 trayWorldPosition = lstTray[0].transform.position;

        // Lấy screen point
        Vector3 trayScreenPoint = Camera.main.WorldToScreenPoint(trayWorldPosition);

        // Chuyển screen point sang local point trong canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            trayScreenPoint,
            null,
            out Vector2 trayCanvasLocalPoint
        );
        SetDoneBooster();
        AudioController.Instance.PlaySound(SoundName.Effectt_Appear);

        // Di chuyển imgDrill tới vị trí tray trên canvas
        await imgDrill.rectTransform.DOAnchorPos(trayCanvasLocalPoint, 0.5f);
        await UniTask.Delay(200);
        //// await animation
        // Kích hoạt tray sau khi imgDrill di chuyển hoàn tất

        await prepareAnimationCanvas.StartAnimation(1);
        parSmoke.transform.position = lstTray[0].transform.position;
        parSmoke.SetActive(true);
        AudioController.Instance.PlaySound(SoundName.Booster_AddHole);
        await simpleAnimationCanvas.StartAnimation(3);
        lstTray[0].gameObject.SetActive(true);
        parSmoke.SetActive(false);

        // Xóa tray khỏi danh sách hoặc xử lý logic khác
        //   imgDrill.DOFade(0, 0);
        imgDrill.gameObject.SetActive(false);
        LevelController.Instance.AddNewUnlockTray(lstTray[0]);
        lstTray.RemoveAt(0);
        BoosterController.Instance.EndAnimation();
        AudioController.Instance.StopSound(SoundName.Booster_AddHole);

        if (lstTray.Count==0)
        {
            tfmHolderHole.DOMoveX(-3f, 0.1f);
        }
        else
        {
            tfmHolderHole.DOMoveX(-1.5f, 0.1f);
        }

        imgDrill.sprite = prepareAnimationCanvas.GetSpriteAt(0);
    }

    public override void SetDoneBooster()
    {
        base.SetDoneBooster();
    }

    public void AddTrayBase()
    {
        if (lstTray.Count == 0)
        {
            return;
        }

        lstTray[0].gameObject.SetActive(true);
        LevelController.Instance.AddNewUnlockTray(lstTray[0]);
        lstTray.RemoveAt(0);

        if (lstTray.Count == 0)
        {
            tfmHolderHole.DOMoveX(-3f, 0f);
        }
        else
        {
            tfmHolderHole.DOMoveX(-1.5f, 0f);
        }
    }
}
