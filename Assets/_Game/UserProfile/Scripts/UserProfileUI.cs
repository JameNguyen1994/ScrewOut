using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UserProfile
{
    public class UserProfileUI : MonoBehaviour
    {
        private DataUserProfileManager _dataUserProfileManager;
        private DBUserProfileController _db;

        [SerializeField] private Text txtName;
        [SerializeField] private Image imgAvatar;
        [SerializeField] private Image imgFrame;
        [SerializeField] private TabAvatar tabAvatar;
        [SerializeField] private TabFrame tabFrame;

        [SerializeField] private Image imgBtnTabAvatar;
        [SerializeField] private Image imgBtnTabFrame;
        [SerializeField] private Text txtBtnTabAvatar;
        [SerializeField] private Text txtBtnTabFrame;
        [SerializeField] private Sprite sprAvatarOn;
        [SerializeField] private Sprite sprAvatarOff;
        [SerializeField] private Sprite sprFrameOn;
        [SerializeField] private Sprite sprFrameOff;
        //[SerializeField] private Color colorCurrentTab;
        //[SerializeField] private Color colorAnotherTab;

        //[SerializeField] private Outline outlineBtnTabAvatar;
        //[SerializeField] private Outline outlineBtnTabFrame;
        [SerializeField] private UserProfileTabName userProfileTabName;

        public void Init()
        {
            GetReference();
            OnClickTabAvatar();
            UpdateUserUI();
        }
        private void GetReference()
        {
            _dataUserProfileManager = DataUserProfileManager.Instance;
            _db = DBUserProfileController.Instance;
        }
        public void UpdateUserUI()
        {
            EventDispatcher.Push(EventId.UpdateUIAvatar);
            var userData = _db.USER_INFO_DATA;
            txtName.text = userData.userName;

            ItemAvatarDataSO _itemAvatarDataSO = _dataUserProfileManager.ItemAvatarDataSO;
            var avatarId = userData.avatarID;
            Sprite spriteAvatar = _itemAvatarDataSO.GetItemAvatarDataByID(avatarId).sprite;
            imgAvatar.sprite = spriteAvatar;

            ItemFrameDataSO _itemframeDataSO = _dataUserProfileManager.ItemFrameDataSO;
            var frameId = userData.frameID;
            Sprite spriteframe = _itemframeDataSO.GetItemFrameDataByID(frameId).sprite;
            imgFrame.sprite = spriteframe;
        }
        public void OnClickTabAvatar()
        {
            /*if (userProfileTabName == UserProfileTabName.Avatar)
                return;*/
            userProfileTabName = UserProfileTabName.Avatar;
            tabAvatar.OnOpenTab();
            tabFrame.OnCloseTab();
            imgBtnTabAvatar.sprite = sprAvatarOn;
            imgBtnTabFrame.sprite = sprFrameOff;

            //outlineBtnTabAvatar.effectColor = colorCurrentTab;
            //outlineBtnTabFrame.effectColor = colorAnotherTab;

         /*   outlineBtnTabAvatar.enabled = true;
            outlineBtnTabFrame.enabled = false;*/

        }
        public void OnClickTabFrame()
        {
            if (userProfileTabName == UserProfileTabName.Frame)
                return;
            userProfileTabName = UserProfileTabName.Frame;
            tabFrame.OnOpenTab();
            tabAvatar.OnCloseTab();
            imgBtnTabAvatar.sprite = sprAvatarOff;
            imgBtnTabFrame.sprite = sprFrameOn;
            //   txtBtnTabAvatar.color = colorAnotherTab;
            //    txtBtnTabFrame.color = colorCurrentTab;

            //outlineBtnTabAvatar.effectColor = colorAnotherTab;
            //outlineBtnTabFrame.effectColor = colorCurrentTab;
        }
    }
    public enum UserProfileTabName
    {
        Avatar = 0,
        Frame = 1
    }
}