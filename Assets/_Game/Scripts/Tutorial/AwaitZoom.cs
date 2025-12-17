using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AwaitZoom : MonoBehaviour
{
    [SerializeField] private WaitUntilStep stepWaiter;
    [SerializeField] private SliderZoom zoomer;

    private float defaultValue;

    private void Start()
    {
        defaultValue = zoomer.ZoomSlider.value;
        zoomer.ZoomSlider.onValueChanged.AddListener(OnValueChanged);
        SliderZoom.Instance.HighLight2ButtonZoom(true);
        Debug.Log($"[AwaitZoom] defaultValue: {defaultValue}");
    }

    void OnValueChanged(float slider)
    {
        if (Mathf.Abs(slider - defaultValue) < 0.05f)
        {
            return;
        }
        
        Invoke(nameof(WaitToCompleted), 1);
    }

    void WaitToCompleted()
    {
        stepWaiter.isContinue = true;
        zoomer.ZoomSlider.onValueChanged.RemoveListener(OnValueChanged);
        SliderZoom.Instance.HighLight2ButtonZoom(false);

    }
}
