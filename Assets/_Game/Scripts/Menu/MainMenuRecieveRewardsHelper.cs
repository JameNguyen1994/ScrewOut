using Coffee.UIEffects;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DailyReward;
using DG.Tweening;
using EasyButtons;
using Life;
using MainMenuBar;
using ScriptsEffect;
using Spin;
using Storage;
using Storage.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuRecieveRewardsHelper : Singleton<MainMenuRecieveRewardsHelper>
{
    [SerializeField] private PurchaseRewardIcon rewardCoin;
    [SerializeField] private List<PurchaseRewardIcon> lstRewardBooster;
    [SerializeField] private PurchaseRewardIcon prbRewardBooser;
    [SerializeField] private PurchaseRewardIcon rewardHeart;
    [SerializeField] private PurchaseRewardIcon rewardItem; //Exp
    [SerializeField] private UIFlyItemToTarget rewardItemFly; //Exp
    [SerializeField] private PurchaseRewardIcon rewardScrew; //Screw
    [SerializeField] private PurchaseRewardIcon rewardWrench; //Screw
    [SerializeField] private GameObject gobjFX;
    [SerializeField] private UIFlyItemToTarget rewardCoinFly; //Coin
    [SerializeField] private UIFlyItemToTarget rewardHeartFly; //Coin

    [Header("Target")]
    [SerializeField] private Transform targetCoin;
    [SerializeField] private Transform targetBooster;
    [SerializeField] private Transform targetHeart;
    [SerializeField] private Transform targetItem;
    [SerializeField] private Transform targetLuckySpin;
    [SerializeField] private Transform buttonLuckySpin;

    [Header("Holder Booster")]
    [SerializeField] private Transform holderBooster;
    [SerializeField] private SimpleAnimationCanvas simpleAnimationBox;
    [SerializeField] private GameObject gobjBlockRaycast;


    [SerializeField] private Animator animBanAds;
    [SerializeField] private UIEffect uIEffectDissolveAds;


    [SerializeField] private BoxLevelAnimation boxLevelAnimation;
    [SerializeField] private Transform tfmButtonPlay;

    //Lucky Spin
    [SerializeField] private UIFlyMultipleItemsToTarget luckySpinFly;

    public async UniTask Init()
    {
        MainMenuEventManager.Instance.CheckUnlockEvent();

        SetUpTargetPos();
        await OnGetReward();

        if (WrenchCollectionService.ShowReveal() && WrenchCollectionService.IsShowInMain())
        {
            WrenchCollectionService.ShowedReveal();
            WrenchCollectionData data = Db.storage.WrenchCollectionData;

            if (WrenchCollectionService.IsShowTutorial())
            {
                WrenchCollectionService.ShowedTutorial();
                await WrenchCollectionController.Instance.Tutorial.Show();
                await UniTask.WaitUntil(() => !WrenchCollectionController.Instance.Tutorial.IsShow);
            }
            else
            {
                await WrenchCollectionController.Instance.Show();
                await UniTask.WaitUntil(() => !WrenchCollectionController.Instance.IsShow);
            }
        }


    }
    private void SetUpTargetPos()
    {
        targetHeart = UITopController.Instance.GetHeartTrans();
        targetItem = UITopController.Instance.GetExpUserPos();
    }

    [Button]
    public void CheatAddReward(PurchaseRewardType purchaseRewardType, int value)
    {
        var rewardData = Db.storage.RewardData.DeepClone();
        switch (purchaseRewardType)
        {
            case PurchaseRewardType.Coin:
                rewardData.coinAmount += value;
                break;
            case PurchaseRewardType.Booster_Hammer:
                rewardData.lstBoosterValue.Add(new BoosterRewardValue(BoosterType.Hammer, value));
                break;
            case PurchaseRewardType.Booster_Drill:
                rewardData.lstBoosterValue.Add(new BoosterRewardValue(BoosterType.AddHole, value));
                break;
            case PurchaseRewardType.Booster_Bloom:
                rewardData.lstBoosterValue.Add(new BoosterRewardValue(BoosterType.Clears, value));
                break;
            case PurchaseRewardType.Booster_UnlockBox:
                rewardData.lstBoosterValue.Add(new BoosterRewardValue(BoosterType.UnlockBox, value));
                break;
            case PurchaseRewardType.Heart:
                rewardData.heartTimeAmount += value;
                break;
            case PurchaseRewardType.Item:
                rewardData.itemAmount += value;
                break;
        }

        Db.storage.RewardData = rewardData;
    }


    [Button]
    public RewardData ConvertDataFormDB()
    {
        EditorLogger.Log(">>>> ConvertDataFormDB");
        var rewardDataToPlayAnim = Db.storage.RewardData.DeepClone();

        MainMenuService.ClaimReward();

        return rewardDataToPlayAnim;
    }

    public bool IsShowReward = false;
    public bool IsWinLevelEventActive;

    [Button]
    public async UniTask OnGetReward()
    {
        IsShowReward = true;

        Debug.Log("OnGetReward 1");
        //BlockController.Instance.AddBlockLayer();
        //tfmButtonPlay.gameObject.SetActive(false);

        //await boxLevelAnimation.Init();

        EditorLogger.Log(">>>>> Move To Home Page");
        await MoveToHomePage();

        //Claim reward or add screw to luckyspin
        IsWinLevelEventActive = CoreRetentionService.IsWinLevelEventActive();

        if (!IsWinLevelEventActive)
        {
            await LoadingFade.Instance.HideLoadingFade();
        }

        bool isHaveNewScrew = SpinService.HaveNewScrew();
        int amountScrew = Db.storage.LuckySpinData.collectedInGamplayScrew;
        SpinService.AddScrewHandler();

        //Logic claim reward end game or other
        RewardData reward = ConvertDataFormDB();
        RewardData rewardWrenchCollection = null;
        WrenchCollectionData data = null;

        //Check have wrench collected
        bool isHaveWrenchNewIncome = WrenchCollectionService.HaveNewWrench() && !WrenchCollectionService.IsMaxLevel();
        List<UniTask> tasks = new List<UniTask>();

        if (isHaveWrenchNewIncome)
        {
            //Collect Wrench
            data = Db.storage.WrenchCollectionData.Clone();
            WrenchCollectionService.UpgradeLevel();
            rewardWrenchCollection = ConvertDataFormDB();
        }

        //Active event win game
        if (IsWinLevelEventActive)
        {
            //Block UI and active speed up if user tap repeatedly
            BlockController.Instance.AddBlockLayer();
            EditorLogger.Log(">>>>> Run LevelUpEvent");
            await CoreRetentionController.Instance.LevelUpEvent();
            EditorLogger.Log(">>>>> LevelUpEvent Done");
            BlockController.Instance.RemoveBlockLayer();
            SpeedGameManager.Instance.Active(false);
        }

        await CheckAndShowBanAds();
        ShopIAPController.Instance.OnClickClose();
        gobjFX.SetActive(true);

        //Show IAP
        await ShowIAPPopup();

        //Update Reward

        if (reward.coinAmount > 0)
        {
            await OnGetCoin(reward);//Coin
        }

        if (reward.itemAmount > 0)
        {
            OnGetItem(reward).Forget();//EXP
            await UniTask.Delay(2000);
        }

        if (reward.heartTimeAmount > 0)
        {
            tasks.Add(OnGetHeart(reward));//Heart
            await UniTask.Delay(2000);
        }

        if (reward.lstBoosterValue != null && reward.lstBoosterValue.Count > 0)
        {
            tasks.Add(OnGetBooster(reward));//Booster sample Hammer...
            await UniTask.Delay(500);
        }

        //Update Left Or Right Button OnMainMenu
        //Screw to lucky spin
        tasks.Add(OnGetScrewToLuckySpin(isHaveNewScrew, amountScrew));

        //Wrench
        if (isHaveWrenchNewIncome)
        {
            //Run Anim Get Wrench
            tasks.Add(OnGetWrench(isHaveWrenchNewIncome, data, rewardWrenchCollection));
        }

        int level = 0;
        float percentage = 0;

        if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
        {
            level = Db.storage.USER_INFO.level;
            percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
        }

        TrackingController.Instance.TrackingInventory(level, percentage);

        gobjFX.SetActive(false);
        // gobjBlockRaycast.gameObject.SetActive(false);
        await boxLevelAnimation.InitLevelNextLevel();

        //tfmButtonPlay.gameObject.SetActive(true);
        //tfmButtonPlay.localScale = Vector3.zero;
        // await tfmButtonPlay.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).ToUniTask();

        //BlockController.Instance.RemoveBlockLayer();

        await UniTask.WhenAll(tasks);

        SpeedGameManager.Instance.Active(false);
        BlockController.Instance.RemoveAllBlockLayer();
        IsShowReward = false;
    }

    private async UniTask MoveToHomePage()
    {
        await MainMenuController.Instance.MoveToHomePage();
        await UniTask.Delay(500);
    }
    private async UniTask OnGetCoin(RewardData reward)
    {
        int countCoin = reward.coinAmount;
        rewardCoin.SetRewardAmount(countCoin);

        if (rewardCoin.Amount > 0)
        {
            AudioController.Instance.PlaySound(SoundName.CollectExp);
            rewardCoinFly.SetIconAndValue(null, rewardCoin.GetText());
            rewardCoinFly.targetPosition = rewardCoinFly.GetComponent<RectTransform>();
            await rewardCoinFly.Play(() => { });
            await DOFlyCoinMainMenu(countCoin, rewardCoin.transform.position, UITopController.Instance.GetCoinPos());
            await UniTask.Delay(150);
        }
    }
    private async UniTask OnGetHeart(RewardData reward)
    {
        int countHeart = reward.heartTimeAmount;
        rewardHeart.SetRewardAmount(countHeart);

        if (rewardHeart.Amount > 0)
        {
            AudioController.Instance.PlaySound(SoundName.CollectHeart);
            rewardHeartFly.targetPosition = targetHeart as RectTransform;
            rewardHeartFly.SetIconAndValue(null, rewardHeart.GetText());
            await rewardHeartFly.Play(() => { });
            LifeController.Instance.UpdateUI();
            await UniTask.Delay(150);
        }
    }
    private async UniTask OnGetBooster(RewardData reward)
    {
        if (reward.lstBoosterValue == null || reward.lstBoosterValue.Count == 0)
        {
            return;
        }

        var lstBooster = reward.lstBoosterValue;
        lstRewardBooster = new List<PurchaseRewardIcon>();

        for (int i = 0; i < lstBooster.Count; i++)
        {
            lstRewardBooster.Add(Instantiate(prbRewardBooser, holderBooster));
            switch (lstBooster[i].boosterType)
            {
                case BoosterType.Hammer:
                    lstRewardBooster[i].InitRewardBooster(PurchaseRewardType.Booster_Hammer, lstBooster[i].value);
                    break;
                case BoosterType.AddHole:
                    lstRewardBooster[i].InitRewardBooster(PurchaseRewardType.Booster_Drill, lstBooster[i].value);
                    break;
                case BoosterType.Clears:
                    lstRewardBooster[i].InitRewardBooster(PurchaseRewardType.Booster_Bloom, lstBooster[i].value);
                    break;
                case BoosterType.UnlockBox:
                    lstRewardBooster[i].InitRewardBooster(PurchaseRewardType.Booster_UnlockBox, lstBooster[i].value);
                    break;
                case BoosterType.FreeRevive:
                    lstRewardBooster[i].InitRewardBooster(PurchaseRewardType.FreeRevive, lstBooster[i].value);
                    break;
            }
        }

        AudioController.Instance.PlaySound(SoundName.CollectBooster);

        var lstTaskBoostser = new List<UniTask>();

        foreach (var booster in lstRewardBooster)
        {
            if (booster.Amount <= 0)
                continue;
            booster.Show(targetBooster.position);
            await UniTask.Delay(150);
        }
        foreach (var booster in lstRewardBooster)
        {
            if (booster.Amount <= 0)
                continue;
            lstTaskBoostser.Add(booster.Move(targetBooster.position));
            // await UniTask.Delay(200);
        }
        await UniTask.WhenAll(lstTaskBoostser);
        if (lstRewardBooster.Count > 0)
            await UniTask.Delay(300);
        foreach (var booster in lstRewardBooster)
        {
            booster.Hide();
        }

    }
    private async UniTask OnGetItem(RewardData reward)
    {
        int item = reward.itemAmount;
        rewardItem.SetRewardAmount(item);

        if (rewardItem.Amount > 0)
        {
            AudioController.Instance.PlaySound(SoundName.CollectExp);

            rewardItemFly.SetIconAndValue(null, rewardItem.GetText());
            rewardItemFly.targetPosition = targetItem.GetComponent<RectTransform>();
            await rewardItemFly.Play(() => { UITopController.Instance.PlayAvatarEffect(); });
            ExpBar.Instance.AddExpUpdateUI();
            await UniTask.WaitUntil(() => ExpLevelUpPopup.Instance == null || (ExpLevelUpPopup.Instance != null && !ExpLevelUpPopup.CheckShowUI()));
        }
    }

    public void CompleteGetReward(DailyRewardItem item)
    {

    }
    public async UniTask CheckAndShowBanAds()
    {
        Debug.Log($"CheckAndShowBanAds: {IngameData.BUY_NO_ADS}");
        if (!IngameData.BUY_NO_ADS)
        {
            return;
        }
        AudioController.Instance.PlaySound(SoundName.Ban_Ads);

        IngameData.BUY_NO_ADS = false;
        /*        animBanAds.Play("Run");

                // Lấy thời gian của animation clip "Run"
                AnimatorClipInfo[] clipInfo = animBanAds.GetCurrentAnimatorClipInfo(0);
                if (clipInfo.Length > 0)
                {
                    float clipLength = clipInfo[0].clip.length;
                    await UniTask.Delay(TimeSpan.FromSeconds(clipLength));
                }

                // Hoặc nếu muốn đảm bảo chờ đúng thời điểm animation kết thúc
                await UniTask.WaitUntil(() => animBanAds.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);*/

        await DOVirtual.Float(0, 1, 2f, (value) =>
         {
             uIEffectDissolveAds.transitionRate = value;
         }).SetEase(Ease.Linear);
        await UniTask.Delay(500);
    }
    public void PlayAudioBox()
    {
    }
    public void PlayAudioScrewDown()
    {
    }

    public async void ShowGetCoinEffect(int amount)
    {
        BlockController.Instance.AddBlockLayer();
        UITopController.Instance.OnShowMainMenu();
        await UniTask.Delay(250);
        AudioController.Instance.PlaySound(SoundName.CollectExp);
        rewardCoin.SetRewardAmount(amount);
        rewardCoinFly.SetIconAndValue(null, rewardCoin.GetText());
        rewardCoinFly.targetPosition = rewardCoinFly.GetComponent<RectTransform>();
        await rewardCoinFly.Play(() => { });
        await DOFlyCoinMainMenu(amount, rewardCoin.transform.position, UITopController.Instance.GetCoinPos());
        await UniTask.Delay(250);
        UITopController.Instance.OnShowTabTask();
        BlockController.Instance.RemoveBlockLayer();
    }

    public async void ShowGetHeartEffect(int timeValue)
    {
        BlockController.Instance.AddBlockLayer();
        UITopController.Instance.OnShowMainMenu();
        await UniTask.Delay(250);
        AudioController.Instance.PlaySound(SoundName.CollectHeart);
        rewardHeart.SetRewardAmount(timeValue);
        rewardHeartFly.targetPosition = targetHeart as RectTransform;
        rewardHeartFly.SetIconAndValue(null, rewardHeart.GetText());
        await rewardHeartFly.Play(() => { });
        LifeController.Instance.UpdateUI();
        await UniTask.Delay(250);
        UITopController.Instance.OnShowTabTask();
        BlockController.Instance.RemoveBlockLayer();
    }

    private async UniTask OnGetScrewToLuckySpin(bool isHaveNewScrew, int amountScrew)
    {
        if (isHaveNewScrew)
        {
            AudioController.Instance.PlaySound(SoundName.CollectExp);
            luckySpinFly.SetIconAndValue(null, amountScrew.ToString());
            await luckySpinFly.PlayMultiple(32, () => { });
            AudioController.Instance.PlaySound(SoundName.CollectExp);
            MainMenuController.Instance.UpdateUIAddScrewToLuckySpin();
            await UniTask.Delay(250);
        }
    }

    private async UniTask OnGetWrench(bool isPlay, WrenchCollectionData data, RewardData reward)
    {
        if (isPlay)
        {
            await WrenchCollectionController.Instance.ProgressBar.UpgradeLevel(data);
            await UniTask.Delay(250);
            //Claim Reward
            CoreRetentionController.Instance.UpdateWrenchReward();
            await OnGetCoin(reward);
            await OnGetHeart(reward);
            await OnGetBooster(reward);
        }
    }

    public void UpdateUIForReward()
    {
        UITopController.Instance.UpdateCoin();
        LifeController.Instance.UpdateUI();
        ExpBar.Instance.AddExpUpdateUI();
    }

    #region Fly Coin Main Menu

    [SerializeField] private Transform parent;
    [SerializeField] private FlyBase coinFlyPrefab;

    public async UniTask DOFlyCoinMainMenu(int totalCoin, Vector3 centerPos, Vector3 targetPos)
    {
        if (totalCoin == 0)
        {
            Debug.LogWarning("<color=red>totalCoin is 0</color>");
            return;
        }

        AudioController.Instance.PlaySound(SoundName.CollectCoin);
        int numOfFlyObject = UnityEngine.Random.Range(10, 20);

        numOfFlyObject = totalCoin > 10 ? Mathf.Clamp(totalCoin, 10, 20) : totalCoin;

        int amount = totalCoin / numOfFlyObject;
        int amountLeft = totalCoin % numOfFlyObject;

        List<UniTask> tasks = new List<UniTask>();

        for (int i = 0; i < numOfFlyObject; i++)
        {
            if (i == numOfFlyObject - 1)
            {
                amount += amountLeft;
            }

            FlyBase coinObj = Instantiate(coinFlyPrefab, parent);
            Vector3 startPos = centerPos + new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), 0);
            coinObj.transform.SetWorldToLocalPosition(startPos);
            coinObj.transform.SetSiblingIndex(0);
            var task = coinObj.ExecuteLocal(startPos, targetPos, amount, 6000, OnUpdateCoinUI);
            tasks.Add(task);
            await UniTask.Delay(UnityEngine.Random.Range(25, 65));
        }

        await UniTask.WhenAll(tasks);
    }

    void OnUpdateCoinUI(int amount)
    {
        if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
        {
            EventDispatcher.Push(EventId.OnIncreaseCoinWithFx, amount);
        }
    }

    #endregion

    private bool isShowRewardIAP = false;

    public async UniTask OnGetRewardIAP()
    {
        isShowRewardIAP = true;
        BlockController.Instance.AddBlockLayer();

        EditorLogger.Log(">>>>> Move To Home Page");
        await MoveToHomePage();

        RewardData reward = Db.storage.RewardIAPData.DeepClone();
        MainMenuService.ClaimRewardIAP();

        await CheckAndShowBanAds();
        ShopIAPController.Instance.OnClickClose();
        gobjFX.SetActive(true);

        if (reward.coinAmount > 0)
        {
            await OnGetCoin(reward);//Coin
        }

        if (reward.itemAmount > 0)
        {
            await OnGetItem(reward);//EXP
        }

        if (reward.heartTimeAmount > 0)
        {
            await OnGetHeart(reward);//Heart
        }

        if (reward.lstBoosterValue != null && reward.lstBoosterValue.Count > 0)
        {
            await OnGetBooster(reward);//Booster sample Hammer...
        }

        gobjFX.SetActive(false);

        BlockController.Instance.RemoveAllBlockLayer();
        isShowRewardIAP = false;
    }

    private bool showPopupIAP = false;
    private static bool showPopupIAPOnLoad1 = false;
    private static bool showPopupIAPOnLoad2 = false;

    private async UniTask ShowIAPPopup()
    {
        if (showPopupIAP)
        {
            return;
        }

        showPopupIAP = true;

        var curLevel = Db.storage.USER_INFO.level;

        if (!showPopupIAPOnLoad1)
        {
            Debug.Log("[BeginerPackService] CheckAutoActive In ShowIAPPopup" + showPopupIAPOnLoad1);
            if (BeginerPackService.CheckAutoActive())
            {
                Debug.Log("[BeginerPackService] CheckAutoActive Success");
            }
        }

        if (BeginerPackService.CheckReveal() || BeginerPackService.IsActive())
        {
            if (BeginerPackService.IsUnlock(curLevel) || !showPopupIAPOnLoad1)
            {
                BeginerPackService.SetReveal();

                showPopupIAPOnLoad1 = true;
                ShopIAPController.Instance.ShowBeginner();
                await UniTask.WaitUntil(() => !ShopIAPController.Instance.Showing);
                await UniTask.WaitUntil(() => !isShowRewardIAP);
            }

            BeginerPackService.SetRevealLevelOnly();
        }

        if (SupperOfferService.CheckRevealSupperOffer() || SupperOfferService.IsActive())
        {
            if (SupperOfferService.IsUnlock(curLevel) || !showPopupIAPOnLoad2)
            {
                SupperOfferService.SetReveal();

                showPopupIAPOnLoad2 = true;
                await PopupSupperOffer.Instance.Show();
                await UniTask.WaitUntil(() => !PopupSupperOffer.Instance.IsShow);
                await UniTask.WaitUntil(() => !isShowRewardIAP);
            }

            SupperOfferService.SetRevealLevelOnly();
        }

        showPopupIAPOnLoad1 = true;
        showPopupIAPOnLoad2 = true;
    }
}