using System;
using System.Collections;
using System.Collections.Generic;
// using EventManager;
using UnityEngine;
using UnityEngine.Events;

public class NetworkChecker : MonoBehaviour
{
    [SerializeField] private float recheckTime = 1;
    [SerializeField] private NetworkCheckerHub _networkCheckerHub;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        _networkCheckerHub.RegisterEvent();
        StartCoroutine(NetworkCheck());
    }

    IEnumerator NetworkCheck()
    {
        Check();
        
        yield return new WaitForSecondsRealtime(recheckTime);

        StartCoroutine(NetworkCheck());
    }

    void Check()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            EventDispatcher.Push(EventId.ShowNetworkError);
        }
    }
}
