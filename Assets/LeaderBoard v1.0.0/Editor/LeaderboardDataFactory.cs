// Editor/LeaderboardDataFactory.cs
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ps.modules.leaderboard
{
#if UNITY_EDITOR
    public static class LeaderboardDataFactory
    {
        /// <summary>
        /// Tạo YearData mới trong thư mục: {BaseDir}/{year}/YearData_{year}.asset
        /// và add vào allYear.lstYearData. Trả về YearData tạo ra (hoặc null nếu fail).
        /// </summary>
        public static YearDataSO CreateYear(AllYearData allYear, int year)
        {
            if (allYear == null)
            {
                EditorUtility.DisplayDialog("Lỗi", "AllYearData null.", "OK");
                return null;
            }

            var baseDir = LeaderboardPathResolver.GetBaseDir(allYear);
            if (string.IsNullOrEmpty(baseDir))
            {
                EditorUtility.DisplayDialog("Lỗi", "Không xác định được thư mục gốc của AllYearData.", "OK");
                return null;
            }

            var yearDir = LeaderboardPathResolver.EnsureFolder(baseDir, year.ToString());
            var assetPath = LeaderboardPathResolver.Combine(yearDir, $"YearData_{year}.asset");

            if (File.Exists(assetPath))
            {
                EditorUtility.DisplayDialog("Tồn tại", $"YearData cho {year} đã tồn tại:\n{assetPath}", "OK");
                return AssetDatabase.LoadAssetAtPath<YearDataSO>(assetPath);
            }

            var yearData = ScriptableObject.CreateInstance<YearDataSO>();
            yearData.year = year;
            yearData.lstMonthData = new List<MonthDataSO>();

            AssetDatabase.CreateAsset(yearData, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Undo.RecordObject(allYear, "Add YearData");
            allYear.lstYearData ??= new List<YearDataSO>();
            allYear.lstYearData.Add(yearData);
            EditorUtility.SetDirty(allYear);
            AssetDatabase.SaveAssets();

            Debug.Log($"✅ CreateYear → {assetPath}");
            return yearData;
        }

        /// <summary>
        /// Tạo 12 MonthData + 12 LeaderboardData cho 1 YearData.
        /// Tạo vào: {YearDir}/Months/ + {YearDir}/Months/LeaderboardData/
        /// overwrite=false: nếu file tồn tại sẽ load dùng lại, không ghi đè.
        /// </summary>
        public static void CreateMonthsAndLeaderboards(YearDataSO yearData, bool overwrite = false)
        {
            if (yearData == null)
            {
                EditorUtility.DisplayDialog("Lỗi", "YearData null.", "OK");
                return;
            }

            var yearDir = LeaderboardPathResolver.GetYearDir(yearData);
            if (string.IsNullOrEmpty(yearDir))
            {
                EditorUtility.DisplayDialog("Lỗi", "Không xác định được thư mục YearData.", "OK");
                return;
            }

            var monthsDir = LeaderboardPathResolver.EnsureFolder(yearDir, "Months");
            var lbDir = LeaderboardPathResolver.EnsureFolder(monthsDir, "LeaderboardData");

            Undo.RecordObject(yearData, "Build Year Months");
            yearData.lstMonthData ??= new List<MonthDataSO>();
            yearData.lstMonthData.Clear();

            for (int m = 1; m <= 12; m++)
            {
                // --- LeaderboardDataSO ---
                var lbPath = LeaderboardPathResolver.Combine(lbDir, $"LeaderboardData_{m}.asset");
                LeaderboardDataSO lbAsset = null;

                if (!overwrite && File.Exists(lbPath))
                {
                    lbAsset = AssetDatabase.LoadAssetAtPath<LeaderboardDataSO>(lbPath);
                }
                else
                {
                    lbAsset = ScriptableObject.CreateInstance<LeaderboardDataSO>();
                    // (tuỳ ý) set default fields:
                    // lbAsset.id = $"leaderboard_{yearData.year}_{m:D2}";
                    // lbAsset.leaderboardType = LeaderboardType.Monthly;
                    AssetDatabase.CreateAsset(lbAsset, lbPath);
                }

                // --- MonthDataSO ---
                var monthPath = LeaderboardPathResolver.Combine(monthsDir, $"MonthData_{m}.asset");
                MonthDataSO monthAsset = null;

                if (!overwrite && File.Exists(monthPath))
                {
                    monthAsset = AssetDatabase.LoadAssetAtPath<MonthDataSO>(monthPath);
                }
                else
                {
                    monthAsset = ScriptableObject.CreateInstance<MonthDataSO>();
                    monthAsset.month = m;
                    monthAsset.data = lbAsset;
                    AssetDatabase.CreateAsset(monthAsset, monthPath);
                }

                // gán link
                monthAsset.data = lbAsset;
                yearData.lstMonthData.Add(monthAsset);
            }

            EditorUtility.SetDirty(yearData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"✅ CreateMonthsAndLeaderboards → Year {yearData.year}");
        }
    }
    #endif
}