using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace ps.modules.journey
{
    public class ShopTabNavigation : Singleton<ShopTabNavigation>
    {
        private Tween navigationTween;
        [SerializeField] private RectTransform rtfmWeeklyTab;
        [SerializeField] private GameObject content;
        [SerializeField] private Canvas canvas;
        [SerializeField] private int layerEnter;
        [SerializeField] private int layerExit;
        [SerializeField] private float width;

        private void Start()
        {
            //content.SetActive(false);
        }

        public async UniTask ShowAtHome(float width, int lastTabIndex)
        {
            var offset =  -width;
            this.width = -width;
            content.SetActive(true);
            canvas.sortingOrder = layerEnter;
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
            });
            JourneyController.Instance.UpdateButtonVisibility(true);
        }
        public async UniTask ExitTab(float width, int lastTabIndex)
        {
            var offset =  -width;
            canvas.sortingOrder = layerExit ;


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
        public void ForceShow()
        {
            navigationTween?.Kill();
            rtfmWeeklyTab.offsetMin = new Vector2(0, rtfmWeeklyTab.offsetMin.y);
            rtfmWeeklyTab.offsetMax = new Vector2(0, rtfmWeeklyTab.offsetMax.y);
            content.SetActive(true);
        }
    }
}