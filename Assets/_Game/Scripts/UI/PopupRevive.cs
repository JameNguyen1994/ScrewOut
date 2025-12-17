using Cysharp.Threading.Tasks;
using DG.Tweening;
using Life;
using PS.Analytic;
using Storage;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeeklyQuest;
using Spin;

public class PopupRevive : PopupBase
{
    [SerializeField] private Transform tfmContent, tfmHeart, tfmHeartBreak, tfmBox, tfmSweeper, tfmExp, tfmPlus;
    [SerializeField] private GameObject objGetBox, objLoseData;
    [SerializeField] private TextMeshProUGUI txtCoin;
    [SerializeField] private Transform btnWatchAd, btnCoins, btnClose;
    [SerializeField] private ReviveType reviveType;
    [SerializeField] private Transform tfmStarterPack, tfmKeepUpOffer;
    [SerializeField] private IBillingBanner starterPackBanner, keepUpOfferBanner;
    [SerializeField] private BoosterHandlerClean boosterHandlerClean;

    [SerializeField] private TextMeshProUGUI txtCoinFree;
    [SerializeField] private GameObject btnReviveCoin, btnReviveFree;

    private int coinToRevive = 0;
    private int cancleCount = 0;
    private bool isRevive = false;

    private bool showStarterPack, showKeepUpOffer;

    private int GetCost()
    {
        var remote = GameAnalyticController.Instance.Remote();
        var costInGame = remote.CostInGame;
        return costInGame.coinRevive;
    }
    [EasyButtons.Button]
    public override void InitData(object data)
    {
        txtCoin.text = $"{GetCost()}";
        // var level = Db.storage.USER_INFO.level;
        var box = LevelController.Instance.BaseBox.GetBoxLock();
        bool hasUnlockBox = box != null;
        if (hasUnlockBox)
        {
            reviveType = ReviveType.add_box;
        }
        else
        {
            reviveType = ReviveType.clear_screw;
        }
    }

    [EasyButtons.Button]
    public override async UniTask Show()
    {
        OnShowUI();
        Setup();
        UITopController.Instance?.OnShowRevive();
        DoShow().Forget();
    }

    void Setup()
    {
        cancleCount = 0;
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);

        tfmContent.localScale = Vector3.zero;
        tfmHeart.localScale = Vector3.zero;
        tfmHeartBreak.localScale = Vector3.zero;
        tfmBox.localScale = Vector3.zero;
        tfmSweeper.localScale = Vector3.zero;
        tfmExp.localScale = Vector3.zero;
        btnWatchAd.localScale = Vector3.zero;
        btnCoins.localScale = Vector3.zero;
        btnReviveFree.transform.localScale = Vector3.zero;
        btnClose.localScale = Vector3.zero;
        tfmPlus.localScale = Vector3.zero;

        tfmStarterPack.localScale = Vector3.zero;
        tfmKeepUpOffer.localScale = Vector3.zero;

        tfmStarterPack.gameObject.SetActive(false);
        tfmKeepUpOffer.gameObject.SetActive(false);

        objGetBox.SetActive(true);
        objLoseData.SetActive(false);

        int level = Db.storage.USER_INFO.level;
        //int index = (level - GameConfig.LEVEL_START_ENABLE_STARTER_PACK) / 3;

        //showStarterPack = GameConfig.LEVEL_START_ENABLE_STARTER_PACK <= level && level % 2 == 0;
        //showKeepUpOffer = !showStarterPack && GameConfig.LEVEL_START_ENABLE_KEEP_UP_OFFER <= level;

        showStarterPack = false;
        showKeepUpOffer = true;

        if (!showKeepUpOffer && !showStarterPack)
        {
            tfmContent.localPosition = Vector3.zero;
        }
        var remote = GameAnalyticController.Instance.Remote();
        print($"data: {remote.RewardControl.rewardReviveAmountPerLevel} and {IngameData.REVIVE_REWARD_PER_LEVEL_COUNT}");

        btnWatchAd.gameObject.SetActive(IngameData.REVIVE_REWARD_PER_LEVEL_COUNT < remote.RewardControl.rewardReviveAmountPerLevel);

    }

    async UniTask DoShow()
    {
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        await UniTask.Delay(200);
        await tfmContent.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        tfmHeart.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        if (reviveType == ReviveType.add_box)
        {
            tfmBox.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            tfmSweeper.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        }

        // tfmExp.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        tfmPlus.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        await UniTask.Delay(200);
        btnCoins.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        btnReviveFree.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await btnWatchAd.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        btnClose.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        if (showStarterPack)
        {
            tfmStarterPack.gameObject.SetActive(true);
            starterPackBanner.Init();
            tfmStarterPack.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        }
        else if (showKeepUpOffer)
        {
            keepUpOfferBanner.Init();
            tfmKeepUpOffer.gameObject.SetActive(true);
            tfmKeepUpOffer.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        }
    }

    async UniTask DORevive()
    {
        LevelDifficultyManager.Instance.SetProcessEasy();

        IngameData.TRACKING_REVIVE_COUNT++;
        PopupController.Instance.HideOutOfSlot();
        var box = LevelController.Instance.BaseBox.GetBoxLock();
        bool hasUnlockBox = box != null;
        WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ReviveTimes, 1);
        UITopController.Instance.OnStartGameplay();
        boosterHandlerClean.ActiveBooster(null);
        await boosterHandlerClean.Action();
        //await LevelController.Instance.MoveScrewOnTrayToSecretBox();
        if (hasUnlockBox)
        {
            box.ChangeState(BoxState.Unlock);
            //box.InitUI(BoxState.Unlock);
            LevelController.Instance.BaseBox.CaculaterLstBoxPos();
            AudioController.Instance.PlaySound(SoundName.Effectt_Appear);
        }

        GameManager.Instance.ChangeGameState(GameState.Play);

        // var user = Db.storage.USER_INFO;
        // bool enoughCoin = user.coin >= GameConfig.COIN_REVIVE;
        // // enoughCoin = true;
        // if (enoughCoin)
        // {
        //     PopupController.Instance.reviveCount++;
        //     PopupController.Instance.HideOutOfSlot();
        //     var box = LevelController.Instance.BaseBox.GetBoxLock();
        //     bool hasUnlockBox = box != null;
        //
        //     LevelController.Instance.MoveScrewOnTrayToSecretBox();
        //     if (hasUnlockBox)
        //     {
        //         box.ChangeState(BoxState.Unlock);
        //         box.InitUI(BoxState.Unlock);
        //         AudioController.Instance.PlaySound(SoundName.Effectt_Appear);
        //     }
        //     user.coin -= GameConfig.COIN_REVIVE;
        //     Db.storage.USER_INFO = user;
        //     EventDispatcher.Push(EventId.UpdateCoinUI, -GameConfig.COIN_REVIVE);
        //     GameAnalyticController.Instance.Tracking().TrackingRevive(Db.storage.USER_INFO.level);
        //     GameManager.Instance.ChangeGameState(GameState.Play);
        //     UITopController.Instance.OnStartGameplay();
        // }
        // else
        // {
        //     IngameData.SHOP_PLACEMENT = shop_placement.revive;
        //
        //     UITopController.Instance.OnClickCoin();
        // }
    }

    public void OnReviveByCoin()
    {
        if (GetAmountFreeRevive() > 0)
        {
            if (Db.storage.ReviveData.count < GetMaxReviveData())
            {
                var reviveData = Db.storage.ReviveData.DeepClone();
                reviveData.count++;
                Db.storage.ReviveData = reviveData;
            }
            else
            {
                var count = Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.FreeRevive);

                if (count > 0)
                {
                    Db.storage.BOOSTER_DATAS.UseBooster(BoosterType.FreeRevive);
                }
            }

            isRevive = true;
            PopupController.Instance.reviveCount++;
            DOHide().Forget();

            return;
        }

        var user = Db.storage.USER_INFO;
        var coin = GetCost();
        if (user.coin >= coin)
        {
            coinToRevive = coin;
            isRevive = true;
            user.coin -= coin;
            SingularSDK.Event("COIN_SPEND");

            WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.UseCoins, coin);

            Db.storage.USER_INFO = user;
            EventDispatcher.Push(EventId.UpdateCoinUI, -coin);
            PopupController.Instance.reviveCount++;
            DOHide().Forget();

            int level = 0;
            float percentage = 0;

            if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
            {
                level = Db.storage.USER_INFO.level;
                percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
            }
            TrackingController.Instance.TrackingRevive(ReviveType_Tracking.coin);

            TrackingController.Instance.TrackingInventory(level, percentage);
        }
        else
        {
            IngameData.SHOP_PLACEMENT = shop_placement.Revive;
            UITopController.Instance.ShowShop();
        }
    }

    public void OnReviveByAd()
    {
        AdsController.Instance.ShowRewardAds(RewardAdsPos.revive, () =>
        {
            coinToRevive = 0;
            IngameData.REVIVE_REWARD_PER_LEVEL_COUNT++;
            TrackingController.Instance.TrackingRevive(ReviveType_Tracking.ads);

            isRevive = true;
            DOHide().Forget();
        }, null, null, "revive");
    }

    async UniTask ChangeToStage2()
    {
        btnClose.DOScale(0, 0.2f).SetEase(Ease.InBack);
        tfmBox.DOScale(0, 0.2f).SetEase(Ease.InBack);
        tfmSweeper.DOScale(0, 0.2f).SetEase(Ease.InBack);
        tfmHeart.DOScale(0, 0.2f).SetEase(Ease.InBack);
        tfmPlus.DOScale(0, 0.2f).SetEase(Ease.InBack);
        btnWatchAd.DOScale(0, 0.2f).SetEase(Ease.InBack);
        btnCoins.DOScale(0, 0.2f).SetEase(Ease.InBack);
        btnReviveFree.transform.DOScale(0, 0.2f).SetEase(Ease.InBack);
        await UniTask.Delay(100);
        objGetBox.SetActive(false);
        objLoseData.SetActive(true);
        tfmHeartBreak.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        tfmPlus.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        tfmExp.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(200);
        btnCoins.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        btnReviveFree.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await btnWatchAd.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        btnClose.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    async UniTask DOHide()
    {
        tfmStarterPack.DOScale(0, 0.2f).SetEase(Ease.InBack);
        tfmKeepUpOffer.DOScale(0, 0.2f).SetEase(Ease.InBack);
        btnClose.DOScale(0, 0.2f).SetEase(Ease.InBack);
        btnWatchAd.DOScale(0, 0.2f).SetEase(Ease.InBack);
        btnCoins.DOScale(0, 0.2f).SetEase(Ease.InBack);
        btnReviveFree.transform.DOScale(0, 0.2f).SetEase(Ease.InBack);
        await UniTask.Delay(150);
        tfmPlus.DOScale(0, 0.2f).SetEase(Ease.InBack);
        tfmExp.DOScale(0, 0.2f).SetEase(Ease.InBack);
        tfmHeartBreak.DOScale(0, 0.2f).SetEase(Ease.InBack);
        tfmHeart.DOScale(0, 0.2f).SetEase(Ease.InBack);
        tfmBox.DOScale(0, 0.2f).SetEase(Ease.InBack);
        tfmSweeper.DOScale(0, 0.2f).SetEase(Ease.InBack);
        await UniTask.Delay(200);
        tfmContent.DOScale(0, 0.2f).SetEase(Ease.InBack);
        await imgFade.DOFade(0f, 0.3f);
        imgFade.gameObject.SetActive(false);
        tfmStarterPack.gameObject.SetActive(false);
        tfmKeepUpOffer.gameObject.SetActive(false);

        if (!isRevive)
        {
            PopupController.Instance.HideOutOfSlot();
            LifeController.Instance.UseLife();
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Life", 1, "life", "Revive");
            PopupController.Instance.ShowPopupLose();
            SerializationManager.ClearAllDataInGamePlay();
            //Lose do not add screw to luckyspin
            //SpinService.CollectScrew(LevelController.Instance.Level.GetRemovedScrewAmount());
        }
        else
        {
            DORevive().Forget();
        }
    }

    public void OnClickGiveUp()
    {
        cancleCount++;

        if (cancleCount <= 1)
        {
            ChangeToStage2().Forget();
        }
        else
        {
            isRevive = false;
            // AudioController.Instance.PlaySound(SoundName.Click);
            DOHide().Forget();
        }
    }

    private int GetMaxReviveData()
    {
        var remote = GameAnalyticController.Instance.Remote();
        return remote != null ? remote.Revive_Free : 5;
    }

    private int GetAmountFreeRevive()
    {
        int result = 0;

        if (Db.storage.ReviveData.count < GetMaxReviveData())
        {
            result += GetMaxReviveData() - Db.storage.ReviveData.count;
        }

        var count = Db.storage.BOOSTER_DATAS.CountBooster(BoosterType.FreeRevive);

        if (count > 0)
        {
            result += count;
        }

        return result;
    }

    private void OnShowUI()
    {
        if (GetAmountFreeRevive() > 0)
        {
            txtCoinFree.text = "<size=50>X</size>" + GetAmountFreeRevive();
            btnReviveCoin.SetActive(false);
            btnReviveFree.SetActive(true);
        }
        else
        {
            txtCoin.text = $"{GetCost()}";
            btnReviveCoin.SetActive(true);
            btnReviveFree.SetActive(false);
        }

        var box = LevelController.Instance.BaseBox.GetBoxLock();
        bool hasUnlockBox = box != null;
        bool hasColor = !ScrewBlockedRealTimeController.Instance.IsFullAll();

        if (hasUnlockBox && hasColor)
        {
            reviveType = ReviveType.add_box;
        }
        else
        {
            reviveType = ReviveType.clear_screw;
        }
    }
}

public enum ReviveType
{
    add_box = 0,
    clear_screw = 1
}