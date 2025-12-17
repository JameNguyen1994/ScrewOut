using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwaitColorSwitchClick : MonoBehaviour
{
    [SerializeField] private WaitUntilStep stepWaiter;
    [SerializeField] private ToogleColorMode toogleColorMode;

    private void Start()
    {
        toogleColorMode.onClicked += WaitToCompleted;
    }

    void WaitToCompleted()
    {
        stepWaiter.isContinue = true;
        toogleColorMode.onClicked -= WaitToCompleted;
    }
}
