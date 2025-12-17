using UnityEngine;

public class RewardConfigManager : Singleton<RewardConfigManager>
{
    public RewardConfig Config;

    public Sprite GetRewardIcon(ResourceIAP.ResourceType resource)
    {
        return Config.GetRewardIcon(resource);
    }
}