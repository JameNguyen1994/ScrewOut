using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using ScriptsEffect;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

public class BoosterHandlerUnlockBoxBottom : BoosterHandlerBase
{
    [SerializeField] private Box box;
    [SerializeField] private Image imgNoBoxToUnlock;
    [SerializeField] private SimpleAnimationCanvas animShow;
    [SerializeField] private SimpleAnimationCanvas animUnlock;
    [SerializeField] private Image imgKey;
    [SerializeField] private Image imgKeyBooster;
    [SerializeField] private Sprite sprDefault;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private ParticleSystem parAppear;

    [SerializeField] private RectTransform canvasRectTransform; // RectTransform của Canvas (phải được tham chiếu)



    private void Start()
    {
        // imgKey.rectTransform.position = BoosterController.Instance.GetBoosterKeyUnlock().position;
        // startPos = imgKey.transform.position;

        imgKey.gameObject.SetActive(false);
        // imgKey.gameObject.SetActive(true);
    }
    public override void ActiveBooster(UnityAction actionCompleteBooster)
    {
        base.ActiveBooster(actionCompleteBooster);
        BoosterController.Instance.StartAnimation(BoosterType.UnlockBox);
        box = LevelController.Instance.BaseBox.GetBoxLock();
        var hasColor = !ScrewBlockedRealTimeController.Instance.IsFullAll();
        if (box == null || !hasColor)
        {

            imgNoBoxToUnlock.DOKill();
            imgNoBoxToUnlock.gameObject.SetActive(true);
            imgNoBoxToUnlock.DOFade(1, 0f);
            imgNoBoxToUnlock.DOFade(0, 1f).SetDelay(0.5f).OnComplete(() =>
            {
                imgNoBoxToUnlock.gameObject.SetActive(false);

            });
            BoosterController.Instance.EndAnimation();
            //HoleWarning.Instance.HideWarning();
            return;
        }
        else
        {
            Action();
            HoleWarning.Instance.HideWarning();

        }

        // Action().Forget();
    }
    [Button("Move")]
    public async UniTask MoveKeyAsync(Box box)
    {
        imgKey.sprite = sprDefault;
        imgKey.transform.position = startPos;
        imgKey.transform.position = imgKeyBooster.transform.position;
        Vector3 trayWorldPosition = box.TfmUnlockPos.position;

        // Lấy screen point
        Vector3 trayScreenPoint = Camera.main.WorldToScreenPoint(trayWorldPosition);

        // Chuyển screen point sang local point trong canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            trayScreenPoint,
            null,
            out Vector2 boxCanvasLocalPoint
        );


        imgKey.gameObject.SetActive(true);


        await animShow.StartAnimation(1, 0.03f);
        await imgKey.transform.DOScale(Vector3.one * 1.1f, 0.4f).SetEase(Ease.OutBack);
        parAppear.Play();

        await imgKey.rectTransform.DOAnchorPos(boxCanvasLocalPoint, 1f);
        await animUnlock.StartAnimation(1, 0.03f);

        imgKey.gameObject.SetActive(false);
    }
    public override async UniTask Action()
    {
        // return base.Action();
        // BoosterController.Instance.StartAnimation(BoosterType.UnlockBox);
        await MoveKeyAsync(box);

        AudioController.Instance.PlaySound(SoundName.Effectt_Appear);
        //box.InitUI(BoxState.Unlock);
        Debug.Log($"Unlock Box {box.name}");
        SetDoneBooster();
        box.ChangeState(BoxState.Unlock);
        BoosterController.Instance.EndAnimation();
    }
    public override void SetDoneBooster()
    {
        base.SetDoneBooster();
        //  actionCompleteBooster?.Invoke();
    }
}
