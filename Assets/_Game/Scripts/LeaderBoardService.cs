using MainMenuBar;
using Storage;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public static class LeaderBoardService
    {
        public static int GetCup(LevelMap levelMap)
        {
            return GetCup(levelMap.LevelDifficulty);
        }

        public static int GetCup(LevelDifficulty levelDifficulty)
        {
            var currentLevel = Db.storage.USER_INFO.level;
            var unlockLevel = DBMainMenuBarController.Instance.DB_MAIN_MENU_ITEMS.lstDBBarItem[3].levelUnlock; // Assuming index 3 is the Cup Menu Item
            if (currentLevel+1<unlockLevel)
            {
                return 0; // Locked
            }
            switch (levelDifficulty)
            {
                case LevelDifficulty.Easy:
                    return 1; // Bronze Cup
                case LevelDifficulty.Normal:
                    return 1; // Silver Cup
                case LevelDifficulty.Hard:
                    return 3; // Gold Cup
            }

            return 0;
        }

        public static int GetCupByLevel(LevelDifficulty levelDifficulty, int level)
        {
            var currentLevel = level;
            var unlockLevel = DBMainMenuBarController.Instance.DB_MAIN_MENU_ITEMS.lstDBBarItem[3].levelUnlock; // Assuming index 3 is the Cup Menu Item
            if (currentLevel + 1 < unlockLevel)
            {
                return 0; // Locked
            }
            switch (levelDifficulty)
            {
                case LevelDifficulty.Easy:
                    return 1; // Bronze Cup
                case LevelDifficulty.Normal:
                    return 1; // Silver Cup
                case LevelDifficulty.Hard:
                    return 3; // Gold Cup
            }

            return 0;
        }
    }
}
