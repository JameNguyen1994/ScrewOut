using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using GameAnalyticsSDK;
using MainMenuBar;
using ps.modules.journey;
using ps.modules.leaderboard;
using PS.Analytic;
using Storage;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainWinPopup : PopupBase
{
    [SerializeField] private Image imgCover;
    [SerializeField] private Transform winPopupContent, miniGameContent, tfmButtonContinue, tfmButtonReward, tfmWellDone, tfmButtons;
    [SerializeField] private Transform tfmCoin, tfmScrew;
    [SerializeField] private TextMeshProUGUI txtCoinMiniGame;
    [SerializeField] private TextMeshProUGUI txtCoinBonusLevel, txtScrewBonus, txtLevel;
    [SerializeField] private MinigameBar miniGame;
    [SerializeField] private PopupLeaderBoardEndGame popupLeaderBoardEnd;
    [SerializeField] private RectTransform rtfmRewards;
    [SerializeField] private RectTransform rtfmContinueHolder;
    [SerializeField] private Transform tfmItemExp;

    [SerializeField] private UIFlyItemToTarget rewardItemFly; //Exp
    [SerializeField] private PurchaseRewardIcon rewardItem; //Exp

    [SerializeField] private UIFlyItemToTarget rewardCoinFly; //Coin
    [SerializeField] private PurchaseRewardIcon rewardCoin; //Coin
    [SerializeField] private LevelProcessBar levelProcessBar; //Coin
    [SerializeField] private MainMenuBarDataSO contentDataUnlock; //Coin


    private int coinsBonusLevel = 0;
    private int screwsBonus = 0;
    private int level = 0;

    private bool isShowing = false;
    public bool IsShowing => isShowing;

    public override async UniTask Show()
    {
        Setup();
        DoShow().Forget();
    }

    [EasyButtons.Button]
    public void Test()
    {
        InitData(new WinData()
        {
            level = 12,
            coins = 40,
            screws = 20
        });

        Show();
    }

    public override void InitData(object data)
    {
        var winData = (WinData)data;
        coinsBonusLevel = winData.coins;
        screwsBonus = winData.screws;
        level = winData.level;
        var point = LeaderBoardService.GetCup(LevelController.Instance.Level);
        popupLeaderBoardEnd.SetStarToAdd(point);
        txtCoinMiniGame.text = $"{coinsBonusLevel * 3}";
    }

    void Setup()
    {
        JourneyController.Instance.OnChangeLevel();

        var imgCoverColor = imgCover.color;
        imgCoverColor.a = 0;
        imgCover.color = imgCoverColor;
        imgCover.gameObject.SetActive(false);
        winPopupContent.localScale = Vector3.zero;
        miniGameContent.localScale = Vector3.zero;
        tfmButtonContinue.localScale = Vector3.zero;
        tfmButtonReward.localScale = Vector3.zero;
        txtCoinBonusLevel.text = "0";
        txtScrewBonus.text = "0";
        txtLevel.text = $"LEVEL {level}";
        miniGame.OnValueChanged = OnMiniGameValueChanged;
        miniGame.Setup();
        levelProcessBar.gameObject.SetActive(true);
        levelProcessBar.SetUp(Db.storage.USER_INFO.level - 1);

    }

    private void OnMiniGameValueChanged(float value)
    {
        int coins = (int)(value * coinsBonusLevel);
        // Debug.Log($"OnMiniGameValueChanged: {value} - coins: {coins}");
        txtCoinMiniGame.text = $"{coins}";
    }

    async UniTask DoShow()
    {
        isHide = false;

        int levelUnlockJourney = DBMainMenuBarController.Instance.DB_MAIN_MENU_ITEMS.lstDBBarItem[1].levelUnlock;
        int levelUnlockLeaderBoard = DBMainMenuBarController.Instance.DB_MAIN_MENU_ITEMS.lstDBBarItem[3].levelUnlock;
        // int levelUnlockWeeklyChallenge = DBMainMenuBarController.Instance.DB_MAIN_MENU_ITEMS.lstDBBarItem[4].levelUnlock;
        isShowing = true;
        imgCover.gameObject.SetActive(true);
        imgCover.DOFade(0.98f, 0.5f);
        await UniTask.Delay(200);


        await winPopupContent.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        var coinLevelBonus = 0;
        if (CoinCollector.Instance != null)
        {
            coinLevelBonus = CoinCollector.Instance.Coin;
            var reward = Db.storage.RewardData.DeepClone();
            reward.coinAmount += coinLevelBonus;
            Db.storage.RewardData = reward;
            WeeklyQuest.WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(WeeklyQuest.QuestType.CollectCoins, coinLevelBonus);
            //coinLevelBonus = 10;
        }
        else
        {
            coinLevelBonus = 0;
        }
        tfmCoin.DOScale(1.3f, 0.3f).SetEase(Ease.OutBack);
        tfmScrew.DOScale(1.3f, 0.3f).SetEase(Ease.OutBack);
        if (level + 1 < levelUnlockLeaderBoard)
        {
            tfmWellDone.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        }
        //DOVirtual.Int(0, coinsBonusLevel, 0.3f, value => { txtCoinBonusLevel.text = $"{value}"; });
        txtCoinBonusLevel.text = $"{coinsBonusLevel}";
        rewardItem.SetRewardAmount(screwsBonus);
        await DOVirtual.Int(0, screwsBonus, 0.3f, value => { txtScrewBonus.text = $"{value}"; });
        if (coinLevelBonus > 0)
        {
            rewardCoin.SetRewardAmount(coinLevelBonus);
            rewardCoinFly.SetIconAndValue(null, rewardCoin.GetText());

            AudioController.Instance.PlaySound(SoundName.CollectExp);
            rewardCoinFly.targetPosition = txtCoinBonusLevel.GetComponent<RectTransform>();
            await rewardCoinFly.Play(() =>
            {
                DOVirtual.Int(0, coinsBonusLevel + coinLevelBonus, 0.3f, value => { txtCoinBonusLevel.text = $"{value}"; });

            });
            AudioController.Instance.PlaySound(SoundName.CollectExp);

        }

        var rewardTwice = Db.storage.RewardData.DeepClone();



        /// Show Exp if level <=2
        if (Db.storage.USER_INFO.level <= 3)
        {
            Debug.Log("Show Exp Level Gameplay Only");
            UITopController.Instance.ShowExpLevelGameplayOnly();
            await UniTask.Delay(200);

            if (rewardTwice.itemAmount > 0)
            {
                Debug.Log($"Exp Received: {rewardTwice.itemAmount}");


                AudioController.Instance.PlaySound(SoundName.CollectExp);
                //rewardItem.Show(txtScrewBonus.transform.position);
                rewardItemFly.SetIconAndValue(null, rewardTwice.itemAmount.ToString());
                //
                //rewardItem.Hide();

                var targetItem = UITopController.Instance.GetExpUserPos();
                rewardItemFly.targetPosition = targetItem.GetComponent<RectTransform>();
                await rewardItemFly.Play(() => { UITopController.Instance.PlayAvatarEffect(); });
                ExpBar.Instance.AddExpUpdateUI();


                ExpBar.Instance.AddExpToUser(rewardTwice.itemAmount);
                rewardTwice.itemAmount = 0;
                Db.storage.RewardData = rewardTwice;
                await UniTask.Delay(1000);

                UITopController.Instance.HideExpLevelGameplayOnly();

            }

        }

        await UniTask.Delay(100);



        if (level + 1 >= levelUnlockLeaderBoard)
        {
            tfmCoin.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            tfmScrew.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            rtfmRewards.DOAnchorPosY(0, 0.5f).SetEase(Ease.Linear);

            popupLeaderBoardEnd.gameObject.SetActive(true);

            await popupLeaderBoardEnd.CheckResetTime();

            await popupLeaderBoardEnd.SetupData(0.3f);
        }

        Debug.Log($"levelUnlockJourney: {levelUnlockJourney} - level: {level + 1}");
        if (level + 1 >= levelUnlockJourney)
        {

            await levelProcessBar.DOShow();
        }
        else
        {
            var targetPos = new Vector3(0, levelProcessBar.transform.position.y, levelProcessBar.transform.position.z);
            tfmButtons.position = targetPos;
            tfmButtons.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, tfmButtons.GetComponent<RectTransform>().anchoredPosition.y);
        }
        tfmButtonContinue.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        var remote = GameAnalyticController.Instance.Remote();
        if (remote != null)
        {

            var isShow = remote.RewardControl.isEnableRewardButtonXCoinWinPopup;
            if (isShow)
            {
                /*miniGameContent.DOScale(1, 0.3f).SetEase(Ease.OutBack);
                await UniTask.Delay(100);*/

                tfmButtonReward.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            }
            else
            {
                rtfmContinueHolder.DOAnchorPosY(350, 0.5f);
                tfmButtonReward.gameObject.SetActive(false);
            }
        }

    }

    private bool isHide = false;

    public void OnContinueBtnClick()
    {
        if (isHide)
        {
            return;
        }

        isHide = true;

        AudioController.Instance.PlaySound(SoundName.Click);
        Hide();
    }

    public void OnRewardBtnClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        AdsController.Instance.ShowRewardAds(RewardAdsPos.win, () =>
        {
            miniGame.OnStopEvent(out float value);
            var reward = Db.storage.RewardData.DeepClone();
            int coinAmount = (int)(coinsBonusLevel * 3);
            reward.coinAmount += coinAmount;
            Db.storage.RewardData = reward;
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coin", coinAmount, "reward_ad", "WinPopup");
            Hide();
        }, null, null, "win");
    }

    [EasyButtons.Button]
    public override void Hide()
    {
        DoHide().Forget();
    }

    async UniTask DoHide()
    {
        tfmButtonContinue.DOScale(0, 0.3f).SetEase(Ease.InBack);
        tfmButtonReward.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await UniTask.Delay(100);
        miniGameContent.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await UniTask.Delay(200);
        winPopupContent.DOScale(0, 0.3f).SetEase(Ease.InBack);
        imgCover.DOFade(1f, 0.5f);

        if (Db.storage.USER_INFO.level <= 3)
        {
            SceneController.Instance.ChangeScene(SceneType.GamePlayNewControl);
        }
        else
        {
            SceneController.Instance.ChangeScene(SceneType.MainMenu);
        }
    }
#if UNITY_EDITOR

    [Button]
    public void ForceShow()
    {
        tfmButtonContinue.localScale = Vector3.one;
        tfmButtonReward.localScale = Vector3.one;
        miniGameContent.localScale = Vector3.one;
        winPopupContent.localScale = Vector3.one;
        imgCover.gameObject.SetActive(true);
        var imgCoverColor = imgCover.color;
        imgCoverColor.a = 0.98f;
        imgCover.color = imgCoverColor;
        isShowing = true;
        EditorUtility.SetDirty(this);
    }

    [Button]
    public void ForceHide()
    {
        tfmButtonContinue.localScale = Vector3.zero;
        tfmButtonReward.localScale = Vector3.zero;
        miniGameContent.localScale = Vector3.zero;
        winPopupContent.localScale = Vector3.zero;
        imgCover.gameObject.SetActive(false);
        isShowing = false;

        EditorUtility.SetDirty(this);
    }
#endif
}

public struct WinData
{
    public int coins;
    public int screws;
    public int level;
}