using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IngameData
{
    public static int MOVE_AMOUNT = 0;
    public static int BREAK_SCREW_COUNT = 0;
    public static int SET_COMPLETED = 0;
    public static float GAME_PLAY_TIME = 0f;
    public static bool IS_COLOR_TRAY_COMPARE_SHIP = true;
    public static bool IS_WIN = false;
    public static bool UNLOCK_SWIP = true;
    public static bool DONE_TUTORIAL = false;
    public static bool IS_SHOW_BOOSTER = false;
    public static int TRACKING_UNLOCK_BOX_COUNT = 0;
    public static int TRACKING_ADD_HOLE_COUNT = 0;
    public static int TRACKING_HAMMER_COUNT = 0;
    public static int TRACKING_CLEAR_COUNT = 0;
    public static int TRACKING_UN_SCREW_COUNT = 0;
    public static int TRACKING_REVIVE_COUNT = 0;
    public static ModeControl MODE_CONTROL = ModeControl.ControlV2;
    public static AdsPlacement adsPlacement;
    public static shop_placement shopPlacement;

    public static shop_placement SHOP_PLACEMENT
    {
        get => shopPlacement;
        set
        {
            shopPlacement = value;
            OfferwallController.Instance?.SetPlacement(shopPlacement.ToString());
        }
    }

    public static int REVIVE_REWARD_PER_LEVEL_COUNT = 0;
    public static bool IS_WIN_LEVEL = false;

    public static bool BUY_NO_ADS;
    public static bool IS_GEN_SCREW_BLOCK = true;

    public static PreBoosterPlace preBoosterPlace = PreBoosterPlace.Home;
    public static bool IsCheckFirstAds = true;

    public static int LoseCount = 0;
    public static GameMode GameMode = GameMode.Normal;


}
public enum AdsPlacement
{
    inter_end,
    inter_restart,
    reward_unlock_box,
    reward_add_hole,
    reward_hammer,
    reward_clear,
    reward_magnet,
    reward_coin_shop,
    reward_coin_bonus,
    reward_revive,
    inter_break
}
public enum GameMode
{
    Normal,
    Bonus
}