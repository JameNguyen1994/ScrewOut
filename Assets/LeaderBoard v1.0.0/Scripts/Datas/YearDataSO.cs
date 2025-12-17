using NUnit.Framework;
using ps.modules.leaderboard;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace ps.modules.leaderboard
{
    [CreateAssetMenu(fileName = "YearDataSO", menuName = "Scriptable Objects/YearDataSO")]
    public class YearDataSO : ScriptableObject
    {
        public int year;
        public List<MonthDataSO> lstMonthData;

        public MonthDataSO GetMonthData(int month)
        {
            if (month < 1 || month > 12)
            {
                Debug.LogError($"Invalid month: {month}. Month should be between 1 and 12.");
                return null;
            }
            if (lstMonthData == null || lstMonthData.Count < 12)
            {
                Debug.LogError($"Month data is not properly initialized for year {year}.");
                return null;
            }
            return lstMonthData[month - 1];
        }
        public MonthDataSO GetLastMonthData()
        {
            if (lstMonthData == null || lstMonthData.Count < 12)
            {
                Debug.LogError($"Month data is not properly initialized for year {year}.");
                return null;
            }
            var time = LeaderboardManager.Instance.GetController<AdapterController>().TimeAdapter.GetCurrentTime();
            Debug.Log($"Get month with current time {time.Month}");

            var monthData = lstMonthData.Find(m => m.month == time.Month);
            return monthData;
        }

    }
}