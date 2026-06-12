using P3D;

namespace P3D.BattleSystem;

public class BattleWeather
{
    public enum WeatherTypes : int
    {
        Clear = 0,
        Rain = 1,
        Sunny = 2,
        Sandstorm = 3,
        Hailstorm = 4,
        Foggy = 5,
        Snow = 6,
        Underwater = 7,
    }

    public static World.Weathers GetWorldWeather(BattleWeather.WeatherTypes fieldWeather)
    {
        switch (fieldWeather)
        {
            case WeatherTypes.Clear:
                return World.Weathers.Clear;
            case WeatherTypes.Foggy:
                return World.Weathers.Fog;
            case WeatherTypes.Hailstorm:
                return World.Weathers.Blizzard;
            case WeatherTypes.Rain:
                return World.Weathers.Rain;
            case WeatherTypes.Sandstorm:
                return World.Weathers.Sandstorm;
            case WeatherTypes.Sunny:
                return World.Weathers.Sunny;
            case WeatherTypes.Snow:
                return World.Weathers.Snow;
            case WeatherTypes.Underwater:
                return World.Weathers.Underwater;
            default:
                return World.Weathers.Clear;
        }
    }

    public static BattleWeather.WeatherTypes GetBattleWeather(World.Weathers worldWeather)
    {
        switch (worldWeather)
        {
            case World.Weathers.Blizzard:
                return WeatherTypes.Hailstorm;
            case World.Weathers.Snow:
                return WeatherTypes.Snow;
            case World.Weathers.Clear:
                return WeatherTypes.Clear;
            case World.Weathers.Fog:
                return WeatherTypes.Foggy;
            case World.Weathers.Rain:
            case World.Weathers.Thunderstorm:
                return WeatherTypes.Rain;
            case World.Weathers.Sandstorm:
                return WeatherTypes.Sandstorm;
            case World.Weathers.Sunny:
                return WeatherTypes.Sunny;
            case World.Weathers.Underwater:
                return WeatherTypes.Underwater;
            default:
                return WeatherTypes.Clear;
        }
    }
}
