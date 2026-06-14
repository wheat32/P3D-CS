using System.Globalization;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class Classified
{
    public static String Remote_Texture_URL = String.Empty; // CLASSIFIED — remote texture server URL
    public const String GameJolt_Game_ID = "";    // CLASSIFIED — GameJolt API game ID
    public const String GameJolt_Game_Key = "";   // CLASSIFIED — GameJolt API private key
    public static String Encryption_Password = String.Empty; // CLASSIFIED — save/network encryption password
}

/// <summary>Controls the game's main workflow.</summary>
public class GameController : Game
{
    public const String GAME_VERSION = "0.61";
    public const String RELEASE_VERSION = "108";
    public const String GAMEDEVELOPMENT_STAGE = "Indev";
    public const String GAMENAME = "Pokémon 3D";
    public const String DEVELOPER_NAME = "P3D Team";
    
    // Variable(s) for the C# port
    public const String PORT_VERSION = "0.1.0";

#if DEBUG
    public const bool IS_DEBUG_ACTIVE = true;
#else
    public const bool IS_DEBUG_ACTIVE = false;
#endif

    public const bool UPDATEONLINEVERSION = false;

    public GraphicsDeviceManager Graphics { get; }
    public FPSMonitor FPSMonitor { get; }

    private bool _windowChange;
    public static bool UpdateChecked;

    private static bool _gameHacked;

    public GameController()
    {
        _windowChange = false;
        InactiveSleepTime = TimeSpan.Zero;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += Window_ClientSizeChanged;
        Window.TextInput += Window_TextInput;

        FPSMonitor = new FPSMonitor();

        _gameHacked = File.Exists(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "temp"));
        if (_gameHacked == true)
        {
            Security.HackerAlerts.Activate();
        }

        if (Thread.CurrentThread.CurrentCulture.Name.Equals("tr-TR"))
        {
            CultureInfo enUs = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentCulture = enUs;
            CultureInfo.DefaultThreadCurrentUICulture = enUs;
            Thread.CurrentThread.CurrentCulture = enUs;
            Thread.CurrentThread.CurrentUICulture = enUs;
        }
    }

    protected override void Initialize()
    {
        Core.Initialize(this);
        base.Initialize();
    }

    protected override void LoadContent() { }

    protected override void UnloadContent() { }

    protected override void Update(GameTime gameTime)
    {
        if (_windowChange == true)
        {
            Core.SetWindowSize(new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height));
            _windowChange = false;
        }
        Core.Update(gameTime);
        base.Update(gameTime);

        GameJolt.SessionManager.Update();
        FPSMonitor.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Core.Draw();
        base.Draw(gameTime);
        FPSMonitor.DrawnFrame();
    }

    public static String DecSeparator =>
        CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

    protected override void OnExiting(Object sender, Microsoft.Xna.Framework.ExitingEventArgs args)
    {
        GameJolt.SessionManager.Close();

        if (Core.ServersManager.ServerConnection.Connected == true)
        {
            Core.ServersManager.ServerConnection.Abort();
        }

        Logger.Debug("---Exit Game---");
    }

    private static void Window_TextInput(Object? sender, Microsoft.Xna.Framework.TextInputEventArgs e)
    {
        KeyCharConverter.EnqueueChar(e.Character);
    }

    private void Window_ClientSizeChanged(Object? sender, EventArgs e)
    {
        _windowChange = true;
        Core.OnWindowClientSizeChanged();
        NetworkPlayer.ScreenRegionChanged();
    }

    protected override void OnActivated(Object sender, EventArgs args)
    {
        base.OnActivated(sender, args);
        NetworkPlayer.ScreenRegionChanged();
    }

    protected override void OnDeactivated(Object sender, EventArgs args)
    {
        base.OnDeactivated(sender, args);
        NetworkPlayer.ScreenRegionChanged();
    }

    /// <summary>If the player hacked any instance of Pokémon3D at some point.</summary>
    public static bool Hacker => _gameHacked;

    /// <summary>The path to the game folder.</summary>
    public static String GamePath =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;

    /// <summary>Returns true if this process owns the foreground window.</summary>
    public static bool IsActiveWindow()
    {
        return Core.GameInstance?.IsActive ?? false;
    }
}

/// <summary>Cross-platform cursor confinement stub.</summary>
public static class CursorClipper
{
    public static bool IsActivated { get; private set; }

    public static void LockCursor()
    {
        IsActivated = true;
    }

    public static void ReleaseCursor()
    {
        IsActivated = false;
    }
}
