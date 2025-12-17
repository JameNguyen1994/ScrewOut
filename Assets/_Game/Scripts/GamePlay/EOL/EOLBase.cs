using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class EOLBase : MonoBehaviour
{
    public abstract UniTask Execute();
}
