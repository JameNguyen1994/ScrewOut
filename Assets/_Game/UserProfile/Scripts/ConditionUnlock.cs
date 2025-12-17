using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserProfile
{

    [System.Serializable]
    public class ConditionUnlock
    {
        public ConditionType conditionType;
        public long value;
        public string description;
        public override string ToString()
        {
            return description;
        }
        public bool Unlocked(long currentValue)
        {
            return currentValue >= value;
        }
    }
    public enum ConditionType
    {
        None = 0,
        ReachLevel = 1,
        ReachCoin = 2,
        ReachDiamond = 3,
        ReachTime = 4
    }
}


