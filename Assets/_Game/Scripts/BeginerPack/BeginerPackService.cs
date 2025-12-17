using PS.Analytic;
using Storage;
using System;
using UnityEngine;

public class BeginerPackService
{
    private const int RepeatLevel = 4;
    private const int UnlockLevel = 5;
    private const int TimeRemain = 8;

    public static int Repeat()
    {
        if (GameAnalyticController.Instance != null)
        {
            var remote = GameAnalyticController.Instance.Remote();
            return remote != null ? remote.SPRepeat : RepeatLevel;
        }

        return RepeatLevel;
    }

    public static bool CheckReveal()
    {
        int level = Db.storage.USER_INFO.level;

        if (!IsActive() && IsUnlock(level))
        {
            BeginerPackData data = Db.storage.BeginerPackData;

            if (data.revealLevel != level)
            {
                //if (data.isBuy)
                //{
                //    data.isBuy = false;
                //    Db.storage.BeginerPackData = data;
                //}

                return true;
            }
        }

        return false;
    }

    public static bool IsActive()
    {
        int level = Db.storage.USER_INFO.level;
        BeginerPackData data = Db.storage.BeginerPackData;

        if (data.revealLevel <= level && data.endYear > 0)
        {
            DateTime dateTime = new DateTime(data.endYear, data.endMonth, data.endDay, data.endHour, data.endMinute, 0);
            TimeSpan remaining = dateTime - TimeGetter.Instance.Now;
            return remaining.TotalSeconds > 0 && !data.isBuy;
        }

        return false;
    }

    public static bool IsUnlock(int level)
    {
        if (level >= UnlockLevel)
        {
            if (level == UnlockLevel || (level - UnlockLevel) % Repeat() == 0)
            {
                return true;
            }
        }

        return false;
    }

    public static void SetReveal()
    {
        int level = Db.storage.USER_INFO.level;
        BeginerPackData data = Db.storage.BeginerPackData;

        if (!IsActive() && level != data.revealLevel && IsUnlock(level))
        {
            DateTime dateTime = TimeGetter.Instance.Now.AddHours(TimeRemain);
            data.revealLevel = level;
            data.endMinute = dateTime.Minute;
            data.endHour = dateTime.Hour;
            data.endDay = dateTime.Day;
            data.endMonth = dateTime.Month;
            data.endYear = dateTime.Year;
            data.isBuy = false;
            Db.storage.BeginerPackData = data;
        }
    }

    public static void SetRevealLevelOnly()
    {
        int level = Db.storage.USER_INFO.level;
        BeginerPackData data = Db.storage.BeginerPackData;
        data.revealLevel = level;
        Db.storage.BeginerPackData = data;
    }


    public static void SetBuy()
    {
        BeginerPackData data = Db.storage.BeginerPackData;
        data.isBuy = true;
        Db.storage.BeginerPackData = data;
    }

    public static bool CheckAutoActive()
    {
        Debug.Log("[BeginerPackService] CheckAutoActive " + IsActive());
        if (!IsActive())
        {
            int level = Db.storage.USER_INFO.level;
            BeginerPackData data = Db.storage.BeginerPackData;

            if (level >= UnlockLevel)
            {
                Debug.Log("[BeginerPackService] Active");
                DateTime dateTime = TimeGetter.Instance.Now.AddHours(8);
                data.revealLevel = level;
                data.endMinute = dateTime.Minute;
                data.endHour = dateTime.Hour;
                data.endDay = dateTime.Day;
                data.endMonth = dateTime.Month;
                data.endYear = dateTime.Year;
                data.isBuy = false;
                Db.storage.BeginerPackData = data;

                return true;
            }
        }

        return false;
    }
}
