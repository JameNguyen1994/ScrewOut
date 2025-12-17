using Cysharp.Threading.Tasks;
using DG.Tweening;
using Life;
using Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.leaderboard

{
    public class ItemResource : MonoBehaviour
    {
        [SerializeField] private ResourceValue resourceValue;
        [SerializeField] private TextMeshProUGUI txtValue;
        [SerializeField] private Image imgResource;
        public void InitResource(ResourceValue resourceValue)
        {
            this.resourceValue = resourceValue;
            txtValue.text = $"{resourceValue.ValueToString()}";
            imgResource.sprite = LeaderboardManager.Instance.GetController<GiftDataManager>().ResourceDataSO.GetResourceData(resourceValue.type).icon;
        }

        public async UniTask Show()
        {
            imgResource.rectTransform.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.OutBack);
            await txtValue.rectTransform.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.OutBack);
        }
        public void Hide()
        {
            imgResource.rectTransform.localScale = Vector3.zero;
            txtValue.rectTransform.localScale = Vector3.zero;
        }
        public void GetGift()
        {
                switch (resourceValue.type)
                {
                    case ResourceType.Coin:
                        var user = Db.storage.USER_INFO;
                        user.coin += resourceValue.value;
                        Db.storage.USER_INFO = user;
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
