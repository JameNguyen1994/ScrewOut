using Life;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserProfile
{
    public class UserProfileManager : Singleton<UserProfileManager>
    {
        [SerializeField] private UserProfileUI userProfileUI;
        [SerializeField] private UserNameHandler userNameHandler;
        [SerializeField] private PopupUserProfile popupUserProfile;
        [SerializeField] private LifeController lifeController;
        [SerializeField] RectTransform rtfmProfileBar;
        public UserProfileUI UserProfileUI { get => userProfileUI; }
        public LifeController LifeController => lifeController;
        private void Start()
        {
            Init();
        }
        public string GetUserName()
        {
            return DBUserProfileController.Instance.USER_INFO_DATA.userName;
        }
        public Sprite GetSpriteAvatar()
        {
            var avatarId = DBUserProfileController.Instance.USER_INFO_DATA.avatarID;
            return DataUserProfileManager.Instance.ItemAvatarDataSO.GetItemAvatarDataByID(avatarId).sprite;
        }
        public Sprite GetSpriteFrame()
        {
            var frameId = DBUserProfileController.Instance.USER_INFO_DATA.frameID;
            return DataUserProfileManager.Instance.ItemFrameDataSO.GetItemFrameDataByID(frameId).sprite;
        }
        private void Init()
        {
            userNameHandler.Init();
            userProfileUI.Init();
        }
      
        public void UpdateUserUI()
        {
            userProfileUI.UpdateUserUI();
        }
        public void OnShowPopupUserProfileClick()
        {
            if (MainMenuRecieveRewardsHelper.Instance != null && MainMenuRecieveRewardsHelper.Instance.IsShowReward)
            {
                return;
            }

            AudioController.Instance.PlaySound(SoundName.Click);
            popupUserProfile.Show();
        }

        public void OnShowProfileBar(bool isShow)
        {
            //lifeController.UpdateUILife();
            rtfmProfileBar.gameObject.SetActive(isShow);
            if (!isShow)
            {
                //setting.HideGroupBtn();
            }

        }
    }
}
