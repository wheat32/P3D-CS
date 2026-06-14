using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class NewOptionScreen : Screen
{
    private int _textSpeed = 2;
    private int _cameraSpeed = 12;
    private float _fov = 60.0f;
    private int _music = 50;
    private int _sound = 50;
    private int _renderDistance = 0;
    private int _graphicStyle = 1;
    private int _showBattleAnimations = 1;
    private bool _diagonalMovement = true;
    private int _difficulty = 0;
    private int _interfaceScale = 0;
    private int _battleStyle = 1;
    private int _loadOffsetMaps = 1;
    private bool _viewBobbing = true;
    private int _showModels = 1;
    private int _muted = 0;
    private bool _gamePadEnabled = true;
    private Vector2 _gamePadInvertRightStick = new Vector2(0, 0);
    private bool _runMode = true;
    private bool _preferMultiSampling = true;
    private int _tempMusicVolume = 50;
    private int _tempSoundVolume = 50;
    private int _tempMuted = 0;
    private int _subMenu = 0;
    private Size _screenSize;

    private int _selectPackNoiseDelay = 10;

    private List<String> _languages = [];
    private List<String> _languageNames = [];
    private String _currentLanguage = Localization.LanguageSuffix;
    private String _tempLanguage = Localization.LanguageSuffix;
    public static int[] languageMenuIndex = new int[4];

    private List<String> _packNames = [];
    private List<String> _enabledPackNames = [];
    public static bool isSelectedEnabled = false;

    private int[] _packsMenuIndex = new int[4];
    private int _packInfoIndex = 0;

    private bool _savedOptions = true;

    public static int ScreenIndex = 0;
    private int _nextIndex = 0;
    private List<Control> _controlList = [];

    private Texture2D? _texture;
    private Texture2D? _menuTexture;

    private float _interfaceFade = 0f;
    private bool _closing = false;
    private bool _opening = false;
    private float _enrollY = 0f;
    private float _itemIntro = 0f;

    private float _pageFade = 1.0f;
    private bool _pageOpening = false;
    private bool _pageClosing = false;

    private Vector2 _cursorPosition;
    private Vector2 _cursorDestPosition;

    private bool _selectedScrollBar = false;

    private String _pInfoName = String.Empty;
    private Texture2D? _pInfoSplash = null;
    private String _pInfoVersion = String.Empty;
    private String _pInfoAuthor = String.Empty;
    private String _pInfoDescription = String.Empty;

    public NewOptionScreen(Screen currentScreen, int submenu = 0)
    {
        _texture = TextureManager.GetTexture(@"GUI\Menus\General");
        _menuTexture = TextureManager.GetTexture(@"GUI\Menus\Options");

        Identification = Identifications.OptionScreen;
        PreScreen = currentScreen;
        CanChat = false;
        MouseVisible = true;
        CanBePaused = false;
        _opening = true;

        _screenSize = new Size((int)Core.windowSize.Width, (int)Core.windowSize.Height);
        _cursorPosition = new Vector2((int)(Core.windowSize.Width / 2) - 400 + 90, (int)(Core.windowSize.Height / 2) - 200 + 80);
        _cursorDestPosition = _cursorPosition;

        GetLanguages();
        GetPacks();

        if (Camera == null)
            Camera = new OverworldCamera();

        SetFunctionality();

        if (submenu > 0 && PreScreen!.Identification == Identifications.MainMenuScreen)
        {
            _subMenu = submenu;
            switch (_subMenu)
            {
                case 1: SwitchToLanguage(); break;
                case 2: SwitchToAudio(); break;
                case 3: SwitchToControls(); break;
                case 4: SwitchToContentPacks(); break;
            }
        }
        else
        {
            _subMenu = 0;
            ScreenIndex = 0;
        }
    }

    private void SetFunctionality()
    {
        if (PreScreen!.Identification != Identifications.MainMenuScreen)
        {
            Camera = (OverworldCamera)Screen.Camera!;
            _fov = ((OverworldCamera)Camera).FOV;
            _textSpeed = TextBox.TextSpeed;
            _cameraSpeed = (int)(((OverworldCamera)Camera).RotationSpeed * 10000);
            _difficulty = Core.Player.DifficultyMode;
            _runMode = Core.Player.RunMode;
        }
        _tempMusicVolume = (int)(MusicManager.MasterVolume * 100);
        _tempSoundVolume = (int)(SoundManager.Volume * 100);
        _tempMuted = int.Parse(MusicManager.Muted.ToNumberString());
        _muted = int.Parse(MusicManager.Muted.ToNumberString());
        _music = (int)(MusicManager.MasterVolume * 100);
        _sound = (int)(SoundManager.Volume * 100);
        _renderDistance = Core.GameOptions.RenderDistance;
        _graphicStyle = Core.GameOptions.GraphicStyle;
        _interfaceScale = Core.GameOptions.InterfaceScale;
        _showBattleAnimations = Core.Player.ShowBattleAnimations;
        _diagonalMovement = Core.Player.DiagonalMovement;
        _battleStyle = Core.Player.BattleStyle;
        _showModels = Core.Player.ShowModelsInBattle == true ? 1 : 0;
        _loadOffsetMaps = Core.GameOptions.LoadOffsetMaps == 0 ? 0 : 101 - Core.GameOptions.LoadOffsetMaps;
        _viewBobbing = Core.GameOptions.ViewBobbing;
        _gamePadEnabled = Core.GameOptions.GamePadEnabled;
        _gamePadInvertRightStick = Core.GameOptions.GamePadInvertRightStick;
        _preferMultiSampling = Core.GraphicsManager.PreferMultiSampling;
    }

    public override void Draw()
    {
        PreScreen?.Draw();
        DrawBackground();
        if (ScreenIndex == 6)
            DrawLanguageMenu();
        if (ScreenIndex == 7)
            DrawPacksMenu();
        if (ScreenIndex == 8)
            DrawPackInformationMenu();
        DrawCurrentPage();
        DrawCursor();
        DrawMessage();

        TextBox.Draw();
        ChooseBox.Draw();
    }

    private void DrawLanguageMenu()
    {
        for (int i = 0; i <= 3; i++)
        {
            Color c = new Color(255, 255, 255, (int)(255 * _interfaceFade * _pageFade));
            if (i + languageMenuIndex[2] == languageMenuIndex[0])
                c = new Color(77, 147, 198, (int)(255 * _interfaceFade * _pageFade));

            Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2 - 258), (int)(Core.windowSize.Height / 2 - 128 + i * 50), 480, 48), c, false);
        }

        Canvas.DrawScrollBar(new Vector2((int)(Core.windowSize.Width / 2 + 250), (int)(Core.windowSize.Height / 2 - 128)),
            _languages.Count, 4, languageMenuIndex[2], new Size(4, 200), false,
            new Color(77, 147, 198, (int)(255 * _interfaceFade * _pageFade)),
            new Color(255, 255, 255, (int)(255 * _interfaceFade * _pageFade)), false);

        int x = Math.Min(_languages.Count - 1, 3);

        for (int i = 0; i <= x; i++)
        {
            String name = _languageNames[i + languageMenuIndex[2]];
            if (i + languageMenuIndex[2] == languageMenuIndex[0])
            {
                Core.SpriteBatch.DrawString(FontManager.InGameFont, name, new Vector2((int)(Core.windowSize.Width / 2 - 246), (int)(Core.windowSize.Height / 2 - 128 + 8 + 2 + i * 50)), new Color(0, 0, 0, (int)(255 * _interfaceFade * _pageFade)));
                Core.SpriteBatch.DrawString(FontManager.InGameFont, name, new Vector2((int)(Core.windowSize.Width / 2 - 248), (int)(Core.windowSize.Height / 2 - 128 + 8 + i * 50)), new Color(255, 255, 255, (int)(255 * _interfaceFade * _pageFade)));
            }
            else
            {
                Core.SpriteBatch.DrawString(FontManager.InGameFont, name, new Vector2((int)(Core.windowSize.Width / 2 - 248), (int)(Core.windowSize.Height / 2 - 128 + 8 + i * 50)), new Color(0, 0, 0, (int)(255 * _interfaceFade * _pageFade)));
            }
        }
    }

    private void UpdateLanguageMenu()
    {
        int currentIndex = languageMenuIndex[0];

        if (Controls.Up(true, true, true) == true)
        {
            languageMenuIndex[0] -= 1;
            if (languageMenuIndex[0] - languageMenuIndex[2] < 0)
                languageMenuIndex[2] -= 1;
        }
        if (Controls.Down(true, true, true) == true)
        {
            languageMenuIndex[0] += 1;
            if (languageMenuIndex[0] + languageMenuIndex[2] > 3)
                languageMenuIndex[2] += 1;
        }

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 3; i++)
            {
                if (new Rectangle((int)(Core.windowSize.Width / 2) - 258, (int)(Core.windowSize.Height / 2 - 128 + i * 50), 480, 48).Contains(MouseHandler.MousePosition) == true)
                {
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                        languageMenuIndex[0] = i + languageMenuIndex[2];
                }
            }
        }

        languageMenuIndex[0] = (int)MathHelper.Clamp(languageMenuIndex[0], 0, _languages.Count - 1);
        languageMenuIndex[2] = (int)MathHelper.Clamp(languageMenuIndex[2], 0, _languages.Count - 4);

        if (languageMenuIndex[0] != currentIndex)
        {
            Localization.Load(_languages[languageMenuIndex[0]]);
            InitializeControls();
        }
    }

    private void GetLanguages()
    {
        _languages.Clear();
        _languageNames.Clear();

        foreach (String file in System.IO.Directory.GetFiles(System.IO.Path.Combine(GameController.GamePath, "Content", "Localization")))
        {
            if (file.EndsWith(".dat") == true)
            {
                String[] content = System.IO.File.ReadAllLines(file);
                String fname = System.IO.Path.GetFileNameWithoutExtension(file);

                if (fname.StartsWith("Tokens_") == true)
                {
                    String tokenName = fname.Remove(0, 7);
                    String languageName = String.Empty;

                    foreach (String line in content)
                    {
                        if (line.StartsWith("language_name,") == true)
                        {
                            languageName = content[0].GetSplit(1);
                            _languages.Add(tokenName);
                            _languageNames.Add(languageName);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void GetPacks(bool reload = false)
    {
        _packNames.Clear();

        if (reload == false)
        {
            _enabledPackNames.Clear();
            _enabledPackNames.AddRange(Core.GameOptions.ContentPackNames);
        }

        _packNames.AddRange(_enabledPackNames);

        String packsDir = System.IO.Path.Combine(GameController.GamePath, "ContentPacks");
        if (System.IO.Directory.Exists(packsDir) == true)
        {
            foreach (String contentPackFolder in System.IO.Directory.GetDirectories(packsDir))
            {
                String newContentPack = contentPackFolder.Remove(0, packsDir.Length + 1);
                if (_packNames.Contains(newContentPack) == false)
                    _packNames.Add(newContentPack);
            }
        }
    }

    private void DrawPacksMenu()
    {
        for (int i = 0; i <= 3; i++)
        {
            Color c = new Color(255, 255, 255, (int)(255 * _interfaceFade * _pageFade));
            if (i + _packsMenuIndex[2] == _packsMenuIndex[0])
            {
                c = new Color(77, 147, 198, (int)(255 * _interfaceFade * _pageFade));
                if (_enabledPackNames.Count > 0)
                    isSelectedEnabled = _enabledPackNames.Contains(_packNames[i + _packsMenuIndex[2]]);
            }

            Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2) - 328, (int)(Core.windowSize.Height / 2 - 128 + i * 50), 500, 48), c, false);
        }

        Canvas.DrawScrollBar(new Vector2((int)(Core.windowSize.Width / 2) + 188, (int)(Core.windowSize.Height / 2 - 128)),
            _packNames.Count, 4, _packsMenuIndex[2], new Size(4, 200), false,
            new Color(77, 147, 198, (int)(255 * _interfaceFade * _pageFade)),
            new Color(255, 255, 255, (int)(255 * _interfaceFade * _pageFade)), false);

        int x = Math.Min(_packNames.Count - 1, 3);
        Color textColor = new Color(0, 0, 0, (int)(255 * _interfaceFade * _pageFade));

        if (_packNames.Count > 0)
        {
            for (int i = 0; i <= x; i++)
            {
                String name = _packNames[i + _packsMenuIndex[2]];
                if (_enabledPackNames.Contains(name) == true)
                {
                    if (i + _packsMenuIndex[2] == _packsMenuIndex[0])
                    {
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, name, new Vector2((int)(Core.windowSize.Width / 2) - 320 + 2, (int)(Core.windowSize.Height / 2 - 120 + i * 50 + 2)), textColor);
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, name, new Vector2((int)(Core.windowSize.Width / 2) - 320, (int)(Core.windowSize.Height / 2 - 120 + i * 50)), new Color(181, 255, 82, (int)(255 * _interfaceFade * _pageFade)));
                    }
                    else
                    {
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, name, new Vector2((int)(Core.windowSize.Width / 2) - 320, (int)(Core.windowSize.Height / 2 - 120 + i * 50)), new Color(98, 205, 8, (int)(255 * _interfaceFade * _pageFade)));
                    }
                }
                else
                {
                    if (i + _packsMenuIndex[2] == _packsMenuIndex[0])
                    {
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, name, new Vector2((int)(Core.windowSize.Width / 2) - 320 + 2, (int)(Core.windowSize.Height / 2 - 120 + i * 50 + 2)), textColor);
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, name, new Vector2((int)(Core.windowSize.Width / 2) - 320, (int)(Core.windowSize.Height / 2 - 120 + i * 50)), new Color(255, 255, 255, (int)(255 * _interfaceFade * _pageFade)));
                    }
                    else
                    {
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, name, new Vector2((int)(Core.windowSize.Width / 2) - 320, (int)(Core.windowSize.Height / 2 - 120 + i * 50)), textColor);
                    }
                }
            }
        }
    }

    private void UpdatePacksMenu()
    {
        int currentIndex = _packsMenuIndex[0];
        Control? currentControl = null;

        foreach (Control control in _controlList)
        {
            if (control.position == _cursorDestPosition)
            {
                currentControl = control;
                break;
            }
        }
        if (currentControl == null) return;

        if (Controls.Up(true, true, true) == true)
        {
            if (currentControl.ID > 4)
            {
                _packsMenuIndex[0] -= 1;
                if (_packsMenuIndex[0] - _packsMenuIndex[2] < 0)
                    _packsMenuIndex[2] -= 1;
                if (_selectPackNoiseDelay == 0)
                {
                    SoundManager.PlaySound("select");
                    _selectPackNoiseDelay = 10;
                }
            }
        }
        if (Controls.Down(true, true, true) == true)
        {
            if (currentControl.ID > 4)
            {
                _packsMenuIndex[0] += 1;
                if (_packsMenuIndex[0] + _packsMenuIndex[2] > 3)
                    _packsMenuIndex[2] += 1;
                if (_selectPackNoiseDelay == 0)
                {
                    SoundManager.PlaySound("select");
                    _selectPackNoiseDelay = 10;
                }
            }
        }
        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 3; i++)
            {
                if (new Rectangle((int)(Core.windowSize.Width / 2 - 328), (int)(Core.windowSize.Height / 2 - 128 + i * 50), 500, 48).Contains(MouseHandler.MousePosition) == true)
                {
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        _packsMenuIndex[0] = i + _packsMenuIndex[2];
                        if (_selectPackNoiseDelay == 0)
                        {
                            SoundManager.PlaySound("select");
                            _selectPackNoiseDelay = 10;
                        }
                    }
                }
            }
        }
        _packsMenuIndex[0] = (int)MathHelper.Clamp(_packsMenuIndex[0], 0, _packNames.Count - 1);
        _packsMenuIndex[2] = (int)MathHelper.Clamp(_packsMenuIndex[2], 0, _packNames.Count - 4);

        if (_selectPackNoiseDelay > 0)
            _selectPackNoiseDelay -= 1;
    }

    private void ButtonPackInformation()
    {
        if (_packNames.Count == 0) return;

        String packName = _packNames[_packsMenuIndex[0]];
        _pInfoSplash = null;

        String splashPath = System.IO.Path.Combine(GameController.GamePath, "ContentPacks", packName, "splash.png");
        try
        {
            if (System.IO.File.Exists(splashPath) == true)
            {
                using System.IO.Stream stream = System.IO.File.Open(splashPath, System.IO.FileMode.OpenOrCreate);
                _pInfoSplash = Texture2D.FromStream(Core.GraphicsDevice, stream);
            }
        }
        catch (Exception ex)
        {
            Logger.Log(Logger.LogTypes.ErrorMessage, "NewOptionScreen.cs/ButtonPackInformation: " + ex.Message);
        }

        String[] s = ContentPackManager.GetContentPackInfo(packName);

        _pInfoVersion = String.Empty;
        _pInfoAuthor = String.Empty;
        _pInfoDescription = String.Empty;

        if (s.Length > 0) _pInfoVersion = s[0].CropStringToWidth(FontManager.InGameFont, (int)(540 - 16 - FontManager.InGameFont.MeasureString(Localization.GetString("option_screen_contentpacks_version")).X));
        if (s.Length > 1) _pInfoAuthor = s[1].CropStringToWidth(FontManager.InGameFont, (int)(540 - 16 - FontManager.InGameFont.MeasureString(Localization.GetString("option_screen_contentpacks_by")).X));
        if (s.Length > 2) _pInfoDescription = s[2].CropStringToWidth(FontManager.InGameFont, 540);
        _pInfoName = packName;
    }

    private void DrawPackInformationMenu()
    {
        if (_pInfoSplash != null)
            Core.SpriteBatch.Draw(_pInfoSplash, new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), Color.White);

        Canvas.DrawRectangle(
            new Rectangle(
                (int)(Core.windowSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(Localization.GetString("option_screen_contentpacks_name") + ": " + _pInfoName).X / 2) - 32,
                (int)(Core.windowSize.Height / 2 - 144),
                (int)FontManager.InGameFont.MeasureString(Localization.GetString("option_screen_contentpacks_name") + ": " + _pInfoName).X + 64,
                64),
            new Color(77, 147, 198, (int)(255 * _interfaceFade * _pageFade)), false);

        Core.SpriteBatch.DrawString(FontManager.InGameFont, Localization.GetString("option_screen_contentpacks_name") + ": " + _pInfoName,
            new Vector2((int)(Core.windowSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(Localization.GetString("option_screen_contentpacks_name") + ": " + _pInfoName).X / 2) + 2, (int)(Core.windowSize.Height / 2 - 128 + 2)),
            new Color(0, 0, 0, (int)(255 * _interfaceFade * _pageFade)));
        Core.SpriteBatch.DrawString(FontManager.InGameFont, Localization.GetString("option_screen_contentpacks_name") + ": " + _pInfoName,
            new Vector2((int)(Core.windowSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(Localization.GetString("option_screen_contentpacks_name") + ": " + _pInfoName).X / 2), (int)(Core.windowSize.Height / 2 - 128)),
            new Color(255, 255, 255, (int)(255 * _interfaceFade * _pageFade)));

        Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2) - 278, (int)(Core.windowSize.Height / 2 - 72), 556, 196), new Color(255, 255, 255, (int)(255 * _interfaceFade * _pageFade)), false);
        Core.SpriteBatch.DrawString(FontManager.InGameFont,
            Localization.GetString("option_screen_contentpacks_version") + ": " + _pInfoVersion + Environment.NewLine +
            Localization.GetString("option_screen_contentpacks_by") + ": " + _pInfoAuthor + Environment.NewLine +
            Localization.GetString("option_screen_contentpacks_description") + ": " + Environment.NewLine +
            _pInfoDescription.Replace("<br>", Environment.NewLine).Replace("~", Environment.NewLine),
            new Vector2((int)(Core.windowSize.Width / 2) - 278 + 16, (int)(Core.windowSize.Height / 2 - 64)),
            new Color(0, 0, 0, (int)(255 * _interfaceFade * _pageFade)));
    }

    private void UpdatePackInformationMenu()
    {
        bool back = KeyBoardHandler.KeyPressed(KeyBindings.EscapeKey) || KeyBoardHandler.KeyPressed(KeyBindings.BackKey1) ||
                    KeyBoardHandler.KeyPressed(KeyBindings.BackKey2) || MouseHandler.ButtonPressed(MouseHandler.MouseButtons.RightButton) ||
                    ControllerHandler.ButtonPressed(Buttons.B);
        if (back == true && _pageClosing == false && _pageOpening == false)
            SwitchToContentPacks();
    }

    private void ButtonUp()
    {
        if (_packNames.Count > 0)
        {
            if (_enabledPackNames.Contains(_packNames[_packsMenuIndex[0]]) == true)
            {
                int idx = _enabledPackNames.IndexOf(_packNames[_packsMenuIndex[0]]);
                if (idx > 0)
                {
                    String temp = _enabledPackNames[idx - 1];
                    _enabledPackNames[idx - 1] = _enabledPackNames[idx];
                    _enabledPackNames[idx] = temp;
                    GetPacks(true);
                }
            }
        }
    }

    private void ButtonDown()
    {
        if (_packNames.Count > 0)
        {
            if (_enabledPackNames.Contains(_packNames[_packsMenuIndex[0]]) == true)
            {
                int idx = _enabledPackNames.IndexOf(_packNames[_packsMenuIndex[0]]);
                if (idx < _enabledPackNames.Count - 1)
                {
                    String temp = _enabledPackNames[idx + 1];
                    _enabledPackNames[idx + 1] = _enabledPackNames[idx];
                    _enabledPackNames[idx] = temp;
                    GetPacks(true);
                }
            }
        }
    }

    private void PackEnabledToggle(ToggleButton c)
    {
        if (_packNames.Count > 0)
        {
            if (_enabledPackNames.Contains(_packNames[_packsMenuIndex[0]]) == true)
            {
                isSelectedEnabled = false;
                c.Toggled = false;
                ButtonToggle(_packNames[_packsMenuIndex[0]]);
            }
            else
            {
                isSelectedEnabled = true;
                c.Toggled = true;
                ButtonToggle(_packNames[_packsMenuIndex[0]]);
            }
        }
        else
        {
            isSelectedEnabled = false;
            c.Toggled = false;
        }
    }

    private void ButtonToggle(String packName)
    {
        if (_packNames.Count > 0)
        {
            if (_enabledPackNames.Contains(packName) == true)
                _enabledPackNames.Remove(packName);
            else
                _enabledPackNames.Add(packName);
        }
        GetPacks(true);
    }

    private void PacksApply()
    {
        if (_packNames.Count > 0)
        {
            Core.GameOptions.ContentPackNames = _enabledPackNames.ToArray();
            Core.GameOptions.SaveOptions();
            FontManager.LoadFonts();
            MusicManager.PlayNoMusic();
            ContentPackManager.Clear();
            GameModeManager.ForceWaterSpeed = -1;

            foreach (String s in Core.GameOptions.ContentPackNames)
                ContentPackManager.Load(System.IO.Path.Combine(GameController.GamePath, "ContentPacks", s, "exceptions.dat"));

            Core.OffsetMaps.Clear();

            Screen pressStart = PreScreen!.PreScreen!;
            PreScreen = new NewMainMenuScreen(pressStart);
            Core.SetScreen(new NewOptionScreen(PreScreen, 4));

            NewMainMenuScreen mainMenu = (NewMainMenuScreen)PreScreen;
            ((NewOptionScreen)Core.CurrentScreen!)._opening = false;
            ((NewOptionScreen)Core.CurrentScreen!)._interfaceFade = 1;
            ((NewOptionScreen)Core.CurrentScreen!)._enrollY = 400;
            mainMenu._fadeInMain = 1.0f;
            mainMenu._fadeInOptions = 1.0f;
            mainMenu._screenOffsetTarget.Y = 0 - 180 - 32;
            mainMenu._screenOffset.Y = mainMenu._screenOffsetTarget.Y;
            mainMenu._screenOffsetTarget.X = 180;
            mainMenu._screenOffset.X = mainMenu._screenOffsetTarget.X;
            mainMenu._optionsOffsetTarget.X = -360;
            mainMenu._optionsOffset.X = mainMenu._optionsOffsetTarget.X;

            NewMainMenuScreen._selectedProfileTemp = 0;
            NewMainMenuScreen._selectedProfile = 3;
            NewMainMenuScreen._menuIndex = 2;

            SoundManager.PlaySound("save", false);
            MusicManager.Play("title");
        }
    }

    private void DrawBackground()
    {
        if (_texture == null) return;

        Color mainBackgroundColor = Color.White;
        if (_closing == true)
            mainBackgroundColor = new Color(255, 255, 255, (int)(255 * _interfaceFade));

        int halfWidth = (int)(Core.windowSize.Width / 2);
        int halfHeight = (int)(Core.windowSize.Height / 2);

        Canvas.DrawRectangle(new Rectangle(halfWidth - 400, halfHeight - 232, 260, 32), new Color(Screens.UI.ColorProvider.MainColor(false).R, Screens.UI.ColorProvider.MainColor(false).G, Screens.UI.ColorProvider.MainColor(false).B, mainBackgroundColor.A), false);
        Canvas.DrawRectangle(new Rectangle(halfWidth - 140, halfHeight - 216, 16, 16), new Color(Screens.UI.ColorProvider.MainColor(false).R, Screens.UI.ColorProvider.MainColor(false).G, Screens.UI.ColorProvider.MainColor(false).B, mainBackgroundColor.A), false);
        Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 140, halfHeight - 232, 16, 16), new Rectangle(80, 0, 16, 16), mainBackgroundColor);
        Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 124, halfHeight - 216, 16, 16), new Rectangle(80, 0, 16, 16), mainBackgroundColor);

        Core.SpriteBatch.DrawString(FontManager.ChatFont, Localization.GetString("option_screen_title", "Options"), new Vector2(halfWidth - 390, halfHeight - 228), mainBackgroundColor);

        for (int y = 0; y <= (int)_enrollY; y += 16)
        {
            for (int x = 0; x <= 800; x += 16)
                Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 400 + x, halfHeight - 200 + y, 16, 16), new Rectangle(64, 0, 4, 4), mainBackgroundColor);
        }

        int modRes = (int)_enrollY % 16;
        if (modRes > 0)
        {
            for (int x = 0; x <= 800; x += 16)
                Core.SpriteBatch.Draw(_texture, new Rectangle(halfWidth - 400 + x, (int)_enrollY + (halfHeight - 200), 16, modRes), new Rectangle(64, 0, 4, 4), mainBackgroundColor);
        }
    }

    private void DrawCursor()
    {
        if (_texture == null) return;
        Core.SpriteBatch.Draw(_texture, new Rectangle((int)_cursorPosition.X + 60, (int)_cursorPosition.Y - 28, 48, 48), new Rectangle(0, 0, 16, 16), new Color(255, 255, 255, (int)(255 * _interfaceFade)), 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
    }

    private void DrawCurrentPage()
    {
        foreach (Control c in _controlList)
            c.Draw();
    }

    private void DrawMessage() { }

    public override void Update()
    {
        PreScreen?.Update();

        if (_opening == true)
        {
            InitializeControls();
            _opening = false;
        }

        if (Core.windowSize.Width != _screenSize.Width || Core.windowSize.Height != _screenSize.Height)
        {
            _screenSize = new Size((int)Core.windowSize.Width, (int)Core.windowSize.Height);
            InitializeControls();
        }

        if (_closing == true)
        {
            if (_interfaceFade > 0f)
            {
                _interfaceFade = MathHelper.Lerp(0, _interfaceFade, 0.8f);
                if (_interfaceFade < 0f) _interfaceFade = 0f;
            }
            if (_enrollY > 0)
            {
                _enrollY = MathHelper.Lerp(0, _enrollY, 0.8f);
                if (_enrollY <= 0) _enrollY = 0;
            }
            if (_enrollY <= 2.0f)
                Core.SetScreen(PreScreen!);
        }
        else
        {
            int maxWindowHeight = 400;
            if (_enrollY < maxWindowHeight)
            {
                _enrollY = MathHelper.Lerp(maxWindowHeight, _enrollY, 0.8f);
                if (_enrollY >= maxWindowHeight) _enrollY = maxWindowHeight;
            }
            if (_interfaceFade < 1.0f)
            {
                _interfaceFade = MathHelper.Lerp(1.0f, _interfaceFade, 0.95f);
                if (_interfaceFade > 1.0f) _interfaceFade = 1.0f;
            }
            if (_itemIntro < 1.0f)
            {
                _itemIntro += 0.05f;
                if (_itemIntro > 1.0f) _itemIntro = 1.0f;
            }

            ChooseBox.Update();
            if (ChooseBox.Showing == false)
                TextBox.Update();

            if (_pageClosing == true)
            {
                _pageFade -= 0.07f;
                if (_pageFade <= 0f)
                {
                    _pageFade = 0f;
                    _pageClosing = false;
                    _pageOpening = true;
                    ScreenIndex = _nextIndex;
                    InitializeControls();
                }
            }
            if (_pageOpening == true)
            {
                _pageFade += 0.07f;
                if (_pageFade >= 1.0f)
                {
                    _pageFade = 1.0f;
                    _pageClosing = false;
                    _pageOpening = false;
                }
            }

            if (_cursorDestPosition.X != _cursorPosition.X || _cursorDestPosition.Y != _cursorPosition.Y)
            {
                _cursorPosition.X = MathHelper.Lerp(_cursorDestPosition.X, _cursorPosition.X, 0.75f);
                _cursorPosition.Y = MathHelper.Lerp(_cursorDestPosition.Y, _cursorPosition.Y, 0.75f);
                if (Math.Abs(_cursorDestPosition.X - _cursorPosition.X) < 0.1f) _cursorPosition.X = _cursorDestPosition.X;
                if (Math.Abs(_cursorDestPosition.Y - _cursorPosition.Y) < 0.1f) _cursorPosition.Y = _cursorDestPosition.Y;
            }

            if (_selectedScrollBar == false)
            {
                if (Controls.Up(true, true, false, true, true, true) == true) SetCursorPosition("up");
                if (Controls.Down(true, true, false, true, true, true) == true) SetCursorPosition("down");
                if (Controls.Right(true, true, false, true, true, true) == true) SetCursorPosition("right");
                if (Controls.Left(true, true, false, true, true, true) == true) SetCursorPosition("left");
                if (ScreenIndex != 6)
                {
                    if (Controls.Left(false, false, true, false, false, false) == true) SetCursorPosition("previous");
                    if (Controls.Right(false, false, true, false, false, false) == true) SetCursorPosition("next");
                }

                bool back = KeyBoardHandler.KeyPressed(KeyBindings.EscapeKey) || KeyBoardHandler.KeyPressed(KeyBindings.BackKey1) ||
                            KeyBoardHandler.KeyPressed(KeyBindings.BackKey2) || MouseHandler.ButtonPressed(MouseHandler.MouseButtons.RightButton) ||
                            ControllerHandler.ButtonPressed(Buttons.B);
                if (back == true && _pageClosing == false && _pageOpening == false)
                {
                    SoundManager.PlaySound("select");
                    if (ScreenIndex == 0 || _subMenu != 0)
                    {
                        if (ScreenIndex == 8)
                            SwitchToContentPacks();
                        else
                            Close();
                    }
                    else
                    {
                        SwitchToMain();
                    }
                }
            }

            switch (ScreenIndex)
            {
                case 6: UpdateLanguageMenu(); break;
                case 7: UpdatePacksMenu(); break;
                case 8: UpdatePackInformationMenu(); break;
            }

            for (int i = 0; i < _controlList.Count; i++)
            {
                if (i <= _controlList.Count - 1)
                    _controlList[i].Update(this);
            }
        }
    }

    private void SetCursorPosition(String direction)
    {
        Vector2 pos = GetButtonPosition(direction);
        Vector2 cPosition = new Vector2(pos.X, pos.Y);

        bool hasScrollControl = false;
        foreach (Control control in _controlList)
        {
            if (control.position == new Vector2(pos.X, pos.Y) && control.controlType == "ScrollBar")
            {
                hasScrollControl = true;
                break;
            }
        }

        _cursorDestPosition = hasScrollControl == true ? new Vector2(cPosition.X + 332, cPosition.Y) : cPosition;
    }

    private Vector2 GetButtonPosition(String direction)
    {
        List<Control> eligibleControls = [];
        Control? currentControl = null;

        foreach (Control control in _controlList)
        {
            if (control.controlType == "ScrollBar")
            {
                if (control.position.Y == _cursorDestPosition.Y)
                {
                    currentControl = control;
                    break;
                }
            }
            else
            {
                if (control.position == _cursorDestPosition)
                {
                    currentControl = control;
                    break;
                }
            }
        }

        if (currentControl == null) return _cursorDestPosition;

        foreach (Control control in _controlList)
        {
            Vector2 r2 = control.position;
            Vector2 r1 = currentControl.position;

            if (r1 == r2) continue;

            switch (direction)
            {
                case "up":
                    if (ScreenIndex == 0)
                    {
                        switch (currentControl.ID)
                        {
                            case 4: if (control.ID == 1) eligibleControls.Add(control); break;
                            case 5: if (control.ID == 3) eligibleControls.Add(control); break;
                            case 6: case 7: if (control.ID == 4) eligibleControls.Add(control); break;
                            case 8: if (control.ID == 5) eligibleControls.Add(control); break;
                        }
                    }
                    else if (ScreenIndex == 7)
                    {
                        if (currentControl.ID <= 4 && control.ID == currentControl.ID - 1)
                            eligibleControls.Add(control);
                    }
                    else if (ScreenIndex == 5)
                    {
                        if (currentControl.ID > 3)
                        {
                            if (control.ID == 3) eligibleControls.Add(control);
                        }
                        else if (control.ID == currentControl.ID - 1)
                        {
                            eligibleControls.Add(control);
                        }
                    }
                    else if (ScreenIndex == 4 && currentControl.ID == 7 && PreScreen!.Identification != Identifications.MainMenuScreen)
                    {
                        if (control.ID == 5) eligibleControls.Add(control);
                    }
                    else if (Math.Abs(r2.X - r1.X) <= -(r2.Y - r1.Y))
                    {
                        eligibleControls.Add(control);
                    }
                    break;

                case "down":
                    if (ScreenIndex == 0)
                    {
                        switch (currentControl.ID)
                        {
                            case 1: case 2: if (control.ID == 4) eligibleControls.Add(control); break;
                            case 3: if (control.ID == 5) eligibleControls.Add(control); break;
                            case 4: if (control.ID == 6) eligibleControls.Add(control); break;
                            case 5: if (control.ID == 8) eligibleControls.Add(control); break;
                        }
                    }
                    else if (ScreenIndex == 5)
                    {
                        if (currentControl.ID < 4 && control.ID == currentControl.ID + 1)
                            eligibleControls.Add(control);
                    }
                    else if (ScreenIndex == 4 && currentControl.ID == 5 && PreScreen!.Identification != Identifications.MainMenuScreen)
                    {
                        if (control.ID == 7) eligibleControls.Add(control);
                    }
                    else if (Math.Abs(r2.X - r1.X) <= -(r1.Y - r2.Y))
                    {
                        eligibleControls.Add(control);
                    }
                    break;

                case "right":
                    if (ScreenIndex == 7)
                    {
                        if (currentControl.ID == 5 && control.ID == 6) eligibleControls.Add(control);
                        else if (currentControl.ID == 6 && control.ID == 4) eligibleControls.Add(control);
                    }
                    else if (control.ID == currentControl.ID + 1)
                    {
                        eligibleControls.Add(control);
                    }
                    break;

                case "left":
                    if (ScreenIndex == 7)
                    {
                        if (currentControl.ID <= 4 && control.ID == 6) eligibleControls.Add(control);
                        else if (currentControl.ID == 6 && control.ID == 5) eligibleControls.Add(control);
                    }
                    else if (control.ID == currentControl.ID - 1)
                    {
                        eligibleControls.Add(control);
                    }
                    break;

                case "next":
                    if (ScreenIndex == 7 && currentControl.ID < 4)
                    {
                        if (control.ID == currentControl.ID + 1) eligibleControls.Add(control);
                    }
                    else if (ScreenIndex != 7)
                    {
                        if (control.ID == currentControl.ID + 1) eligibleControls.Add(control);
                    }
                    break;

                case "previous":
                    if (ScreenIndex == 7 && currentControl.ID <= 4)
                    {
                        if (control.ID == currentControl.ID - 1) eligibleControls.Add(control);
                    }
                    else if (ScreenIndex != 7)
                    {
                        if (control.ID == currentControl.ID - 1) eligibleControls.Add(control);
                    }
                    break;
            }
        }

        Vector2 nextPosition = new Vector2(currentControl.position.X, currentControl.position.Y);
        double cDistance = 99999d;
        foreach (Control control in eligibleControls)
        {
            Vector2 deltaR = control.position - currentControl.position;
            double distance = deltaR.Length();
            if (distance < cDistance)
            {
                nextPosition = control.position;
                cDistance = distance;
            }
        }

        return nextPosition;
    }

    private void InitializeControls()
    {
        _controlList.Clear();
        _selectedScrollBar = false;

        int halfWidth = (int)(Core.windowSize.Width / 2);
        int halfHeight = (int)(Core.windowSize.Height / 2);
        int deltaX = halfWidth - 400;
        int deltaY = halfHeight - 200;

        switch (ScreenIndex)
        {
            case 0:
                _controlList.Add(new CommandButton(new Vector2(deltaX + 90, deltaY + 80), 1, 64, Localization.GetString("option_screen_game", "Game"), SwitchToGame, 1));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 310, deltaY + 80), 1, 64, Localization.GetString("option_screen_graphics", "Graphics"), SwitchToGraphics, 2));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 530, deltaY + 80), 1, 64, Localization.GetString("option_screen_battle", "Battle"), SwitchToBattle, 3));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 200, deltaY + 168), 1, 64, Localization.GetString("option_screen_controls", "Controls"), SwitchToControls, 4));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 420, deltaY + 168), 1, 64, Localization.GetString("option_screen_audio", "Audio"), SwitchToAudio, 5));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 90 + 24, deltaY + 336), 1, 48, Localization.GetString("global_apply", "Apply"), Apply, 6));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 286 + 24, deltaY + 336), 2, 48, Localization.GetString("option_screen_resetoptions", "Reset Options"), Reset, 7));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 530 + 24, deltaY + 336), 1, 48, Localization.GetString("global_close", "Close"), Close, 8));
                break;

            case 1:
                _controlList.Add(new ScrollBar(new Vector2(deltaX + 100, deltaY + 60), 400, Localization.GetString("option_screen_game_textspeed", "Text Speed"), _textSpeed, 1, 4, ChangeTextspeed, 1));
                if (bool.Parse(GameModeManager.GetGameRuleValue("LockDifficulty", "0")) == false)
                {
                    Dictionary<int, String> d = [];
                    d.Add(0, Localization.GetString("option_screen_game_difficulty_easy", "Easy"));
                    d.Add(1, Localization.GetString("option_screen_game_difficulty_hard", "Hard"));
                    d.Add(2, Localization.GetString("option_screen_game_difficulty_superhard", "Super Hard"));
                    _controlList.Add(new ScrollBar(new Vector2(deltaX + 100, deltaY + 120), 400, Localization.GetString("option_screen_game_difficulty", "Difficulty"), _difficulty, 0, 2, ChangeDifficulty, d, 2));
                }
                {
                    Dictionary<int, String> s = [];
                    s.Add(0, Localization.GetString("option_screen_game_interfacescale_automatic", "Automatic"));
                    s.Add(1, "0.5x");
                    s.Add(2, "1x");
                    s.Add(3, "2x");
                    _controlList.Add(new ScrollBar(new Vector2(deltaX + 100, deltaY + 180), 400, Localization.GetString("option_screen_game_interfacescale", "Interface Scale"), _interfaceScale, 0, 3, ChangeInterfaceScale, s, 3));
                }
                _controlList.Add(new ToggleButton(new Vector2(deltaX + 100, deltaY + 240), 3, 64, Localization.GetString("option_screen_game_viewbobbing", "View Bobbing"), _viewBobbing, ToggleBobbing, [Localization.GetString("global_off", "Off"), Localization.GetString("global_on", "On")], 4));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 530 + 24, deltaY + 336), 1, 48, Localization.GetString("global_back", "Back"), SwitchToMain, 5));
                break;

            case 2:
                _controlList.Add(new ScrollBar(new Vector2(deltaX + 100, deltaY + 40), 400, Localization.GetString("option_screen_graphics_fov", "Field of View"), (int)_fov, 45, 120, ChangeFOV, 1));
                {
                    Dictionary<int, String> d = [];
                    d.Add(0, Localization.GetString("option_screen_graphics_renderdistance_tiny", "Tiny"));
                    d.Add(1, Localization.GetString("option_screen_graphics_renderdistance_short", "Short"));
                    d.Add(2, Localization.GetString("option_screen_graphics_renderdistance_normal", "Normal"));
                    d.Add(3, Localization.GetString("option_screen_graphics_renderdistance_far", "Far"));
                    d.Add(4, Localization.GetString("option_screen_graphics_renderdistance_extreme", "Extreme"));
                    _controlList.Add(new ScrollBar(new Vector2(deltaX + 100, deltaY + 100), 400, Localization.GetString("option_screen_graphics_renderdistance", "Render Distance"), _renderDistance, 0, 4, ChangeRenderDistance, d, 2));
                }
                {
                    Dictionary<int, String> d1 = [];
                    d1.Add(0, Localization.GetString("option_screen_graphics_offset_mapquality_off", "Off"));
                    _controlList.Add(new ScrollBar(new Vector2(deltaX + 100, deltaY + 160), 400, Localization.GetString("option_screen_graphics_offset_mapquality", "Offset Map Quality"), _loadOffsetMaps, 0, 100, ChangeOffsetMaps, d1, 3));
                }
                _controlList.Add(new ToggleButton(new Vector2(deltaX + 100, deltaY + 220), 3, 64, Localization.GetString("option_screen_graphics_graphics", "Graphics"), _graphicStyle != 0, ToggleGraphicsStyle, [Localization.GetString("option_screen_graphics_graphics_fast", "Fast"), Localization.GetString("option_screen_graphics_graphics_fancy", "Fancy")], 4));
                _controlList.Add(new ToggleButton(new Vector2(deltaX + 100, deltaY + 300), 3, 64, Localization.GetString("option_screen_graphics_multisampling", "Multi Sampling"), _preferMultiSampling, ToggleMultiSampling, [Localization.GetString("global_off", "Off"), Localization.GetString("global_on", "On")], 5));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 530 + 24, deltaY + 336), 1, 48, Localization.GetString("global_back", "Back"), SwitchToMain, 6));
                break;

            case 3:
                _controlList.Add(new ToggleButton(new Vector2(deltaX + 100 + 20, deltaY + 100), 2, 64, Localization.GetString("option_screen_battle_3dmodels", "3D Models"), _showModels != 0, ToggleShowModels, [Localization.GetString("global_off", "Off"), Localization.GetString("global_on", "On")], 1));
                _controlList.Add(new ToggleButton(new Vector2(deltaX + 400 + 20, deltaY + 100), 2, 64, Localization.GetString("option_screen_battle_animations", "Animations"), _showBattleAnimations != 0, ToggleAnimations, [Localization.GetString("global_off", "Off"), Localization.GetString("global_on", "On")], 2));
                _controlList.Add(new ToggleButton(new Vector2(deltaX + 100 + 20, deltaY + 200), 2, 64, Localization.GetString("option_screen_battle_battlestyle", "Battle Style"), _battleStyle != 0, ToggleBattleStyle, [Localization.GetString("option_screen_battle_battlestyle_shift", "Shift"), Localization.GetString("option_screen_battle_battlestyle_set", "Set")], 3));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 530 + 24, deltaY + 336), 1, 48, Localization.GetString("global_back", "Back"), SwitchToMain, 4));
                break;

            case 4:
                if (PreScreen!.Identification == Identifications.MainMenuScreen)
                {
                    _controlList.Add(new ToggleButton(new Vector2(deltaX + 58, deltaY + 100), 3, 64, Localization.GetString("option_screen_controls_xboxgamepad", "Xbox Gamepad"), _gamePadEnabled, ToggleXBOX360Controller, [Localization.GetString("global_disabled", "Disabled"), Localization.GetString("global_enabled", "Enabled")], 1));
                    _controlList.Add(new ToggleButton(new Vector2(deltaX + 58 + (5 * 64) + 16, deltaY + 100), 4, 64, Localization.GetString("option_screen_controls_invertrightstick_x", "Invert Right Stick X"), _gamePadInvertRightStick.X != 0, ToggleInvertRightStickX, [Localization.GetString("global_off", "Off"), Localization.GetString("global_on", "On")], 2));
                    _controlList.Add(new CommandButton(new Vector2(deltaX + 58, deltaY + 200), 3, 64, Localization.GetString("option_screen_controls_resetkeybindings", "Reset Key Bindings"), ResetKeyBindings, 3));
                    _controlList.Add(new ToggleButton(new Vector2(deltaX + 58 + (5 * 64) + 16, deltaY + 200), 4, 64, Localization.GetString("option_screen_controls_invertrightstick_y", "Invert Right Stick Y"), _gamePadInvertRightStick.Y != 0, ToggleInvertRightStickY, [Localization.GetString("global_off", "Off"), Localization.GetString("global_on", "On")], 4));
                    _controlList.Add(new CommandButton(new Vector2(deltaX + 122 + 24, deltaY + 336), 1, 48, Localization.GetString("global_apply", "Apply"), ControlsApply, 5));
                    _controlList.Add(new CommandButton(new Vector2(deltaX + 490 + 24, deltaY + 336), 1, 48, Localization.GetString("global_back", "Back"), Close, 6));
                }
                else
                {
                    Dictionary<int, String> d = [];
                    d.Add(1, Localization.GetString("option_screen_controls_cameraspeed_slow", "...Slow..."));
                    d.Add(12, Localization.GetString("option_screen_controls_cameraspeed_medium", "Standard"));
                    d.Add(38, Localization.GetString("option_screen_controls_cameraspeed_fast", "Super fast!"));
                    d.Add(50, Localization.GetString("option_screen_controls_cameraspeed_fastest", "SPEED OF LIGHT!"));
                    _controlList.Add(new ScrollBar(new Vector2(deltaX + 58, deltaY + 60), 400, Localization.GetString("option_screen_controls_cameraspeed", "Camera Speed"), _cameraSpeed, 1, 50, ChangeCameraSpeed, d, 1));
                    _controlList.Add(new ToggleButton(new Vector2(deltaX + 58, deltaY + 120), 3, 64, Localization.GetString("option_screen_controls_xboxgamepad", "Xbox Gamepad"), _gamePadEnabled, ToggleXBOX360Controller, [Localization.GetString("global_disabled", "Disabled"), Localization.GetString("global_enabled", "Enabled")], 2));
                    _controlList.Add(new ToggleButton(new Vector2(deltaX + 58 + (5 * 64) + 16, deltaY + 120), 4, 64, Localization.GetString("option_screen_controls_invertrightstick_x", "Invert Right Stick X"), _gamePadInvertRightStick.X != 0, ToggleInvertRightStickX, [Localization.GetString("global_off", "Off"), Localization.GetString("global_on", "On")], 3));
                    _controlList.Add(new ToggleButton(new Vector2(deltaX + 58, deltaY + 200), 3, 64, Localization.GetString("option_screen_controls_running", "Running"), _runMode, ToggleRunningToggle, [Localization.GetString("option_screen_controls_running_hold", "Hold"), Localization.GetString("option_screen_controls_running_toggle", "Toggle")], 4));
                    _controlList.Add(new ToggleButton(new Vector2(deltaX + 58 + (5 * 64) + 16, deltaY + 200), 4, 64, Localization.GetString("option_screen_controls_invertrightstick_y", "Invert Right Stick Y"), _gamePadInvertRightStick.Y != 0, ToggleInvertRightStickY, [Localization.GetString("global_off", "Off"), Localization.GetString("global_on", "On")], 5));
                    _controlList.Add(new CommandButton(new Vector2(deltaX + 58, deltaY + 280), 3, 64, Localization.GetString("option_screen_controls_resetkeybindings", "Reset Key Bindings"), ResetKeyBindings, 6));
                    _controlList.Add(new CommandButton(new Vector2(deltaX + 530 + 24, deltaY + 336), 1, 48, Localization.GetString("global_back", "Back"), SwitchToMain, 7));
                }
                break;

            case 5:
                _controlList.Add(new ScrollBar(new Vector2(deltaX + 100, deltaY + 60), 400, Localization.GetString("option_screen_audio_volume_music", "Music Volume"), _music, 0, 100, ChangeMusicVolume, 1));
                _controlList.Add(new ScrollBar(new Vector2(deltaX + 100, deltaY + 120), 400, Localization.GetString("option_screen_audio_volume_sfx", "SoundFX Volume"), _sound, 0, 100, ChangeSoundVolume, 2));
                _controlList.Add(new ToggleButton(new Vector2(deltaX + 100, deltaY + 200), 1, 64, Localization.GetString("option_screen_audio_muted", "Muted"), _muted != 0, ToggleMute, [Localization.GetString("global_no", "No"), Localization.GetString("global_yes", "Yes")], 3));
                if (PreScreen!.Identification == Identifications.MainMenuScreen)
                {
                    _controlList.Add(new CommandButton(new Vector2(deltaX + 90 + 24, deltaY + 336), 1, 48, Localization.GetString("global_apply", "Apply"), AudioSave, 4));
                    _controlList.Add(new CommandButton(new Vector2(deltaX + 530 + 24, deltaY + 336), 1, 48, Localization.GetString("global_back", "Back"), Close, 5));
                }
                else
                {
                    _controlList.Add(new CommandButton(new Vector2(deltaX + 530 + 24, deltaY + 336), 1, 48, Localization.GetString("global_back", "Back"), SwitchToMain, 4));
                }
                break;

            case 6:
                _controlList.Add(new CommandButton(new Vector2(deltaX + 90 + 24, deltaY + 336), 1, 48, Localization.GetString("global_apply", "Apply"), LanguageApply, 1));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 530 + 24, deltaY + 336), 1, 48, Localization.GetString("global_back", "Back"), Close, 2));
                break;

            case 7:
                _controlList.Add(new CommandButton(new Vector2(deltaX + 604, deltaY + 64), 2, 48, Localization.GetString("option_screen_contentpacks_up"), ButtonUp, 1));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 604, deltaY + 120), 2, 48, Localization.GetString("option_screen_contentpacks_down"), ButtonDown, 2));
                _controlList.Add(new ToggleButton(new Vector2(deltaX + 604, deltaY + 176), 2, 48, String.Empty, isSelectedEnabled, PackEnabledToggle, [Localization.GetString("global_enable"), Localization.GetString("global_disable")], 3));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 604, deltaY + 232), 2, 48, Localization.GetString("option_screen_contentpacks_information"), SwitchToPackInformation, 4));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 90 + 24, deltaY + 336), 1, 48, Localization.GetString("global_apply", "Apply"), PacksApply, 5));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 310 + 24, deltaY + 336), 1, 48, Localization.GetString("global_back", "Back"), Close, 6));
                break;

            case 8:
                _controlList.Add(new ToggleButton(new Vector2(deltaX + 90 + 24, deltaY + 336), 2, 48, Localization.GetString("global_enabled"), isSelectedEnabled, PackEnabledToggle, [Localization.GetString("global_no"), Localization.GetString("global_yes")], 1));
                _controlList.Add(new CommandButton(new Vector2(deltaX + 530 + 24, deltaY + 336), 1, 48, Localization.GetString("global_back", "Back"), SwitchToContentPacks, 2));
                break;
        }

        if (ScreenIndex != 7)
        {
            if (_controlList[0].controlType == "ScrollBar")
                _cursorDestPosition = new Vector2(_controlList[0].position.X + 332, _controlList[0].position.Y);
            else
                _cursorDestPosition = _controlList[0].position;
        }
        else
        {
            if (_controlList[0].controlType == "ScrollBar")
                _cursorDestPosition = new Vector2(_controlList[4].position.X + 332, _controlList[4].position.Y);
            else
                _cursorDestPosition = _controlList[4].position;
        }
    }

    private void Apply(CommandButton c) { Save(); Close(c); }

    private void Close(CommandButton c) { Close(); }

    private void Close()
    {
        if (_currentLanguage != _tempLanguage)
            Localization.Load(_tempLanguage);
        if (MusicManager.MasterVolume * 100 != _tempMusicVolume)
            MusicManager.MasterVolume = _tempMusicVolume / 100f;
        if (SoundManager.Volume * 100 != _tempSoundVolume)
            SoundManager.Volume = _tempSoundVolume / 100f;
        if (MusicManager.Muted != (_tempMuted != 0) || SoundManager.Muted != (_tempMuted != 0))
        {
            MusicManager.Muted = _tempMuted != 0;
            SoundManager.Muted = _tempMuted != 0;
        }
        _closing = true;
    }

    private void ControlsApply(CommandButton c)
    {
        Core.GameOptions.GamePadEnabled = _gamePadEnabled;
        Core.GameOptions.GamePadInvertRightStick = _gamePadInvertRightStick;
        Core.GameOptions.SaveOptions();
        SoundManager.PlaySound("save");
        _closing = true;
    }

    private void Reset(CommandButton c)
    {
        _fov = 60.0f;
        _textSpeed = 2;
        _cameraSpeed = 12;
        _music = 50;
        _sound = 50;
        _renderDistance = 2;
        _graphicStyle = 1;
        _interfaceScale = 0;
        _showBattleAnimations = 1;
        _diagonalMovement = false;
        _difficulty = 0;
        _battleStyle = 1;
        _loadOffsetMaps = 100;
        _viewBobbing = true;
        _showModels = 1;
        _muted = 0;
        _gamePadEnabled = true;
        _preferMultiSampling = true;
        MusicManager.Muted = _muted != 0;
        SoundManager.Muted = _muted != 0;
    }

    private void Save()
    {
        MusicManager.MasterVolume = _music / 100f;
        SoundManager.Volume = _sound / 100f;
        MusicManager.Muted = _muted != 0;
        SoundManager.Muted = _muted != 0;
        _tempMusicVolume = (int)(MusicManager.MasterVolume * 100);
        _tempSoundVolume = (int)(SoundManager.Volume * 100);
        _tempMuted = int.Parse(MusicManager.Muted.ToNumberString());
        Core.GameOptions.RenderDistance = _renderDistance;
        Core.GameOptions.GraphicStyle = _graphicStyle;
        Core.GameOptions.InterfaceScale = _interfaceScale;
        if (PreScreen!.Identification != Identifications.MainMenuScreen)
        {
            Camera.CreateNewProjection(_fov);
            TextBox.TextSpeed = _textSpeed;
            Camera.RotationSpeed = _cameraSpeed / 10000f;
            Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
            Core.Player.RunMode = _runMode;
            PreScreen.Update();
        }
        Core.Player.ShowBattleAnimations = _showBattleAnimations;
        Core.Player.DiagonalMovement = _diagonalMovement;
        Core.Player.DifficultyMode = _difficulty;
        Core.Player.BattleStyle = _battleStyle;
        Core.Player.ShowModelsInBattle = _showModels != 0;
        Core.GameOptions.GamePadEnabled = _gamePadEnabled;
        Core.GameOptions.GamePadInvertRightStick = _gamePadInvertRightStick;
        Core.GraphicsManager.PreferMultiSampling = _preferMultiSampling;
        Core.GameOptions.LoadOffsetMaps = _loadOffsetMaps == 0 ? 0 : 101 - _loadOffsetMaps;
        Core.GameOptions.ViewBobbing = _viewBobbing;
        Core.GameOptions.SaveOptions();
        SoundManager.PlaySound("save");
    }

    public override void ToggledMute()
    {
        if (ScreenIndex == 5)
        {
            _muted = int.Parse(MusicManager.Muted.ToNumberString());
            InitializeControls();
        }
    }

    private void ButtonUp(CommandButton c) { ButtonUp(); }
    private void ButtonDown(CommandButton c) { ButtonDown(); }
    private void PacksApply(CommandButton c) { PacksApply(); }
    private void SwitchToMain(CommandButton c) { SwitchToMain(); }
    private void SwitchToGame(CommandButton c) { SwitchToGame(); }
    private void SwitchToGraphics(CommandButton c) { SwitchToGraphics(); }
    private void SwitchToBattle(CommandButton c) { SwitchToBattle(); }
    private void SwitchToControls(CommandButton c) { SwitchToControls(); }
    private void SwitchToAudio(CommandButton c) { SwitchToAudio(); }
    private void SwitchToPackInformation(CommandButton c) { SwitchToPackInformation(); }
    private void SwitchToContentPacks(CommandButton c) { SwitchToContentPacks(); }

    private void SwitchToMain() { _nextIndex = 0; _pageClosing = true; }
    private void SwitchToGame() { _nextIndex = 1; _pageClosing = true; }
    private void SwitchToGraphics() { _nextIndex = 2; _pageClosing = true; }
    private void SwitchToBattle() { _nextIndex = 3; _pageClosing = true; }

    private void SwitchToControls()
    {
        _nextIndex = 4;
        if (PreScreen!.Identification == Identifications.MainMenuScreen)
        {
            ScreenIndex = _nextIndex;
            InitializeControls();
        }
        else
        {
            _pageClosing = true;
        }
    }

    private void SwitchToAudio()
    {
        _nextIndex = 5;
        if (PreScreen!.Identification == Identifications.MainMenuScreen)
        {
            ScreenIndex = _nextIndex;
            InitializeControls();
        }
        else
        {
            _pageClosing = true;
        }
    }

    private void SwitchToLanguage()
    {
        GetLanguages();
        if (_languages.Contains(_currentLanguage) == true)
        {
            languageMenuIndex[0] = _languages.IndexOf(_currentLanguage);
            _tempLanguage = _currentLanguage;
        }
        languageMenuIndex[1] = 0;
        languageMenuIndex[2] = 0;
        if (languageMenuIndex[0] > 3)
            languageMenuIndex[2] = languageMenuIndex[0];
        languageMenuIndex[2] = (int)MathHelper.Clamp(languageMenuIndex[2], 0, _languages.Count - 4);
        _nextIndex = 6;
        ScreenIndex = _nextIndex;
        InitializeControls();
    }

    private void SwitchToContentPacks()
    {
        if (ScreenIndex != 7 && ScreenIndex != 8)
        {
            GetPacks();
            _packsMenuIndex[0] = 0;
            _packsMenuIndex[1] = 0;
            _packsMenuIndex[2] = 0;
        }
        _nextIndex = 7;
        if (ScreenIndex == 8)
            _pageClosing = true;
        else
            ScreenIndex = _nextIndex;
        InitializeControls();
    }

    private void SwitchToPackInformation()
    {
        if (_packNames.Count > 0)
        {
            _nextIndex = 8;
            _pageClosing = true;
        }
        ButtonPackInformation();
    }

    private void ChangeFOV(ScrollBar c) { _fov = c.Value; }
    private void ChangeRenderDistance(ScrollBar c) { _renderDistance = c.Value; }
    private void ToggleGraphicsStyle(ToggleButton c) { _graphicStyle = c.Toggled == true ? 1 : 0; }
    private void ChangeOffsetMaps(ScrollBar c) { _loadOffsetMaps = c.Value; }
    private void ToggleMultiSampling(ToggleButton c) { _preferMultiSampling = !_preferMultiSampling; }
    private void ToggleBobbing(ToggleButton c) { _viewBobbing = !_viewBobbing; }
    private void ChangeTextspeed(ScrollBar c) { _textSpeed = c.Value; }
    private void ChangeDifficulty(ScrollBar c) { _difficulty = c.Value; }
    private void ChangeInterfaceScale(ScrollBar c) { _interfaceScale = c.Value; }
    private void ToggleShowModels(ToggleButton c) { _showModels = _showModels == 0 ? 1 : 0; }
    private void ToggleAnimations(ToggleButton c) { _showBattleAnimations = _showBattleAnimations != 1 ? 1 : 0; }
    private void ToggleBattleStyle(ToggleButton c) { _battleStyle = _battleStyle == 0 ? 1 : 0; }
    private void ToggleXBOX360Controller(ToggleButton c) { _gamePadEnabled = !_gamePadEnabled; }
    private void ToggleInvertRightStickX(ToggleButton c) { _gamePadInvertRightStick.X = _gamePadInvertRightStick.X == 0 ? 1 : 0; }
    private void ToggleInvertRightStickY(ToggleButton c) { _gamePadInvertRightStick.Y = _gamePadInvertRightStick.Y == 0 ? 1 : 0; }
    private void ToggleRunningToggle(ToggleButton c) { _runMode = !_runMode; }
    private void ChangeCameraSpeed(ScrollBar c) { _cameraSpeed = c.Value; }
    private void ResetKeyBindings(CommandButton c) { KeyBindings.CreateKeySave(true); KeyBindings.LoadKeys(); }

    private void ChangeMusicVolume(ScrollBar c) { _music = c.Value; ApplyAudioChange(); }
    private void ChangeSoundVolume(ScrollBar c) { _sound = c.Value; ApplyAudioChange(); }

    private void ToggleMute(ToggleButton c)
    {
        _muted = _muted == 0 ? 1 : 0;
        ApplyAudioChange();
    }

    private void ApplyAudioChange()
    {
        MusicManager.Muted = _muted != 0;
        SoundManager.Muted = _muted != 0;
        MusicManager.MasterVolume = _music / 100f;
        SoundManager.Volume = _sound / 100f;
    }

    private void AudioSave(CommandButton c)
    {
        MusicManager.MasterVolume = _music / 100f;
        SoundManager.Volume = _sound / 100f;
        MusicManager.Muted = _muted != 0;
        SoundManager.Muted = _muted != 0;
        Core.GameOptions.SaveOptions();
        SoundManager.PlaySound("save");
        _closing = true;
    }

    private void LanguageApply(CommandButton c)
    {
        if (_currentLanguage != _languages[languageMenuIndex[0]])
            _currentLanguage = _languages[languageMenuIndex[0]];
        Localization.Load(_currentLanguage);
        Core.GameOptions.SaveOptions();
        SoundManager.PlaySound("save");
        _closing = true;
    }

    // ---- Inner control classes ----

    private abstract class Control
    {
        public String controlType = String.Empty;
        public abstract void Draw();
        public abstract void Update(NewOptionScreen s);
        public Vector2 position = new Vector2(0);
        private int _size = 1;
        public int ID { get; set; }

        public int Size
        {
            get => _size;
            set => _size = value;
        }
    }

    private class ToggleButton : Control
    {
        private int _buttonWidth = 1;
        public bool Toggled { get; set; }
        public String Text { get; set; } = String.Empty;
        public List<String> Settings { get; set; } = [];

        public delegate void OnToggle(ToggleButton t);
        public OnToggle onToggleTrigger;

        public ToggleButton(Vector2 pos, int buttonWidth, int size, String text, bool toggled, OnToggle triggerSub, List<String> settings, int id)
        {
            position = pos;
            _buttonWidth = buttonWidth;
            controlType = "ToggleButton";
            Size = size;
            Text = text;
            Toggled = toggled;
            ID = id;
            onToggleTrigger = triggerSub;
            Settings = settings;
        }

        public override void Draw()
        {
            NewOptionScreen s = (NewOptionScreen)Core.CurrentScreen!;
            if (s._menuTexture == null) return;
            Color c = new Color(255, 255, 255, (int)(255 * s._interfaceFade * s._pageFade));
            String toggleDivider = Text == String.Empty ? String.Empty : ": ";
            Vector2 b;
            String t = Text;
            Color textColor;
            if (Toggled == true)
            {
                t += toggleDivider + Settings[1];
                b = new Vector2(16, 32);
                textColor = new Color(255, 255, 255, (int)(255 * s._interfaceFade * s._pageFade));
            }
            else
            {
                t += toggleDivider + Settings[0];
                b = new Vector2(16, 16);
                textColor = new Color(0, 0, 0, (int)(255 * s._interfaceFade * s._pageFade));
            }

            Core.SpriteBatch.Draw(s._menuTexture, new Rectangle((int)position.X, (int)position.Y, Size, Size), new Rectangle((int)b.X, (int)b.Y, 16, 16), c);
            Core.SpriteBatch.Draw(s._menuTexture, new Rectangle((int)position.X + Size, (int)position.Y, Size * _buttonWidth, Size), new Rectangle((int)b.X + 16, (int)b.Y, 16, 16), c);
            Core.SpriteBatch.Draw(s._menuTexture, new Rectangle((int)position.X + Size * (_buttonWidth + 1), (int)position.Y, Size, Size), new Rectangle((int)b.X, (int)b.Y, 16, 16), c, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);

            int fontWidth = (int)(FontManager.MainFont.MeasureString(t).X * 1.0f);
            Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2((int)(position.X + (Size * (2 + _buttonWidth) - fontWidth) * 0.5f), (int)position.Y + (int)(16 * Size / 64)), textColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
        }

        public override void Update(NewOptionScreen s)
        {
            if (ScreenIndex == 7 && ID == 3)
                Toggled = isSelectedEnabled;
            else if (ScreenIndex == 8)
                Toggled = isSelectedEnabled;

            Rectangle r = new Rectangle((int)position.X, (int)position.Y, (2 + _buttonWidth) * Size, Size);

            if (r.Contains(MouseHandler.MousePosition) == true)
            {
                if (Controls.Accept(true, false, false) == true)
                {
                    Toggled = !Toggled;
                    onToggleTrigger(this);
                    SoundManager.PlaySound("select");
                }
            }

            if (Controls.Accept(false, true, true) == true)
            {
                if (position == s._cursorDestPosition)
                {
                    Toggled = !Toggled;
                    onToggleTrigger(this);
                    SoundManager.PlaySound("select");
                }
            }
        }
    }

    private class CommandButton : Control
    {
        private int _buttonWidth = 1;
        private int _textureY = 16;

        public String Text { get; set; } = String.Empty;

        public delegate void OnClick(CommandButton c);
        public OnClick onClickTrigger;

        public CommandButton(Vector2 pos, int buttonWidth, int size, String text, OnClick clickSub, int id)
        {
            position = pos;
            _buttonWidth = buttonWidth;
            controlType = "CommandButton";
            Size = size;
            Text = text;
            ID = id;
            onClickTrigger = clickSub;
            _textureY = 16;
        }

        public override void Draw()
        {
            NewOptionScreen s = (NewOptionScreen)Core.CurrentScreen!;
            if (s._menuTexture == null) return;
            Color c = new Color(255, 255, 255, (int)(255 * s._interfaceFade * s._pageFade));

            Core.SpriteBatch.Draw(s._menuTexture, new Rectangle((int)position.X, (int)position.Y, Size, Size), new Rectangle(16, _textureY, 16, 16), c);
            Core.SpriteBatch.Draw(s._menuTexture, new Rectangle((int)position.X + Size, (int)position.Y, Size * _buttonWidth, Size), new Rectangle(32, _textureY, 16, 16), c);
            Core.SpriteBatch.Draw(s._menuTexture, new Rectangle((int)position.X + Size * (_buttonWidth + 1), (int)position.Y, Size, Size), new Rectangle(16, _textureY, 16, 16), c, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);

            int fontWidth = (int)(FontManager.MainFont.MeasureString(Text).X * 1.0f);
            Core.SpriteBatch.DrawString(FontManager.MainFont, Text, new Vector2((int)(position.X + (Size * (2 + _buttonWidth) - fontWidth) * 0.5f), (int)position.Y + (int)(16 * Size / 64)), new Color(0, 0, 0, (int)(255 * s._interfaceFade * s._pageFade)), 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
        }

        public override void Update(NewOptionScreen s)
        {
            bool click = false;
            if (s._pageClosing == false && s._pageOpening == false)
            {
                Rectangle r = new Rectangle((int)position.X, (int)position.Y, (2 + _buttonWidth) * Size, Size);
                if (r.Contains(MouseHandler.MousePosition) == true)
                {
                    if (Controls.Accept(true, false, false) == true)
                    {
                        SoundManager.PlaySound("select");
                        click = true;
                        onClickTrigger(this);
                    }
                }
                if (click == true)
                    _textureY = 32;
                if (MouseHandler.ButtonUp(MouseHandler.MouseButtons.LeftButton) == true)
                {
                    _textureY = 16;
                    click = false;
                }

                if (Controls.Accept(false, true, true) == true)
                {
                    if (position == s._cursorDestPosition)
                    {
                        SoundManager.PlaySound("select");
                        onClickTrigger(this);
                    }
                }

                bool keyDown = KeyBoardHandler.KeyDown(KeyBindings.EnterKey1) || KeyBoardHandler.KeyDown(KeyBindings.EnterKey2) || ControllerHandler.ButtonDown(Buttons.A);
                if (keyDown == true)
                    _textureY = position == s._cursorDestPosition ? 32 : 16;
            }
            else
            {
                click = false;
            }
        }
    }

    private class ScrollBar : Control
    {
        private int _value = 0;
        private int _max = 0;
        private int _min = 0;
        private bool _selected = false;
        private bool _clicked = false;

        public int Value { get => _value; set => _value = value; }
        public int Max { get => _max; set => _max = value; }
        public int Min { get => _min; set => _min = value; }
        public String Text { get; set; } = String.Empty;
        public bool DrawPercentage { get; set; } = false;
        public Dictionary<int, String> Settings { get; set; } = [];

        public delegate void OnChange(ScrollBar s);
        public OnChange onChangeTrigger;

        public ScrollBar(Vector2 pos, int size, String text, int value, int min, int max, OnChange changeSub, int id)
            : this(pos, size, text, value, min, max, changeSub, [], id) { }

        public ScrollBar(Vector2 pos, int size, String text, int value, int min, int max, OnChange changeSub, Dictionary<int, String> settings, int id)
        {
            position = pos;
            Size = size;
            Text = text;
            _value = value;
            _max = max;
            _min = min;
            controlType = "ScrollBar";
            Settings = settings;
            onChangeTrigger = changeSub;
            ID = id;
        }

        public override void Draw()
        {
            NewOptionScreen s = (NewOptionScreen)Core.CurrentScreen!;
            if (s._menuTexture == null) return;

            int length = Size + 16;
            int height = 36;
            Color c = new Color(255, 255, 255, (int)(255 * s._interfaceFade * s._pageFade));

            Rectangle barRect1, barRect2, sliderRect;
            Color textColor;
            if (_selected == true || _clicked == true)
            {
                barRect1 = new Rectangle(0, 60, 12, 12);
                barRect2 = new Rectangle(12, 60, 12, 12);
                sliderRect = new Rectangle(6, 32, 6, 12);
                textColor = new Color(25, 67, 91, (int)(255 * s._interfaceFade * s._pageFade));
            }
            else
            {
                barRect1 = new Rectangle(0, 48, 12, 12);
                barRect2 = new Rectangle(12, 48, 12, 12);
                sliderRect = new Rectangle(0, 32, 6, 12);
                textColor = new Color(0, 0, 0, (int)(255 * s._interfaceFade * s._pageFade));
            }

            Core.SpriteBatch.Draw(s._menuTexture, new Rectangle((int)position.X, (int)position.Y, height, height), barRect1, c);
            Core.SpriteBatch.Draw(s._menuTexture, new Rectangle((int)position.X + 36, (int)position.Y, length - 72, height), barRect2, c);
            Core.SpriteBatch.Draw(s._menuTexture, new Rectangle((int)position.X + length - 36, (int)position.Y, height, height), barRect1, c, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
            Core.SpriteBatch.Draw(s._menuTexture, GetSliderBox(), sliderRect, c);

            String t = Text + ": ";
            if (Settings.ContainsKey(_value) == true)
                t += Settings[_value];
            else if (DrawPercentage == true)
                t += (int)(_value / (_max - _min) * 100.0);
            else
                t += _value.ToString();

            Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2(position.X + (float)(400 / 2) - (FontManager.MainFont.MeasureString(t).X / 2), position.Y + 6 - 32), textColor);
        }

        public override void Update(NewOptionScreen s)
        {
            if (MouseHandler.ButtonDown(MouseHandler.MouseButtons.LeftButton) == true)
            {
                if (GetSliderBox().Contains(MouseHandler.MousePosition.X, MouseHandler.MousePosition.Y) == true && _clicked == false)
                {
                    _clicked = true;
                    _selected = false;
                    s._selectedScrollBar = false;
                }
                if (_clicked == true)
                {
                    double x = MouseHandler.MousePosition.X - position.X;
                    if (x < 0) x = 0d;
                    if (x > Size + 16) x = Size + 16;

                    _value = (int)(x * ((_max - _min) / 100.0) * (100.0 / Size)) + _min;
                    _value = _value.Clamp(_min, _max);
                    onChangeTrigger(this);
                }
            }
            else
            {
                _clicked = false;
                if (_selected == true)
                {
                    if (Controls.Dismiss(false, true, true) == true || Controls.Accept(false, true, true) == true)
                    {
                        _selected = false;
                        s._selectedScrollBar = false;
                    }
                    else if (Controls.Left(true) == true)
                    {
                        _value = (_value - 1).Clamp(_min, _max);
                        onChangeTrigger(this);
                    }
                    else if (Controls.Right(true) == true)
                    {
                        _value = (_value + 1).Clamp(_min, _max);
                        onChangeTrigger(this);
                    }
                }
                else
                {
                    if (Controls.Accept(false, true, true) == true)
                    {
                        if (s._cursorDestPosition.Y == position.Y)
                        {
                            _selected = true;
                            s._selectedScrollBar = true;
                        }
                    }
                }
            }
        }

        private Rectangle GetSliderBox()
        {
            int x = (int)(((100.0 / (_max - _min)) * (_value - _min)) * (Size / 100.0));
            if (_value == _min)
                x = 0;
            else if (x == 0 && _value > 0)
                x = 1;

            return new Rectangle(x + (int)position.X, (int)position.Y, 18, 36);
        }
    }
}
