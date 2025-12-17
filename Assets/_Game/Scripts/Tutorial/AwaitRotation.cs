using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwaitRotation : MonoBehaviour
{
    [SerializeField] private WaitUntilStep stepWaiter;
    private bool isCallOnce;

    private void Start()
    {
        SwipeRotation360Degrees.Instance.OnTutorialRotation += OnRotationCompleted;
    }

    void OnRotationCompleted()
    {
        if(isCallOnce) return;
        isCallOnce = true;
        Invoke(nameof(WaitToCompleted), 1);
    }
    
    void WaitToCompleted()
    {
        stepWaiter.isContinue = true;
        SwipeRotation360Degrees.Instance.OnTutorialRotation -= OnRotationCompleted;
    }
}
