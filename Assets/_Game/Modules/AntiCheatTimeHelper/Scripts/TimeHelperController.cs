using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Storage;
using System;
using UnityEngine;
using WeeklyQuest;
using static Storage.LocalDb;

public class TimeHelperController : Singleton<TimeHelperController>
{
    [SerializeField] private bool isInitedTime = false; // Flag to check if the time has been initialized
    public string firstTimeActiveString = "2023-01-01T00:00:00Z"; // Example initial value, can be changed
    public DateTime firstTimeActiveDateTime;
    public string firstTimeOfWeekString = "2023-01-01T00:00:00Z"; // Example initial value, can be changed
    public DateTime firstTimeOfWeekDateTime;
    public string lastTimeOfWeekString = "2023-01-01T00:00:00Z"; // Example initial value, can be changed
    public DateTime lastTimeOfWeekDateTime;
    public WeekInfo weekInfo;

    
    public bool IsInitedTime { get => isInitedTime; }

    private void Start()
    {
        //  gobjCheat.SetActive(Debug.unityLogger.logEnabled); // Hide cheat button by default
        Init().Forget();
        TimeGetter.Instance.RegisActionOnUpdateTime(CheckTime); // Subscribe to time updates
    }
    public async UniTask Init()
    {
        isInitedTime = false;
        await Db.storage.InitializeDataTime();
        await CheckNextWeek();
        isInitedTime = true;
        DebugValue();

    }
    public async UniTask CheckNextWeek()
    {
        if (!Db.storage.Inited)
        {
            Debug.LogError("LocalDb is not initialized. Please call InitializeDataTime first.");
            return;
        }
        CheckTime();
    }
    public void CheckTime()
    {
        var weekInfo = Db.storage.WEEK_INFO;
        // Check if the current time is past the last time of the week
        long currentTime = TimeGetter.Instance.CurrentTime;
        long lastTimeOfWeek = weekInfo.lastTimeOfWeek;
        if (currentTime > lastTimeOfWeek)
        {
            // Reset the week data
            Db.storage.WEEK_INFO.NextWeekData();
            var data = WeeklyQuestManager.Instance.WeeklyDataHelper.GetWeeklyData();
            data = (WeeklyData)data.Clone();
            Db.storage.LoadWeeklyQuestData(data, true);
            DebugValue();
            WeeklyQuestManager.Instance.WeeklyQuestController.DestroyOldItem();
            WeeklyQuestManager.Instance.WeeklyQuestController.Init();
           // WeeklyQuest.WeeklyQuestManager.Instance.WeeklyQuestController.InitGifts();

            Debug.Log("Week data has been reset.");
        }
        else
        {
            //  Debug.Log("Current time is within the same week. No reset needed.");
        }
    }
    public void DebugValue()
    {
        var weekInfo = Db.storage.WEEK_INFO;
        var firstTimeActiveTime = System.DateTimeOffset.FromUnixTimeMilliseconds(weekInfo.firstTimeActive);
        var firstTimeOfWeekTime = System.DateTimeOffset.FromUnixTimeMilliseconds(weekInfo.firstTimeOfWeek);
        var lastTimeOfWeekTime = System.DateTimeOffset.FromUnixTimeMilliseconds(weekInfo.lastTimeOfWeek);

        firstTimeActiveString = firstTimeActiveTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
        firstTimeOfWeekString = firstTimeOfWeekTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
        lastTimeOfWeekString = lastTimeOfWeekTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

    }

    public long GetTimeToEndWeek()
    {
        var weekInfo = Db.storage.WEEK_INFO;
        long currentTime = TimeGetter.Instance.CurrentTime;
        long lastTimeOfWeek = weekInfo.lastTimeOfWeek;
        return lastTimeOfWeek - currentTime; // Returns the time left until the end of the week in milliseconds
    }

}
