using System.Collections.Generic;
using UnityEngine;

public class SegmentService : SingularLinkHandler, SingularDeviceAttributionCallbackHandler
{
    public void OnSingularLinkResolved(SingularLinkParams linkParams)
    {
        Debug.Log("Singular Link Resolved");

        // Extract parameters from the tracking link
        string deeplink = linkParams.Deeplink;
        string passthrough = linkParams.Passthrough;
        bool isDeferred = linkParams.IsDeferred;

        // Log the parameters
        Debug.Log($"Deeplink: {deeplink ?? "null"}");
        Debug.Log($"Passthrough: {passthrough ?? "null"}");
        Debug.Log($"Is Deferred: {isDeferred}");
    }

    public void OnSingularDeviceAttributionCallback(Dictionary<string, object> attributionInfo)
    {
        Debug.Log("Singular Device Attribution Callback Received");

        // Log each key-value pair in the attribution info
        foreach (var kvp in attributionInfo)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }
    }
}
