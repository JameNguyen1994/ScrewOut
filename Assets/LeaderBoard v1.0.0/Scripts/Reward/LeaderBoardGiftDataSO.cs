using ps.modules.leaderboard;
using UnityEngine;

namespace ps.modules.leaderboard
{
    [CreateAssetMenu(fileName = "LeaderBoardGiftDataSO", menuName = "Scriptable Objects/Reward/LeaderBoardGiftDataSO")]
    public class LeaderBoardGiftDataSO : ScriptableObject
    {
        public GiftDataSO giftDay;
        public GiftDataSO giftMonth;
    }
}
