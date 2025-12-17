using Cysharp.Threading.Tasks;
using DG.Tweening;
using Life;
using ps.modules.journey;
using Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeeklyQuest
{

    public class ItemResourcePopup : MonoBehaviour
    {
        [SerializeField] private Image imgItem;
        [SerializeField] private TextMeshProUGUI txtValue;
        [SerializeField] private ResourceValue resourceValue;

        public void SetData(ResourceValue resourceValue)
        {
            imgItem.sprite = WeeklyQuestManager.Instance.WeeklyDataHelper.GetResourceIcon(resourceValue.type);
            txtValue.text = $"X{resourceValue.value}";
            imgItem.gameObject.SetActive(false);
            txtValue.gameObject.SetActive(false);
            this.resourceValue = resourceValue;
            GetGift();
        }
        public async UniTask MoveTo(Vector3 targetPosition)
        {
            txtValue.gameObject.SetActive(false);
            await transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutQuad).ToUniTask();
            imgItem.gameObject.SetActive(false);
        }
        public async UniTask Show()
        {
            imgItem.gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            await transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).ToUniTask();

            txtValue.gameObject.SetActive(true);
            await txtValue.DOFade(1f, 0.1f).SetEase(Ease.Linear).ToUniTask();
        }
        public void GetGift()
        {
            Debug.Log($"Get gift: {resourceValue.type} x{resourceValue.value}");
            switch (resourceValue.type)
                {
                    case ResourceType.Coin:
                        var user = Db.storage.USER_INFO;
                        user.coin += resourceValue.value;
                        Db.storage.USER_INFO = user;
                        EventDispatcher.Push(EventId.UpdateCoinUI);
                    break;
                    case ResourceType.BoosterAddHold:
                        Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.AddHole, resourceValue.value);
                        break;
                    case ResourceType.BoosterUnlockBox:
                        Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.UnlockBox, resourceValue.value);
                        break;
                    case ResourceType.BoosterBloom:
                        Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Clears, resourceValue.value);
                        break;
                    case ResourceType.BoosterHammer:
                        Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Hammer, resourceValue.value);
                        break;
                    case ResourceType.InfiniteLives:
                        LifeController.Instance.AddInfinityTime(resourceValue.value * 60 * 60 * 1000);
                        break;

            }
        }
    }
}