using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class NewMenuScreen : Screen
{
    private List<String> _menuOptions = [];
    public int _menuIndex = 0;

    private Texture2D? _texture;

    private int _gradientFade = 0;
    private int _buttonFadeIndex = 0;
    private int _currentButtonFade = 0;
    private bool _buttonIntroFinished = false;

    private Vector2 _cursorPosition = Vector2.Zero;
    public Vector2 _cursorDestPosition = Vector2.Zero;

    private RenderTarget2D? _preScreenTexture;
    private RenderTarget2D? _preScreenTarget;
    private Resources.Blur.BlurHandler? _blur;

    public NewMenuScreen(Screen currentScreen)
    {
        Identification = Identifications.MenuScreen;
        PreScreen = currentScreen;
        IsDrawingGradients = true;

        MouseVisible = true;
        if (Core.windowSize.Width > 0 && Core.windowSize.Height > 0)
        {
            _preScreenTarget = new RenderTarget2D(Core.GraphicsDevice, Core.windowSize.Width, Core.windowSize.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _blur = new Resources.Blur.BlurHandler(Core.windowSize.Width, Core.windowSize.Height);
        }
        _texture = TextureManager.GetTexture(@"GUI\Menus\General");

        ConstructMenu();

        if (Player.Temp.MenuIndex < _menuOptions.Count)
            _menuIndex = Player.Temp.MenuIndex;
        else
            _menuIndex = 0;
        SetCursorPosition(_menuIndex);
        _cursorPosition = _cursorDestPosition;
    }

    private void ConstructMenu()
    {
        if (Level.DisabledMenus.Contains("pokedex") == false)
        {
            if (Core.Player.HasPokedex == true)
                _menuOptions.Add(Localization.GetString("game_menu_pokedex", "Pokédex"));
        }

        if (Level.IsBugCatchingContest == true)
        {
            _menuOptions.Add(Level.BugCatchingContestData.GetSplit(2) + " x" + Core.Player.Inventory.GetItemAmount(177.ToString()));
            if (Level.DisabledMenus.Contains("bag") == false)
                _menuOptions.Add(Localization.GetString("game_menu_bag", "Bag"));
            if (Level.DisabledMenus.Contains("trainercard") == false)
                _menuOptions.Add("|||" + Core.Player.Name);
            _menuOptions.Add(Localization.GetString("game_menu_end_contest", "End Contest"));
        }
        else
        {
            if (Level.DisabledMenus.Contains("pokemon") == false)
            {
                if (Core.Player.Pokemons.Count > 0)
                    _menuOptions.Add(Localization.GetString("game_menu_party", "Pokémon"));
            }
            if (Level.DisabledMenus.Contains("bag") == false)
                _menuOptions.Add(Localization.GetString("game_menu_bag", "Bag"));
            if (Level.DisabledMenus.Contains("trainercard") == false)
                _menuOptions.Add("|||" + Core.Player.Name);
            if (Level.DisabledMenus.Contains("save") == false)
                _menuOptions.Add(Localization.GetString("game_menu_save", "Save"));
        }

        if (Level.DisabledMenus.Contains("options") == false)
            _menuOptions.Add(Localization.GetString("game_menu_options", "Options"));
    }

    private void DrawPrescreen()
    {
        if (Core.windowSize.Width <= 0 || Core.windowSize.Height <= 0) return;

        if (_preScreenTarget == null)
            _preScreenTarget = new RenderTarget2D(Core.GraphicsDevice, Core.windowSize.Width, Core.windowSize.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        if (_blur == null)
            _blur = new Resources.Blur.BlurHandler(Core.windowSize.Width, Core.windowSize.Height);

        if (_preScreenTexture == null || _preScreenTexture.IsContentLost == true)
        {
            Core.SpriteBatch.EndBatch();

            RenderTarget2D target = _preScreenTarget;
            Core.GraphicsDevice.SetRenderTarget(target);
            Core.GraphicsDevice.Clear(Core.BackgroundColor);

            Core.SpriteBatch.BeginBatch();
            PreScreen?.Draw();
            Core.SpriteBatch.EndBatch();

            Core.GraphicsDevice.SetRenderTarget(null);
            Core.SpriteBatch.BeginBatch();

            _preScreenTexture = target;
        }

        if (_preScreenTexture != null && _preScreenTexture.Width > 0 && _preScreenTexture.Height > 0)
        {
            Core.SpriteBatch.Draw(_blur.Perform(_preScreenTexture), Core.windowSize, Color.White);
        }
    }

    public override void Draw()
    {
        DrawPrescreen();
        DrawGradients(_gradientFade);

        if (IsCurrentScreen() == true)
        {
            if (_gradientFade == 255)
            {
                if (Core.Player.IsGameJoltSave == true)
                {
                    GameJolt.Emblem.Draw(GameJolt.API.username, Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Points, Core.GameJoltSave.Gender, Core.GameJoltSave.Emblem,
                        new Vector2(Core.windowSize.Width / 2 - 256, 30), 4, Core.GameJoltSave.DownloadedSprite);
                }

                String remainingText = String.Empty;
                if (Core.Player.ScriptDelayDisplaySteps == true)
                    remainingText += Localization.GetString("global_steps_left", "Steps left:") + " " + Core.Player.ScriptDelaySteps;
                if (Level.IsSafariZone == true)
                {
                    if (remainingText != String.Empty) remainingText += Environment.NewLine;
                    remainingText += Localization.GetString("global_safari_balls_remaining", "Safari Balls x") + " " + Core.Player.Inventory.GetItemAmount(181.ToString());
                }
                Core.SpriteBatch.DrawString(FontManager.MainFont, remainingText,
                    new Vector2(Core.windowSize.Width / 2 - (int)FontManager.MainFont.MeasureString(remainingText).X + 4, Core.windowSize.Height / 2 - 272 - (int)FontManager.MainFont.MeasureString(remainingText).Y + 4),
                    Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                Core.SpriteBatch.DrawString(FontManager.MainFont, remainingText,
                    new Vector2(Core.windowSize.Width / 2 - (int)FontManager.MainFont.MeasureString(remainingText).X, Core.windowSize.Height / 2 - 272 - (int)FontManager.MainFont.MeasureString(remainingText).Y),
                    Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

                for (int i = 0; i <= _menuOptions.Count - 1; i++)
                {
                    String text = _menuOptions[i].Replace("|||", String.Empty);

                    if (_buttonIntroFinished == true || _buttonFadeIndex > i)
                    {
                        Vector2 pos = GetButtonPosition(i);
                        Core.SpriteBatch.Draw(_texture, new Rectangle((int)pos.X, (int)pos.Y, 64, 64), new Rectangle(16, 16, 16, 16), Color.White);
                        Core.SpriteBatch.Draw(_texture, new Rectangle((int)pos.X + 64, (int)pos.Y, 64 * 4, 64), new Rectangle(32, 16, 16, 16), Color.White);
                        Core.SpriteBatch.Draw(_texture, new Rectangle((int)pos.X + 64 * 5, (int)pos.Y, 64, 64), new Rectangle(16, 16, 16, 16), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
                        Core.SpriteBatch.DrawString(FontManager.MainFont, text, new Vector2((int)pos.X + 20, (int)pos.Y + 20), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        Vector2 pos = GetButtonPosition(i);
                        Core.SpriteBatch.Draw(_texture, new Rectangle((int)pos.X, (int)pos.Y, 64, 64), new Rectangle(16, 16, 16, 16), new Color(255, 255, 255, _currentButtonFade));
                        Core.SpriteBatch.Draw(_texture, new Rectangle((int)pos.X + 64, (int)pos.Y, 64 * 4, 64), new Rectangle(32, 16, 16, 16), new Color(255, 255, 255, _currentButtonFade));
                        Core.SpriteBatch.Draw(_texture, new Rectangle((int)pos.X + 64 * 5, (int)pos.Y, 64, 64), new Rectangle(16, 16, 16, 16), new Color(255, 255, 255, _currentButtonFade), 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
                        Core.SpriteBatch.DrawString(FontManager.MainFont, text, new Vector2((int)pos.X + 20, (int)pos.Y + 20), new Color(0, 0, 0, _currentButtonFade), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        break;
                    }
                }
            }

            DrawCursor();
        }
        else
        {
            _buttonFadeIndex = 0;
            _currentButtonFade = 0;
            _buttonIntroFinished = false;
        }
    }

    private void DrawCursor()
    {
        if (_buttonIntroFinished == true || _buttonFadeIndex > _menuIndex)
        {
            Core.SpriteBatch.Draw(_texture, new Rectangle((int)_cursorPosition.X, (int)_cursorPosition.Y, 64, 64), new Rectangle(0, 0, 16, 16), Color.White);
        }
    }

    private void SetCursorPosition(int buttonIndex)
    {
        Vector2 pos = GetButtonPosition(buttonIndex);
        _cursorDestPosition = new Vector2((int)(pos.X + 180), (int)(pos.Y - 42));
    }

    public void Minimized()
    {
        SetCursorPosition(_menuIndex);
        _cursorPosition = _cursorDestPosition;
    }

    public Vector2 GetButtonPosition(int index)
    {
        float x;
        float y;

        if ((index % 2) == 0)
            x = Core.windowSize.Width / 2.0f - 384 - 75;
        else
            x = Core.windowSize.Width / 2.0f + 75;

        switch ((int)Math.Floor(index / 2.0))
        {
            case 0: y = Core.windowSize.Height / 2.0f - 64 - 80 - 32; break;
            case 1: y = Core.windowSize.Height / 2.0f - 32; break;
            case 2: y = Core.windowSize.Height / 2.0f + 32 + 80; break;
            default: y = Core.windowSize.Height / 2.0f - 32; break;
        }

        return new Vector2(x, y);
    }

    public override void Update()
    {
        if (_gradientFade < 255)
        {
            _gradientFade += 25;
            if (_gradientFade >= 255) _gradientFade = 255;
        }
        else
        {
            if (_buttonIntroFinished == false)
            {
                _currentButtonFade += 45;
                if (_currentButtonFade >= 255)
                {
                    _currentButtonFade = 0;
                    _buttonFadeIndex += 1;
                    if (_buttonFadeIndex > _menuOptions.Count - 1)
                        _buttonIntroFinished = true;
                }
            }
        }

        if (_buttonIntroFinished == true)
        {
            Player.Temp.MenuIndex = _menuIndex;
            int preMenuIndex = _menuIndex;

            if (_cursorDestPosition.X != _cursorPosition.X || _cursorDestPosition.Y != _cursorPosition.Y)
            {
                _cursorPosition.X = (int)MathHelper.Lerp(_cursorDestPosition.X, _cursorPosition.X, 0.75f);
                _cursorPosition.Y = (int)MathHelper.Lerp(_cursorDestPosition.Y, _cursorPosition.Y, 0.75f);
                if (Math.Abs(_cursorDestPosition.X - _cursorPosition.X) < 0.1f) _cursorPosition.X = _cursorDestPosition.X;
                if (Math.Abs(_cursorDestPosition.Y - _cursorPosition.Y) < 0.1f) _cursorPosition.Y = _cursorDestPosition.Y;
            }

            if (Math.Abs(_cursorDestPosition.Y - _cursorPosition.Y) < 5.0f)
            {
                if (Controls.Accept(true, false, false) == true)
                {
                    for (int i = 0; i <= _menuOptions.Count - 1; i++)
                    {
                        Vector2 pos = GetButtonPosition(i);
                        if (new Rectangle((int)pos.X, (int)pos.Y, 64 * 6, 64).Contains(MouseHandler.MousePosition) == true)
                        {
                            if (_menuIndex == i)
                            {
                                _cursorPosition.X = _cursorDestPosition.X;
                                SoundManager.PlaySound("select");
                                PressButton();
                            }
                            else
                            {
                                _menuIndex = i;
                                SetCursorPosition(_menuIndex);
                                preMenuIndex = _menuIndex;
                            }
                            break;
                        }
                    }
                }
                if (Controls.Accept(false, true, true) == true)
                {
                    _cursorPosition.X = _cursorDestPosition.X;
                    SoundManager.PlaySound("select");
                    PressButton();
                }
            }

            if (Controls.Up(true, true, false, true, true, true) == true)
                if (_menuIndex > 1) _menuIndex -= 2;
            if (Controls.Down(true, true, false, true, true, true) == true)
                if (_menuIndex < _menuOptions.Count - 2) _menuIndex += 2;
            if (Controls.Right(true, true, true, true, true, true) == true)
                if (_menuIndex < _menuOptions.Count - 1) _menuIndex += 1;
            if (Controls.Left(true, true, true, true, true, true) == true)
                if (_menuIndex > 0) _menuIndex -= 1;

            if (_menuIndex != preMenuIndex)
                SetCursorPosition(_menuIndex);
        }

        if (Core.CurrentScreen?.Identification == Identifications.MenuScreen)
        {
            if (Controls.Dismiss() == true)
                Core.SetScreen(PreScreen!);
        }
    }

    private void PressButton()
    {
        String opt = _menuOptions[_menuIndex];
        if (opt == Localization.GetString("game_menu_pokedex", "Pokédex"))
            Core.SetScreen(new TransitionScreen(Core.CurrentScreen!, new PokedexSelectScreen(this), Color.White, false));
        else if (opt == Localization.GetString("game_menu_party", "Pokémon"))
            Core.SetScreen(new PartyScreen(this));
        else if (opt == Localization.GetString("game_menu_bag", "Bag"))
            Core.SetScreen(new NewInventoryScreen(this));
        else if (opt == "|||" + Core.Player.Name)
            Core.SetScreen(new NewTrainerScreen(this));
        else if (opt == Localization.GetString("game_menu_save", "Save"))
        {
            if (bool.Parse(GameModeManager.GetGameRuleValue("SavingDisabled", "0")) == true && Core.Player.SandBoxMode == false)
            {
                Screen s = Core.CurrentScreen!;
                while (s.PreScreen != null && s.Identification != Identifications.OverworldScreen)
                    s = s.PreScreen;
                Core.SetScreen(s);
                TextBox.Show(Localization.GetString("game_menu_save_notpossible", "Saving is not possible right now."));
            }
            else
            {
                Core.SetScreen(new SaveScreen(this));
            }
        }
        else if (opt == Localization.GetString("game_menu_options", "Options"))
            Core.SetScreen(new NewOptionScreen(this, 0));
        else if (opt == Localization.GetString("game_menu_exit", "Exit"))
            Core.SetScreen(PreScreen!);
        else if (opt == Level.BugCatchingContestData.GetSplit(2) + " x" + Core.Player.Inventory.GetItemAmount(177.ToString()))
            ShowBalls();
        else if (opt == Localization.GetString("game_menu_end_contest", "End Contest"))
            EndContest();
    }

    private void ShowBalls()
    {
        Screen s = PreScreen!;
        ((OverworldScreen)s).ActionScript.StartScript(Level.BugCatchingContestData.GetSplit(1), 0);
        Core.SetScreen(s);
    }

    private void EndContest()
    {
        Screen s = PreScreen!;
        ((OverworldScreen)s).ActionScript.StartScript(Level.BugCatchingContestData.GetSplit(0), 0);
        Core.SetScreen(s);
    }

    public override void SizeChanged()
    {
        SetCursorPosition(_menuIndex);
        _cursorPosition = _cursorDestPosition;
    }
}
