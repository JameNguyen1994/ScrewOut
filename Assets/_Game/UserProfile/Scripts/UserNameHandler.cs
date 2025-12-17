using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserProfile
{
    public class UserNameHandler : MonoBehaviour
    {
        private UserProfileManager userProfileManager;
        private DBUserProfileController _db;
        [SerializeField] private TMP_InputField ipfName;
        [SerializeField] private GameObject gobjTextName;
        [SerializeField] private GameObject gobjButtonChangeName;
        [SerializeField] private PopupUserProfile popupUserProfile;

        public void Init()
        {
            GetReference();
        }
        private void GetReference()
        {
            userProfileManager = UserProfileManager.Instance;
            _db = DBUserProfileController.Instance;
        }
        public void OnClickChangeName()
        {
            ipfName.text = "";
            ToggleButton(true);

            if (popupUserProfile != null)
            {
                popupUserProfile.ActiveSaveButton(true);
            }
        }
        public void OnDoneChangeName()
        {
            string newName = ipfName.text;
            bool isNameValidate = IsNameValid(newName);
            if (isNameValidate)
            {
                var userInfo = _db.USER_INFO_DATA;
                userInfo.SetUserName(newName);
                TrackingController.Instance.TrackingEditProfile(EditProfileType.Name, $"{newName}");

                userProfileManager.UpdateUserUI();
                ToggleButton(false);
            } else
            {
                ToggleButton(false);
            }
        }
        private void ToggleButton(bool willChangeName)
        {
            ipfName.gameObject.SetActive(willChangeName);
            gobjTextName.SetActive(!willChangeName);
            gobjButtonChangeName.SetActive(!willChangeName);
        }
        private bool IsNameValid(string name)
        {

            return name.Length > 0;
        }
        public void OnEditName()
        {
            if (ipfName.text.Length == 0)
                return;
            List<char> invalidChar = new List<char>() { ' '};
            if (invalidChar.Contains(ipfName.text[ipfName.text.Length-1]))
            {
                Debug.Log($"Remove char invalid : '{ipfName.text[ipfName.text.Length - 1]}'");
                ipfName.text = ipfName.text.Remove(ipfName.text.Length - 1);
            }
        }
    }
}
