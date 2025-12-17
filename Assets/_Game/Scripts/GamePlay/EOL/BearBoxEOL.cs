using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class BearBoxEOL : EOLBase
{
    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 targetRotation;
    [SerializeField] private float rotationDuration;
    [SerializeField] private Vector3 targetZoom = new Vector3(0, 3.9f, 29.8f);
    [SerializeField] private GameObject fxHeart;
    
    public override async UniTask Execute()
    {
        InputHandler.Instance.IsLockInput = true;
        SwipeRotation360Degrees.Instance.IsLockRotation = true;
        SwipeRotation360Degrees.Instance.IsLockAutoRotation = true;
        await DoEOL();
    }

    private async UniTask DoEOL()
    {
        await parent.DORotate(targetRotation, rotationDuration).SetEase(Ease.OutBack);
        await parent.DOMove(targetZoom, 0.2f);
        fxHeart.SetActive(true);

        await UniTask.Delay(3000);
    }
}
