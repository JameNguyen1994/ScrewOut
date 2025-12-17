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
    public class ItemGiftJourneyPopupWin : MonoBehaviour
    {
        [SerializeField] private RectTransform rtfmItem;
        [SerializeField] private Transform tfmPreviewGift;
        [SerializeField] private Transform tfmLstRewardHolder;
        [SerializeField] private Transform tfmOneRewardHolder;
        [SerializeField] private Transform tfmBoxLstReward;
        [SerializeField] private Transform tfmBoxOneReward;
        [SerializeField] private Image imgRewardBackground;
        [SerializeField] private Transform tfmBackground;
        [SerializeField] private bool hasBackground;
        [SerializeField] private string name;
        [SerializeField] private bool isShowingAnimation;

        [SerializeField] private List<ResourceValueJourney> lstResourceValue;

        [SerializeField] private List<ItemResourceJourney> lstItemResourceJourney;

        public bool IsShowingAnimation { get => isShowingAnimation; }

        public void Reset()
        {
            tfmPreviewGift.gameObject.SetActive(false);
            tfmPreviewGift.transform.localScale = Vector3.zero;
            tfmBoxLstReward.gameObject.SetActive(false);
            tfmBoxOneReward.gameObject.SetActive(false);
            tfmBackground.gameObject.SetActive(false);
        }
        public void SetData(List<ResourceValueJourney> lstResourceData)
        {
            Reset();
            isShowingAnimation = false;

            lstItemResourceJourney = new List<ItemResourceJourney>();
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
                    lstItemResourceJourney.Add(itemResource);
                }
            }
        }
        public void SetJourneyData(JourneyData journeyData)
        {
            var sprite = journeyData.sprBG;
            name = journeyData.name;
            imgRewardBackground.sprite = sprite;
            tfmBackground.gameObject.SetActive(true);
            tfmBoxLstReward.gameObject.SetActive(false);
            tfmBoxOneReward.gameObject.SetActive(false);
            hasBackground = true;
        }
        public async UniTask GetGift()
        {
            var db = Db.storage.JOURNEY_DB;
            db.lstLevelClaim.Add(Db.storage.USER_INFO.level-1);
            Db.storage.JOURNEY_DB = db;
            isShowingAnimation = true;
            if (hasBackground)
            {
                await LevelProcessBar.Instance.ShowBackgroundReward(tfmBackground, name);
                isShowingAnimation = false;
                return;
            }

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
                        LifeController.Instance.AddInfinityTime(res.value * 60 * 1000);
                        break;
                    case ResourceTypeJourney.InfiniteGlass:
                        Db.storage.PreBoosterData.AddFreeTime(PreBoosterType.Glass, res.value * 60  * 1000);


                        break;
                    case ResourceTypeJourney.InfiniteRocket:
                        Db.storage.PreBoosterData.AddFreeTime(PreBoosterType.Rocket, res.value * 60  * 1000);

                        break;


                }
            }
            JourneyController.Instance.OnChangeLevel();
            tfmBoxLstReward.gameObject.SetActive(false);
            tfmBoxOneReward.gameObject.SetActive(true);
            for (int i = 0; i < lstItemResourceJourney.Count; i++)
            {
                var item = lstItemResourceJourney[i];

                item.transform.SetParent(tfmBoxOneReward, false);
                item.gameObject.SetActive(true);
                item.transform.localScale = Vector3.one;
                await item.Show();
                await item.transform.DOLocalMove(item.transform.localPosition + new Vector3(0, 10, 0), 0.1f).SetEase(Ease.InCubic);
                await UniTask.Delay(200);
                item.Hide();
                await UniTask.Delay(200);

            }



            lstResourceValue.Clear();


            isShowingAnimation = false;

        }
    }
}
