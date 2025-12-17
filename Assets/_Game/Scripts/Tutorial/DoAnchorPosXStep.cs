using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DoAnchorPosXStep : IStep
{
    [SerializeField] private float targetX;
    [SerializeField] private float duration;
    public override async UniTask Execute()
    {
        await transform.GetComponent<RectTransform>().DOAnchorPosX(targetX, duration).ToUniTask();
    }
}
