using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace P3D;

public class MainMenuScreen : Screen
{
    private int _mainmenuIndex = 0;
    private int[] _loadMenuIndex = new int[4];
    private int[] _languageMenuIndex = new int[4];
    private int[] _packsMenuIndex = new int[4];
    private int[] _gameModeMenuIndex = new int[4];
    private int _packInfoIndex = 0;
    private int _deleteIndex = 0;
    public int menuIndex = 0;
    private int _loadGameJoltIndex = 0;

    private int _currentLevel = -1;
    private int _levelChangeDelay = 0;

    private Texture2D? _mainTexture;

    private List<String> _saves = [];
    private List<String> _saveNames = [];

    private List<String> _languages = [];
    private List<String> _languageNames = [];
    private String _currentLanguage = "en";

    private List<String> _packNames = [];
    private List<String> _enabledPackNames = [];

    private List<String> _modeNames = [];

    private String _tempLoadDisplay = String.Empty;

    public override String GetScreenStatus()
    {
        return "MenuIndex=" + menuIndex + Environment.NewLine +
               "CurrentLevel=" + _currentLevel + Environment.NewLine +
               "LevelChangeDelay=" + _levelChangeDelay;
    }

    public MainMenuScreen()
    {
        GameModeManager.SetGameModePointer("Kolben");

        Identification = Identifications.MainMenuScreen;
        CanBePaused = false;
        MouseVisible = true;
        CanChat = false;
        _currentLanguage = Localization.LanguageSuffix;

        TextBox.Showing = false;
        PokemonImageView.Showing = false;
        ChooseBox.Showing = false;

        Effect = new BasicEffectWithAlphaTest(Core.GraphicsDevice);
        Effect.FogEnabled = true;
        SkyDome = new SkyDome();
        Camera = new MainMenuCamera();

        Core.Player.Skin = "Hilbert";
        Level = new Level();
        ChangeLevel();

        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");

        Level?.World.Initialize(Level.EnvironmentType, Level.WeatherType);

        if (Directory.Exists(GameController.GamePath + @"\Save\") == false)
        {
            Directory.CreateDirectory(GameController.GamePath + @"\Save\");
        }

        GetSaves();
        GetLanguages();
        GetPacks();

        GameJolt.Emblem.ClearOnlineSpriteCache();
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

        if (Directory.Exists(GameController.GamePath + @"\ContentPacks\") == true)
        {
            foreach (String contentPackFolder in Directory.GetDirectories(GameController.GamePath + @"\ContentPacks\"))
            {
                String newContentPack = contentPackFolder.Remove(0, (GameController.GamePath + @"\ContentPacks\").Length);
                if (_packNames.Contains(newContentPack) == false)
                {
                    _packNames.Add(newContentPack);
                }
            }
        }
    }

    private void GetLanguages()
    {
        _languages.Clear();
        _languageNames.Clear();

        foreach (String filePath in Directory.GetFiles(GameController.GamePath + @"\Content\Localization\"))
        {
            String file = filePath;
            if (file.EndsWith(".dat") == true)
            {
                String[] content = File.ReadAllLines(file);
                file = Path.GetFileNameWithoutExtension(file);

                if (file.StartsWith("Tokens_") == true)
                {
                    String tokenName = file.Remove(0, 7);
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

    private void GetSaves()
    {
        if (File.Exists(GameController.GamePath + @"\Save\lastSession.id") == true)
        {
            String idData = File.ReadAllText(GameController.GamePath + @"\Save\lastSession.id");
            if (Directory.Exists(GameController.GamePath + @"\Save\" + idData) == false)
            {
                File.Delete(GameController.GamePath + @"\Save\lastSession.id");
            }
        }

        _saves.Clear();
        _saveNames.Clear();

        foreach (String folder in Directory.GetDirectories(GameController.GamePath + @"\Save"))
        {
            if (P3D.Player.IsSaveGameFolder(folder) == true)
            {
                _saves.Add(folder);
            }
        }

        for (int i = 0; i <= _saves.Count - 1; i++)
        {
            if (i <= _saves.Count - 1)
            {
                String entry = _saves[i];

                String[] data = File.ReadAllText(entry + @"\Player.dat").SplitAtNewline();
                String name = "Missingno.";
                bool autosave = false;

                foreach (String line in data)
                {
                    if (line.StartsWith("Name|") == true)
                    {
                        name = line.GetSplit(1, "|");
                    }
                    if (line.StartsWith("AutoSave|") == true)
                    {
                        autosave = true;
                    }
                }

                if (autosave == true)
                {
                    _saves.RemoveAt(i);
                    i -= 1;
                }
                else
                {
                    _saveNames.Add(name);
                }
            }
        }
    }

    private void GetGameModes()
    {
        _modeNames.Clear();

        foreach (String folder in Directory.GetDirectories(GameController.GamePath + @"\GameModes\"))
        {
            if (File.Exists(folder + @"\GameMode.dat") == true)
            {
                String directory = Path.GetFileName(folder.TrimEnd('/', '\\'));

                _modeNames.Add(directory);
            }
        }
    }

    private void ChangeLevel()
    {
        int levelCount = 0;
        foreach (String levelPath in Directory.GetFiles(GameController.GamePath + @"\maps\mainmenu\"))
        {
            String levelFile = Path.GetFileName(levelPath);
            if (levelFile.StartsWith("mainmenu") == true && levelFile.EndsWith(".dat") == true)
            {
                levelCount += 1;
            }
        }

        int levelID = Core.Random.Next(0, levelCount);

        if (levelCount > 1)
        {
            while (levelID == _currentLevel)
            {
                levelID = Core.Random.Next(0, levelCount);
            }
        }

        switch (levelID)
        {
            case 0:
                if (Camera != null) Camera.Position = new Vector3(13, 2, 14);
                break;
            case 1:
                if (Camera != null) Camera.Position = new Vector3(23, 2, 10);
                break;
            case 2:
                if (Camera != null) Camera.Position = new Vector3(23, 2, 12);
                break;
            case 3:
                if (Camera != null) Camera.Position = new Vector3(24, 2, 14);
                break;
        }

        if (_currentLevel != levelID)
        {
            _currentLevel = levelID;
            Level?.Load(@"mainmenu\mainmenu" + levelID + ".dat");
        }

        _levelChangeDelay = 1000;
    }

    public override void Update()
    {
        BasicEffectWithAlphaTest? lightEffect = Effect;
        if (lightEffect != null)
        {
            Lighting.UpdateLighting(ref lightEffect);
            Effect = lightEffect;
        }

        Camera?.Update();
        Level?.Update();
        SkyDome?.Update();
        Level?.World.Initialize(Level.EnvironmentType, Level.WeatherType);

        if (Core.GameInstance.IsActive == true)
        {
            switch (menuIndex)
            {
                case 0:
                    UpdateMainMenu();
                    break;
                case 1:
                    UpdateLoadMenu();
                    break;
                case 2:
                    UpdateDeleteMenu();
                    break;
                case 3:
                    UpdateLanguageMenu();
                    break;
                case 4:
                    UpdatePacksMenu();
                    break;
                case 5:
                    UpdatePackInformationMenu();
                    break;
                case 6:
                    UpdateNewGameMenu();
                    break;
                case 7:
                    UpdateLoadGameJoltSaveMenu();
                    break;
            }
        }

        if (_levelChangeDelay <= 0)
        {
            if (Core.Random.Next(0, 1000) == 0)
            {
                ChangeLevel();
            }
        }
        else
        {
            _levelChangeDelay -= 1;
        }
    }

    public override void Draw()
    {
        SkyDome?.Draw(45.0f);
        Level?.Draw();
        World.DrawWeather(Level?.World.CurrentMapWeather ?? 0);

        switch (menuIndex)
        {
            case 0:
                DrawMainMenu();
                break;
            case 1:
                DrawLoadMenu();
                break;
            case 2:
                DrawDeleteMenu();
                break;
            case 3:
                DrawLanguageMenu();
                break;
            case 4:
                DrawPacksMenu();
                break;
            case 5:
                DrawPackInformationMenu();
                break;
            case 6:
                DrawNewGameMenu();
                break;
            case 7:
                DrawLoadGameJoltSaveMenu();
                break;
        }

        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, GameController.DEVELOPER_NAME,
            new Vector2(7, Core.ScreenSize.Height - FontManager.InGameFont.MeasureString(GameController.DEVELOPER_NAME).Y - 1),
            Color.Black);
        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, GameController.DEVELOPER_NAME,
            new Vector2(4, Core.ScreenSize.Height - FontManager.InGameFont.MeasureString(GameController.DEVELOPER_NAME).Y - 4),
            Color.White);
        Core.SpriteBatch.DrawInterface(TextureManager.GetTexture(@"GUI\Logos\P3D"),
            new Rectangle((int)(Core.ScreenSize.Width / 2) - 260, 40, 500, 110),
            Color.White);

        if (Core.GameOptions.ShowDebug == 0)
        {
            String s = GameController.GAMENAME + " " + GameController.GAMEDEVELOPMENT_STAGE + " " + GameController.GAME_VERSION + " (.NET 10 v. " + GameController.PORT_VERSION + ")";
            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, s, new Vector2(7, 7), Color.Black);
            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, s, new Vector2(5, 5), Color.White);
        }
    }

    private void DrawMainMenu()
    {
        Texture2D canvasTexture;

        for (int i = 0; i <= 7; i++)
        {
            String text = String.Empty;
            switch (i)
            {
                case 0:
                    text = Localization.GetString("main_menu_continue");
                    break;
                case 1:
                    text = Localization.GetString("main_menu_load_game");
                    break;
                case 2:
                    text = Localization.GetString("main_menu_new_game");
                    break;
                case 3:
                    text = Localization.GetString("main_menu_quit_game");
                    break;
                case 7:
                    text = "Play online";
                    break;
            }

            if (i == _mainmenuIndex)
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
            }
            else
            {
                if ((i < 2 && _saves.Count == 0) || (i == 0 && Directory.Exists(GameController.GamePath + @"\Save\autosave") == false))
                {
                    canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(48, 0, 48, 48), String.Empty);
                }
                else
                {
                    canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
                }
            }

            if (i == 4)
            {
                if (i == _mainmenuIndex)
                {
                    Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(Core.ScreenSize.Width - 64, 0, 64, 64), new Rectangle(96, 80, 16, 16), Color.White);
                }
                else
                {
                    Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(Core.ScreenSize.Width - 64, 0, 64, 64), new Rectangle(96, 64, 16, 16), Color.White);
                }
            }
            else if (i == 5)
            {
                if (i == _mainmenuIndex)
                {
                    Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(Core.ScreenSize.Width - 64, 64, 64, 64), new Rectangle(112, 80, 16, 16), Color.White);
                }
                else
                {
                    Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(Core.ScreenSize.Width - 64, 64, 64, 64), new Rectangle(112, 64, 16, 16), Color.White);
                }
            }
            else if (i == 6)
            {
                if (Security.FileValidation.IsValid(false) == true)
                {
                    if (GameJolt.API.LoggedIn == true)
                    {
                        if (i == _mainmenuIndex)
                        {
                            Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(Core.ScreenSize.Width - 196, Core.ScreenSize.Height - 60, 192, 56), new Rectangle(160, 96, 96, 28), Color.White);
                        }
                        else
                        {
                            Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(Core.ScreenSize.Width - 196, Core.ScreenSize.Height - 60, 192, 56), new Rectangle(160, 65, 96, 28), Color.White);
                        }
                        Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont, "Logged in as", new Vector2(Core.ScreenSize.Width - 148, Core.ScreenSize.Height - 54), Color.White);
                        Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont, GameJolt.API.username, new Vector2(Core.ScreenSize.Width - 148, Core.ScreenSize.Height - 34), new Color(204, 255, 0));
                    }
                    else
                    {
                        if (i == _mainmenuIndex)
                        {
                            Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(Core.ScreenSize.Width - 60, Core.ScreenSize.Height - 60, 56, 56), new Rectangle(129, 96, 28, 28), Color.White);
                        }
                        else
                        {
                            Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(Core.ScreenSize.Width - 60, Core.ScreenSize.Height - 60, 56, 56), new Rectangle(129, 65, 28, 28), Color.White);
                        }
                    }
                }
                else
                {
                    Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont, "File Validation failed. Download a new copy of the game to fix this.", new Vector2(220, Core.ScreenSize.Height - 30), Color.White);
                }
            }
            else if (i == 7)
            {
                if (GameJolt.API.LoggedIn == true && Security.FileValidation.IsValid(false) == true)
                {
                    Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2), 160 + 128, 320, 64), true);
                    Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text,
                        new Vector2((int)(Core.ScreenSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(text).X / 2) + 160 + 20, 196 + 128),
                        Color.Black);
                }
            }
            else if (i == 1 && GameJolt.API.LoggedIn == true)
            {
                Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 180 - 160 - 20, 160 + i * 128, 320, 64), true);
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text,
                    new Vector2((int)(Core.ScreenSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(text).X / 2) - 10 - 160 - 20, 196 + i * 128),
                    Color.Black);
            }
            else
            {
                Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 180, 160 + i * 128, 320, 64), true);
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text,
                    new Vector2((int)(Core.ScreenSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(text).X / 2) - 10, 196 + i * 128),
                    Color.Black);
            }
        }

        Dictionary<Buttons, String> d = new Dictionary<Buttons, String>
        {
            { Buttons.A, "Accept" }
        };
        if (GameJolt.API.LoggedIn == true)
        {
            DrawGamePadControls(d, new Vector2(Core.ScreenSize.Width - 170, Core.ScreenSize.Height - 100));
        }
        else
        {
            DrawGamePadControls(d, new Vector2(Core.ScreenSize.Width - 234, Core.ScreenSize.Height - 40));
        }
    }

    private void UpdateMainMenu()
    {
        if (Controls.Up(true, true) == true)
        {
            _mainmenuIndex -= 1;
        }
        if (Controls.Down(true, true) == true)
        {
            _mainmenuIndex += 1;
        }

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 7; i++)
            {
                if (i == 4)
                {
                    if (Core.ScaleScreenRec(new Rectangle(Core.ScreenSize.Width - 64, 0, 64, 64)).Contains(MouseHandler.MousePosition) == true)
                    {
                        _mainmenuIndex = 4;
                        if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                        {
                            LanguageButton();
                        }
                    }
                }
                else if (i == 5)
                {
                    if (Core.ScaleScreenRec(new Rectangle(Core.ScreenSize.Width - 64, 64, 64, 64)).Contains(MouseHandler.MousePosition) == true)
                    {
                        _mainmenuIndex = 5;
                        if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                        {
                            PacksButton();
                        }
                    }
                }
                else if (i == 6)
                {
                    Rectangle r = Core.ScaleScreenRec(new Rectangle(Core.ScreenSize.Width - 196, Core.ScreenSize.Height - 60, 192, 56));
                    if (GameJolt.API.LoggedIn == false)
                    {
                        r = Core.ScaleScreenRec(new Rectangle(Core.ScreenSize.Width - 64, Core.ScreenSize.Height - 64, 64, 64));
                    }

                    if (r.Contains(MouseHandler.MousePosition) == true)
                    {
                        _mainmenuIndex = 6;
                        if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                        {
                            GameJoltButton();
                        }
                    }
                }
                else if (i == 1 && GameJolt.API.LoggedIn == true)
                {
                    if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 180 - 160 - 20, 160 + i * 128, 320 + 32, 64 + 32)).Contains(MouseHandler.MousePosition) == true)
                    {
                        _mainmenuIndex = i;
                        if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                        {
                            LoadGameButton();
                        }
                    }
                }
                else if (i == 7 && GameJolt.API.LoggedIn == true)
                {
                    if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2), 160 + 128, 320 + 32, 64 + 32)).Contains(MouseHandler.MousePosition) == true)
                    {
                        _mainmenuIndex = i;
                        if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                        {
                            LoadGameJoltButton();
                        }
                    }
                }
                else
                {
                    if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 180, 160 + i * 128, 320 + 32, 64 + 32)).Contains(MouseHandler.MousePosition) == true)
                    {
                        _mainmenuIndex = i;
                        if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                        {
                            switch (_mainmenuIndex)
                            {
                                case 0: ContinueButton(); break;
                                case 1: LoadGameButton(); break;
                                case 2: NewGameButton(); break;
                                case 3: CloseGameButton(); break;
                            }
                        }
                    }
                }
            }
        }

        if (Security.FileValidation.IsValid(false) == true)
        {
            if (GameJolt.API.LoggedIn == true)
            {
                _mainmenuIndex = (int)MathHelper.Clamp(_mainmenuIndex, 0, 7);
            }
            else
            {
                _mainmenuIndex = (int)MathHelper.Clamp(_mainmenuIndex, 0, 6);
            }
        }
        else
        {
            _mainmenuIndex = _mainmenuIndex.Clamp(0, 5);
        }

        if (Controls.Accept(false, true) == true)
        {
            switch (_mainmenuIndex)
            {
                case 0: ContinueButton(); break;
                case 1: LoadGameButton(); break;
                case 2: NewGameButton(); break;
                case 3: CloseGameButton(); break;
                case 4: LanguageButton(); break;
                case 5: PacksButton(); break;
                case 6: GameJoltButton(); break;
                case 7: LoadGameJoltButton(); break;
            }
        }
    }

    private void PacksButton()
    {
        GetPacks();
        _packsMenuIndex[0] = 0;
        _packsMenuIndex[2] = 0;
        menuIndex = 4;
    }

    private void LanguageButton()
    {
        GetLanguages();
        if (_languages.Contains(_currentLanguage) == true)
        {
            _languageMenuIndex[0] = _languages.IndexOf(_currentLanguage);
        }
        menuIndex = 3;
    }

    private void ContinueButton()
    {
        if (_saves.Count > 0 && P3D.Player.IsSaveGameFolder(GameController.GamePath + @"\Save\autosave") == true)
        {
            Core.Player.IsGameJoltSave = false;
            Core.Player.LoadGame("autosave");
            Core.SetScreen(new JoinServerScreen(this));
        }
    }

    private void LoadGameButton()
    {
        GetSaves();
        if (_saves.Count > 0)
        {
            menuIndex = 1;
        }
    }

    private void LoadGameJoltButton()
    {
        if (Security.FileValidation.IsValid(false) == true)
        {
            if (GameJolt.API.LoggedIn == true)
            {
                Core.GameJoltSave.DownloadSave(GameJolt.LogInScreen.LoadedGameJoltID, true);
            }
            menuIndex = 7;
        }
    }

    private void CloseGameButton()
    {
        Core.GameOptions.SaveOptions();
        Core.GameInstance.Exit();
    }

    private void GameJoltButton()
    {
        if (Security.FileValidation.IsValid(false) == true)
        {
            Core.SetScreen(new GameJolt.LogInScreen(this));
        }
    }

    private void DrawLoadMenu()
    {
        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

        for (int i = 0; i <= 3; i++)
        {
            Color c = Color.White;
            if (i + _loadMenuIndex[2] == _loadMenuIndex[0])
            {
                c = new Color(101, 142, 255);
            }
            Canvas.DrawRectangle(new Rectangle((int)(Core.ScreenSize.Width / 2) - 258, 180 + i * 50, 480, 48), c, true);
        }

        Canvas.DrawScrollBar(new Vector2((int)(Core.ScreenSize.Width / 2) + 250, 180), _saves.Count, 4, _loadMenuIndex[2],
            new Size(4, 200), false, new Color(190, 190, 190), new Color(63, 63, 63), true);

        int x = _saves.Count - 1;
        x = (int)MathHelper.Clamp(x, 0, 3);

        for (int i = 0; i <= x; i++)
        {
            String name = _saveNames[i + _loadMenuIndex[2]];

            if (i + _loadMenuIndex[2] == _loadMenuIndex[0])
            {
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 245, 191 + i * 50), Color.Black);
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 248, 188 + i * 50), Color.White);
            }
            else
            {
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 248, 188 + i * 50), Color.Black);
            }
        }

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 272, 388, 512, 128), true);

        if (_tempLoadDisplay == String.Empty)
        {
            String dispName = "(Unknown)";
            String dispBadges = "(Unknown)";
            String dispPlayTime = "(Unknown)";
            String dispLocation = "(Unknown)";
            String dispGameMode = "Kolben";

            String[] data = File.ReadAllText(_saves[_loadMenuIndex[0]] + @"\Player.dat").SplitAtNewline();
            foreach (String line in data)
            {
                if (line.Contains("|") == true)
                {
                    String id = line.Remove(line.IndexOf("|"));
                    String value = line.Remove(0, line.IndexOf("|") + 1);
                    switch (id)
                    {
                        case "Name":
                            dispName = value;
                            break;
                        case "Badges":
                            int bCount = 0;
                            if (value == "0")
                            {
                                bCount = 0;
                            }
                            else
                            {
                                if (value.Contains(",") == false)
                                {
                                    bCount = 1;
                                }
                                else
                                {
                                    String[] s = value.Split(',');
                                    bCount = s.Length;
                                }
                            }
                            dispBadges = bCount.ToString();
                            break;
                        case "PlayTime":
                            String[] dd = value.Split(',');
                            TimeSpan tSpan = TimeSpan.Zero;
                            if (dd.Length == 3)
                            {
                                tSpan = new TimeSpan(int.Parse(dd[0]), int.Parse(dd[1]), int.Parse(dd[2]));
                            }
                            else if (dd.Length == 4)
                            {
                                tSpan = new TimeSpan(int.Parse(dd[3]), int.Parse(dd[0]), int.Parse(dd[1]), int.Parse(dd[2]));
                            }
                            dispPlayTime = TimeHelpers.GetDisplayTime(tSpan, true);
                            break;
                        case "location":
                            dispLocation = value;
                            break;
                        case "GameMode":
                            dispGameMode = value;
                            break;
                    }
                }
            }

            _tempLoadDisplay =
                Localization.GetString("load_menu_name") + ": " + dispName + Environment.NewLine +
                Localization.GetString("load_menu_gamemode") + ": " + dispGameMode + Environment.NewLine +
                Localization.GetString("load_menu_badges") + ": " + dispBadges + Environment.NewLine +
                Localization.GetString("load_menu_location") + ": " + Localization.GetString("Places_" + dispLocation) + Environment.NewLine +
                Localization.GetString("load_menu_time") + ": " + dispPlayTime;
        }

        Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont, _tempLoadDisplay, new Vector2((int)(Core.ScreenSize.Width / 2) - 252, 416), Color.Black);

        for (int i = 0; i <= 2; i++)
        {
            String text = String.Empty;
            switch (i)
            {
                case 0: text = Localization.GetString("load_menu_load"); break;
                case 1: text = Localization.GetString("load_menu_delete"); break;
                case 2: text = Localization.GetString("load_menu_back"); break;
            }

            if (i == _loadMenuIndex[1])
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
            }
            else
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
            }

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 272 + i * 192, 550, 128, 64), true);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text, new Vector2((int)(Core.ScreenSize.Width / 2) - 254 + i * 192, 582), Color.Black);
        }

        Dictionary<Buttons, String> d = new Dictionary<Buttons, String>
        {
            { Buttons.A, "Accept" },
            { Buttons.B, "Back" }
        };
        DrawGamePadControls(d);
    }

    private void UpdateLoadMenu()
    {
        if (Controls.Up(true, true, true) == true)
        {
            _loadMenuIndex[0] -= 1;
            if (_loadMenuIndex[0] - _loadMenuIndex[2] < 0)
            {
                _loadMenuIndex[2] -= 1;
            }
            _tempLoadDisplay = String.Empty;
        }
        if (Controls.Down(true, true, true) == true)
        {
            _loadMenuIndex[0] += 1;
            if (_loadMenuIndex[0] + _loadMenuIndex[2] > 3)
            {
                _loadMenuIndex[2] += 1;
            }
            _tempLoadDisplay = String.Empty;
        }

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 272 + i * 192, 550, 128 + 32, 64 + 32)).Contains(MouseHandler.MousePosition) == true)
                {
                    _loadMenuIndex[1] = i;

                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        switch (_loadMenuIndex[1])
                        {
                            case 0:
                                Core.Player.IsGameJoltSave = false;
                                Core.Player.LoadGame(Path.GetFileName(_saves[_loadMenuIndex[0]]));
                                Core.SetScreen(new JoinServerScreen(this));
                                break;
                            case 1:
                                menuIndex = 2;
                                break;
                            case 2:
                                menuIndex = 0;
                                _tempLoadDisplay = String.Empty;
                                break;
                        }
                    }
                }
            }

            for (int i = 0; i <= 3; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 258, 180 + i * 50, 480, 48)).Contains(MouseHandler.MousePosition) == true)
                {
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        _loadMenuIndex[0] = i + _loadMenuIndex[2];
                        _tempLoadDisplay = String.Empty;
                    }
                }
            }
        }

        _loadMenuIndex[0] = (int)MathHelper.Clamp(_loadMenuIndex[0], 0, _saves.Count - 1);
        _loadMenuIndex[2] = (int)MathHelper.Clamp(_loadMenuIndex[2], 0, _saves.Count - 4);

        if (Controls.Right(true, true, false) == true)
        {
            _loadMenuIndex[1] += 1;
        }
        if (Controls.Left(true, true, false) == true)
        {
            _loadMenuIndex[1] -= 1;
        }

        _loadMenuIndex[1] = (int)MathHelper.Clamp(_loadMenuIndex[1], 0, 2);

        if (Controls.Accept(false, true) == true)
        {
            switch (_loadMenuIndex[1])
            {
                case 0:
                    Core.Player.IsGameJoltSave = false;
                    Core.Player.LoadGame(Path.GetFileName(_saves[_loadMenuIndex[0]]));
                    Core.SetScreen(new JoinServerScreen(this));
                    break;
                case 1:
                    menuIndex = 2;
                    break;
                case 2:
                    menuIndex = 0;
                    _tempLoadDisplay = String.Empty;
                    break;
            }
        }

        if (Controls.Dismiss() == true)
        {
            menuIndex = 0;
        }
    }

    private void DrawLoadGameJoltSaveMenu()
    {
        if (Core.GameJoltSave.DownloadFailed == false)
        {
            bool downloaded = Core.GameJoltSave.DownloadFinished;

            if (downloaded == true)
            {
                Rectangle r = new Rectangle((int)(Core.ScreenSize.Width / 2) - 256, 300, 512, 128);
                if ((Core.ScaleScreenRec(r).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible == true) ||
                    (Core.GameInstance.IsMouseVisible == false && _loadGameJoltIndex == 0))
                {
                    Canvas.DrawRectangle(Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 264, 292, 528, 144)), new Color(255, 255, 255, 150));
                }

                if (GameJolt.LogInScreen.UserBanned(Core.GameJoltSave.GameJoltID) == true)
                {
                    String reason = GameJolt.LogInScreen.GetBanReasonByID(GameJolt.LogInScreen.BanReasonIDForUser(Core.GameJoltSave.GameJoltID));
                    Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, reason,
                        new Vector2((float)(Core.ScreenSize.Width / 2 - FontManager.MainFont.MeasureString(reason).X / 2) + 2, 260 + 2),
                        Color.Black);
                    Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, reason,
                        new Vector2((float)(Core.ScreenSize.Width / 2 - FontManager.MainFont.MeasureString(reason).X / 2), 260),
                        Color.Red);
                }

                GameJolt.Emblem.Draw(GameJolt.API.username, Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Points,
                    Core.GameJoltSave.Gender, Core.GameJoltSave.Emblem,
                    Core.ScaleScreenVec(new Vector2((float)(Core.ScreenSize.Width / 2) - 256, 300)),
                    (float)(4 * Core.SpriteBatch.InterfaceScale()), Core.GameJoltSave.DownloadedSprite);

                int y = 0;
                if ((Core.ScaleScreenRec(new Rectangle(r.X + 32 + r.Width, r.Y, 32, 32)).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible == true) ||
                    (Core.GameInstance.IsMouseVisible == false && _loadGameJoltIndex == 1))
                {
                    y = 16;
                    Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont, "Change to male.", new Vector2(r.X + 64 + 4 + r.Width, r.Y + 4), Color.White);
                }
                Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(r.X + 32 + r.Width, r.Y, 32, 32), new Rectangle(144, 32 + y, 16, 16), Color.White);

                y = 0;
                if ((Core.ScaleScreenRec(new Rectangle(r.X + 32 + r.Width, r.Y + 48, 32, 32)).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible == true) ||
                    (Core.GameInstance.IsMouseVisible == false && _loadGameJoltIndex == 2))
                {
                    y = 16;
                    Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont, "Change to female.", new Vector2(r.X + 64 + 4 + r.Width, r.Y + 4 + 48), Color.White);
                }
                Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(r.X + 32 + r.Width, r.Y + 48, 32, 32), new Rectangle(160, 32 + y, 16, 16), Color.White);

                y = 0;
                if ((Core.ScaleScreenRec(new Rectangle(r.X + 32 + r.Width, r.Y + 96, 32, 32)).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible == true) ||
                    (Core.GameInstance.IsMouseVisible == false && _loadGameJoltIndex == 3))
                {
                    y = 16;
                    Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont, "Reset save.", new Vector2(r.X + 64 + 4 + r.Width, r.Y + 4 + 96), Color.White);
                }
                Core.SpriteBatch.DrawInterface(_mainTexture!, new Rectangle(r.X + 32 + r.Width, r.Y + 96, 32, 32), new Rectangle(176, 32 + y, 16, 16), Color.White);
            }
            else
            {
                int downloadProgress = Core.GameJoltSave.DownloadProgress;
                int total = Core.GameJoltSave.TotalDownloadItems;

                String downloadtext = "Downloading profile";
                Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, downloadtext + LoadingDots.Dots,
                    new Vector2((float)(Core.ScreenSize.Width / 2 - FontManager.MainFont.MeasureString(downloadtext).X / 2) + 2, 322),
                    Color.Black);
                Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, downloadtext + LoadingDots.Dots,
                    new Vector2((float)(Core.ScreenSize.Width / 2 - FontManager.MainFont.MeasureString(downloadtext).X / 2), 320),
                    Color.White);

                Canvas.DrawScrollBar(new Vector2((int)(Core.ScreenSize.Width / 2) - 256, 400), total, downloadProgress, 0,
                    new Size(512, 8), true, Color.Black, Color.White, true);
            }
        }
        else
        {
            String failText = "The download failed! Please try again.";
            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, failText,
                new Vector2((float)(Core.ScreenSize.Width / 2 - FontManager.MainFont.MeasureString(failText).X / 2) + 2, 322),
                Color.Black);
            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, failText,
                new Vector2((float)(Core.ScreenSize.Width / 2 - FontManager.MainFont.MeasureString(failText).X / 2), 320),
                Color.DarkRed);
        }

        if (ControllerHandler.IsConnected() == false)
        {
            String text = "Right-Click to quit to the main menu";
            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, text,
                new Vector2((float)(Core.ScreenSize.Width / 2 - FontManager.MainFont.MeasureString(text).X / 2) + 2, 502),
                Color.Black);
            Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, text,
                new Vector2((float)(Core.ScreenSize.Width / 2 - FontManager.MainFont.MeasureString(text).X / 2), 500),
                Color.White);
        }

        Dictionary<Buttons, String> d = new Dictionary<Buttons, String>
        {
            { Buttons.A, "Select" },
            { Buttons.B, "Back" }
        };
        DrawGamePadControls(d);
    }

    private void UpdateLoadGameJoltSaveMenu()
    {
        bool downloaded = Core.GameJoltSave.DownloadFinished;

        if (downloaded == true)
        {
            if (Controls.Down(true, true, false, true, true) == true || Controls.Right(true, true, false, true, true) == true)
            {
                _loadGameJoltIndex += 1;
            }
            if (Controls.Up(true, true, false, true, true) == true || Controls.Left(true, true, false, true, true) == true)
            {
                _loadGameJoltIndex -= 1;
            }

            _loadGameJoltIndex = _loadGameJoltIndex.Clamp(0, 3);

            if (Controls.Accept(true, true) == true)
            {
                Rectangle r = Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 256, 300, 512, 128));
                if ((Core.GameInstance.IsMouseVisible == false && _loadGameJoltIndex == 0) ||
                    (r.Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible == true))
                {
                    Core.Player.IsGameJoltSave = true;
                    Core.Player.LoadGame("GAMEJOLTSAVE");
                    Core.SetScreen(new JoinServerScreen(this));
                }

                if ((Core.GameInstance.IsMouseVisible == false && _loadGameJoltIndex == 1) ||
                    (Core.ScaleScreenRec(new Rectangle(r.X + 32 + r.Width, r.Y, 32, 32)).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible == true))
                {
                    ButtonChangeMale();
                }
                if ((Core.GameInstance.IsMouseVisible == false && _loadGameJoltIndex == 2) ||
                    (Core.ScaleScreenRec(new Rectangle(r.X + 32 + r.Width, r.Y + 48, 32, 32)).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible == true))
                {
                    ButtonChangeFemale();
                }
                if ((Core.GameInstance.IsMouseVisible == false && _loadGameJoltIndex == 3) ||
                    (Core.ScaleScreenRec(new Rectangle(r.X + 32 + r.Width, r.Y + 96, 32, 32)).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible == true))
                {
                    ButtonResetSave();
                }
            }
        }

        if (Controls.Dismiss(true, true) == true)
        {
            menuIndex = 0;
        }
    }

    private void ButtonChangeMale()
    {
        Core.GameJoltSave.Gender = "0";
        Core.Player.Skin = GameJolt.Emblem.GetPlayerSpriteFile(
            GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points), Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
        Core.Player.Male = Core.GameJoltSave.Gender != "1";
    }

    private void ButtonChangeFemale()
    {
        Core.GameJoltSave.Gender = "1";
        Core.Player.Skin = GameJolt.Emblem.GetPlayerSpriteFile(
            GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points), Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
        Core.Player.Male = Core.GameJoltSave.Gender != "1";
    }

    private void ButtonResetSave()
    {
        Core.GameJoltSave.ResetSave();
    }

    private void DrawLanguageMenu()
    {
        Texture2D canvasTexture;

        for (int i = 0; i <= 3; i++)
        {
            Color c = Color.White;
            if (i + _languageMenuIndex[2] == _languageMenuIndex[0])
            {
                c = new Color(101, 142, 255);
            }
            Canvas.DrawRectangle(new Rectangle((int)(Core.ScreenSize.Width / 2) - 258, 180 + i * 50, 480, 48), c, true);
        }

        Canvas.DrawScrollBar(new Vector2((int)(Core.ScreenSize.Width / 2) + 250, 180), _languages.Count, 4, _languageMenuIndex[2],
            new Size(4, 200), false, new Color(190, 190, 190), new Color(63, 63, 63), true);

        int x = _languages.Count - 1;
        x = (int)MathHelper.Clamp(x, 0, 3);

        for (int i = 0; i <= x; i++)
        {
            String name = _languageNames[i + _languageMenuIndex[2]];

            if (i + _languageMenuIndex[2] == _languageMenuIndex[0])
            {
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 245, 191 + i * 50), Color.Black);
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 248, 188 + i * 50), Color.White);
            }
            else
            {
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 248, 188 + i * 50), Color.Black);
            }
        }

        canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

        for (int i = 0; i <= 1; i++)
        {
            String text = String.Empty;
            switch (i)
            {
                case 0: text = Localization.GetString("language_menu_apply"); break;
                case 1: text = Localization.GetString("language_menu_back"); break;
            }

            if (i == _languageMenuIndex[1])
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
            }
            else
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
            }

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 208 + i * 192, 550, 128, 64), true);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text, new Vector2((int)(Core.ScreenSize.Width / 2) - 190 + i * 192, 582), Color.Black);
        }
    }

    private void UpdateLanguageMenu()
    {
        int currentIndex = _languageMenuIndex[0];

        if (Controls.Up(true, true, true) == true)
        {
            _languageMenuIndex[0] -= 1;
            if (_languageMenuIndex[0] - _languageMenuIndex[2] < 0)
            {
                _languageMenuIndex[2] -= 1;
            }
        }
        if (Controls.Down(true, true, true) == true)
        {
            _languageMenuIndex[0] += 1;
            if (_languageMenuIndex[0] + _languageMenuIndex[2] > 3)
            {
                _languageMenuIndex[2] += 1;
            }
        }

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 208 + i * 192, 550, 128 + 32, 64 + 32)).Contains(MouseHandler.MousePosition) == true)
                {
                    _languageMenuIndex[1] = i;

                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        switch (_languageMenuIndex[1])
                        {
                            case 0:
                                _currentLanguage = _languages[_languageMenuIndex[0]];
                                Core.GameOptions.SaveOptions();
                                menuIndex = 0;
                                break;
                            case 1:
                                Localization.Load(_currentLanguage);
                                menuIndex = 0;
                                break;
                        }
                    }
                }
            }

            for (int i = 0; i <= 3; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 258, 180 + i * 50, 480, 48)).Contains(MouseHandler.MousePosition) == true)
                {
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        _languageMenuIndex[0] = i + _languageMenuIndex[2];
                    }
                }
            }
        }

        _languageMenuIndex[0] = (int)MathHelper.Clamp(_languageMenuIndex[0], 0, _languages.Count - 1);
        _languageMenuIndex[2] = (int)MathHelper.Clamp(_languageMenuIndex[2], 0, _languages.Count - 4);

        if (_languageMenuIndex[0] != currentIndex)
        {
            Localization.Load(_languages[_languageMenuIndex[0]]);
        }

        if (Controls.Right(true, true, false) == true)
        {
            _languageMenuIndex[1] += 1;
        }
        if (Controls.Left(true, true, false) == true)
        {
            _languageMenuIndex[1] -= 1;
        }

        _languageMenuIndex[1] = (int)MathHelper.Clamp(_languageMenuIndex[1], 0, 1);

        if (Controls.Accept(false, true) == true)
        {
            switch (_languageMenuIndex[1])
            {
                case 0:
                    _currentLanguage = _languages[_languageMenuIndex[0]];
                    Core.GameOptions.SaveOptions();
                    menuIndex = 0;
                    break;
                case 1:
                    Localization.Load(_currentLanguage);
                    menuIndex = 0;
                    break;
            }
        }

        if (Controls.Dismiss() == true)
        {
            menuIndex = 0;
        }
    }

    private void DrawPacksMenu()
    {
        bool isSelectedEnabled = false;

        for (int i = 0; i <= 3; i++)
        {
            Color c = Color.White;
            if (i + _packsMenuIndex[2] == _packsMenuIndex[0])
            {
                c = new Color(101, 142, 255);
                if (_enabledPackNames.Count > 0)
                {
                    if (_enabledPackNames.Contains(_packNames[i + _packsMenuIndex[2]]) == true)
                    {
                        isSelectedEnabled = true;
                    }
                }
            }
            Canvas.DrawRectangle(new Rectangle((int)(Core.ScreenSize.Width / 2) - 258, 180 + i * 50, 480, 48), c, true);
        }

        Canvas.DrawScrollBar(new Vector2((int)(Core.ScreenSize.Width / 2) + 250, 180), _packNames.Count, 4, _packsMenuIndex[2],
            new Size(4, 200), false, new Color(190, 190, 190), new Color(63, 63, 63), true);

        int x = _packNames.Count - 1;
        x = (int)MathHelper.Clamp(x, 0, 3);

        if (_packNames.Count > 0)
        {
            for (int i = 0; i <= x; i++)
            {
                String name = _packNames[i + _packsMenuIndex[2]];
                Color textColor = Color.Gray;

                if (_enabledPackNames.Contains(name) == true)
                {
                    name += " (" + Localization.GetString("pack_menu_enabled") + ")";
                    textColor = Color.Black;
                }

                if (i + _packsMenuIndex[2] == _packsMenuIndex[0])
                {
                    Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 245, 191 + i * 50), textColor);
                    Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 248, 188 + i * 50), Color.White);
                }
                else
                {
                    Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 248, 188 + i * 50), textColor);
                }
            }
        }

        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

        for (int i = 0; i <= 1; i++)
        {
            String text = i == 0 ? Localization.GetString("pack_menu_apply") : Localization.GetString("pack_menu_back");

            canvasTexture = i == _packsMenuIndex[1]
                ? TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty)
                : TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 208 + i * 192, 550, 128, 64), true);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text, new Vector2((int)(Core.ScreenSize.Width / 2) - 190 + i * 192, 582), Color.Black);
        }

        for (int i = 2; i <= 5; i++)
        {
            String text = String.Empty;
            switch (i)
            {
                case 2: text = Localization.GetString("pack_menu_up"); break;
                case 3: text = Localization.GetString("pack_menu_down"); break;
                case 4:
                    text = isSelectedEnabled == true
                        ? Localization.GetString("pack_menu_toggle_off")
                        : Localization.GetString("pack_menu_toggle_on");
                    break;
                case 5: text = Localization.GetString("pack_menu_information"); break;
            }

            if (i == _packsMenuIndex[1])
            {
                if (i == 2 || i == 3 || _packNames.Count == 0)
                {
                    canvasTexture = isSelectedEnabled == true
                        ? TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty)
                        : TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(48, 0, 48, 48), String.Empty);
                }
                else
                {
                    canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
                }
            }
            else
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
            }

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) + 280, (i - 2) * 64 + 180, 160, 32), true);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text, new Vector2((int)(Core.ScreenSize.Width / 2) + 280 + 15, (i - 2) * 64 + 16 + 180), Color.Black);
        }
    }

    private void UpdatePacksMenu()
    {
        if (Controls.Up(true, true, true) == true)
        {
            _packsMenuIndex[0] -= 1;
            if (_packsMenuIndex[0] - _packsMenuIndex[2] < 0)
            {
                _packsMenuIndex[2] -= 1;
            }
        }
        if (Controls.Down(true, true, true) == true)
        {
            _packsMenuIndex[0] += 1;
            if (_packsMenuIndex[0] + _packsMenuIndex[2] > 3)
            {
                _packsMenuIndex[2] += 1;
            }
        }

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 208 + i * 192, 550, 128 + 32, 64 + 32)).Contains(MouseHandler.MousePosition) == true)
                {
                    _packsMenuIndex[1] = i;
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        switch (_packsMenuIndex[1])
                        {
                            case 0: ButtonApplyPacks(); break;
                            case 1: menuIndex = 0; break;
                        }
                    }
                }
            }

            for (int i = 2; i <= 5; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) + 280, (i - 2) * 64 + 180, 160 + 32, 32 + 32)).Contains(MouseHandler.MousePosition) == true)
                {
                    _packsMenuIndex[1] = i;
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        switch (_packsMenuIndex[1])
                        {
                            case 2: ButtonUp(); break;
                            case 3: ButtonDown(); break;
                            case 4:
                                if (_packNames.Count > 0) ButtonToggle(_packNames[_packsMenuIndex[0]]);
                                break;
                            case 5: ButtonPackInformation(); break;
                        }
                    }
                }
            }

            for (int i = 0; i <= 3; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 258, 180 + i * 50, 480, 48)).Contains(MouseHandler.MousePosition) == true)
                {
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        _packsMenuIndex[0] = i + _packsMenuIndex[2];
                    }
                }
            }
        }

        _packsMenuIndex[0] = (int)MathHelper.Clamp(_packsMenuIndex[0], 0, _packNames.Count - 1);
        _packsMenuIndex[2] = (int)MathHelper.Clamp(_packsMenuIndex[2], 0, _packNames.Count - 4);

        if (Controls.Right(true, true, false) == true) _packsMenuIndex[1] += 1;
        if (Controls.Left(true, true, false) == true) _packsMenuIndex[1] -= 1;

        _packsMenuIndex[1] = (int)MathHelper.Clamp(_packsMenuIndex[1], 0, 5);

        if (Controls.Accept(false, true) == true)
        {
            switch (_packsMenuIndex[1])
            {
                case 0: ButtonApplyPacks(); break;
                case 1: menuIndex = 0; break;
                case 2: ButtonUp(); break;
                case 3: ButtonDown(); break;
                case 4:
                    if (_packNames.Count > 0) ButtonToggle(_packNames[_packsMenuIndex[0]]);
                    break;
                case 5: ButtonPackInformation(); break;
            }
        }

        if (Controls.Dismiss() == true)
        {
            menuIndex = 0;
        }
    }

    private void ButtonPackInformation()
    {
        if (_packNames.Count == 0)
        {
            return;
        }

        menuIndex = 5;

        String packName = _packNames[_packsMenuIndex[0]];
        _pInfoSplash = null;
        _pInfoContent = String.Empty;

        try
        {
            String splashPath = GameController.GamePath + @"\ContentPacks\" + packName + @"\splash.png";
            if (File.Exists(splashPath) == true)
            {
                using Stream stream = File.Open(splashPath, FileMode.OpenOrCreate);
                _pInfoSplash = Texture2D.FromStream(Core.GraphicsDevice, stream);
            }
        }
        catch (Exception ex)
        {
            Logger.Log(Logger.LogTypes.ErrorMessage, "MainMenuScreen.cs/ButtonPackInformation: " + ex.Message);
        }

        String contentPackPath = GameController.GamePath + @"\ContentPacks\" + packName + @"\";
        if (Directory.Exists(contentPackPath + "Songs") == true)
        {
            bool hasWMA = false, hasXNB = false, hasMP3 = false;
            foreach (String file in Directory.GetFiles(contentPackPath + "Songs"))
            {
                String ext = Path.GetExtension(file).ToLower();
                if (ext == ".xnb") hasXNB = true;
                if (ext == ".wma") hasWMA = true;
                if (ext == ".mp3") hasMP3 = true;
            }
            if (hasMP3 == true || (hasWMA == true && hasXNB == true))
            {
                _pInfoContent = Localization.GetString("pack_menu_songs");
            }
        }
        if (Directory.Exists(contentPackPath + "Sounds") == true)
        {
            bool hasWMA = false, hasXNB = false, hasWAV = false;
            foreach (String file in Directory.GetFiles(contentPackPath + "Sounds"))
            {
                String ext = Path.GetExtension(file).ToLower();
                if (ext == ".xnb") hasXNB = true;
                if (ext == ".wma") hasWMA = true;
                if (ext == ".wav") hasWAV = true;
            }
            if (hasWAV == true || (hasWMA == true && hasXNB == true))
            {
                if (_pInfoContent != String.Empty) _pInfoContent += ", ";
                _pInfoContent += Localization.GetString("pack_menu_sounds");
            }
        }

        String[] textureDirectories = ["Textures", "GUI", "Items", "Pokemon", "SkyDomeResource"];
        foreach (String folder in textureDirectories)
        {
            if (Directory.Exists(contentPackPath + folder) == true)
            {
                bool hasXNB = false, hasPNG = false;
                foreach (String file in Directory.GetFiles(contentPackPath + folder, "*.*", SearchOption.AllDirectories))
                {
                    String ext = Path.GetExtension(file).ToLower();
                    if (ext == ".xnb") hasXNB = true;
                    if (ext == ".png") hasPNG = true;
                }
                if (hasXNB == true || hasPNG == true)
                {
                    if (_pInfoContent != String.Empty) _pInfoContent += ", ";
                    _pInfoContent += Localization.GetString("pack_menu_textures");
                    break;
                }
            }
        }

        String[] s = ContentPackManager.GetContentPackInfo(packName);

        if (s.Length > 0) _pInfoVersion = s[0];
        if (s.Length > 1) _pInfoAuthor = s[1];
        if (s.Length > 2) _pInfoDescription = s[2];
        _pInfoName = packName;
    }

    private String _pInfoName = String.Empty;
    private Texture2D? _pInfoSplash = null;
    private String _pInfoVersion = String.Empty;
    private String _pInfoAuthor = String.Empty;
    private String _pInfoDescription = String.Empty;
    private String _pInfoContent = String.Empty;

    private void DrawPackInformationMenu()
    {
        bool isEnabled = _enabledPackNames.Contains(_pInfoName) == true;

        if (_pInfoSplash != null)
        {
            Core.SpriteBatch.DrawInterface(_pInfoSplash, Core.ScreenSize, Color.White);
        }

        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 256, 160, 480, 64), true);
        String nameText = Localization.GetString("pack_menu_name") + ": " + _pInfoName;
        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, nameText,
            new Vector2((int)(Core.ScreenSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(nameText).X / 2), 195),
            Color.Black);

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 256, 288, 480, 224), true);
        Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont,
            Localization.GetString("pack_menu_version") + ": " + _pInfoVersion + Environment.NewLine +
            Localization.GetString("pack_menu_by") + ": " + _pInfoAuthor + Environment.NewLine +
            Localization.GetString("pack_menu_content") + ": " + _pInfoContent + Environment.NewLine +
            Localization.GetString("pack_menu_description") + ": " + _pInfoDescription.Replace("<br>", Environment.NewLine),
            new Vector2((int)(Core.ScreenSize.Width / 2) - 220, 323),
            Color.Black);

        for (int i = 0; i <= 1; i++)
        {
            if (i == _packInfoIndex)
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
            }
            else
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
            }

            String text = i == 0
                ? (isEnabled == true ? Localization.GetString("pack_menu_toggle_off") : Localization.GetString("pack_menu_toggle_on"))
                : Localization.GetString("pack_menu_back");

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 180 + 200 * i, 550, 128, 64), true);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text, new Vector2((int)(Core.ScreenSize.Width / 2) - 160 + 200 * i, 582), Color.Black);
        }
    }

    private void UpdatePackInformationMenu()
    {
        String packName = _pInfoName;

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 180 + 200 * i, 550, 160, 96)).Contains(MouseHandler.MousePosition) == true)
                {
                    _packInfoIndex = i;
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        if (i == 0) ButtonToggle(packName);
                        else menuIndex = 4;
                    }
                }
            }
        }

        if (Controls.Right(true, true, true, true) == true) _packInfoIndex += 1;
        if (Controls.Left(true, true, true, true) == true) _packInfoIndex -= 1;

        _packInfoIndex = (int)MathHelper.Clamp(_packInfoIndex, 0, 1);

        if (Controls.Accept(false) == true)
        {
            if (_packInfoIndex == 0) ButtonToggle(packName);
            else menuIndex = 4;
        }

        if (Controls.Dismiss(false) == true)
        {
            menuIndex = 4;
        }
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

    private void ButtonToggle(String packName)
    {
        if (_packNames.Count > 0)
        {
            if (_enabledPackNames.Contains(packName) == true)
            {
                _enabledPackNames.Remove(packName);
            }
            else
            {
                _enabledPackNames.Add(packName);
            }
            GetPacks(true);
        }
        else
        {
            GetPacks(true);
        }
    }

    private void ButtonApplyPacks()
    {
        if (_packNames.Count > 0)
        {
            Core.GameOptions.ContentPackNames = _enabledPackNames.ToArray();
            Core.GameOptions.SaveOptions();
            MediaPlayer.Stop();
            ContentPackManager.Clear();
            foreach (String s in Core.GameOptions.ContentPackNames)
            {
                ContentPackManager.Load(GameController.GamePath + @"\ContentPacks\" + s + @"\exceptions.dat");
            }
            MusicManager.PlayNoMusic();
            Core.OffsetMaps.Clear();
            Core.SetScreen(new MainMenuScreen());
        }
        menuIndex = 0;
    }

    private void DrawDeleteMenu()
    {
        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2 - 352), 172, 704, 96), Color.White, true);

        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, Localization.GetString("delete_menu_delete_confirm"),
            new Vector2((int)(Core.ScreenSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(Localization.GetString("delete_menu_delete_confirm")).X / 2), 200),
            Color.Black);

        String confirmStr = "\"" + _saveNames[_loadMenuIndex[0]] + "\" ?";
        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, confirmStr,
            new Vector2((int)(Core.ScreenSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(confirmStr).X / 2), 240),
            Color.Black);

        for (int i = 0; i <= 1; i++)
        {
            String text = i == 0 ? Localization.GetString("delete_menu_delete") : Localization.GetString("delete_menu_cancel");

            canvasTexture = i == _deleteIndex
                ? TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty)
                : TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 182 + i * 192, 370, 128, 64), true);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text, new Vector2((int)(Core.ScreenSize.Width / 2) - 164 + i * 192, 402), Color.Black);
        }
    }

    private void UpdateDeleteMenu()
    {
        if (Controls.Right(true, true, false) == true) _deleteIndex = 1;
        if (Controls.Left(true, true, false) == true) _deleteIndex = 0;

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 182 + i * 192, 370, 128 + 32, 64 + 32)).Contains(MouseHandler.MousePosition) == true)
                {
                    _deleteIndex = i;
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        if (_deleteIndex == 0) Delete();
                        else menuIndex = 1;
                    }
                }
            }
        }

        if (Controls.Accept(false, true) == true)
        {
            if (_deleteIndex == 0) Delete();
            else menuIndex = 1;
        }
    }

    private void Delete()
    {
        Directory.Delete(_saves[_loadMenuIndex[0]], true);

        bool deleteAutosave = false;
        foreach (String f in Directory.GetDirectories(GameController.GamePath + @"\Save\"))
        {
            if (File.Exists(f + @"\Player.dat") == true)
            {
                String[] data = File.ReadAllText(f + @"\Player.dat").SplitAtNewline();
                foreach (String line in data)
                {
                    if (line.StartsWith("AutoSave|") == true)
                    {
                        String autosaveName = line.GetSplit(1, "|");
                        if (autosaveName == Path.GetFileName(_saves[_loadMenuIndex[0]]))
                        {
                            deleteAutosave = true;
                        }
                    }
                }
            }
        }
        if (deleteAutosave == true)
        {
            Directory.Delete(GameController.GamePath + @"\Save\autosave", true);
        }

        _tempLoadDisplay = String.Empty;
        GetSaves();
        _loadMenuIndex[0] = 0;
        _loadMenuIndex[1] = 0;
        _loadMenuIndex[2] = 0;
        menuIndex = _saves.Count == 0 ? 0 : 1;
    }

    private String _tempGameModesDisplay = String.Empty;
    private Texture2D? _gameModeSplash = null;

    public void NewGameButton()
    {
        if (Core.GameOptions.StartedOfflineGame == true)
        {
            if (GameModeManager.GameModeCount < 2)
            {
                GameModeManager.SetGameModePointer("Kolben");
            }
            else
            {
                GetGameModes();
                _gameModeSplash = null;
                menuIndex = 6;
            }
        }
        else
        {
            Core.GameOptions.StartedOfflineGame = true;
            Core.GameOptions.SaveOptions();
            Core.SetScreen(new OfflineGameWarningScreen(this));
        }
    }

    private void DrawNewGameMenu()
    {
        if (_gameModeSplash != null)
        {
            Core.SpriteBatch.DrawInterface(_gameModeSplash, Core.ScreenSize, Color.White);
        }

        for (int i = 0; i <= 3; i++)
        {
            Color c = Color.White;
            if (i + _gameModeMenuIndex[2] == _gameModeMenuIndex[0])
            {
                c = new Color(101, 142, 255);
            }
            Canvas.DrawRectangle(new Rectangle((int)(Core.ScreenSize.Width / 2) - 258, 180 + i * 50, 480, 48), c, true);
        }

        Canvas.DrawScrollBar(new Vector2((int)(Core.ScreenSize.Width / 2) + 250, 180), _modeNames.Count, 4, _gameModeMenuIndex[2],
            new Size(4, 200), false, new Color(190, 190, 190), new Color(63, 63, 63), true);

        int x = _modeNames.Count - 1;
        x = (int)MathHelper.Clamp(x, 0, 3);

        for (int i = 0; i <= x; i++)
        {
            String name = _modeNames[i + _gameModeMenuIndex[2]];

            if (i + _gameModeMenuIndex[2] == _gameModeMenuIndex[0])
            {
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 245, 191 + i * 50), Color.Black);
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 248, 188 + i * 50), Color.White);
            }
            else
            {
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, name, new Vector2((int)(Core.ScreenSize.Width / 2) - 248, 188 + i * 50), Color.Black);
            }
        }

        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 272, 388, 512, 128), true);

        if (_tempGameModesDisplay == String.Empty && _modeNames.Count > 0)
        {
            GameMode? gameMode = GameModeManager.GetGameMode(_modeNames[_gameModeMenuIndex[0]]);
            if (gameMode != null)
            {
                _tempGameModesDisplay =
                    Localization.GetString("gamemode_menu_name") + ": " + gameMode.Name + Environment.NewLine +
                    Localization.GetString("gamemode_menu_version") + ": " + gameMode.Version + Environment.NewLine +
                    Localization.GetString("gamemode_menu_author") + ": " + gameMode.Author + Environment.NewLine +
                    Localization.GetString("gamemode_menu_contentpath") + ": " + gameMode.ContentPath + Environment.NewLine +
                    Localization.GetString("gamemode_menu_description") + ": " + gameMode.Description;
            }
        }

        Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont, _tempGameModesDisplay, new Vector2((int)(Core.ScreenSize.Width / 2) - 252, 416), Color.Black);

        for (int i = 0; i <= 1; i++)
        {
            if (i == _gameModeMenuIndex[1])
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
            }
            else
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
            }

            String text = i == 0 ? Localization.GetString("gamemode_menu_create") : Localization.GetString("gamemode_menu_back");

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.ScreenSize.Width / 2) - 180 + 200 * i, 550, 128, 64), true);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text, new Vector2((int)(Core.ScreenSize.Width / 2) - 160 + 200 * i, 582), Color.Black);
        }
    }

    private void UpdateNewGameMenu()
    {
        if (Controls.Up(true, true, true) == true)
        {
            _gameModeMenuIndex[0] -= 1;
            if (_gameModeMenuIndex[0] - _gameModeMenuIndex[2] < 0) _gameModeMenuIndex[2] -= 1;
            _tempGameModesDisplay = String.Empty;
            _gameModeSplash = null;
        }
        if (Controls.Down(true, true, true) == true)
        {
            _gameModeMenuIndex[0] += 1;
            if (_gameModeMenuIndex[0] + _gameModeMenuIndex[2] > 3) _gameModeMenuIndex[2] += 1;
            _tempGameModesDisplay = String.Empty;
            _gameModeSplash = null;
        }

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 180 + 200 * i, 550, 160, 96)).Contains(MouseHandler.MousePosition) == true)
                {
                    _gameModeMenuIndex[1] = i;
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        if (_gameModeMenuIndex[1] == 0) AcceptGameMode();
                        else { menuIndex = 0; _tempGameModesDisplay = String.Empty; }
                    }
                }
            }

            for (int i = 0; i <= 3; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 258, 180 + i * 50, 480, 48)).Contains(MouseHandler.MousePosition) == true)
                {
                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                    {
                        _gameModeMenuIndex[0] = i + _gameModeMenuIndex[2];
                        _tempGameModesDisplay = String.Empty;
                        _gameModeSplash = null;
                    }
                }
            }
        }

        _gameModeMenuIndex[0] = (int)MathHelper.Clamp(_gameModeMenuIndex[0], 0, _modeNames.Count - 1);
        _gameModeMenuIndex[2] = (int)MathHelper.Clamp(_gameModeMenuIndex[2], 0, _modeNames.Count - 4);

        if (Controls.Right(true, true, false) == true) _gameModeMenuIndex[1] += 1;
        if (Controls.Left(true, true, false) == true) _gameModeMenuIndex[1] -= 1;

        _gameModeMenuIndex[1] = (int)MathHelper.Clamp(_gameModeMenuIndex[1], 0, 1);

        if (Controls.Accept(false, true) == true)
        {
            if (_gameModeMenuIndex[1] == 0) AcceptGameMode();
            else { menuIndex = 0; _tempGameModesDisplay = String.Empty; }
        }

        if (_gameModeSplash == null && _modeNames.Count > 0)
        {
            try
            {
                String fileName = GameController.GamePath + @"\GameModes\" + _modeNames[_gameModeMenuIndex[0]] + @"\GameMode.png";
                if (File.Exists(fileName) == true)
                {
                    using Stream stream = File.Open(fileName, FileMode.OpenOrCreate);
                    _gameModeSplash = Texture2D.FromStream(Core.GraphicsDevice, stream);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogTypes.ErrorMessage, "MainMenuScreen.cs/UpdateNewGameMenu: " + ex.Message);
            }
        }

        if (Controls.Dismiss() == true)
        {
            menuIndex = 0;
        }
    }

    private void AcceptGameMode()
    {
        if (_modeNames.Count > 0)
        {
            GameModeManager.SetGameModePointer(_modeNames[_gameModeMenuIndex[0]]);
        }
    }

    public override void ChangeTo()
    {
        Core.Player.Unload();
        Core.Player.Skin = "Hilbert";
        TextBox.Hide();
        TextBox.CanProceed = true;
        OverworldScreen.FadeValue = 0;

        MusicManager.PlayMusic("title", true);
    }
}
