using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class SkyDome
{
    private const bool FAST_TIME_CYCLE = false;

    private Model _skydomeModel;
    public Texture2D? TextureUp { get; set; }
    public Texture2D? TextureDown { get; set; }
    private Texture2D _textureSun;
    private Texture2D _textureMoon;

    public float Yaw { get; set; } = 0f;

    private int _hour = 0;
    private int _minute = 0;
    private int _second = 0;

    private static Color[]? _daycycleTextureData;
    private static Texture2D? _daycycleTexture;
    private static Color _lastSkyColor = new Color(0, 0, 0, 0);
    private static Color _lastEntityColor = new Color(0, 0, 0, 0);

    public SkyDome()
    {
        _skydomeModel = Core.Content.Load<Model>("SkyDomeResource/SkyDome");

        TextureUp = TextureManager.GetTexture(@"SkyDomeResource\Sky_Day");
        TextureDown = TextureManager.GetTexture(@"SkyDomeResource\Stars");
        _textureSun = TextureManager.GetTexture(@"SkyDomeResource\sun");
        _textureMoon = TextureManager.GetTexture(@"SkyDomeResource\moon");

        SetLastColor();
    }

    public void Update()
    {
        Yaw += 0.0001f;
        while (Yaw > MathHelper.TwoPi)
        {
            Yaw -= MathHelper.TwoPi;
        }
        SetLastColor();

        if (FAST_TIME_CYCLE == true)
        {
            _second += 60;
            if (_second == 60)
            {
                _second = 0;
                _minute += 1;
                if (_minute == 60)
                {
                    _minute = 0;
                    _hour += 1;
                    if (_hour == 24)
                    {
                        _hour = 0;
                    }
                }
            }
        }
    }

    public void Draw(float fov)
    {
        if (Screen.Level.World.EnvironmentType == World.EnvironmentTypes.Outside)
        {
            bool drawSky;
            switch (World.GetWeatherFromWeatherType(Screen.Level.WeatherType))
            {
                case World.Weathers.Fog:
                case World.Weathers.Blizzard:
                case World.Weathers.Thunderstorm:
                    drawSky = false;
                    break;
                default:
                    drawSky = true;
                    break;
            }
            if (drawSky == true)
            {
                RenderHalf(fov, Yaw, 0.0f, true, GetSkyTexture(), 20, 1.0f);
                RenderHalf(fov, MathHelper.TwoPi, 0.0f, true, TextureDown, 18, GetStarsAlpha());
                if (GetSunAlpha() > 0)
                {
                    RenderHalf(fov, MathHelper.TwoPi, 0.0f, true, _textureSun, 16, 1.0f);
                }
                else
                {
                    RenderHalf(fov, MathHelper.TwoPi, 0.0f, true, _textureMoon, 16, 1.0f);
                }
            }
            if (World.GetWeatherFromWeatherType(Screen.Level.WeatherType) != World.Weathers.Fog)
            {
                RenderHalf(fov, MathHelper.TwoPi - Yaw * 2, 0.0f, true, GetCloudsTexture(), 12, GetCloudAlpha());
            }
        }
        else
        {
            if (Screen.Level.World.EnvironmentType == World.EnvironmentTypes.Cave ||
                Screen.Level.World.EnvironmentType == World.EnvironmentTypes.Forest)
            {
                RenderHalf(fov, MathHelper.TwoPi, 0.0f, true, TextureUp, 20, 1.0f);
            }
            else
            {
                RenderHalf(fov, Yaw, 0.0f, true, TextureUp, 16, 1.0f);
                RenderHalf(fov, MathHelper.TwoPi, 0.0f, true, _textureSun, 12, GetSunAlpha());
                RenderHalf(fov, MathHelper.TwoPi - Yaw, 0.0f, true,
                    TextureManager.GetTexture(@"SkyDomeResource\Clouds_Day"), 8, GetCloudAlpha());
            }
            if (TextureDown != null)
            {
                RenderHalf(fov, Yaw, 0.0f, false, TextureDown, 16, 1.0f);
            }
        }
    }

    private void RenderHalf(float fov, float useYaw, float usePitch, bool up, Texture2D? texture, float scale, float alpha)
    {
        float roll = 0.0f;
        if (up == false)
        {
            roll = MathHelper.Pi;
        }

        BlendState previousBlendState = Core.GraphicsDevice.BlendState;
        Core.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

        foreach (ModelMesh modelMesh in _skydomeModel.Meshes)
        {
            foreach (BasicEffect basicEffect in modelMesh.Effects)
            {
                basicEffect.World = Matrix.CreateScale(scale)
                    * Matrix.CreateTranslation(new Vector3(
                        Screen.Camera.Position.X,
                        Screen.Camera.Position.Y - 2,
                        Screen.Camera.Position.Z))
                    * Matrix.CreateFromYawPitchRoll(useYaw, usePitch, roll);

                basicEffect.View = Screen.Camera.View;
                basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(fov),
                    Core.GraphicsDevice.Viewport.AspectRatio,
                    0.01f, 10000f);

                basicEffect.TextureEnabled = true;
                basicEffect.Texture = texture;
                basicEffect.Alpha = alpha;

                if (basicEffect.Texture == TextureDown)
                {
                    basicEffect.DiffuseColor = new Vector3(1);
                }
                else
                {
                    switch (Screen.Level.World.CurrentMapWeather)
                    {
                        case World.Weathers.Clear:
                            basicEffect.DiffuseColor = new Vector3(1);
                            break;
                        case World.Weathers.Sunny:
                            basicEffect.DiffuseColor = new Vector3(1.2f, 1.1f, 1.1f);
                            break;
                        case World.Weathers.Rain:
                        case World.Weathers.Thunderstorm:
                            basicEffect.DiffuseColor = new Vector3(0.7f, 0.7f, 0.8f);
                            break;
                        case World.Weathers.Snow:
                            basicEffect.DiffuseColor = new Vector3(0.9f);
                            break;
                        case World.Weathers.Underwater:
                            basicEffect.DiffuseColor = new Vector3(0.1f, 0.3f, 0.9f);
                            break;
                        case World.Weathers.Fog:
                            basicEffect.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
                            break;
                        case World.Weathers.Sandstorm:
                            basicEffect.DiffuseColor = new Vector3(0.8f, 0.5f, 0.2f);
                            break;
                        case World.Weathers.Ash:
                        case World.Weathers.Blizzard:
                            basicEffect.DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f);
                            break;
                    }
                }

                if (basicEffect.DiffuseColor != new Vector3(1))
                {
                    basicEffect.DiffuseColor = GetWeatherColorMultiplier(basicEffect.DiffuseColor);
                }
            }

            modelMesh.Draw();
        }

        Core.GraphicsDevice.BlendState = previousBlendState;
    }

    public static Color GetDaytimeColor(bool shader)
    {
        if (shader == true)
        {
            return _lastEntityColor;
        }
        else
        {
            if (World.IsAurora == true)
            {
                return new Color(64, 101, 164);
            }
            switch (Screen.Level.DayTime)
            {
                case 1:
                    return new Color(40, 88, 136);
                case 2:
                    return new Color(168, 224, 248);
                case 3:
                    return new Color(48, 200, 248);
                case 4:
                    return new Color(192, 152, 184);
                default:
                    return Color.White;
            }
        }
    }

    private void SetLastColor()
    {
        if (_daycycleTextureData == null)
        {
            Texture2D daycycleTexture = TextureManager.GetTexture(@"SkyDomeResource\daycycle");
            _daycycleTextureData = new Color[daycycleTexture.Width * daycycleTexture.Height];
            daycycleTexture.GetData(_daycycleTextureData);
            _daycycleTexture = daycycleTexture;
        }

        int pixel = GetTimeValue();
        Color pixelColor = _daycycleTextureData[pixel];
        if (pixelColor != _lastSkyColor)
        {
            _lastSkyColor = pixelColor;
            int clampedIndex = Math.Clamp(pixel + _daycycleTexture!.Width, 0, _daycycleTexture.Width * _daycycleTexture.Height - 1);
            _lastEntityColor = _daycycleTextureData[clampedIndex];
        }
    }

    private float GetCloudAlpha()
    {
        if (Screen.Level.World.EnvironmentType == (int)World.EnvironmentTypes.Outside && World.IsAurora == false)
        {
            switch (World.GetWeatherFromWeatherType(Screen.Level.WeatherType))
            {
                case World.Weathers.Blizzard:
                case World.Weathers.Thunderstorm:
                    return 0.25f;
                default:
                    return 1.0f;
            }
        }
        else
        {
            return 0.0f;
        }
    }

    private float GetStarsAlpha()
    {
        if (Screen.Level.World.EnvironmentType == (int)World.EnvironmentTypes.Outside && World.IsAurora == false)
        {
            switch (Screen.Level.DayTime)
            {
                case 1:
                    return 1.0f;
                default:
                    return 0.0f;
            }
        }
        else
        {
            return 0.0f;
        }
    }

    private float GetSunAlpha()
    {
        if (Screen.Level.World.EnvironmentType == (int)World.EnvironmentTypes.Outside && World.IsAurora == false)
        {
            switch (Screen.Level.DayTime)
            {
                case 1:
                    return 0.0f;
                case 2:
                case 3:
                    return 1.0f;
                default:
                    return 0.0f;
            }
        }
        else
        {
            return 0.0f;
        }
    }

    private Texture2D? GetCloudsTexture()
    {
        switch (Screen.Level.World.CurrentMapWeather)
        {
            case World.Weathers.Rain:
            case World.Weathers.Blizzard:
            case World.Weathers.Thunderstorm:
            case World.Weathers.Snow:
                return TextureManager.GetTexture(@"SkyDomeResource\Clouds_Weather");
            case World.Weathers.Clear:
                switch (Screen.Level.DayTime)
                {
                    case 1:
                        return TextureManager.GetTexture(@"SkyDomeResource\Clouds_Night");
                    case 2:
                        return TextureManager.GetTexture(@"SkyDomeResource\Clouds_Morning");
                    case 3:
                        return TextureManager.GetTexture(@"SkyDomeResource\Clouds_Day");
                    case 4:
                        return TextureManager.GetTexture(@"SkyDomeResource\Clouds_Evening");
                    default:
                        break;
                }
                break;
        }
        return null;
    }

    private Texture2D? GetSkyTexture()
    {
        if (World.IsAurora == true)
        {
            return TextureManager.GetTexture(@"SkyDomeResource\AuroraBorealis");
        }
        switch (Screen.Level.DayTime)
        {
            case 1:
                return TextureManager.GetTexture(@"SkyDomeResource\Sky_Night");
            case 2:
                return TextureManager.GetTexture(@"SkyDomeResource\Sky_Morning");
            case 3:
                return TextureManager.GetTexture(@"SkyDomeResource\Sky_Day");
            case 4:
                return TextureManager.GetTexture(@"SkyDomeResource\Sky_Evening");
        }
        switch (Screen.Level.World.CurrentMapWeather)
        {
            case World.Weathers.Clear:
                World.DayTimes time = World.GetTime();
                switch (time)
                {
                    case World.DayTimes.Morning:
                        return TextureManager.GetTexture(@"SkyDomeResource\Sky_Morning");
                    case World.DayTimes.Day:
                        return TextureManager.GetTexture(@"SkyDomeResource\Sky_Day");
                    case World.DayTimes.Evening:
                        return TextureManager.GetTexture(@"SkyDomeResource\Sky_Evening");
                    default:
                        return TextureManager.GetTexture(@"SkyDomeResource\Sky_Night");
                }
        }
        return TextureUp;
    }

    public Vector3 GetWeatherColorMultiplier(Vector3 v)
    {
        int progress = GetTimeValue();

        float p;
        if (progress < 720)
        {
            p = (720 - progress) / 720f;
        }
        else
        {
            p = (progress - 720) / 720f;
        }

        return new Vector3(
            v.X + ((1 - v.X) * p),
            v.Y + ((1 - v.Y) * p),
            v.Z + ((1 - v.Z) * p));
    }

    private int GetTimeValue()
    {
        if (FAST_TIME_CYCLE == true)
        {
            return _hour * 60 + _minute;
        }
        else
        {
            if (World.IsMainMenu == true)
            {
                return 720;
            }
            return World.MinutesOfDay;
        }
    }
}
