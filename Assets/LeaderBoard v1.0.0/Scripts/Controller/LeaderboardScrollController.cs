using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine.PlayerLoop;
using DG.Tweening;

namespace ps.modules.leaderboard
{
    public class LeaderboardScrollController : LeaderBoardCtrBase
    {
        [Header("Refs")]
        [SerializeField] private InfinityScroll infinityScroll;
        [SerializeField] private UserItem userItem;
        [SerializeField] private Transform holderUserInTop;
        [SerializeField] private Transform holderUserInBottom;
        [SerializeField] private bool hasPlayer;
        [SerializeField] private bool isLocked = false;


        [Header("Settings")]
        [SerializeField] private float gotoDuration = 1f;
        [SerializeField] private float autoScrollSpeed = 150f;

        private void Start()
        {
            /*  if (!infinityScroll)
                  Debug.LogError("⚠️ InfinityScroll reference missing!");*/

            // Gắn các sự kiện từ InfinityScroll

        }
        public void GoTop()
        {
            if (isLocked) return;
            ActionGoTopAsync();
        }
        public void GoBottom()
        {
            if (isLocked) return;
            ActionGoBottomAsync();
        }
        public async UniTask ActionGoTopAsync()
        {
            isLocked = true;
            manager.GetController<TabController>().LockTab(true);
            if (infinityScroll != null)
                await infinityScroll.GotoUserAsync(0, gotoDuration);
            manager.GetController<TabController>().LockTab(false);
            isLocked = false;
        }
        public async UniTask ActionGoBottomAsync()
        {
            isLocked = true;
            manager.GetController<TabController>().LockTab(true);
            if (infinityScroll != null)
                await infinityScroll.GotoUserAsync(infinityScroll.TotalCount - 1, gotoDuration);
            manager.GetController<TabController>().LockTab(false);
            isLocked = false;
        }

        public void SetPlayerData(bool hasPlayer, UserItem userItem)
        {
            this.hasPlayer = hasPlayer;
            this.userItem = userItem;
        }
        public void SetInfinityScroll(InfinityScroll infinityScroll, Transform tfmTopUser, Transform tfmBottmUser)
        {
            this.infinityScroll = infinityScroll;
            infinityScroll.ResetEvents();
            infinityScroll.OnReachMin.AddListener(HandleReachMin);
            infinityScroll.OnLeaveMin.AddListener(HandleLeaveMin);
            infinityScroll.OnReachMax.AddListener(HandleReachMax);
            infinityScroll.OnLeaveMax.AddListener(HandleLeaveMax);
            infinityScroll.OnShowPlayerData.AddListener(OnShowPlayer);
            infinityScroll.OnHidePlayerData.AddListener(OnHidePlayer);

            this.holderUserInTop = tfmTopUser;
            this.holderUserInBottom = tfmBottmUser;
            this.hasPlayer = hasPlayer;
        }

        // --- Event Handlers ---
        private void HandleReachMin()
        {
            UpdateStatus("Reached top limit.");
        }

        private void HandleLeaveMin()
        {
        }

        private void HandleReachMax()
        {
            UpdateStatus("Reached bottom limit.");
        }

        private void HandleLeaveMax()
        {
        }
        private void OnShowPlayer()
        {
            if (hasPlayer)
                userItem.gameObject.SetActive(false);
        }
        private void OnHidePlayer(bool isTop)
        {
            if (hasPlayer)
            {
                userItem.gameObject.transform.SetParent(isTop ? holderUserInTop : holderUserInBottom, false);
                userItem.ResetPos();
                userItem.gameObject.SetActive(true);
                //userItem.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack);

                Debug.Log($"OnHidePlayer to {(isTop ? "Top" : "Bottom")} {userItem.transform.localPosition}");
            }

        }


        // --- Helper ---
        private void UpdateStatus(string msg)
        {
            Debug.Log("[LeaderboardScrollController] " + msg);
        }
        public async UniTask GotoPlayer()
        {
            if (isLocked) return;
            if (!infinityScroll) return;
            isLocked = true;
            manager.GetController<TabController>().LockTab(true);

            if (hasPlayer)
            {
                await infinityScroll.GoToPlayer();
            }
            else
            {
                UpdateStatus("⚠️ No player data set!");
            }
            isLocked = false;
            manager.GetController<TabController>().LockTab(false);
        }
    }
}
