using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoosterHandlerMagnet : BoosterHandlerBase
{
    [SerializeField] private Image imgMagnet;
    [SerializeField] private Box box;
    [SerializeField] private RectTransform canvasRectTransform; // RectTransform của Canvas (phải được tham chiếu)
    [SerializeField] private Vector3 defaultPos;
    [SerializeField] private List<ParticleSystem> lstParSmoke;
    [SerializeField] private GameObject gobjMagnet;
    [SerializeField] private ParticleSystem parStar;

    private void Start()
    {
        defaultPos = imgMagnet.rectTransform.anchoredPosition;
    }
    public override void ActiveBooster(UnityAction actionCompleteBooster)
    {
        base.ActiveBooster(actionCompleteBooster);

        /*        var trayFill = LevelController.Instance.GetTrayFill();
                if (trayFill == null)
                {
                    Debug.Log("No tray fill");
                }
                else*/
        {
            box = LevelController.Instance.GetBoxToFill();
            if (box == null)
                return;

            Action();
        }
    }
    public override async UniTask Action()
    {
        // return base.Action();
        BoosterController.Instance.StartAnimation(BoosterType.Magnet);
        //imgMagnet.rectTransform.anchoredPosition = defaultPos;
        Debug.Log("Magnet");
       // imgMagnet.DOFade(1, 0);

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
        await imgMagnet.rectTransform.DOAnchorPos(Vector3.zero, 0).ToUniTask();

        imgMagnet.gameObject.SetActive(true);

        await imgMagnet.rectTransform.DOScale(Vector3.one*3,0.8f).SetEase(Ease.OutBack).ToUniTask();
        parStar.gameObject.SetActive(true);
        parStar.Play();
        AudioController.Instance.PlaySound(SoundName.Effectt_Appear);

        await UniTask.Delay(500);
        imgMagnet.rectTransform.DOScale(Vector3.one, 0.5f).ToUniTask();
        await imgMagnet.rectTransform.DOAnchorPos(trayCanvasLocalPoint, 0.5f).SetEase(Ease.OutQuad).ToUniTask();

        gobjMagnet.transform.position = new Vector3(box.transform.position.x, box.transform.position.y, box.transform.position.z);
        // gobjMagnet.SetActive(true);
        foreach (var par in lstParSmoke)
        {
            par.Play();
        }
        AudioController.Instance.PlaySound(SoundName.Booster_Magnet);

        imgMagnet.gameObject.SetActive(true);
        await UniTask.Delay(1000);

       /* await LevelController.Instance.FillBox(box, () =>
        {
            BoosterController.Instance.EndAnimation();

        });*/
        
        foreach (var par in lstParSmoke)
        {
            par.Stop();
        }
       // AudioController.Instance.StopSound(SoundName.Booster_Magnet);

        imgMagnet.gameObject.SetActive(false);


    }
    public override void SetDoneBooster()
    {
        base.SetDoneBooster();
        //  actionCompleteBooster?.Invoke();
    }
}
