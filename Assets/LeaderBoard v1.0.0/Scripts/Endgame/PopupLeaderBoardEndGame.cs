using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.leaderboard
{
    /// <summary>
    /// Popup LeaderBoard End Game
    /// </summary>
    public class PopupLeaderBoardEndGame : LeaderBoardCtrBase
    {
        [SerializeField] private InfinityScrollEndScreen infinityScroll;
        [SerializeField] private List<UserData> displayData;
        [SerializeField] private AnimationPopupEndGame animationPopup;
        [SerializeField] private AnimationStar animationStar;
        [SerializeField] private TMP_Text txtStarAmount;
        [SerializeField] private bool isShowing;


        [SerializeField] private int playerIndex = -1;
        [SerializeField] private int starWillAdd = 3;


        private async UniTask Start()
            
        {
            isShowing = false;
            Hide();

            //SetupData();
           
        }
        public void SetStarToAdd(int star)
        {
            starWillAdd = star;
            txtStarAmount.text = starWillAdd.ToString();
        }
        public override async UniTask Init()
        {
            await base.Init();
        }
        [Button]
        public void Hide()
        {
            animationPopup.Hide();
            animationStar.Hide();
        }
        public async UniTask CheckResetTime()
        {
            LeaderboardManager.Instance.GetController<PlayerDataManager>().CheckTime(false);
        }
        [Button]
        public async UniTask SetupData(float time)
        {
            await Init();

            await UniTask.WaitUntil(() => manager.GetController<LeaderBoardController>() != null && manager.GetController<LeaderBoardController>().InitData);
            LeaderboardManager.Instance.GetController<AdapterController>().OnShow();

            isShowing = true;
            var dataController = manager.GetController<LeaderboardDataController>();
            var playerData = manager.GetController<PlayerDataManager>();

            var dayData = dataController.GetDailyData();
            var player = playerData.CurrentUser.GetDayData();

            Combine(dayData.users, player);
            infinityScroll.Init(displayData, player, 0, 0, displayData.Count, playerIndex);

            //infinityScroll.InitAtTopId(playerIndex);
            infinityScroll.GoToPlayer();

            playerData.AddDailyPoint(starWillAdd);
            playerData.AddMonthlyPoint(starWillAdd);

            manager.GetController<TabController>().OnNeedToReset();
            animationPopup.Hide();
            animationStar.Hide();
            await animationPopup.AnimationShow(time);
            await UniTask.Delay(100);

            await ShowStar(starWillAdd);

            await UniTask.Delay(500);
            isShowing = false;
        }
        [Button]
        public async UniTask Show()
        {
            animationPopup.Hide();
            animationStar.Hide();
            await animationPopup.AnimationShow(0.3f);
            ShowStar(starWillAdd);
        }
        [Button]
        public async UniTask ShowStar(int point = 3)
        {
            animationStar.Hide();
            await animationStar.ShowStar();
            infinityScroll.CloneAndScaleUp();
            var itemPlayer = infinityScroll.GetPlayerItem();
            if (itemPlayer != null)
            {
                await animationStar.Show(itemPlayer.GetRectTransformStar(), point);
            }
            AudioController.Instance.PlaySound(SoundName.Coin);

            await infinityScroll.FlyToTop(1f, point);
        }

        private void Combine(List<UserData> data, UserData playerData)
        {
            displayData = new List<UserData>(data);
            int index = RankCalculator.GetIndex(data, playerData);
            playerIndex = index;
            displayData.Insert(index, playerData);
        }


    }
}
