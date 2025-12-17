using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class TabMonthly : TabBaseLB
    {
        [SerializeField] private InfinityScroll scroll;
        [SerializeField] private Top3UserItem top3;
        [SerializeField] private bool playerIsInTop3 = false;
        [SerializeField] private int playerIndex = -1;
        [SerializeField] private List<UserData> displayData;
        [SerializeField] private UserItem playerUserItem;
        [SerializeField] private Transform holderUserInTop;
        [SerializeField] private Transform holderUserInBottom;
        public override void Init()
        {
            base.Init();
            ShowLeaderboard();
            scroll.Hide();
            top3.Hide();
            LeaderboardManager.Instance.GetController<TabGiftController>().HideLstItemGiftMonthly();
        }
        protected override void Show()
        {
            ShowLeaderboard();
            base.Show();
            scroll.Show();
            LeaderboardManager.Instance.GetController<LeaderboardScrollController>().SetInfinityScroll(scroll, holderUserInTop, holderUserInBottom);

            LeaderboardManager.Instance.GetController<LeaderboardScrollController>().SetPlayerData(!playerIsInTop3, playerUserItem);

            SetTop3();
            top3.Show();
            LeaderboardManager.Instance.GetController<TabGiftController>().ShowLstItemGiftMonthly();

            /*
                        var playerData = LeaderboardManager.Instance.GetController<PlayerDataManager>().CurrentUser;
                        var dailyData = playerData.GetDayData();
                        playerUserItem.SetData(playerIndex, dailyData, true);*/
            if (!playerIsInTop3)
            {
                scroll.CheckPlayerVisibility(true);
            }
        }
        protected override void Hide()
        {
            base.Hide();
            scroll.Hide();
            top3.Hide();
            LeaderboardManager.Instance.GetController<TabGiftController>().HideLstItemGiftMonthly();

        }
        public void ShowLeaderboard()
        {
            var dataController = LeaderboardManager.Instance.GetController<LeaderboardDataController>();

            var data = dataController.GetMonthlyData();
            var playerData = LeaderboardManager.Instance.GetController<PlayerDataManager>().CurrentUser;

            var monthData = playerData.GetMonthData();
            Combine(data.data.users, monthData);

            displayData = displayData.GetRange(0, Mathf.Min(displayData.Count, 100));



            if (playerIsInTop3)
            {
                playerUserItem.gameObject.SetActive(false);
            }
            else
            {
                playerUserItem.gameObject.SetActive(true);
                playerUserItem.SetData(playerIndex, monthData, true);
                scroll.CheckPlayerVisibility(true);

                Debug.Log($"Player is in top 3: {playerIsInTop3} \n RankData{playerData} \n{monthData.points}");

            }


            top3.SetData(displayData[0], displayData[1], displayData[2], playerIndex);
            scroll.Init(displayData.GetRange(3, displayData.Count - 3), monthData, 3, 0, 100,playerIndex);
            scroll.InitAtTopId(0);


        }
        public void SetTop3()
        {
            top3.SetData(displayData[0], displayData[1], displayData[2], playerIndex);
        }

        private void Combine(List<UserData> data, UserData playerData)
        {
            displayData = new List<UserData>(data);
            int index = RankCalculator.GetIndex(data, playerData);
            playerIndex = index;
            displayData.Insert(index, playerData);
            if (index < 3)
                playerIsInTop3 = true;
            else
                playerIsInTop3 = false;
        }

    }
}
