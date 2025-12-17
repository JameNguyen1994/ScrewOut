using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using Storage;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DailyReward
{
    public class DailyRewardItem : MonoBehaviour
    {
        [SerializeField] private int index;
        [SerializeField] private DayData dayData;
        [SerializeField] private List<ItemResource> lstItemResource;
        [SerializeField] private Transform tfmHolder;
        [SerializeField] private TextMeshProUGUI txtDay;
        [SerializeField] private Transform tfmCheck;
        [SerializeField] private Transform tfmHighLight;
        [SerializeField] private UIEffect uiHighLight;
        [SerializeField] private Sprite sprDayPass;
        [SerializeField] private Sprite sprDay;
        [SerializeField] private Image imgDay;
        [SerializeField] private Transform holder;

        public int Index { get => index; }

        public void Reset()
        {
            tfmCheck.gameObject.SetActive(false);
            tfmHighLight.gameObject.SetActive(false);
            uiHighLight.enabled = false;

            imgDay.sprite = sprDay;
            txtDay.text = $"DAY {index + 1}";
        }
        public void Hide()
        {
            holder.transform.localScale = Vector3.zero;
        }
        public async UniTask ShowAsync(float time)
        {
            holder.transform.localScale = Vector3.zero;
            await holder.transform.DOScale(Vector3.one, time).SetEase(Ease.OutBack).ToUniTask();
        }
        public void Init(int day, DayData dayData)
        {
            Reset();
            txtDay.text = $"DAY {day + 1}";
            index = day;
            this.dayData = dayData;
            float scale = 1;
            if (index == 6)
            {
                scale = 0.8f;
            }
            for (int i = 0; i < dayData.resources.Count; i++)
            {
                var resource = DailyRewardManager.Instance.DailyRewardHelper.CreateResource(dayData.resources[i], scale);
                lstItemResource.Add(resource);
                resource.transform.SetParent(tfmHolder, false);
            }
        }
        public void SetPassedDay()
        {
            Debug.Log($"SetPassedDay for day {index}");
            tfmCheck.gameObject.SetActive(true);
            imgDay.sprite = sprDayPass;
        }
        public void SetNextDay()
        {
            txtDay.text = $"Tomorrow";
        }
        public async UniTask SetCheckAsync()
        {
           // Debug.Log($"SetCheckAsync for day {id}");
            tfmCheck.gameObject.SetActive(true);
            tfmCheck.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true).From(Vector3.zero);
            tfmHighLight.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            imgDay.sprite = sprDayPass;
        }
        public async UniTask SetCurrentDay()
        {
          //  Debug.Log($"SetCurrentDay for day {id}");
            tfmHighLight.gameObject.SetActive(true);
            uiHighLight.enabled = true;
            txtDay.text = $"DAY {index + 1}";
        }
        public void GetReward()
        {
            var rewardData = Db.storage.RewardData.DeepClone();
            if (rewardData != null)
            {
                for (int i = 0; i < lstItemResource.Count; i++)
                {
                    var item = lstItemResource[i].ResourceValue;
                    if (item != null)
                    {
                        switch (item.type)
                        {
                            case ResourceType.Coin:
                                rewardData.coinAmount += item.value;
                                break;
                            case ResourceType.BoosterAddHold:
                                rewardData.BoosterValue(BoosterType.AddHole, item.value);
                                break;
                            case ResourceType.BoosterHammer:
                                rewardData.BoosterValue(BoosterType.Hammer, item.value);
                                break;
                            case ResourceType.BoosterBloom:
                                rewardData.BoosterValue(BoosterType.Clears, item.value);
                                break;
                            case ResourceType.InfiniteLives:
                                rewardData.heartTimeAmount += item.value * 3600 * 1000; // Convert hours to milliseconds
                                break;
                            case ResourceType.BoosterUnlockBox:
                                rewardData.BoosterValue(BoosterType.UnlockBox, item.value);
                                break;
                            default:
                                Debug.LogWarning($"Unknown resource type: {item.type} for item {i} in DailyRewardItem.GetReward()");
                                break;
                        }
                    }
                }

                Db.storage.RewardData = rewardData;
            }
        }
    }
}
