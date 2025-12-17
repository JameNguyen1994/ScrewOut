using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    /// <summary>
    /// Dữ liệu đầy đủ cho 1 dòng leaderboard: chứa UserData + thông tin rank, điểm, thưởng, v.v.
    /// </summary>
    [Serializable]
    public class PlayerRankData
    {
        [Header("Core Info")]        // thông tin người chơi cơ bản
        public int dailyPoint;    // điểm trong tháng hiện tại
        public int monthlyPoint;    // điểm trong tháng hiện tại
        public List<RankLegendData> rankLegends; // lịch sử điểm theo tháng/năm

        public PlayerRankData(int dailyPoints, int monthPoints)
        {
            this.dailyPoint = dailyPoints;
            this.monthlyPoint = monthPoints;
            this.rankLegends = new List<RankLegendData>();
        }
        public UserData GetDayData()
        {
            return new UserData
            {
                points = dailyPoint,
            };
        }
        public UserData GetMonthData()
        {
            return new UserData
            {
                points = monthlyPoint,
            };
        }
        public UserData GetPointLegend(int year, int month)
        {
            var legend = rankLegends.Find(l => l.year == year && l.month == month);
            return new UserData
            {
                points = legend != null ? legend.points : -1,
            };
        }

    }
    [Serializable]
    public class RankLegendData
    {
        public int year;
        public int month;
        public int points;
    }
}
