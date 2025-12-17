using DailyReward;
using UnityEngine;

namespace DailyReward
{
    public class DailyRewardManager : Singleton<DailyRewardManager>
    {
        [SerializeField] private DailyRewardController dailyRewardController;
        [SerializeField] private DailyRewardHelper dailyRewardHelper;
        [SerializeField] private DailyRewardPopup dailyRewardPopup;
        public DailyRewardController DailyRewardController => dailyRewardController;
        public DailyRewardHelper DailyRewardHelper => dailyRewardHelper;
        public DailyRewardPopup DailyRewardPopup => dailyRewardPopup;
    }
}