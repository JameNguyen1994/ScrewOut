using PS.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwipeRotation360Degrees : Singleton<SwipeRotation360Degrees>
{
    private bool isLockAutoRotation;
    public bool IsLockRotation;
    private UnityAction onTutorialRotation;

    public UnityAction OnTutorialRotation
    {
        get => onTutorialRotation;
        set
        {
            onTutorialRotation = value;
            mobileCtrl?.OnTutorialRotation(value);
        }
    }

    public bool IsLockAutoRotation
    {
        get => isLockAutoRotation;
        set
        {
            mobileCtrl?.SetLockAutoRotation(value);
            isLockAutoRotation = value;
        }
    }

    [SerializeField] float maxRotationSpeed = 5f; // max rotate speed
    [SerializeField] float acceleration = 10f;   // a when hold mouse
    [SerializeField] float deceleration = 5f; // a when release mouse
    [SerializeField] private float rotationSpeedMultiplier = 0.1f;
    [SerializeField] private float delayTimeToAutoRotate = 3;
    [SerializeField] private Vector3 autoRotateDirection;
    [SerializeField] private float autoRotationSpeed;
    [SerializeField] private float swipeThreshold = 0.125f;
    [SerializeField] private float swipeBoostThreshold = 100;
    [SerializeField] private float stopThreshold = 1;

    private float currentSpeed = 0f;   // current rotation speed
    private float targetSpeed = 0f;

    private Vector3 lastMousePosition;
    private Vector3 firstMousePosition;
    private bool isDragging = false;
    private Vector2 currentDirection;

    private Vector2 prevDirection;

    private float timeCountToAutoRotation = 0;

    private float timeCountSwipe = 0;
    private float lastFrameTime = 0;

    private bool isReduceSpeed = false;
    private float prevDistance = 0;

    private Swipe360DegreeForMobile mobileCtrl;

    private void Start()
    {
        Application.targetFrameRate = 120;
        timeCountToAutoRotation = delayTimeToAutoRotate;
        mobileCtrl = GetComponent<Swipe360DegreeForMobile>();
    }



    public void SwipeRotate()
    {
        if (IsLockRotation) return;
        if (GameManager.Instance.GameState == GameState.Stop) return;
        if (BoosterController.Instance.IsShowTutorial || PopupController.Instance.PopupCount > 0) return;
        if (!BoosterController.Instance.UsingHammer && BoosterController.Instance.CurrentAnimationBooster != BoosterType.None)
        {
            // Debug.Log("None");
            return;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
            firstMousePosition = lastMousePosition;
            timeCountSwipe = Time.time;
            lastFrameTime = timeCountSwipe;
            prevDistance = 0;
            //currentSpeed = 0;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;

            Vector3 delta = mousePosition - lastMousePosition;

            currentDirection = new Vector2(delta.x, delta.y).normalized;

            if (currentDirection.magnitude > 0)
            {
                prevDirection = Vector2.Lerp(prevDirection, currentDirection, 10 * Time.deltaTime);
            }

            var distance = delta.magnitude;

            if (distance > stopThreshold)
            {

                // if (distance < 5)
                // {
                //     rotationSpeedMultiplier = 0.9f;
                // }else if (distance < 10)
                // {
                //     rotationSpeedMultiplier = 1f;
                // }
                // else if (distance < 20)
                // {
                //     rotationSpeedMultiplier = 1.2f;
                // }
                // else
                // {
                //     rotationSpeedMultiplier = 1.5f;
                // }
                currentSpeed = Mathf.Clamp(currentSpeed + distance * rotationSpeedMultiplier, 8, maxRotationSpeed);

                //print($"boost distance: {distance}");
                if (distance >= 50)
                {
                    targetSpeed = Mathf.Clamp(distance * rotationSpeedMultiplier * 2, currentSpeed, maxRotationSpeed);
                }

                // Debug.Log(distance / Time.deltaTime);

                // if (distance / Time.deltaTime > swipeBoostThreshold)
                // {
                //     currentSpeed =  Mathf.Clamp(maxRotationSpeed, 10, maxRotationSpeed);
                // }

                //print(currentSpeed);

                if (currentSpeed > 0)
                {
                    RotateObject(prevDirection, currentSpeed);
                }

                if (currentSpeed >= 10)
                {
                    onTutorialRotation?.Invoke();
                }
            }
            else
            {
                targetSpeed = 0;
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

                if (currentSpeed > 0)
                {
                    RotateObject(prevDirection, currentSpeed);
                }
            }

            lastMousePosition = mousePosition;

            lastFrameTime = Time.time;
        }
        else if (!isDragging)
        {
            targetSpeed = 0;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

            if (currentSpeed > 0)
            {
                RotateObject(prevDirection, currentSpeed);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            lastFrameTime = Time.time;

            if (currentSpeed >= 10)
            {
                onTutorialRotation?.Invoke();
            }

            var mousePosition = Input.mousePosition;

            Vector3 delta = mousePosition - firstMousePosition;

            var distance = delta.magnitude;

            var time = Time.time - timeCountSwipe;

            if (time <= swipeThreshold && distance >= 500)
            {
                currentSpeed = targetSpeed = Mathf.Clamp(distance, 0, 500);
                isReduceSpeed = true;
            }
            else
            {
                targetSpeed -= 20 * Time.deltaTime; ;
            }
        }

        if (isReduceSpeed)
        {
            targetSpeed -= targetSpeed * Time.deltaTime;

            if (targetSpeed <= 0)
            {
                targetSpeed = 0;
                isReduceSpeed = false;
            }
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        if (currentSpeed > 0)
        {
            RotateObject(prevDirection, currentSpeed);
        }

        // if (currentSpeed > 0)
        // {
        //     RotateObject(prevDirection, currentSpeed);
        // }

        AutoRotation();
#endif

#if !UNITY_EDITOR
        mobileCtrl?.UpdateRotation();
#endif
    }

    void AutoRotation()
    {
        if (IsLockAutoRotation) return;

        if (isDragging)
        {
            timeCountToAutoRotation = delayTimeToAutoRotate;
            return;
        }

        timeCountToAutoRotation -= Time.deltaTime;

        if (timeCountToAutoRotation > 0)
        {
            return;
        }

        timeCountToAutoRotation = 0;
        //transform.Rotate(autoRotateDirection * (autoRotationSpeed * Time.deltaTime), Space.World);
    }

    void RotateObject(Vector2 direction, float speed)
    {
        transform.Rotate(Vector3.up, -direction.x * speed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right, direction.y * speed * Time.deltaTime, Space.World);
    }

    public void SetDefaultData()
    {
        maxRotationSpeed = 300;
        acceleration = 4;
        deceleration = 3.5f;
        rotationSpeedMultiplier = 0.5f;
        delayTimeToAutoRotate = 3;
        autoRotateDirection = new Vector3(0, 1, 0);
        autoRotationSpeed = 5;
        swipeThreshold = 0.5f;
        swipeBoostThreshold = 200;
        stopThreshold = 1;
    }
}
