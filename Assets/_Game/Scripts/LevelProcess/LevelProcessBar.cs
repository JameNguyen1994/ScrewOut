using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using ps.modules.journey;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UniWebViewLogger;

public class LevelProcessBar : Singleton<LevelProcessBar>
{
    [SerializeField] private RectTransform rtfmBarFill;
    [SerializeField] private List<ItemLevelProcess> lstItem;
    [SerializeField] private int level;
    [SerializeField] private int currentIndex;
    [SerializeField] private int minLevel;
    [SerializeField] private int maxLevel;
    [SerializeField] private bool hasReward;
    [SerializeField] private RectTransform rtfmBackgroundGiftHolder;
    [SerializeField] private Image imgFadeBackground;
    [SerializeField] private Text txtNameRoom;

    [Button]
    public void SetUp(int level)
    {
        this.level = level;

        int minLevel = level == 1 ? 1 : level - 1;
        currentIndex = level == 1 ? 0 : 1;
        Debug.Log($"LevelProcessBar SetUp - level: {level}, minLevel: {minLevel}, maxLevel: {maxLevel}, currentIndex: {currentIndex}");

        for (int i = 0; i < lstItem.Count; i++)
        {
            var item = lstItem[i];
            int itemLevel = minLevel + (i);
            if (itemLevel < level)
            {
                item.SetUp(ItemLevelProcessType.Passed, itemLevel);
            }
            else if (itemLevel == level)
            {
                item.SetUp(ItemLevelProcessType.Next, itemLevel);
            }
            else
            {
                item.SetUp(ItemLevelProcessType.Next, itemLevel);
            }
        }
        SetUpBarFill();
        HideBeforeShow();
    }
    private async UniTask HideBeforeShow()
    {
        transform.localScale = Vector3.zero;
        foreach (var item in lstItem)
        {
            item.HideBeforeShow();
        }

    }
    public async UniTask DOShow()
    {
        await transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        var lstTask = new List<UniTask>();
        foreach (var item in lstItem)
        {
            lstTask.Add(item.DOShow());
            await UniTask.Delay(100);
        }
        await UniTask.WhenAll(lstTask);
        await UniTask.Delay(500);
        await AnimtionBarFill();
    }
    public void SetUpBarFill()
    {
        float minWidth = 0;
        float maxWidth = Screen.width;
        float width = 0;
        if (currentIndex == 0)
        {
            width = 0;
        }
        else
            width = lstItem[currentIndex - 1].RtfmItem.anchoredPosition.x;
        rtfmBarFill.sizeDelta = new Vector2(width, rtfmBarFill.sizeDelta.y);
    }
    public async UniTask DOAnimationCurrent()
    {
        if (currentIndex < 0 || currentIndex >= lstItem.Count)
            return;
        var itemCurrent = lstItem[currentIndex];
        await itemCurrent.DOAnimationCurrent();
    }
    [Button]

    public async UniTask AnimtionBarFill()
    {
        if (currentIndex > 0)
        {
            lstItem[currentIndex - 1].DOPassAnimation();
        }
        float minWidth = 0;
        float maxWidth = Screen.width;
        float width = 0;

        width = lstItem[currentIndex].RtfmItem.anchoredPosition.x;


        var targetSize = new Vector2(width, rtfmBarFill.sizeDelta.y);
        await rtfmBarFill.DOSizeDelta(targetSize, 0.5f).SetEase(Ease.OutCubic);
        //GetRewarad();
        await DOAnimationCurrent();

    }
    [Button]

    public void Reset()
    {
        foreach (var item in lstItem)
        {
            item.Reset();
        }
        rtfmBarFill.sizeDelta = new Vector2(0, rtfmBarFill.sizeDelta.y);
    }

    public async UniTask ShowBackgroundReward(Transform tfmBackground, string backroundName)
    {
        tfmBackground.transform.SetParent(rtfmBackgroundGiftHolder, true);
        imgFadeBackground.color = new Color(0, 0, 0, 0);
        imgFadeBackground.gameObject.SetActive(true);
        imgFadeBackground.DOFade(0.9f, 0.5f);
        tfmBackground.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutCubic);
        await tfmBackground.transform.DOScale(Vector3.one * 4, 0.5f).SetEase(Ease.OutCubic);
        txtNameRoom.gameObject.SetActive(true);

        txtNameRoom.DOText(backroundName, 0.5f).SetEase(Ease.OutCubic).From("");

        await UniTask.Delay(3000);

        imgFadeBackground.DOFade(0f, 0.5f);
        tfmBackground.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCubic);
        txtNameRoom.gameObject.SetActive(false);
        await UniTask.Delay(500);
        imgFadeBackground.gameObject.SetActive(false);

    }

}
public enum ItemLevelProcessType
{
    Passed,
    Current,
    Next
}