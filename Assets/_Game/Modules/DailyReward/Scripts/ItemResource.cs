using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DailyReward
{
    public class ItemResource : MonoBehaviour
    {
        [SerializeField] private ResourceValue resourceValue;
        [SerializeField] private TextMeshProUGUI txtValue;
        [SerializeField] private Image imgResource;

        public ResourceValue ResourceValue { get => resourceValue;}

        public void InitResource(ResourceValue resourceValue, float scale = 1f)
        {
            transform.localScale = Vector3.one * scale;
            this.resourceValue = resourceValue;
            txtValue.text = $"{resourceValue.ValueToString()}";
            imgResource.sprite = DailyRewardManager.Instance.DailyRewardHelper.GetSpriteByResourceType(resourceValue.type);
        }
    }
}
