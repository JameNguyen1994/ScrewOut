using EasyButtons;
using UnityEngine;

public class SetUpEOLPos : MonoBehaviour
{
    [SerializeField] private Transform tfmLeftTrumpetHolder;
    [SerializeField] private Transform tfmRightTrumpetHolder;
    [SerializeField] private Transform tfmTopTrumpetHolder;
    [SerializeField] private Camera camera;
    [SerializeField] private float offsetZ = 5f; // khoảng cách tính từ camera
    [SerializeField] private float yPos = 0f;    // chiều cao tùy chỉnh

    void Start()
    {
        SetUpPos();
    }
    [Button]
    private void SetUpPos()
    {
        Camera cam = camera;
        if (cam == null) return;

        // Lấy tọa độ X rìa trái và phải (theo viewport)
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, offsetZ));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, offsetZ));
        Vector3 top = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, offsetZ));
        // Chỉ set theo X
        if (tfmLeftTrumpetHolder != null)
        {
            Vector3 pos = tfmLeftTrumpetHolder.position;
            pos.x = left.x;
            pos.y = yPos;
            tfmLeftTrumpetHolder.position = pos;
            tfmLeftTrumpetHolder.localPosition = new Vector3(tfmLeftTrumpetHolder.localPosition.x, 0, offsetZ);
        }

        if (tfmRightTrumpetHolder != null)
        {
            Vector3 pos = tfmRightTrumpetHolder.position;
            pos.x = right.x;
            pos.y = yPos;
            tfmRightTrumpetHolder.position = pos;
            tfmRightTrumpetHolder.localPosition = new Vector3(tfmRightTrumpetHolder.localPosition.x, 0, offsetZ);

        }

        if (tfmTopTrumpetHolder != null)
        {
            Vector3 pos = tfmTopTrumpetHolder.position;
            pos.x = 0;
            pos.y = top.y;
            tfmTopTrumpetHolder.position = pos;
            tfmTopTrumpetHolder.localPosition = new Vector3(0, tfmTopTrumpetHolder.localPosition.y, offsetZ);
        }
    }
}
