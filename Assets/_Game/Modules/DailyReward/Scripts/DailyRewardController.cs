using Cysharp.Threading.Tasks;
using DailyReward;
using Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardController : MonoBehaviour
{
    [SerializeField] private List<DailyRewardItem> dailyRewards;
    [SerializeField] private bool canGetReward = false;
    [SerializeField] private Transform tfmButtonClaim;
    [SerializeField] private Transform tfmNoti;
    [SerializeField] private Button btnClaim;
    [SerializeField] private DailyRewardItem currentDay;
    [SerializeField] private TextMeshProUGUI txtNoti;
    [SerializeField] private TextMeshProUGUI txtStreak;
    [SerializeField] private TextMeshProUGUI txtNextTimeAvailable;
    [SerializeField] private bool inited;

    [SerializeField] private TextMeshProUGUI txtButtonDaily;
    [SerializeField] private GameObject goButtonDaily;

    public bool Inited { get => inited; }

    private async UniTask Start()
    {
        await UniTask.WaitUntil(() => TimeHelperController.Instance.IsInitedTime);
        inited = false;
        tfmNoti.gameObject.SetActive(false);
        await InitData();
        Init();
        CheckDayCanReceive();
        SetUIReward();
        TimeGetter.Instance.RegisActionOnUpdateTime(UpdateTime);

        inited = true;
    }
    private void OnEnable()
    {
    }
    private void OnDestroy()
    {
        TimeGetter.Instance.UnRegisActionOnUpdateTime(UpdateTime);
    }
    public async UniTask InitData()
    {
        await UniTask.WaitUntil(() => Db.storage.Inited);
        Db.storage.InitializeDailyReward().Forget();

    }
    public void Init()
    {
        var weekData = DailyRewardManager.Instance.DailyRewardHelper.WeeklyData;
        int maxDay = Mathf.Min(weekData.days.Count, dailyRewards.Count);
        int streak = Db.storage.DAILY_REWARD_DATA.streak;
        Debug.Log($"Max days: {maxDay}, Total daily rewards: {dailyRewards.Count}");
        for (int i = 0; i < maxDay; i++)
        {
            var dayData = weekData.GetData(i);
            var dailyRewardItem = dailyRewards[i];
            dailyRewardItem.gameObject.SetActive(i < maxDay);
            dailyRewardItem.Init(i, dayData);
        }
    }
    public async UniTask<bool> HasReward()
    {
        await InitData();
        if (Db.storage.DAILY_REWARD_DATA.nextAvailableTime == 0)
        {
            canGetReward = true;
            Db.storage.DAILY_REWARD_DATA.SetNextAvailableTime(TimeGetter.Instance.CurrentTime);
        }
        else
        {
            if (Db.storage.DAILY_REWARD_DATA.nextAvailableTime <= TimeGetter.Instance.CurrentTime)
            {
                canGetReward = true;
            }
            else
            {
                canGetReward = false;
            }
        }
        if (TimeGetter.Instance.CurrentTime - Db.storage.DAILY_REWARD_DATA.nextAvailableTime > 24 * 60 * 60 * 1000)
        {
            Db.storage.DAILY_REWARD_DATA.ResetStreak(TimeGetter.Instance.CurrentTime);
            Reset();
        }
        txtNoti.gameObject.SetActive(!canGetReward);
        return canGetReward;
    }
    private void CheckDayCanReceive()
    {
        if (Db.storage.DAILY_REWARD_DATA.nextAvailableTime == 0)
        {
            canGetReward = true;
            Db.storage.DAILY_REWARD_DATA.SetNextAvailableTime(TimeGetter.Instance.CurrentTime);
        }
        else
        {
            if (Db.storage.DAILY_REWARD_DATA.nextAvailableTime <= TimeGetter.Instance.CurrentTime)
            {
                canGetReward = true;
            }
            else
            {
                canGetReward = false;
            }
        }
        if (TimeGetter.Instance.CurrentTime - Db.storage.DAILY_REWARD_DATA.nextAvailableTime > 24 * 60 * 60 * 1000)
        {
            Db.storage.DAILY_REWARD_DATA.ResetStreak(TimeGetter.Instance.CurrentTime);
            Reset();
        }
        currentDay = dailyRewards[Db.storage.DAILY_REWARD_DATA.streak];
        if (canGetReward)
        {
            currentDay.SetCurrentDay();
            if (currentDay.Index + 1 < dailyRewards.Count)
                dailyRewards[currentDay.Index + 1].SetNextDay();
        }
        else
            currentDay.SetNextDay();
        tfmNoti.gameObject.SetActive(canGetReward);
        tfmButtonClaim.gameObject.SetActive(canGetReward);
    }

    public void SetUIReward()
    {
        for (int i = 0; i < dailyRewards.Count; i++)
        {
            if (i < currentDay.Index)
            {
                dailyRewards[i].SetPassedDay();
            }
        }
    }
    public void OnClickClaim()
    {
        tfmButtonClaim.gameObject.SetActive(false);
        tfmNoti.gameObject.SetActive(false);
        OnClaimReward();
    }
    public async UniTask OnClaimReward()
    {
        if (canGetReward)
        {
            canGetReward = false;
            currentDay.GetReward();
            Db.storage.DAILY_REWARD_DATA.AddStreak();
            await currentDay.SetCheckAsync();
            await UniTask.Delay(100);
            await DailyRewardManager.Instance.DailyRewardPopup.Hide(true);
            //MainMenuRecieveRewardsHelper.Instance.ConvertDataFormDB();
        }
    }
    private void Reset()
    {
        for (int i = 0; i < dailyRewards.Count; i++)
        {
            dailyRewards[i].Reset();
        }
    }
    private void UpdateTime()
    {
        if (!TimeGetter.Instance.IsGettedTime || !Db.storage.Inited)
        {
            return;
        }
        txtStreak.text = $"Streak: {Db.storage.DAILY_REWARD_DATA.streak}";
        txtNextTimeAvailable.text = $"Next available: {System.DateTimeOffset.FromUnixTimeMilliseconds(Db.storage.DAILY_REWARD_DATA.nextAvailableTime)}";
        //txtNextTimeAvailable.text = $"Next available: {Db.storage.DAILY_REWARD_DATA.nextAvailableTime}";

        /*        if (canGetReward)
                {
                    CheckDayCanReceive();
                    txtNoti.text = "Your reward is available now !!!";
                    return;
                }*/

        var nextTick = Db.storage.DAILY_REWARD_DATA.nextAvailableTime - TimeGetter.Instance.CurrentTime;
        var nextTime = System.TimeSpan.FromMilliseconds(nextTick);
        if (nextTime.TotalSeconds <= 0)
        {
            //Debug.Log("Next available time is now or in the past, resetting data.");

            //txtNoti.text = "Your reward is available now !!!";
            txtButtonDaily.text = string.Empty;
            txtNoti.gameObject.SetActive(false);
            goButtonDaily.SetActive(true);
            if (Db.storage.DAILY_REWARD_DATA.streak == 7 || Db.storage.DAILY_REWARD_DATA.streak ==0 )
                Reset();
            InitData();

            CheckDayCanReceive();
        }
        else
        {
            goButtonDaily.SetActive(false);
            txtNoti.gameObject.SetActive(true);
            txtNoti.text = $"Reward unlocks in\n {nextTime.Hours}H {nextTime.Minutes}M {nextTime.Seconds}S";
            txtButtonDaily.text = $"{nextTime.Hours}H {nextTime.Minutes}M";
        }
    }
}
