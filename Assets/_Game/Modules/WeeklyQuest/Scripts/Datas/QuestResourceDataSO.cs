using System.Collections.Generic;
using UnityEngine;

namespace WeeklyQuest
{
    [CreateAssetMenu(fileName = "QuestResourceDataSO", menuName = "Scriptable Objects/WeeklyQuest/QuestResourceDataSO")]
    public class QuestResourceDataSO : ScriptableObject
    {
        public List<QuestResourceImage> questResources;
        public Sprite GetQuestResourceIcon(QuestType type)
        {
            foreach (var resource in questResources)
            {
                if (resource.type == type)
                {
                    return resource.icon;
                }
            }
            Debug.LogWarning($"No icon found for QuestType: {type}");
            return null;
        }
        public string GetQuestDescription(QuestType type, int value)
        {
            switch (type)
            {
                case QuestType.ReviveTimes:
                    return $"REVIVE {value} TIMES.";
                case QuestType.StayOnline_minues:
                    return $"STAY ONLINE FOR {value}M";
                case QuestType.CollectCoins:
                    return $"COLLECT {value} COINS PASS LEVELS.";
                case QuestType.ClaimScrews:
                    return $"CLAIM {value} SCREWS.";
                case QuestType.ClaimScrews_Color_Blue:
                    return $"CLAIM {value} BLUE SCREWS.";
                case QuestType.ClaimScrews_Color_Orange:
                    return $"CLAIM {value} ORANGE SCREWS.";
                case QuestType.ClaimScrews_Color_GreenBlack:
                    return $"CLAIM {value} GREEN SCREWS.";
                case QuestType.ClaimScrews_Color_Purple:
                    return $"CLAIM {value} PURPLE SCREWS.";
                case QuestType.ClaimScrews_Color_Gray:
                    return $"CLAIM {value} GRAY SCREWS.";
                case QuestType.ClaimScrews_Color_Sky:
                    return $"CLAIM {value} SKY SCREWS.";
                case QuestType.ClaimScrews_Color_Pink:
                    return $"CLAIM {value} PINK SCREWS.";
                case QuestType.ClaimScrews_Color_Red:
                    return $"CLAIM {value} RED SCREWS.";
                case QuestType.ClaimScrews_Color_Green:
                    return $"CLAIM {value} GREEN SCREWS.";
                case QuestType.ClaimScrews_Color_Yellow:
                    return $"CLAIM {value} YELLOW SCREWS.";
                case QuestType.UseCoins:
                    return $"USE {value} COINS";
                default:
                    return $"No description available for this quest type.";
            }
        }
    }
    [System.Serializable]
    public class QuestResourceImage
    {
        public QuestType type;
        public Sprite icon;

    }
}
