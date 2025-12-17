using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SettingGameStep : IStep
{
    [SerializeField] private bool isLockAutoRotation;
    [SerializeField] private bool isLockRotation;
    [SerializeField] private bool isLockInputHandler;
    public override async UniTask Execute()
    {
        InputHandler.Instance.IsLockInput = isLockInputHandler;
        SwipeRotation360Degrees.Instance.IsLockRotation = isLockRotation;
        SwipeRotation360Degrees.Instance.IsLockAutoRotation = isLockAutoRotation;
    }
}
