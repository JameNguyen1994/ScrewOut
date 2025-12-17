using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PS.Analytic.RemoteConfig
{
    public class GameAnalyticRemoteTestItem : MonoBehaviour
    {
        [SerializeField] private Text nameTxt, valueTxt;

        public void Init(string name, string value)
        {
            nameTxt.text = name;
            valueTxt.text = value;
        }
    }


}

