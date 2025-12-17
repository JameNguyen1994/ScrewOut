using System.Collections;
using System.Collections.Generic;
using PS.Analytic.RemoteConfig;
using UnityEngine;

namespace PS.Analytic.RemoteConfig
{
    public class GameAnalyticRemoteTestUI : MonoBehaviour
    {
        [SerializeField] private GameAnalyticRemoteTestItem itemPrefab;
        [SerializeField] private Transform content;

        public void Spawn(List<UnitTestItem> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = Instantiate(itemPrefab, content);
                item.Init(list[i].name, list[i].value);
            }
        
        }

        public void Exit()
        {
            Destroy(gameObject);
        }
    }

}


