public static class TimeConverter
{
    public static string ConvertMinutesToHoursAndMinutes(int totalMinutes)
    {
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        if (hours == 0)
        {
            return $"{minutes:D2}m";

        }
        if (minutes == 0)
        {
            return $"{hours}h";

        }
        return $"{hours}h{minutes:D2}m";
    }
}