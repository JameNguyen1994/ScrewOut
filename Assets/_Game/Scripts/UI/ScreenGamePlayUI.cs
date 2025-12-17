using PS.Utils;
using Storage;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PS.Analytic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGamePlayUI : Singleton<ScreenGamePlayUI>
{
    [SerializeField] private Text txtLevel;
    [SerializeField] private GameObject gobjTutorialClick;
    [SerializeField] private GameObject gobjTutorialSwip;

    [SerializeField] private GameObject gobjExpBar;
    [SerializeField] private GameObject gobjProcess;
    [SerializeField] private GameObject gobjBtnPause;
    [SerializeField] private Image imgProcess;
    [SerializeField] private Button btnSoftCurrency;
    [SerializeField] private PanelCoin panelCoin;
    [SerializeField] private LevelProgressBar levelProgressBar;
    [SerializeField] private GameObject gobjWarningLose;

    
    private bool isTrackingClick = true;

    public LevelProgressBar LevelProgressBar { get => levelProgressBar;  }

    protected override void CustomAwake()
    {
        base.CustomAwake();
        gobjBtnPause.SetActive(false);

        btnSoftCurrency.onClick.AddListener(OnSoftCurrencyClick);
    }
    public void HideUIBtnPause()
    {
        gobjBtnPause.SetActive(false);
    }
    private void Start()
    {
        var level = Db.storage.USER_INFO.level;
        txtLevel.text = $"Level {level}";
        
        DelayToInitProgressBar(level).Forget();

        var remote = GameAnalyticController.Instance.Remote();
        //gobjExpBar.SetActive(remote.SegmentFlow.enableExpBar);
       // gobjProcess.SetActive(true);//!remote.SegmentFlow.enableExpBar);
        //gobjExpBar.SetActive(!true);
        //gobjProcess.SetActive(!!true);
        //imgProcess.enabled = !remote.SegmentFlow.enableExpBar;
        
        //ShopIAPController.Instance.OfferwallRewardPopup.Show();
    }
    public void OnLoadGameCompleted()
    {
        gobjBtnPause.SetActive(true);
    }

    async UniTask DelayToInitProgressBar(int level)
    {
        await UniTask.Delay(200);
        InitLevelProgressBar(level);
    }

    void InitLevelProgressBar(int level)
    {
        levelProgressBar.InitLevel(level);
        levelProgressBar.SetProgress(0);
    }
    
    public void UpdateLevelProgressBar(int plus)
    {
        levelProgressBar.SetProgress(plus);
    }

    public void OnEnableTutorialSwip(bool enable)
    {
        if (IngameData.MODE_CONTROL == ModeControl.ControlV2) return;
        if (!IngameData.DONE_TUTORIAL && Db.storage.USER_INFO.level == 1)
        {
            if (enable)
            {
                OnEnableTutorialClick(false);
                if (isTrackingClick)
                {
                   // TrackingController.Instance.TrackingTutorial();
                    isTrackingClick = false;
                }
            }
            else
            {
                //GameAnalyticController.Instance.Tracking().TrackingTUTORIAL(2);

            }

            gobjTutorialSwip.SetActive(enable);

        }

    }
    public void OnEnableTutorialClick(bool enable)
    {
        if (IngameData.MODE_CONTROL == ModeControl.ControlV2) return;

        if (!IngameData.DONE_TUTORIAL && Db.storage.USER_INFO.level == 1)
        {
            gobjTutorialClick.SetActive(enable);
        }

    }

    public void OnRetryClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        TrackingController.Instance.TrackingRetry();
        var scene = SceneType.GamePlayNewControl;

        SceneController.Instance.ChangeScene(scene);
    }
    public void OnTryAgainClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        TrackingController.Instance.TrackingGamePlay(LEVEL_STATUS.out_of_slot);
        var scene =SceneType.GamePlayNewControl ;

        SceneController.Instance.ChangeScene(scene);
    }

    public void OnSoftCurrencyClick()
    {
        //ShopIAPController.Instance.ShowAbbreviated();
        InputHandler.Instance.IsLockInput = true;
        ShopIAPController.Instance.ShowFullGameplay();
        UITopController.Instance.OnShowCoinAndHeart();
        ShopIAPController.Instance.OnClickShowPopup(() =>
        {
            PopupController.Instance.PopupCount--;
            InputHandler.Instance.IsLockInput =false;
            UITopController.Instance.OnHideCoinAndHeart();
            panelCoin.TogglePanel(true);
        }, true);
        ShopIAPController.Instance.ExpandBottom(false);
        panelCoin.TogglePanel(false);
    }
    

    public void OnRevive()
    {
        AdsController.Instance.ShowRewardAds(RewardAdsPos.revive, () =>
        {
            PopupController.Instance.reviveCount++;
            PopupController.Instance.HideOutOfSlot();
            LevelController.Instance.MoveScrewOnTrayToSecretBox().Forget();
        }, null, null, AdsPlacement.reward_revive.ToString());
    }

  public async UniTask ShowWarningLose(float time)
    {
        gobjWarningLose.SetActive(true);
        await UniTask.WaitForSeconds(time);
        gobjWarningLose.SetActive(false);
    }
}
