using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserProfile
{
    public class TabAvatar : MonoBehaviour, TabBase
    {
        private DataUserProfileManager _dataUserProfileManager;
        [SerializeField] private ItemAvatarDataSO _itemAvatarDataSO;
        private DBUserProfileController _db;
        private UserProfileManager userProfileManager;
        private NotiUnlock _notiUnlock;

        [SerializeField] private ItemAvatarUser prbItemAvatarUser;
        [SerializeField] private List<ItemAvatarUser> lstItemAvater = new List<ItemAvatarUser>();
        [SerializeField] private Transform tfmHolder;
        [SerializeField] private ItemAvatarUser currentItem;
        [SerializeField] private PopupUserProfile popupUserProfile;

        [ContextMenu("Create")]
        private void Createitems()
        {
            RemoveOlditem();
            var data = _itemAvatarDataSO.data;
            for (int i = 0; i < data.Count; i++)
            {
                ItemAvatarUser itemAvatarUser = Instantiate(prbItemAvatarUser, tfmHolder);
                lstItemAvater.Add(itemAvatarUser);
            }
        }
        private void RemoveOlditem()
        {
            for (int i = 0; i < lstItemAvater.Count; i++)
            {
                DestroyImmediate(lstItemAvater[i].gameObject);
            }
            lstItemAvater.Clear();
        }
        private void Start()
        {
            GetReference();
            Init();
        }
        public void GetReference()
        {
            _dataUserProfileManager = DataUserProfileManager.Instance;
            _itemAvatarDataSO = _dataUserProfileManager.ItemAvatarDataSO;
            _db = DBUserProfileController.Instance;
            userProfileManager = UserProfileManager.Instance;
            _notiUnlock = NotiUnlock.Instance;
        }
        public void Init()
        {
            var data = _itemAvatarDataSO.data;
            DBItemAvatar itemAvatars = _db.ITEM_AVATARS;
            var lstItem = itemAvatars.data;
            for (int i = 0; i < lstItemAvater.Count; i++)
            {
                lstItemAvater[i].Init(i, data[i].sprite, lstItem[i].itemState);
                lstItemAvater[i].SetActionClick(OnClickItem);
            }
            currentItem = lstItemAvater[_db.USER_INFO_DATA.avatarID];
        }
        public void OnClickItem(ItemUserProfileBase itemUserProfileBase)
        {
            //Debug.Log($"{itemUserProfileBase.name} {itemUserProfileBase.Id} {itemUserProfileBase.ItemState}");

            if (popupUserProfile != null)
            {
                popupUserProfile.ActiveSaveButton(true);
            }

            switch (itemUserProfileBase.ItemState)
            {
                case ItemState.Unlock:
                    UseItem(itemUserProfileBase);
                    break;
                case ItemState.Lock:
                    _notiUnlock.ShowNoti(_itemAvatarDataSO.data[itemUserProfileBase.Id].conditionUnlock.description);
                    break;
                case ItemState.Select:
                    break;

            }

        }
        private void UseItem(ItemUserProfileBase itemUserProfileBase)
        {
            var userInfo = _db.USER_INFO_DATA;
            DBItemAvatar itemAvatars = _db.ITEM_AVATARS;
            var lstItem = itemAvatars.data;
            int currentId = userInfo.avatarID;
            //Update old select -> unlock;
            lstItem[currentId].itemState = ItemState.Unlock;
            lstItemAvater[currentId].UpdateState(ItemState.Unlock);

            // Update new select
            userInfo.SetAvatarID(itemUserProfileBase.Id);
            lstItem[itemUserProfileBase.Id].itemState = ItemState.Select;
            lstItemAvater[itemUserProfileBase.Id].UpdateState(ItemState.Select);
            currentItem = itemUserProfileBase as ItemAvatarUser;
            _db.ITEM_AVATARS = itemAvatars;
            TrackingController.Instance.TrackingEditProfile(EditProfileType.Avatar,$"{itemUserProfileBase.Id}");

            userProfileManager.UpdateUserUI();
        }

        public void UpdateAvatar(int avatarId)
        {
            if (_db == null)
            {
                return;
            }

            UserInfoData userInfo = _db.USER_INFO_DATA;
            DBItemAvatar itemAvatars = _db.ITEM_AVATARS;

            itemAvatars.data[userInfo.avatarID].itemState = ItemState.Unlock;
            lstItemAvater[userInfo.avatarID].UpdateState(ItemState.Unlock);

            itemAvatars.data[avatarId].itemState = ItemState.Select;
            lstItemAvater[avatarId].UpdateState(ItemState.Select);

            userInfo.SetAvatarID(avatarId);
            _db.ITEM_AVATARS = itemAvatars;
        }

        public void OnCloseTab()
        {
            gameObject.SetActive(false);

        }

        public void OnOpenTab()
        {
            gameObject.SetActive(true);
        }
    }

}
