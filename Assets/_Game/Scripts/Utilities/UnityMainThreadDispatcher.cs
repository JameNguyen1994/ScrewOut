using System;
using System.Collections.Generic;
using UnityEngine;

namespace OfferWall
{

    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static UnityMainThreadDispatcher _instance;
        private readonly Queue<Action> _executionQueue = new Queue<Action>();

        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("[AdsLab]UnityMainThreadDispatcher");
                    _instance = obj.AddComponent<UnityMainThreadDispatcher>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
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

        public void Enqueue(Action action)
        {
            if (action == null) return;

            lock (_executionQueue)
            {
                _executionQueue.Enqueue(action);
            }
        }

        public void Enqueue<T>(Action<T> action, T param)
        {
            if (action == null) return;

            lock (_executionQueue)
            {
                _executionQueue.Enqueue(() => action?.Invoke(param));
            }
        }

        public void Enqueue<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
        {
            if (action == null) return;

            lock (_executionQueue)
            {
                _executionQueue.Enqueue(() => action?.Invoke(param1, param2));
            }
        }
    }
}