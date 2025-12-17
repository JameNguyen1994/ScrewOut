using CodeStage.AntiCheat.Utils;
using Cysharp.Threading.Tasks;
using Life;
using PS.Utils;
using Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PS.Analytic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using WeeklyQuest;
using EasyButtons;
using DG.Tweening;

public class LevelController : Singleton<LevelController>
{
    //[SerializeField] private List<LevelMap> lstLevelMap;
    [SerializeField] private LevelMap levelMap;

    [SerializeField] private List<Tray> lstTray = new List<Tray>();
    [SerializeField] private BaseBox baseBox;
    [SerializeField] private List<Screw> lstScrewOnTray = new List<Screw>();
    [SerializeField] private Transform tfmSecretBox;
    [SerializeField] private ToogleColorMode toogleColorMode;
    [SerializeField] private CheckNoAds checkNoAds;
    [SerializeField] private LevelScrewHelper levelScrewHelper;
    [SerializeField] private SecretBox secretBox;
    [SerializeField] private LevelScaleHelper levelScaleHelper;
    [SerializeField] private LevelMapScrewBlockedHelper levelMapScrewBlockedHelper;
    [SerializeField] private bool isCheckingSecretBox;
    [SerializeField] private bool isCheckingListTray;
    [SerializeField] private bool canUseTransparentMode = false;
    [SerializeField] private bool completeAnimationScale = false;
    [SerializeField] private PreBoosterRocket preBoosterRocket;
    [SerializeField] private PreBoosterGlass preBoosterGlass;


    [Header("Setup Highlight Screw")]
    [SerializeField] private Material matHighlight;
    [SerializeField] private LayerMask layerScrewHighlight;
    [SerializeField] private LayerMask layerSCrewNor;
    [Header("Setup Highlight Shape")]
    [SerializeField] private LayerMask layerShapeHighlight;
    [SerializeField] private LayerMask layerShapeNor;

    [SerializeField] private bool isBonusLevel = false;
    [SerializeField] private bool reloadedLevel = true;

    public LevelMap Level
    {
        get => levelMap;
    }
    public List<Tray> LstTray { get => lstTray; set => lstTray = value; }
    public BaseBox BaseBox { get => baseBox; }

    public string UniqueId => Define.LEVEL_CONTROLLER_ID;
    public SecretBox SecretBox => secretBox;
    public bool CanUseTransparentMode { get => canUseTransparentMode; }
    public PreBoosterGlass PreBoosterGlass { get => preBoosterGlass; }
    public bool CompleteAnimationScale { get => completeAnimationScale; }
    public bool IsBonusLevel { get => isBonusLevel; set => isBonusLevel = value; }
    public bool ReloadedLevel { get => reloadedLevel; set => reloadedLevel = value; }

    public bool IsReload = false;

    public static UnityAction<int, int> OnGameWinExpAddEvent; //param1: exp, param2: level

    private async UniTask CheckLevelBonus()
    {

        bool isHardLevel = levelMap.LevelDifficulty == LevelDifficulty.Hard;

        if (isHardLevel)
        {
            isBonusLevel = true;
            await LevelBonusController.Instance.InitLevelCache();
            reloadedLevel = true;
        }

    }

    private async UniTask Start()
    {
        GameManager.Instance.ChangeGameState(GameState.Stop);
        HideAllTray();

        SerializationManager.IsQuitGame = false;
        reloadedLevel = true;
        IngameData.GameMode = GameMode.Normal;
        UITopController.Instance.OnStartGameplay();
        IsReload = SerializationManager.HaveDataToReloadGamePlay();
        // await LoadingFade.Instance.HideLoadingFade();
        ResetTrackingData();
        await InitListLevelMap();
        WrenchCollectionGamePlayController.Instance.Init();
        //ShowLevelDifficulty();
        if (!IsReload)
        {
            await LoadingFade.Instance.HideLoadingFade();
        }
        CheckLevelBonus();


        await ShowMissionStartGame(!IsReload);


        //// Check Booster

        bool isShowTutorialGlass = false;
        if (!IsReload)
        {
            if (PreBoosterController.Instance.PreBoosterRocket.Select)
            {

                InputHandler.Instance.IsLockInput = true;
                await UIPreBoosterGameplay.Instance.ShowRocket();
                await UniTask.Delay(500);
                UIPreBoosterGameplay.Instance.HideRocket();
                await preBoosterRocket.StartAction();
                InputHandler.Instance.IsLockInput = false;
            }
            isShowTutorialGlass = Db.storage.PreBoosterData.IsFree(PreBoosterType.Glass);

            PreBoosterController.Instance.OnStartGame();

            if (PreBoosterController.Instance.PreBoosterGlass.Select)
            {
                canUseTransparentMode = true;
                InputHandler.Instance.IsLockInput = true;
                if (PreBoosterController.Instance.PreBoosterRocket.Select)
                {
                    await UniTask.Delay(300);
                }
                await UIPreBoosterGameplay.Instance.ShowGlass();
                await UniTask.Delay(500);
                UIPreBoosterGameplay.Instance.HideGlass();
                await levelMap.AllShapeTransparent();
                await UniTask.Delay(2000);
                await levelMap.AllShapeNormal();


                InputHandler.Instance.IsLockInput = false;
                //   await TutorialPreBoosterGlass.Instance.ShowTutorial();


                if (isShowTutorialGlass)
                {
                    SwipeRotation360Degrees.Instance.IsLockRotation = true;
                    SwipeRotation360Degrees.Instance.IsLockAutoRotation = true;
                    GameManager.Instance.ChangeGameState(GameState.Play);

                    await TutorialPreBoosterGlass.Instance.ShowTutorial();
                    SwipeRotation360Degrees.Instance.IsLockRotation = false;
                    SwipeRotation360Degrees.Instance.IsLockAutoRotation = false;
                    GameManager.Instance.ChangeGameState(GameState.Stop);

                }

            }
        }

        await baseBox.InitBoxs(IsReload);

        if (!IsReload)
        {
            DoTutorial();
        }

        if (Db.storage.USER_INFO.level > 3)
            await SliderZoom.Instance.ShowBoxColor();
        await InitToogle(IsReload);
        await SliderZoom.Instance.ShowSlideZoom();
        await ShowAllTrayAsync();


        await BoosterController.Instance.StartInitBoosters();
        if (IsReload)
        {
            await SerializationManager.InitializeFromSaveAll();

            if (Db.storage.PreBoosterData.IsSaveUsing(PreBoosterType.Glass))
            {
                canUseTransparentMode = true;
            }


            SerializationManager.ClearAllDataInGamePlay();
            await LoadingFade.Instance.HideLoadingFade();
            IsReload = false;
        }
        else
        {


        }

        SetUpHighlightScrewsAndShape();
        ScreenGamePlayUI.Instance.OnLoadGameCompleted();
        GameManager.Instance.ChangeGameState(GameState.Play);

    }

    [EasyButtons.Button]
    public async UniTask ChangeCenterPos()
    {
        // Block
        InputHandler.Instance.IsLockInput = true;
        await levelScaleHelper.ChangCenterPos(levelMap.LstShape, levelMap.LstLinkObstacle);
        InputHandler.Instance.IsLockInput = false;

        // Release
    }
    async UniTask ShowMissionStartGame(bool showBanner = true)
    {
        SetLevelMapFocus();
        ChangeCenterPos();
        await UniTask.Delay(200);

        if (showBanner)
            await PopupController.Instance.ShowMissionStartGame();
        levelScaleHelper.SetLevelTransform(levelMap.transform);
        SetLevelMapFocus();
        var taskStartAnimation = StartAnimationLevelMap();
        await UniTask.Delay(100);

        //if (levelMap.StartPostion == Vector3.zero && levelMap.StartRotation == Vector3.zero)
        if (true)
        {
            EditorLogger.Log(">>>> Auto Default Posstion Level Map");
            await levelScaleHelper.CastListScrewTranform(levelMap.LstScrew, levelMap.LevelMapSize);
            completeAnimationScale = true;
            var screwTask = StartAnimationLevelMapScrew();
            var focusTas = SetLevelMapFocusAsync();
            await UniTask.WhenAll(screwTask, focusTas, taskStartAnimation);
            await UniTask.Delay(100);

            // await levelScaleHelper.ChangCenterPos(levelMap.LstScrew);


        }
        else
        {
            EditorLogger.Log(">>>> Load Default Position Level Map");

            await levelMap.transform.DOScale(1.6f * levelMap.transform.localScale, 0.5f).SetEase(Ease.InOutSine);
            await levelMap.transform.DOMove(levelMap.StartPostion, 0.5f).SetEase(Ease.InOutSine);
            await levelMap.transform.DORotate(levelMap.StartRotation, 0.5f).SetEase(Ease.InOutSine);

            var screwTask = StartAnimationLevelMapScrew();
            await UniTask.WhenAll(screwTask, taskStartAnimation);
            await UniTask.Delay(100);
        }

        levelMapScrewBlockedHelper.InitLevel(levelMap.LevelId, levelMap);
        levelMap.SetDataColor();

    }

    void ResetTrackingData()
    {
        IngameData.TRACKING_UN_SCREW_COUNT = 0;
        IngameData.TRACKING_UNLOCK_BOX_COUNT = 0;
        IngameData.TRACKING_HAMMER_COUNT = 0;
        IngameData.TRACKING_ADD_HOLE_COUNT = 0;
        IngameData.TRACKING_CLEAR_COUNT = 0;
        IngameData.TRACKING_REVIVE_COUNT = 0;
    }

    async Task InitListLevelMap()
    {
        //lstLevelMap = AssetReferenceController.Instance.GetLevelMap(Db.storage.USER_INFO.level);
        await Init();

        GetMoreLevel();
        //GetObstacleLevelMap3();

        ScrewCounter.Instance.Init(levelMap.LstScrew.Count);
    }
    public async void GetMoreLevel()
    {
        //int levelToLoad = Db.storage.USER_INFO.level;

        //if (levelToLoad % 5 != 0)
        //{
        //    return;
        //}

        //var lstLevelMore = await AssetReferenceController.Instance.LoadAssetByLevel<LevelMap>(++levelToLoad);

        //AssetReferenceController.Instance.AddLevelMap(levelToLoad, lstLevelMore);

        var curLevel = Db.storage.USER_INFO.level;
        AssetBundleService.DownloadMap(curLevel + 1).Forget();
        AssetBundleService.DownloadMap(curLevel + 2).Forget();
        AssetBundleService.DownloadMap(curLevel + 3).Forget();
    }

    public async void GetMoreLevelForUnitTest()
    {
        //int levelToLoad = Db.storage.USER_INFO.level;
        //var listLevel = AssetReferenceController.Instance.GetLevelMap(levelToLoad);

        //if (listLevel == null)
        //{
        //    var lstLevelMore = await AssetReferenceController.Instance.LoadAssetByLevel<LevelMap>(levelToLoad);
        //    AssetReferenceController.Instance.AddLevelMap(levelToLoad, lstLevelMore);
        //}
    }

    public void ShowLevelDifficulty()
    {
        if (levelMap.LevelDifficulty != LevelDifficulty.Hard) return;
        PopupController.Instance.PopupHardLevel.Show();
    }
    public void AddNewUnlockTray(Tray tray)
    {
        lstTray.Add(tray);
    }
    async UniTask DoTutorial()
    {
        if (!Db.storage.IS_TUTORIAL || Db.storage.USER_INFO.level != 1)
        {
            return;
        }

        await WaitToDoTutorial();
    }
    async UniTask InitToogle(bool isReload)
    {
        if (Db.storage.USER_INFO.level <= 3)
        {
            toogleColorMode.gameObject.SetActive(false);
        }

        toogleColorMode.Init(this);

        if (!isReload && Db.storage.USER_INFO.level == 4)
        {
            await TutorialController.Instance.DoTutorialSwitchColor();
        }
    }
    async UniTask WaitToDoTutorial()
    {
        await UniTask.Delay(10);
        await TutorialController.Instance.DoTutorial();
    }
#if UNITY_EDITOR
    public async UniTask ForceLoadLevel(int index)
    {
        if (levelMap != null)
        {
            Destroy(levelMap.gameObject);
        }

        LevelMap _levelMap = await AssetReferenceController.Instance.LoadLevelPrefabAsync<LevelMap>(index);


        levelMap = Instantiate(_levelMap);

        levelMapScrewBlockedHelper.InitLevel(_levelMap.LevelId, levelMap);
        //levelMap = Instantiate(lstLevelMap[curLevel - 1]);
        levelScrewHelper.SetLevelData(levelMap);


        levelMap.Init();
        levelMap.HideBeforeShow();
        IngameData.SET_COMPLETED = 0;
        IngameData.IS_WIN = false;
        // TrackingController.Instance. StartSessionGamePlay();
        baseBox.Init();

        //levelMap.gameObject.transform.position += new Vector3(0, 1, 0);
        GameManager.Instance.ChangeGameState(GameState.Play);
        CheckNoAds.Instance.CheckIsNoAds();
        // StartCoroutine(AdsController.Instance.StartAdByLevel(index));
        for (int i = 0; i < lstTray.Count; i++)
        {
            lstTray[i].InitNewTray();
        }
        await ShowMissionStartGame(false);
        baseBox.InitBoxs();

    }
#endif

    public async UniTask Init()
    {
        var curLevel = Db.storage.USER_INFO.level;

        //if (DBLifeController.Instance != null)
        //    DBLifeController.Instance.LIFE_INFO.SetQuitGameWhenLose(true);

        int realLevelMap = LevelMapService.GetLevelMap(curLevel);
        EditorLogger.Log(">>>>Real Level Map: " + realLevelMap);

        if (AssetBundleService.HasAssetBundle(realLevelMap))
        {
            GameObject prefabLevel = await AssetBundleService.LoadPrefabAsync(realLevelMap);

            if (prefabLevel != null)
            {
                GameObject level = Instantiate(prefabLevel);
                levelMap = level.GetComponent<LevelMap>();
            }

            if (levelMap == null)
            {
                //Process Error Level
                Debug.Log(">>>>Load Level Default");
                LevelMap _levelMap = await AssetReferenceController.Instance.LoadLevelPrefabAsync<LevelMap>(LevelMapService.GetDefaultLevel());
                levelMap = Instantiate(_levelMap);
            }
        }
        else
        {
            LevelMap _levelMap = await AssetReferenceController.Instance.LoadLevelPrefabAsync<LevelMap>(realLevelMap);
            levelMap = Instantiate(_levelMap);
        }

        if (!IsReload)
        {
            levelMap.SpawnWrench();
            TrackingController.Instance.StartSessionGamePlay();
        }

        levelMap.Init();
        levelMap.HideBeforeShow();

        IngameData.SET_COMPLETED = 0;
        IngameData.IS_WIN = false;
        baseBox.Init();
        //levelMap.gameObject.transform.position += new Vector3(0, 1, 0);
        CheckNoAds.Instance.CheckIsNoAds();
        StartCoroutine(AdsController.Instance.StartAdByLevel(curLevel));
        for (int i = 0; i < lstTray.Count; i++)
        {
            lstTray[i].InitNewTray();
        }


    }
    public async UniTask StartAnimationLevelMap()
    {
        await levelMap.ShowLevelMap();
    }
    public async UniTask StartAnimationLevelMapScrew()
    {
        await levelMap.ShowLevelMapScrew();
    }

    private bool IsGameProgressForCheckingScrewOrBox()
    {
        return baseBox.HaveBoxFull() || baseBox.HaveBoxWaitingChangeColor() || newBoxCheckCounter > 0;
    }

    public async UniTask CheckLose()
    {
        await UniTask.WaitUntil(() => !IsGameProgressForCheckingScrewOrBox());

        for (int i = 0; i < lstScrewOnTray.Count; i++)
        {
            if (baseBox.IsBoxWithColorUnlocked(lstScrewOnTray[i].ScrewColor))
            {
                AssignScrewsToBoxes();
                return;
            }
        }

        for (int i = 0; i < lstTray.Count; i++)
        {
            if (!lstTray[i].IsFill)
            {
                return;
            }
        }

        if (PopupController.Instance.PopupCount <= 0
         && GameManager.Instance.GameState != GameState.Stop)
        {
            OnTrayLose();
            await PopupController.Instance.ShowOutOfSlot();
        }
    }

    public void AssignScrewsToBoxesWhenBoxOut()
    {
        for (int i = 0; i < lstScrewOnTray.Count; i++)
        {
            if (lstScrewOnTray[i] != null)
            {
                Box box = baseBox.GetBoxUnlockByColor(lstScrewOnTray[i].ScrewColor);

                if (box != null)
                {
                    lstScrewOnTray[i].MoveToBox(box);
                }
            }
        }
    }

    private void AssignScrewsToBoxes()
    {
        for (int i = 0; i < lstScrewOnTray.Count; i++)
        {
            Box box = baseBox.GetBoxUnlockByColor(lstScrewOnTray[i].ScrewColor);

            if (box != null)
            {
                lstScrewOnTray[i].MoveToBox(box);
            }
        }
    }

    public async UniTask CheckWin()
    {
        baseBox.CaculaterLstBoxPos(true);
        if (IngameData.IS_WIN)
            return;
        Debug.Log($"CheckWin {baseBox.GetBoxUnlock()}");


        if (baseBox.GetBoxUnlock() == 0)
        {
            BoosterController.Instance.OnWinGame();
            IngameData.IS_WIN = true;
            TrackingController.Instance.TrackingGamePlay(LEVEL_STATUS.WIN);
            //isHardLevel = true;
            PopupController.Instance.GetDataWin();
            int itemAmount = Db.storage.RewardData.itemAmount;
            OnGameWinExpAddEvent?.Invoke(itemAmount, Db.storage.USER_INFO.level - 1);

            if (IsBonusLevel)
            {
                await UniTask.Delay(500);
                await LevelBonusController.Instance.ShowBonusLevel();
            }



            CancelInvoke(nameof(ShowWin));
            Invoke(nameof(ShowWin), 1);
        }
    }

    public async UniTask ForceWin()
    {
        baseBox.ForceDataWin();
        CheckWin();
    }
    async UniTask ShowWin()
    {
        //TrackingController.Instance.TrackingGamePlay(LEVEL_STATUS.WIN);
        if (levelMap.EOL != null)
        {
            AudioController.Instance.PlaySound(SoundName.Win);
            DoEOLAfterWin().Forget();
        }
        else
        {
            AudioController.Instance.PlaySound(SoundName.Win);

            await PopupController.Instance.ShowPrevWin();
        }
    }

    private async UniTaskVoid DoEOLAfterWin()
    {
        await levelMap.EOL.Execute();
        PopupController.Instance.ShowPrevWin();
    }
    public async UniTask CheckAllBox()
    {
        //bool isWait = true;
        var lstBoxUnlock = baseBox.GetAllUnlock();
        var lstTask = new List<UniTask>();
        for (int i = 0; i < lstBoxUnlock.Count; i++)
        {
            var box = lstBoxUnlock[i];
            if (!box.IsBoxFill())
            {
                CheckNewBoxOnTray(box);
                CheckNewBoxOnSecret(box);
            }
        }
        await UniTask.WhenAll(lstTask);
    }

    private int newBoxCheckCounter = 0;

    public async UniTask CheckNewBox(Box box)
    {
        if (box == null)
            return;
        newBoxCheckCounter++;
        await CheckNewBoxOnTray(box);
        await CheckNewBoxOnSecret(box);
        newBoxCheckCounter--;
    }

    private async UniTask CheckNewBoxOnTray(Box box)
    {
        await UniTask.WaitUntil(() => !isCheckingListTray);
        isCheckingListTray = true;

        List<Screw> lstScrewToCheck = lstScrewOnTray;

        var lstTask = new List<UniTask>();

        for (int i = 0; i < box.LstTray.Count; i++)
        {
            var screw = lstScrewToCheck.Find(x => x.ScrewColor == box.Color);
            if (screw == null)
                continue;

            if (box.LstTray[i].TryFill(screw))
            {
                HoleWarning.Instance.HideWarning();
                screw.Tray.Fill(null);
                screw.ChangeState(ScrewState.OnBox);
                lstScrewToCheck.Remove(screw);
                lstTask.Add(screw.MoveToTray(box.LstTray[i], true, () =>
                {
                    box.CheckFull();
                }, 0));
            }
        }

        await UniTask.WhenAll(lstTask);
        isCheckingListTray = false;
    }
    private async UniTask CheckNewBoxOnSecret(Box box)
    {
        await UniTask.WaitUntil(() => !isCheckingSecretBox);
        isCheckingSecretBox = true;
        List<Screw> lstScrewToCheck = secretBox.LstScrew;
        var lstTask = new List<UniTask>();

        for (int i = 0; i < box.LstTray.Count; i++)
        {
            var screw = lstScrewToCheck.Find(x => x.ScrewColor == box.Color);
            if (screw == null)
                continue;

            if (box.LstTray[i].TryFill(screw))
            {
                lstScrewToCheck.Remove(screw);
                await secretBox.RemoveScrew(screw);
                screw.ChangeState(ScrewState.OnBox);

                lstTask.Add(screw.MoveToTray(box.LstTray[i], false, () =>
                {
                    secretBox.OnDoneAnimationMove();
                    box.CheckFull();

                }, 0));
            }
        }
        await UniTask.WhenAll(lstTask);
        isCheckingSecretBox = false;

    }

    public void OnCollectScrew(ScrewColor screwColor)
    {
        Debug.Log($"OnCollectScrew: {screwColor}");
        /*    WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_Sky, 1);
            return;*/
        switch (screwColor)
        {
            case ScrewColor.Blue:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_Blue, 1);
                break;
            case ScrewColor.Orange:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_Orange, 1);
                break;
            case ScrewColor.Brown:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_GreenBlack, 1);
                break;
            case ScrewColor.Purple:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_Purple, 1);
                break;
            case ScrewColor.Be:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_Gray, 1);
                break;
            case ScrewColor.Sky:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_Sky, 1);
                break;
            case ScrewColor.Pink:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_Pink, 1);
                break;
            case ScrewColor.Red:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_Red, 1);
                break;
            case ScrewColor.Green:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_Green, 1);
                break;
            case ScrewColor.Yellow:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews_Color_Yellow, 1);
                break;
            default:
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.ClaimScrews, 1);
                break;
        }
    }
    public void MoveScrewToTray(Screw screw, UnityAction onConplete)
    {
        var moveUpDistance = 15f;
        Box box = null;
        if (IngameData.GameMode == GameMode.Normal)
            box = baseBox.GetBoxUnlockByColor(screw.ScrewColor);
        else
            box = LevelBonusController.Instance.BoxLevelBonusController.GetBoxUnlockByColor(screw.ScrewColor);
        if (box != null) // move to box
        {
            var trayOnBox = box.GetTrayOnBoxNonFill();
            if (trayOnBox != null)
            {
                if (screw.State == ScrewState.OnTray)
                {
                    screw.Tray.Fill(null);
                }
                // screw.transform.SetParent(trayOnBox.transform);
                screw.ChangeState(ScrewState.OnBox);
                if (lstScrewOnTray.Contains(screw))
                {
                    lstScrewOnTray.Remove(screw);
                }
                waitCount++;

                var scale = 2.6f;
                screw.MoveToTray(trayOnBox, true, () =>
                {
                    OnCollectScrew(screw.ScrewColor);
                    waitCount--;
                    box.AddScrew(screw);
                    box.CheckFull();
                    var lstTrayNonFill = lstTray.FindAll(x => !x.IsFill);
                    if (lstTrayNonFill.Count != 1 && lstScrewOnTray.Count != 0)
                    {
                        HoleWarning.Instance.HideWarning();

                    }
                    onConplete.Invoke();

                }, moveUpDistance);

            }
            else
            {
                Debug.LogError("BUGGGGG1");

            }
        }
        else // move to tray
        {

            var trayNonFill = GetTrayNonFill();
            if (trayNonFill != null)
            {
                screw.ChangeState(ScrewState.OnTray);
                lstScrewOnTray.Add(screw);
                var scale = IngameData.MODE_CONTROL == ModeControl.ControlV2
                    ? new Vector3(7, 7, 7)
                    : new Vector3(5, 5, 5);
                var rotateX = -90f;
                screw.MoveToTray(trayNonFill, true, () =>
                {

                    CheckLose();
                    var lstTrayNonFill = lstTray.FindAll(x => !x.IsFill);
                    Debug.Log($"lstTrayNonFill: {lstTrayNonFill.Count}");
                    if (lstTrayNonFill.Count == 1 || lstTrayNonFill.Count == 0)
                    {
                        HoleWarning.Instance.ShowWarning();
                        //  GameAnalyticController.Instance.Tracking().TrackingLastHold();
                    }
                    else
                    {
                        HoleWarning.Instance.HideWarning();

                    }
                    onConplete.Invoke();
                }, moveUpDistance);
            }
            else
            {
                //Debug.LogError("BUGGGGG4");
            }
        }

    }
    /* public void MoveScrewToTargetBox(Screw screw, UnityAction onConplete, Box box)
     {
         if (box != null && screw != null) // move to box
         {

             var trayOnBox = box.GetTrayOnBoxNonFill();
             if (trayOnBox != null)
             {
                 if (screw.State == ScrewState.OnTray)
                 {
                     screw.Tray.Fill(false);
                 }

                 trayOnBox.Fill(true);
                 screw.transform.SetParent(trayOnBox.transform);
                 screw.ChangeState(ScrewState.OnBox);
                 if (lstScrewOnTray.Contains(screw))
                 {
                     lstScrewOnTray.Remove(screw);
                 }

                 var scale = 2.6f;
                 screw.MoveToTray(trayOnBox.transform.position, Vector3.zero, Vector3.one * scale, () =>
                 {
                     OnCollectScrew(screw.ScrewColor);

                     box.AddScrewToSecretBox(screw);
                     box.CheckFull();

                     onConplete.Invoke();
                 });
             }

         }
     }
 */
    public async UniTask MoveScrewOnTrayToSecretBox()
    {
        InputHandler.Instance.IsLockInput = true;
        // await UniTask.Delay(500);
        lstScrewOnTray.Sort((a, b) => b.transform.position.x.CompareTo(a.transform.position.x));
        //print($"lst screw count: {lstScrewOnTray.Count}");
        string positionLog = "";
        for (int i = 0; i < lstScrewOnTray.Count; i++)
        {
            positionLog += lstScrewOnTray[i].transform.position + "\n";
        }

        await LevelController.Instance.SecretBox.CheckToShowSecretBox();
        Debug.Log($"lstScrewOnTray position {lstScrewOnTray.Count}: {positionLog}");
        for (int i = 0; i < lstScrewOnTray.Count; i++)
        {

            //print($"screw id {i}: {lstScrewOnTray.Count}");
            var screw = lstScrewOnTray[i];
            screw.SetState(ScrewState.OnReviveBox);
            //screw.transform.SetParent(tfmSecretBox.transform);
            LevelController.Instance.AddScrewToSecretBox(screw);
            await UniTask.Delay(50);
        }

        lstScrewOnTray.Clear();

        await UniTask.Delay(1500);
        InputHandler.Instance.IsLockInput = false;
        GameManager.Instance.ChangeGameState(GameState.Play);
    }
    public void SetListScrewToSecretBox(List<Screw> lstScrew)
    {
        for (int i = 0; i < lstScrew.Count; i++)
        {
            var screw = lstScrew[i];
            if (screw == null)
                continue;

            if (IngameData.GameMode == GameMode.Normal)
                Instance.Level.FilledScrew(screw.UniqueId);
            else
                LevelBonusController.Instance.Level.FilledScrew(screw.UniqueId);

            bool gotBox = false;
            var box = baseBox.GetBoxUnlockByColor(screw.ScrewColor);
            if (box != null) // move to box
            {
                var trayOnBox = box.GetTrayOnBoxNonFill();
                if (trayOnBox != null)
                {
                    if (screw.State == ScrewState.OnTray)
                    {
                        screw.Tray.Fill(null);
                    }
                    gotBox = true;
                    screw.transform.SetParent(trayOnBox.transform);
                    screw.ChangeState(ScrewState.OnBox);
                    if (lstScrewOnTray.Contains(screw))
                    {
                        lstScrewOnTray.Remove(screw);
                    }
                    screw.MoveToTray(trayOnBox, false, () =>
                    {
                        box.AddScrew(screw);
                        box.CheckFull();
                    }, 0);
                }
                else
                {
                    gotBox = false;
                    //Debug.Log("Fail");
                }
            }
            if (!gotBox)
            {
                //print($"screw id {i}: {lstScrewOnTray.Count}");
                if (screw == null)
                    continue;
                screw.SetState(ScrewState.OnReviveBox);
                screw.transform.SetParent(tfmSecretBox.transform);
                //var scale = IngameData.MODE_CONTROL == ModeControl.ControlV2 ? new Vector3(8, 7, 7) : new Vector3(5, 5, 5);
                var rotateX = 0;
                var scale = 2.6f;
                // scale = 0f;

                LevelController.Instance.AddScrewToSecretBox(screw);
            }
        }

    }

    public float GetHoleFill()
    {
        return CountTrayIsFill() * 1.0f / lstTray.Count;
    }

    int CountTrayIsFill()
    {
        return lstTray.Count(x => x.IsFill);
    }

    public Tray GetTrayNonFill()
    {
        for (int i = 0; i < lstTray.Count; i++)
        {
            if (!lstTray[i].IsFill)
            {
                return lstTray[i];
            }
        }

        return null;
    }
    public Tray GetTrayFill()
    {
        for (int i = 0; i < lstTray.Count; i++)
        {
            if (lstTray[i].IsFill)
            {
                return lstTray[i];
            }
        }

        return null;
    }
    public Box GetBoxToFill()
    {
        return baseBox.GetBoxToFill();
    }

    [SerializeField] int waitCount;
    /* public async UniTask FillBox(Box box, UnityAction actionAfterFill)
     {
         if (box == null)
         {
             actionAfterFill?.Invoke();

             return;
         }
         await UniTask.WaitUntil(() => waitCount == 0);
         var boxColor = box.Color;
         int numNeed = box.NeedCount;
         Debug.Log(numNeed);
         var lstScrew = levelMap.LstScrew;
         var lstScrewFinded = new List<Screw>();
         for (int i = lstScrew.Count - 1; i >= 0; i--)
         {
             if (lstScrewFinded.Count >= numNeed)
                 break;
             if (boxColor == lstScrew[i].ScrewColor && lstScrew[i].State == ScrewState.OnShape)
                 lstScrewFinded.Add(lstScrew[i]);
         }
         ScrewCounter.Instance.UpdateScrew(lstScrewFinded.Count);
         Debug.Log(lstScrewFinded.Count);
         waitCount += lstScrewFinded.Count;
         for (int i = 0; i < lstScrewFinded.Count; i++)
         {
             lstScrewFinded[i].OnFillByBooster();
             MoveScrewToTargetBox(lstScrewFinded[i], () => waitCount--, box);
             lstScrew.Remove(lstScrewFinded[i]);
         }

         await UniTask.WaitUntil(() => waitCount == 0);
         actionAfterFill?.Invoke();
     }*/
    public void RemovScrewOnMap(List<Screw> lstScrews)
    {
        for (int i = 0; i < lstScrews.Count; i++)
        {
            levelMap.LstScrew.Remove(lstScrews[i]);
        }
    }
    public async UniTask AddScrewToSecretBox(Screw screw)
    {
        await secretBox.AddScrewToSecretBox(screw);
    }
    public async UniTask Get3ScrewNotMatch()
    {
        var lstColorBox = baseBox.GetListColorNotMatch();
        await UniTask.WaitUntil(() => levelMap.LstScrew != null);
        var lstScrewFinded = new List<Screw>();
        Debug.Log($"{levelMap.LstScrew.Count}/{lstScrewFinded.Count}/{lstColorBox.Count}");
        for (int i = 0; i < levelMap.LstScrew.Count; i++)
        {
            if (lstScrewFinded.Count >= 3)
                break;
            var screw = levelMap.LstScrew[i];
            if (!lstColorBox.Contains(screw.ScrewColor) && screw.State == ScrewState.OnShape)
            {
                lstColorBox.Add(screw.ScrewColor);
                lstScrewFinded.Add(screw);
            }

        }

        for (int i = 0; i < lstScrewFinded.Count; i++)
        {
            lstScrewFinded[i].OnFillByBooster();
            Debug.Log("Move");
            MoveScrewToTray(lstScrewFinded[i], () => waitCount--);
            levelMap.LstScrew.Remove(lstScrewFinded[i]);
        }
    }

    [ContextMenu("Set id level")]
    public void SetIdLevel()
    {
#if UNITY_EDITOR
        //for (int i = 0; i < lstLevelMap.Count; i++)
        //{
        //    lstLevelMap[i].LevelId = i + 1;
        //    EditorUtility.SetDirty(lstLevelMap[i]);
        //}

        //AssetDatabase.SaveAssets();
#endif
    }

    public void SetLevelMapFocus()
    {
        levelMap.SetLevelMapFocus();
    }
    public async UniTask SetLevelMapFocusAsync()
    {
        /*#if UNITY_EDITOR
                levelMap.SetLevelMapFocus();
        #else*/
        await levelMap.SetLevelMapFocusAsync();
        //#endif
    }

    private void OnDestroy()
    {
        //AssetReferenceController.Instance.UnloadAllAssets();
        //AssetBundleService.ClearCache();
    }

#if UNITY_EDITOR

    [EasyButtons.Button]
    public void SetData()
    {
        Serialize();
        EditorLogger.Log("Set Data: " + PlayerPrefs.GetString(UniqueId));
    }

    [EasyButtons.Button]
    public void GetData()
    {
        EditorLogger.Log("Get Data: " + PlayerPrefs.GetString(UniqueId));
    }

    [EasyButtons.Button]
    public void TestInitializeFromSave()
    {
        InitializeFromSave();
    }

#endif

    public void Serialize()
    {
        BaseTrayData baseTray = new BaseTrayData(UniqueId);

        for (int i = 0; i < lstTray.Count; i++)
        {
            baseTray.Trays.Add(new TrayData() { ScrewId = lstTray[i].GetScrewId() });
        }

        SerializationService.SerializeObject(UniqueId, baseTray);
    }

    public void InitializeFromSave()
    {
        BaseTrayData baseTray = SerializationService.DeserializeObject<BaseTrayData>(UniqueId);

        //Reload Box State
        for (int i = 0; i < lstTray.Count; i++)
        {
            lstTray[i].InitNewTray();
        }

        if (baseTray.Trays.Count == 6)
        {
            BoosterHandlerAddHole boosterHandler = FindFirstObjectByType<BoosterHandlerAddHole>();
            boosterHandler.AddTrayBase();
        }
        else if (baseTray.Trays.Count == 7)
        {
            BoosterHandlerAddHole boosterHandler = FindFirstObjectByType<BoosterHandlerAddHole>();
            boosterHandler.AddTrayBase();
            boosterHandler.AddTrayBase();
        }

        for (int i = 0; i < baseTray.Trays.Count; i++)
        {
            Screw screw = ScrewManager.GetScrewById(baseTray.Trays[i].ScrewId);

            if (screw != null)
            {
                lstTray[i].Fill(screw);
                lstTray[i].SetComletedAnim(true);
                screw.SetTray(lstTray[i]);
                lstScrewOnTray.Add(screw);
                screw.ChangeState(ScrewState.OnTray);
                screw.transform.parent = lstTray[i].transform;
                screw.transform.localRotation = Quaternion.Euler(0, 90, -90);
                screw.transform.localPosition = new Vector3(0, 0, -0.1f);
                screw.transform.localScale = Vector3.one;
            }
        }

        var lstTrayNonFill = lstTray.FindAll(x => !x.IsFill);

        if (lstTrayNonFill.Count == 1 || lstTrayNonFill.Count == 0)
        {
            HoleWarning.Instance.ShowWarning();
        }
        else
        {
            HoleWarning.Instance.HideWarning();
        }
    }

    public List<Screw> GetListBoxOnTray()
    {
        return lstScrewOnTray;
    }
    public void ClearData()
    {
        SerializationService.ClearData(UniqueId);
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        EditorLogger.Log(">>>> OnApplicationQuit");
        SerializationManager.SaveDataInGamePlayAll(true);
#endif
    }

    private void OnApplicationPause(bool pause)
    {
#if !UNITY_EDITOR
        if (pause)
        {
            Debug.Log(">>>> OnApplicationPause");
            SerializationManager.SaveDataInGamePlayAll(true);
        }
#endif
    }

    public bool VerifyCanSave()
    {
        if (Db.storage.USER_INFO.level <= 3 || GameManager.Instance.GameState == GameState.Stop)
        {
            return false;
        }

        if (LstTray != null && LstTray.Count > 0)
        {
            for (int i = 0; i < LstTray.Count; i++)
            {
                if (!LstTray[i].IsFill)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void RemoveScrew(Screw screw)
    {
        if (lstScrewOnTray.Contains(screw))
        {
            lstScrewOnTray.Remove(screw);
        }
    }

    public void AddScrew(Screw screw)
    {
        lstScrewOnTray.Add(screw);
    }

    public void CheckShowWarning()
    {
        var lstTrayNonFill = lstTray.FindAll(x => !x.IsFill);
        Debug.Log($"lstTrayNonFill: {lstTrayNonFill.Count}");
        if (lstTrayNonFill.Count == 1 || lstTrayNonFill.Count == 0)
        {
            HoleWarning.Instance.ShowWarning();
            // GameAnalyticController.Instance.Tracking().TrackingLastHold();
        }
        else
        {
            HoleWarning.Instance.HideWarning();
        }

        CheckLose();
    }


    public List<Screw> GetListScrewOnTray()
    {
        return lstScrewOnTray;
    }
    public void SetCanUseTransparentMode()
    {
        canUseTransparentMode = true;
    }

    public void SetUpHighlightScrewsAndShape()
    {
        foreach (var screw in levelMap.LstScrew)
        {
            screw.SetUpHighlight(matHighlight, layerScrewHighlight, layerSCrewNor);
        }
        foreach (var shape in levelMap.LstShape)
        {
            shape.SetUpHighlight(layerShapeHighlight, layerShapeNor);
        }
        foreach (var obstacle in levelMap.LstLinkObstacle)
        {
            obstacle.SetUpHighlight(layerShapeHighlight, layerShapeNor);
        }
    }
    public void ChangeHolderPosByBanner(bool isShowBanner)
    {
        levelScaleHelper.MoveOffset(isShowBanner);
    }
    public void RemoveShape(Shape shape)
    {
        if (shape == null)
            return;
        if (levelMap.LstShape.Contains(shape))
            levelMap.LstShape.Remove(shape);
        var linkObstacle = shape as LinkObstacle;
        if (linkObstacle != null)
        {
            RemoveLinkObstacle(linkObstacle);
        }
        if (levelMap.LstShape.Count == 0 && levelMap.LstLinkObstacle.Count == 0)
        {
            Debug.Log(">>>> Force Win Level Map No Shape No Link Obstacle");
            Invoke(nameof(ForceWin), 4);
        }
    }
    private void RemoveLinkObstacle(LinkObstacle linkObstacle)
    {
        if (linkObstacle == null)
            return;
        if (levelMap.LstLinkObstacle.Contains(linkObstacle))
            levelMap.LstLinkObstacle.Remove(linkObstacle);
    }
    public async UniTask OnTrayLose()
    {
        for (int i = 0; i < lstScrewOnTray.Count; i++)
        {
            var screw = lstScrewOnTray[i];
            screw.EnableHighlight();
        }
        await UniTask.Delay(1000);

        for (int i = 0; i < lstScrewOnTray.Count; i++)
        {
            var screw = lstScrewOnTray[i];
            screw.DisableHighlight();
        }
    }
    private async UniTask HideAllTray()
    {
        for (int i = 0; i < lstTray.Count; i++)
        {
            lstTray[i].transform.DOScale(Vector3.zero, 0).SetEase(Ease.OutBack);
        }
    }
    private async UniTask ShowAllTrayAsync()
    {
        var defaultScale = new Vector3(1, 1, 1) * 3.5f;
        for (int i = 0; i < lstTray.Count; i++)
        {
            lstTray[i].transform.DOScale(defaultScale, 0.3f).SetEase(Ease.OutBack);
            await UniTask.Delay(50);
        }
    }
}