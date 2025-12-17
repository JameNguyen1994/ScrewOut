using TMPro;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class CheatLeaderBoard : MonoBehaviour
    {
        [SerializeField] private TMP_InputField txtCheatScore;

        public void OnClickCheat()
        {
            var manager = LeaderboardManager.Instance;
            var dataCtr = manager.GetController<PlayerDataManager>();
            var point = 0;
            try
            {
                point = int.Parse(txtCheatScore.text);
            }
            catch
            {
                Debug.LogError("Invalid cheat score input");
                return;
            }
            dataCtr.CheatPoint(point,point);
        }
    }
}