using ps.modules.leaderboard;
using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    [CreateAssetMenu(fileName = "GiftDataSO", menuName = "Scriptable Objects/Reward /GiftDataSO")]
    public class GiftDataSO : ScriptableObject
    {
        public List<GiftData> rewards;
    }

    [System.Serializable]
    public class GiftData
    {
        public List<ResourceValue> rewards;
    }
}
