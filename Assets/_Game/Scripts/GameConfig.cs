using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConfig
{
    public static int COIN_REVIVE = 1000;
    public static readonly int MAX_LEVEL = 150;
    public static readonly int LOOP_LEVEL_START = 5;

    public static long TIME_COIN_FREE = 24 * 60 * 60 * 1000;
    public static long TIME_COIN_ADS = 300000;
    public static int SHOP_COIN_FREE = 10;
    public static int SHOP_COIN_ADS = 30;
    public static int COIN_WIN
    {
        get
        {
            return Utility.RewardWinLevelCoin();
        }
    }

    public static int COIN_WIN_HARD = 50;
    public static int COIN_WIN_NORMAL_OR_EASY = 20;


    public static bool OLD_VERSION = false;
    
    public static int LEVEL_START_ENABLE_STARTER_PACK = 5;
    public static int LEVEL_START_ENABLE_KEEP_UP_OFFER = 8;
    
}
