using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace ps.modules.leaderboard
{


    public class LeaderboardManager : MonoBehaviour
    {

        private static LeaderboardManager _instance;
        public static LeaderboardManager Instance => _instance;
        private Dictionary<Type, object> _controllers = new Dictionary<Type, object>();
        [SerializeField] private List<LeaderBoardCtrBase> controllers;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void RegisterController<T>(T controller) where T : LeaderBoardCtrBase
        {
            _controllers[controller.GetType()] = controller;
            controllers.Add(controller);
        }

        public T GetController<T>()
        {
            /* Debug.Log($"GetController<{typeof(T).Name}>");
             Debug.Log($"_controllers count: {_controllers.Count}");
             foreach (var controllera in _controllers)
             {
                 Debug.Log($"Controller Type: {controllera.Key.Name}");
             }*/
            return _controllers.TryGetValue(typeof(T), out var controller) ? (T)controller : default;
        }
    }
}
