using Cysharp.Threading.Tasks;
using DG.Tweening;
using Life;
using NUnit.Framework;
using Storage;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.journey
{
    public class ItemGiftJourney : MonoBehaviour
    {
        [SerializeField] private RectTransform rtfmItem;
        [SerializeField] private Transform tfmPreviewGift;
        [SerializeField] private Transform tfmLstRewardHolder;
        [SerializeField] private Transform tfmOneRewardHolder;
        [SerializeField] private Transform tfmBoxLstReward;
        [SerializeField] private Transform tfmBoxOneReward;

        [SerializeField] private bool isPreview;

        [SerializeField] private List<ResourceValueJourney> lstResourceValue;
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
        public void SetData(List<ResourceValueJourney> lstResourceData)
        {
            Reset();

            if (tfmPreviewGift != null)
            {
                Debug.Log($"SetData Gift with {lstResourceData.Count} rewards");
                tfmBoxLstReward.gameObject.SetActive(lstResourceData.Count > 1);
                tfmBoxOneReward.gameObject.SetActive(lstResourceData.Count <= 1);
                var parentRewardHolder = lstResourceData.Count > 1 ? tfmLstRewardHolder : tfmOneRewardHolder;
                var lstResource = lstResourceData;
                for (int i = 0; i < lstResource.Count; i++)
                {
                    lstResourceValue.Add(lstResource[i]);
                    var itemResource = JourneyPooling.Instance.GetInstance();
                    itemResource.InitResource(lstResource[i]);
                    itemResource.transform.SetParent(parentRewardHolder, false);
                    itemResource.gameObject.SetActive(true);
                }
            }
            // rtfmItem.gameObject.SetActive(true);
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
            rtfmItem.gameObject.SetActive(true);
        }
        public void Hide()
        {
            rtfmItem.gameObject.SetActive(false);
            TurnPreviewGift(false).Forget();
        }
        public void GetGift()
        {
            foreach (var res in lstResourceValue)
            {
                switch (res.type)
                {
                    case ResourceTypeJourney.Coin:
                        var user = Db.storage.USER_INFO;
                        user.coin += res.value;
                        Db.storage.USER_INFO = user;
                        EventDispatcher.Push(EventId.UpdateCoinUI);
                        break;
                    case ResourceTypeJourney.BoosterAddHold:
                        Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.AddHole, res.value);
                        break;
                    case ResourceTypeJourney.BoosterUnlockBox:
                        Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.UnlockBox, res.value);
                        break;
                    case ResourceTypeJourney.BoosterBloom:
                        Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Clears, res.value);
                        break;
                    case ResourceTypeJourney.BoosterHammer:
                        Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Hammer, res.value);
                        break;
                    case ResourceTypeJourney.InfiniteLives:
                        LifeController.Instance.AddInfinityTime(res.value *60*1000);
                        break;
                    case ResourceTypeJourney.InfiniteGlass:
                        Db.storage.PreBoosterData.AddFreeTime(PreBoosterType.Glass, res.value * 60 * 1000);


                        break;
                    case ResourceTypeJourney.InfiniteRocket:
                        Db.storage.PreBoosterData.AddFreeTime(PreBoosterType.Rocket, res.value * 60  * 1000);

                        break;


                }
                Debug.Log($"Get Gift Resource {res.type} - Value: {res.value}");
            }
            lstResourceValue.Clear();
        }
    }
}
