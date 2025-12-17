using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ps.modules.leaderboard
{
    public class UserItemListUI : UserItemUIBase
    {
        [SerializeField] private Image imgBGItem;
        [SerializeField] private Sprite sprPlayer;
        [SerializeField] private Sprite sprNormal;
                        

        public override void SetData(int index, string userName, int points, Sprite avatarSprite, Sprite borderSprite, bool isPlayerData =false)
        {
            txtIndex.text = $"{index+1}";
            txtName.text = FormatName.Format(userName);
            txtPoint.text = points.ToString("N0");
            if (isPlayerData)
            Debug.Log($"Set Data User Item List UI: {txtName.text} - Points:{points} - {txtPoint.text}");
            avatar.sprite = avatarSprite;
            border.sprite = borderSprite;

            imgBGItem.sprite = isPlayerData ? sprPlayer : sprNormal;
        }
    }
}
