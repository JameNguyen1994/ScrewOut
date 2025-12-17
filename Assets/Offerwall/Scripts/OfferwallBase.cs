using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class OfferwallBase : MonoBehaviour
{
    public event UnityAction<Dictionary<string, object>> OnImpressionEvent;
    public event UnityAction<Dictionary<string, object>> OnCloseEvent;
    public bool IsInitialized { get; protected set; }
    public abstract void Show();
    public abstract void Init(Dictionary<string, object> data);
    public abstract void FetchReward();

    protected void InvokeImpression(Dictionary<string, object> data)
    {
        OnImpressionEvent?.Invoke(data);
    }

    protected void InvokeClose(Dictionary<string, object> data)
    {
        OnCloseEvent?.Invoke(data);
    }
}
