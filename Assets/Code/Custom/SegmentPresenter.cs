using System;
using UnityEngine;

public class SegmentPresenter : MonoBehaviour
{
    SegmentService segmentService;
    private void Awake()
    {
        InitServices();
    }

    void InitServices()
    {
        segmentService = new SegmentService();
        SingularSDK.SetSingularLinkHandler(segmentService);
        SingularSDK.SetSingularDeviceAttributionCallbackHandler(segmentService);
    }
}
