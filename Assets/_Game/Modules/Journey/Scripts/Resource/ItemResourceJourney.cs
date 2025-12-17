using Cysharp.Threading.Tasks;
using DG.Tweening;
using ps.modules.leaderboard;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.journey

{
    public class ItemResourceJourney : MonoBehaviour
    {
        [SerializeField] private ResourceValueJourney resourceValue;
        [SerializeField] private TextMeshProUGUI txtValue;
        [SerializeField] private Image imgResource;
        public void InitResource(ResourceValueJourney resourceValue)
        {
            this.resourceValue = resourceValue;
            txtValue.text = $"{resourceValue.ValueToString()}";
            imgResource.sprite = JourneyResourceDataManager.Instance.ResourceDataSO.GetResourceData(resourceValue.type).icon;
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
    }
}
