using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActiveObjectStep : IStep
{
    [SerializeField] private bool enable = false;
    public override async UniTask Execute()
    {
        gameObject.SetActive(enable);
    }
}
