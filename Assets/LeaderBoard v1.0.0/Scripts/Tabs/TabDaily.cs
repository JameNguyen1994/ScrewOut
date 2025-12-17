using DailyReward;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class TabDaily : TabBaseLB
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
            LeaderboardManager.Instance.GetController<TabGiftController>().HideLstItemGiftDaily();
        }
        protected override void Show()
        {
            ShowLeaderboard();

            base.Show();
            scroll.Show();

            LeaderboardManager.Instance.GetController<LeaderboardScrollController>().SetInfinityScroll(scroll,holderUserInTop,holderUserInBottom);
            LeaderboardManager.Instance.GetController<LeaderboardScrollController>().SetPlayerData(!playerIsInTop3, playerUserItem);

            SetTop3();
            top3.Show();
            LeaderboardManager.Instance.GetController<TabGiftController>().ShowLstItemGiftDaily();

          /*  var playerData = LeaderboardManager.Instance.GetController<PlayerDataManager>().CurrentUser;
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
            LeaderboardManager.Instance.GetController<TabGiftController>().HideLstItemGiftDaily();

        }
        public void ShowLeaderboard()
        {
            var dataController = LeaderboardManager.Instance.GetController<LeaderboardDataController>();

            var data = dataController.GetDailyData();
            var playerData = LeaderboardManager.Instance.GetController<PlayerDataManager>().CurrentUser;
            var dailyData = playerData.GetDayData();

            Combine(data.users, dailyData);

            displayData = displayData.GetRange(0, Mathf.Min(displayData.Count, 100));

            top3.SetData(displayData[0], displayData[1], displayData[2], playerIndex);
            scroll.Init(displayData.GetRange(3, displayData.Count - 3), dailyData, 3, 0, 100,playerIndex);

            if (playerIsInTop3)
            {
                playerUserItem.gameObject.SetActive(false);

            }
            else
            {
                playerUserItem.gameObject.SetActive(true);
                playerUserItem.SetData(playerIndex, dailyData, true);
                scroll.CheckPlayerVisibility(true);
                Debug.Log($"Tab Daily Player Data: {playerData}");
            }
            scroll.InitAtTopId(0);

            Debug.Log($"Player is in top 3: {playerIsInTop3} \n RankData{playerData}");
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
