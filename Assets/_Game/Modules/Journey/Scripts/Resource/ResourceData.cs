using UnityEngine;

namespace ps.modules.journey
{
    public enum ResourceTypeJourney
    {
        None = 0,
        Coin = 1,
        BoosterAddHold = 2,     /// 1 unit
        BoosterHammer = 3,  /// 1 unit
        BoosterBloom = 4,    /// 1 unit
        InfiniteLives = 5,   /// 1 Hour = 3600 seconds = 3600* 1000 milliseconds
        BoosterUnlockBox = 6,   /// 1 Hour = 3600 seconds = 3600* 1000 milliseconds
        InfiniteRocket = 7,   /// 1 Hour = 3600 seconds = 3600* 1000 milliseconds
        InfiniteGlass = 8,   /// 1 Hour = 3600 seconds = 3600* 1000 milliseconds
    }
    [System.Serializable]
    public class ResourceDataJourney
    {
        public ResourceTypeJourney type;
        public Sprite icon;

    }
    [System.Serializable]
    public class ResourceValueJourney
    {
        public ResourceTypeJourney type;
        public int value;
        public string ValueToString()
        {
            switch (type)
            {
                case ResourceTypeJourney.Coin:
                    return $"{value}";
                case ResourceTypeJourney.BoosterAddHold:
                    return $"{value}";
                case ResourceTypeJourney.BoosterHammer:
                    return $"{value}";
                case ResourceTypeJourney.BoosterBloom:
                    return $"{value}";
                case ResourceTypeJourney.InfiniteLives:
                    return $"{value}M";
                case ResourceTypeJourney.BoosterUnlockBox:
                    return $"{value}";
                case ResourceTypeJourney.InfiniteRocket:
                    return $"{value}M";
                case ResourceTypeJourney.InfiniteGlass:
                    return $"{value}M";
                default:
                    return "Unknown Resource";
            }
        }
    }

}
