using Cysharp.Threading.Tasks;
using DG.Tweening;
using ps.modules.journey;
using TMPro;
using UnityEngine;
using static UniWebViewLogger;

public class ItemLevelProcess : MonoBehaviour
{
    [SerializeField] private Transform tfmEffectBack;
    [SerializeField] private Transform tfmPassed;
    [SerializeField] private Transform tfmNext;
    [SerializeField] private Transform tfmTick;
    [SerializeField] private Transform tfmReward;
    [SerializeField] private RectTransform rtfmItem;
    [SerializeField] private TMP_Text txtLevel;

    [SerializeField] private ItemGiftJourneyPopupWin itemGiftJourneyPopupWin;
    [SerializeField] private bool hasReward;

    public RectTransform RtfmItem { get => rtfmItem; }

    public void Reset()
    {

        tfmEffectBack.gameObject.SetActive(false);
        tfmPassed.gameObject.SetActive(false);
        tfmNext.gameObject.SetActive(true);
        tfmTick.gameObject.SetActive(false);
        tfmReward.gameObject.SetActive(false);
        itemGiftJourneyPopupWin.gameObject.SetActive(false);

    }
    public async UniTask SetUp(ItemLevelProcessType itemLevelProcessType, int level)
    {
        txtLevel.text = $"Lv.{level}";
        tfmReward.gameObject.SetActive(false);
        itemGiftJourneyPopupWin.Reset();
        switch (itemLevelProcessType)
        {
            case ItemLevelProcessType.Passed:
                tfmPassed.gameObject.SetActive(true);
                tfmNext.gameObject.SetActive(false);
                tfmTick.gameObject.SetActive(true);
                break;
            case ItemLevelProcessType.Current:
                tfmPassed.gameObject.SetActive(true);
                tfmNext.gameObject.SetActive(false);
                tfmTick.gameObject.SetActive(false);

                break;
            case ItemLevelProcessType.Next:
                tfmPassed.gameObject.SetActive(false);
                tfmNext.gameObject.SetActive(true);
                tfmTick.gameObject.SetActive(false);

                break;
        }


        if (itemLevelProcessType == ItemLevelProcessType.Current || itemLevelProcessType == ItemLevelProcessType.Next)
        {
            var lstResource = JourneyController.Instance.GetRewardByLevel(level);
            var journeyData = JourneyController.Instance.GetBackgroundByLevel(level);
            if (journeyData != null)
            {
                hasReward = true;
                itemGiftJourneyPopupWin.SetJourneyData(journeyData);
            }
            else
            if (lstResource != null && lstResource.Count > 0)
            {
                hasReward = true;
                itemGiftJourneyPopupWin.SetData(lstResource);
                itemGiftJourneyPopupWin.transform.localScale = Vector3.one * 0.7f;
            }
            else
                hasReward = false;
        }
        if (hasReward)
        {
            tfmNext.gameObject.SetActive(false);
            tfmTick.gameObject.SetActive(false);
            tfmPassed.gameObject.SetActive(false);
            tfmReward.gameObject.SetActive(true);
            itemGiftJourneyPopupWin.gameObject.SetActive(true);
        }

    }

    public void HideBeforeShow()
    {
        transform.localScale = Vector3.zero;
    }
    public async UniTask DOShow()
    {
        await transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    public async UniTask DOAnimationCurrent()
    {
        if (hasReward)
        {
            tfmReward.DOScale(Vector3.zero, 0.3f).SetEase(Ease.Linear);
            itemGiftJourneyPopupWin.GetGift();
        }


        tfmEffectBack.gameObject.SetActive(true);
        tfmNext.gameObject.SetActive(false);
        await tfmEffectBack.DOScale(Vector3.one * 1.3f, 0.3f).SetEase(Ease.OutBack).From(Vector3.one);
        await tfmEffectBack.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear);
        //await UniTask.Delay(500);


        await UniTask.WaitUntil(() => itemGiftJourneyPopupWin.IsShowingAnimation == false);
        tfmTick.gameObject.SetActive(true);
        await tfmTick.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).From(Vector3.zero);
        tfmPassed.gameObject.SetActive(true);
        //await UniTask.Delay(500);
        //tfmEffectBack.gameObject.SetActive(false);
    }
    public async UniTask DOPassAnimation()
    {
        tfmTick.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
    }
    private async UniTask GetRewarad()
    {
        /*
                if (hasReward && level == maxLevel)
                {
                    itemGiftJourneyPopupWin.GetGift();
                }*/
    }

}
