using ps.modules.journey;
using PS.Utils;
using Storage;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PS.UnitTest;
using UnityEngine;
using UnityEngine.UI;
using WeeklyQuest;
using TMPro;

public class GamePlayCheat : Singleton<GamePlayCheat>
{
    [SerializeField] private GameObject boxCheat;
    [SerializeField] private List<GameObject> areaCheat;
    [SerializeField] private InputField inputLevel;
    [SerializeField] private Text currentCheatTime;
    [SerializeField] private List<GameObject> lstGameObjectSafeArea;
    public bool IsEnabledCheat = false;

    [SerializeField] private InputField inputWrench;
    [SerializeField] private InputField inputAALevel;
    [SerializeField] private TMP_InputField inputExp;
    [SerializeField] private Text txtEasyMode;


    public void Start()
    {
#if UNITY_EDITOR
        boxCheat.SetActive(true);
#else
        boxCheat.SetActive(Debug.unityLogger.logEnabled);
#endif
        OnEnableClick();
        currentCheatTime.text = $"Cheat Time:({Db.storage.WEEK_INFO.cheatTime / (60 * 60 * 1000)}) +6";
        bool easyMode = PlayerPrefs.GetInt("EasyMode", 0) == 1;
        txtEasyMode.text = easyMode ? "Easy Mode: ON" : "Easy Mode: OFF";
    }

    public void OnEnableClick()
    {
        GlobalCanvas.Instance.EnableButton(!areaCheat[0].active);
        for (int i = 0; i < areaCheat.Count; i++)
        {
            areaCheat[i].SetActive(!areaCheat[i].active);
        }
    }

    public void OnNextToLevelClick()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        IsEnabledCheat = true;
        //PopupController.Instance.ShowWin();
        var data = Db.storage.USER_INFO;
        int value = 0;
        if (int.TryParse(inputLevel.text, out value))
        {
            data.level = value;
        }
        else
        {
            data.level++;
            JourneyController.Instance.OnChangeLevel();
            Debug.LogError("Invalid level input. Please enter a valid number.");
        }

        Db.storage.USER_INFO = data;
        SceneController.Instance.ChangeScene(SceneType.GamePlayNewControl);
        LevelController.Instance.GetMoreLevelForUnitTest();
#endif
    }

    public void AddWrench()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        int value = 0;

        if (int.TryParse(inputWrench.text, out value))
        {
            WrenchCollectionService.CollectWrench(value);
        }
#endif
    }

    public void OnNextLevelClick()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        LevelController.Instance.Level.transform.localScale = Vector3.zero;
        LevelController.Instance.ForceWin();
#endif
    }

    public void OnReturnLevelClick()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        var userInfo = Db.storage.USER_INFO;
        userInfo.level--;
        Db.storage.USER_INFO = userInfo;
        var scene = SceneType.GamePlayNewControl;
        SceneController.Instance.ChangeScene(scene);
#endif
    }
    public void OnClickCheatBooster()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        var boosterData = Db.storage.BOOSTER_DATAS;
        boosterData.AddBooster(BoosterType.UnlockBox, 1);
        boosterData.AddBooster(BoosterType.AddHole, 1);
        boosterData.AddBooster(BoosterType.Hammer, 1);
        boosterData.AddBooster(BoosterType.Clears, 1);
        //boosterData.AddBooster(BoosterType.Magnet, 1);
        BoosterController.Instance.Init();
        Db.storage.BOOSTER_DATAS = boosterData;
#endif
    }

    [ContextMenu("OnCheatExpClick")]
    public void OnCheatExpClick()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        var expToAdd = 0;
        if (int.TryParse(inputExp.text, out expToAdd))
        {
            ExpBar.Instance.AddExp(expToAdd);
        }
        else
        {
            Debug.LogError("Invalid exp input. Please enter a valid number.");
        }
        ExpBar.Instance.AddExp(expToAdd);
#endif
    }
    [ContextMenu("OnCheatExpClick")]
    public void OnCheatAddCoin()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        var user = Db.storage.USER_INFO;
        user.coin += 100;
        Db.storage.USER_INFO = user;
        EventDispatcher.Push(EventId.UpdateCoinUI, 100);
#endif
    }
    [ContextMenu("OnCheatExpClick")]
    public void OnCheatSubCoin()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        var user = Db.storage.USER_INFO;
        int coinsub = Mathf.Min(100, user.coin);
        user.coin -= coinsub;
        SingularSDK.Event("COIN_SPEND");

        WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.UseCoins, coinsub);

        Db.storage.USER_INFO = user;
        EventDispatcher.Push(EventId.UpdateCoinUI, -coinsub);
#endif

    }
    public void OnSafeAreaClick()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        for (int i = 0; i < lstGameObjectSafeArea.Count; i++)
        {
            lstGameObjectSafeArea[i].SetActive(!lstGameObjectSafeArea[i].active);
        }
#endif
    }
    public void CheatAddPreBooster()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        var preBoosterData = Db.storage.PreBoosterData;
        preBoosterData.AddValue(PreBoosterType.Rocket, 10);
        preBoosterData.AddValue(PreBoosterType.Glass, 10);
#endif
    }

    public void RemoveAALevel()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        int level = 0;
        if (int.TryParse(inputAALevel.text, out level))
        {
            AssetBundleService.RemoveOldLevelBundleAsset(level);

            AssetBundleData bundleData = AssetBundleService.GetBundleDataByLevel(level);

            if (bundleData != null)
            {
                string localPath = AssetBundleService.GetLocalPath(bundleData.GetBundleName());

                if (File.Exists(localPath))
                {
                    Debug.Log("[RemoveAALevel] Failed");
                }
                else
                {
                    Debug.Log("[RemoveAALevel] Success");
                }
            }
        }
#endif
    }

    public async void Redownload()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        int level = 0;
        if (int.TryParse(inputAALevel.text, out level))
        {
            AssetBundleData bundleData = AssetBundleService.GetBundleDataByLevel(level);

            string fullURL = AssetBundleService.Config.GetFullURL(bundleData.GetBundleURL());
            string localPath = AssetBundleService.GetLocalPath(bundleData.GetBundleName());

            bool success = await WebRequestService.DownloadFileAsync(fullURL, localPath); // Download file

            if (!success)
            {
                Debug.Log("[Redownload] Failed");
            }
            else
            {
                Debug.Log("[Redownload] Success");
                AssetBundleService.UnloadBundle(bundleData);
                SceneController.Instance.ChangeScene(SceneType.GamePlayNewControl);
            }
        }
#endif
    }
    public async void ForceEasyMode()
    {
#if DEBUG_ENABLE || UNITY_EDITOR
        bool easyMode = PlayerPrefs.GetInt("EasyMode", 0) == 1;
        easyMode = !easyMode;
        PlayerPrefs.SetInt("EasyMode", easyMode ? 1 : 0);
        txtEasyMode.text = easyMode ? "Easy Mode: ON" : "Easy Mode: OFF";
#endif
    }
}