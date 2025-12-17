using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
// using EventManager;
using UnityEngine;

public class NetworkCheckerHub : MonoBehaviour
{
    [SerializeField] private Transform main;

    private bool isOpen;
    private float currentTimeScale;

    public void RegisterEvent()
    {
        Register();
    }

    void Register()
    {
        EventDispatcher.Register(EventId.ShowNetworkError, Open);
    }

    void Open(object data)
    {
        if(isOpen) return;
        currentTimeScale = Time.timeScale;
        isOpen = true;
        gameObject.SetActive(true);
        Time.timeScale = 0;
        main.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void Close()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable ) return;
        main.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            Time.timeScale = currentTimeScale;
            isOpen = false;
            gameObject.SetActive(false);
        }).SetUpdate(true);
    }

    private void OnDestroy()
    {
        EventDispatcher.RemoveCallback(EventId.ShowNetworkError, Open);
    }
}
