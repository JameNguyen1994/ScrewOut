using UnityEngine;

namespace ResourceIAP
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
        NoADs = 7,
        InfiniteGlass = 8,
        InfiniteRocket = 9,
        Glass = 10,
        Rocket = 11,
        FreeRevive = 12,
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
                    return $"{value/ (60*60*1000)}h";
                case ResourceType.BoosterUnlockBox:
                    return $"{value}";
                case ResourceType.Rocket:
                    return $"{value}";
                case ResourceType.Glass:
                    return $"{value}";
                case ResourceType.FreeRevive:
                    return $"{value}";
                case ResourceType.NoADs:
                    return $"7D";
                default:
                    return "Unknown Resource";
            }
        }
    }

}
