using PS.Analytic;
using Storage;
using System;

public static class SupperOfferService
{
    public static int Repeat()
    {
        if (GameAnalyticController.Instance != null)
        {
            var remote = GameAnalyticController.Instance.Remote();
            return remote != null ? remote.SORepeat : 10;
        }

        return 10;
    }

    public static bool CheckRevealSupperOffer()
    {
        int level = Db.storage.USER_INFO.level;

        if (!IsActive() && IsUnlock(level))
        {
            SupperOfferData data = Db.storage.SupperOfferData;

            if (data.revealLevel != level)
            {
                //if (data.isBuy)
                //{
                //    data.isBuy = false;
                //    Db.storage.SupperOfferData = data;
                //}

                return true;
            }
        }

        return false;
    }

    public static bool IsActive()
    {
        int level = Db.storage.USER_INFO.level;
        SupperOfferData data = Db.storage.SupperOfferData;

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
        if (level >= 9)
        {
            if (level == 9 || (level - 9) % Repeat() == 0)
            {
                return true;
            }
        }

        return false;
    }

    public static void SetReveal()
    {
        int level = Db.storage.USER_INFO.level;
        SupperOfferData data = Db.storage.SupperOfferData;

        if (!IsActive() && level != data.revealLevel && IsUnlock(level))
        {
            DateTime dateTime = TimeGetter.Instance.Now.AddHours(2);
            data.revealLevel = level;
            data.endMinute = dateTime.Minute;
            data.endHour = dateTime.Hour;
            data.endDay = dateTime.Day;
            data.endMonth = dateTime.Month;
            data.endYear = dateTime.Year;
            data.isBuy = false;
            Db.storage.SupperOfferData = data;
        }
    }

    public static void SetRevealLevelOnly()
    {
        int level = Db.storage.USER_INFO.level;
        SupperOfferData data = Db.storage.SupperOfferData;
        data.revealLevel = level;
        Db.storage.SupperOfferData = data;
    }


    public static void SetBuy()
    {
        SupperOfferData data = Db.storage.SupperOfferData;
        data.isBuy = true;
        Db.storage.SupperOfferData = data;
    }
}