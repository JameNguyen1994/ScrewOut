using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeRotation360DegreesWithRigidbody : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private Vector2 swipeDelta;

    private float swipeVelocity;
    private float lastFrameTime;
    private Vector2 lastTouchPosition;

    private Vector3 rotationAxis;
    private float targetTorque = 0f;
    private float currentTorque = 0f;

    public Rigidbody rb; // Rigidbody gắn trên object
    public float torqueMultiplier = 10f; // Hệ số lực xoay
    public float accelerationFactor = 5f; // Gia tốc mượt mà
    public float maxTorque = 50f; // Giới hạn lực xoay
    public float swipeBoostThreshold = 500f; // Ngưỡng vận tốc vuốt để kích hoạt boost
    public float boostMultiplier = 2f; // Hệ số tăng tốc khi vuốt nhanh
    public float angularDrag = 2f; // Lực cản quay

    private bool isSwiping = false;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.angularDamping = angularDrag; // Đặt lực cản quay
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    lastTouchPosition = touch.position;
                    lastFrameTime = Time.time;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                    currentTouchPosition = touch.position;
                    swipeDelta = currentTouchPosition - startTouchPosition;

                    // Tính trục xoay từ hướng swipe
                    rotationAxis = new Vector3(-swipeDelta.y, swipeDelta.x, 0).normalized;

                    // Tính vận tốc vuốt
                    swipeVelocity = (currentTouchPosition - lastTouchPosition).magnitude / (Time.time - lastFrameTime);

                    // Tính torque mục tiêu dựa trên khoảng cách vuốt
                    targetTorque = Mathf.Clamp(swipeDelta.magnitude * torqueMultiplier, 0, maxTorque);

                    // Nếu vận tốc vuốt vượt ngưỡng, tăng torque mục tiêu
                    if (swipeVelocity > swipeBoostThreshold)
                    {
                        targetTorque *= boostMultiplier;
                    }

                    lastTouchPosition = currentTouchPosition;
                    lastFrameTime = Time.time;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isSwiping = false;
                    targetTorque = 0f; // Khi dừng swipe, torque giảm về 0
                    break;
            }
        }
        else if (Input.GetMouseButtonDown(0)) // Test trên Editor
        {
            startTouchPosition = Input.mousePosition;
            lastTouchPosition = Input.mousePosition;
            lastFrameTime = Time.time;
            isSwiping = true;
        }
        else if (Input.GetMouseButton(0) && isSwiping)
        {
            currentTouchPosition = Input.mousePosition;
            swipeDelta = (Vector2)currentTouchPosition - lastTouchPosition;

            rotationAxis = new Vector3(swipeDelta.y, -swipeDelta.x, 0).normalized;

            if (swipeDelta.magnitude > 2)
            {
                targetTorque = Mathf.Clamp(swipeDelta.magnitude * torqueMultiplier, 0, maxTorque);
            }
            else
            {
                targetTorque = 0;
            }
            //
            // if (swipeVelocity > swipeBoostThreshold)
            // {
            //     targetTorque *= boostMultiplier;
            // }

            lastTouchPosition = currentTouchPosition;
            lastFrameTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
            targetTorque = 0f;
        }

        // Mượt mà hóa lực xoay
        currentTorque = Mathf.Lerp(currentTorque, targetTorque, Time.deltaTime * accelerationFactor);
    }

    void FixedUpdate()
    {
        if (currentTorque > 0.01f) // Nếu lực xoay đáng kể
        {
            // Áp dụng lực xoay cho Rigidbody
            rb.AddTorque(rotationAxis * (currentTorque * Time.fixedDeltaTime), ForceMode.Impulse);
        }
    }
}
