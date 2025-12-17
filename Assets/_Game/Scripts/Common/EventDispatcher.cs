using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventDispatcher
{
    

    private static Dictionary<EventId, UnityAction<object>> listenner = new Dictionary<EventId,UnityAction<object>>();

    public static void Register(EventId id, UnityAction<object> callback)
    {
        if (HasExistEvent(id))
        {
            listenner[id] += callback;
        }
        else
        {
            listenner.Add(id, callback);
        }
    }

    public static bool HasExistEvent(EventId id)
    {
        return listenner.ContainsKey(id);
    }

    public static void Push(EventId id, object data = null)
    {
        if (!HasExistEvent(id))
        {
            Debug.LogError($"Can not push. The event {id} not exist.");
            return;
        }
        
        listenner[id]?.Invoke(data);
    }

    public static void RemoveCallback(EventId id, UnityAction<object> callback)
    {
        if (!HasExistEvent(id))
        {
            Debug.LogError($"Can not remove callback. The event {id} not exist.");
            return;
        }

        listenner[id] -= callback;

        if (listenner[id] == null)
        {
            RemoveEvent(id);
        }
        
    }

    public static void RemoveEvent(EventId id)
    {
        if (!HasExistEvent(id))
        {
            Debug.LogError($"Can not remove event. The event {id} not exist.");
            return;
        }

        listenner.Remove(id);
    }

    public static void RemoveAll()
    {
        listenner.Clear();
    }
}
