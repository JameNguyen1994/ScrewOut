using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Storage;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlyEffectController : Singleton<FlyEffectController>
{
    [SerializeField] private Transform parent;
    
    [SerializeField] private FlyBase coinFlyPrefab;
    [SerializeField] private Transform testTarget;
    [SerializeField] private Transform testCenter;
    
    

    public async UniTask DOFlyCoin(int totalCoin, Vector3 centerPos, Vector3 targetPos)
    {
        if (totalCoin==0)
        {
            Debug.LogWarning("<color=red>totalCoin is 0</color>");
            return;
        }
        int numOfFlyObject =  Random.Range(10, 20);
        
        if (totalCoin > 10)
        {
            numOfFlyObject = Mathf.Clamp(totalCoin, 10, 20);
        }
        else
        {
            numOfFlyObject = totalCoin;
        }
        
        
        int amount = totalCoin / numOfFlyObject;
        int amountLeft = totalCoin % numOfFlyObject;
        
        List<UniTask> tasks = new List<UniTask>();

        for (int i = 0; i < numOfFlyObject; i++)
        {
            if (i == numOfFlyObject - 1)
            {
                amount += amountLeft;
            }
            
            var coinObj = Instantiate(coinFlyPrefab, parent);
            Vector3 startPos = centerPos + new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0);
            coinObj.transform.position = startPos;
            coinObj.transform.SetSiblingIndex(0);
            
            //Debug.Log($"<color=red>startPos: {startPos}</color>");
            
            //lstFlyBase.Add(coinObj);
            
            var task = coinObj.Execute(startPos, targetPos, amount, 2600, OnUpdateCoinUI);
            tasks.Add(task);
            await UniTask.Delay(Random.Range(50, 100));
        }

        await UniTask.WhenAll(tasks);

        // for (int i = 0; i < lstFlyBase.Count; i++)
        // {
        //     lstFlyBase[i].Execute(lstFlyBase[i].transform.position, targetPos, amount, 1000, OnUpdateCoinUI);
        //     await UniTask.Delay(100);
        // }
    }

    void OnUpdateCoinUI(int amount)
    {
        EventDispatcher.Push(EventId.OnIncreaseCoinWithFx, amount);
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         DOFlyCoin(1000, testCenter.position, testTarget.position);
    //     }
    // }
}
