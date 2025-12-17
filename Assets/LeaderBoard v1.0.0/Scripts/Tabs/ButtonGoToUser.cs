using UnityEngine;

namespace ps.modules.leaderboard
{

    public class ButtonGoToUser : MonoBehaviour
    {
        public void OnClick()
        {
            LeaderboardManager.Instance.GetController<LeaderboardScrollController>().GotoPlayer();
        }
    }
}