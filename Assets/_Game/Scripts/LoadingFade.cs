using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LoadingFade : Singleton<LoadingFade>
{
    [SerializeField] private List<Image> lstImageFade;
    [SerializeField] private float timeClose = 0.8f;
    [SerializeField] private float timeOpen = 0.5f;
    [SerializeField] private Ease easeOpen = Ease.OutQuad;
    [SerializeField] private Ease easeClose = Ease.OutQuad;
    [SerializeField] private List<Sprite> lstSprTop;
    [SerializeField] private List<Sprite> lstSprMid;
    [SerializeField] private Image imgTop;
    [SerializeField] private Image imgMid;
    [SerializeField] private Image imgBackground;

    [SerializeField] private List<int> lstIdAvailable;

    private void Start()
    {
        foreach (var image in lstImageFade)
        {
            image.gameObject.SetActive(false);
            lstIdAvailable.Add(lstIdAvailable.Count);
        }
    }
    private void SetUp()
    {
        if (lstIdAvailable.Count == 0)
        {
            for (int i = 0; i < lstSprTop.Count; i++)
            {
                lstIdAvailable.Add(i);
            }
        }
        int indexId = UnityEngine.Random.Range(0, lstIdAvailable.Count);
        int id = lstIdAvailable[indexId];
        lstIdAvailable.RemoveAt(indexId);

        imgTop.sprite = lstSprTop[id];
        imgMid.sprite = lstSprMid[id];
        imgMid.transform.localScale = Vector3.one * 0.5f;
    }
    public async UniTask ShowLoadingFade()
    {
        DOKill();
        imgBackground.gameObject.SetActive(true);

        imgBackground.DOFade(1, 0.3f).From(0).SetEase(Ease.OutQuad);
        SetUp();
        imgMid.transform.DOScale(Vector3.one, timeOpen / 2).SetEase(easeOpen);
        var lstTask = new List<UniTask>();
        foreach (var imgFade in lstImageFade)
        {
            imgFade.gameObject.SetActive(true);
            lstTask.Add(imgFade.DOFade(1, timeOpen).From(0).SetEase(easeOpen).ToUniTask());
        }
        await UniTask.WhenAll(lstTask);

    }
    private void DOKill()
    {
        imgBackground.DOKill();
        imgMid.transform.DOKill();
        foreach (var imgFade in lstImageFade)
        {
            imgFade.DOKill();
        }
    }
    public async UniTask HideLoadingFade()
    {
        DOKill();
        var lstTask = new List<UniTask>();
        foreach (var imgFade in lstImageFade)
        {
            lstTask.Add(imgFade.DOFade(0, timeClose).From(1).SetEase(easeClose).ToUniTask());

        }
        imgMid.transform.DOScale(Vector3.one*0.5f, timeClose / 2).SetEase(easeClose);
        imgBackground.DOFade(0, timeClose*3/2).From(1).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

        await UniTask.WhenAll(lstTask);
        imgBackground.gameObject.SetActive(false);

        foreach (var image in lstImageFade)
        {
            image.gameObject.SetActive(false);
        }
    }
}
