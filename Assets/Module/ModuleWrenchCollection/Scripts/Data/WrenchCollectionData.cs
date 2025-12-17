using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections.Generic;

public class WrenchCollectionData
{
    public ObscuredInt collectedWrench;
    public ObscuredInt collectedInGamplayWrench;
    public ObscuredInt level;
    public ObscuredInt rewardGroup;
    public ObscuredBool isComplete;
    public ObscuredBool isActive;
    public ObscuredBool isReveal;

    public List<int> claimRewars;

    public ObscuredInt endMinute;
    public ObscuredInt endHour;
    public ObscuredInt endDay;
    public ObscuredInt endMonth;
    public ObscuredInt endYear;
    public ObscuredBool isShowTutorial;

    public WrenchCollectionData()
    {
        isReveal = new ObscuredBool();
        isComplete = new ObscuredBool();
        isActive = new ObscuredBool();
        rewardGroup = new ObscuredInt();
        collectedWrench = new ObscuredInt();
        collectedInGamplayWrench = new ObscuredInt();
        level = new ObscuredInt();
        claimRewars = new List<int>();
        endMinute = new ObscuredInt();
        endHour = new ObscuredInt();
        endDay = new ObscuredInt();
        endMonth = new ObscuredInt();
        endYear = new ObscuredInt();
        isShowTutorial = new ObscuredBool();
    }

    public WrenchCollectionData Clone()
    {
        WrenchCollectionData clone = new WrenchCollectionData();

        clone.isComplete = isComplete;
        clone.isReveal = isReveal;
        clone.isActive = isActive;
        clone.rewardGroup = rewardGroup;
        clone.collectedWrench = collectedWrench;
        clone.collectedInGamplayWrench = collectedInGamplayWrench;
        clone.level = level;
        clone.claimRewars = claimRewars;
        clone.endMinute = endMinute;
        clone.endHour = endHour;
        clone.endDay = endDay;
        clone.endMonth = endMonth;
        clone.endYear = endYear;
        clone.isShowTutorial = isShowTutorial;

        return clone;
    }
}

[Serializable]
public class WrenchCollectionRewardData : RewardBaseData
{
    public int WrenchAmount;
    public int EventConstant;
}