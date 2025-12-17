
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WeeklyQuest
{

    [CreateAssetMenu(fileName = "WeeklyData", menuName = "Scriptable Objects/WeeklyQuest/WeeklyData")]
    public class WeeklyData : ScriptableObject, ICloneable
    {
        public int maxPoint;
        public List<GiftData> gifts;
        public List<QuestDataValue> questValues;

        public object Clone()
        {
            return new WeeklyData
            {
                maxPoint = this.maxPoint,
                gifts = new List<GiftData>(this.gifts),
                questValues = new List<QuestDataValue>(this.questValues)
            };
        }
    }

    [System.Serializable]
    public class GiftData
    {
        public List<ResourceValue> rewards;
        public int requiredPoint;
    }
    [System.Serializable]
    public class QuestsData
    {
        public List<QuestDataValue> quests;
    }
    [System.Serializable]
    public class QuestDataValue
    {
        public QuestType type;
        public int targetValue;
        public int pointValue;
    }


    public enum QuestType
    {
        None = 0,
        ReviveTimes = 1,
        StayOnline_minues = 2,
        CollectCoins = 3,
        ClaimScrews = 4,
        ClaimScrews_Color_Blue = 5,
        ClaimScrews_Color_Orange = 6,
        ClaimScrews_Color_GreenBlack = 7,
        ClaimScrews_Color_Purple = 8,
        ClaimScrews_Color_Gray = 9,
        ClaimScrews_Color_Sky = 10,
        ClaimScrews_Color_Pink = 11,
        ClaimScrews_Color_Green = 12,
        ClaimScrews_Color_Yellow = 13,
        ClaimScrews_Color_Red = 14,
        UseCoins = 15,
    }
}
