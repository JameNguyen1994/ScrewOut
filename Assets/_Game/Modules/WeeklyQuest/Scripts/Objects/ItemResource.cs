using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeeklyQuest
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
            imgResource.sprite = WeeklyQuestManager.Instance.WeeklyDataHelper.GetResourceIcon(resourceValue.type);
        }
    }
}
