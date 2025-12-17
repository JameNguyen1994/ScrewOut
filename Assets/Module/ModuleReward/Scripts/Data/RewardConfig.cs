using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardConfig", menuName = "RewardConfig", order = 0)]
public class RewardConfig : ScriptableObject
{
    public List<RewardInfo> Rewards = new List<RewardInfo>();

    public Sprite GetRewardIcon(ResourceIAP.ResourceType resource)
    {
        for (int i = 0; i < Rewards.Count; i++)
        {
            if (Rewards[i].RewardType == resource)
            {
                return Rewards[i].Icon;
            }
        }

        EditorLogger.Log("[GetRewardIcon] Missing Icon " + resource);
        return null;
    }
}