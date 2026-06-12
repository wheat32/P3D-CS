using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

/// <summary>Base class for all screens in the game.</summary>
public abstract class Screen
{
    public enum Identifications
    {
        MainMenuScreen,
        OverworldScreen,
        MenuScreen,
        PokedexSelectScreen,
        PokedexScreen,
        PokedexViewScreen,
        PokedexSearchScreen,
        PokedexHabitatScreen,
        PartyScreen,
        SummaryScreen,
        InventoryScreen,
        BerryScreen,
        TrainerScreen,
        PauseScreen,
        SaveScreen,
        NewGameScreen,
        OptionScreen,
        StorageSystemScreen,
        TradeScreen,
        MapScreen,
        ChatScreen,
        UseItemScreen,
        ItemDetailScreen,
        ChooseItemScreen,
        ChoosePokemonScreen,
        EvolutionScreen,
        ApricornScreen,
        TransitionScreen,
        BattleCatchScreen,
        BlackOutScreen,
        BattlePokemonScreen,
        CreditsScreen,
        DonationScreen,
        NameObjectScreen,
        LearnAttackScreen,
        SecretBaseScreen,
        PokegearScreen,
        BattleGrowStatsScreen,
        DaycareScreen,
        HatchEggScreen,
        SplashScreen,
        GameJoltLoginScreen,
        GameJoltUserViewerScreen,
        GameJoltLobbyScreen,
        GameJoltAddFriendScreen,
        ChooseAttackScreen,
        BattleScreen,
        BattleIniScreen,
        BattleAnimationScreen,
        GTSMainScreen,
        GTSInboxScreen,
        GTSSearchScreen,
        GTSSelectLevelScreen,
        GTSSelectPokemonScreen,
        GTSSelectGenderScreen,
        GTSSetupScreen,
        GTSEditTradeScreen,
        GTSSelectAreaScreen,
        GTSSelectUserScreen,
        GTSTradeScreen,
        GTSTradingScreen,
        TeachMovesScreen,
        OfflineGameWarningScreen,
        HallofFameScreen,
        ViewModelScreen,
        MailSystemScreen,
        PVPLobbyScreen,
        InputScreen,
        JoinServerScreen,
        ConnectScreen,
        AddServerScreen,
        MysteryEventScreen,
        DirectTradeScreen,
        StorageSystemFilterScreen,
        WonderTradeScreen,
        RegisterBattleScreen,
        StatisticsScreen,
        MapPreviewScreen,
        KeyBindingScreen,
        MessageBoxScreen,
        PressStartScreen,
        CharacterSelectionScreen,
        GameModeSelectionScreen,
        VoltorbFlipScreen,
        NewMenuScreen,

        // Legacy
        PokemonScreen,
        IntroScreen,
        PokemonStatusScreen
    }

    // Shared (global) state

    public static Camera? Camera { get; set; }

    private static Level? _globalLevel;
    public static Level? Level
    {
        get => _globalLevel;
        set
        {
            if (_globalLevel != null)
            {
                _globalLevel.StopOffsetMapUpdate();
            }
            _globalLevel = value;
            _globalLevel?.StartOffsetMapUpdate();
        }
    }

    public static BasicEffectWithAlphaTest? Effect { get; set; }
    public static SkyDome? SkyDome { get; set; }
    public static TextBox TextBox { get; set; } = new TextBox();
    public static ChooseBox ChooseBox { get; set; } = new ChooseBox();
    public static PokemonImageView PokemonImageView { get; set; } = new PokemonImageView();
    public static ImageView ImageView { get; set; } = new ImageView();

    // Instance fields

    public Screen? PreScreen { get; set; }
    public Identifications Identification { get; set; } = Identifications.MainMenuScreen;
    public bool MouseVisible { get; set; }
    public bool CanBePaused { get; set; } = true;
    public bool CanMuteAudio { get; set; } = true;
    public bool CanChat { get; set; } = true;
    public bool CanTakeScreenshot { get; set; } = true;
    public bool CanDrawDebug { get; set; } = true;
    public bool CanGoFullscreen { get; set; } = true;
    protected bool IsDrawingGradients { get; set; }
    public bool IsOverlay { get; set; }
    public bool UpdateFadeOut;
    public bool UpdateFadeIn;

    // Lifecycle

    protected void CopyFrom(Screen src)
    {
        MouseVisible = src.MouseVisible;
        CanBePaused = src.CanBePaused;
        CanMuteAudio = src.CanMuteAudio;
        CanChat = src.CanChat;
        CanTakeScreenshot = src.CanTakeScreenshot;
        CanDrawDebug = src.CanDrawDebug;
        CanGoFullscreen = src.CanGoFullscreen;
    }

    public virtual void Draw() { }
    public virtual void Render() { }
    public virtual void Update() { }
    public virtual void ChangeTo() { }
    public virtual void ChangeFrom() { }
    public virtual void SizeChanged() { }
    public virtual void ToggledMute() { }

    public virtual void EscapePressed()
    {
        if (Core.CurrentScreen.CanBePaused == true)
        {
            Core.SetScreen(new PauseScreen(Core.CurrentScreen));
        }
    }

    public bool IsCurrentScreen()
    {
        return Core.CurrentScreen.Identification == Identification;
    }

    // GamePad controls HUD

    private const int GAMEPAD_BUTTON_SIZE = 32;
    private const int GAMEPAD_SHOULDER_SIZE = 64;
    private const int GAMEPAD_SPACING = 4;
    private const int GAMEPAD_BOTTOM_OFFSET = 40;
    private const int GAMEPAD_TEXT_X_PADDING = 16;
    private const int GAMEPAD_TEXT_Y_OFFSET = 4;
    private const int GAMEPAD_TEXT_Y_SHADOW = 7;
    private const int GAMEPAD_SHADOW_OFFSET = 3;

    public void DrawGamePadControls(Dictionary<Buttons, String> descriptions)
    {
        int x = Core.ScreenSize.Width;
        foreach (KeyValuePair<Buttons, String> kv in descriptions)
        {
            switch (kv.Key)
            {
                case Buttons.A:
                case Buttons.B:
                case Buttons.X:
                case Buttons.Y:
                case Buttons.Start:
                case Buttons.Back:
                case Buttons.LeftStick:
                case Buttons.RightStick:
                case Buttons.LeftTrigger:
                case Buttons.RightTrigger:
                    x -= GAMEPAD_BUTTON_SIZE + GAMEPAD_SPACING;
                    break;

                case Buttons.LeftShoulder:
                case Buttons.RightShoulder:
                    x -= GAMEPAD_SHOULDER_SIZE + GAMEPAD_SPACING;
                    break;
            }
            x -= (int)((int)(FontManager.MainFont?.MeasureString(kv.Value).X ?? 0) + GAMEPAD_TEXT_X_PADDING * Core.SpriteBatch.InterfaceScale());
        }
        DrawGamePadControls(descriptions, new Vector2(x, Core.ScreenSize.Height - GAMEPAD_BOTTOM_OFFSET));
    }

    public void DrawGamePadControls(Dictionary<Buttons, String> descriptions, Vector2 position)
    {
        if (GamePad.GetState(PlayerIndex.One).IsConnected == false ||
            Core.GameOptions.GamePadEnabled == false ||
            IsCurrentScreen() == false)
        {
            return;
        }

        int x = (int)position.X;
        int y = (int)position.Y;

        foreach (KeyValuePair<Buttons, String> kv in descriptions)
        {
            String texturePath = @"GUI\GamePad\xboxController";
            int width = GAMEPAD_BUTTON_SIZE;
            int height = GAMEPAD_BUTTON_SIZE;

            switch (kv.Key)
            {
                case Buttons.A:            texturePath += "ButtonA"; break;
                case Buttons.B:            texturePath += "ButtonB"; break;
                case Buttons.X:            texturePath += "ButtonX"; break;
                case Buttons.Y:            texturePath += "ButtonY"; break;
                case Buttons.LeftShoulder: texturePath += "LeftShoulder"; width = GAMEPAD_SHOULDER_SIZE; break;
                case Buttons.RightShoulder: texturePath += "RightShoulder"; width = GAMEPAD_SHOULDER_SIZE; break;
                case Buttons.LeftStick:    texturePath += "LeftStick"; break;
                case Buttons.RightStick:   texturePath += "RightStick"; break;
                case Buttons.LeftTrigger:  texturePath += "LeftTrigger"; break;
                case Buttons.RightTrigger: texturePath += "RightTrigger"; break;
                case Buttons.Start:        texturePath += "Start"; break;
                case Buttons.Back:         texturePath += "Back"; break;
                default:                   break;
            }

            Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(texturePath),
                new Rectangle(x + GAMEPAD_SHADOW_OFFSET, y + GAMEPAD_SHADOW_OFFSET, width, height), Color.Black);
            Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(texturePath),
                new Rectangle(x, y, width, height), Color.White);

            x += width + GAMEPAD_SPACING;

            if (FontManager.MainFont != null)
            {
                Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, kv.Value,
                    new Vector2(x + GAMEPAD_SHADOW_OFFSET, y + GAMEPAD_TEXT_Y_SHADOW), Color.Black);
                Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, kv.Value,
                    new Vector2(x, y + GAMEPAD_TEXT_Y_OFFSET), Color.White);
            }

            x += (int)((int)(FontManager.MainFont?.MeasureString(kv.Value).X ?? 0) +
                       GAMEPAD_TEXT_X_PADDING * Core.SpriteBatch.InterfaceScale());
        }
    }

    // Gradient drawing

    protected void DrawGradients(int alpha)
    {
        DrawGradients(alpha, Screens.UI.ColorProvider.IsGameJolt);
    }

    protected void DrawGradients(int alpha, bool isGameJolt)
    {
        bool canDraw = true;
        Screen s = this;

        while (s.PreScreen != null && canDraw == true)
        {
            if (s.IsOverlay == false)
            {
                s = s.PreScreen;
                if (s.IsDrawingGradients == true)
                {
                    canDraw = false;
                }
            }
            else
            {
                break;
            }
        }

        if (canDraw == false)
        {
            return;
        }

        Color c = Screens.UI.ColorProvider.GradientColor(isGameJolt, 0);
        Color cA = Screens.UI.ColorProvider.GradientColor(isGameJolt, alpha);

        Canvas.DrawGradient(new Rectangle(0, 0, Core.windowSize.Width, 200), cA, c, false, -1);
        Canvas.DrawGradient(
            new Rectangle(0, Core.windowSize.Height - 200, Core.windowSize.Width, 200),
            c, cA, false, -1);
    }

    // Scale helpers

    private static readonly Size DEFAULT_SCALE_MIN = new Size(800, 620);
    private static readonly Size DEFAULT_SCALE_MAX = new Size(2560, 1440);

    public virtual String GetScreenStatus()
    {
        return "Screen state not implemented for screen class: " + Identification;
    }

    public virtual Size GetScreenScaleMinimum()
    {
        return DEFAULT_SCALE_MIN;
    }

    public virtual Size GetScreenScaleMaximum()
    {
        return DEFAULT_SCALE_MAX;
    }

    protected virtual SpriteBatch GetFontRenderer()
    {
        return IsCurrentScreen() == true ? Core.FontRenderer : Core.SpriteBatch;
    }
}
