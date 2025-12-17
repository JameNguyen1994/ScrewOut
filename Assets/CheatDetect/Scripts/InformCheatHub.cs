using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

public class InformCheatHub : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        ObscuredCheatingDetector.StartDetection(OnCheaterDetected);
    }

    void OnCheaterDetected()
    {
        var hub = transform.GetChild(0);

        if (hub)
        {
            hub.gameObject.SetActive(true);
        }
    }
}
