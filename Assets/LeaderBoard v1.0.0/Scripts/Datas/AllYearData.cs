using System;
using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    [CreateAssetMenu(fileName = "AllYearData", menuName = "Scriptable Objects/AllYearData")]
    public class AllYearData : ScriptableObject
    {
        public List<YearDataSO> lstYearData;

        public YearDataSO GetYearData(int year)
        {
            var data = lstYearData.Find(y => y.year == year);
            if (data == null)
            {
                Debug.LogError($"YearData for year {year} not found!");
            }
            return lstYearData.Find(y => y.year == year);
        }
        public YearDataSO GetLastYearData()
        {
            if (lstYearData == null || lstYearData.Count == 0)
            {
                Debug.LogError("Year data list is empty or not initialized.");
                return null;
            }
            var time = LeaderboardManager.Instance.GetController<AdapterController>().TimeAdapter.GetCurrentTime();


            var year = lstYearData.Find(y => y.year == time.Year);
            if (year == null)
            {
                for (int i = 0; i < lstYearData.Count; i++)
                {
                    if (lstYearData[i].year < time.Year)
                    {
                        year = lstYearData[i];
                    }
                }
                Debug.LogWarning($"Current year data for {time.Year} not found. Returning the latest available year: {year.year}");
            }
            return year;
        }
        public YearDataSO GetLastYearDataBefore()
        {
            if (lstYearData == null || lstYearData.Count == 0)
            {
                Debug.LogError("Year data list is empty or not initialized.");
                return null;
            }
            var time = LeaderboardManager.Instance.GetController<AdapterController>().TimeAdapter.GetCurrentTime();


            var year = lstYearData.Find(y => y.year == time.Year-1);
            if (year == null)
            {
                for (int i = 0; i < lstYearData.Count; i++)
                {
                    if (lstYearData[i].year < time.Year)
                    {
                        year = lstYearData[i];
                    }
                }
                Debug.LogWarning($"Current year data for {time.Year} not found. Returning the latest available year: {year.year}");
            } else
            {
                Debug.Log($"Found year data for previous year: {year.year}");
            }
                return year;
        }
    }
}