using Cysharp.Threading.Tasks;
using DG.Tweening;
using PS.Analytic;
using Storage;
using UnityEngine;

public class PinchToZoom : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SliderZoom sliderZoom;
    [SerializeField] private Transform tfmHolderLevel;
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float zoomThreshold = 0.1f; // Giá trị ngưỡng zoom
    [SerializeField] private Vector3 minPosition = new Vector3(0, 9.11f, 18);
    [SerializeField] private Vector3 maxPosition = new Vector3(0, 12.38f, 11.71f);
    [SerializeField] private Vector3 defaultPosition = new Vector3(0, 10.745f, 14.855f);

    [SerializeField] private bool isZooming = false;
    [SerializeField] private float startDistance = 0f;

    public Vector3 MinPosition => minPosition;
    public Vector3 MaxPosition => maxPosition;

    private bool isTrackingZoom;

    private async UniTask Start()
    {
        await UniTask.WaitUntil(() => LevelController.Instance != null && LevelController.Instance.Level != null && LevelController.Instance.CompleteAnimationScale);
        //return;
        if (sliderZoom != null)
        {
            sliderZoom.Initialize(this, InitValue());

        }
        /*        var zoom = Db.storage.USER_INFO.level == 1 ? 0.5f : 0.25f;
                SetZoom(Mathf.InverseLerp(0f, 1f, zoom));*/
        isZooming = false;
    }

    private void Update()
    {
        bool isGameplaying = GameManager.Instance.GameState == GameState.Stop;
        bool isTutorial = BoosterController.Instance.IsShowTutorial || PopupController.Instance.PopupCount > 0;
        if (Input.touchCount == 2 && !isGameplaying && !isTutorial)
        {
            HandlePinchZoom();
            isTrackingZoom = true;
        }
        else if (!isZooming)
        {
            if (SwipeRotation360Degrees.Instance != null)
            {
                SwipeRotation360Degrees.Instance.SwipeRotate();
            }
        }

        sliderZoom?.UpdateSlider(GetNormalizedZoom());

        if (Input.GetMouseButtonUp(0))
        {
            if (isTrackingZoom)
            {
                /*    if (GameAnalyticController.Instance)
                        GameAnalyticController.Instance.Tracking().TrackingZoom(Db.storage.USER_INFO.level,
                            Mathf.FloorToInt((sliderZoom.ZoomSlider.value * 100)));*/
                isTrackingZoom = false;
            }
            isZooming = false;
            startDistance = 0f;
        }
    }

    public void SetZoom(float value)
    {
        isZooming = true;
        var minMax = LevelController.Instance.Level.ValueMinMaxZoom;

        var min = minMax.x;
        var max = minMax.y;


        var offset = max - min;
        var v = offset / 10;

        var realValue = min + (offset * value);

        Debug.Log($"[PinchToZoom] SetZoom value:{value} - realValue:{realValue} - min:{min} - max:{max} - offset:{offset} - v:{v}");

        //var scale = Mathf.Lerp(min, max, realValue);
        tfmHolderLevel.localScale = Vector3.one * realValue;

        Vector3 targetPosition = Vector3.Lerp(maxPosition, minPosition, value);
        mainCamera.transform.position = targetPosition;

        sliderZoom?.UpdateSlider(GetNormalizedZoom());
    }
    private float InitValue()
    {
        var minMax = LevelController.Instance.Level.ValueMinMaxZoom;

        var min = minMax.x;
        var max = minMax.y;
        var offset = max - min;
        var v = offset / 10;
        var realValue = 1f;
        var value = (realValue - min) / offset;


        Debug.Log($"[PinchToZoom] InitValue value:{value} - realValue:{realValue} - min:{min} - max:{max} - offset:{offset} - v:{v}");
        return value;
    }

    private void HandlePinchZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (startDistance == 0f)
        {
            startDistance = Vector2.Distance(touch1.position, touch2.position);
        }

        float currentDistance = Vector2.Distance(touch1.position, touch2.position);

        //float percentageChange = ((currentDistance - startDistance) / startDistance) / 10f;
        float percentageChange = Mathf.Lerp(0, (currentDistance - startDistance) / startDistance, 0.1f);
        float sliderValue = Mathf.Clamp01(sliderZoom.ZoomSlider.value + percentageChange);
        sliderZoom.UpdateSlider(sliderValue);
        Debug.Log($"percentageChange: {percentageChange} - sliderValue:{sliderValue}");


        //mainCamera.transform.DOMove(targetPosition, 0.2f).SetEase(Ease.OutQuad);
        float previousDistance = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);

        float deltaDistance = currentDistance - previousDistance;

        if (deltaDistance == 0F)
        {
            startDistance = currentDistance;
        }
    }



    private Vector3 ClampPosition(Vector3 position)
    {
        position.y = Mathf.Clamp(position.y, minPosition.y, maxPosition.y);
        position.z = Mathf.Clamp(position.z, minPosition.z, maxPosition.z);
        return position;
    }

    public float GetNormalizedZoom()
    {
        float normalizedY = Mathf.InverseLerp(maxPosition.y, minPosition.y, mainCamera.transform.position.y);
        float normalizedZ = Mathf.InverseLerp(maxPosition.z, minPosition.z, mainCamera.transform.position.z);
        return (normalizedY + normalizedZ) / 2; // Lấy trung bình của Y và Z
    }

    public void TweenZoom(float value, float duration)
    {
        Vector3 targetPosition = Vector3.Lerp(maxPosition, minPosition, value);

        DOTween.Kill(mainCamera.transform); // Hủy mọi tween đang chạy trước đó
        DOTween.To(() => mainCamera.transform.position, x => mainCamera.transform.position = x, targetPosition, duration)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(() => sliderZoom?.UpdateSlider(GetNormalizedZoom()));
    }
}
