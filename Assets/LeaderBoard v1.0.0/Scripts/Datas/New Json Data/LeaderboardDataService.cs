using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public static class LeaderboardDataService
    {
        // ==============================================================
        // ROOT FOLDER (Editor + Runtime dùng chung)
        // ==============================================================

        private static string Root =>
            Path.Combine(Application.persistentDataPath, "Leaderboard");

        public static void EnsureFolder()
        {
            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);
        }

        // ==============================================================
        // PATH HELPERS (Public for external use)
        // ==============================================================

        public static string GetYearPath(int year) =>
            Path.Combine(Root, $"year_{year}.json");

        public static string GetDailyPath() =>
            Path.Combine(Root, "daily30.json");

        // ==============================================================
        // Internal Path Helpers
        // ==============================================================

        private static string YearFile(int year) => GetYearPath(year);
        private static string Daily30File => GetDailyPath();

        // ==============================================================
        // LOAD YEAR (Auto-create if needed)
        // ==============================================================

        public static Y LoadYear(int year)
        {
            EnsureFolder();
            string path = YearFile(year);

            if (!File.Exists(path))
            {
                Debug.Log($"[Leaderboard] Year file missing → creating new: {path}");
                Y newYear = CreateEmptyYear(year);
                SaveYear(newYear);
                return newYear;
            }

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<Y>(json);
        }

        // ==============================================================
        // SAVE YEAR
        // ==============================================================

        public static void SaveYear(Y data)
        {
            EnsureFolder();
            string path = YearFile(data.y);

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            Debug.Log($"[Leaderboard] Saved Year → {path}");
        }

        // ==============================================================
        // LOAD DAILY30 (Auto-create empty)
        // ==============================================================

        public static D30 LoadDaily30()
        {
            EnsureFolder();
            string path = Daily30File;

            if (!File.Exists(path))
            {
                D30 data = new D30();
                SaveDaily30(data);
                return data;
            }

            return JsonUtility.FromJson<D30>(File.ReadAllText(path));
        }

        // ==============================================================
        // SAVE DAILY30
        // ==============================================================

        public static void SaveDaily30(D30 data)
        {
            EnsureFolder();
            string path = Daily30File;

            File.WriteAllText(path, JsonUtility.ToJson(data, true));

            Debug.Log($"[Leaderboard] Saved Daily30 → {path}");
        }

        // ==============================================================
        // CREATE EMPTY YEAR (12 months, no users)
        // ==============================================================

        private static Y CreateEmptyYear(int year)
        {
            var y = new Y();
            y.y = year;
            y.m = new List<M>();

            for (int i = 1; i <= 12; i++)
            {
                y.m.Add(new M
                {
                    m = i,
                    u = new List<U>()
                });
            }

            return y;
        }
    }
}
