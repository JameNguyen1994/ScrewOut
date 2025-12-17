using Storage;
using UnityEngine;
using System.Collections.Generic;
using System;

public static class WrenchCollectionService
{
    public static bool IsActive()
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;

        if (data.endYear > 0)
        {
            DateTime dateTime = new DateTime(data.endYear, data.endMonth, data.endDay, data.endHour, data.endMinute, 0);
            TimeSpan remaining = dateTime - TimeGetter.Instance.Now;
            return remaining.TotalSeconds > 0;
        }

        return false;
    }

    public static bool IsShowInMain()
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        return !data.isComplete && data.isActive;
    }

    public static bool IsUnlock(int userLevel)
    {
        return userLevel >= Define.UNLOCK_WRENCH_COLLECTION;
    }

    public static void ActiveEvent()
    {
        if (!IsActive())
        {
            WrenchCollectionData data = Db.storage.WrenchCollectionData;

            if (data.collectedInGamplayWrench > 0)
            {
                return;
            }

            int userLevel = Db.storage.USER_INFO.level;

            if (!IsUnlock(userLevel))
            {
                return;
            }

            data = new WrenchCollectionData();

            DateTime dateTime = TimeGetter.Instance.Now;
            dateTime = dateTime.AddDays(3);

            data.isActive = true;
            data.isReveal = false;

            data.endYear = dateTime.Year;
            data.endMonth = dateTime.Month;
            data.endDay = dateTime.Day;
            data.endHour = dateTime.Hour;
            data.endMinute = dateTime.Minute;
            data.rewardGroup = UnityEngine.Random.Range(0, 3);

            Db.storage.WrenchCollectionData = data;

            return;
        }

        return;
    }

    public static Vector3 UIToWorldPosition(RectTransform uiElement, float worldZ = 0f)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, uiElement.position);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, worldZ));
        return worldPos;
    }

    public static void CollectWrench(int amount)
    {
        EditorLogger.Log("[WrenchCollection] CollectWrench " + amount);

        int userLevel = Db.storage.USER_INFO.level;

        if (IsMaxLevel() || !IsUnlock(userLevel))
        {
            return;
        }

        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        data.collectedInGamplayWrench += amount;
        Db.storage.WrenchCollectionData = data;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static void UpgradeLevel()
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        WrenchCollectionRewardData reward = WrenchCollectionManager.Instance.Config.GetConfigByIndex(data.level, data.rewardGroup);

        if (reward != null)
        {
            if (data.collectedWrench + data.collectedInGamplayWrench < reward.WrenchAmount)
            {
                data.collectedWrench = data.collectedWrench + data.collectedInGamplayWrench;
                data.collectedInGamplayWrench = 0;
            }
            else if (data.collectedWrench + data.collectedInGamplayWrench >= reward.WrenchAmount)
            {
                Storage.Model.RewardData rewardData = Db.storage.RewardData.DeepClone();

                switch (reward.RewardType)
                {
                    case ResourceIAP.ResourceType.Coin:
                        rewardData.coinAmount += reward.GetValue();
                        break;

                    case ResourceIAP.ResourceType.BoosterAddHold:
                        rewardData.BoosterValue(BoosterType.AddHole, reward.GetValue());
                        break;

                    case ResourceIAP.ResourceType.BoosterHammer:
                        rewardData.BoosterValue(BoosterType.Hammer, reward.GetValue());
                        break;

                    case ResourceIAP.ResourceType.BoosterBloom:
                        rewardData.BoosterValue(BoosterType.Clears, reward.GetValue());
                        break;

                    case ResourceIAP.ResourceType.BoosterUnlockBox:
                        rewardData.BoosterValue(BoosterType.UnlockBox, reward.GetValue());
                        break;

                    case ResourceIAP.ResourceType.InfiniteLives:
                        rewardData.heartTimeAmount += reward.GetValue();
                        break;

                    case ResourceIAP.ResourceType.InfiniteGlass:
                        Db.storage.PreBoosterData.AddFreeTime(PreBoosterType.Glass, reward.GetValue());
                        break;

                    case ResourceIAP.ResourceType.InfiniteRocket:
                        Db.storage.PreBoosterData.AddFreeTime(PreBoosterType.Rocket, reward.GetValue());
                        break;

                    case ResourceIAP.ResourceType.Glass:
                        Db.storage.PreBoosterData.AddValue(PreBoosterType.Glass, reward.GetValue());
                        break;

                    case ResourceIAP.ResourceType.Rocket:
                        Db.storage.PreBoosterData.AddValue(PreBoosterType.Rocket, reward.GetValue());
                        break;
                }

                Db.storage.RewardData = rewardData;

                data.claimRewars.Add(data.level);
                data.collectedInGamplayWrench = data.collectedWrench + data.collectedInGamplayWrench - reward.WrenchAmount;
                data.level++;

                if (IsMaxLevel())
                {
                    data.collectedWrench = reward.WrenchAmount;
                    data.collectedInGamplayWrench = 0;
                }
                else
                {
                    data.collectedWrench = 0;
                }

                if (data.collectedInGamplayWrench > 0 && !IsMaxLevel())
                {
                    Db.storage.WrenchCollectionData = data;
                    UpgradeLevel();
                    return;
                }
            }

            Db.storage.WrenchCollectionData = data;
        }
    }

    public static bool HaveNewWrench()
    {
        return Db.storage.WrenchCollectionData.collectedInGamplayWrench > 0;
    }

    public static bool IsMaxLevel()
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        WrenchCollectionRewardData reward = WrenchCollectionManager.Instance.Config.GetConfigByIndex(data.level, data.rewardGroup);

        return reward == null;
    }

    public static bool IsCompleteLevel(int level)
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        return data.level >= level;
    }

    public static bool IsClaimed(int level)
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        return data.claimRewars.Contains(level);
    }

    public static void ClaimReward(int level)
    {
        if (IsClaimed(level))
        {
            return;
        }

        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        WrenchCollectionRewardData reward = WrenchCollectionManager.Instance.Config.GetConfigByIndex(level, data.rewardGroup);

        Storage.Model.RewardData rewardData = Db.storage.RewardData.DeepClone();

        switch (reward.RewardType)
        {
            case ResourceIAP.ResourceType.Coin:
                rewardData.coinAmount += reward.GetValue();
                break;

            case ResourceIAP.ResourceType.BoosterAddHold:
                rewardData.BoosterValue(BoosterType.AddHole, reward.GetValue());
                break;

            case ResourceIAP.ResourceType.BoosterHammer:
                rewardData.BoosterValue(BoosterType.Hammer, reward.GetValue());
                break;

            case ResourceIAP.ResourceType.BoosterBloom:
                rewardData.BoosterValue(BoosterType.Clears, reward.GetValue());
                break;

            case ResourceIAP.ResourceType.BoosterUnlockBox:
                rewardData.BoosterValue(BoosterType.UnlockBox, reward.GetValue());
                break;

            case ResourceIAP.ResourceType.InfiniteLives:
                rewardData.heartTimeAmount += reward.GetValue();
                break;
        }

        data.claimRewars.Add(level);

        Db.storage.WrenchCollectionData = data;
        Db.storage.RewardData = rewardData;
    }

    public static bool IsShowTutorial()
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        return !data.isShowTutorial;
    }

    public static void ShowedTutorial()
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        data.isShowTutorial = true;
        Db.storage.WrenchCollectionData = data;
    }

    public static bool ShowReveal()
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        return !data.isReveal;
    }

    public static void ShowedReveal()
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        data.isReveal = true;
        Db.storage.WrenchCollectionData = data;
    }

    public static int WrenchAmountToSpawnInGamePlay(int totalScrew)
    {
        int result = 0;

        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        WrenchCollectionRewardData reward = WrenchCollectionManager.Instance.Config.GetConfigByIndex(data.level, data.rewardGroup);

        result = (int)((float)totalScrew / 40 + 0.5f) + (reward != null ? reward.EventConstant : 0);

        return result;
    }
}