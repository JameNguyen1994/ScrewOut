using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyIncreaseValueEffect : MonoBehaviour
{
    [SerializeField] private EventId eventId;
    [SerializeField] protected Text txtValue;
    [SerializeField] private Transform tfmScaleFx;
    [SerializeField] private GameObject fxPrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private Transform target;
    
    

    private bool isTween;
    
    
    private void OnEnable()
    {
        EventDispatcher.Register(eventId, OnValueChanged);
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveCallback(eventId, OnValueChanged);
    }

    protected virtual void OnValueChanged(object data)
    {
        DoFxBar().Forget();
        // try
        // {
        //     
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError($"class {nameof(CurrencyIncreaseValueEffect)} error: {e.Message}");
        // }
    }

    async UniTask DoFxBar()
    {
        if (isTween)
        {
            return;
        }

        isTween = true;
        var fx = Instantiate(fxPrefab, parent);
        fx.transform.position = target.position;
        await tfmScaleFx.DOScale(Vector3.one * 1.2f, 0.15f).SetEase(Ease.OutBack);
        await tfmScaleFx.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        isTween = false;
    }
}
