using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserProfile
{
    public class TabFrame : MonoBehaviour, TabBase
    {
        private DataUserProfileManager _dataUserProfileManager;
        [SerializeField] private ItemFrameDataSO _itemFrameDataSO;
        private DBUserProfileController _db;
        private UserProfileManager userProfileManager;
        private NotiUnlock _notiUnlock;


        [SerializeField] private ItemFrameUser prbItemFrameUser;
        [SerializeField] private List<ItemFrameUser> lstItemAvater = new List<ItemFrameUser>();
        [SerializeField] private Transform tfmHolder;
        [SerializeField] private ItemFrameUser currentItem;
        [SerializeField] private PopupUserProfile popupUserProfile;

        [ContextMenu("Create")]
        private void Createitems()
        {
            RemoveOlditem();
            var data = _itemFrameDataSO.data;
            for (int i = 0; i < data.Count; i++)
            {
                ItemFrameUser itemFrameUser = Instantiate(prbItemFrameUser, tfmHolder);
                lstItemAvater.Add(itemFrameUser);
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
            _itemFrameDataSO = _dataUserProfileManager.ItemFrameDataSO;
            _db = DBUserProfileController.Instance;
            userProfileManager = UserProfileManager.Instance;
            _notiUnlock = NotiUnlock.Instance;

        }
        public void Init()
        {
            var data = _itemFrameDataSO.data;
            DBItemFrame itemFrames = _db.ITEM_FRAMES;
            var lstItem = itemFrames.data;
            for (int i = 0; i < lstItemAvater.Count; i++)
            {
                lstItemAvater[i].Init(i, data[i].sprite,data[i].color, lstItem[i].itemState);
                lstItemAvater[i].SetActionClick(OnClickItem);
            }
            currentItem = lstItemAvater[_db.USER_INFO_DATA.frameID];
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
                    _notiUnlock.ShowNoti(_itemFrameDataSO.data[itemUserProfileBase.Id].conditionUnlock.description);
                    break;
                case ItemState.Select:
                    break;

            }

        }
        private void UseItem(ItemUserProfileBase itemUserProfileBase)
        {
            var userInfo = _db.USER_INFO_DATA;
            DBItemFrame itemFrames = _db.ITEM_FRAMES;
            var lstItem = itemFrames.data;
            int currentId = userInfo.frameID;
            //Update old select -> unlock;
            lstItem[currentId].itemState = ItemState.Unlock;
            lstItemAvater[currentId].UpdateState(ItemState.Unlock);

            // Update new select
            userInfo.SetFrameID(itemUserProfileBase.Id);
            lstItem[itemUserProfileBase.Id].itemState = ItemState.Select;
            lstItemAvater[itemUserProfileBase.Id].UpdateState(ItemState.Select);
            currentItem = itemUserProfileBase as ItemFrameUser;
            _db.ITEM_FRAMES = itemFrames;
            TrackingController.Instance.TrackingEditProfile(EditProfileType.Border, $"{itemUserProfileBase.Id}");

            userProfileManager.UpdateUserUI();
        }

        public void UpdateFrameData(int frameId)
        {
            if (_db == null)
            {
                return;
            }

            UserInfoData userInfo = _db.USER_INFO_DATA;
            DBItemFrame itemFrames = _db.ITEM_FRAMES;

            itemFrames.data[userInfo.frameID].itemState = ItemState.Unlock;
            lstItemAvater[userInfo.frameID].UpdateState(ItemState.Unlock);

            itemFrames.data[frameId].itemState = ItemState.Select;
            lstItemAvater[frameId].UpdateState(ItemState.Select);

            //Save DB
            _db.ITEM_FRAMES = itemFrames;
            userInfo.SetFrameID(frameId);
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
