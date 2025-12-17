using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using PS.Analytic.Database;
using PS.Analytic.Event;
using PS.Analytic.RemoteConfig;
using PS.Utils;
using UnityEngine;

namespace PS.Analytic
{
    public class GameAnalyticController : Singleton<GameAnalyticController>, IGameAnalyticsATTListener
    {
        private GameAnalyticRemoteConfig remoteConfig;
        private AnalyticsDatabase db;

        private bool isEndSession;

        public GameAnalyticRemoteConfig Remote() => remoteConfig;
        [SerializeField] private GameTrackingByGa tracking;
        public GameTrackingByGa Tracking() => tracking;
        
        private void Start()
        {
            db = new AnalyticsDatabase();
            remoteConfig = new GameAnalyticRemoteConfig();
            GameAnalytics.SetEnabledManualSessionHandling(false);
            Init();
        }

        void Init()
        {
            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GameAnalytics.RequestTrackingAuthorization(this);
            }
            else
            {
                GameAnalytics.Initialize();
                Invoke(nameof(SendSessionStart), 0.5f);
            }
        }

        public void GameAnalyticsATTListenerNotDetermined()
        {
            GameAnalytics.Initialize();
            Invoke(nameof(SendSessionStart), 0.5f);
        }

        public void GameAnalyticsATTListenerRestricted()
        {
            GameAnalytics.Initialize();
            Invoke(nameof(SendSessionStart), 0.5f);
        }

        public void GameAnalyticsATTListenerDenied()
        {
            GameAnalytics.Initialize();
            Invoke(nameof(SendSessionStart), 0.5f);
        }

        public void GameAnalyticsATTListenerAuthorized()
        {
            GameAnalytics.Initialize();
            Invoke(nameof(SendSessionStart), 0.5f);
        }

//        private IEnumerator OnApplicationPause(bool pauseStatus)
//        {
//            if (!pauseStatus)
//            {
//                print("================> IS FOCUS STATUS");
//                yield return new WaitForSeconds(0.1f);
//                SendSessionStart();
//            }
//            else
//            {
//                print("===============> IS PAUSE STATUS");
//            }
//        }
        
        void SendSessionStart()
        {
            string isFirstOen = db.IS_FIRST_OPEN ? "true" : "false";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("is_first_open", isFirstOen);
            GameAnalyticEvent.Event("is_first_open", dic);
            db.IS_FIRST_OPEN = false;
            
            GameAnalyticEvent.Event("device_resolution", new Dictionary<string, object>()
            {
                {"resolution", $"{Screen.width}x{Screen.height}"}
            });
        }

        [ContextMenu("Test Remote")]
        public void UnitTest()
        {
            var list = Remote().UnitTest();

            var obj = Resources.Load<GameAnalyticRemoteTestUI>("GARemoteUnitTest");

            if (obj != null)
            {
                var uiTest = Instantiate(obj, new Vector3(0,0,0), Quaternion.identity);
                if (uiTest != null)
                {
                    uiTest.Spawn(list);
                }
            }
        }
    }
}

