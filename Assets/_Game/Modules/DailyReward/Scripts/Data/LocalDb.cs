using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Storage
{
    public partial class LocalDb
    {

        private DailyRewardData dailyRewardData;
        public DailyRewardData DAILY_REWARD_DATA
        {
            get => dailyRewardData;
            set
            {
                dailyRewardData = value;
                Debug.Log($"Setting DAILY_REWARD_DATA: Streak = {dailyRewardData.streak}, NextAvailableTime = {dailyRewardData.nextAvailableTime}");
                SetFromTData(DBKeyDailyReward.DAILY_REWARD_DATA, dailyRewardData);
            }
        }
        public async UniTask InitializeDailyReward()
        {
            if (!ObscuredPrefs.HasKey(DBKeyDailyReward.DAILY_REWARD_DATA))
            {
                var dailyRewardData = new DailyRewardData
                {
                    streak = 0,
                    nextAvailableTime = TimeGetter.Instance.CurrentTime
                };
                DAILY_REWARD_DATA = dailyRewardData; // Initialize with default values
            }
            dailyRewardData = GetFromJson<DailyRewardData>(DBKeyDailyReward.DAILY_REWARD_DATA);
        }
        public class DBKeyDailyReward
        {
            public static readonly string DAILY_REWARD_DATA = "DAILY_REWARD_DATA";

        }

        [System.Serializable]
        public class DailyRewardData
        {
            public int streak; // Current streak of consecutive days claimed
            public ObscuredLong nextAvailableTime; // Timestamp for the next available reward

            public void SetNextAvailableTime(long currentTime)
            {
                streak = 0; // Reset streak when setting a new next available time
                var day = System.DateTimeOffset.FromUnixTimeMilliseconds(currentTime).Date;
                var nextDay = day.AddDays(0); // Set to the next day
                nextAvailableTime = new System.DateTimeOffset(nextDay).ToUnixTimeMilliseconds();
                Db.storage.DAILY_REWARD_DATA = this; // Save the updated data
            }
            public void AddStreak()
            {
                streak++;
                if (streak >= 7) // Assuming a maximum streak of 7 days
                {
                    streak = 0; // Cap the streak at 7
                }
                nextAvailableTime += 1000 * 60 * 60 * 24; // Add 24 hours in milliseconds
                Db.storage.DAILY_REWARD_DATA = this; // Save the updated data
            }
            public void ResetStreak(long currentTime)
            {
                streak = 0;
                var day = System.DateTimeOffset.FromUnixTimeMilliseconds(currentTime).Date;
                var nextDay = day.AddDays(0); // Set to the next day
                nextAvailableTime = new System.DateTimeOffset(nextDay).ToUnixTimeMilliseconds();
                Db.storage.DAILY_REWARD_DATA = this; // Save the updated data
            }
        }
    }
}
