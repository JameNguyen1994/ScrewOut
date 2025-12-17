using Cysharp.Threading.Tasks;
using EasyButtons;
using ps.modules.leaderboard.reward;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

namespace ps.modules.leaderboard
{
    public class SaveTop3Data
    {
        public int playerTop;
        public DateTime time;
        public UserData[] top3Users;
    }

    public class PlayerDataManager : LeaderBoardCtrBase
    {
        private const string SAVE_KEY = "PLAYER_DATA_JSON";
        private const string SAVE_KEY_TOP3_DAILY = "PLAYER_TOP3_DAILY";
        private const string SAVE_KEY_TOP3_MONTHLY = "PLAYER_TOP3_MONTHLY";

        [Header("Current Player Data")]
        [SerializeField] private PlayerRankData currentUser;

        [Header("Last Save Time (Server Time)")]
        [SerializeField] private DateTime lastSaveTime;

        [Header("Reset Settings")]
        [SerializeField, Range(0, 23)] private int resetHour = 7; // 7h sáng

        [Header("Saved Top 3 Snapshots")]
        [SerializeField] private SaveTop3Data saveTop3Daily;
        [SerializeField] private SaveTop3Data saveTop3Monthly;

        public PlayerRankData CurrentUser => currentUser;

        // =====================================================
        // 🔹 INIT
        // =====================================================

        public override async UniTask Init()
        {
            await base.Init();
            Load();
            LoadTop3();
            // await CheckTime();
            // await CheatNextTime();

        }

        // =====================================================
        // 🔹 LOAD & SAVE PLAYER
        // =====================================================

        public void Load()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                currentUser = JsonUtility.FromJson<PlayerRankData>(json);

                if (PlayerPrefs.HasKey(SAVE_KEY + "_TIME"))
                {
                    long ticks = Convert.ToInt64(PlayerPrefs.GetString(SAVE_KEY + "_TIME"));
                    lastSaveTime = new DateTime(ticks);
                }
                else
                {
                    lastSaveTime = CurrentTime();
                }

                Debug.Log($"[PlayerDataManager] Loaded player data. LastSave: {lastSaveTime:yyyy-MM-dd HH:mm:ss}");
            }
            else
            {
                Debug.Log("[PlayerDataManager] No save data found → Creating default player.");
                CreateDefaultPlayer();
            }
        }

        [Button]
        public void Save()
        {
            string json = JsonUtility.ToJson(currentUser);
            PlayerPrefs.SetString(SAVE_KEY, json);

            // --- Lưu thời gian hiện tại ---
            lastSaveTime = CurrentTime();
            PlayerPrefs.SetString(SAVE_KEY + "_TIME", lastSaveTime.Ticks.ToString());
            PlayerPrefs.SetInt(SAVE_KEY + "_YEAR", lastSaveTime.Year);
            PlayerPrefs.SetInt(SAVE_KEY + "_MONTH", lastSaveTime.Month);
            PlayerPrefs.SetInt(SAVE_KEY + "_DAY", lastSaveTime.Day);

            PlayerPrefs.Save();

            Debug.Log($"[PlayerDataManager] Saved at {lastSaveTime:yyyy-MM-dd HH:mm:ss}");
        }

        private void CreateDefaultPlayer()
        {
            int dailyPoint = 0;
            int monthPoint = 0;

            currentUser = new PlayerRankData(dailyPoint, monthPoint);
            Save();
        }

        // =====================================================
        // 🔹 CHECK & RESET (SAU 7h SÁNG)
        // =====================================================

        public async UniTask CheckTime(bool Show = true)
        {
            var now = CurrentTime();

            if (lastSaveTime == default)
            {
                lastSaveTime = now;
                Save();
                return;
            }

            bool isNewMonth = IsNextMonth(lastSaveTime, now);
            bool isNewDay = IsNextDay(lastSaveTime, now);

            if (isNewMonth)
            {
                Debug.Log("[PlayerDataManager] → Detected new month (after 7:00)");
                await OnNextMonth(Show);
            }
            if (isNewDay)
            {
                Debug.Log("[PlayerDataManager] → Detected new day (after 7:00)");
                await OnNextDay(Show);
            }
            else
            {
                Debug.Log("[PlayerDataManager] → Same period, no reset.");
            }
            if (Show)
            {

                await ShowDailyTop3();
                await ShowMonthlyTop3();
            }
        }
        public async UniTask CheatNextTime()
        {
            await OnNextDay(false);
            await OnNextMonth(false);
        }
        public async UniTask OnNextDay(bool showTab)
        {
            Debug.Log("[PlayerDataManager] → New Day: save Top3 + reset dailyPoint");
            SaveTop3Daily();
            currentUser.dailyPoint = 0;
            Save();
            var data = manager.GetController<LeaderboardDataController>();
            data.Reset();
            var tabLeaderBoard = manager.GetController<TabController>();
            if (showTab)
            tabLeaderBoard.ShowTab();




        }
        public async UniTask ShowDailyTop3()
        {
            var congratulationCrtl = manager.GetController<CongratulationController>();

            await congratulationCrtl.ShowDailyData(saveTop3Daily);

            ResetTop3Daily();
        }
        public async UniTask ShowMonthlyTop3()
        {
            var congratulationCrtl = manager.GetController<CongratulationController>();

            await congratulationCrtl.ShowMonthlyData(saveTop3Monthly);
            ResetTop3Monthly();
        }
        public async UniTask OnNextMonth(bool showTab)
        {
            Debug.Log("[PlayerDataManager] → New Month: save Top3 + archive last month points");
            SaveTop3Monthly();

            var time = CurrentTime();
            int lastMonth = time.Month == 1 ? 12 : time.Month - 1;
            int lastYear = time.Month == 1 ? time.Year - 1 : time.Year;

            // Lưu lịch sử điểm tháng trước
            currentUser.rankLegends.Add(new RankLegendData
            {
                year = lastYear,
                month = lastMonth,
                points = currentUser.monthlyPoint
            });

            // Reset điểm tháng mới
            currentUser.monthlyPoint = 0;
            Save();


            var tabLeaderBoard = manager.GetController<TabController>();
            if (showTab)

            tabLeaderBoard.ShowTab();
            manager.GetController<TabController>().ResetTabLegend();

        }

        // =====================================================
        // 🔹 TIME HELPERS
        // =====================================================

        private DateTime CurrentTime()
        {
            return manager.GetController<AdapterController>().TimeAdapter.GetCurrentTime();
        }

        private bool IsNextDay(DateTime last, DateTime now)
        {
            var resetTime = new DateTime(last.Year, last.Month, last.Day, resetHour, 0, 0);
           if (last >= resetTime)
                resetTime = resetTime.AddDays(1);
            Debug.Log($"[PlayerDataManager] Reset Time Calculated: {CurrentTime():yyyy-MM-dd HH:mm:ss}");
            Debug.Log($"[PlayerDataManager] Checking Next Day: Last={last:yyyy-MM-dd HH:mm:ss}, Now={now:yyyy-MM-dd HH:mm:ss}, ResetTime={resetTime:yyyy-MM-dd HH:mm:ss}");
            return now >= resetTime;
        }

        private bool IsNextMonth(DateTime last, DateTime now)
        {
            int daysInLastMonth = DateTime.DaysInMonth(last.Year, last.Month);
            var monthReset = new DateTime(last.Year, last.Month, daysInLastMonth, resetHour, 0, 0);
            monthReset = monthReset.AddDays(1);
           if (last >= monthReset)
                monthReset = monthReset.AddDays(1);
            Debug.Log($"Month reset: {monthReset}");

            return now >= monthReset;
        }

        // =====================================================
        // 🔹 TOP 3 SAVE / LOAD / RESET
        // =====================================================

        public async UniTask SaveTop3Daily()
        {

            var leaderboardCtrl = manager.GetController<LeaderboardDataController>();
            //await UniTask.Delay()
            var lstUserData = leaderboardCtrl.GetDailyData().users;

            int index = GetIndexInsert(lstUserData, currentUser.dailyPoint);
            var time = CurrentTime();
            if (index <= 2)
            {
                var lstTop3 = new List<UserData>(lstUserData);
                if (lstTop3.Count < 3)
                    while (lstTop3.Count < 3)
                        lstTop3.Add(new UserData());

                lstTop3.Insert(index, currentUser.GetDayData());
                saveTop3Daily = new SaveTop3Data()
                {
                    playerTop = index + 1,
                    time = time,
                    top3Users = lstTop3.GetRange(0, 3).ToArray()
                };

                string json = JsonUtility.ToJson(saveTop3Monthly);
                PlayerPrefs.SetString(SAVE_KEY_TOP3_DAILY, json);
                PlayerPrefs.Save();

                Debug.Log($"[PlayerDataManager] Saved Top3 Daily → {json}");
            }
        }

        public void SaveTop3Monthly()
        {
            var leaderboardCtrl = manager.GetController<LeaderboardDataController>();
            var lstUserData = leaderboardCtrl.GetMonthlyData().data.users;

            int index = GetIndexInsert(lstUserData, currentUser.monthlyPoint);
            var time = CurrentTime();
            if (index <= 2)
            {
                var lstTop3 = new List<UserData>(lstUserData);
                if (lstTop3.Count < 3)
                    while (lstTop3.Count < 3)
                        lstTop3.Add(new UserData());

                lstTop3.Insert(index, currentUser.GetMonthData());
                saveTop3Monthly = new SaveTop3Data()
                {
                    playerTop = index + 1,
                    time = time,
                    top3Users = lstTop3.GetRange(0, 3).ToArray()
                };


                string json = JsonUtility.ToJson(saveTop3Monthly);
                PlayerPrefs.SetString(SAVE_KEY_TOP3_MONTHLY, json);
                PlayerPrefs.Save();

                Debug.Log($"[PlayerDataManager] Saved Top3 Monthly → {json}");
            }
        }

        public void LoadTop3()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY_TOP3_DAILY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY_TOP3_DAILY);
                saveTop3Daily = JsonUtility.FromJson<SaveTop3Data>(json);
                //  Debug.Log($"[PlayerDataManager] Loaded Top3 Daily ({saveTop3Daily.Length})");
            }

            if (PlayerPrefs.HasKey(SAVE_KEY_TOP3_MONTHLY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY_TOP3_MONTHLY);
                saveTop3Monthly = JsonUtility.FromJson<SaveTop3Data>(json);
                // Debug.Log($"[PlayerDataManager] Loaded Top3 Monthly ({saveTop3Monthly.Length})");
            }
        }

        [Button]
        public void ResetTop3Daily()
        {
            saveTop3Daily = new SaveTop3Data();
            PlayerPrefs.DeleteKey(SAVE_KEY_TOP3_DAILY);
            PlayerPrefs.Save();
            Debug.Log("[PlayerDataManager] Reset Top3 Daily completed.");
        }

        [Button]
        public void ResetTop3Monthly()
        {
            saveTop3Monthly = new SaveTop3Data();
            PlayerPrefs.DeleteKey(SAVE_KEY_TOP3_MONTHLY);
            PlayerPrefs.Save();
            Debug.Log("[PlayerDataManager] Reset Top3 Monthly completed.");
        }

        private int GetIndexInsert(List<UserData> lstUserData, int point)
        {
            int index = lstUserData.FindIndex(u => point > u.points);
            return index >= 0 ? index : lstUserData.Count;
        }

        // =====================================================
        // 🔹 DEBUG BUTTONS
        // =====================================================

#if UNITY_EDITOR
        [Button]
        private void ShowSaveInfo()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                int y = PlayerPrefs.GetInt(SAVE_KEY + "_YEAR", 0);
                int m = PlayerPrefs.GetInt(SAVE_KEY + "_MONTH", 0);
                int d = PlayerPrefs.GetInt(SAVE_KEY + "_DAY", 0);
                Debug.Log($"[PlayerDataManager] Last Saved on: {y:D4}-{m:D2}-{d:D2} at {resetHour:00}:00 reset rule");
            }
            else
            {
                Debug.Log("[PlayerDataManager] No save data found.");
            }
        }

        [Button]
        private void ForceNextDay()
        {
            lastSaveTime = lastSaveTime.AddDays(-1);
            CheckTime();
        }

        [Button]
        private void ForceNextMonth()
        {
            lastSaveTime = lastSaveTime.AddMonths(-1);
            CheckTime();
        }
#endif
        [Button]
        public void CheatPoint(int dayPoint, int monthPoint)
        {
            currentUser.dailyPoint += dayPoint;
            currentUser.monthlyPoint += dayPoint;
        }
        public void AddDailyPoint(int point)
        {
            currentUser.dailyPoint += point;
            Save();
        }
        public void AddMonthlyPoint(int point)
        {
            currentUser.monthlyPoint += point;
            Save();
        }
    }

    // =====================================================
    // 🔹 JSON HELPER for array serialization
    // =====================================================
}
