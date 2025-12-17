using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class AnimationPopupEndGame : MonoBehaviour
    {
        [SerializeField] private RectTransform rtfmBanner;
        [SerializeField] private RectTransform rtfmScroll;
        [SerializeField] private RectTransform rtfmHolder;

        [Button]
        public async UniTask AnimationShow(float time)
        {
            rtfmBanner.localScale = Vector3.zero;
            rtfmScroll.localScale = new Vector3(1, 0, 1);
            rtfmBanner.anchoredPosition = new Vector2(0, -rtfmHolder.rect.height / 2);


            rtfmBanner.gameObject.SetActive(true);
            rtfmScroll.gameObject.SetActive(true);
            await rtfmBanner.DOScale(Vector3.one, time).SetEase(Ease.OutBack).ToUniTask();
            await UniTask.Delay(150);

            var lstTask = new List<UniTask>();
            lstTask.Add(rtfmBanner.DOAnchorPosY(0, time).ToUniTask());
            lstTask.Add(rtfmBanner.DOScale(Vector3.one * 1.1f, time).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo).ToUniTask());
            lstTask.Add(rtfmScroll.DOScale(Vector3.one, time).SetEase(Ease.OutBack).ToUniTask());

            await UniTask.WhenAll(lstTask);

        }
        public void Hide()
        {
            rtfmBanner.gameObject.SetActive(false);
            rtfmScroll.gameObject.SetActive(false);
        }
    }
}
