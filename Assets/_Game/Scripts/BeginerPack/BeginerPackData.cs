using CodeStage.AntiCheat.ObscuredTypes;

public class BeginerPackData
{
    public ObscuredInt revealLevel;
    public ObscuredBool isBuy;
    public ObscuredInt endMinute;
    public ObscuredInt endHour;
    public ObscuredInt endDay;
    public ObscuredInt endMonth;
    public ObscuredInt endYear;

    public BeginerPackData()
    {
        isBuy = new ObscuredBool();
        revealLevel = new ObscuredInt();
        endMinute = new ObscuredInt();
        endHour = new ObscuredInt();
        endDay = new ObscuredInt();
        endMonth = new ObscuredInt();
        endYear = new ObscuredInt();
    }

    public BeginerPackData Clone()
    {
        BeginerPackData clone = new BeginerPackData();

        clone.revealLevel = revealLevel;
        clone.isBuy = isBuy;
        clone.endMinute = endMinute;
        clone.endHour = endHour;
        clone.endDay = endDay;
        clone.endMonth = endMonth;
        clone.endYear = endYear;

        return clone;
    }
}