namespace P3D;

public static class TimeHelpers
{
    private const int SECONDS_PER_MINUTE = 60;
    private const int MINUTES_PER_HOUR = 60;
    private const int HOURS_PER_DAY = 24;

    /// <summary>Converts a number of seconds to a TimeSpan.</summary>
    public static TimeSpan ConvertSecondToTime(int seconds)
    {
        int minutes = 0;
        int hours = 0;

        while (seconds > SECONDS_PER_MINUTE)
        {
            minutes++;
            seconds -= SECONDS_PER_MINUTE;

            if (minutes > MINUTES_PER_HOUR)
            {
                minutes = 0;
                hours++;
            }
        }

        return new TimeSpan(hours, minutes, seconds);
    }

    /// <summary>Returns the total play time for the current save.</summary>
    public static TimeSpan GetCurrentPlayTime()
    {
        TimeSpan pTime = Core.Player.PlayTime;
        TimeSpan diff = DateTime.Now - Core.Player.GameStart;
        return pTime + diff;
    }

    /// <summary>Formats a DateTime for display (HH:MM or HH:MM.SS).</summary>
    public static String GetDisplayTime(DateTime dateTime, bool showSeconds)
    {
        return GetDisplayTime(new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second), showSeconds);
    }

    /// <summary>Formats a TimeSpan for display (HH:MM or HH:MM.SS).</summary>
    public static String GetDisplayTime(TimeSpan time, bool showSeconds)
    {
        int hour = time.Hours + time.Days * HOURS_PER_DAY;
        String hours = hour.ToString("D2");
        String minutes = time.Minutes.ToString("D2");
        String seconds = time.Seconds.ToString("D2");

        String result = hours + ":" + minutes;
        if (showSeconds == true)
        {
            result += "." + seconds;
        }
        return result;
    }
}
