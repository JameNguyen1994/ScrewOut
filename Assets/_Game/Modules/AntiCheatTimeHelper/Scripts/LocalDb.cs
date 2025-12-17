using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Storage.Model;
using UnityEngine;

namespace Storage
{
    /// <summary>
    /// LocalDb is a partial class that handles local database operations.
    /// AntiCheatTimeHelper module uses this class to manage user data and preferences.
    /// </summary>
    public partial class LocalDb
    {
        private WeekInfo weekInfo;
        public WeekInfo WEEK_INFO
        {
            get
            {
                DateTime dt = new DateTime(3000, 1, 1);
                return ObscuredPrefs.Get(DbKey.H_W_I, "") != weekInfo.GetH() ? new WeekInfo(dt) : weekInfo;
            }
            set
            {
                DateTime dt = new DateTime(3000, 1, 1);
                weekInfo = ObscuredPrefs.Get(DbKey.H_W_I, "") != weekInfo.GetH() ? new WeekInfo(dt) : value;
                SetFromTData(DBKey.WEEK_INFO, weekInfo);
                ObscuredPrefs.Set(DbKey.H_W_I, weekInfo.GetH());

            }
        }
        private bool inited = false;
        public bool Inited => inited;
        public async UniTask InitializeDataTime()
        {
            inited = false;
            if (TimeGetter.Instance == null)
            {
                Debug.LogError("TimeGetter.Instance is null. Ensure that TimeGetter is initialized before LocalDb.");
                inited = true;
                return;
            }
            await UniTask.WaitUntil(() => TimeGetter.Instance.IsGettedTime);

            // Ensure that the database keys are set up correctly
            if (!ObscuredPrefs.HasKey(DBKey.WEEK_INFO))
            {
                var weekInfo = new WeekInfo(TimeGetter.Instance.CurrentTime);
                ObscuredPrefs.Set(DbKey.H_W_I, weekInfo.GetH());
                this.weekInfo = weekInfo;
                WEEK_INFO = weekInfo;
            }

            weekInfo = GetFromJson<WeekInfo>(DBKey.WEEK_INFO);

            if (!ObscuredPrefs.HasKey(DbKey.H_W_I))
            {
                SetFromTData(DbKey.H_W_I, weekInfo.GetH());
            }

            inited = true;
        }




        public static class DBKey
        {
            public static readonly string WEEK_INFO = "WEEK_INFO";
        }
        [System.Serializable]
        public class WeekInfo
        {
            public ObscuredLong firstTimeActive;
            public ObscuredLong firstTimeActiveDay;
            public ObscuredLong firstTimeOfWeek;
            public ObscuredLong lastTimeOfWeek;
            public ObscuredLong numOfWeek;
            public ObscuredLong cheatTime;
            public WeekInfo()
            {
            }

            public WeekInfo(DateTime dt)
            {
                long firstTimeActive = new DateTimeOffset(dt).ToUnixTimeMilliseconds();
                this.firstTimeActive = firstTimeActive;

                var firstActiveDate = System.DateTimeOffset.FromUnixTimeMilliseconds(firstTimeActive).Date;
                var firstDayOfWeek = firstActiveDate.AddDays(-(int)firstActiveDate.DayOfWeek + 1);
                var firstTimeOfWeek = new System.DateTimeOffset(firstDayOfWeek).ToUnixTimeMilliseconds() + 1;
                var lastTimeOfWeek = firstTimeOfWeek + 604800 * 1000 - 1; // 604800 seconds in a week  = 7*24*60*60

                this.firstTimeActiveDay = firstTimeActive; // Assuming this is the same as firstTimeActive
                this.firstTimeOfWeek = firstTimeOfWeek;
                this.lastTimeOfWeek = lastTimeOfWeek;
                this.numOfWeek = 1;
            }

            public WeekInfo(long firstTimeActive)
            {
                this.firstTimeActive = firstTimeActive;

                var firstActiveDate = System.DateTimeOffset.FromUnixTimeMilliseconds(firstTimeActive).Date;
                var firstDayOfWeek = firstActiveDate.AddDays(-(int)firstActiveDate.DayOfWeek + 1);
                var firstTimeOfWeek = new System.DateTimeOffset(firstDayOfWeek).ToUnixTimeMilliseconds() + 1;
                var lastTimeOfWeek = firstTimeOfWeek + 604800 * 1000 - 1; // 604800 seconds in a week  = 7*24*60*60

                this.firstTimeActiveDay = firstTimeActive; // Assuming this is the same as firstTimeActive
                this.firstTimeOfWeek = firstTimeOfWeek;
                this.lastTimeOfWeek = lastTimeOfWeek;
                this.numOfWeek = 1;
                // Get the first day of the week (Sunday)
            }
            public void NextWeekData()
            {
                var newWeekInfo = new WeekInfo();
                newWeekInfo.firstTimeActive = firstTimeOfWeek + 604800 * 1000; // 604800 seconds in a week = 7*24*60*60
                newWeekInfo.firstTimeActiveDay = firstTimeOfWeek + 604800 * 1000; // 604800 seconds in a week = 7*24*60*60
                newWeekInfo.firstTimeOfWeek = firstTimeOfWeek + 604800 * 1000; // 604800 seconds in a week = 7*24*60*60
                newWeekInfo.lastTimeOfWeek = newWeekInfo.firstTimeOfWeek + 604800 * 1000; // 604800 seconds in a week = 7*24*60*60
                newWeekInfo.numOfWeek = numOfWeek + 1;
                newWeekInfo.cheatTime = cheatTime;
                Db.storage.WEEK_INFO = newWeekInfo;
            }
            public void CheatTime(long cheatTime)
            {
                this.cheatTime += cheatTime;
                Db.storage.WEEK_INFO = this;
            }

            [System.Reflection.Obfuscation(Exclude = false)]
            public string GetH()
            {
                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    { "firstTimeActive", firstTimeActive.GetDecrypted() },
                    { "firstTimeActiveDay", firstTimeActiveDay.GetDecrypted() },
                    { "firstTimeOfWeek", firstTimeOfWeek.GetDecrypted() },
                    { "lastTimeOfWeek", lastTimeOfWeek.GetDecrypted() },
                    { "numOfWeek", numOfWeek.GetDecrypted() },
                };

                var json = JsonConvert.SerializeObject(data, Formatting.None);
                using var sha256 = SHA256.Create();
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

    }
}
