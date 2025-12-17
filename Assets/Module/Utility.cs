using Cysharp.Threading.Tasks;
using DG.Tweening;
using Storage;
using System;
using TMPro;
using UnityEngine;

public static class Utility
{
    public static string CountDownTimeToString(int endYear, int endMonth, int endDay, int endHour = 0, int endMinute = 0, int endSecond = 0)
    {
        DateTime dateTime = new DateTime(endYear, endMonth, endDay, endHour, endMinute, endSecond);
        TimeSpan remaining = dateTime - TimeGetter.Instance.Now;

        return TimeSpanToString(remaining);
    }

    public static string TimeSpanToString(TimeSpan remaining)
    {
        if (remaining.TotalSeconds <= 0)
        {
            return string.Empty;
        }

        if (remaining.Days > 0)
        {
            return $"{remaining.Days}D {remaining.Hours:D2}H";
        }
        else if (remaining.Hours > 0)
        {
            return $"{remaining.Hours:D2}H {remaining.Minutes:D2}M";
        }

        return $"{remaining.Minutes:D2}M {remaining.Seconds:D2}S";
    }

    public static string TimeSpanRewardToString(TimeSpan remaining)
    {
        if (remaining.TotalSeconds <= 0)
        {
            return string.Empty;
        }

        if (remaining.Days > 0)
        {
            return $"{remaining.Days}D" + (remaining.Hours > 0 ? $" {remaining.Hours}H" : string.Empty);
        }
        else if (remaining.Hours > 0)
        {
            return $"{remaining.Hours}H" + (remaining.Minutes > 0 ? $" {remaining.Minutes}M" : string.Empty);
        }

        return $"{remaining.Minutes}M" + (remaining.Seconds > 0 ? $" {remaining.Seconds}S" : string.Empty);
    }

    public static void DoTextProgress(this TextMeshProUGUI txtProgress, int currentValue, int targetValue, float duration)
    {
        DOTween.To(() => currentValue, x => txtProgress.text = $"{x}/{targetValue}", targetValue, duration).SetEase(Ease.OutCubic);
    }

    public static async UniTask DoTextProgressAsync(this TextMeshProUGUI txtProgress, int currentValue, int targetValue, float duration)
    {
        await DOTween.To(() => currentValue, x => txtProgress.text = $"{x}/{targetValue}", targetValue, duration).SetEase(Ease.OutCubic);
    }

    public static string RewardAmoutString(ResourceIAP.ResourceType rewardType, int value)
    {
        switch (rewardType)
        {
            case ResourceIAP.ResourceType.InfiniteLives: return TimeSpanRewardToString(new TimeSpan(0, value, 0));
            case ResourceIAP.ResourceType.InfiniteGlass: return TimeSpanRewardToString(new TimeSpan(0, value, 0));
            case ResourceIAP.ResourceType.InfiniteRocket: return TimeSpanRewardToString(new TimeSpan(0, value, 0));
        }

        return $"X{value}";
    }

    public static string RewardAmoutString(this RewardBaseData reward)
    {
        return RewardAmoutString(reward.RewardType, reward.Value);
    }

    public static int RewardWinLevelCoin()
    {
        int level = Db.storage.USER_INFO.level;
        LevelDifficulty levelDifficulty = LevelMapService.GetLevelDifficulty(level);
        return levelDifficulty == LevelDifficulty.Hard ? GameConfig.COIN_WIN_HARD : GameConfig.COIN_WIN_NORMAL_OR_EASY;
    }

    public static string ClockDownTimeToString(int endYear, int endMonth, int endDay, int endHour = 0, int endMinute = 0, int endSecond = 0)
    {
        DateTime dateTime = new DateTime(endYear, endMonth, endDay, endHour, endMinute, endSecond);
        TimeSpan remaining = dateTime - TimeGetter.Instance.Now;

        return $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
    }
}