using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
namespace ps.modules.leaderboard
{
    public class AnimationStar : MonoBehaviour
    {
        [SerializeField] private RectTransform starTransform;
        [SerializeField] private List<ItemStarFly> lstStar;
        [SerializeField] private float timeMove = 0.8f;
        [SerializeField] private float delayMove = 0.1f;
        [SerializeField] private float scaleTo = 0.5f;
        [SerializeField] private ParticleSystem parShow;


        public async UniTask ShowStar()
        {
            starTransform.localScale = Vector3.zero;
            starTransform.gameObject.SetActive(true);
            //starTransform.DORotate(new Vector3(0, 0, 360), 0.4f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            await starTransform.DOScale(Vector3.one * 1.1f, 0.4f).SetEase(Ease.OutBack).ToUniTask();

            parShow.gameObject.SetActive(true);
            await UniTask.Delay(200);
            // await starTransform.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo);
        }
        [Button]
        public async UniTask Show(RectTransform target, int starAmount)
        {
            foreach (var star in lstStar)
            {
                star.Hide();
            }

            var lstTask = new List<UniTask>();
            for (int i = 0; i < starAmount && i < lstStar.Count; i++)
            {
                var star = lstStar[i];
                starTransform.DOScale(Vector3.one * 1.1f, delayMove / 2)
                    .SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo);
                // Quỹ đạo cong (tạo điểm giữa bay vòng)
                var startPos = starTransform.position;
                var endPos = target.position;

                lstTask.Add(star.MovePath(startPos, endPos, timeMove, scaleTo, i % 2 == 0));

                await UniTask.Delay((int)(delayMove * 1000));

            }
            await UniTask.WhenAll(lstTask);
            AudioController.Instance.PlaySound(SoundName.CollectExp);

        }
        public void Hide()
        {
            starTransform.gameObject.SetActive(false);
        }

    }
}