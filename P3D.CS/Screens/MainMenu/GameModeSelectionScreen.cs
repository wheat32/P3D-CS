using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class GameModeSelectionScreen : Screen
{
    private GameMode[] _gameModes = [];
    private int _index = 0;
    private float _offset = 0f;

    private String _tempGameModesDisplay = String.Empty;
    private Texture2D? _gameModeSplash;

    private Texture2D? _menuTexture;

    private const int WIDTH = 320;
    private const int HEIGHT = 64;
    private const int GAP = 32;

    public GameModeSelectionScreen(Screen currentScreen)
    {
        Identification = Identifications.GameModeSelectionScreen;
        CanBePaused = false;
        CanChat = false;
        CanDrawDebug = true;
        CanGoFullscreen = true;
        CanMuteAudio = true;
        CanTakeScreenshot = true;

        PreScreen = currentScreen;
        _gameModes = GameModeManager.GetAllGameModes();
        if (_gameModes.Length > 0)
        {
            GameModeManager.SetGameModePointer(_gameModes[_index].DirectoryName);
            Localization.ReloadGameModeTokens();
        }
        _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
    }

    public override void Draw()
    {
        PreScreen?.Draw();

        if (_gameModeSplash != null)
        {
            DrawGameModeSplash();
        }

        String text = Localization.GetString("gamemode_menu_select1", "Select a GameMode") + Environment.NewLine +
            Localization.GetString("gamemode_menu_select2", "to start the new game with.");

        GetFontRenderer().DrawString(FontManager.InGameFont, text, new Vector2(30, 30), Color.White);

        int center = (int)(Core.windowSize.Width / 2 + 320);
        for (int i = 0; i <= _gameModes.Length - 1; i++)
        {
            int buttonY = (int)(i * (HEIGHT + GAP) + _offset + Core.windowSize.Height / 2 - HEIGHT / 2);
            int halfWidth = WIDTH / 2;
            Rectangle buttonColor = new Rectangle(0, 0, 16, 16);
            Color buttonAccent = Screens.UI.ColorProvider.AccentColor(false, 255);
            if (i != _index)
            {
                buttonColor = new Rectangle(40, 48, 16, 16);
                buttonAccent = Screens.UI.ColorProvider.MainColor(false, 255);
            }

            String displayText = _gameModes[i].Name.CropStringToWidth(FontManager.InGameFont, WIDTH - 32);
            if (displayText == "Kolben") displayText = "Pokémon 3D";

            for (int x = 0; x <= (int)(WIDTH / 16); x++)
            {
                for (int y = 0; y <= (int)(HEIGHT / 16); y++)
                {
                    Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)(x * 16 + (center - halfWidth)), (int)(y * 16 + buttonY - 8), 16, 16), buttonColor, Color.White);
                    Canvas.DrawRectangle(new Rectangle((int)(center - halfWidth), (int)(buttonY - 8), WIDTH + 16, 3), buttonAccent);
                }
            }

            Vector2 textSize = FontManager.InGameFont.MeasureString(displayText);
            GetFontRenderer().DrawString(FontManager.InGameFont, displayText,
                new Vector2(center - halfWidth + 32 + 2, (int)(buttonY + HEIGHT / 2 - textSize.Y / 2 + 2)),
                Color.Black, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
            GetFontRenderer().DrawString(FontManager.InGameFont, displayText,
                new Vector2(center - halfWidth + 32, (int)(buttonY + HEIGHT / 2 - textSize.Y / 2)),
                Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        if (_tempGameModesDisplay == String.Empty && _gameModes.Length > 0)
        {
            GameMode gameMode = GameModeManager.GetGameMode(_gameModes[_index].DirectoryName) ?? _gameModes[_index];

            String gameModeName = gameMode.Name;
            if (Localization.TokenExists("gamemode_name_" + gameMode.DirectoryName) == true)
                gameModeName = Localization.GetString("gamemode_name_" + gameMode.DirectoryName);
            else if (Localization.TokenExists(("gamemode_name_" + gameMode.DirectoryName).ToLower()) == true)
                gameModeName = Localization.GetString(("gamemode_name_" + gameMode.DirectoryName).ToLower());
            if (gameModeName == "Kolben") gameModeName = "Pokémon 3D";

            String gameModeDesc = gameMode.Description;
            if (Localization.TokenExists("gamemode_desc_" + gameMode.DirectoryName) == true)
                gameModeDesc = Localization.GetString("gamemode_desc_" + gameMode.DirectoryName);
            else if (Localization.TokenExists(("gamemode_desc_" + gameMode.DirectoryName).ToLower()) == true)
                gameModeDesc = Localization.GetString(("gamemode_desc_" + gameMode.DirectoryName).ToLower());
            String dispDescription = gameModeDesc.Replace("~", Environment.NewLine).Replace("*", Environment.NewLine);

            _tempGameModesDisplay =
                Localization.GetString("gamemode_menu_name") + ": " + gameModeName + Environment.NewLine +
                Localization.GetString("gamemode_menu_version") + ": " + gameMode.Version + Environment.NewLine +
                Localization.GetString("gamemode_menu_author") + ": " + gameMode.Author + Environment.NewLine +
                Localization.GetString("gamemode_menu_description") + ": " + dispDescription;
        }

        if (_tempGameModesDisplay != String.Empty)
        {
            _tempGameModesDisplay = _tempGameModesDisplay.CropStringToWidth(FontManager.InGameFont, 400);
            Vector2 textMeasure = FontManager.InGameFont.MeasureString(_tempGameModesDisplay);
            Rectangle displayRect = new Rectangle(
                (int)(Core.windowSize.Width / 2 - 400 - 32),
                (int)(Core.windowSize.Height / 2 - textMeasure.Y / 2 - 32),
                480 + 32,
                (int)(textMeasure.Y + 64));
            int displayWidth = (int)(displayRect.Width / 16);
            int displayHeight = (int)(displayRect.Height / 16);

            for (int x = 0; x <= displayWidth; x++)
            {
                for (int y = 0; y <= displayHeight; y++)
                {
                    Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)(x * 16 + displayRect.X), (int)(y * 16) + displayRect.Y - 8, 16, 16), new Rectangle(0, 0, 16, 16), Color.White);
                }
            }
            Canvas.DrawRectangle(new Rectangle(displayRect.X, displayRect.Y - 8, displayWidth * 16 + 16, 3), Screens.UI.ColorProvider.AccentColor(false, 255));

            Core.SpriteBatch.DrawString(FontManager.InGameFont, _tempGameModesDisplay, new Vector2(displayRect.X + 32 + 2, displayRect.Y + 32 + 2), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _tempGameModesDisplay, new Vector2(displayRect.X + 32, displayRect.Y + 32), Color.White);

            Core.SpriteBatch.Draw(_menuTexture,
                new Rectangle((int)(displayRect.X + displayRect.Width + 16), (int)(displayRect.Y + (textMeasure.Y + 64) / 2 - 16), 16, 32),
                new Rectangle(64, 0, 16, 32), Color.White);
        }
    }

    public void DrawGameModeSplash()
    {
        if (_gameModeSplash == null) return;
        int origW = _gameModeSplash.Width;
        int origH = _gameModeSplash.Height;
        float aspectRatio = (float)origW / origH;

        int bw = (int)(Core.windowSize.Width * aspectRatio);
        int bh = (int)(bw / aspectRatio);

        if (bw > bh)
        {
            bw = Core.windowSize.Width;
            bh = (int)(Core.windowSize.Width / aspectRatio);
        }
        else
        {
            bh = Core.windowSize.Height;
            bw = (int)(Core.windowSize.Height / aspectRatio);
        }
        if (bh < Core.windowSize.Height)
        {
            bh = Core.windowSize.Height;
            bw = (int)((float)Core.windowSize.Height / origH * origW);
        }

        int xOffset = 0;
        if (Core.windowSize.Width < bw)
        {
            float xAspectRatio = (float)origW / bw;
            xOffset = (int)(Math.Floor((bw - Core.windowSize.Width) * xAspectRatio) / 2);
        }

        Core.SpriteBatch.Draw(_gameModeSplash, new Rectangle(0, 0, bw, bh), new Rectangle(xOffset, 0, origW, origH), Color.White);
    }

    public override void Update()
    {
        PreScreen?.PreScreen?.Update();

        if (_gameModes.Length == 0) return;

        if (_index > 0 && Controls.Up(true, true, true, true, true, true) == true)
        {
            _index -= 1;
            GameModeManager.SetGameModePointer(_gameModes[_index].DirectoryName);
            Localization.ReloadGameModeTokens();
            FontManager.LoadFonts();
            _tempGameModesDisplay = String.Empty;
            _gameModeSplash = null;
            _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
        }
        if (_index < _gameModes.Length - 1 && Controls.Down(true, true, true, true, true, true) == true)
        {
            _index += 1;
            GameModeManager.SetGameModePointer(_gameModes[_index].DirectoryName);
            Localization.ReloadGameModeTokens();
            FontManager.LoadFonts();
            _tempGameModesDisplay = String.Empty;
            _gameModeSplash = null;
            _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
        }

        if (KeyBoardHandler.KeyPressed(KeyBindings.EscapeKey) == true ||
            KeyBoardHandler.KeyPressed(KeyBindings.BackKey1) == true ||
            KeyBoardHandler.KeyPressed(KeyBindings.BackKey2) == true ||
            MouseHandler.ButtonPressed(MouseHandler.MouseButtons.RightButton) == true ||
            ControllerHandler.ButtonPressed(Buttons.B) == true)
        {
            SoundManager.PlaySound("select");
            Core.SetScreen(PreScreen!);
        }

        if (Controls.Accept(true, true, true) == true)
        {
            GameModeManager.SetGameModePointer(_gameModes[_index].DirectoryName);
            Localization.ReloadGameModeTokens();
            FontManager.LoadFonts();
            SoundManager.PlaySound("select");
            if (GameModeManager.ActiveGameMode?.IntroType == "0")
                Core.SetScreen(new TransitionScreen(PreScreen!, new NewGameScreen(), Color.Black, false));
            else
                Core.SetScreen(new NewNewGameScreen(PreScreen!));
        }

        float targetOffset = GetTargetOffset();
        if (_offset != targetOffset)
        {
            _offset = MathHelper.Lerp(_offset, targetOffset, 0.25f);
            if (Math.Abs(_offset - targetOffset) <= 0.01f) _offset = targetOffset;
        }

        if (_gameModeSplash == null)
        {
            try
            {
                String fileName = GameController.GamePath + @"\GameModes\" + _gameModes[_index].DirectoryName + @"\GameMode.png";
                if (File.Exists(fileName) == true)
                {
                    using Stream stream = File.Open(fileName, FileMode.OpenOrCreate);
                    _gameModeSplash = Texture2D.FromStream(Core.GraphicsDevice, stream);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogTypes.ErrorMessage, "GameModeSelectionScreen.cs/Update: " + ex.Message);
            }
        }
    }

    private int GetTargetOffset()
    {
        return -_index * (HEIGHT + GAP);
    }
}
