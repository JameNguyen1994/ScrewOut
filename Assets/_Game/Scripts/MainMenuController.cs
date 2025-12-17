using Cysharp.Threading.Tasks;
using DailyReward;
using EasyButtons;
using MainMenuBar;
using Storage;
using System.Threading.Tasks;
using UnityEngine;
using WeeklyQuest;

public class MainMenuController : Singleton<MainMenuController>
{
    [SerializeField] private TabHome tabHome;
    [SerializeField] private PopupSettings popupSettings;
    [SerializeField] private RandomAnimMainMenu randomAnimMainMenu;
    [SerializeField] private BackgroundChange backgroundChange;

    public static bool IsShowSupperOfferOnMain = false;

    private async UniTask Start()
    {
        await UniTask.WaitUntil(()=> TimeGetter.Instance.IsGettedTime);
        //LoadingFade.Instance.HideLoadingFade();

        await backgroundChange.ChangeBackgroundAsync();
        await PreBoosterController.Instance.Init();
        await MainMenuRecieveRewardsHelper.Instance.Init();

        await CheckNewDayReward();
        await CheckWeeklyQuest();

        if (MainMenuBarController.Instance != null && PreBoosterController.Instance.IsShow == false)
            await MainMenuBarController.Instance.CheckTutorial();

        randomAnimMainMenu.StartAnim();
    }
    public async UniTask MoveToHomePage()
    {
        await MainMenuBarController.Instance.GoToTabHome();
    }
    public async UniTask CheckNewDayReward()
    {
        if (!MainMenuService.IsUnlockDailyGift())
        {
            return;
        }

        await UniTask.WaitUntil(() => DailyRewardManager.Instance.DailyRewardController.Inited);
        bool hasReward = await DailyRewardManager.Instance.DailyRewardController.HasReward();
        if (hasReward)
        {
           await  DailyRewardManager.Instance.DailyRewardPopup.Show();
            await DailyRewardManager.Instance.DailyRewardPopup.WaitToClose();
        }
    }
    public async UniTask CheckWeeklyQuest()
    {

        await UniTask.WaitUntil(() => Db.storage.WeeklyQuestData != null);
        WeeklyQuestManager.Instance.WeeklyQuestController.UpdateQuestNoti();
    }
    [Button]
    public void ShowPopupPause()
    {
        if (MainMenuRecieveRewardsHelper.Instance != null && MainMenuRecieveRewardsHelper.Instance.IsShowReward)
        {
            return;
        }

        AudioController.Instance.PlaySound(SoundName.Click);

        popupSettings.Show();
    }

    public void OnClickShowPopupPrebooster()
    {
        PreBoosterController.Instance.ShowPopupBooster();
    }

    public void UpdateUILuckySpin()
    {
        tabHome.UpdateUILuckySpin();
    }

    public void UpdateUIAddScrewToLuckySpin()
    {
        tabHome.UpdateUIAddScrewToLuckySpin();
    }
}
