using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using WeeklyQuest;

namespace Storage
{
    public partial class LocalDb
    {
        private WeeklyQuestDataDB _weeklyQuestData;
        public WeeklyQuestDataDB WeeklyQuestData
        {
            get
            {
                return _weeklyQuestData;
            }
            set
            {
                _weeklyQuestData = value;
                SetFromTData(DBKeyWeeklyQuest.WEEKLY_QUEST_DATA, _weeklyQuestData);

            }
        }
        public class DBKeyWeeklyQuest
        {
            public static readonly string WEEKLY_QUEST_DATA = "WEEKLY_QUEST_DATA";
        }
        public void LoadWeeklyQuestData(WeeklyData weeklyData, bool newData =false)
        {
            Debug.Log($"Weekly Data: {weeklyData.maxPoint}");
            if (!ObscuredPrefs.HasKey(DBKeyWeeklyQuest.WEEKLY_QUEST_DATA)|| newData)
            {
                Debug.Log("Initializing new WeeklyQuestDataDB in LocalDb.");
                var weeklyQuestDataDB = new WeeklyQuestDataDB(weeklyData);
                WeeklyQuestData = weeklyQuestDataDB;
            }
            _weeklyQuestData = GetFromJson<WeeklyQuestDataDB>(DBKeyWeeklyQuest.WEEKLY_QUEST_DATA);
        }

        [System.Serializable]
        public class WeeklyQuestDataDB
        {
            public bool isFirstTimeInWeek; // Flag to check if it's the first time the weekly quest is being initialized
            public int curWeek;
            public int currentPoint; // Current streak of consecutive days claimed
            public int maxPoint; // Maximum points for the week
            public List<QuestDataDB> quests; // List of quests
            public List<GiftDataDB> gifts; // List of gifts
            public WeeklyQuestDataDB()
            {

            }
            public WeeklyQuestDataDB(WeeklyData WeeklyData)
            {
                currentPoint = 0;
                isFirstTimeInWeek = true;
                maxPoint = WeeklyData.maxPoint; // Initialize with the maximum points
                quests = new List<QuestDataDB>();
                foreach (var questValue in WeeklyData.questValues)
                {
                    quests.Add(new QuestDataDB(questValue.type, questValue.targetValue, questValue.pointValue));
                }
                gifts = new List<GiftDataDB>();
                foreach (var gift in WeeklyData.gifts)
                {
                    Debug.Log($"Adding gift with required points: count ${WeeklyData.gifts.Count} {gift.requiredPoint}");
                    gifts.Add(new GiftDataDB(gift));
                }

            }
            public string ToString()
            {
                string giftsInfo = string.Join(", ", gifts.ConvertAll(g => $"{g.requirePoints} points: {string.Join(", ", g.lstResourceValue.ConvertAll(r => r.ValueToString()))} (Received: {g.isReceived}) \n"));
                string questsInfo = string.Join(", ", quests.ConvertAll(q => $"{q.questType} (Target: {q.targetValue},Old: {q.oldValue} Current: {q.currentValue}, Point: {q.point})\n"));
                return $"WeeklyQuestDataDB(CurrentPoint: {currentPoint}, MaxPoint: {maxPoint}, Quests: [{questsInfo}], Gifts: [{giftsInfo}])";
            }
            public void Save()
            {
                Db.storage.WeeklyQuestData = this; // Save the updated data
            }
        }
        [System.Serializable]
        public class GiftDataDB
        {
            public bool isReceived;
            public int requirePoints; // The target value for the quest
            public List<ResourceValue> lstResourceValue; // The target value for the quest
            public GiftDataDB(GiftData giftData)
            {
                isReceived = false;
                lstResourceValue = new List<ResourceValue>();
                if (giftData == null)
                {
                    //Debug.LogError("GiftData is null. Cannot initialize GiftDataDB.");
                    return;
                }
                // Initialize with the target value
                for (int i = 0; i < giftData.rewards.Count; i++)
                {
                    lstResourceValue.Add(giftData.rewards[i]);
                }
                this.requirePoints = giftData.requiredPoint;
            }
            public void ReceiveGift()
            {
                isReceived = true; // Mark the gift as received
            }
        }
        [System.Serializable]
        public class QuestDataDB
        {
            public QuestType questType;
            public int targetValue; // The target value for the quest
            public int currentValue; // The current progress towards the target value
            public int oldValue; // The current progress towards the target value
            public int point; // The current progress towards the target value
            public bool isComplete; // The current progress towards the target value
            public QuestDataDB(QuestType type, int target, int point)
            {
                questType = type;
                if (questType == QuestType.ClaimScrews)
                {
                    questType = (QuestType)Random.Range((int)QuestType.ClaimScrews_Color_Blue, (int)(QuestType.ClaimScrews_Color_Red) + 1);
                }
                targetValue = target;
                currentValue = 0; // Initialize current value to 0
                oldValue = 0; // Initialize old value to 0
                this.point = point; // Initialize point value
                isComplete = false;
            }
        }
    }
}
