using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

/// <summary>Global game state — VB Module ported to a static class.</summary>
public static class Core
{
    private const int DEFAULT_WINDOW_WIDTH = 1200;
    private const int DEFAULT_WINDOW_HEIGHT = 680;
    private const int GAME_MESSAGE_PADDING_X = 10;
    private const int GAME_MESSAGE_PADDING_Y = 40;
    private const int GAME_MESSAGE_TEXT_X = 10;
    private const int GAME_MESSAGE_TEXT_Y = 10;
    private const int TEXT_BOX_BOTTOM_OFFSET = 160;
    private const int SKY_BLUE_R = 173;
    private const int SKY_BLUE_G = 216;
    private const int SKY_BLUE_B = 255;

    public static GameController GameInstance { get; private set; } = null!;

    public static GraphicsDeviceManager GraphicsManager => GameInstance.Graphics;
    public static GraphicsDevice GraphicsDevice => GameInstance.GraphicsDevice;
    public static ContentManager Content => GameInstance.Content;
    public static GameWindow Window => GameInstance.Window;

    public static CoreSpriteBatch SpriteBatch { get; set; } = null!;
    public static SpriteBatch FontRenderer { get; set; } = null!;
    public static GameTime GameTime { get; set; } = null!;
    public static Random Random { get; } = new Random();

    public static KeyboardInput KeyboardInput { get; set; } = null!;

    public static Rectangle windowSize = new Rectangle(0, 0, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
    public static GameMessage GameMessage { get; set; } = null!;

    public static Servers.ServersManager ServersManager { get; set; } = null!;

    public static Screen CurrentScreen { get; set; } = null!;

    public static Player Player { get; set; } = null!;
    public static GameJolt.GamejoltSave GameJoltSave { get; set; } = null!;

    public static GameOptions GameOptions { get; set; } = null!;

    public static SamplerState Sampler { get; set; } = null!;

    public static Color BackgroundColor { get; set; } = new Color(SKY_BLUE_R, SKY_BLUE_G, SKY_BLUE_B);

    public static Dictionary<String, List<List<Entity>>> OffsetMaps { get; } = [];

    public static void Initialize(GameController gameReference)
    {
        GameInstance = gameReference;

        Window.Title = $"{GameController.GAMENAME} {GameController.GAMEDEVELOPMENT_STAGE} {GameController.GAME_VERSION} - .NET 10 v. {GameController.PORT_VERSION}" +
            (CommandLineArgHandler.ForceGraphics == true ? "(FORCED GRAPHICS)" : String.Empty);

        GameOptions = new GameOptions();
        GameOptions.LoadOptions();

        GraphicsManager.PreferredBackBufferWidth = (int)GameOptions.WindowSize.X;
        GraphicsManager.PreferredBackBufferHeight = (int)GameOptions.WindowSize.Y;
        GraphicsManager.SynchronizeWithVerticalRetrace = false;
        GraphicsDevice.PresentationParameters.BackBufferFormat = SurfaceFormat.Rgba1010102;
        GraphicsDevice.PresentationParameters.DepthStencilFormat = DepthFormat.Depth24Stencil8;

        GraphicsManager.PreferMultiSampling = true;
        GraphicsManager.GraphicsProfile = GraphicsProfile.HiDef;
        GraphicsManager.ApplyChanges();

        windowSize = new Rectangle(0, 0, (int)GameOptions.WindowSize.X, (int)GameOptions.WindowSize.Y);

        SpriteBatch = new CoreSpriteBatch(GraphicsDevice);
        FontRenderer = new CoreSpriteBatch(GraphicsDevice);

        Canvas.SetupCanvas();
        Player = new Player();
        GameJoltSave = new GameJolt.GamejoltSave();

        GameMessage = new GameMessage(null, new Size(0, 0), new Vector2(0, 0));

        Sampler = new SamplerState
        {
            Filter = TextureFilter.Point,
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp
        };

        ServersManager = new Servers.ServersManager();

        GraphicsDevice.SamplerStates[0] = Sampler;
        KeyboardInput = new KeyboardInput();

        if (CommandLineArgHandler.NoSplash == true)
        {
            LoadContent();
            SetScreen(new PressStartScreen());
        }
        else
        {
            SetScreen(new SplashScreen(GameInstance));
        }
    }

    public static void LoadContent()
    {
        GameModeManager.LoadGameModes();
        Logger.Debug("Loaded game modes.");

        FontManager.LoadFonts();

        Screen.TextBox.TextFont = FontManager.GetFontContainer("textfont");
        Logger.Debug("Loaded fonts.");

        KeyBindings.LoadKeys();
        TextureManager.InitializeTextures();
        MusicManager.Setup();
        Logger.Debug("Loaded content.");

        Logger.Debug($"Validated files. Result: {Security.FileValidation.IsValid(true)}");
        if (Security.FileValidation.IsValid(false) == false)
        {
            Logger.Log(Logger.LogTypes.Warning,
                "Core.cs: File Validation failed! Download a fresh copy of the game to fix this issue.");
        }

        GameMessage = new GameMessage(
            TextureManager.DefaultTexture,
            new Size(GAME_MESSAGE_PADDING_X, GAME_MESSAGE_PADDING_Y),
            new Vector2(0, 0))
        {
            Dock = GameMessage.DockStyles.Top,
            BackgroundColor = Color.Black,
            TextPosition = new Vector2(GAME_MESSAGE_TEXT_X, GAME_MESSAGE_TEXT_Y)
        };
        Logger.Debug("Gamemessage initialized.");

        GameOptions.LoadOptions();

        String tempDir = Path.Combine(GameController.GamePath, "Temp");
        if (Directory.Exists(tempDir) == true)
        {
            try
            {
                Directory.Delete(tempDir, true);
                Logger.Log(Logger.LogTypes.Message, "Core.cs: Deleted Temp directory.");
            }
            catch
            {
                Logger.Log(Logger.LogTypes.Warning, "Core.cs: Failed to delete the Temp directory.");
            }
        }

        GameJolt.StaffProfile.SetupStaff();
        ScriptVersion2.ScriptLibrary.InitializeLibrary();
    }

    public static void Update(GameTime gameTime)
    {
        Core.GameTime = gameTime;

        ConnectScreen.UpdateConnectSet();

        if (GameController.IsActiveWindow() == false)
        {
            if (CurrentScreen.CanBePaused == true)
            {
                SetScreen(new PauseScreen(CurrentScreen));
            }
        }
        else
        {
            KeyBoardHandler.Update();
            ControllerHandler.Update();
            Controls.MakeMouseVisible();
            MouseHandler.Update();
            if (KeyBoardHandler.KeyPressed(KeyBindings.EscapeKey) == true ||
                ControllerHandler.ButtonPressed(Buttons.Start) == true)
            {
                CurrentScreen.EscapePressed();
            }
        }

        CurrentScreen.Update();
        if (CurrentScreen.CanChat == true)
        {
            if (KeyBoardHandler.KeyPressed(KeyBindings.ChatKey) == true ||
                ControllerHandler.ButtonPressed(Buttons.RightShoulder) == true)
            {
                if (JoinServerScreen.Online == true ||
                    Player.SandBoxMode == true ||
                    GameController.IS_DEBUG_ACTIVE == true)
                {
                    SetScreen(new ChatScreen(CurrentScreen));
                }
            }
        }

        MainGameFunctions.FunctionKeys();
        MusicManager.Update();
        SoundManager.Update();

        GameMessage.Update();

        LoadingDots.Update();
        ForcedCrash.Update();

        ServersManager.Update();
    }

    public static void Draw()
    {
        GraphicsDevice.Clear(BackgroundColor);

        if (SpriteBatch.Running == true)
        {
            SpriteBatch.EndBatch();
        }
        else
        {
            SpriteBatch.BeginBatch();
            FontRenderer.Begin();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.SamplerStates[0] = Sampler;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            CurrentScreen.Draw();

            if (Core.Player != null)
            {
                if (Core.Player.IsGameJoltSave == true)
                {
                    GameJolt.Emblem.DrawNewEmblems();
                }
                Core.Player.DrawLevelUp();
            }

            if (JoinServerScreen.Online == true ||
                Player.SandBoxMode == true ||
                GameController.IS_DEBUG_ACTIVE == true)
            {
                if (CurrentScreen.Identification != Screen.Identifications.ChatScreen)
                {
                    ChatScreen.DrawNewMessages();
                }
            }

            if (GameOptions.ShowDebug > 0)
            {
                DebugDisplay.Draw();
            }

            GameMessage.Draw();
            OnlineStatus.Draw();

            Logger.DrawLog();

            SpriteBatch.EndBatch();
            FontRenderer.End();

            Render();
        }
    }

    /// <summary>Renders 3D models on top of sprites.</summary>
    private static void Render()
    {
        CurrentScreen.Render();
    }

    public static void SetScreen(Screen newScreen)
    {
        if (CurrentScreen != null)
        {
            CurrentScreen.ChangeFrom();
        }

        CurrentScreen = newScreen;

        if (ControllerHandler.IsConnected() == true)
        {
            GameInstance.IsMouseVisible = GameInstance.IsMouseVisible && newScreen.MouseVisible;
        }
        else
        {
            GameInstance.IsMouseVisible = CurrentScreen.MouseVisible;
        }

        CurrentScreen.ChangeTo();
    }

    public static Vector2 GetMiddlePosition(Size offsetFull)
    {
        return new Vector2(
            Core.windowSize.Width / 2f - offsetFull.Width / 2f,
            Core.windowSize.Height / 2f - offsetFull.Height / 2f);
    }

    public static Vector2 GetMiddleInterfacePosition(Size offsetFull)
    {
        return new Vector2(
            Core.ScreenSize.Width / 2f - offsetFull.Width / 2f,
            Core.ScreenSize.Height / 2f - offsetFull.Height / 2f);
    }

    public static void StartThreadedSub(System.Threading.ParameterizedThreadStart s)
    {
        Thread t = new Thread(s) { IsBackground = true };
        t.Start();
    }

    public static Rectangle ScreenSize
    {
        get
        {
            double scale = SpriteBatch.InterfaceScale();
            if (scale == 1d)
            {
                return windowSize;
            }
            return new Rectangle(
                (int)(windowSize.X / scale), (int)(windowSize.Y / scale),
                (int)(windowSize.Width / scale), (int)(windowSize.Height / scale));
        }
    }

    public static Rectangle ScaleScreenRec(Rectangle rec)
    {
        double scale = SpriteBatch.InterfaceScale();
        if (scale == 1d)
        {
            return rec;
        }
        return new Rectangle(
            (int)(rec.X * scale), (int)(rec.Y * scale),
            (int)(rec.Width * scale), (int)(rec.Height * scale));
    }

    public static Vector2 ScaleScreenVec(Vector2 vec)
    {
        double scale = SpriteBatch.InterfaceScale();
        if (scale == 1d)
        {
            return vec;
        }
        return new Vector2((float)(vec.X * scale), (float)(vec.Y * scale));
    }

    public static void SetWindowSize(Vector2 size)
    {
        GraphicsManager.PreferredBackBufferWidth = (int)size.X;
        GraphicsManager.PreferredBackBufferHeight = (int)size.Y;
        GraphicsManager.ApplyChanges();
        windowSize = new Rectangle(0, 0, (int)size.X, (int)size.Y);
    }

    public static void OnWindowClientSizeChanged()
    {
        Core.windowSize = new Rectangle(
            0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);

        if (Core.CurrentScreen != null)
        {
            Core.CurrentScreen.SizeChanged();
            Screen.TextBox.PositionY = Core.windowSize.Height - TEXT_BOX_BOTTOM_OFFSET;
        }
    }
}
