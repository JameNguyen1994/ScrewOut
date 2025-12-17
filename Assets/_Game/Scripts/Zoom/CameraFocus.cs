using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    public Camera mainCamera;          // Camera chính
    public Transform parentObject;     // Đối tượng cha chứa các object con
    public float smoothTime = 0.3f;    // Thời gian di chuyển mượt
    public float distanceMultiplier = 1.5f; // Hệ số nhân để đảm bảo vùng nhìn bao phủ toàn bộ object con
    public float minDistance = 5f;     // Khoảng cách tối thiểu từ camera đến object
    public float maxDistance = 50f;    // Khoảng cách tối đa từ camera đến object

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (parentObject.childCount == 0)
            return;

        Vector3 centerPoint = CalculateCenterPoint();
        float maxDistance = CalculateMaxDistance(centerPoint);
        float requiredDistance = Mathf.Clamp(maxDistance * distanceMultiplier, minDistance, this.maxDistance);

        AdjustCamera(centerPoint, requiredDistance);
    }

    Vector3 CalculateCenterPoint()
    {
        Vector3 totalPosition = Vector3.zero;

        for (int i = 0; i < parentObject.childCount; i++)
        {
            totalPosition += parentObject.GetChild(i).position;
        }

        return totalPosition / parentObject.childCount;
    }

    float CalculateMaxDistance(Vector3 centerPoint)
    {
        float maxDistance = 0f;

        for (int i = 0; i < parentObject.childCount; i++)
        {
            float distance = Vector3.Distance(centerPoint, parentObject.GetChild(i).position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }

        return maxDistance;
    }

    void AdjustCamera(Vector3 centerPoint, float requiredDistance)
    {
        Vector3 direction = mainCamera.transform.forward;
        Vector3 targetPosition = centerPoint - direction.normalized * requiredDistance;

        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPosition, ref velocity, smoothTime);
        mainCamera.transform.LookAt(centerPoint);
    }
}
