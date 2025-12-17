using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DuckDanciingEOL : EOLBase
{
    private static readonly int IsDancing = Animator.StringToHash("IsDancing");
    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 targetRotation;
    [SerializeField] private float rotationDuration;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector3 targetZoom = new Vector3(0, 3.9f, 29.8f);
    
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

        await UniTask.Delay(1000);

        //EOLManager.Instance.SetActiveLevel10();

        //await UniTask.Delay(500);
        AudioController.Instance.ChangeMusic(SoundName.DuckDance, true);
        animator.SetBool(IsDancing, true);

        await UniTask.Delay(10000);

        AudioController.Instance.ChangeMusic(SoundName.Music, true);

        //AudioController.Instance.StopMusic(SoundName.DuckDance);
    }
}
