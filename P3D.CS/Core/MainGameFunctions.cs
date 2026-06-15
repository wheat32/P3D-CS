using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

internal static class MainGameFunctions
{
    private const int DEBUG_CYCLE_MAX = 2;
    private const int DEFAULT_WINDOW_WIDTH = 1200;
    private const int DEFAULT_WINDOW_HEIGHT = 680;
    private const int SCREENSHOT_SHOW_DURATION = 12;

    public static void FunctionKeys()
    {
        if (KeyBoardHandler.KeyPressed(KeyBindings.GUIControlKey) == true)
        {
            Core.GameOptions.ShowGUI = !Core.GameOptions.ShowGUI;
            Core.GameOptions.SaveOptions();
            Core.GameMessage.ShowMessage(Core.GameOptions.ShowGUI == true
                ? Localization.GetString("game_message_gui_on", "GUI Enabled")
                : Localization.GetString("game_message_gui_off", "GUI Disabled"),
                SCREENSHOT_SHOW_DURATION, FontManager.MainFont, Color.White);
        }
        else if (KeyBoardHandler.KeyPressed(KeyBindings.ScreenshotKey) == true &&
                 Core.CurrentScreen.CanTakeScreenshot == true)
        {
            CaptureScreen();
        }
        else if (KeyBoardHandler.KeyPressed(KeyBindings.DebugKey) == true)
        {
            Core.GameOptions.ShowDebug = (Core.GameOptions.ShowDebug + 1) % DEBUG_CYCLE_MAX;
            Core.GameOptions.SaveOptions();
        }
        else if (KeyBoardHandler.KeyPressed(KeyBindings.LightKey) == true)
        {
            Core.GameOptions.LightingEnabled = !Core.GameOptions.LightingEnabled;
            Core.GameOptions.SaveOptions();
            Core.GameMessage.ShowMessage(Core.GameOptions.LightingEnabled == true
                ? Localization.GetString("game_message_lighting_on", "Lighting Enabled")
                : Localization.GetString("game_message_lighting_off", "Lighting Disabled"),
                SCREENSHOT_SHOW_DURATION, FontManager.MainFont, Color.White);
        }
        else if (KeyBoardHandler.KeyPressed(KeyBindings.FullScreenKey) == true &&
                 Core.CurrentScreen.CanGoFullscreen == true)
        {
            ToggleFullScreen();
        }
        else if (KeyBoardHandler.KeyPressed(KeyBindings.MuteAudioKey) == true &&
                 Core.CurrentScreen.CanMuteAudio == true)
        {
            bool mute = !MusicManager.Muted;
            MusicManager.Muted = mute;
            SoundManager.Muted = mute;
            Core.GameOptions.SaveOptions();
            Core.CurrentScreen.ToggledMute();
        }

        if (KeyBoardHandler.KeyDown(KeyBindings.DebugKey) == true)
        {
            if (KeyBoardHandler.KeyPressed(Keys.F) == true)
            {
                TextureManager.TextureList.Clear();
                Core.GameMessage.ShowMessage(
                    Localization.GetString("game_message_debug_texture_list_clear", "Texture list have cleared"),
                    SCREENSHOT_SHOW_DURATION, FontManager.MainFont, Color.White);
            }
            else if (KeyBoardHandler.KeyPressed(Keys.S) == true)
            {
                Core.SetWindowSize(new Vector2(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            }
            else if (KeyBoardHandler.KeyPressed(Keys.L) == true)
            {
                Logger.DisplayLog = !Logger.DisplayLog;
            }
            else if (KeyBoardHandler.KeyPressed(Keys.B) == true)
            {
                Entity.drawViewBox = !Entity.drawViewBox;
            }
        }

        if (KeyBoardHandler.KeyPressed(KeyBindings.DisableControllerKey) == true)
        {
            Core.GameOptions.GamePadEnabled = !Core.GameOptions.GamePadEnabled;
            Core.GameMessage.ShowMessage(Core.GameOptions.GamePadEnabled == true
                ? Localization.GetString("game_message_gamepad_support_on", "Enabled XBOX 360 GamePad support")
                : Localization.GetString("game_message_gamepad_support_off", "Disabled XBOX 360 GamePad support"),
                SCREENSHOT_SHOW_DURATION, FontManager.MainFont, Color.White);
            Core.GameOptions.SaveOptions();
        }
    }

    private static void CaptureScreen()
    {
        try
        {
            Core.GameMessage.HideMessage();

            DateTime now = DateTime.Now;
            String fileName = $"{now.Year}-{now.Month:D2}-{now.Day:D2}_{now.Hour:D2}.{now.Minute:D2}.{now.Second:D2}.png";

            String screenshotsDir = AppPaths.ScreenshotsDir;
            Directory.CreateDirectory(screenshotsDir);

            PresentationParameters pp = Core.GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            Color[] data = new Color[width * height];
            Core.GraphicsDevice.GetBackBufferData(data);

            using Texture2D texture = new Texture2D(Core.GraphicsDevice, width, height);
            texture.SetData(data);

            using FileStream stream = File.OpenWrite(Path.Combine(screenshotsDir, fileName));
            texture.SaveAsPng(stream, width, height);

            Core.GameMessage.SetupText(
                Localization.GetString("game_message_screenshot") + " " + fileName,
                FontManager.MainFont, Color.White);
            Core.GameMessage.ShowMessage(SCREENSHOT_SHOW_DURATION, Core.GraphicsDevice);
        }
        catch (Exception ex)
        {
            Logger.Log(Logger.LogTypes.ErrorMessage,
                "MainGameFunctions.cs: " +
                Localization.GetString("game_message_screenshot_failed") +
                ". More information: " + ex.Message);
        }
    }

    private static void ToggleFullScreen()
    {
        Core.GraphicsManager.HardwareModeSwitch = false;
        if (Core.GraphicsManager.IsFullScreen == false)
        {
            DisplayMode mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            Core.GraphicsManager.PreferredBackBufferWidth = mode.Width;
            Core.GraphicsManager.PreferredBackBufferHeight = mode.Height;
            Core.windowSize = new Rectangle(0, 0, mode.Width, mode.Height);
            Core.GraphicsManager.ToggleFullScreen();
            Core.GameMessage.ShowMessage(
                Localization.GetString("game_message_fullscreen_on"),
                SCREENSHOT_SHOW_DURATION, FontManager.MainFont, Color.White);
        }
        else
        {
            Core.GraphicsManager.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH;
            Core.GraphicsManager.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT;
            Core.windowSize = new Rectangle(0, 0, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
            Core.GraphicsManager.ToggleFullScreen();
            Core.GameMessage.ShowMessage(
                Localization.GetString("game_message_fullscreen_off"),
                SCREENSHOT_SHOW_DURATION, FontManager.MainFont, Color.White);
        }

        Core.GraphicsManager.ApplyChanges();
        NetworkPlayer.ScreenRegionChanged();
    }
}
