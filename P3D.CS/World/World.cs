namespace P3D;

// TODO Phase 4: full World port
public static class World
{
    private static Weathers _regionWeather = Weathers.Clear;

    public enum Seasons
    {
        Winter = 0,
        Spring = 1,
        Summer = 2,
        Fall = 3
    }

    public enum Weathers
    {
        Clear = 0,
        Rain = 1,
        Snow = 2,
        Underwater = 3,
        Sunny = 4,
        Fog = 5,
        Thunderstorm = 6,
        Sandstorm = 7,
        Ash = 8,
        Blizzard = 9,
        Mist = 10
    }

    public enum EnvironmentTypes
    {
        Outside = 0,
        Inside = 1,
        Cave = 2,
        Dark = 3,
        Underwater = 4,
        Forest = 5
    }

    public enum DayTimes
    {
        Night = 0,
        Morning = 1,
        Day = 2,
        Evening = 3
    }

    public static Seasons CurrentSeason
    {
        get
        {
            int month = DateTime.Now.Month;
            if (month >= 12 || month <= 2) return Seasons.Winter;
            if (month >= 3 && month <= 5) return Seasons.Spring;
            if (month >= 6 && month <= 8) return Seasons.Summer;
            return Seasons.Fall;
        }
    }

    public static DayTimes GetTime()
    {
        int hour = DateTime.Now.Hour;
        if (hour >= 6 && hour < 10) return DayTimes.Morning;
        if (hour >= 10 && hour < 17) return DayTimes.Day;
        if (hour >= 17 && hour < 20) return DayTimes.Evening;
        return DayTimes.Night;
    }

    public static Weathers GetCurrentRegionWeather()
    {
        return _regionWeather;
    }
}
