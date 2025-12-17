using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public static class RuntimeLeaderboardDataFactory
    {
        // =============================
        // 🔹 PATH HELPERS
        // =============================
        private static string GetBaseDir()
        {
            string dir = Path.Combine(Application.persistentDataPath, "Leaderboard");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        private static string GetYearDir(int year)
        {
            string dir = Path.Combine(GetBaseDir(), year.ToString());
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        // =============================
        // 🔹 CREATE YEAR
        // =============================
        /// <summary>
        /// Tạo YearDataRuntime mới, thêm vào AllYearDataRuntime và lưu ra file JSON.
        /// Nếu đã tồn tại thì load lại.
        /// </summary>
        public static YearDataSO CreateYear(LDGenerateData lDGenerateData, AllYearData allTimeData, int year)
        {
            if (allTimeData == null)
            {
                Debug.LogError("❌ AllYearDataRuntime is null!");
                return null;
            }

            // Kiểm tra nếu năm đã tồn tại trong danh sách
            var exist = allTimeData.lstYearData.Find(y => y.year == year);
            if (exist != null)
            {
                Debug.Log($"⚠️ Year {year} đã tồn tại trong runtime data.");
                return exist;
            }

            // Tạo YearData mới
            var newYear = new YearDataSO
            {
                year = year,
                lstMonthData = new List<MonthDataSO>()
            };

            // Tạo thư mục riêng
            string yearDir = GetYearDir(year);

            // Lưu ra file JSON riêng cho năm này
            string path = Path.Combine(yearDir, $"YearData_{year}.json");
            string json = JsonUtility.ToJson(newYear, true);
            File.WriteAllText(path, json);

            // Thêm vào AllYearDataRuntime
            allTimeData.lstYearData.Add(newYear);

            // Lưu AllYearDataRuntime tổng
            string allPath = Path.Combine(Application.persistentDataPath, "AllYearDataRuntime.json");
            File.WriteAllText(allPath, JsonUtility.ToJson(allTimeData, true));

            Debug.Log($"✅ Created YearDataRuntime for year {year} at {path}");
            return newYear;
        }

        // =============================
        // 🔹 CREATE MONTHS + LEADERBOARD
        // =============================
        public static void CreateMonthsAndLeaderboards(LDGenerateData lDGenerateData, YearDataSO yearData, bool overwrite = false)
        {
            if (yearData == null)
            {
                Debug.LogError("❌ YearDataRuntime is null!");
                return;
            }

            string yearDir = GetYearDir(yearData.year);
            yearData.lstMonthData ??= new List<MonthDataSO>();
            yearData.lstMonthData.Clear();

            for (int m = 1; m <= 12; m++)
            {
                string monthPath = Path.Combine(yearDir, $"Month_{m}.json");

                MonthDataSO monthData = null;

                if (!overwrite && File.Exists(monthPath))
                {
                    string existing = File.ReadAllText(monthPath);
                    monthData = JsonUtility.FromJson<MonthDataSO>(existing);
                }
                else
                {
                    monthData = new MonthDataSO
                    {
                        month = m,
                        data = new LeaderboardDataSO
                        {
                        }
                    };
                    lDGenerateData.ApplyTo(monthData.data);
                    string json = JsonUtility.ToJson(monthData, true);
                    File.WriteAllText(monthPath, json);
                }
                yearData.lstMonthData.Add(monthData);
            }

            // Lưu YearDataRuntime sau khi thêm 12 tháng
            string path = Path.Combine(yearDir, $"YearData_{yearData.year}.json");
            File.WriteAllText(path, JsonUtility.ToJson(yearData, true));

            Debug.Log($"✅ Created 12 months for Year {yearData.year}");
        }
    }

    /* // =============================
     // 🔹 DATA STRUCTURES
     // =============================
     [Serializable]
     public class AllYearDataRuntime
     {
         public List<YearDataRuntime> lstYearData = new();
     }

     [Serializable]
     public class YearDataRuntime
     {
         public int year;
         public List<MonthDataRuntime> months;
     }

     [Serializable]
     public class MonthDataRuntime
     {
         public int month;
         public LeaderboardDataRuntime leaderboard;
     }

     [Serializable]
     public class LeaderboardDataRuntime
     {
         public string id;
         public List<UserDataRuntime> players;
     }

     [Serializable]
     public class UserDataRuntime
     {
         public string name;
         public int score;
         public int rank;
     }*/
}
