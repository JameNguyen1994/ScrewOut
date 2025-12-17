using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Swipe360DegreeForMobile : MonoBehaviour
{
    bool IsLockAutoRotation;
    UnityAction onTutorialRotation;

    [SerializeField] private float delayTimeToAutoRotate = 3;
    [SerializeField] float rotationSpeed = 6;
    [SerializeField] float maxSpeed = 6;
    private Vector2 lastDelta;
    private bool isDragging = false;

    [SerializeField] float inertiaThreshold = 5;

    [SerializeField] private Vector3 autoRotateDirection = new Vector3(0, -1, 0);
    [SerializeField] private float autoRotationSpeed = 5;

    private bool isBoost;
    private float timeCountToAutoRotation = 0;

    public void SetLockAutoRotation(bool isLock)
    {
        this.IsLockAutoRotation = isLock;
    }

    public void OnTutorialRotation(UnityAction onTutorialRotation)
    {
        this.onTutorialRotation = onTutorialRotation;
    }

    private void Start()
    {
        timeCountToAutoRotation = delayTimeToAutoRotate;
    }

    public void UpdateRotation()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            timeCountToAutoRotation = delayTimeToAutoRotate;

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;

                if (delta.magnitude > inertiaThreshold)
                {
                    rotationSpeed = maxSpeed;
                    isBoost = false;
                    isDragging = true;
                    float deltaTime = 0;

                    if (delta.magnitude > lastDelta.magnitude)
                    {
                        deltaTime = 18;

                        if (delta.magnitude > 60)
                        {
                            deltaTime = 30;
                        }
                    }
                    else
                    {
                        deltaTime = 20;
                    }

                    lastDelta = Vector2.Lerp(lastDelta, delta, deltaTime * Time.unscaledDeltaTime);
                }

                if (delta.magnitude > 10)
                {
                    onTutorialRotation?.Invoke();
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                rotationSpeed = 8;
            }
            else if (touch.phase == TouchPhase.Stationary)
            {
                if (!isBoost)
                {
                    isBoost = true;
                    isDragging = false;
                    //rotationSpeed = 8;
                }
            }
        }

        float rotationX = lastDelta.y * rotationSpeed;
        float rotationY = -lastDelta.x * rotationSpeed;
        transform.Rotate(new Vector3(rotationX, rotationY, 0) * Time.unscaledDeltaTime, Space.World);

        if (!isDragging && lastDelta.magnitude > 0.01f)
        {
            lastDelta = Vector2.Lerp(lastDelta, Vector2.zero, 5 * Time.unscaledDeltaTime);
        }

        AutoRotation();
    }

    void AutoRotation()
    {
        if (IsLockAutoRotation) return;

        if (isDragging)
        {
            timeCountToAutoRotation = delayTimeToAutoRotate;
            return;
        }

        timeCountToAutoRotation -= Time.unscaledDeltaTime;

        if (timeCountToAutoRotation > 0)
        {
            return;
        }

        timeCountToAutoRotation = 0;
        transform.Rotate(autoRotateDirection * (autoRotationSpeed * Time.unscaledDeltaTime), Space.World);
    }

    public void SetDefaultData()
    {
        delayTimeToAutoRotate = 3;
        rotationSpeed = 6;
        maxSpeed = 17;
        inertiaThreshold = 1;
        autoRotateDirection = new Vector3(0, 1, 0);
        autoRotationSpeed = 5;
    }
}