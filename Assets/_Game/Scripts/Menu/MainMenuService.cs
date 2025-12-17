using Life;
using Storage;
using Storage.Model;
using System.Collections.Generic;

public class MainMenuService
{
    public static void ClaimReward()
    {
        //logic claim reward
        var reward = Db.storage.RewardData.DeepClone();

        if (reward.coinAmount > 0)
        {
            var userInfo = Db.storage.USER_INFO;
            userInfo.coin += reward.coinAmount;
            Db.storage.USER_INFO = userInfo;

            reward.coinAmount = 0;
            Db.storage.RewardData = reward;
        }

        if (reward.heartTimeAmount > 0)
        {
            LifeController.Instance.StopCountDown();
            LifeController.Instance.AddInfinityTimeWihoutUpdateUI(reward.heartTimeAmount);
            reward.heartTimeAmount = 0;
            Db.storage.RewardData = reward;
        }

        if (reward.itemAmount > 0)
        {
            // var trkData = Db.storage.TRK_DATA.DeepClone();
            //
            // if (Db.storage.USER_INFO.level % 2 == 0)
            // {
            //     trkData.mulScoreExp += reward.itemAmount;
            // }
            // else
            // {
            //     trkData.singleScoreExp += reward.itemAmount;
            // }
            //
            // trkData.expOfSection += reward.itemAmount;
            //
            // Db.storage.TRK_DATA = trkData;
            ExpBar.Instance.AddExpToUser(reward.itemAmount);
            reward.itemAmount = 0;
            Db.storage.RewardData = reward;
        }

        if (reward.lstBoosterValue != null && reward.lstBoosterValue.Count > 0)
        {
            var boosterData = Db.storage.BOOSTER_DATAS;

            foreach (var booster in reward.lstBoosterValue)
            {
                var type = booster.boosterType;
                switch (type)
                {
                    case BoosterType.Hammer:
                        boosterData.AddBooster(BoosterType.Hammer, booster.value);
                        break;
                    case BoosterType.AddHole:
                        boosterData.AddBooster(BoosterType.AddHole, booster.value);
                        break;
                    case BoosterType.Clears:
                        boosterData.AddBooster(BoosterType.Clears, booster.value);
                        break;
                    case BoosterType.UnlockBox:
                        boosterData.AddBooster(BoosterType.UnlockBox, booster.value);
                        break;
                    case BoosterType.FreeRevive:
                        boosterData.AddBooster(BoosterType.FreeRevive, booster.value);
                        break;
                }
            }

            reward.lstBoosterValue = new List<BoosterRewardValue>();
            Db.storage.RewardData = reward;
        }
    }

    public static bool IsUnlockDailyGift()
    {
        return Db.storage.USER_INFO.level >= Define.UNLOCK_DAILY_GIFT;
    }

    public static bool IsUnlockNoADS()
    {
        return Db.storage.USER_INFO.level >= Define.UNLOCK_NO_ADS;
    }

    public static void ClaimRewardIAP()
    {
        //logic claim reward
        var reward = Db.storage.RewardIAPData.DeepClone();

        if (reward.coinAmount > 0)
        {
            var userInfo = Db.storage.USER_INFO;
            userInfo.coin += reward.coinAmount;
            Db.storage.USER_INFO = userInfo;
            reward.coinAmount = 0;
        }

        if (reward.heartTimeAmount > 0)
        {
            LifeController.Instance.StopCountDown();
            LifeController.Instance.AddInfinityTimeWihoutUpdateUI(reward.heartTimeAmount);
            reward.heartTimeAmount = 0;
        }

        if (reward.itemAmount > 0)
        {
            ExpBar.Instance.AddExpToUser(reward.itemAmount);
            reward.itemAmount = 0;
        }

        if (reward.lstBoosterValue != null && reward.lstBoosterValue.Count > 0)
        {
            var boosterData = Db.storage.BOOSTER_DATAS;

            foreach (var booster in reward.lstBoosterValue)
            {
                var type = booster.boosterType;
                switch (type)
                {
                    case BoosterType.Hammer:
                        boosterData.AddBooster(BoosterType.Hammer, booster.value);
                        break;
                    case BoosterType.AddHole:
                        boosterData.AddBooster(BoosterType.AddHole, booster.value);
                        break;
                    case BoosterType.Clears:
                        boosterData.AddBooster(BoosterType.Clears, booster.value);
                        break;
                    case BoosterType.UnlockBox:
                        boosterData.AddBooster(BoosterType.UnlockBox, booster.value);
                        break;
                    case BoosterType.FreeRevive:
                        boosterData.AddBooster(BoosterType.FreeRevive, booster.value);
                        break;
                }
            }

            reward.lstBoosterValue = new List<BoosterRewardValue>();
        }

        Db.storage.RewardIAPData = reward;
    }
}