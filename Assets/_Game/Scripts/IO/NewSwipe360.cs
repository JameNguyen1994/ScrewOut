using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSwipe360 : MonoBehaviour
{
    public float rotationSpeed = 0.1f;  // Tốc độ xoay cơ bản
    public float friction = 0.95f;      // Hệ số ma sát (giảm tốc độ dần)
    public float maxRotationSpeed = 5f; // Giới hạn tốc độ xoay tối đa
    private Vector2 previousPosition;   // Vị trí trước đó của ngón tay
    private Vector2 velocity = Vector2.zero; // Vận tốc di chuyển
    private bool isDragging = false;    // Trạng thái kéo

    void Update()
    {
     /*   if (!BoosterController.Instance.UsingHammer && BoosterController.Instance.CurrentAnimationBooster != BoosterType.None)
        {
            Debug.Log("None");
            return;
        }
                ;
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            previousPosition = Input.mousePosition;
            velocity = Vector2.zero; // Reset vận tốc khi bắt đầu kéo
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            // Lấy vị trí hiện tại và tính deltaPosition
            Vector2 currentPosition = Input.mousePosition;
            Vector2 deltaPosition = currentPosition - previousPosition;

            if (deltaPosition.magnitude > 0.01f) // Nếu ngón tay đang di chuyển
            {
                velocity = deltaPosition / Time.deltaTime;
                previousPosition = currentPosition; // Cập nhật vị trí trước đó
            }
            else
            {
                // Ngón tay không di chuyển, giảm dần vận tốc
                velocity *= friction;
            }

            // Giới hạn tốc độ xoay để tránh quá nhanh
            velocity = Vector2.ClampMagnitude(velocity, maxRotationSpeed);
        }
        else
        {
            // Nếu không kéo, giảm dần vận tốc
            velocity *= friction;
        }

        // Xoay đối tượng
        if (velocity.magnitude > 0.01f) // Nếu vận tốc còn đủ lớn
        {
            float rotationX = velocity.y * rotationSpeed * Time.deltaTime; // Xoay quanh trục X
            float rotationY = -velocity.x * rotationSpeed * Time.deltaTime; // Xoay quanh trục Y

            transform.Rotate(Vector3.right, rotationX, Space.World); // Xoay quanh trục X
            transform.Rotate(Vector3.up, rotationY, Space.World);   // Xoay quanh trục Y
        }*/
    }
}
