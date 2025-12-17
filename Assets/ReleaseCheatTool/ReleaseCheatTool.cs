using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PS.UnitTest
{
    public class ReleaseCheatTool : PS.Utils.Singleton<ReleaseCheatTool>
    {
        [SerializeField] private InputField inputCode;
        List<int> passCode = new List<int>()
        {
            0, 1, 1, 1, 0, 1, 0, 0, 1
        };

        private int index = 0;
        private bool unlock;

        private string passCodeStr = "123242123Zo23Zo23Uong";

        private CheatToolDatabase db;

        public PopupConfirm PopupConfirm;

        protected override void CustomAwake()
        {
            db = new CheatToolDatabase();

            if (db.IsEnabledCheat)
            {
                db.IsEnabledCheat = false;
                Debug.unityLogger.logEnabled = true;
            }
            else
            {
                Debug.unityLogger.logEnabled = false;
            }
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#endif
            inputCode.gameObject.SetActive(false);

            if (PopupConfirm != null)
            {
                PopupConfirm.ShowBuild();
            }
        }

        public void OnEnabledCheatClick(int number)
        {
            if (unlock)
            {
                return;
            }

            if (passCode[index] == number)
            {
                index++;
            }
            else
            {
                index = 0;
            }

            if (index >= passCode.Count)
            {
                // unlock
                unlock = true;
                EnabledInputCode();
            }
        }

        void EnabledInputCode()
        {
            inputCode.gameObject.SetActive(true);
        }

        public void OnInputCodeChangedValue(string value)
        {
            if (value == passCodeStr)
            {
                inputCode.gameObject.SetActive(false);
                db.IsEnabledCheat = true;
            }
        }

    }
}
