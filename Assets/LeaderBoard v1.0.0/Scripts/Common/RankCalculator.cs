using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public static class RankCalculator
    {
        public static int GetIndex(List<UserData> sortedList, UserData user)
        {

            for (int i = 0; i < sortedList.Count; i++)
            {
                if (sortedList[i].points < user.points)
                    return i;
            }
            return sortedList.Count;

        }
    }
}
