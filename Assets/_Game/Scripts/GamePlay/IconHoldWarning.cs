using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

public class IconHoldWarning : MonoBehaviour
{
    [SerializeField] private Transform tfmRed;
   private float startScale = 0.75f;
    private float endScale = 1f;

    private CancellationTokenSource cts;

    private void Start()
    {
        RunEffectLoop().Forget();
    }
    public void StartEffect()
    {
        tfmRed.gameObject.SetActive(true);
    }

    public void StopEffect()
    {
        tfmRed.gameObject.SetActive(false);
    }

    private async UniTask RunEffectLoop()
    {
            while (true)
            {
                await tfmRed.DOScale(endScale, 0.5f).SetEase(Ease.InOutSine);
                await tfmRed.DOScale(startScale, 0.5f).SetEase(Ease.InOutSine);
            }
    }

    private void OnDestroy()
    {
        StopEffect();
    }
}
