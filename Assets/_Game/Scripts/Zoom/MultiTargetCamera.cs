using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class MultiTargetCamera : MonoBehaviour
{
    [SerializeField] private List<Transform> targets;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float zoomDuration = 0.5f;

    private PinchToZoom pinchToZoom;
    private Vector3 fixedCenterPoint; // Lưu trữ tâm cố định
    private bool isCenterCalculated = false;

    public void Initialize(PinchToZoom pinchToZoom)
    {
        this.pinchToZoom = pinchToZoom;
        CalculateFixedCenterPoint(); // Tính toán tâm khi khởi động
    }

    [ContextMenu("Move To Targets")]
    public void MoveToTargets()
    {
        if (targets.Count == 0 || pinchToZoom == null)
            return;

        Vector3 targetPosition = new Vector3(fixedCenterPoint.x, fixedCenterPoint.y, transform.position.z);
        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutQuad);
    }

    [ContextMenu("Zoom To Targets")]
    public void ZoomToTargets()
    {
        //if (targets.Count == 0 || pinchToZoom == null)
        //    return;

        //float requiredZoom = CalculateRequiredZoom(fixedCenterPoint);
        //pinchToZoom.TweenZoom(requiredZoom, zoomDuration);
    }

    private void CalculateFixedCenterPoint()
    {
        if (isCenterCalculated || targets.Count == 0)
            return;

        if (targets.Count == 1)
        {
            fixedCenterPoint = targets[0].position;
        }
        else
        {
            Vector3 totalPosition = Vector3.zero;
            foreach (Transform target in targets)
            {
                totalPosition += target.position;
            }
            fixedCenterPoint = totalPosition / targets.Count;
        }

        isCenterCalculated = true;
        Debug.Log($"Fixed Center Point: {fixedCenterPoint}");
    }

    //private float CalculateRequiredZoom(Vector3 centerPoint)
    //{
    //    float maxDistance = 0f;

    //    foreach (Transform target in targets)
    //    {
    //        float distance = Vector3.Distance(centerPoint, target.position);
    //        if (distance > maxDistance)
    //        {
    //            maxDistance = distance;
    //        }
    //    }

    //    Debug.Log($"Max distance between center and targets: {maxDistance}");
    //    //return Mathf.Clamp(maxDistance * 2f, pinchToZoom.MinZoom, pinchToZoom.MaxZoom);
    //}
}
