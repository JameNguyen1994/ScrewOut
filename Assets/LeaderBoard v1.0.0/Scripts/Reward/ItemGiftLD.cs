using Cysharp.Threading.Tasks;
using DG.Tweening;
using Life;
using NUnit.Framework;
using ps.modules.journey;
using Storage;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.leaderboard
{
    public class ItemGiftLD : MonoBehaviour
    {
        [SerializeField] private RectTransform rtfmItem;
        [SerializeField] private Transform tfmPreviewGift;
        [SerializeField] private Transform tfmGiftBox;
        [SerializeField] private Transform tfmRewardHolder;

        [SerializeField] private Image imgGiftBox;
        [SerializeField] private Image imgGiftLid;

        [SerializeField] private bool isPreview;

        [SerializeField] private List<ResourceValue> lstResourceValue;
        public async UniTask TurnPreviewGift(bool on)
        {
            if (on)
            {
                isPreview = true;
                tfmPreviewGift.DOKill();
                tfmPreviewGift.gameObject.SetActive(true);
                await tfmPreviewGift.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                isPreview = false;
                tfmPreviewGift.DOKill();
                await tfmPreviewGift.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
                tfmPreviewGift.gameObject.SetActive(false);
            }
        }

        public void Reset()
        {
            isPreview = false;
            tfmPreviewGift.gameObject.SetActive(false);
            tfmPreviewGift.transform.localScale = Vector3.zero;
        }
        public void SetData(ImageGiftData imageGift, GiftData gift)
        {
            Reset();

            imgGiftBox.sprite = imageGift.sprBox;
            imgGiftLid.sprite = imageGift.sprLid;
            tfmGiftBox.localScale = Vector3.one * imageGift.scale;


            if (tfmPreviewGift != null)
            {
                var lstResource = gift.rewards;
                for (int i = 0; i < lstResource.Count; i++)
                {
                    lstResourceValue.Add(lstResource[i]);
                    var itemResource = LeaderboardManager.Instance.GetController<PoolingReward>().GetItemResource();
                    itemResource.InitResource(lstResource[i]);
                    itemResource.transform.SetParent(tfmRewardHolder, false);
                    itemResource.gameObject.SetActive(true);
                }
            }
            rtfmItem.gameObject.SetActive(true);
        }
        public void OnClickItem()
        {
            TurnPreviewGift(!isPreview).Forget();
        }
        private void OnDisable()
        {
            TurnPreviewGift(false);
        }

        public void Show()
        {
           // GetGift();
            rtfmItem.gameObject.SetActive(true);
        }
        public void Hide()
        {
            rtfmItem.gameObject.SetActive(false);
            TurnPreviewGift(false).Forget();
        }

    }
}
