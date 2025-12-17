using Storage;
using UnityEngine;


namespace WeeklyQuest
{
    public class WeeklyDataHelper : MonoBehaviour
    {
        [SerializeField] private WeeklyDataSO weaklyDataSO;
        [SerializeField] private GiftSpriteSO giftSpriteSO;
        [SerializeField] private QuestResourceDataSO questResourceDataSO;
        [SerializeField] private ResourceDataSO resourceDataSO;

        public WeeklyData GetWeeklyData()
        {
            if (weaklyDataSO == null)
            {
                Debug.LogError("WeeklyDataSO is not assigned.");
                return null;
            }
            int weekIndex = (int)Db.storage.WEEK_INFO.numOfWeek;

            return weaklyDataSO.GetWeeklyData((int)Db.storage.WEEK_INFO.numOfWeek);
        }
        public ImageGiftData GetImageGift(int index)
        {
            if (giftSpriteSO == null)
            {
                Debug.LogError("GiftSpriteSO is not assigned.");
                return null;
            }
            return giftSpriteSO.GetSprite(index);
        }
        public Sprite GetResourceIcon(ResourceType resourceType)
        {
            if (resourceDataSO == null)
            {
                Debug.LogError("QuestResourceDataSO is not assigned.");
                return null;
            }
            return resourceDataSO.GetResourceData(resourceType).icon;
        }
        public Sprite GetQuestResourceIcon(QuestType type)
        {
            if (questResourceDataSO == null)
            {
                Debug.LogError("QuestResourceDataSO is not assigned.");
                return null;
            }
            return questResourceDataSO.GetQuestResourceIcon(type);
        }
        public string GetQuestDescription(QuestType type, int value)
        {
            if (questResourceDataSO == null)
            {
                Debug.LogError("QuestResourceDataSO is not assigned.");
                return string.Empty;
            }
            return questResourceDataSO.GetQuestDescription(type, value);
        }
    }
}
