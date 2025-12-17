using EasyButtons;
using MainMenuBar;
using Storage;
using UnityEngine;
using WeeklyQuest;

namespace WeeklyQuest
{

    public class QuestTriggerController : MonoBehaviour
    {
        [Button]
        public void OnTrigger1Quest(QuestType questType, int value)
        {
            if (Db.storage.USER_INFO.level < DBMainMenuBarController.Instance.DB_MAIN_MENU_ITEMS.lstDBBarItem[4].levelUnlock)
            {
                return;
            }
                var weeklyQuestData = Db.storage.WeeklyQuestData;
            if (weeklyQuestData == null)
            {
                Debug.LogError("WeeklyQuestData is not initialized.");
                return;
            }
            // Check if the quest type is valid
            if (questType == QuestType.None)
            {
                Debug.LogWarning("Quest type is None, skipping trigger.");
                return;
            }

            var quests = weeklyQuestData.quests;
            for (int i = 0; i < quests.Count; i++)
            {
                if (quests[i].questType == questType)
                {
                    // Update the quest progress
                    quests[i].currentValue += value;
                    Debug.Log($"Quest {quests[i].currentValue} updated to {quests[i].currentValue}/{quests[i].targetValue}");

                    // Check if the quest is completed
                    if (quests[i].currentValue >= quests[i].targetValue)
                    {
                        Debug.Log($"Quest {quests[i].currentValue} completed!");
                        quests[i].currentValue = quests[i].targetValue; // Ensure it doesn't exceed the target value
                        // Optionally, you can trigger a reward or notification here
                    }
                    // return; // Exit after updating the first matching quest
                }
            }
            Db.storage.WeeklyQuestData = weeklyQuestData; // Save the updated data back to storage
            Debug.Log($"After {questType} {Db.storage.WeeklyQuestData.ToString()}");
        }
    }
}