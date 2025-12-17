using NUnit.Framework;
using ps.modules.leaderboard;
using System.Collections.Generic;
using UnityEngine;


namespace ps.modules.leaderboard
{
    [CreateAssetMenu(fileName = "MonthDataSO", menuName = "Scriptable Objects/MonthDataSO")]
    public class MonthDataSO : ScriptableObject
    {
        public int month;
        public LeaderboardDataSO data;
    }
}

