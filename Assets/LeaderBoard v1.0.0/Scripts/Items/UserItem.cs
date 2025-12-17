using Cysharp.Threading.Tasks;
using ps.modules.leaderboard;
using UnityEngine;

namespace ps.modules.leaderboard
{

    public class UserItem : MonoBehaviour
    {
        [SerializeField] private int id;
        [SerializeField] private bool isPlayer;
        [SerializeField] private UserData userData;

        [SerializeField] private UserItemUIBase userItemUI;
        [SerializeField] private RectTransform rectTransform;

        public UserItemUIBase UI => userItemUI;

        public int Id { get => id;}

        public void ResetPos()
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
        public void SetData(int id, UserData userData, bool isPlayerData = false)
        {
            this.id = id;
            this.userData = userData;
            isPlayer = isPlayerData;

            if (isPlayerData)
            {
                var manager =LeaderboardManager.Instance;
                var data = manager.GetController<AdapterController>().PlayerDataAdapter.GetPlayerData();
                userData.name = data.PlayerName;
                userData.avatar = data.SprAvatar;
                userData.border = data.SprBorder;

                //  userData.points = data.;
                // Debug.Log($"Set player data: {userData.name}, {userData.avatar}, {userData.border}");
                Debug.Log($"Set Data User Item: {userData.name} - Points: {userData.points}");

            }
            if (userItemUI != null)
            {
                userItemUI.SetData(id, userData.name, userData.points, userData.avatar, userData.border, isPlayerData);
            }
            else
            {
              //  Debug.LogWarning("UserItemUIBase reference is missing.");
            }
        }

        public RectTransform GetRectTransformStar()
        {
            return userItemUI.GetRectTransformStar();
        }
        public RectTransform GetRectTransform()
        {
            return rectTransform;
        }
    }
    /// <summary>
    /// Item hiển thị thông tin người dùng trong bảng xếp hạng
    /// </summary>
}