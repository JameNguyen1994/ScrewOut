using Life;
using Storage;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Spin;
using DG.Tweening;

public class TabHome : MainMenuTabBase
{
    //LuckySpin
    [SerializeField] private Image progressBar;
    [SerializeField] private TMPro.TextMeshProUGUI txtValue;
    [SerializeField] private GameObject notifiLuckySpin;
    [SerializeField] private GameObject luckySpin;
    [SerializeField] private TMPro.TextMeshProUGUI txtLuckySpinAmount;
    [SerializeField] private GameObject luckySpinOpen;

    private void Awake()
    {
    }

    public void CheckUnlockLuckySpin()
    {
        if (SpinService.IsUnlock())
        {
            SpinService.Reveal();
            UpdateUILuckySpin();
            //luckySpin.SetActive(true);
        }
        else
        {
            //luckySpin.SetActive(false);
        }
    }

    private void UpdateCountdown()
    {
        luckySpinOpen.SetActive(SpinService.CanSpinByADS());

        if (!SpinService.CanSpinByADS())
        {
            System.DateTime Now = TimeGetter.Instance.Now;
            System.TimeSpan remaining = Now.Date.AddDays(1) - Now;
            txtValue.text = $"{remaining.Hours}H {remaining.Minutes}M";
            Invoke(nameof(UpdateCountdown), 1f);
        }
        else
        {
            txtValue.text = string.Empty;
        }
    }

    public void UpdateUILuckySpin()
    {
        notifiLuckySpin.SetActive(SpinService.CanSpinByScrew());
        float progressValue = (float)Db.storage.LuckySpinData.collectedScrew / SpinDefine.REQURIED_SCREW;
        progressValue = progressValue > 1 ? 1 : progressValue;
        progressBar.fillAmount = GetValueFromPercent(progressValue);
        //txtValue.text = progressValue >= 1 ? "SPIN" : string.Format("{0}/{1}", Db.storage.LuckySpinData.collectedScrew, SpinDefine.REQURIED_SCREW);
        int amount = Db.storage.LuckySpinData.collectedScrew / SpinDefine.REQURIED_SCREW;
        txtLuckySpinAmount.text = amount < 100 ? amount.ToString() : "99+";
        UpdateCountdown();
    }

    public void UpdateUIAddScrewToLuckySpin()
    {
        notifiLuckySpin.SetActive(SpinService.CanSpinByScrew());
        float progressValue = (float)Db.storage.LuckySpinData.collectedScrew / SpinDefine.REQURIED_SCREW;
        progressValue = progressValue > 1 ? 1 : progressValue;
        //txtValue.text = progressValue >= 1 ? "SPIN" : string.Format("{0}/{1}", Db.storage.LuckySpinData.collectedScrew, SpinDefine.REQURIED_SCREW);
        progressBar.DOFillAmount(GetValueFromPercent(progressValue), 0.5f);
        int amount = Db.storage.LuckySpinData.collectedScrew / SpinDefine.REQURIED_SCREW;
        txtLuckySpinAmount.text = amount < 100 ? amount.ToString() : "99+";
        UpdateCountdown();
    }

    private float GetValueFromPercent(float percent)
    {
        return 0.67f * percent + 0.18f;
    }

    public override void Init(int index)
    {
        base.Init(index);
        GoToThisTab();

        int level = Db.storage.USER_INFO.level;
        // txtLevel.text = $"Level {level}";
    }
    public override void GoToThisTab()
    {
        base.GoToThisTab();
        UITopController.Instance.OnShowTabHome();
        CheckUnlockLuckySpin();
        EditorLogger.Log("[TabHome] GoToThisTab");
        CoreRetentionController.Instance.OnShowUI();
    }
    public override void ExitThisTab()
    {
        // ShopIAPController.Instance.OnClickClose();
        EditorLogger.Log("[TabHome] ExitThisTab");
        CoreRetentionController.Instance.OnHideUI();
    }

    public void OnClickPlay()
    {
        PlayAsync().Forget();
    }
    public async UniTask PlayAsync()
    {
        await CoreRetentionController.Instance.MoveToCurentLevel(null);

        SpeedGameManager.Instance.Active(false);
        AudioController.Instance.PlaySound(SoundName.Click);

        bool willContinue = await CheckBeforeGoToGamePlay(SceneType.GamePlayNewControl);
        willContinue = false;
        if (willContinue)
        {
            await ReloadGamePlayAsync();
        }
        else
        {
            if (DBLifeController.Instance.LIFE_INFO.lifeAmount == 0 && DBLifeController.Instance.LIFE_INFO.timeInfinity <= 0)
            {
                LifeController.Instance.ShowPopupLife();
            }
            else
            {
                PreBoosterController.Instance.ShowPopupBooster();
                //SceneController.Instance.ChangeScene(SceneType.GamePlayNewControl);
                UITopController.Instance.OnStartGameplay();
            }
        }
    }

    private async UniTask ReloadGamePlayAsync()
    {
        UITopController.Instance.OnStartGameplay();

        SceneController.Instance.ChangeScene(SceneType.GamePlayNewControl);
    }

    private async UniTask<bool> CheckBeforeGoToGamePlay(SceneType targetScene)
    {
        if (SerializationManager.HaveDataToReloadGamePlay())
        {
            if (PlayerPrefsSaveTime.GetMinutesSinceLastSave() < Define.AUTO_RELOAD_TIME)
            {
                return false;
            }

            bool userConfirmed = await SerializationManager.ShowConfirmAsync(Define.RELOAD_CONFIRM);

            if (userConfirmed)
            {
                return true;
            }

            SerializationManager.ClearAllDataInGamePlay();
        }

        return true;
    }
}