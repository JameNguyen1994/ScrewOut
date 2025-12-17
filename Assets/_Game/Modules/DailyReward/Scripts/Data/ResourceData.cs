using UnityEngine;

namespace DailyReward
{
    public enum ResourceType
    {
        None = 0,
        Coin = 1,
        BoosterAddHold = 2,     /// 1 unit
        BoosterHammer = 3,  /// 1 unit
        BoosterBloom = 4,    /// 1 unit
        InfiniteLives = 5,   /// 1 Hour = 3600 seconds = 3600* 1000 milliseconds
        BoosterUnlockBox = 6,   /// 1 Hour = 3600 seconds = 3600* 1000 milliseconds

    }
    [System.Serializable]
    public class ResourceData
    {
        public ResourceType type;
        public Sprite icon;

    }
    [System.Serializable]
    public class ResourceValue
    {
        public ResourceType type;
        public int value;
        public string ValueToString()
        {
            switch (type)
            {
                case ResourceType.Coin:
                    return $"{value}";
                case ResourceType.BoosterAddHold:
                    return $"{value}";
                case ResourceType.BoosterHammer:
                    return $"{value}";
                case ResourceType.BoosterBloom:
                    return $"{value}";
                case ResourceType.InfiniteLives:
                    return $"{value}h";
                case ResourceType.BoosterUnlockBox:
                    return $"{value}";
                default:
                    return "Unknown Resource";
            }
        }
    }

}
