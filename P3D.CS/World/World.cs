using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using P3D.BattleSystem;

namespace P3D;

public class World
{
    // Constants

    private const int REGION_WEATHER_WINTER_RAIN_UPPER = 40;
    private const int REGION_WEATHER_SPRING_RAIN_LOWER = 70;
    private const int REGION_WEATHER_SPRING_RAIN_UPPER = 90;
    private const int REGION_WEATHER_SUMMER_CLEAR_LOWER = 60;
    private const int REGION_WEATHER_SUMMER_SUNNY_UPPER = 95;
    private const int REGION_WEATHER_FALL_RAIN_LOWER = 60;
    private const int REGION_WEATHER_FALL_RAIN_UPPER = 90;

    private const int WINTER_NIGHT_HOUR = 18;
    private const int WINTER_MORNING_START = 6;
    private const int WINTER_MORNING_END = 11;
    private const int WINTER_DAY_END = 17;
    private const int SPRING_NIGHT_HOUR = 19;
    private const int SPRING_MORNING_START = 4;
    private const int SPRING_MORNING_END = 10;
    private const int SPRING_DAY_END = 17;
    private const int SPRING_EVENING_END = 20;
    private const int SUMMER_NIGHT_HOUR = 20;
    private const int SUMMER_MORNING_START = 3;
    private const int SUMMER_MORNING_END = 9;
    private const int SUMMER_DAY_END = 19;
    private const int SUMMER_EVENING_END = 21;
    private const int FALL_NIGHT_HOUR = 19;
    private const int FALL_MORNING_START = 5;
    private const int FALL_MORNING_END = 10;
    private const int FALL_DAY_END = 18;
    private const int FALL_EVENING_END = 20;

    private const int RENDER_DISTANCE_FOG_OFFSET = 3;
    private const float RENDER_DISTANCE_MAX_FOG_START = 999f;
    private const float RENDER_DISTANCE_MAX_FOG_END = 1000f;
    private const float RENDER_DISTANCE_MAX_FAR_PLANE = 1000f;
    private const int RENDER_DISTANCE_MAX = 4;

    // Static properties

    public static int setSeason = -1;
    public static int setDaytime = -1;

    public static bool IsMainMenu;
    public static bool IsAurora;

    private static Vector2 _weatherOffset = Vector2.Zero;
    private static List<Rectangle> _objectsList = [];
    private static Dictionary<Texture2D, Texture2D> _seasonTextureBuffer = [];
    private static Seasons _bufferSeason = Seasons.Fall;

    public static readonly Weathers[] NoParticlesList =
        [ Weathers.Clear, Weathers.Sunny, Weathers.Fog, Weathers.Mist ];

    // Server data

    public static Seasons ServerSeason = Seasons.Spring;
    public static Weathers ServerWeather = Weathers.Clear;
    public static String ServerTimeData = "0,0,0";
    public static DateTime LastServerDataReceived = DateTime.Now;

    // Enums

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

    // Instance properties

    public Weathers CurrentMapWeather { get; set; } = Weathers.Clear;
    public EnvironmentTypes EnvironmentType { get; set; } = EnvironmentTypes.Outside;
    public bool UseLighting { get; set; }

    // Constructor

    public World(int environmentType, int weatherType)
    {
        Initialize(environmentType, weatherType);
    }

    // Static properties

    public static int WeekOfYear
    {
        get
        {
            DateTime now = DateTime.Now;
            return ((now.DayOfYear - ((int)now.DayOfWeek - 1)) / 7) + 1;
        }
    }

    public static Seasons CurrentSeason
    {
        get
        {
            if (IsMainMenu == true)
            {
                return Seasons.Summer;
            }

            if (setSeason != -1)
            {
                return (Seasons)setSeason;
            }

            if (NeedServerObject() == true)
            {
                return ServerSeason;
            }

            switch (WeekOfYear % 4)
            {
                case 1:
                    return Seasons.Winter;
                case 2:
                    return Seasons.Spring;
                case 3:
                    return Seasons.Summer;
                case 0:
                    return Seasons.Fall;
            }

            return Seasons.Summer;
        }
    }

    public static DayTimes GetTime()
    {
        if (IsMainMenu == true)
        {
            return DayTimes.Day;
        }

        if (setDaytime != -1)
        {
            return (DayTimes)setDaytime;
        }

        DayTimes time = DayTimes.Day;
        int hour = DateTime.Now.Hour;

        if (NeedServerObject() == true)
        {
            String[] data = ServerTimeData.Split(',');
            hour = int.Parse(data[0]);
        }

        switch (CurrentSeason)
        {
            case Seasons.Winter:
                if (hour > WINTER_NIGHT_HOUR || hour < WINTER_MORNING_START)
                {
                    time = DayTimes.Night;
                }
                else if (hour > WINTER_MORNING_START - 1 && hour < WINTER_MORNING_END)
                {
                    time = DayTimes.Morning;
                }
                else if (hour > WINTER_MORNING_END - 1 && hour < WINTER_DAY_END)
                {
                    time = DayTimes.Day;
                }
                else if (hour > WINTER_DAY_END - 1 && hour < WINTER_NIGHT_HOUR + 1)
                {
                    time = DayTimes.Evening;
                }
                break;

            case Seasons.Spring:
                if (hour > SPRING_NIGHT_HOUR || hour < SPRING_MORNING_START)
                {
                    time = DayTimes.Night;
                }
                else if (hour > SPRING_MORNING_START - 1 && hour < SPRING_MORNING_END)
                {
                    time = DayTimes.Morning;
                }
                else if (hour > SPRING_MORNING_END - 1 && hour < SPRING_DAY_END)
                {
                    time = DayTimes.Day;
                }
                else if (hour > SPRING_DAY_END - 1 && hour < SPRING_EVENING_END)
                {
                    time = DayTimes.Evening;
                }
                break;

            case Seasons.Summer:
                if (hour > SUMMER_NIGHT_HOUR || hour < SUMMER_MORNING_START)
                {
                    time = DayTimes.Night;
                }
                else if (hour > SUMMER_MORNING_START - 1 && hour < SUMMER_MORNING_END)
                {
                    time = DayTimes.Morning;
                }
                else if (hour > SUMMER_MORNING_END - 1 && hour < SUMMER_DAY_END)
                {
                    time = DayTimes.Day;
                }
                else if (hour > SUMMER_DAY_END - 1 && hour < SUMMER_EVENING_END)
                {
                    time = DayTimes.Evening;
                }
                break;

            case Seasons.Fall:
                if (hour > FALL_NIGHT_HOUR || hour < FALL_MORNING_START)
                {
                    time = DayTimes.Night;
                }
                else if (hour > FALL_MORNING_START - 1 && hour < FALL_MORNING_END)
                {
                    time = DayTimes.Morning;
                }
                else if (hour > FALL_MORNING_END - 1 && hour < FALL_DAY_END)
                {
                    time = DayTimes.Day;
                }
                else if (hour > FALL_DAY_END - 1 && hour < FALL_EVENING_END)
                {
                    time = DayTimes.Evening;
                }
                break;
        }

        return time;
    }

    public static int SecondsOfDay
    {
        get
        {
            if (NeedServerObject() == true)
            {
                String[] data = ServerTimeData.Split(',');
                int hours = int.Parse(data[0]);
                int minutes = int.Parse(data[1]);
                int seconds = int.Parse(data[2]);

                seconds += (int)Math.Abs((DateTime.Now - LastServerDataReceived).TotalSeconds);

                return hours * 3600 + minutes * 60 + seconds;
            }
            else
            {
                DateTime now = DateTime.Now;
                return now.Hour * 3600 + now.Minute * 60 + now.Second;
            }
        }
    }

    public static int MinutesOfDay
    {
        get
        {
            if (NeedServerObject() == true)
            {
                String[] data = ServerTimeData.Split(',');
                int hours = int.Parse(data[0]);
                int minutes = int.Parse(data[1]);

                minutes += (int)Math.Abs((DateTime.Now - LastServerDataReceived).TotalMinutes);

                return hours * 60 + minutes;
            }
            else
            {
                DateTime now = DateTime.Now;
                return now.Hour * 60 + now.Minute;
            }
        }
    }

    public static Weathers RegionWeather { get; set; } = Weathers.Clear;
    public static bool RegionWeatherSet { get; set; }

    // Static methods

    public static Weathers GetRegionWeather(Seasons season)
    {
        if (IsMainMenu == true)
        {
            return Weathers.Clear;
        }

        int r = Core.Random.Next(0, 100);

        switch (season)
        {
            case Seasons.Winter:
                if (r < 30)
                {
                    return Weathers.Clear;
                }
                else if (r >= 30 && r < REGION_WEATHER_WINTER_RAIN_UPPER)
                {
                    return Weathers.Rain;
                }
                else
                {
                    return Weathers.Snow;
                }

            case Seasons.Spring:
                if (r < REGION_WEATHER_SPRING_RAIN_LOWER)
                {
                    return Weathers.Clear;
                }
                else if (r >= REGION_WEATHER_SPRING_RAIN_LOWER && r < REGION_WEATHER_SPRING_RAIN_UPPER)
                {
                    return Weathers.Rain;
                }
                else
                {
                    return Weathers.Snow;
                }

            case Seasons.Summer:
                if (r < REGION_WEATHER_SUMMER_CLEAR_LOWER)
                {
                    return Weathers.Clear;
                }
                else if (r >= REGION_WEATHER_SUMMER_CLEAR_LOWER && r < REGION_WEATHER_SUMMER_SUNNY_UPPER)
                {
                    DayTimes dayTime = Screen.Level != null ? (DayTimes)(Screen.Level.DayTime - 1) : DayTimes.Day;
                    switch (dayTime)
                    {
                        case DayTimes.Day:
                        case DayTimes.Morning:
                            return Weathers.Sunny;
                        default:
                            return Weathers.Clear;
                    }
                }
                else
                {
                    return Weathers.Rain;
                }

            case Seasons.Fall:
                if (r < REGION_WEATHER_FALL_RAIN_LOWER)
                {
                    return Weathers.Clear;
                }
                else if (r >= REGION_WEATHER_FALL_RAIN_LOWER && r < REGION_WEATHER_FALL_RAIN_UPPER)
                {
                    return Weathers.Rain;
                }
                else
                {
                    return Weathers.Snow;
                }
        }

        return Weathers.Clear;
    }

    public static Weathers GetWeatherFromWeatherType(int weatherType)
    {
        if (IsMainMenu == true)
        {
            return Weathers.Clear;
        }

        switch (weatherType)
        {
            case 0:
                return GetCurrentRegionWeather();
            case 1:
                return Weathers.Clear;
            case 2:
                return Weathers.Rain;
            case 3:
                return Weathers.Snow;
            case 4:
                return Weathers.Underwater;
            case 5:
                return Weathers.Sunny;
            case 6:
                return Weathers.Fog;
            case 7:
                return Weathers.Sandstorm;
            case 8:
                return Weathers.Ash;
            case 9:
                return Weathers.Blizzard;
            case 10:
                return Weathers.Thunderstorm;
            case 11:
                return Weathers.Mist;
        }

        return Weathers.Clear;
    }

    public static int GetWeatherTypeFromWeather(Weathers weather)
    {
        if (IsMainMenu == true)
        {
            return 1;
        }

        switch (weather)
        {
            case Weathers.Clear:
                return 1;
            case Weathers.Rain:
                return 2;
            case Weathers.Snow:
                return 3;
            case Weathers.Underwater:
                return 4;
            case Weathers.Sunny:
                return 5;
            case Weathers.Fog:
                return 6;
            case Weathers.Sandstorm:
                return 7;
            case Weathers.Ash:
                return 8;
            case Weathers.Blizzard:
                return 9;
            case Weathers.Thunderstorm:
                return 10;
            case Weathers.Mist:
                return 11;
            default:
                return 0;
        }
    }

    public static void SetRenderDistance(EnvironmentTypes environmentType, Weathers weather)
    {
        SetRenderDistanceCore(environmentType, weather);
        Screen.Camera.CreateNewProjection(Screen.Camera.FOV);
    }

    private static void SetRenderDistanceCore(EnvironmentTypes environmentType, Weathers weather)
    {
        if (Screen.Effect == null)
        {
            return;
        }

        if (weather == Weathers.Fog)
        {
            Screen.Effect.FogStart = -40;
            Screen.Effect.FogEnd = 12;
            Screen.Camera.FarPlane = 15;
            return;
        }

        if (weather == Weathers.Blizzard)
        {
            Screen.Effect.FogStart = -40;
            Screen.Effect.FogEnd = 18;
            Screen.Camera.FarPlane = 24;
            return;
        }

        if (weather == Weathers.Thunderstorm)
        {
            Screen.Effect.FogStart = -40;
            Screen.Effect.FogEnd = 20;
            Screen.Camera.FarPlane = 24;
            return;
        }

        if (weather == Weathers.Mist)
        {
            Screen.Effect.FogStart = 0;
            Screen.Effect.FogEnd = 19;
            Screen.Camera.FarPlane = 20;
            return;
        }

        switch (environmentType)
        {
            case EnvironmentTypes.Cave:
            case EnvironmentTypes.Dark:
            case EnvironmentTypes.Forest:
                ApplyRenderDistanceBands(
                    [ (-2, 19, 20), (-2, 39, 40), (-2, 59, 60), (-5, 79, 80), (-20, 99, 100) ]);
                break;

            case EnvironmentTypes.Inside:
                ApplyRenderDistanceBands(
                    [ (16, 19, 20), (36, 39, 40), (56, 59, 60), (76, 79, 80), (96, 99, 100) ]);
                break;

            case EnvironmentTypes.Outside:
                ApplyOutdoorRenderDistance();
                break;

            case EnvironmentTypes.Underwater:
                ApplyRenderDistanceBands(
                    [ (0, 19, 20), (0, 39, 40), (0, 59, 60), (0, 79, 80), (0, 99, 100) ]);
                break;
        }

        if (Core.GameOptions.RenderDistance >= RENDER_DISTANCE_MAX)
        {
            Screen.Effect.FogStart = RENDER_DISTANCE_MAX_FOG_START;
            Screen.Effect.FogEnd = RENDER_DISTANCE_MAX_FOG_END;
            Screen.Camera.FarPlane = RENDER_DISTANCE_MAX_FAR_PLANE;
        }
    }

    private static void ApplyRenderDistanceBands((float fogStart, float fogEnd, float farPlane)[] bands)
    {
        if (Screen.Effect == null)
        {
            return;
        }

        int index = Math.Clamp(Core.GameOptions.RenderDistance, 0, bands.Length - 1);
        Screen.Effect.FogStart = bands[index].fogStart;
        Screen.Effect.FogEnd = bands[index].fogEnd;
        Screen.Camera.FarPlane = bands[index].farPlane;
    }

    private static void ApplyOutdoorRenderDistance()
    {
        if (Screen.Effect == null || Screen.Level == null)
        {
            return;
        }

        (float, float, float)[] nightBands =
            [ (-2, 19, 20), (-2, 39, 40), (-2, 59, 60), (-5, 79, 80), (-20, 99, 100) ];
        (float, float, float)[] morningDayBands =
            [ (16, 19, 20), (36, 39, 40), (56, 59, 60), (76, 79, 80), (96, 99, 100) ];
        (float, float, float)[] eveningBands =
            [ (0, 19, 20), (0, 39, 40), (0, 59, 60), (0, 79, 80), (0, 99, 100) ];

        (float, float, float)[] selected;

        switch ((DayTimes)(Screen.Level.DayTime - 1))
        {
            case DayTimes.Night:
                selected = nightBands;
                break;
            case DayTimes.Morning:
            case DayTimes.Day:
                selected = morningDayBands;
                break;
            case DayTimes.Evening:
                selected = eveningBands;
                break;
            default:
                selected = morningDayBands;
                break;
        }

        ApplyRenderDistanceBands(selected);
    }

    public static void DrawWeather(Weathers mapWeather)
    {
        if (NoParticlesList.Contains(mapWeather) == true)
        {
            return;
        }

        if (Core.GameOptions.GraphicStyle == 1)
        {
            Screen.Identifications[] validScreens =
            [
                Screen.Identifications.OverworldScreen,
                Screen.Identifications.MainMenuScreen,
                Screen.Identifications.BattleScreen,
                Screen.Identifications.BattleCatchScreen
            ];

            if (validScreens.Contains(Core.CurrentScreen.Identification) == false)
            {
                return;
            }

            if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
            {
                if (Screen.TextBox.Showing == false)
                {
                    GenerateParticles(0, mapWeather);
                }
            }
            else
            {
                GenerateParticles(0, mapWeather);
            }
        }
        else
        {
            Texture2D? t = null;
            int size = 128;
            int opacity = 30;

            switch (mapWeather)
            {
                case Weathers.Rain:
                    t = TextureManager.GetTexture(@"Textures\Weather\rain");
                    _weatherOffset.X += 8;
                    _weatherOffset.Y += 16;
                    break;
                case Weathers.Thunderstorm:
                    t = TextureManager.GetTexture(@"Textures\Weather\rain");
                    _weatherOffset.X += 12;
                    _weatherOffset.Y += 20;
                    opacity = 50;
                    break;
                case Weathers.Snow:
                    t = TextureManager.GetTexture(@"Textures\Weather\snow");
                    _weatherOffset.X += 1;
                    _weatherOffset.Y += 1;
                    break;
                case Weathers.Blizzard:
                    t = TextureManager.GetTexture(@"Textures\Weather\snow");
                    _weatherOffset.X += 8;
                    _weatherOffset.Y += 2;
                    opacity = 80;
                    break;
                case Weathers.Sandstorm:
                    t = TextureManager.GetTexture(@"Textures\Weather\sand");
                    _weatherOffset.X += 4;
                    _weatherOffset.Y += 1;
                    opacity = 80;
                    size = 48;
                    break;
                case Weathers.Underwater:
                    t = TextureManager.GetTexture(@"Textures\Weather\bubble");
                    if (Core.Random.Next(0, 100) == 0)
                    {
                        _objectsList.Add(new Rectangle(
                            Core.Random.Next(0, Math.Max(1, Core.windowSize.Width - 32)),
                            Core.windowSize.Height, 32, 32));
                    }
                    for (int i = 0; i <= _objectsList.Count - 1; i++)
                    {
                        Rectangle r = _objectsList[i];
                        _objectsList[i] = new Rectangle(r.X, r.Y - 2, r.Width, r.Height);
                        Core.SpriteBatch.Draw(t, _objectsList[i], new Color(255, 255, 255, 150));
                    }
                    break;
                case Weathers.Ash:
                    t = TextureManager.GetTexture(@"Textures\Weather\ash2");
                    _weatherOffset.Y += 1;
                    opacity = 65;
                    size = 48;
                    break;
            }

            if (_weatherOffset.X >= size)
            {
                _weatherOffset.X = 0;
            }
            if (_weatherOffset.Y >= size)
            {
                _weatherOffset.Y = 0;
            }

            switch (mapWeather)
            {
                case Weathers.Rain:
                case Weathers.Snow:
                case Weathers.Sandstorm:
                case Weathers.Ash:
                case Weathers.Blizzard:
                case Weathers.Thunderstorm:
                    if (t != null)
                    {
                        for (int x = -size; x <= Core.windowSize.Width; x += size)
                        {
                            for (int y = -size; y <= Core.windowSize.Height; y += size)
                            {
                                Core.SpriteBatch.Draw(t,
                                    new Rectangle((int)(x + _weatherOffset.X), (int)(y + _weatherOffset.Y), size, size),
                                    new Color(255, 255, 255, opacity));
                            }
                        }
                    }
                    break;
            }
        }
    }

    public static void GenerateParticles(int chance, Weathers mapWeather)
    {
        if (mapWeather == Weathers.Thunderstorm)
        {
            if (Core.Random.Next(0, 250) == 0)
            {
                float pitch = -(Core.Random.Next(8, 11) / 10.0f);
                SoundManager.PlaySound(@"Battle\Attacks\Electric\Thunderbolt", pitch, 0f, SoundManager.Volume, false);
            }
        }

        if (LevelLoader.IsBusy == true)
        {
            return;
        }

        Screen.Identifications[] validScreens =
        [
            Screen.Identifications.OverworldScreen,
            Screen.Identifications.BattleScreen,
            Screen.Identifications.BattleCatchScreen,
            Screen.Identifications.MainMenuScreen
        ];

        if (validScreens.Contains(Core.CurrentScreen.Identification) == false)
        {
            return;
        }

        if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
        {
            if (((OverworldScreen)Core.CurrentScreen).ActionScript.IsReady == false)
            {
                return;
            }
        }

        Texture2D? t = null;
        float speed = 0.0f;
        Vector3 scale = Vector3.One;
        int range = 3;

        switch (mapWeather)
        {
            case Weathers.Rain:
                speed = 0.1f;
                t = TextureManager.GetTexture(@"Textures\Weather\rain3");
                if (chance > -1)
                {
                    chance = 3;
                }
                scale = new Vector3(0.03f, 0.06f, 0.1f);
                break;
            case Weathers.Thunderstorm:
                speed = 0.15f;
                switch (Core.Random.Next(0, 4))
                {
                    case 0:
                        t = TextureManager.GetTexture(@"Textures\Weather\rain2");
                        scale = new Vector3(0.1f, 0.1f, 0.1f);
                        break;
                    default:
                        t = TextureManager.GetTexture(@"Textures\Weather\rain3");
                        scale = new Vector3(0.03f, 0.06f, 0.1f);
                        break;
                }
                if (chance > -1)
                {
                    chance = 1;
                }
                break;
            case Weathers.Snow:
                speed = 0.02f;
                t = TextureManager.GetTexture(@"Textures\Weather\snow2");
                if (chance > -1)
                {
                    chance = 5;
                }
                scale = new Vector3(0.03f, 0.03f, 0.1f);
                break;
            case Weathers.Underwater:
                speed = -0.02f;
                t = TextureManager.GetTexture(@"Textures\Weather\bubble");
                if (chance > -1)
                {
                    chance = 60;
                }
                scale = new Vector3(0.5f);
                range = 1;
                break;
            case Weathers.Sandstorm:
                speed = 0.1f;
                t = TextureManager.GetTexture(@"Textures\Weather\sand");
                if (chance > -1)
                {
                    chance = 4;
                }
                scale = new Vector3(0.03f, 0.03f, 0.1f);
                break;
            case Weathers.Ash:
                speed = 0.02f;
                t = TextureManager.GetTexture(@"Textures\Weather\ash");
                if (chance > -1)
                {
                    chance = 20;
                }
                scale = new Vector3(0.03f, 0.03f, 0.1f);
                break;
            case Weathers.Blizzard:
                speed = 0.1f;
                t = TextureManager.GetTexture(@"Textures\Weather\snow");
                if (chance > -1)
                {
                    chance = 1;
                }
                scale = new Vector3(0.12f, 0.12f, 0.1f);
                break;
        }

        if (chance == -1)
        {
            chance = 1;
        }

        Vector3 cameraPosition = Screen.Camera.Position;
        if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
        {
            cameraPosition = ((OverworldCamera)Screen.Camera).CPosition;
        }
        else if (Core.CurrentScreen.Identification == Screen.Identifications.BattleScreen)
        {
            cameraPosition = ((BattleCamera)Screen.Camera).CPosition;
        }

        if (Core.Random.Next(0, chance) != 0 || t == null || Screen.Level == null)
        {
            return;
        }

        for (float px = cameraPosition.X - range; px <= cameraPosition.X + range; px++)
        {
            for (float pz = cameraPosition.Z - range; pz <= cameraPosition.Z + range; pz++)
            {
                if (pz != 0 || px != 0)
                {
                    float rY = (float)(Core.Random.Next(0, 40) / 10.0) - 2.0f;
                    float rX = (float)Core.Random.NextDouble() - 0.5f;
                    float rZ = (float)Core.Random.NextDouble() - 0.5f;
                    Particle p = new Particle(
                        new Vector3(px + rX, cameraPosition.Y + 1.8f + rY, pz + rZ),
                        [t],
                        [0, 0],
                        Core.Random.Next(0, 2),
                        scale,
                        BaseModel.BillModel,
                        Vector3.One);
                    p.MoveSpeed = speed;

                    if (mapWeather == Weathers.Rain)
                    {
                        p.Opacity = 0.7f;
                    }
                    if (mapWeather == Weathers.Thunderstorm)
                    {
                        p.Opacity = 1.0f;
                    }
                    if (mapWeather == Weathers.Underwater)
                    {
                        p.Position = new Vector3(p.Position.X, 0.0f, p.Position.Z);
                        p.Destination = 10;
                        p.Behavior = Particle.Behaviors.Rising;
                    }
                    if (mapWeather == Weathers.Sandstorm)
                    {
                        p.Behavior = Particle.Behaviors.LeftToRight;
                        p.Destination = cameraPosition.X + 5;
                        p.Position = new Vector3(p.Position.X - 2, p.Position.Y, p.Position.Z);
                    }
                    if (mapWeather == Weathers.Blizzard)
                    {
                        p.Opacity = 1.0f;
                    }

                    Screen.Level.Entities.Add(p);
                }
            }
        }
    }

    public static Texture2D? GetSeasonTexture(Texture2D seasonTexture, Texture2D t)
    {
        if (_bufferSeason != CurrentSeason)
        {
            _bufferSeason = CurrentSeason;
            _seasonTextureBuffer.Clear();
        }

        if (t == null)
        {
            return null;
        }

        if (_seasonTextureBuffer.ContainsKey(t) == true)
        {
            return _seasonTextureBuffer[t];
        }

        int x = 0;
        int y = 0;

        switch (CurrentSeason)
        {
            case Seasons.Winter:
                x = 0;
                y = 0;
                break;
            case Seasons.Spring:
                x = 2;
                y = 0;
                break;
            case Seasons.Summer:
                x = 0;
                y = 2;
                break;
            case Seasons.Fall:
                x = 2;
                y = 2;
                break;
        }

        Color[] inputColors =
        [
            new Color(255, 255, 255),
            new Color(170, 170, 170),
            new Color(85, 85, 85),
            new Color(0, 0, 0)
        ];

        Color[] data = new Color[4];
        seasonTexture.GetData(0, new Rectangle(x, y, 2, 2), data, 0, 4);

        Texture2D result = t.ReplaceColors(inputColors, data);
        _seasonTextureBuffer.Add(t, result);
        return result;
    }

    public static Weathers GetCurrentRegionWeather()
    {
        if (NeedServerObject() == true)
        {
            return ServerWeather;
        }
        return RegionWeather;
    }

    public static bool IsNight()
    {
        DayTimes currentTime = GetTime();
        return currentTime.Equals(DayTimes.Night) || currentTime.Equals(DayTimes.Evening);
    }

    // Instance methods

    public void Initialize(int environmentType, int weatherType)
    {
        if (RegionWeatherSet == false)
        {
            RegionWeather = GetRegionWeather(CurrentSeason);
            RegionWeatherSet = true;
        }

        CurrentMapWeather = GetWeatherFromWeatherType(weatherType);

        switch (environmentType)
        {
            case 0:
                EnvironmentType = EnvironmentTypes.Outside;
                UseLighting = true;
                break;
            case 1:
                EnvironmentType = EnvironmentTypes.Inside;
                UseLighting = false;
                break;
            case 2:
                EnvironmentType = EnvironmentTypes.Cave;
                if (weatherType == 0)
                {
                    CurrentMapWeather = Weathers.Clear;
                }
                UseLighting = false;
                break;
            case 3:
                EnvironmentType = EnvironmentTypes.Dark;
                if (weatherType == 0)
                {
                    CurrentMapWeather = Weathers.Clear;
                }
                UseLighting = false;
                break;
            case 4:
                EnvironmentType = EnvironmentTypes.Underwater;
                if (weatherType == 0)
                {
                    CurrentMapWeather = Weathers.Underwater;
                }
                UseLighting = true;
                break;
            case 5:
                EnvironmentType = EnvironmentTypes.Forest;
                UseLighting = true;
                break;
        }

        SetWeatherLevelColor();
        ChangeEnvironment();
        SetRenderDistance(EnvironmentType, CurrentMapWeather);
    }

    private void SetWeatherLevelColor()
    {
        if (Screen.Effect == null || Screen.SkyDome == null)
        {
            return;
        }

        switch (CurrentMapWeather)
        {
            case Weathers.Clear:
                Screen.Effect.DiffuseColor = new Vector3(1);
                break;
            case Weathers.Rain:
            case Weathers.Thunderstorm:
                Screen.Effect.DiffuseColor = new Vector3(0.7f, 0.7f, 0.8f);
                break;
            case Weathers.Snow:
                Screen.Effect.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);
                break;
            case Weathers.Underwater:
                Screen.Effect.DiffuseColor = new Vector3(0.1f, 0.3f, 0.9f);
                break;
            case Weathers.Sunny:
                Screen.Effect.DiffuseColor = new Vector3(1.2f, 1.1f, 1.1f);
                break;
            case Weathers.Fog:
                Screen.Effect.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
                break;
            case Weathers.Sandstorm:
                Screen.Effect.DiffuseColor = new Vector3(0.8f, 0.5f, 0.2f);
                break;
            case Weathers.Ash:
                Screen.Effect.DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f);
                break;
            case Weathers.Blizzard:
                Screen.Effect.DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f);
                break;
        }

        Screen.Effect.DiffuseColor = Screen.SkyDome.GetWeatherColorMultiplier(Screen.Effect.DiffuseColor);
    }

    private Color GetWeatherBackgroundColor(Color defaultColor)
    {
        Vector3 v = Vector3.One;

        switch (CurrentMapWeather)
        {
            case Weathers.Clear:
            case Weathers.Sunny:
                v = new Vector3(1);
                break;
            case Weathers.Rain:
            case Weathers.Thunderstorm:
                v = new Vector3(0.7f);
                break;
            case Weathers.Snow:
                v = new Vector3(0.8f);
                break;
            case Weathers.Underwater:
                v = new Vector3(0.1f, 0.3f, 0.9f);
                break;
            case Weathers.Fog:
                v = new Vector3(0.7f);
                break;
            case Weathers.Sandstorm:
                v = new Vector3(0.8f, 0.5f, 0.2f);
                break;
            case Weathers.Ash:
                v = new Vector3(0.5f);
                break;
            case Weathers.Blizzard:
                v = new Vector3(0.6f);
                break;
        }

        if (Screen.SkyDome == null)
        {
            return defaultColor;
        }

        Vector3 colorV = defaultColor.ToVector3() * Screen.SkyDome.GetWeatherColorMultiplier(v);
        return colorV.ToColor();
    }

    private void ChangeEnvironment()
    {
        if (Screen.Effect == null || Screen.SkyDome == null)
        {
            return;
        }

        switch (EnvironmentType)
        {
            case EnvironmentTypes.Outside:
                Vector3 multiplier = new Vector3(1.0f);
                switch (CurrentMapWeather)
                {
                    case Weathers.Clear:
                    case Weathers.Sunny:
                    case Weathers.Fog:
                        multiplier = new Vector3(1.0f);
                        break;
                    case Weathers.Rain:
                    case Weathers.Thunderstorm:
                        multiplier = new Vector3(0.7f);
                        break;
                    case Weathers.Snow:
                        multiplier = new Vector3(0.8f);
                        break;
                }
                Core.BackgroundColor = (Lighting.GetEnvironmentColor(2) * multiplier).ToColor();
                Screen.Effect.FogColor = Lighting.GetEnvironmentColor(2) * multiplier;
                Screen.SkyDome.TextureDown = TextureManager.GetTexture(@"SkyDomeResource\Stars");
                break;

            case EnvironmentTypes.Inside:
                Core.BackgroundColor = Lighting.GetEnvironmentColor(2).ToColor();
                Screen.Effect.FogColor = Lighting.GetEnvironmentColor(2);
                Screen.SkyDome.TextureUp = TextureManager.GetTexture(@"SkyDomeResource\Inside");
                Screen.SkyDome.TextureDown = null;
                break;

            case EnvironmentTypes.Dark:
                Core.BackgroundColor = Lighting.GetEnvironmentColor(2).ToColor();
                Screen.Effect.FogColor = Lighting.GetEnvironmentColor(2);
                Screen.SkyDome.TextureUp = TextureManager.GetTexture(@"SkyDomeResource\Dark");
                Screen.SkyDome.TextureDown = null;
                break;

            case EnvironmentTypes.Cave:
                Core.BackgroundColor = Lighting.GetEnvironmentColor(2).ToColor();
                Screen.Effect.FogColor = Lighting.GetEnvironmentColor(2);
                Screen.SkyDome.TextureUp = TextureManager.GetTexture(@"SkyDomeResource\Cave");
                Screen.SkyDome.TextureDown = null;
                break;

            case EnvironmentTypes.Underwater:
                Core.BackgroundColor = Lighting.GetEnvironmentColor(2).ToColor();
                Screen.Effect.FogColor = Lighting.GetEnvironmentColor(2);
                Screen.SkyDome.TextureUp = TextureManager.GetTexture(@"SkyDomeResource\Underwater");
                Screen.SkyDome.TextureDown = null;
                break;

            case EnvironmentTypes.Forest:
                Core.BackgroundColor = Lighting.GetEnvironmentColor(2).ToColor();
                Screen.Effect.FogColor = Lighting.GetEnvironmentColor(2);
                Screen.SkyDome.TextureUp = TextureManager.GetTexture(@"SkyDomeResource\Forest");
                Screen.SkyDome.TextureDown = null;
                break;
        }
    }

    // Private helpers

    private static bool NeedServerObject()
    {
        return JoinServerScreen.Online == true && ConnectScreen.Connected == true;
    }
}
