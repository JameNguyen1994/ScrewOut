using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WaitUntilStep : IStep
{
    public bool isContinue;
    
    public override async UniTask Execute()
    {
        await UniTask.WaitUntil(() => isContinue);
    }
}
