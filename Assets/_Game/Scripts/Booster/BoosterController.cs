using Cysharp.Threading.Tasks;
using PS.Utils;
using Storage;
using System.Collections;
using System.Collections.Generic;
using PS.Analytic;
using UnityEngine;
using UnityEngine.UI;
using EasyButtons;

public class BoosterController : Singleton<BoosterController>
{
    [SerializeField] private List<Booster> lstBoosterBottom;
    [SerializeField] private List<Booster> lstBoosterUnlockBox;
    [SerializeField] private Shape shapeHammer;
    [SerializeField] private bool usingHammer;
    [SerializeField] private bool isShowTutorial;
    [SerializeField] private GameObject gobjShowHammer;
    [SerializeField] private RectTransform rtfmBooster;
    [SerializeField] private HorizontalLayoutGroup horiGroup;
    [SerializeField] private RectTransform rectBooster;

    public Shape ShapeHammer { get => shapeHammer; }
    public bool UsingHammer { get => usingHammer; }
    public BoosterType CurrentAnimationBooster { get => currentAnimationBooster; }
    public bool IsShowTutorial { get => isShowTutorial; }

    [SerializeField] private Image imgHolderBoosterHammer;
    [SerializeField] private BoosterType currentAnimationBooster;

    protected override void CustomAwake()
    {
        base.CustomAwake();
        /*        horiGroup.enabled = true;
                LayoutRebuilder.ForceRebuildLayoutImmediate(rtfmBooster);
                horiGroup.enabled = false;*/
        HideAllBeforeShow();
    }
    private void OnEnable()
    {
        EventDispatcher.Register(EventId.OnToggleBanner, CheckPosBooster);
    }
    private void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnToggleBanner, CheckPosBooster);
    }
    public void HideAllBeforeShow()
    {
        for (int i = 0; i < lstBoosterBottom.Count; i++)
        {
            lstBoosterBottom[i].ToggleUI(false, true);
        }
    }
    public async UniTask StartInitBoosters()
    {
        usingHammer = false;
        currentAnimationBooster = BoosterType.None;
        bool isShowBooster = GameAnalyticController.Instance.Remote().SegmentFlow.boosterEnable;

        if (!isShowBooster || GameConfig.OLD_VERSION)
        {
            ToggleAllBooster(false);
            return;
        }
        ToggleAllBooster(Db.storage.USER_INFO.level.GetDecrypted() >= 2);
        CheckPosBooster();
        await Init();
        StartCountTimeToShowHighLight();
    }
    public void CheckPosBooster(object data = null)
    {
        int isTurnOffBanner = GameAnalyticController.Instance.Remote().SegmentFlow.bannerLvl;
        var posY = DeviceDetection.Instance.DeviceType == DeviceType.Phone ? 200 : 200;
        if (!AdsController.Instance.IsShowBanner)
        {
            rtfmBooster.anchoredPosition = new Vector2(rtfmBooster.anchoredPosition.x, 30); // Set bottom position to 0
        }
        else
        {
            rtfmBooster.anchoredPosition = new Vector2(rtfmBooster.anchoredPosition.x, posY); // Set bottom position to 0

        }
    }
    private async UniTask ToggleAllBooster(bool active)
    {
        IngameData.IS_SHOW_BOOSTER = active;
        if (Db.storage.USER_INFO.level >= 2)
        {
            SliderZoom.Instance.OnChangePos(false);

        }

        for (int i = 0; i < lstBoosterBottom.Count; i++)
        {
            lstBoosterBottom[i].ToggleUI(active, true);
            if (active)
            {
                lstBoosterBottom[i].ShowAnimation();
                await UniTask.Delay(100);
            }
        }
        /*        for (int i = 0; i < lstBoosterUnlockBox.Count; i++)
                {
                    lstBoosterUnlockBox[i].gameObject.SetActive(active);
                }*/
    }
    public void StartAnimation(BoosterType boosterType)
    {
        currentAnimationBooster = boosterType;
    }
    public void EndAnimation()
    {
        currentAnimationBooster = BoosterType.None;
    }
    public async UniTask Init()
    {
        //  await UniTask.Delay(100);
        isShowTutorial = false;
        lstBoosterBottom.Find(x => x.BoosterType == BoosterType.AddHole).Init(Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.AddHole), ActionResetUI);
        lstBoosterBottom.Find(x => x.BoosterType == BoosterType.Hammer).Init(Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.Hammer), ActionResetUI);
        lstBoosterBottom.Find(x => x.BoosterType == BoosterType.Clears).Init(Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.Clears), ActionResetUI);
        lstBoosterBottom.Find(x => x.BoosterType == BoosterType.UnlockBox).Init(Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.UnlockBox), ActionResetUI);

        for (int i = 0; i < lstBoosterUnlockBox.Count; i++)
        {
            lstBoosterUnlockBox[i].Init(Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.UnlockBox), ActionResetUI);
        }
        for (int i = 0; i < lstBoosterBottom.Count; i++)
        {
            lstBoosterBottom[i].ToggleUI(true, false);
        }


        await UniTask.Delay(100);

        horiGroup.enabled = false;
        var boosterTutorial = GetBoosterTutorial();
        if (boosterTutorial != null)
        {
            isShowTutorial = true;
            await TutorialUnlockBooster.Instance.StartTutorial(boosterTutorial);
        }
    }
    public void DoneTutorial()
    {
        if (!isShowTutorial)
            return;
        TrackingController.Instance.TrackingTutorial(TUTORIAL_TYPE.Booster);

        isShowTutorial = false;
    }
    public void HighLightBooster(BoosterType boosterType)
    {
        var lstBooster = lstBoosterBottom.FindAll(x => x.BoosterType == boosterType);
        lstBooster.AddRange(lstBoosterUnlockBox.FindAll(x => x.BoosterType == boosterType));
        foreach (var booster in lstBooster)
        {
            booster.HighLight();
        }
    }
    public void ActionResetUI(BoosterType boosterType)
    {
        lstBoosterBottom.Find(x => x.BoosterType == BoosterType.AddHole).UpdateAmount(Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.AddHole));
        lstBoosterBottom.Find(x => x.BoosterType == BoosterType.Hammer).UpdateAmount(Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.Hammer));
        lstBoosterBottom.Find(x => x.BoosterType == BoosterType.Clears).UpdateAmount(Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.Clears));
        lstBoosterBottom.Find(x => x.BoosterType == BoosterType.UnlockBox).UpdateAmount(Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.UnlockBox));

        for (int i = 0; i < lstBoosterUnlockBox.Count; i++)
        {
            lstBoosterUnlockBox[i].UpdateAmount(Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.UnlockBox));
        }
    }
    public void OnUseShape(Shape shapeHammer)
    {
        usingHammer = false;
        this.shapeHammer = shapeHammer;
        lstBoosterBottom.Find(x => x.BoosterType == BoosterType.Hammer).HandlerDone();

        for (int i = 0; i < lstBoosterBottom.Count; i++)
        {
            if (lstBoosterBottom[i].BoosterType != BoosterType.Hammer)
                lstBoosterBottom[i].ToggleUI(true, true);
        }
        gobjShowHammer.gameObject.SetActive(false);
        imgHolderBoosterHammer.rectTransform.anchoredPosition = new Vector2(0, imgHolderBoosterHammer.rectTransform.anchoredPosition.y);

    }
    public void ChangeToHammerScrewState()
    {
        usingHammer = true;
        for (int i = 0; i < lstBoosterBottom.Count; i++)
        {
            if (lstBoosterBottom[i].BoosterType != BoosterType.Hammer)
                lstBoosterBottom[i].ToggleUI(false, true);
        }
        gobjShowHammer.gameObject.SetActive(true);
        imgHolderBoosterHammer.rectTransform.anchoredPosition = new Vector2(-200, imgHolderBoosterHammer.rectTransform.anchoredPosition.y);

    }
    public void OnClickCancelScrewState()
    {
        usingHammer = false;
        for (int i = 0; i < lstBoosterBottom.Count; i++)
        {
            if (lstBoosterBottom[i].BoosterType != BoosterType.Hammer)
                lstBoosterBottom[i].ToggleUI(true, true);
        }
        gobjShowHammer.gameObject.SetActive(false);
        imgHolderBoosterHammer.rectTransform.anchoredPosition = new Vector2(0, imgHolderBoosterHammer.rectTransform.anchoredPosition.y);

    }

    public Booster GetBoosterTutorial()
    {
        var booster = lstBoosterBottom.Find(x => x.IsUnlockAtThisLevel && !Db.storage.BOOSTER_DATAS.IsReceived(x.BoosterType));
        Debug.Log($"Null booster {booster == null} at level {Db.storage.USER_INFO.level}");
        return booster;
    }

    public Transform GetBoosterKeyUnlock()
    {
        var booster = lstBoosterBottom.Find(x => x.BoosterType == BoosterType.UnlockBox);
        if (booster != null)
            return booster.transform;
        return null;
    }
    [Button]
    public void HighLightAfterIdleTime(BoosterType boosterType)
    {
        for (int i = 0; i < lstBoosterBottom.Count; i++)
        {
            if (lstBoosterBottom[i].BoosterType == boosterType)
            {
                lstBoosterBottom[i].HighLightIdleTime();
                break;
            }
        }
    }
    [Button]
    public void HighLightAfterIdleTimeAll()
    {
        for (int i = 0; i < lstBoosterBottom.Count; i++)
        {
                lstBoosterBottom[i].HighLightIdleTime();
        }
    }
    Coroutine coroutineHighLight;
    public void StartCountTimeToShowHighLight()
    {
        if (coroutineHighLight != null)
        {
            return;
        }
        coroutineHighLight = StartCoroutine(CountTimeToShowHighLight());
    }
    IEnumerator CountTimeToShowHighLight()
    {
        yield return new WaitForSeconds(10);
        var boosterType = BoosterType.None;
        boosterType = lstBoosterBottom[Random.Range(0, lstBoosterBottom.Count)].BoosterType;
       // HighLightAfterIdleTime(boosterType);
        HighLightAfterIdleTimeAll();
        StopCountTimeToShowHighLight();
    }
    public void StopCountTimeToShowHighLight()
    {
        StopCoroutine(coroutineHighLight);
        coroutineHighLight = null;
        StartCountTimeToShowHighLight();
    }
    public void OnWinGame()
    {
        OnClickCancelScrewState();
        HideAllBeforeShow();

    }
}