using Storage;
using Storage.Model;
using System;

namespace Spin
{
    /// <summary>
    /// Provides core logic for handling spin actions, energy usage,
    /// reward claiming, and random selection within the spin system.
    /// </summary>
    public static class SpinService
    {
        /// <summary>
        /// Applies the reward based on its type.
        /// </summary>
        /// <param name="rewardData">The reward data to be claimed.</param>
        public static void ClaimReward(SpinReward rewardData)
        {
            switch (rewardData.RewardType)
            {
                case ResourceIAP.ResourceType.Coin:
                    var user = Db.storage.USER_INFO;
                    user.coin += rewardData.GetValue();
                    Db.storage.USER_INFO = user;
                    break;

                case ResourceIAP.ResourceType.BoosterAddHold:
                    Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.AddHole, rewardData.GetValue());
                    break;

                case ResourceIAP.ResourceType.BoosterHammer:
                    Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Hammer, rewardData.GetValue());
                    break;

                case ResourceIAP.ResourceType.BoosterBloom:
                    Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.Clears, rewardData.GetValue());
                    break;

                case ResourceIAP.ResourceType.BoosterUnlockBox:
                    Db.storage.BOOSTER_DATAS.AddBooster(BoosterType.UnlockBox, rewardData.GetValue());
                    break;

                case ResourceIAP.ResourceType.InfiniteGlass:
                    Db.storage.PreBoosterData.AddFreeTime(PreBoosterType.Glass, rewardData.GetValue());
                    break;
                case ResourceIAP.ResourceType.InfiniteRocket:
                    Db.storage.PreBoosterData.AddFreeTime(PreBoosterType.Rocket, rewardData.GetValue());
                    break;

                case ResourceIAP.ResourceType.InfiniteLives:
                    Life.LifeController.Instance.StopCountDown();
                    Life.LifeController.Instance.AddInfinityTimeWihoutUpdateUI(rewardData.GetValue());
                    break;
            }

            EditorLogger.Log($"[Spin] Claimed [{rewardData.RewardType}] +{rewardData.GetValue()}");
        }

        /// <summary>
        /// Calculates the total reward weight and returns a random point value within that range.
        /// </summary>
        /// <param name="rewardData">The collection of possible rewards.</param>
        /// <returns>A random point used to determine which reward is selected.</returns>
        public static int GetRandomPoint(SpinRewardData rewardData)
        {
            int totalPercent = 0;

            for (int i = 0; i < rewardData.Rewards.Count; i++)
            {
                totalPercent += rewardData.Rewards[i].Percent;
            }

            EditorLogger.Log($"[Spin] Total Percent: {totalPercent}");

            return UnityEngine.Random.Range(0, totalPercent);
        }

        /// <summary>
        /// Determines which reward segment should be selected based on the random point.
        /// </summary>
        /// <param name="rewardData">The collection of rewards and their probabilities.</param>
        /// <returns>The id of the selected reward.</returns>
        public static int GetTargetSegment(SpinRewardData rewardData)
        {
            if (Db.storage.LuckySpinData.spinCount == 0)
            {
                IncreaseSpinCount();
                return 6;
            }

            if (Db.storage.LuckySpinData.spinCount == 1)
            {
                IncreaseSpinCount();
                return 2;
            }

            if (Db.storage.LuckySpinData.spinCount == 2)
            {
                IncreaseSpinCount();
                return 0;
            }

            if (Db.storage.LuckySpinData.spinCount == 3)
            {
                IncreaseSpinCount();
                return 5;
            }
            if (Db.storage.LuckySpinData.spinCount == 4)
            {
                IncreaseSpinCount();
                return 5;
            }
            if (Db.storage.LuckySpinData.spinCount == 5)
            {
                IncreaseSpinCount();
                return 3;
            }
            if (Db.storage.LuckySpinData.spinCount == 6)
            {
                IncreaseSpinCount();
                return 3;
            }
            if (Db.storage.LuckySpinData.spinCount == 7)
            {
                IncreaseSpinCount();
                return 3;
            }
            if (Db.storage.LuckySpinData.spinCount == 8)
            {
                IncreaseSpinCount();
                return 2;
            }
            if (Db.storage.LuckySpinData.spinCount == 9)
            {
                IncreaseSpinCount();
                return 4;
            }
            if (Db.storage.LuckySpinData.spinCount == 10)
            {
                IncreaseSpinCount();
                return 7;
            }
            if (Db.storage.LuckySpinData.spinCount == 11)
            {
                IncreaseSpinCount();
                return 1;
            }
            if (Db.storage.LuckySpinData.spinCount == 12)
            {
                IncreaseSpinCount();
                return 3;
            }
            if (Db.storage.LuckySpinData.spinCount == 13)
            {
                IncreaseSpinCount();
                return 5;
            }
            if (Db.storage.LuckySpinData.spinCount == 14)
            {
                IncreaseSpinCount();
                return 6;
            }
           
            IncreaseSpinCount();

            int point = GetRandomPoint(rewardData);
            int cumulative = 0;

            for (int i = 0; i < rewardData.Rewards.Count; i++)
            {
                cumulative += rewardData.Rewards[i].Percent;
                if (point < cumulative)
                {
                    return i;
                }
            }

            return 0; // fallback in case of calculation edge case
        }

        private static void IncreaseSpinCount()
        {
            LuckySpinData luckySpinData = Db.storage.LuckySpinData;
            luckySpinData.spinCount++;
            Db.storage.LuckySpinData = luckySpinData;
        }

        /// <summary>
        /// Saves spin-related data (stub function - implement persistent logic here).
        /// </summary>
        public static void SaveData()
        {
            EditorLogger.Log("[Spin] Data saved successfully");
        }

        /// <summary>
        /// Adds collected screws to the player's storage.
        /// </summary>
        public static void CollectScrew(int amount)
        {
            if (!IsUnlock())
            {
                return;
            }

            EditorLogger.Log("[Spin] CollectScrew " + amount);

            LuckySpinData luckySpinData = Db.storage.LuckySpinData;
            luckySpinData.collectedInGamplayScrew += amount;
            Db.storage.LuckySpinData = luckySpinData;
        }

        /// <summary>
        /// Handles a spin attempt triggered by watching an ad.
        /// Updates the date and increments the daily ad spin counter.
        /// </summary>
        public static void SpinByADSConsumeHandler()
        {
            EditorLogger.Log("[Spin] SpinByADSConsumeHandler");

            LuckySpinData luckySpinData = Db.storage.LuckySpinData;
            DateTime Now = TimeGetter.Instance.Now;
            luckySpinData.adsWatchedDay = Now.Day;
            luckySpinData.adsWatchedMonth = Now.Month;
            luckySpinData.adsWatchedYear = Now.Year;
            luckySpinData.dailySpinByADS++;
            Db.storage.LuckySpinData = luckySpinData;
        }

        /// <summary>
        /// Consumes screws when the player spins using screws as currency.
        /// </summary>
        public static void SpinByScrewConsumeHandler()
        {
            EditorLogger.Log("[Spin] SpinByScrewConsumeHandler");

            LuckySpinData luckySpinData = Db.storage.LuckySpinData;
            luckySpinData.collectedScrew -= SpinDefine.REQURIED_SCREW;
            if (luckySpinData.collectedScrew < 0)
            {
                luckySpinData.collectedScrew = 0;
            }
            Db.storage.LuckySpinData = luckySpinData;
        }

        /// <summary>
        /// Checks if the player has enough screws to spin.
        /// </summary>
        public static bool CanSpinByScrew()
        {
            return Db.storage.LuckySpinData.collectedScrew >= SpinDefine.REQURIED_SCREW;
        }

        /// <summary>
        /// Checks if the player can still spin via ads today.
        /// </summary>
        public static bool CanSpinByADS()
        {
            LuckySpinData spinData = Db.storage.LuckySpinData;

            if (spinData.dailySpinByADS >= SpinDefine.DAILY_SPIN)
            {
                DateTime Now = TimeGetter.Instance.Now;

                if (spinData.adsWatchedDay != Now.Date.Day
                 || spinData.adsWatchedMonth != Now.Date.Month
                 || spinData.adsWatchedYear != Now.Date.Year)
                {
                    spinData.dailySpinByADS = 0;
                    Db.storage.LuckySpinData = spinData;
                    return true;
                }

                return false;
            }

            return true;
        }

        public static bool HaveNewScrew()
        {
            return Db.storage.LuckySpinData.collectedInGamplayScrew > 0;
        }

        public static void AddScrewHandler()
        {
            EditorLogger.Log("[Spin] AddScrewHandler");

            LuckySpinData spinData = Db.storage.LuckySpinData;
            spinData.collectedScrew += spinData.collectedInGamplayScrew;
            spinData.collectedInGamplayScrew = 0;
            Db.storage.LuckySpinData = spinData;
        }

        public static bool HaveTutorial()
        {
            return !Db.storage.LuckySpinData.isShowTutorial;
        }

        public static void TutorialDone()
        {
            LuckySpinData spinData = Db.storage.LuckySpinData;
            spinData.isShowTutorial = true;
            Db.storage.LuckySpinData = spinData;
        }

        public static bool IsUnlock()
        {
            return Db.storage.USER_INFO.level >= Define.UNLOCK_LUCKY_SPIN;
        }

        public static bool IsUnlock(int level)
        {
            return level >= Define.UNLOCK_LUCKY_SPIN;
        }

        public static void Reveal()
        {
            LuckySpinData spinData = Db.storage.LuckySpinData;

            if (!spinData.isReveal)
            {
                spinData.isReveal = true;
                spinData.collectedScrew = 0;
                Db.storage.LuckySpinData = spinData;
            }
        }
    }
}