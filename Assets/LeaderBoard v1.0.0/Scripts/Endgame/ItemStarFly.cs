using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ps.modules.leaderboard
{

    public class ItemStarFly : MonoBehaviour
    {
        [SerializeField] private RectTransform rtfmStar;
        [SerializeField] private ParticleSystem parTrail;
        [SerializeField] private ParticleSystem parExplode;

        [SerializeField] private RectTransform rtfmTrail;
        [SerializeField] private RectTransform rtfmExplode;


        public void Hide()
        {
            rtfmStar.gameObject.SetActive(false);
        }
        public async UniTask MovePath(Vector3 startPos, Vector3 endPos, float timeMove, float scaleTo, bool isOdd)
        {
            rtfmStar.localScale = Vector3.zero;
            rtfmStar.gameObject.SetActive(true);
            parTrail.gameObject.SetActive(true);
            // Hiệu ứng bật nhỏ
            rtfmTrail.position = startPos;
            rtfmStar.position = startPos;

            rtfmStar.DOScale(Vector3.one * scaleTo, timeMove / 2)
                .SetEase(Ease.OutBack).OnComplete(() =>
                {
                    rtfmStar.DOScale(Vector3.zero, timeMove / 2).SetEase(Ease.InBack);
                });

            // Offset đường cong để mỗi sao bay hướng khác nhau
            float curveDir = (isOdd ? 1 : -1);
            float curveHeight = UnityEngine.Random.Range(100f, 200f);
            Vector3 controlPos = (startPos + endPos) / 2f + new Vector3(curveDir * 150f, curveHeight, 0);

            // Tạo quỹ đạo cong bằng DOPath
            Vector3[] path = new Vector3[]
            {
                startPos,
                controlPos,
                endPos
            };
            rtfmTrail.DOPath(path, timeMove, PathType.CatmullRom)
                  .SetEase(Ease.Linear).ToUniTask();
            await rtfmStar.DOPath(path, timeMove, PathType.CatmullRom)
                 .SetEase(Ease.Linear).ToUniTask();
            rtfmExplode.position = endPos;
            rtfmStar.gameObject.SetActive(false);
            parTrail.gameObject.SetActive(false);
            parExplode.gameObject.SetActive(true);
        }
    }

}