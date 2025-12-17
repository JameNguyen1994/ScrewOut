using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserProfile
{
    [System.Serializable]
    public class UserInfoData
    {
        public string userID;
        public string userName;
        public int avatarID;
        public int frameID;

        public UserInfoData(string userID, string userName, int avatarID, int frameID)
        {
            this.userID = userID;
            this.userName = userName;
            this.avatarID = avatarID;
            this.frameID = frameID;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
            SaveToDB();
        }
        public void SetAvatarID(int avatarID)
        {
            this.avatarID = avatarID;
            SaveToDB();

        }
        public void SetFrameID(int frameID)
        {
            this.frameID = frameID;
            SaveToDB();
        }
        private void SaveToDB()
        {
            DBUserProfileController.Instance.USER_INFO_DATA = this;

        }
    }
}
