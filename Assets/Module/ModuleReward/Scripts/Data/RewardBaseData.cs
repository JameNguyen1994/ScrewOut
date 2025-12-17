using System;
using UnityEngine;

[Serializable]
public class RewardBaseData : RewardInfo
{
    [Tooltip("The amount or value of this reward.")]
    public int Value;

    public int GetValue()
    {
        if (RewardType is ResourceIAP.ResourceType.InfiniteLives
                       or ResourceIAP.ResourceType.InfiniteRocket
                       or ResourceIAP.ResourceType.InfiniteGlass)
        {
            return Value * 60 * 1000;
        }

        return Value;
    }
}

[Serializable]
public class RewardInfo
{
    [Tooltip("The type of reward (Energy, Coin, Exp, etc.)")]
    public ResourceIAP.ResourceType RewardType;

    [Tooltip("The icon representing this reward in the UI.")]
    public Sprite Icon;

    public Sprite GetIcon()
    {
        if (Icon != null)
        {
            return Icon;
        }

        return RewardConfigManager.Instance.GetRewardIcon(RewardType);
    }
}