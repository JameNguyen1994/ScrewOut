using UnityEngine;

namespace ps.modules.leaderboard
{

    public class Top3UserItem : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private UserItem userItem1;
        [SerializeField] private UserItem userItem2;
        [SerializeField] private UserItem userItem3;

        public void Show()
        {
            if (root != null)
            {
                root.gameObject.SetActive(true);
            }
        }
        public void Hide()
        {
            if (root != null)
            {
                root.gameObject.SetActive(false);
            }
        }
        public void SetData(UserData user1, UserData user2, UserData user3, int indexPlayer)
        {
            if (userItem1 != null && user1 != null)
            {
                userItem1.SetData(0, user1, indexPlayer==0);
                userItem1.gameObject.SetActive(true);
            }
            else if (userItem1 != null)
            {
                userItem1.gameObject.SetActive(false);
            }
            if (userItem2 != null && user2 != null)
            {
                userItem2.SetData(1, user2, indexPlayer == 1);
                userItem2.gameObject.SetActive(true);
            }
            else if (userItem2 != null)
            {
                userItem2.gameObject.SetActive(false);
            }
            if (userItem3 != null && user3 != null)
            {
                userItem3.SetData(2, user3, indexPlayer == 2);
                userItem3.gameObject.SetActive(true);
            }
            else if (userItem3 != null)
            {
                userItem3.gameObject.SetActive(false);
            }
        }
    }
}
