using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ToggleZoomUIStep : IStep
{
    [SerializeField] private Transform tfmZoomUI;
    [SerializeField] private bool isShow;
    [SerializeField] private bool showCompleted = false;
    public override async UniTask Execute()
    {
        if (isShow)
        {
            tfmZoomUI.gameObject.SetActive(true);
            Show();
            showCompleted = false;
        }
        else
        {

            tfmZoomUI.gameObject.SetActive(false);
        }
        await UniTask.WaitUntil(() => showCompleted);
    }
    private async UniTask Show()
    {
        await tfmZoomUI.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        SliderZoom.Instance.HighLight2ButtonZoom(true);
    }
    public void OnCompleteShow()
    {
        showCompleted = true;
        SliderZoom.Instance.HighLight2ButtonZoom(false);

    }
}
