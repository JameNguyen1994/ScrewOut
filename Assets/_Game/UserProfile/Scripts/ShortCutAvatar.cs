using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UserProfile
{
    public class ShortCutAvatar : MonoBehaviour
    {
        private DataUserProfileManager _dataUserProfileManager;
        private DBUserProfileController _db;
        [SerializeField] private Image imgAvatar;
        [SerializeField] private Image imgFrame;
        private void Start()
        {
            GetReference();
            UpdateUI(null);
        }
        private void GetReference()
        {
            _dataUserProfileManager = DataUserProfileManager.Instance;
            _db = DBUserProfileController.Instance;
        }
        private void OnEnable()
        {
            EventDispatcher.Register(EventId.UpdateUIAvatar, UpdateUI);
        }
        private void OnDestroy()
        {
            EventDispatcher.RemoveCallback(EventId.UpdateUIAvatar, UpdateUI);

        }
        public void UpdateUI(object dontCare)
        {
            GetReference();
            var userData = _db.USER_INFO_DATA;
            ItemAvatarDataSO _itemAvatarDataSO = _dataUserProfileManager.ItemAvatarDataSO;
            var avatarId = userData.avatarID;
            Sprite spriteAvatar = _itemAvatarDataSO.GetItemAvatarDataByID(avatarId).sprite;
            imgAvatar.sprite = spriteAvatar;

            ItemFrameDataSO _itemframeDataSO = _dataUserProfileManager.ItemFrameDataSO;
            var frameId = userData.frameID;
            Debug.Log("frameId: " + frameId);
            Sprite spriteframe = _itemframeDataSO.GetItemFrameDataByID(frameId).sprite;
            imgFrame.sprite = spriteframe;
        }
    }
}
