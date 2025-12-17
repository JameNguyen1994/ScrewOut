using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserProfile
{
    public class DataUserProfileManager : Singleton<DataUserProfileManager>
    {
        [SerializeField] private ItemAvatarDataSO itemAvatarDataSO;
        [SerializeField] private ItemFrameDataSO itemFrameDataSO;

        public ItemAvatarDataSO ItemAvatarDataSO { get => itemAvatarDataSO; }
        public ItemFrameDataSO ItemFrameDataSO { get => itemFrameDataSO; }
    }
}
