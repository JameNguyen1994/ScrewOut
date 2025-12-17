using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHardLevel : PopupBase
{
    [SerializeField] protected int timeShow;

    public override async UniTask Show()
    {
        base.Show();
        await UniTask.Delay(timeShow * 1000);
        Hide();
    }
}
