using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace ps.modules.leaderboard
{
    public class DefaultPlayerDataAdapter : MonoBehaviour, IPlayerDataAdapter
    {
        [SerializeField] private Sprite sprAvatar;
        [SerializeField] private Sprite sprBorder;
        [SerializeField] private string playerName = "You";

        public async UniTask Init()
        {


        }
        public PlayerData GetPlayerData()
        {
            return new PlayerData(sprAvatar, sprBorder, playerName);
        }
    }
}
