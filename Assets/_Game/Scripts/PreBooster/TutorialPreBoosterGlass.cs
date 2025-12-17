using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using UnityEngine;

public class TutorialPreBoosterGlass : Singleton<TutorialPreBoosterGlass>
{
    [SerializeField] private bool isComplete = false;
    [SerializeField] private bool started = false;
    [SerializeField] private Transform tfmText;
    [SerializeField] private GameObject gobjAnimHand;


    [SerializeField] private RectTransform imgHand;
    [SerializeField] private RectTransform rectUICanvas;
    [SerializeField] private Camera cam;


    public bool IsComplete { get => isComplete; set => isComplete = value; }
    public bool Started { get => started; set => started = value; }

    [Button]
    public async UniTask ShowTutorial()
    {
        started = true;
        isComplete = false;
        tfmText.gameObject.SetActive(true);
        tfmText.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        SetPos();
        await UniTask.WaitUntil(() => isComplete);
    }

    public async UniTask OnHoldComplete()
    {

        await tfmText.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            tfmText.gameObject.SetActive(false);
        });
        gobjAnimHand.gameObject.SetActive(false);
        isComplete = true;

    }

    private void SetPos()
    {
        Transform target = LevelController.Instance.Level.GetListShapeNearestCamera(1)[0].transform;
        // 1. World → Screen point (bằng camera 3D)
        Vector3 screenPos = cam.WorldToScreenPoint(target.transform.position);


        // 2. Screen point → Local point trong imgSafeArea (Canvas Overlay -> camera = null)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectUICanvas,
            screenPos,
            null, // 🔥 Vì canvas overlay
            out Vector2 localPos
        );
        imgHand.anchoredPosition = localPos;
        imgHand.anchoredPosition = Vector3.zero;
        gobjAnimHand.gameObject.SetActive(true);
    }
}
