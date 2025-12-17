using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ps.modules.leaderboard
{

    public class PlayerProfileDataAdapter :MonoBehaviour, IPlayerDataAdapter
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite avatar;
        [SerializeField] private Sprite frame;
        public PlayerData GetPlayerData()
        {
            return new PlayerData(avatar, frame, name);
        }

        public async UniTask Init()
        {
            if (UserProfile.UserProfileManager.Instance != null)
            {
                name = UserProfile.UserProfileManager.Instance.GetUserName();
                avatar = UserProfile.UserProfileManager.Instance.GetSpriteAvatar();
                frame = UserProfile.UserProfileManager.Instance.GetSpriteFrame();
            }

        }
    }

}