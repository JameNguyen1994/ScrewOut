using Cysharp.Threading.Tasks;
using MainMenuBar;
using Storage;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Booster : MonoBehaviour
{
    [SerializeField] private BoosterType boosterType;
    [SerializeField] private BoosterHandlerBase boosterHandler;
    [SerializeField] private int amount;
    [SerializeField] private BoosterData boosterData;
    [SerializeField] private BoosterUIBase boosterUI;

    UnityAction<BoosterType> actionOnComplete;

    public BoosterType BoosterType { get => boosterType; }
    public bool IsUnlockAtThisLevel => boosterData.levelUnlock == Db.storage.USER_INFO.level;

    public BoosterUIBase BoosterUI { get => boosterUI; }
    public BoosterData BoosterData { get => boosterData; }

    public void Init(int amount, UnityAction<BoosterType> actionOnComplete)
    {
        // gameObject.SetActive(false);
        boosterData = BoosterDataHelper.Instance.GetBoosterData(boosterType);
        this.amount = amount;
        boosterUI.SetUI(boosterData, amount);
        this.actionOnComplete = actionOnComplete;
    }
    public void UpdateAmount(int amount)
    {
        this.amount = amount;
        boosterUI.SetUI(boosterData, amount);
    }

    public void OnClickBooster()
    {
        Debug.Log($"Click {boosterType}");
        if (TutorialUnlockBooster.Instance.IsShowing)
        {
            Debug.Log("In tutorial");
            return;
        }

        /* TrackingController.Instance.TrackingBooster(Db.storage.USER_INFO.level, IngameData.TRACKING_UN_SCREW_COUNT * 1.0f/
             LevelController.Instance.Level.LstScrew.Count, "clicked", preBoosterType);*/

        boosterUI.SetUI(boosterData, amount);
        bool isUnlocked = Db.storage.USER_INFO.level >= boosterData.levelUnlock;
        if (!isUnlocked)
        {
            ToastMessage.Instance.ShowToast($"Unlock at level {boosterData.levelUnlock}");
            return;
        }
        if (BoosterController.Instance.CurrentAnimationBooster != BoosterType.None)
        {
            Debug.Log($"Animation {BoosterController.Instance.CurrentAnimationBooster}");
            return;
        }
        if (GameManager.Instance.GameState != GameState.Play)
            return;
        AudioController.Instance.PlaySound(SoundName.Click);
        Db.storage.BOOSTER_DATAS.GetFreeBooster(boosterType);
        var count = Db.storage.BOOSTER_DATAS.CountBooster(boosterType);
        if (count <= 0)
        {
            /* TrackingController.Instance.TrackingBooster(Db.storage.USER_INFO.level, IngameData.TRACKING_UN_SCREW_COUNT * 1.0f/
                 LevelController.Instance.Level.LstScrew.Count, "plus_popup", preBoosterType);*/
            PopupController.Instance.ShowPopupBooster(boosterType);
        }
        else
        {
            HandlerBooster();

        }
        BoosterController.Instance.DoneTutorial();
    }
    public void HandlerBooster()
    {
        if (boosterHandler != null)
            boosterHandler.ActiveBooster(OnDoneUseBooster);
    }
    public void HandlerDone()
    {
        boosterHandler.SetDoneBooster();
    }
    public void HighLight(bool isMove = true)
    {
        boosterUI.HighLightBooster(isMove);
    }
    public void HighLightIdleTime()
    {
        if(boosterData.levelUnlock > Db.storage.USER_INFO.level)
        {
            return;
        }
        boosterUI.ShowHighLightIdle();
    }
    void OnDoneUseBooster()
    {
        //Debug.Log("Complete");
        Db.storage.BOOSTER_DATAS.UseBooster(boosterType);
        actionOnComplete?.Invoke(boosterType);
        LevelDifficultyManager.Instance.SetProcessEasy();
        TrackingController.Instance.TrackingBooster(boosterType);
        TrackingController.Instance.TrackingBooster(Db.storage.USER_INFO.level, IngameData.TRACKING_UN_SCREW_COUNT * 1.0f /
            LevelController.Instance.Level.LstScrew.Count, "sink", boosterType);
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, GetBoosterTrackingName(), 1, "gameplay", "GamePlay");

    }

    string GetBoosterTrackingName()
    {
        string boosterName = "";

        switch (boosterType)
        {
            case BoosterType.AddHole:
                boosterName = "AddHole";
                break;
            case BoosterType.Hammer:
                boosterName = "Hammer";
                break;
            case BoosterType.Clears:
                boosterName = "Clear";
                break;
            case BoosterType.UnlockBox:
                boosterName = "UnlockBox";
                break;
        }

        return boosterName;
    }

    public void ToggleUI(bool active, bool resetUI)
    {
        boosterUI.ToggleUI(active, resetUI);
    }
    public async UniTask ShowAnimation()
    {
        await boosterUI.ShowAnimation();
    }
}
