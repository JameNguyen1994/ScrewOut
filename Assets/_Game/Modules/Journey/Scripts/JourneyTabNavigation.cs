using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.journey
{
    public class JourneyTabNavigation : Singleton<JourneyTabNavigation>
    {
        private Tween navigationTween;
        [SerializeField] private RectTransform rtfmWeeklyTab;
        [SerializeField] private GameObject content;

        private void Start()
        {
            //content.SetActive(false);
        }

        public async UniTask ShowAtHome(float width, int lastTabIndex)
        {
            var offset = lastTabIndex <= 1 ? width: -width;
            content.SetActive(true);
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
                navigationTween = null;
                JourneyController.Instance.GoToPlayer();
            });
            JourneyController.Instance.UpdateButtonVisibility(true);
        }
        public async UniTask ExitTab(float width, int lastTabIndex)
        {
            var offset = lastTabIndex <= 1 ? width: -width;

            navigationTween?.Kill();
            navigationTween = DOVirtual.Float(0, offset, 0.5f, value =>
            {
                rtfmWeeklyTab.offsetMin = new Vector2(value, rtfmWeeklyTab.offsetMin.y);
                rtfmWeeklyTab.offsetMax = new Vector2(value, rtfmWeeklyTab.offsetMax.y);
            }).OnComplete(() =>
            {
                content.SetActive(false);

            });
            await navigationTween;
        }
    }
}