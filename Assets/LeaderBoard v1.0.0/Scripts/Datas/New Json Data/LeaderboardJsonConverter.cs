using ps.modules.leaderboard;
using System.Collections.Generic;
using UnityEngine;

public static class LBJsonConverter
{
    public static LeaderboardDataSO ConvertDaily(M m)
    {
        var so = ScriptableObject.CreateInstance<LeaderboardDataSO>();
        so.users = new List<UserData>();
        var avatarController = LeaderboardManager.Instance.GetController<AvatarBorderProvider>();
        if (m?.u != null)
        {
            foreach (var u in m.u)
            {
                var avatar = avatarController.GetAvatar(u.a);
                var border = avatarController.GetBorder(u.b);

                so.users.Add(new UserData
                {
                    name = u.n,
                    points = u.p,
                    avatar = avatar,
                    border = border
                });
            }
        }
        return so;
    }

    public static MonthDataSO ConvertMonth(M m)
    {
        var so = ScriptableObject.CreateInstance<MonthDataSO>();
        so.month = m.m;
        so.data = new LeaderboardDataSO();
        so.data.users = new List<UserData>();
        var avatarController = LeaderboardManager.Instance.GetController<AvatarBorderProvider>();

        if (m?.u != null)
        {
            foreach (var u in m.u)
            {
                var avatar = avatarController.GetAvatar(u.a);
                var border = avatarController.GetBorder(u.b);
                so.data.users.Add(new UserData
                {
                    name = u.n,
                    points = u.p,
                    avatar = avatar,
                    border = border
                });
            }
        }
        return so;
    }

    public static YearDataSO ConvertYear(Y y)
    {
        var so = ScriptableObject.CreateInstance<YearDataSO>();
        so.year = y.y;
        so.lstMonthData = new List<MonthDataSO>();

        if (y?.m != null)
        {
            foreach (var m in y.m)
                so.lstMonthData.Add(ConvertMonth(m));
        }
        return so;
    }
}
