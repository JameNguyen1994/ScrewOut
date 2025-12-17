using UnityEngine;
using UnityEngine.UI;

public class Define
{
    public const string LEVEL_ASSET_KEY = "Assets/_Game/SOF_Lvl/LevelMapRelease/Level_{0}.prefab";
    public const string LEVEL_BONUS_ASSET_KEY = "Assets/_Game/SOF_Lvl/LevelBonus/Level_Bonus_{0}.prefab";
    public const string CONFIG_SCREEN_RATIO = "Config/ScreenRatio";
    public const string SCREW_LAYER = "screw";
    public const string CONFIG_LEVEL_MAP = "Config/LevelMap";
    public const string CONFIG_POOL = "Config/Pool";

    public const string SECRET_BOX_ID = "3c31f225-b351-4012-858a-3fb348b895fd";
    public const string LEVEL_CONTROLLER_ID = "0525f278-4632-4147-9328-10e91f9cb773";

    public static readonly Identifier[] IDENTIFIERS = new Identifier[]
    {
        new Identifier("Box-f6575e05", "f6575e05-e59b-49e9-9878-88343c0c0e80"),
        new Identifier("Box-ded7b82a", "ded7b82a-30ba-4952-96dd-4dd38c1b4445"),
        new Identifier("Box-7400c3e9", "7400c3e9-19f5-43b4-917f-caa8babeb5f7"),
        new Identifier("Box-35b356c4", "35b356c4-d23a-44b7-8948-e7899ed77f4f"),
    };

    public const int TUTORIAL_LEVEL = 2;

    public const string RELOAD_CONFIRM = "DETECTED UNFINISHED LEVEL, WOULD YOU LIKE TO CONTINUE?";

    public const int AUTO_RELOAD_TIME = 0;

    public const int UNLOCK_DAILY_GIFT = 9;
    public const int UNLOCK_NO_ADS = 1;
    public const int UNLOCK_LUCKY_SPIN = 1;
    public const int UNLOCK_WRENCH_COLLECTION = 12;
}