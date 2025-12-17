using System;
using UnityEngine;

[Serializable]
public class LevelStartAd
{
    public long focusLvl;
}


[Serializable]
public class SegmentFlow
{
    public int interLvl;
    public int bannerLvl;
    public int breakLvl;
    public int breakCount;
    public bool boosterEnable;
    public int noads;
    public int noadsWithCombo;
    public int beginAds;
    public bool enableExpBar;

    public override string ToString()
    {
        return $"i:{interLvl}, b:{bannerLvl}, br:{breakLvl}, brc:{breakCount}, be:{boosterEnable}, na:{noads}, ee:{enableExpBar}";
    }
}

[SerializeField]
public class BeginAds
{
    public int beginAds;
    public void RandomizeCryptoKey()
    {

    }
}
[SerializeField]
public class NoAdsWithCombo
{
    public int noAdsWithCombo;
    public void RandomizeCryptoKey()
    {

    }
}

[Serializable]
public class LevelStartFeature
{
    public long cannon;
    public long shuffle;
    public long hammer;
    public long tnt;

    public void RandomizeCryptoKey()
    {
    }
}

[Serializable]
public class MovePercent
{
    public int begin;
    public int master;
    public void RandomizeCryptoKey()
    {

    }
}

[Serializable]
public class LifeCountDownLevel
{
    public int newUserLvl;
    public int newUserCd;
    public int stickyUserLvl;
    public int stickyUserCd;
    public int superLoyaltyUserLvl;
    public int superLoyaltyUserCd;
}

[Serializable]
public class ConfigPopupMoreLife
{
    public int lifeRewardAds;
    public int lifeCoin;
    public int coinPrice;

}