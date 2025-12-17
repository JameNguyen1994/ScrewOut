using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WeeklyQuest
{
    public class WeeklyTabNavigation : MonoBehaviour
    {
        private Tween navigationTween;
        [SerializeField] private RectTransform rtfmWeeklyTab;

        [SerializeField] private Transform tfmPopup;
        public async UniTask ShowAtHome(float width)
        {
            tfmPopup.gameObject.SetActive(true);
            navigationTween?.Kill();

            rtfmWeeklyTab.offsetMin = new Vector2(width, rtfmWeeklyTab.offsetMin.y);
            rtfmWeeklyTab.offsetMax = new Vector2(width, rtfmWeeklyTab.offsetMax.y);

            navigationTween = DOVirtual.Float(width, 0, 0.5f, value =>
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
        }
        public async UniTask ExitTab(float width)
        {
          
            navigationTween?.Kill();
            navigationTween = DOVirtual.Float(0, width, 0.5f, value =>
            {
                rtfmWeeklyTab.offsetMin = new Vector2(value, rtfmWeeklyTab.offsetMin.y);
                rtfmWeeklyTab.offsetMax = new Vector2(value, rtfmWeeklyTab.offsetMax.y);
            }).OnComplete(() =>
            {
                tfmPopup.gameObject.SetActive(false);

            });
            await navigationTween;
        }
    }
}