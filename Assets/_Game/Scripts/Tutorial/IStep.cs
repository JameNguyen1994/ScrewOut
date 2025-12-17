using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class IStep : MonoBehaviour
{
    [SerializeField] private bool isWaitingComplete;
    public bool IsWaitingComplete => isWaitingComplete;
    
    public abstract UniTask Execute();
}
