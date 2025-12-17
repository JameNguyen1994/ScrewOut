using Cysharp.Threading.Tasks;
using UnityEngine;



namespace ps.modules.leaderboard
{
    public class PlayerData
    {
        public Sprite SprAvatar;
        public Sprite SprBorder;
        public string PlayerName;
        public PlayerData(Sprite avatar, Sprite border, string name)
        {
            SprAvatar = avatar;
            SprBorder = border;
            PlayerName = name;
        }
    }
    public interface IPlayerDataAdapter
    {
        public UniTask Init();
        public PlayerData GetPlayerData();
    }
}
