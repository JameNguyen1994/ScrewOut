using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PS.Analytic;
using Storage;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;



#if UNITY_EDITOR
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class LevelBonusController : Singleton<LevelBonusController>
{
    //[SerializeField] private List<LevelMap> lstLevelMap;
    [SerializeField] private LevelMap levelMap;
    [SerializeField] private LevelMap levelMapLoad;
    [SerializeField] private LevelMap levelDefault;

    [SerializeField] private List<Tray> lstTray = new List<Tray>();
    [SerializeField] private BoxLevelBonusController boxLevelBonusController;
    [SerializeField] private BaseBox baseBox;
    [SerializeField] private List<Screw> lstScrewOnTray = new List<Screw>();
    [SerializeField] private LevelScaleHelper levelScaleHelper;
    [SerializeField] private List<LevelMap> lstLevelNap;

    [SerializeField] private PopupLevelBonus popupLevelBonus;
    [SerializeField] private UILevelBonus uILevelBonus;
    [SerializeField] private bool completed = false;
    [SerializeField] private int numBox = 0;
    [SerializeField] private Transform levelBonusHolder;
    [SerializeField] private List<ScrewColor> lstLevelBoxColor;
    [SerializeField] private bool isCheckingListTray;
    [SerializeField] private Transform tfmHolderHole;
    [SerializeField] private bool isStartCountDownTime = false;
    [SerializeField] private UIEffect uIEffectDissolveAds;
    [SerializeField] private GameObject gobjLevelLoad;


    [Header("Setup Highlight Screw")]
    [SerializeField] private Material matHighlight;
    [SerializeField] private LayerMask layerScrewHighlight;
    [SerializeField] private LayerMask layerSCrewNor;
    [Header("Setup Highlight Shape")]
    [SerializeField] private LayerMask layerShapeHighlight;
    [SerializeField] private LayerMask layerShapeNor;


    public bool IsCompleted
    {
        get => numBox == 0;
    }
    public LevelMap Level
    {
        get => levelMap;
    }
    public void StartCountDownTime()
    {
        isStartCountDownTime = true;
    }
    public async UniTask WaitUntilStartCountDownTime()
    {
        await UniTask.WaitUntil(() => isStartCountDownTime);
    }
    public BoxLevelBonusController BoxLevelBonusController { get => boxLevelBonusController; }


    [EasyButtons.Button]
    public async UniTask ShowBonusLevel()
    {
        var remote = GameAnalyticController.Instance.Remote();
        numBox = 0;
        AudioController.Instance.StopMusic(SoundName.Music);
        AudioController.Instance.PlayMusic(SoundName.MUSIC_LEVEL_BONUS, true);
        isCheckingListTray = false;
        IngameData.GameMode = GameMode.Bonus;
        Db.storage.IS_COLOR_MODE = true;
        BoosterController.Instance.HideAllBeforeShow();
        SliderZoom.Instance.Hide();
        baseBox.HideAll();
        ScreenGamePlayUI.Instance.HideUIBtnPause();
        ScreenGamePlayUI.Instance.LevelProgressBar.HideUI();
        WrenchCollectionGamePlayController.Instance.HideUI();
        Destroy(LevelController.Instance.Level.gameObject);




        for (int i = 0; i < lstTray.Count; i++)
        {
            lstTray[i].gameObject.SetActive(true);
        }
        tfmHolderHole.DOMoveX(-3f, 0.1f);
        // Color
        uIEffectDissolveAds.gameObject.SetActive(true);
        DOVirtual.Float(1, 0, 2f, (value) =>
        {
            uIEffectDissolveAds.transitionRate = value;
        }).SetEase(Ease.Linear);
        await popupLevelBonus.ShowBonusLevel();
        await uILevelBonus.ShowClock(remote.BonusTime);


        await Init();
        Debug.Log($"levelMap == null {levelMap == null}");
        //  await baseBox.InitBoxs();
        InitData();
        levelScaleHelper.ChangCenterPos(levelMap.LstShape, levelMap.LstLinkObstacle);
        levelScaleHelper.SetLevelTransform(levelMap.transform);
        await levelScaleHelper.CastListScrewTranform(levelMap.LstScrew, levelMap.LevelMapSize);
        var focusTas = SetLevelMapFocusAsync();
        await UniTask.WhenAll(focusTas);
        await UniTask.Delay(100);
        boxLevelBonusController.gameObject.SetActive(true);
        boxLevelBonusController.InitBoxs();
        //  await levelMap.ShowLevelMapScrew();
        await WaitUntilStartCountDownTime();
        await uILevelBonus.StartCountTime(remote.BonusTime, ActionOnFinish);

        CheckAllBoxLoop().Forget();
        await UniTask.WaitUntil(() => completed);

        AudioController.Instance.StopMusic(SoundName.MUSIC_LEVEL_BONUS);
        AudioController.Instance.PlayMusic(SoundName.Music, true);
    }
    private void ActionOnFinish()
    {
        completed = true;
    }
    private void InitData()
    {
        var colorCount = levelMap.LstScrew.Count;
        var lstColors = new List<ScrewColor>() { ScrewColor.Red, ScrewColor.Blue, ScrewColor.Green, ScrewColor.Yellow, ScrewColor.Purple, ScrewColor.Orange };
        int numColor = 2;
        var lstSpawnColor = new List<ScrewColor>();
        var lstLevelBoxColor = new List<ScrewColor>();
        var numBoxPerColor = colorCount / 3 / numColor;
        var remainder = colorCount / 3 % numColor;
        for (int i = 0; i < numColor; i++)
        {
            for (int j = 0; j < numBoxPerColor; j++)
            {
                lstSpawnColor.Add(lstColors[i]);
                lstSpawnColor.Add(lstColors[i]);
                lstSpawnColor.Add(lstColors[i]);
                lstLevelBoxColor.Add(lstColors[i]);
                numBox++;
            }
        }
        for (int i = 0; i < remainder; i++)
        {
            lstSpawnColor.Add(lstColors[0]);
            lstSpawnColor.Add(lstColors[0]);
            lstSpawnColor.Add(lstColors[0]);
            lstLevelBoxColor.Add(lstColors[0]);
            numBox++;
        }
        for (int i = 0; i < lstSpawnColor.Count; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, lstSpawnColor.Count);
            var temp = lstSpawnColor[i];

            lstSpawnColor[i] = lstSpawnColor[randIndex];
            lstSpawnColor[randIndex] = temp;
        }
        lstLevelBoxColor = AlternateStrict_NoWhile(lstLevelBoxColor);
        levelMap.SetDataColor(lstSpawnColor);
        boxLevelBonusController.Init(lstLevelBoxColor);
    }
    // ==================================================================
    // 🔥 Hàm đảm bảo xen kẽ 1–1 tuyệt đối: A B A B A B ...
    private List<ScrewColor> AlternateStrict_NoWhile(List<ScrewColor> input)
    {
        // Gom màu
        Dictionary<ScrewColor, List<ScrewColor>> groups = new();
        foreach (var c in input)
        {
            if (!groups.ContainsKey(c))
                groups[c] = new List<ScrewColor>();
            groups[c].Add(c);
        }

        var keys = groups.Keys.ToList(); // ví dụ [Red, Blue]
        int colorCount = keys.Count;

        // Tính tổng để chạy đúng số lần
        int total = input.Count;

        // Con trỏ mỗi màu
        int[] idx = new int[colorCount];

        List<ScrewColor> output = new(total);

        int current = 0;

        for (int i = 0; i < total; i++)
        {
            // tìm màu còn slot
            int attempt = 0;
            ScrewColor chosen = keys[current];

            while (idx[current] >= groups[chosen].Count)
            {
                current = (current + 1) % colorCount;
                chosen = keys[current];
                attempt++;

                if (attempt >= colorCount)
                    break; // fallback nếu hết màu
            }

            if (idx[current] < groups[chosen].Count)
            {
                output.Add(groups[chosen][idx[current]]);
                idx[current]++;
            }

            // Đảm bảo lần sau đổi màu (xen kẽ)
            current = (current + 1) % colorCount;
        }

        return output;
    }

    public async UniTask InitLevelCache()
    {
        var data = Db.storage.LEVEL_BONUS_DATA;
        var lstUsed = data.lstLevelBonusUsed;
        var lstIndex = new List<int>();
        string name = "Level_Bonus_";
        int numLevelBonus = 10;
        string lstStrUsed = "";
        int lstUsedCount = lstUsed.Count;

        int levelCacheCount = 2;
        int minIndex = Mathf.Min(levelCacheCount + lstUsedCount, numLevelBonus);
        /*        for (int i = lstUsedCount; i < minIndex; i++)
                {
                    int index = data.lstLevelBonusStart[i];
                    var levelName = name + index;
                    AssetBundleService.DownloadMapLevelBonus(levelName);
                    Debug.Log($"LevelBonusController Download : {levelName}");
                }
        */
    }
    public void OnComplete1Box()
    {
        numBox--;
        Debug.Log($"OnComplete1Box numBox: {numBox}");
        if (numBox <= 0)
        {
            //completed = true;
            Debug.Log("OnComplete1Box Complete Level Bonus!");
        }
    }
    private int GetLevelIndex()
    {
        var data = Db.storage.LEVEL_BONUS_DATA;
        var lstUsed = data.lstLevelBonusUsed;
        return lstUsed.Count;
    }
    public async UniTask Init()
    {
        int levelIndex = GetLevelIndex();
        Debug.Log($"LevelBonusController Init: levelIndex {levelIndex}");
        if (levelIndex >= 10)
        {
            Debug.Log($"LevelBonus_test_1");
            levelIndex = 0;
            var data = Db.storage.LEVEL_BONUS_DATA;
            data.lstLevelBonusUsed.Clear();
            Db.storage.LEVEL_BONUS_DATA = data;
        }
        else
        {
            Debug.Log($"LevelBonus_test_2");
            var data = Db.storage.LEVEL_BONUS_DATA;
            data.lstLevelBonusUsed.Add(levelIndex);
            Db.storage.LEVEL_BONUS_DATA = data;
        }
        Debug.Log($"LevelBonus_test_3");
        var level = Db.storage.LEVEL_BONUS_DATA.lstLevelBonusStart[levelIndex];
        var name = $"Level_Bonus_{level}";
        levelMapLoad = await AssetReferenceController.Instance.LoadLevelBonusPrefabAsync<LevelMap>(level);
        Debug.Log($"LevelBonus_test_4");
        if (levelMapLoad != null)
        {
            Debug.Log("LevelBonusController InitLevelCache: Instantiate Level Bonus from AssetBundle");
            levelMap = Instantiate(levelMapLoad);
        }
        else
        {
            Debug.Log($"LevelBonusController InitLevelCache: Load Level Bonus from Cache {name}");
            levelMap = levelDefault;
        }
        Debug.Log($"LevelBonus_test_5");
        /////
        if (levelMap == null)
        {
            Debug.Log("LevelBonusController Init: Load Level Bonus from Cache");
            levelMap = levelDefault;
        }
        levelMap.gameObject.SetActive(true);
        levelMap.transform.SetParent(levelBonusHolder);
        levelMap.transform.localPosition = Vector3.zero;
        levelMap.Init();
        SetUpHighlightScrewsAndShape();
        //levelMap.ShowLevelMap();
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
    public void SetLevelMapFocus()
    {
        levelMap.SetLevelMapFocus();
    }
    public async UniTask SetLevelMapFocusAsync()
    {
        await levelMap.SetLevelMapFocusAsync();
    }
    public void RemoveScrew(Screw screw)
    {
        if (lstScrewOnTray.Contains(screw))
        {
            lstScrewOnTray.Remove(screw);
            Debug.Log($"RemoveScrew Color: {screw.ScrewColor} - LstScrewOnTray: {lstScrewOnTray.Count}");
            if (lstLevelBoxColor.Count == 0)
            {
                Debug.LogError("RemoveScrew lstLevelBoxColor is empty!");
            }
        }
    }
    public void AddScrew(Screw screw)
    {
        lstScrewOnTray.Add(screw);
    }
    public async UniTask CheckAllBox()
    {
        //bool isWait = true;
        var lstBoxUnlock = boxLevelBonusController.GetAllUnlock();
        var lstTask = new List<UniTask>();
        for (int i = 0; i < lstBoxUnlock.Count; i++)
        {
            var box = lstBoxUnlock[i];
            if (!box.IsBoxFull())
            {
                CheckNewBoxOnTray(box);
            }
        }
        await UniTask.WhenAll(lstTask);
    }
    private void Update()
    {

    }
    private async UniTask CheckAllBoxLoop()
    {
        while (!completed)
        {
            await UniTask.WaitForSeconds(1f);
            CheckAllBox();
        }
    }
    private async UniTask CheckNewBoxOnTray(Box box)
    {
        await UniTask.WaitUntil(() => !isCheckingListTray);
        isCheckingListTray = true;
        Debug.Log($"CheckNewBoxOnTray Color: {box.Color} - LstScrewOnTray: {lstScrewOnTray.Count}");
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
}