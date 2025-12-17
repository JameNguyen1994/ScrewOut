using Cysharp.Threading.Tasks;
using DG.Tweening;
using Life;
using PS.Analytic;
using Storage;
using Storage.Model;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupPreBooster : PopupBase
{
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private Button btnPlay, btnClose;
    [SerializeField] private Transform tfmPopup;
    [SerializeField] private Image imgBG;
    [SerializeField] private Sprite sprNormal;
    [SerializeField] private Sprite sprHard;
    [SerializeField] private GameObject gobjTagHard;

    [SerializeField] private Image imgButtonPlay;
    [SerializeField] private Sprite sprNormalPlay;
    [SerializeField] private Sprite sprHardPlay;

    [SerializeField] private GameObject txtNormal;
    [SerializeField] private GameObject txtHard;
    [SerializeField] private bool isShow = false;

    public bool IsShow { get => isShow;}

    public async UniTask Setup()
    {
        // base.InitData(data);
        IngameData.preBoosterPlace = PreBoosterPlace.Home;
        Db.storage.PreBoosterData.OnNewGame();
        Debug.Log("PopupPreBooster InitData");
        int level = Db.storage.USER_INFO.level;
        txtLevel.text = $"Level {level}";
        btnPlay.transform.localScale = Vector3.zero;
        btnClose.transform.localScale = Vector3.zero;
        tfmPopup.localScale = Vector3.zero;
        PreBoosterController.Instance.PreBoosterRocket.Reset();
        PreBoosterController.Instance.PreBoosterGlass.Reset();
        LevelDifficulty levelDifficulty = LevelMapService.GetLevelDifficulty(level);
        var isHard = levelDifficulty == LevelDifficulty.Hard;
        imgBG.sprite = isHard ? sprHard : sprNormal;
        imgButtonPlay.sprite = isHard ? sprHardPlay : sprNormalPlay;
        gobjTagHard.SetActive(isHard);
        txtNormal.SetActive(!isHard);
        txtHard.SetActive(isHard);
    }
    public void ResetUI()
    {
    }    
    public override async UniTask Show()
    {
        isShow = true;

        await base.Show();
        tfmPopup.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        PreBoosterController.Instance.PreBoosterRocket.Show(true);
        await PreBoosterController.Instance.PreBoosterGlass.Show(true);

        await btnPlay.transform.DOScale(Vector3.one * 0.85f, 0.3f).SetEase(Ease.OutBack);
        await btnClose.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    public void OnClickHidePopup()
    {
        Hide();
        UITopController.Instance?.OnShowMainMenu();
    }
    public void OnClickPlay()
    {
        PlayHandler();
    }

    public async void PlayHandler()
    {
        UserInfo userInfo = Db.storage.USER_INFO;
        bool isHaveCacheLevel = await AssetBundleService.IsHaveCachLevel(userInfo.level);
        int realLevel = LevelMapService.GetLevelMap(userInfo.level);

        if (!isHaveCacheLevel && AssetBundleService.HasAssetBundle(realLevel))
        {
            Debug.Log("[Cache Level] Do not have cache Level");
            Hide();
            PopupDownloadLevel.Instance.Show().Forget();
            return;
        }

        MainMenuRecieveRewardsHelper.Instance.UpdateUIForReward();
        SceneController.Instance.ChangeScene(SceneType.GamePlayNewControl);
        UITopController.Instance.OnStartGameplay();
        Hide();
    }
    public override void Hide()
    {
        base.Hide();
        isShow = false;

    }
}
