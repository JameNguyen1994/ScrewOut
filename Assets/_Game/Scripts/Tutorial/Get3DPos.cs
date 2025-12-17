using EasyButtons;
using UnityEngine;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

public class Get3DPos : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransformImage;   // UI cần đặt vị trí
    [SerializeField] private Canvas canvasUI;               // Canvas chứa rectTransform
    [SerializeField] private RectTransform rectTransformUI;               // Canvas chứa rectTransform
    [SerializeField] private Camera fallbackCamera;         // Nếu canvas.worldCamera null thì dùng cái này
    [SerializeField] private Transform tfm3D;               // Transform nguồn 3D (optional)
    [SerializeField] private Vector2 uiOffset;              // Offset trên UI (px) nếu cần lệch một chút

    private void Start()
    {
        /*if (tfm3D != null)
        {
            SetPosFrom3D(tfm3D);
        }
        else
        {
            Debug.LogWarning("tfm3D is not assigned. Please assign a Transform with a 3D position.");
        }*/
    }

    // Nút trong Inspector để test nhanh theo Transform
    [Button("Set Position From 3D (Transform)")]
    public void SetPosFrom3D(Transform tfmPos)
    {
        Vector3 trayWorldPosition = tfmPos.position;

        // Lấy screen point
        Vector3 trayScreenPoint = Camera.main.WorldToScreenPoint(trayWorldPosition);

        // Chuyển screen point sang local point trong canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransformUI,
            trayScreenPoint,
            null,
            out Vector2 trayCanvasLocalPoint
        );

        // Trường hợp hiếm khi chuyển đổi thất bại (ngoài màn hình v.v.)
       // rectTransformImage.gameObject.SetActive(false);
    }
}
