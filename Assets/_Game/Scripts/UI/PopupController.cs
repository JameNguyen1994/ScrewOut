using System;
using Cysharp.Threading.Tasks;
using PS.Utils;
using Storage;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using PS.Analytic;
using UnityEngine;
using UnityEngine.UI;
using WeeklyQuest;
using System.Threading.Tasks;

public class PopupController : Singleton<PopupController>
{
    [SerializeField] private PopupBase popupMissionStartGame;

    [SerializeField] private PopupWin popupWin;
    [SerializeField] private PopupPrevWin popupPrevWin;

    [SerializeField] private MainWinPopup popupWinNew;
    [SerializeField] private PopupPrevLose popupPrevLose;

    [SerializeField] private PopupReviveOld popupReviveOld;
    [SerializeField] private PopupRevive popupOutOfSlot;
    [SerializeField] private PopupRating popupRating;
    [SerializeField] private PopupBooster popupBooster;
    [SerializeField] private PopupLose popupLose;
    [SerializeField] private PopupPause popupPause;
    [SerializeField] private RevivePanel revivePanel;
    [SerializeField] private PopupHardLevel popupHardLevel;
    [SerializeField] private PopupExpLevelUp popupExpLevelUp;
    [SerializeField] private SwitchButton buttonSettings;
    [SerializeField] private GameObject gobjFade;
    [SerializeField] private bool isPushEvent = false;



    [HideInInspector] public int reviveCount = 0;
    [SerializeField] private int popupCount = 0;

    public int PopupCount { get => popupCount; set => popupCount = value; }
    public PopupHardLevel PopupHardLevel { get => popupHardLevel; }
    public PopupExpLevelUp PopupExpLevelUp { get => popupExpLevelUp; }
    public MainWinPopup PopupWinNew { get => popupWinNew; }

    private void Start()
    {
        isPushEvent = false;
        //buttonSettings.SetUIButton(!GameConfig.OLD_VERSION);
    }

    public async UniTask ShowMissionStartGame()
    {
        await popupMissionStartGame.Show();
    }

    [EasyButtons.Button]
    public async Task ShowPrevWin()
    {
        GameManager.Instance.ChangeGameState(GameState.Stop);
        await popupPrevWin.Show();
    }

    public async UniTask ShowWin()
    {
        GameManager.Instance.ChangeGameState(GameState.Stop);
        PopupCount++;

        /*        if (GamePlayCheat.Instance.IsEnabledCheat)
                {
                    GamePlayCheat.Instance.IsEnabledCheat = false;
                    OnShowWin().Forget();
                    return;
                }
        */

        AdsController.Instance.ShowInterEnd(async (result) =>
        {
            OnShowWin();
        });
    }
    public void GetDataWin()
    {
        int coin = GameConfig.COIN_WIN;

        popupWinNew.InitData(new WinData()
        {
            level = Db.storage.USER_INFO.level,
            coins = coin,
            screws = LevelController.Instance.Level.TotalScrew
        });

        var reward = Db.storage.RewardData.DeepClone();
        int expReceived = (LevelController.Instance.Level.TotalScrew / 3) * GameAnalyticController.Instance.Remote().ExpCompleteBox;
        Debug.Log($"Exp Received: {expReceived}");
        reward.itemAmount += expReceived;
        reward.coinAmount += coin;
        IngameData.IS_WIN_LEVEL = true;
        WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.CollectCoins, coin);

        Db.storage.RewardData = reward;

        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coin", coin, "gameplay", "Win");

        var userInfo = Db.storage.USER_INFO;
        Spin.SpinService.CollectScrew(LevelController.Instance.Level.TotalScrew);

        //Win Game
        AssetBundleService.OnWinLevel(userInfo.level);

        userInfo.level++;

        SerializationManager.ClearAllDataInGamePlay();
        LevelMapService.UserLevelMapUp(userInfo.level);
        WrenchCollectionGamePlayController.Instance.OnWinGame();
        CoreRetentionService.ActiveWinLevelEvent();

        Db.storage.USER_INFO = userInfo;
    }
    private async UniTask OnShowWin()
    {
        await ShowBeginSalePack();
        //  AudioController.Instance.PlaySound(SoundName.Win_Popup);
        VibrationController.Instance.DoubleVibrate(VibrationType.Medium).Forget();

        //if (!isPushEvent)
        {
            // TrackingController.Instance.TrackingGamePlay(LEVEL_STATUS.WIN);
            //  isPushEvent = true;
            ShowRating();
        }

        await UniTask.WaitUntil(() => !popupRating.IsShowing);


        popupWinNew.Show();
    }
    private async UniTask ShowBeginSalePack()
    {
        return;
        if (Db.storage.USER_INFO.level < 5) return;

        var userInfo = Db.storage.USER_INFO;

        if (DateTime.UtcNow < Db.storage.USER_INFO.beginSaleAdsValidUntil) return;

        if (Db.storage.USER_INFO.level == 5)
        {
            ShopIAPController.Instance.ShowBeginner();
        }
        else
        {
            if (IsHave2BoosterUnderAmount())
            {
                if (DateTime.UtcNow >= Db.storage.USER_INFO.beginSaleAdsNextAvailable)
                {
                    var validUntil = DateTime.UtcNow.AddDays(2);
                    Db.storage.USER_INFO.beginSaleAdsValidUntil = validUntil;
                    Db.storage.USER_INFO.beginSaleAdsNextAvailable = validUntil.AddDays(5);
                    ShopIAPController.Instance.ShowBeginner();
                }
            }
        }
        Db.storage.USER_INFO = userInfo;
        await UniTask.WaitWhile(() => ShopIAPController.Instance.Showing);
    }

    private bool IsHave2BoosterUnderAmount()
    {
        int count = 0;
        var boosterData = Db.storage.BOOSTER_DATAS;
        foreach (var data in boosterData.lstBoosterData)
        {
            if (data.value <= 1) count++;
        }
        return count >= 2;
    }
    public async UniTask ShowOutOfSlot()
    {
        await ShowBeginSalePack();
        GameManager.Instance.ChangeGameState(GameState.Stop);

        // if (reviveCount > 0)
        // {
        //     btnRevive.interactable = false;
        // }
        var user = Db.storage.USER_INFO;
        await ScreenGamePlayUI.Instance.ShowWarningLose(1f);
        await ShowPrevLose();
        popupCount++;

    }

    public async UniTask ShowPrevLose()
    {
        await popupPrevLose.Show();
    }

    public async UniTask ShowRevive()
    {
        if (Life.DBLifeController.Instance != null)
            Life.DBLifeController.Instance.LIFE_INFO.SetQuitGameWhenLose(true);

        SerializationManager.ClearAllDataInGamePlay();

        AudioController.Instance.PlaySound(SoundName.Lose);

        popupOutOfSlot.InitData(null);
        // await revivePanel.ShowLose();
        popupOutOfSlot.Show();
        if (!isPushEvent && IngameData.MODE_CONTROL == ModeControl.ControlV1)
        {
            Debug.Log("ShowOutOfSlot");
           // TrackingController.Instance.TrackingGamePlay(LEVEL_STATUS.out_of_slot);
            isPushEvent = true;
        }
    }
    public void ShowReviveOld()
    {
        gobjFade.SetActive(true);
        popupReviveOld.Init();
        popupReviveOld.gameObject.SetActive(true);

    }

    public void HideOutOfSlot()
    {
        popupCount--;
        // gobjFade.SetActive(false);
        // popupOutOfSlot.Hide();
    }

    [ContextMenu("ShowRating")]
    public void ShowRating()
    {
        Debug.Log(Db.storage.IS_SHOW_RATING);

        if (!Db.storage.IS_SHOW_RATING) return;
        popupCount++;
        var remote = GameAnalyticController.Instance.Remote();
        var level = Db.storage.USER_INFO.level;
        Debug.Log($"ShowRating Level: {level} {remote.NumToShowReview} {remote.NumToLoopShowReview}");
        if ((level != 0 && level == remote.NumToShowReview)
            ||( (level > remote.NumToShowReview)
            && (level - remote.NumToShowReview) % remote.NumToLoopShowReview == 0))
        {
            PopupCount++;
            popupRating.gameObject.SetActive(true);
            popupRating.ShowPopup();
            Debug.Log(Db.storage.IS_SHOW_RATING);

        }
    }
    public void HideRating()
    {
        popupCount--;
        AudioController.Instance.PlaySound(SoundName.Click);
        popupRating.HidePopup();
        popupRating.gameObject.SetActive(false);
    }

    public void OnSubmitRatingClick()
    {
        popupRating.OnSubmit();
        HideRating();
    }
    public void ShowPopupBooster(BoosterType boosterType)
    {
        popupCount++;
        popupBooster.InitData(boosterType);
        popupBooster.Show();
    }
    public void ShowPopupLose()
    {
        PopupCount++;
        popupLose.Show();
        //TrackingController.Instance.TrackingGamePlay(LEVEL_STATUS.out_of_slot);

    }
    public void ShowPopupPause()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        PopupCount++;
        popupPause.Show();
    }
    public void HidePopupPause()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        PopupCount--;
        popupPause.OnContinueBtnClick();
    }

    public void OnDoneBuyBooster()
    {
        popupBooster.OnDoneBuyBooster();
    }
}
