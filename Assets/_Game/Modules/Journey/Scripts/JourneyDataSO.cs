using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.journey
{
    [CreateAssetMenu(fileName = "JourneyDataSO", menuName = "Scriptable Objects/ Journey/ JourneyDataSO")]
    public class JourneyDataSO : ScriptableObject
    {
        public List<JourneyData> lstJourneyData;
    }
    [System.Serializable]
    public class JourneyData
    {
        public int idStory;
        public string name;
        public int levelStart;
        public int levelEnd;
        public Sprite sprBG;
        public List<JourneyMarkLevel> lstMarkLevel;
    }
    [System.Serializable]

    public class JourneyMarkLevel
    {
        public int level;
        public List<ResourceValueJourney> lstReward;
    }
}
