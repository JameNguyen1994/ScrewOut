using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private readonly Queue<System.Action> _executionQueue = new Queue<System.Action>();

    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("UnityMainThreadDispatcher");
                _instance = obj.AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                var action = _executionQueue.Dequeue();
                action?.Invoke();
            }
        }
    }

    public void Enqueue(System.Action action)
    {
        if (action == null) return;
        
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    public void Enqueue<T>(System.Action<T> action, T param)
    {
        if (action == null) return;

        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => action?.Invoke(param));
        }
    }

    public void Enqueue<T1, T2>(System.Action<T1, T2> action, T1 param1, T2 param2)
    {
        if (action == null) return;

        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => action?.Invoke(param1, param2));
        }
    }
}
