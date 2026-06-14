using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class CreditsScreen : Screen
{
    private const String COPYRIGHTYEAR = "2026";

    private List<CreditsPage> _creditsPages = new List<CreditsPage>();
    private int _currentPageIndex = 0;

    private List<CameraLevel> _cameraLevels = new List<CameraLevel>();
    private int _currentCameraLevelIndex = 0;
    private bool _executedCameraLevel = false;
    private bool _canBeSkipped = false;

    private bool _theEnd = false;
    private int _fadeAlpha = 0;
    private DateTime _waitAfterSkipping = DateTime.Now;

    public OverworldStorage SavedOverworld;

    public CreditsScreen(Screen overworldScreen)
    {
        SavedOverworld = new OverworldStorage();
        SavedOverworld.SetToCurrentEnvironment();
    }

    public void InitializeScreen(String ending, bool canBeSkipped = false)
    {
        Identification = Identifications.CreditsScreen;
        CanBePaused = false;
        MouseVisible = false;
        CanChat = false;
        CanDrawDebug = true;
        CanMuteAudio = true;
        CanTakeScreenshot = true;
        _canBeSkipped = canBeSkipped;
        _waitAfterSkipping = DateTime.Now;

        Screen.TextBox.Showing = false;
        Screen.PokemonImageView.Showing = false;
        Screen.ImageView.Showing = false;
        Screen.ChooseBox.Showing = false;

        Effect = new BasicEffectWithAlphaTest(Core.GraphicsDevice);
        Effect.FogEnabled = true;
        SkyDome = new SkyDome();
        Camera = new CreditsCamera();

        InitializeCreditsPages(ending);
        InitializeCameraLevels(ending);
        if (_cameraLevels != null && _cameraLevels.Count > 0)
        {
            Level = new Level();
            ExecuteCameraLevel();
        }
        MusicManager.Stop();
        MusicManager.Play("credits", true, false);
    }

    private void InitializeCreditsPages(String ending)
    {
        _creditsPages.Add(new CreditsPage("Pokémon 3D Staff", Color.White, Color.Black));
        _creditsPages.Add(new CreditsPage("Pokémon", Color.White, Color.Black, new List<String> { "made by", "Nintendo", "Game Freak", "The Pokémon Company" }));
        if (GameModeManager.ActiveGameMode.IsDefaultGamemode == true)
        {
            _creditsPages.Add(new CreditsPage("Pokémon 3D", Color.White, Color.Black, new List<String> { "Trademark (TM) 2012 - " + COPYRIGHTYEAR, "made by Kolben Games" }));
        }
        else
        {
            _creditsPages.Add(new CreditsPage("Pokémon 3D", Color.White, Color.Black, new List<String> { "Trademark (TM) 2012 - " + COPYRIGHTYEAR, "made by Kolben Games", String.Empty, "GameMode made by", GameModeManager.ActiveGameMode.Author }));
        }
        _creditsPages.Add(new CreditsPage("Pokémon 3D Team", Color.White, Color.Black, new List<String> { "Benjamin Smith", "\"Aragas\"", "\"Fanta\"", "Jorge Luis Espinoza", "Conner Joseph Brewster", "\"The Omega Ghost\"", "Daniel S. Billing", "Jasper Speelman" }));
        _creditsPages.Add(new CreditsPage("Director", Color.White, Color.Black, new List<String> { "Benjamin Smith" }));
        _creditsPages.Add(new CreditsPage("Initial Development", Color.White, Color.Black, new List<String> { "\"nilllzz\"", "Jason Houston", "Daniel S. Billing", "Benjamin Smith", "Hunter Graves" }));
        _creditsPages.Add(new CreditsPage("Programming", Color.White, Color.Black, new List<String> { "Benjamin Smith", "Jorge Luis Espinoza", "\"Aragas\"", "Jasper Speelman" }));
        _creditsPages.Add(new CreditsPage("Script System Development", Color.White, Color.Black, new List<String> { "Benjamin Smith", "\"Aragas\"", "Yong Jian Ming", "Jasper Speelman" }));
        _creditsPages.Add(new CreditsPage("Website Host/Server Development", Color.White, Color.Black, new List<String> { "Daniel S. Billing", "\"Aragas\"", "\"iErws[GR]\"" }));

        if (Core.Player.IsGameJoltSave == true)
            _creditsPages.Add(new CreditsPage("GameJolt Service/API Programming", Color.White, Color.Black, new List<String> { "David DeCarmine", "\"nilllzz\"" }));

        if (GameModeManager.ActiveGameMode.IsDefaultGamemode == true || System.IO.File.Exists(GameModeManager.GetContentFilePath(@"Data\Credits.dat")) == false)
        {
            _creditsPages.Add(new CreditsPage("Graphic Design", Color.White, Color.Black, new List<String> { "Benjamin Smith", "\"The Omega Ghost\"", "Jasper Speelman", "\"Godeken\"", "Caleb Coleman", "Miguel Nunez", "Grant Garrett", "\"Anvil555\"", "\"princess-phoenix\"", "\"AgentPaperCraft\"" }));
            _creditsPages.Add(new CreditsPage("Map Design", Color.White, Color.Black, new List<String> { "Benjamin Smith", "\"Fanta\"", "Conner Joseph Brewster" }));
            _creditsPages.Add(new CreditsPage("ActionScript", Color.White, Color.Black, new List<String> { "Benjamin Smith", "\"Fanta\"", "Conner Joseph Brewster", "Jasper Speelman" }));
            _creditsPages.Add(new CreditsPage("Community Staff", Color.White, Color.Black, new List<String> { "Conner Joseph Brewster", "Benjamin Smith", "Daniel S. Billing", "\"Fanta\"", "\"MamaLeef\"", "Tim ten Brink" }));
            _creditsPages.Add(new CreditsPage("0.59 QA Team", Color.White, Color.Black, new List<String> { "Tim ten Brink", "\"iErws[GR]\"", "\"Runaryu/agravedigger\"", "\"AlexCorruptor\"", "\"HantomPro\"", "\"Lexichu_\"" }));
            _creditsPages.Add(new CreditsPage("Past QA Team", Color.White, Color.Black, new List<String> { "Tim Drescher", "Daniel Steinborn", "Marc Boisvert-Dupras", "Matt Chambers", "William Hunn", "Torben Carrington", "\"Sanio\"", "\"Vanilla\"" }));
            _creditsPages.Add(new CreditsPage("Legacy Contributors", Color.White, Color.Black, new List<String> { "Yong Jian Ming", "Andrew Leach", "Manuel Lampe", "Robert Nobbmann", "Maximilian Schröder", "Jan Mika Eine" }));
            _creditsPages.Add(new CreditsPage("Special Thanks", Color.White, Color.Black, new List<String> { "\"MunchingOrange\"", "\"TheFlamingSpade\"", "\"SlyFoxHound\"", "\"ArsenioDev\"", "\"TrUShade\"", "\"Isaaking6\"", "\"ParadiseGamer13\"" }));
            _creditsPages.Add(new CreditsPage("Special Thanks", Color.White, Color.Black, new List<String> { "Davey Van Raaij", "Diego López", "The GameJolt Team", "The AppSharp Team", "The Smogon University Sprite Project Team" }));
            _creditsPages.Add(new CreditsPage(String.Empty, Color.White, Color.Black, new List<String> { "And probably a lot more.", "Especially all the awesome people from", "the Discord server and", "the pokemon3d.net community.", "Thanks for helping and playing this great game." }));
            if (Core.Player.IsGameJoltSave == false)
                _creditsPages.Add(new CreditsPage(String.Empty, Color.White, Color.Black));
            _creditsPages.Add(new CreditsPage(String.Empty, Color.White, Color.Black));
            _creditsPages.Add(new CreditsPage(String.Empty, Color.White, Color.Black));
            _creditsPages.Add(new CreditsPage("THE END", Color.White, Color.Black, new List<String> { "Thank you for playing!" }));
        }
        else
        {
            String[] credits = System.IO.File.ReadAllLines(GameModeManager.GetContentFilePath(@"Data\Credits.dat"));
            foreach (String line in credits)
            {
                if (line.Contains("|") == true && line.StartsWith("#") == false && line.StartsWith("Credits") == true)
                {
                    if (line.GetSplit(1, "|").ToLower() == ending.ToLower() || line.GetSplit(1, "|") == String.Empty)
                    {
                        String creditTitle = line.GetSplit(2, "|");
                        List<String> creditNames = line.GetSplit(3, "|").Split(",").ToList();
                        _creditsPages.Add(new CreditsPage(creditTitle, Color.White, Color.Black, creditNames));
                    }
                }
            }
        }
    }

    private void InitializeCameraLevels(String ending)
    {
        if (GameModeManager.ActiveGameMode.IsDefaultGamemode == true || System.IO.File.Exists(GameModeManager.GetContentFilePath(@"Data\Credits.dat")) == false)
        {
            switch (ending.ToLower())
            {
                case "johto":
                    _cameraLevels.Add(new CameraLevel("route29.dat", new Vector3(50, 2, 10), new Vector3(0, 2, 10), 0.04f, MathHelper.Pi * 1.5f, -0.3f));
                    _cameraLevels.Add(new CameraLevel(@"routes\route42.dat", new Vector3(50, 2, 10), new Vector3(0, 2, 10), 0.04f, MathHelper.Pi * 1.5f, -0.3f));
                    _cameraLevels.Add(new CameraLevel("route38.dat", new Vector3(34, 2, 10), new Vector3(0, 2, 10), 0.04f, MathHelper.Pi * 1.5f, -0.3f));
                    _cameraLevels.Add(new CameraLevel(@"routes\route45.dat", new Vector3(10, 2, 3), new Vector3(10, 2, 30), 0.04f, 0.0f, -0.3f));
                    _cameraLevels.Add(new CameraLevel("Ecruteak.dat", new Vector3(22, 0, 22), new Vector3(36, 14, 8), 0.04f, MathHelper.Pi * 1.7f, 0.3f));
                    _cameraLevels.Add(new CameraLevel(@"routes\route34.dat", new Vector3(10, 2, 3), new Vector3(10, 2, 40), 0.04f, 0.0f, -0.3f));
                    _cameraLevels.Add(new CameraLevel("route31.dat", new Vector3(29, 2, 10), new Vector3(0, 2, 10), 0.04f, MathHelper.Pi * 1.5f, -0.3f));
                    _cameraLevels.Add(new CameraLevel(@"routes\route43.dat", new Vector3(10, 2, 3), new Vector3(10, 2, 30), 0.04f, 0.0f, -0.3f));
                    _cameraLevels.Add(new CameraLevel("barktown.dat", new Vector3(20, 1.5f, 14), new Vector3(20, 1.5f, 28), 0.04f, 0.0f, -0.1f));
                    break;
                case "kanto":
                    break;
            }
        }
        else
        {
            String[] credits = System.IO.File.ReadAllLines(GameModeManager.GetContentFilePath(@"Data\Credits.dat"));
            foreach (String line in credits)
            {
                if (line.Contains("|") == true && line.StartsWith("#") == false && line.StartsWith("Background") == true)
                {
                    if (line.GetSplit(1, "|").ToLower() == ending.ToLower() || line.GetSplit(1, "|") == String.Empty)
                    {
                        String mapPath = line.GetSplit(2, "|");

                        String[] startPosition = line.GetSplit(3, "|").Split(",");
                        Vector3 startVector = new Vector3(float.Parse(startPosition[0].InsertDecSeparator()), float.Parse(startPosition[1].InsertDecSeparator()), float.Parse(startPosition[2].InsertDecSeparator()));

                        String[] endPosition = line.GetSplit(4, "|").Split(",");
                        Vector3 endVector = new Vector3(float.Parse(endPosition[0].InsertDecSeparator()), float.Parse(endPosition[1].InsertDecSeparator()), float.Parse(endPosition[2].InsertDecSeparator()));

                        float cameraSpeed = float.Parse(line.GetSplit(5, "|").InsertDecSeparator()) * 0.04f;
                        float cameraYaw = float.Parse(line.GetSplit(6, "|").InsertDecSeparator());
                        float cameraPitch = float.Parse(line.GetSplit(7, "|").InsertDecSeparator());
                        _cameraLevels.Add(new CameraLevel(mapPath, endVector, startVector, cameraSpeed, cameraYaw, cameraPitch));
                    }
                }
            }
        }
    }

    private void ExecuteCameraLevel()
    {
        if (_cameraLevels != null && _cameraLevels.Count > 0)
        {
            if (_executedCameraLevel == false)
            {
                _executedCameraLevel = true;
                _cameraLevels[_currentCameraLevelIndex].Apply((CreditsCamera)Camera);
            }
        }
    }

    public override void Draw()
    {
        if (_cameraLevels != null && _cameraLevels.Count > 0)
        {
            SkyDome.Draw(Camera.FOV);
            Level.Draw();
        }
        else
        {
            Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), new Color(0, 0, 0));
        }

        if (_theEnd == true)
        {
            if (_fadeAlpha < 255)
            {
                _fadeAlpha += 5;
                if (_fadeAlpha >= 255)
                    _fadeAlpha = 255;
            }

            String continueString = Localization.GetString("credits_continue", "Press [<system.button(enter1)>] to continue.");
            Core.SpriteBatch.DrawString(FontManager.InGameFont, continueString,
                new Vector2((int)(Core.windowSize.Width / 2 - FontManager.InGameFont.MeasureString(continueString).X / 2 + 2), (int)(Core.windowSize.Height - 128 + 2)),
                new Color(Color.Black, _fadeAlpha));
            Core.SpriteBatch.DrawString(FontManager.InGameFont, continueString,
                new Vector2((int)(Core.windowSize.Width / 2 - FontManager.InGameFont.MeasureString(continueString).X / 2), (int)(Core.windowSize.Height - 128)),
                new Color(Color.White, _fadeAlpha));

            _creditsPages[_creditsPages.Count - 1].Draw();
        }
        else
        {
            if (_canBeSkipped == true && _currentPageIndex > 2)
            {
                if (_fadeAlpha < 255)
                {
                    _fadeAlpha += 5;
                    if (_fadeAlpha >= 255)
                        _fadeAlpha = 255;
                }

                String skipString = Localization.GetString("credits_skip", "Press [<system.button(enter1)>] to skip to the end.");
                Core.SpriteBatch.DrawString(FontManager.InGameFont, skipString,
                    new Vector2((int)(Core.windowSize.Width / 2 - FontManager.InGameFont.MeasureString(skipString).X / 2 + 2), (int)(Core.windowSize.Height - 128 + 2)),
                    new Color(Color.Black, _fadeAlpha));
                Core.SpriteBatch.DrawString(FontManager.InGameFont, skipString,
                    new Vector2((int)(Core.windowSize.Width / 2 - FontManager.InGameFont.MeasureString(skipString).X / 2), (int)(Core.windowSize.Height - 128)),
                    new Color(Color.White, _fadeAlpha));
            }

            _creditsPages[_currentPageIndex].Draw();
        }
    }

    public override void Update()
    {
        if (_cameraLevels != null && _cameraLevels.Count > 0)
        {
            Camera.Update();
            Level.Update();
        }

        _creditsPages[_currentPageIndex].Update();

        int nextPageIndex = _currentPageIndex + 1;
        bool skipPage = false;

        if (_canBeSkipped == true && _currentPageIndex > 2 && _theEnd == false)
        {
            if (Controls.Accept(true, true) == true)
            {
                SoundManager.PlaySound("select");
                nextPageIndex = _creditsPages.Count - 1;
                skipPage = true;
                _waitAfterSkipping = DateTime.Now + new TimeSpan(0, 0, 1);
            }
        }

        if ((_creditsPages[_currentPageIndex].IsReady == true || skipPage == true) && _theEnd == false)
        {
            _currentPageIndex = nextPageIndex;
            if (_currentPageIndex == _creditsPages.Count - 1)
                _theEnd = true;
        }

        if (_theEnd == true)
        {
            if (_creditsPages[_currentPageIndex].OnScreenTime >= 500)
                _creditsPages[_currentPageIndex].AlwaysVisible = true;
        }

        if (_theEnd == true)
        {
            if (DateTime.Now >= _waitAfterSkipping)
            {
                if (Controls.Accept(true, true) == true)
                {
                    SoundManager.PlaySound("select");
                    Core.SetScreen(new TransitionScreen(this, SavedOverworld.OverworldScreen, Color.Black, false, ChangeSavedScreen));
                }
            }
        }

        if (_cameraLevels != null && _cameraLevels.Count > 0)
        {
            if (((CreditsCamera)Camera).IsReady == true && _theEnd == false)
            {
                _currentCameraLevelIndex += 1;
                if (_currentCameraLevelIndex > _cameraLevels.Count - 1)
                    _currentCameraLevelIndex = 0;
                _executedCameraLevel = false;
            }

            ExecuteCameraLevel();
        }

        SkyDome.Update();
    }

    public void ChangeSavedScreen()
    {
        Screen.Level = SavedOverworld.Level;
        Screen.Camera = SavedOverworld.Camera;
        Screen.Effect = SavedOverworld.Effect;
        Screen.SkyDome = SavedOverworld.SkyDome;
        Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
    }

    private class CreditsPage
    {
        private String _title = "Test";
        private List<String> _rows = new List<String>();
        private Color _color = Color.White;
        private Color _color2 = Color.Black;
        private Texture2D? _image = null;

        private int _onScreenTime = 0;
        private bool _alwaysVisible = false;

        public CreditsPage(String title, Color color1, Color color2)
            : this(title, color1, color2, new List<String>()) { }

        public CreditsPage(String title, Color color1, Color color2, List<String> rows)
            : this(title, color1, color2, rows, null) { }

        public CreditsPage(String title, Color color1, Color color2, List<String> rows, Texture2D? image)
        {
            _title = title;
            _color = color1;
            _color2 = color2;
            _rows = rows;
            _image = image;
        }

        public void Draw()
        {
            String title = _title;
            if (Localization.TokenExists("credits_title_" + _title) == true)
                title = Localization.GetString("credits_title_" + _title, _title);

            Vector2 posTitle = new Vector2((int)(Core.windowSize.Width / 2 - FontManager.InGameFont.MeasureString(title).X / 2), 100);

            Core.SpriteBatch.DrawString(FontManager.InGameFont, title, new Vector2(posTitle.X + 2, posTitle.Y + 2), AColor(_color2));
            Core.SpriteBatch.DrawString(FontManager.InGameFont, title, posTitle, AColor(_color));

            for (int i = 0; i <= _rows.Count - 1; i++)
            {
                String line = _rows[i];
                if (Localization.TokenExists("credits_line_" + _rows[i]) == true)
                    line = Localization.GetString("credits_line_" + _rows[i], _rows[i]);

                Vector2 posLine = new Vector2((int)(Core.windowSize.Width / 2 - FontManager.MainFont.MeasureString(line).X / 2), 200 + i * 35);
                Core.SpriteBatch.DrawString(FontManager.MainFont, line, new Vector2(posLine.X + 2, posLine.Y + 2), AColor(_color2));
                Core.SpriteBatch.DrawString(FontManager.MainFont, line, posLine, AColor(_color));
            }
        }

        public void Update()
        {
            _onScreenTime += 4;
        }

        private byte GetAlphaValue()
        {
            if (_alwaysVisible == true)
                return 255;
            if (_onScreenTime < 255)
                return (byte)_onScreenTime;
            if (_onScreenTime > 1255)
                return 0;
            if (_onScreenTime > 1000)
                return (byte)(255 - (_onScreenTime - 1000));
            return 255;
        }

        private Color AColor(Color c)
        {
            return new Color(c.R, c.G, c.B, GetAlphaValue());
        }

        public bool IsReady => _onScreenTime > 1255;

        public int OnScreenTime => _onScreenTime;

        public bool AlwaysVisible
        {
            get => _alwaysVisible;
            set => _alwaysVisible = value;
        }
    }

    private class CameraLevel
    {
        private String _levelFile = "testlevel.dat";
        private Vector3 _target = new Vector3(0);
        private Vector3 _startPosition = new Vector3(0);
        private float _speed = 0.04f;
        private float _yaw = 0.0f;
        private float _pitch = 0.0f;

        public CameraLevel(String levelFile, Vector3 target, Vector3 startPosition, float speed, float yaw, float pitch)
        {
            _levelFile = levelFile;
            _target = target;
            _startPosition = startPosition;
            _speed = speed;
            _yaw = yaw;
            _pitch = pitch;
        }

        public void Apply(CreditsCamera creditsCamera)
        {
            creditsCamera.Speed = _speed;
            creditsCamera.Position = _startPosition;
            creditsCamera.Target = _target;
            creditsCamera.Yaw = _yaw;
            creditsCamera.Pitch = _pitch;

            Screen.Level.Load(_levelFile);
        }
    }
}
