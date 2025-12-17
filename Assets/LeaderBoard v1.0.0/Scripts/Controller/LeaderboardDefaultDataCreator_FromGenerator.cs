using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public static class LeaderboardDefaultDataCreator_FromGenerator
    {
        private const string FIRST_KEY = "LEADERBOARD_DEFAULT_GENERATED";

        // ------------------------------------------------------
        // ENTRY POINT – gọi khi first install
        // ------------------------------------------------------
        public static void RunFirstSetup(
            LDGenerateData dayGenerator,
            LDGenerateData monthGenerator,
            int currentYear
        )
        {
            if (PlayerPrefs.HasKey(FIRST_KEY))
                return;

            Debug.Log("[Leaderboard] First app – creating default JSON via LDGenerateData");

            LeaderboardDataService.EnsureFolder();

            CreateDefaultDailyData(dayGenerator);
            CreateDefaultYearData(monthGenerator, currentYear);

            PlayerPrefs.SetInt(FIRST_KEY, 1);
            PlayerPrefs.Save();
        }

        // ------------------------------------------------------
        // DAILY30.JSON
        // ------------------------------------------------------
        private static void CreateDefaultDailyData(LDGenerateData gen)
        {
            if (gen == null)
            {
                Debug.LogError("[LeaderboardDefault] Missing daily generator!");
                return;
            }

            // Tạo object M
            M m = new M
            {
                m = 1,
                u = new List<U>()
            };

            gen.GenerateForJson(m);

            // Lưu vào D30
            D30 d30 = new D30();
            d30.m = m;

            LeaderboardDataService.SaveDaily30(d30);

            Debug.Log("[LeaderboardDefault] Created daily30.json");
        }

        // ------------------------------------------------------
        // YEAR_{YEAR}.JSON
        // ------------------------------------------------------
        private static void CreateDefaultYearData(LDGenerateData gen, int year)
        {
            if (gen == null)
            {
                Debug.LogError("[LeaderboardDefault] Missing month generator!");
                return;
            }

            Y y = new Y();
            y.y = year;
            y.m = new List<M>();

            for (int month = 1; month <= 12; month++)
            {
                M m = new M
                {
                    m = month,
                    u = new List<U>()
                };

                gen.GenerateForJson(m);

                y.m.Add(m);
            }

            LeaderboardDataService.SaveYear(y);

            Debug.Log($"[LeaderboardDefault] Created year_{year}.json (12 months by generator)");
        }
    }
}
