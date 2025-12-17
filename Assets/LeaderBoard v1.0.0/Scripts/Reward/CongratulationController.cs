using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace ps.modules.leaderboard.reward
{
    public class CongratulationController : LeaderBoardCtrBase
    {
        [SerializeField] private PopupCongratulations popupCongratulations;


        public async UniTask ShowDailyData(SaveTop3Data data)
        {
            if (data == null)
                return;
            var top = data.playerTop;
            if (top == 0 || top > 3)
                return;
            var userData = data.top3Users[top - 1];

            var playerData = manager.GetController<AdapterController>().PlayerDataAdapter.GetPlayerData();

            var playerUserData = new UserData
            {
                name = playerData.PlayerName,
                points = userData.points,
                avatar = playerData.SprAvatar,
                border = playerData.SprBorder
            };
            var time = data.time;
            var timeString = $"{time.Day}/{time.Month}/{time.Year}";

            var giftData = manager.GetController<GiftDataManager>();
            var giftDataDaily = giftData.GiftDay;
            var giftReward = giftDataDaily.rewards[top - 1];




            await popupCongratulations.Show(top, timeString, playerUserData, giftReward);



        }
        public async UniTask ShowMonthlyData(SaveTop3Data data)
        {
            if (data == null)
                return;
            var top = data.playerTop;
            if (top == 0 || top > 3)
                return;
            var userData = data.top3Users[top - 1];

            var playerData = manager.GetController<AdapterController>().PlayerDataAdapter.GetPlayerData();

            var playerUserData = new UserData
            {
                name = playerData.PlayerName,
                points = userData.points,
                avatar = playerData.SprAvatar,
                border = playerData.SprBorder
            };
            var time = data.time;
            var timeString = $"{time.Month}/{time.Year}";


            var giftData = manager.GetController<GiftDataManager>();
            var giftDataDaily = giftData.GiftMonth;
            var giftReward = giftDataDaily.rewards[top - 1];


            await popupCongratulations.Show(top, timeString, playerUserData, giftReward);
        }
    }
}
