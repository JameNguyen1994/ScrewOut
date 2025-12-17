using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class FlyBase : MonoBehaviour
{
    public abstract UniTask Execute(Vector3 startPos, Vector3 targetPos, int amount, float duration, UnityAction<int> onUpdateUI);
    public abstract UniTask ExecuteLocal(Vector3 startPos, Vector3 targetPos, int amount, float duration, UnityAction<int> onUpdateUI);
}
