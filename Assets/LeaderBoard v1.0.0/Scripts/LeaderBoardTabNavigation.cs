using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.leaderboard
{
    public class LeaderBoardTabNavigation : LeaderBoardCtrBase
    {
        private Tween navigationTween;
        [SerializeField] private RectTransform rtfmWeeklyTab;



        public async UniTask ShowAtHome(float width, int lastTabIndex)
        {
            var offset = lastTabIndex <= 2 ? width: -width;
            manager.GetController<TabController>().ShowTab();
            navigationTween?.Kill();

            rtfmWeeklyTab.offsetMin = new Vector2(offset, rtfmWeeklyTab.offsetMin.y);
            rtfmWeeklyTab.offsetMax = new Vector2(offset, rtfmWeeklyTab.offsetMax.y);

            navigationTween = DOVirtual.Float(offset, 0, 0.5f, value =>
            {
                rtfmWeeklyTab.offsetMin = new Vector2(value, rtfmWeeklyTab.offsetMin.y);
                rtfmWeeklyTab.offsetMax = new Vector2(value, rtfmWeeklyTab.offsetMax.y);
            });
            //rtfmShop.anchoredPosition = new Vector2(-width, 0);

            //rtfmShop.DOAnchorPosX(0, 0.5f);
            navigationTween.OnComplete(() =>
            {
                manager.GetController<PlayerDataManager>().CheckTime();
                navigationTween = null;
            });
        }
        public async UniTask ExitTab(float width, int lastTabIndex)
        {
            var offset = lastTabIndex <= 2 ? width: -width;

            navigationTween?.Kill();
            navigationTween = DOVirtual.Float(0, offset, 0.5f, value =>
            {
                rtfmWeeklyTab.offsetMin = new Vector2(value, rtfmWeeklyTab.offsetMin.y);
                rtfmWeeklyTab.offsetMax = new Vector2(value, rtfmWeeklyTab.offsetMax.y);
            }).OnComplete(() =>
            {
                manager.GetController<TabController>().HideTab();


            });
            await navigationTween;
        }
    }
}