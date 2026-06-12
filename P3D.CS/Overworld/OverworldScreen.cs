using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class OverworldScreen : Screen
{
    private static Color _fadeColor = Color.Black;
    private static int _fadeValue = 0;
    private static int _drawRodID = -1;

    private ActionScript _actionScript;
    private Texture2D _particlesTexture = null!;
    private bool _trainerEncountered;
    private List<Title> _titles = [];

    private float _showControlsDelay = 4.0f;

    public String AfterRoamingBattleScript = String.Empty;
    public bool GlobalGameModeScriptStarted;
    public List<NotificationPopup> NotificationPopupList { get; } = [];

    public bool ActivatedScriptedNotification
    {
        get
        {
            if (NotificationPopupList.Count > 0)
            {
                if ((NotificationPopupList[0]._interacted == true || NotificationPopupList[0]._forceAccept == true) &&
                    NotificationPopupList[0]._scriptFile.Equals(String.Empty) == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public ActionScript ActionScript => _actionScript;
    public bool TrainerEncountered { get => _trainerEncountered; set => _trainerEncountered = value; }
    public List<Title> Titles => _titles;

    public static Color FadeColor { get => _fadeColor; set => _fadeColor = value; }
    public static int FadeValue { get => _fadeValue; set => _fadeValue = value; }
    public static int DrawRodID { get => _drawRodID; set => _drawRodID = value; }

    public OverworldScreen()
    {
        Identification = Identifications.OverworldScreen;
        CanChat = true;
        MouseVisible = false;

        Effect = new BasicEffectWithAlphaTest(Core.GraphicsDevice);
        Effect.FogEnabled = true;

        Camera = new OverworldCamera();
        SkyDome = new SkyDome();
        Level = new Level();
        Level.Load(Core.Player.StartMap);

        Level.Update();
        Screen.Camera!.Update();

        if (Level.Surfing == true)
        {
            MusicManager.Play("surf", true);
        }
        else
        {
            if (Level.Riding == true)
            {
                MusicManager.Play("ride", true);
            }
            else
            {
                MusicManager.Play(Level.MusicLoop, true, 0.01f);
            }
        }

        Level.RouteSign.Setup(Level.MapName);

        _actionScript = new ActionScript(Level);

        Screen.Level!.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);

        _particlesTexture = TextureManager.GetTexture(@"GUI\Overworld\Particles");

        if (GameController.IS_DEBUG_ACTIVE == true)
        {
            DebugFileWatcher.StartWatching();
        }
    }

    public override String GetScreenStatus()
    {
        return "IsSurfing=" + Level!.Surfing.ToString() + Environment.NewLine +
               "IsRiding=" + Level.Riding.ToString() + Environment.NewLine +
               "LevelFile=" + Level.LevelFile + Environment.NewLine +
               "UsedStrength=" + Level.UsedStrength.ToString() + Environment.NewLine +
               "EntityCount=" + Level.Entities.Count;
    }

    public override void Update()
    {
        if (GameModeManager.ActiveGameMode.StartScript.Equals(String.Empty) == false &&
            _actionScript.IsReady == true &&
            GlobalGameModeScriptStarted == false)
        {
            _actionScript.reDelay = 0f;
            _actionScript.StartScript(GameModeManager.ActiveGameMode.StartScript, 0, scriptTrigger: "StartScript");
            GlobalGameModeScriptStarted = true;
        }

        if (LevelLoader.MapScript.Equals(String.Empty) == false && _actionScript.IsReady == true)
        {
            _actionScript.reDelay = 0f;
            _actionScript.StartScript(LevelLoader.MapScript, 0, scriptTrigger: "MapScript");
            LevelLoader.MapScript = String.Empty;
        }

        BasicEffectWithAlphaTest? lightEffect = Screen.Effect;
        if (lightEffect != null)
        {
            Lighting.UpdateLighting(ref lightEffect);
        }

        ChooseBox.Update();
        if (ChooseBox.Showing == false)
        {
            TextBox.Update();
        }
        if (PokemonImageView.Showing == true)
        {
            PokemonImageView.Update();
        }
        if (ImageView.Showing == true)
        {
            ImageView.Update();
        }

        if (NotificationPopupList.Count > 0)
        {
            NotificationPopupList[0].Update();
            if (NotificationPopupList[0].IsReady == true)
            {
                NotificationPopupList.Remove(NotificationPopupList[0]);
            }
        }

        if (_actionScript.IsReady == true && Screen.Camera!.IsMoving == false)
        {
            if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.MiddleButton) == true ||
                ControllerHandler.ButtonPressed(Buttons.LeftStick) == true)
            {
                if (Core.Player.Pokemons.Count > 0)
                {
                    Core.SetScreen(new SummaryScreen(Core.CurrentScreen, Core.Player.Pokemons.ToArray(), 0));
                }
            }
        }

        if (TextBox.Showing == false && ChooseBox.Showing == false &&
            PokemonImageView.Showing == false && ImageView.Showing == false)
        {
            if (_actionScript.IsReady == true && LevelLoader.MapScript.Equals(String.Empty) == true)
            {
                if (HandleServerRequests() == true)
                {
                    Camera!.Update();
                    Level!.Update();
                }
            }
            else
            {
                if (JoinServerScreen.Online == true)
                {
                    Level!.SortEntities();
                }
            }

            if (KeyBoardHandler.KeyPressed(KeyBindings.OpenInventoryKey) == true ||
                ControllerHandler.ButtonPressed(Buttons.X) == true)
            {
                if (ActivatedScriptedNotification == false &&
                    Screen.Camera!.IsMoving == false &&
                    _actionScript.IsReady == true &&
                    Screen.Level!.DisabledMenus.Contains("all") == false &&
                    Screen.Level.DisabledMenus.Contains("startmenus") == false)
                {
                    Level!.RouteSign.Hide();
                    SoundManager.PlaySound("menu_open");
                    Core.SetScreen(new NewMenuScreen(this));
                }
            }

            if (KeyBoardHandler.KeyPressed(KeyBindings.SpecialKey) == true ||
                ControllerHandler.ButtonPressed(Buttons.Back) == true)
            {
                if (NotificationPopupList.Count > 0)
                {
                    NotificationPopupList[0].Dismiss();
                }
                else
                {
                    if (Core.Player.HasPokegear == true ||
                        GameController.IS_DEBUG_ACTIVE == true ||
                        Core.Player.SandBoxMode == true)
                    {
                        if (ActivatedScriptedNotification == false &&
                            Screen.Camera!.IsMoving == false &&
                            _actionScript.IsReady == true &&
                            Screen.Level!.DisabledMenus.Contains("pokegear") == false &&
                            Screen.Level.DisabledMenus.Contains("all") == false &&
                            Screen.Level.IsBugCatchingContest == false)
                        {
                            Core.SetScreen(new GameJolt.PokegearScreen(Core.CurrentScreen, GameJolt.PokegearScreen.EntryModes.MainMenu, []));
                        }
                    }
                }
            }

            _actionScript.Update();
        }
        else
        {
            if (Camera!.Name.Equals("Overworld") == true)
            {
                OverworldCamera c = (OverworldCamera)Camera;
                if (c.ThirdPerson == false)
                {
                    if (c.IsPointingToNormalDirection() == false)
                    {
                        if (Camera.Turning == false)
                        {
                            Camera.Turning = true;
                            c.SetAimDirection(Camera.GetFacingDirection());
                        }
                        c.AimCamera();
                        Level!.UpdateEntities();
                    }
                }
                c.PitchForward();
                c.UpdateViewMatrix();
                c.UpdateFrustum();
            }

            if (JoinServerScreen.Online == true)
            {
                foreach (NetworkPlayer p in Level!.NetworkPlayers)
                {
                    p.UpdateEntity();
                    p.Update();
                }
                foreach (NetworkPokemon p in Level.NetworkPokemon)
                {
                    p.UpdateEntity();
                    p.Update();
                }
            }
        }

        if (Core.Player.CheckForTrainersLater == true &&
            Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true &&
            ((OverworldScreen)Core.CurrentScreen)._actionScript.IsReady == true)
        {
            Screen.Level!.PokemonEncounterData.EncounteredPokemon = false;
            Screen.Level.CheckTrainerSights();
            if (Core.CurrentScreen.Identification.Equals(Screen.Identifications.OverworldScreen) == true)
            {
                if (((OverworldScreen)Core.CurrentScreen)._actionScript.IsReady == false)
                {
                    Core.Player.stepEventStartedTrainer = true;
                }
            }
            Core.Player.CheckForTrainersLater = false;
        }

        SkyDome!.Update();
        Level!.RouteSign.Update();
        Screen.Level!.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);

        UpdateShowControlDelay();
        GameJolt.LogInScreen.KickFromOnlineScreen(this);
        UpdateTitles();
    }

    private void UpdateShowControlDelay()
    {
        if (_showControlsDelay > 0f)
        {
            if (Core.GameOptions.GamePadEnabled == true && GamePad.GetState(PlayerIndex.One).IsConnected == true)
            {
                _showControlsDelay -= 0.1f;
                if (_showControlsDelay <= 0f)
                {
                    _showControlsDelay = 0f;
                }
            }
        }
        if (Screen.Camera!.IsMoving == true ||
            Camera!.Turning == true ||
            _actionScript.IsReady == false ||
            TextBox.Showing == true ||
            ChooseBox.Showing == true)
        {
            _showControlsDelay = 8f;
        }
    }

    private bool HandleServerRequests()
    {
        if (Screen.Level!.IsBugCatchingContest == false)
        {
            if (GameJolt.PokegearScreen.BattleRequestData != -1)
            {
                if (ActivatedScriptedNotification == false &&
                    Core.ServersManager.PlayerCollection.HasPlayer(GameJolt.PokegearScreen.BattleRequestData) == true &&
                    Screen.Level.DisabledMenus.Contains("pokegear") == false &&
                    Screen.Level.DisabledMenus.Contains("all") == false)
                {
                    Core.SetScreen(new GameJolt.PokegearScreen(Core.CurrentScreen,
                        GameJolt.PokegearScreen.EntryModes.BattleRequest,
                        [GameJolt.PokegearScreen.BattleRequestData,
                         Core.ServersManager.PlayerCollection.GetPlayer(GameJolt.PokegearScreen.BattleRequestData).GameJoltId]));
                    return false;
                }
                else
                {
                    GameJolt.PokegearScreen.BattleRequestData = -1;
                }
            }
            if (GameJolt.PokegearScreen.TradeRequestData != -1)
            {
                if (ActivatedScriptedNotification == false &&
                    Core.ServersManager.PlayerCollection.HasPlayer(GameJolt.PokegearScreen.TradeRequestData) == true &&
                    Screen.Level.DisabledMenus.Contains("pokegear") == false &&
                    Screen.Level.DisabledMenus.Contains("all") == false)
                {
                    Core.SetScreen(new GameJolt.PokegearScreen(Core.CurrentScreen,
                        GameJolt.PokegearScreen.EntryModes.TradeRequest,
                        [GameJolt.PokegearScreen.TradeRequestData,
                         Core.ServersManager.PlayerCollection.GetPlayer(GameJolt.PokegearScreen.TradeRequestData).GameJoltId]));
                    return false;
                }
                else
                {
                    GameJolt.PokegearScreen.TradeRequestData = -1;
                }
            }
        }
        else
        {
            if (GameJolt.PokegearScreen.BattleRequestData != -1)
            {
                GameJolt.PokegearScreen.BattleRequestData = -1;
            }
            if (GameJolt.PokegearScreen.TradeRequestData != -1)
            {
                GameJolt.PokegearScreen.TradeRequestData = -1;
            }
        }
        return true;
    }

    public override void Draw()
    {
        SkyDome!.Draw(Camera!.FOV);
        Level!.Draw();
        World.DrawWeather(Screen.Level!.World.CurrentMapWeather);

        DrawGUI();

        PokemonImageView.Draw();
        ImageView.Draw();
        TextBox.Draw();

        if (IsCurrentScreen() == true)
        {
            ChooseBox.Draw();
        }

        Level.RouteSign.Draw();

        if (NotificationPopupList.Count > 0)
        {
            NotificationPopupList[0].Draw();
        }

        if (_showControlsDelay == 0f)
        {
            Dictionary<Buttons, String> d = [];

            if (NotificationPopupList.Count > 0)
            {
                d.Add(Buttons.A, Localization.GetString("game_interaction_notification", "Notification"));
            }
            else
            {
                d.Add(Buttons.A, Localization.GetString("game_interaction_interact", "Interact"));
            }

            d.Add(Buttons.X, Localization.GetString("game_interaction_gamemenu", "Game Menu"));
            if (Core.Player.HasPokegear == true)
            {
                d.Add(Buttons.Back, Localization.GetString("game_interaction_pokegear", "Pokégear"));
            }
            d.Add(Buttons.Start, Localization.GetString("game_interaction_pausemenu", "Game Menu"));

            DrawGamePadControls(d);
        }
    }

    private void DrawGUI()
    {
        bool isThirdPerson = true;
        if (Camera!.Name.Equals("Overworld") == true)
        {
            OverworldCamera c = (OverworldCamera)Camera;
            isThirdPerson = c.ThirdPerson;
        }

        if (DrawRodID > -1 && isThirdPerson == false)
        {
            Texture2D t = TextureManager.GetTexture(@"GUI\Overworld\Rods",
                new Rectangle(DrawRodID * 8, 0, 8, 64), String.Empty);
            Vector2 p = new Vector2((float)(Core.windowSize.Width / 2 - 32), Core.windowSize.Height - 490);
            Core.SpriteBatch.Draw(t, new Rectangle((int)p.X, (int)p.Y, 64, 512), Color.White);
        }

        if (Core.GameOptions.ShowGUI == true && isThirdPerson == false)
        {
            Vector2 p = Core.GetMiddlePosition(new Size(16, 16));
            Core.SpriteBatch.Draw(_particlesTexture,
                new Rectangle((int)p.X, (int)p.Y, 16, 16),
                new Rectangle(0, 0, 9, 9), Color.White);
        }

        foreach (Title title in _titles)
        {
            title.Draw();
        }

        if (FadeValue > 0)
        {
            Canvas.DrawRectangle(Core.windowSize,
                new Color(FadeColor.R, FadeColor.G, FadeColor.B, FadeValue));
        }
    }

    public override void ChangeTo()
    {
        DrawRodID = -1;

        OverworldCamera c = (OverworldCamera)Screen.Camera!;
        Mouse.SetPosition((int)(Core.windowSize.Width / 2), (int)(Core.windowSize.Height / 2));
        c.oldMousePos = new Vector2((int)(Core.windowSize.Width / 2), (int)(Core.windowSize.Height / 2));
        Player.Temp.IsInBattle = false;

        if (_trainerEncountered == false)
        {
            String theme = Level!.MusicLoop;
            if (Screen.Level!.Surfing == true)
            {
                theme = "surf";
            }
            if (Screen.Level.Riding == true)
            {
                theme = "ride";
            }

            if (Level.IsRadioOn == true && GameJolt.PokegearScreen.StationCanPlay(Screen.Level.SelectedRadioStation) == true)
            {
                theme = Level.SelectedRadioStation.Music;
            }
            else
            {
                Level.IsRadioOn = false;
            }

            if (MusicManager._currentSongName.Equals("silence") == true || MusicManager.CurrentSong == null)
            {
                MusicManager.Play(theme, true);
            }
        }

        if (AfterRoamingBattleScript.Equals(String.Empty) == false)
        {
            _actionScript.StartScript(AfterRoamingBattleScript, 0, scriptTrigger: "AfterRoamingBattleScript");
            AfterRoamingBattleScript = String.Empty;
        }
    }

    public override void ChangeFrom()
    {
        CursorClipper.ReleaseCursor();
    }

    private void UpdateTitles()
    {
        foreach (Title t in _titles)
        {
            t.Update();
            if (t.IsReady == true)
            {
                _titles.Remove(t);
                break;
            }
        }
    }

    public class Title
    {
        private String _text = "Sample Text";
        private Color _textColor = Color.White;
        private float _scale = 10.0f;
        private Vector2 _position = Vector2.Zero;
        private bool _isCentered = true;
        private float _delay = 20.0f;

        public String Text { get => _text; set => _text = value; }
        public Color TextColor { get => _textColor; set => _textColor = value; }
        public float Scale { get => _scale; set => _scale = value; }
        public Vector2 Position { get => _position; set => _position = value; }
        public bool IsCentered { get => _isCentered; set => _isCentered = value; }
        public float Delay { get => _delay; set => _delay = value; }
        public bool IsReady => _delay == 0f;

        public Title()
        {
        }

        public Title(String text, float delay, Color textColor, float scale, Vector2 position, bool isCentered)
        {
            _text = text;
            _delay = delay;
            _textColor = textColor;
            _scale = scale;
            _position = position;
            _isCentered = isCentered;
        }

        public void Draw()
        {
            Vector2 p = Vector2.Zero;

            if (_isCentered == true)
            {
                Vector2 v = FontManager.TextFont.MeasureString(_text) * _scale;
                p = new Vector2(
                    (float)(Core.windowSize.Width / 2 - v.X / 2),
                    (float)(Core.windowSize.Height / 2 - v.Y / 2));
            }
            p += _position;

            int a = 255;
            if (_delay <= 3.0f)
            {
                a = (int)(255 * (1f / 3f * _delay));
            }

            Core.SpriteBatch.DrawString(FontManager.TextFont, _text, p,
                new Color(_textColor.R, _textColor.G, _textColor.B, a),
                0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
        }

        public void Update()
        {
            if (_delay > 0f)
            {
                _delay -= 0.1f;
                if (_delay <= 0f)
                {
                    _delay = 0f;
                }
            }
        }
    }
}
