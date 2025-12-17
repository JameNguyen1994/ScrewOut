using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

namespace PS.Analytic.Event
{
    public class GameAnalyticEvent
    {
        public static void VirtualEvent(GAResourceFlowType type, 
            string currency, 
            float amount, 
            string itemType, 
            string itemId)
        {
            GameAnalytics.NewResourceEvent(type, currency, amount, itemType, itemId);
        }

        public static void Event(string eventName)
        {
            GameAnalytics.NewDesignEvent(eventName);
        }
    
        public static void Event(string eventName, float value)
        {
            GameAnalytics.NewDesignEvent(eventName, value);
        }
    
        public static void Event(string eventName, Dictionary<string, object> param)
        {
            GameAnalytics.NewDesignEvent(eventName, param);
        }

        public static void ProgressionEvent(GAProgressionStatus status, params string[] progressions)
        {
            int countEvent = progressions.Length > 3 ? 3 : progressions.Length;

            switch (countEvent)
            {
                case 1:
                    GameAnalytics.NewProgressionEvent(status, progressions[0]);
                    break;
                case 2:
                    GameAnalytics.NewProgressionEvent(status, progressions[0], progressions[1]);
                    break;
                case 3:
                    GameAnalytics.NewProgressionEvent(status, 
                        progressions[0], progressions[1], progressions[2]);
                    break;
                default:
                    Debug.LogWarning("Progression event param is empty.");
                    break;
            }
        }
        
        public static void ProgressionEvent(GAProgressionStatus status, int value, params string[] progressions)
        {
            int countEvent = progressions.Length > 3 ? 3 : progressions.Length;

            switch (countEvent)
            {
                case 1:
                    GameAnalytics.NewProgressionEvent(status, progressions[0], value);
                    break;
                case 2:
                    GameAnalytics.NewProgressionEvent(status, progressions[0], progressions[1], value);
                    break;
                case 3:
                    GameAnalytics.NewProgressionEvent(status, 
                        progressions[0], progressions[1], progressions[2], value);
                    break;
                default:
                    Debug.LogWarning("Progression event param is empty.");
                    break;
            }
        }
    }
}

