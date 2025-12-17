using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace DailyReward
{
    [CreateAssetMenu(fileName = "WeeklyDataSO", menuName = "Scriptable Objects/DailyReward/WeeklyDataSO")]
    public class WeeklyDataSO : ScriptableObject
    {
        public List<DayData> days = new List<DayData>();
        public DayData GetData(int dayIndex)
        {
            if (dayIndex < 0 || dayIndex >= days.Count)
            {
                Debug.LogError($"Day {dayIndex} is out of range. Valid range is 0 to {days.Count - 1}.");
                return null; // or throw an exception
            }
            return days[dayIndex];
        }
    }

    [System.Serializable]
    public class DayData
    {
        public List<ResourceValue> resources = new List<ResourceValue>();
    }
}

