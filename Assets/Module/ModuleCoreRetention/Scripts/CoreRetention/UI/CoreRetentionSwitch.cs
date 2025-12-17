using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class CoreRetentionSwitch : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform lever;
    public float maxAngle = 30f;
    public float threshold = 0.7f;

    private float returnDuration = 0.15f;
    private float currentNormalized;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.delta;
        currentNormalized += delta.x / 100f;
        currentNormalized = Mathf.Clamp(currentNormalized, -1f, 1f);

        float angle = currentNormalized * maxAngle;
        lever.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragHandler().Forget();
    }

    private async UniTask EndDragHandler()
    {
        BlockController.Instance.AddBlockLayer();

        if (Mathf.Abs(currentNormalized) >= threshold)
        {
            if (currentNormalized > 0)
            {
                EditorLogger.Log("[CoreRetentionSwitch] OnLeverRight");
                //AudioController.Instance.PlaySound(SoundName.ScrewDown);
                await lever.DOLocalRotate(new Vector3(0, 0, -maxAngle), returnDuration).SetEase(Ease.OutBack);
                await CoreRetentionController.Instance.MoveRight();
            }
            else
            {
                EditorLogger.Log("[CoreRetentionSwitch] OnLeverLeft");
                //AudioController.Instance.PlaySound(SoundName.ScrewDown);
                await lever.DOLocalRotate(new Vector3(0, 0, maxAngle), returnDuration).SetEase(Ease.OutBack);
                await CoreRetentionController.Instance.MoveLeft();
            }
        }

        await BackToCenter();

        BlockController.Instance.RemoveBlockLayer();
    }

    public void BackToCenterHandler()
    {
        BackToCenter();
    }

    public async UniTask BackToCenter()
    {
        await lever.DOLocalRotate(Vector3.zero, returnDuration).SetEase(Ease.OutBack);
        currentNormalized = 0f;
    }

    public async void MoveLeft()
    {
        if (BlockController.Instance.IsLock()) return;

        BlockController.Instance.AddBlockLayer();

        AudioController.Instance.PlaySound(SoundName.ScrewDown);
        await lever.DOLocalRotate(new Vector3(0, 0, maxAngle), 0).SetEase(Ease.OutBack);
        await CoreRetentionController.Instance.MoveLeft();
        await BackToCenter();

        BlockController.Instance.RemoveBlockLayer();
    }

    public async void MoveRight()
    {
        if (BlockController.Instance.IsLock()) return;

        BlockController.Instance.AddBlockLayer();

        AudioController.Instance.PlaySound(SoundName.ScrewDown);
        await lever.DOLocalRotate(new Vector3(0, 0, -maxAngle), 0).SetEase(Ease.OutBack);
        await CoreRetentionController.Instance.MoveRight();
        await BackToCenter();

        BlockController.Instance.RemoveBlockLayer();
    }

    public async UniTask MoveToNewLevel()
    {
        AudioController.Instance.PlaySound(SoundName.ScrewDown);
        await lever.DOLocalRotate(new Vector3(0, 0, maxAngle), returnDuration).SetEase(Ease.OutBack);
        await CoreRetentionController.Instance.MoveLeft();
        await BackToCenter();
    }
}