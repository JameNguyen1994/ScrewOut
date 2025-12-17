using System;
using System.Collections;
using System.Collections.Generic;
using PS.Analytic;
using PS.Utils;
using UnityEngine;

namespace PS.UnitTest
{
    public class GlobalCanvas : Singleton<GlobalCanvas>
    {
        [SerializeField] private GameObject testBtn, remoteValueBtn;

        private void Start()
        {
            bool isDebug = Debug.unityLogger.logEnabled;
            
            testBtn.SetActive(isDebug);
            remoteValueBtn.SetActive(isDebug);
        }
        public void EnableButton(bool enable)
        {
            // testBtn.SetActive(enable);
            // remoteValueBtn.SetActive(enable);
        }
        public void OpenTestSuit()
        {
            MaxSdk.ShowMediationDebugger();
        }

        public void OpenRemoteValue()
        {
            GameAnalyticController.Instance.UnitTest();
        }
    }

}
