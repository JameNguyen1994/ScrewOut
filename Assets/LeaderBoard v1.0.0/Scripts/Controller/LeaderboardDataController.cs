using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class LeaderboardDataController : LeaderBoardCtrBase
    {
        [Header("Default Scriptable Objects (Fallback)")]
        [SerializeField] private LeaderBoardDailyData dailyDataDefault;
        [SerializeField] private MonthDataSO monthlyDataDefault;
        [SerializeField] private YearDataSO yearDataDefault;

        [Header("Runtime Data (Loaded from JSON or SO Default)")]
        [SerializeField] private LeaderboardDataSO dailyData;
        [SerializeField] private MonthDataSO monthlyData;
        [SerializeField] private YearDataSO yearData;

        [SerializeField] private LDGenerateData settingsDayDefault;
        [SerializeField] private LDGenerateData settingsMonthDefault;

        // =====================================================================
        // INITIALIZATION
        // =====================================================================
        public override async UniTask Init()
        {
            await base.Init();
            Setup();
        }

        private void Setup()
        {
            Debug.Log("[LeaderboardDataController] Setup");

            var time = LeaderboardManager.Instance
                        .GetController<AdapterController>()
                        .TimeAdapter.GetCurrentTime();
            LeaderboardDefaultDataCreator_FromGenerator.RunFirstSetup(
      settingsDayDefault,      // LDGenerateData daily
      settingsMonthDefault,    // LDGenerateData monthly
      time.Year
  );


            int year = time.Year;
            int month = time.Month;

            // --------------------------
            // LOAD YEAR DATA
            // --------------------------
            yearData = LoadYearFromJson(year) ?? yearDataDefault;

            // --------------------------
            // LOAD MONTH DATA
            // --------------------------
            monthlyData = LoadMonthFromJson(year, month)
                          ?? yearData.GetLastMonthData()
                          ?? monthlyDataDefault;

            // --------------------------
            // LOAD DAILY DATA
            // --------------------------
            dailyData = LoadDailyFromJson()
                        ?? dailyDataDefault.GetRandomDailyData();
        }

        // =====================================================================
        // PUBLIC API
        // =====================================================================
        public LeaderboardDataSO GetDailyData() => dailyData;
        public MonthDataSO GetMonthlyData() => monthlyData;
        public YearDataSO GetYearlyData() => yearData;

        public void Reset()
        {
            Debug.Log("[LeaderboardDataController] Reset Daily Data");
            dailyData = dailyDataDefault.GetRandomDailyData();
        }

        // =====================================================================
        // LOAD DAILY (D30 → M → U)
        // =====================================================================
        private LeaderboardDataSO LoadDailyFromJson()
        {
            string path = LeaderboardDataService.GetDailyPath();
            Debug.Log($"[LeaderboardDataController] Load Daily JSON: {path}");

            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);
            D30 d30 = JsonUtility.FromJson<D30>(json);

            if (d30 == null || d30.m == null)
                return null;

            // d30.m là 1 object M (không phải danh sách)
            return LBJsonConverter.ConvertDaily(d30.m);
        }

        // =====================================================================
        // LOAD YEAR (Y → M → U)
        // =====================================================================
        private YearDataSO LoadYearFromJson(int year)
        {
            string path = LeaderboardDataService.GetYearPath(year);
            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);
            Y y = JsonUtility.FromJson<Y>(json);

            return y != null ? LBJsonConverter.ConvertYear(y) : null;
        }

        // =====================================================================
        // LOAD MONTH (từ Year JSON)
        // =====================================================================
        private MonthDataSO LoadMonthFromJson(int year, int month)
        {
            string path = LeaderboardDataService.GetYearPath(year);
            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);
            Y y = JsonUtility.FromJson<Y>(json);

            if (y == null || y.m == null)
                return null;

            M m = y.m.Find(x => x.m == month);
            if (m == null)
                return null;

            return LBJsonConverter.ConvertMonth(m);
        }

        // =====================================================================
        // YEAR BEFORE
        // =====================================================================
        public YearDataSO GetYearlyDataBefore()
        {
            var time = LeaderboardManager.Instance
                        .GetController<AdapterController>()
                        .TimeAdapter.GetCurrentTime();

            int prevYear = time.Year - 1;

            var yearBefore = LoadYearFromJson(prevYear);
            if (yearBefore != null)
                return yearBefore;

            Debug.LogWarning($"[Leaderboard] Không tìm thấy dữ liệu năm {prevYear}, dùng default.");
            return yearDataDefault;
        }
    }
}
