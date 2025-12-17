using UnityEngine;


namespace WeeklyQuest
{
    public class WeeklyQuestManager : Singleton<WeeklyQuestManager>
    {
        [SerializeField] private WeeklyQuestController weeklyQuestController;
        [SerializeField] private ObjectPoolWeekly objectPoolWeekly;
        [SerializeField] private WeeklyDataHelper weeklyDataHelper;
        [SerializeField] private PopupGiftBoxQuest popupGiftBoxQuest;
        [SerializeField] private WeeklyTabNavigation weeklyTabNavigation;
        [SerializeField] private QuestTriggerController questTriggerController;
        [SerializeField] private PopupWeeklyTutorial popupWeeklyTutorial;

        public WeeklyQuestController WeeklyQuestController
        {
            get
            {
                if (weeklyQuestController == null)
                {
                    Debug.LogError("WeeklyQuestController is not assigned.");
                }
                return weeklyQuestController;
            }
        }
        public ObjectPoolWeekly ObjectPoolWeekly
        {
            get
            {
                if (objectPoolWeekly == null)
                {
                    Debug.LogError("ObjectPoolWeekly is not assigned.");
                }
                return objectPoolWeekly;
            }
        }
        public WeeklyDataHelper WeeklyDataHelper
        {
            get
            {
                if (weeklyDataHelper == null)
                {
                    Debug.LogError("WeeklyDataHelper is not assigned.");
                }
                return weeklyDataHelper;
            }
        }
        public PopupGiftBoxQuest PopupGiftBoxQuest { get => popupGiftBoxQuest; }
        public WeeklyTabNavigation WeeklyTabNavigation { get => weeklyTabNavigation; }
        public QuestTriggerController QuestTriggerController { get => questTriggerController; }
        public PopupWeeklyTutorial PopupWeeklyTutorial { get => popupWeeklyTutorial; }
    }
}