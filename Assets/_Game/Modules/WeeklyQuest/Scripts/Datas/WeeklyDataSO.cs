using System.Collections.Generic;
using UnityEngine;


namespace WeeklyQuest
{
    [CreateAssetMenu(fileName = "WeeklyDataSO", menuName = "Scriptable Objects/WeeklyQuest/WeeklyDataSO")]
    public class WeeklyDataSO : ScriptableObject
    {
        public List<WeeklyData> datas;
        public WeeklyData GetWeeklyData(int weekIndex)
        {
            if (weekIndex > 0)
            {
                weekIndex = Random.Range(1, 3 + 1);
            }
            if (weekIndex < 0 || weekIndex >= datas.Count)
            {
                Debug.LogError($"Week id {weekIndex} is out of range. Returning null.");
                return datas[0]; // or throw an exception, or return a default value
            }
            return datas[weekIndex];
        }
    }
}
