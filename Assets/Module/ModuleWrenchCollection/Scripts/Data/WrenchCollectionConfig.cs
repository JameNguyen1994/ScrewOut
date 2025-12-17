using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WrenchCollectionConfig", menuName = "Wrench Collection/Wrench Collection Config", order = 0)]
public class WrenchCollectionConfig : ScriptableObject
{
    public List<WrenchCollectionRewardData> RewardsGroup1 = new List<WrenchCollectionRewardData>();
    public List<WrenchCollectionRewardData> RewardsGroup2 = new List<WrenchCollectionRewardData>();
    public List<WrenchCollectionRewardData> RewardsGroup3 = new List<WrenchCollectionRewardData>();

    public GameObject Wrench;

    public WrenchCollectionRewardData GetConfigByIndex(int index, int group)
    {
        List<WrenchCollectionRewardData> configs = GetConfigsByGroup(group);

        if (index < 0 || index >= configs.Count)
        {
            Debug.LogWarning($"[WrenchCollection] Invalid config id: {index}");
            return null;
        }

        return configs[index];
    }

    public List<WrenchCollectionRewardData> GetConfigsByGroup(int group)
    {
        switch (group)
        {
            case 0: return RewardsGroup1;
            case 1: return RewardsGroup2;
            case 2: return RewardsGroup3;
        }

        return null;
    }

    public WrenchCollectionRewardData GetFinalReward(int group)
    {
        List<WrenchCollectionRewardData> configs = GetConfigsByGroup(group);
        return configs[configs.Count - 1];
    }

#if UNITY_EDITOR

    [EasyButtons.Button]
    public void ParseDataSCVToConfig()
    {
        string path = "D:\\Data.csv";

        RewardsGroup1 = new List<WrenchCollectionRewardData>();
        RewardsGroup2 = new List<WrenchCollectionRewardData>();
        RewardsGroup3 = new List<WrenchCollectionRewardData>();

        List<ResourceIAP.ResourceType> resources = new List<ResourceIAP.ResourceType>();

        resources.Add(ResourceIAP.ResourceType.Coin);
        resources.Add(ResourceIAP.ResourceType.BoosterAddHold);
        resources.Add(ResourceIAP.ResourceType.BoosterHammer);
        resources.Add(ResourceIAP.ResourceType.BoosterBloom);
        resources.Add(ResourceIAP.ResourceType.InfiniteLives);
        resources.Add(ResourceIAP.ResourceType.BoosterUnlockBox);

        List<int> coins = new List<int>();

        coins.Add(150);
        coins.Add(350);
        coins.Add(100);
        coins.Add(250);

        List<int> times = new List<int>();

        times.Add(15);
        times.Add(30);
        times.Add(60);
        times.Add(45);

        foreach (var line in System.IO.File.ReadLines(path))
        {
            string[] values = line.Split(',');

            resources.Shuffle();
            coins.Shuffle();
            times.Shuffle();

            RewardsGroup1.Add(new WrenchCollectionRewardData()
            {
                RewardType = resources[0],
                Value = GetValue(resources[0], coins[0], times[0]),
                EventConstant = int.Parse(values[0]),
                WrenchAmount = int.Parse(values[1]),
            });

            RewardsGroup2.Add(new WrenchCollectionRewardData()
            {
                RewardType = resources[1],
                Value = GetValue(resources[1], coins[1], times[1]),
                EventConstant = int.Parse(values[0]),
                WrenchAmount = int.Parse(values[1]),
            });

            RewardsGroup3.Add(new WrenchCollectionRewardData()
            {
                RewardType = resources[2],
                Value = GetValue(resources[2], coins[2], times[2]),
                EventConstant = int.Parse(values[0]),
                WrenchAmount = int.Parse(values[1]),
            });
        }
    }

    private int GetValue(ResourceIAP.ResourceType resourceType, int coin, int time)
    {
        if (resourceType == ResourceIAP.ResourceType.Coin)
        {
            return coin;
        }

        if (resourceType == ResourceIAP.ResourceType.InfiniteLives)
        {
            return time;
        }


        return Random.Range(1, 4);
    }

#endif
}