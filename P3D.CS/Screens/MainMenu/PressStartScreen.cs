using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class PressStartScreen : Screen
{
    public float _fadeInMain = 1.0f;
    private float _introDelay = 4.0f;
    private float _logoFade = 1.0f;

    private Texture2D? _logoTexture;
    private Texture2D? _shineTexture;

    private SpriteBatch? _logoRenderer;
    private SpriteBatch? _shineRenderer;
    private SpriteBatch? _backgroundRenderer;

    private RenderTarget2D? _target;
    private Resources.Blur.BlurHandler? _blurHandler;

    private List<Screens.MainMenu.MainMenuEntity> _entities = [];
    private Screens.MainMenu.Scene.MainMenuCamera? _camera;
    private Color _fromColor;
    private Color _toColor;
    private Color _textColor;

    private float _tempF;
    private float _tempG;

    public PressStartScreen()
    {
        Identification = Identifications.PressStartScreen;
        CanBePaused = false;
        MouseVisible = true;
        CanChat = false;

        TextBox.Showing = false;
        PokemonImageView.Showing = false;
        ImageView.Showing = false;
        ChooseBox.Showing = false;
        Level = new Level();

        GameModeManager.SetGameModePointer("Kolben");

        Core.Player.Skin = "Hilbert";

        if (Directory.Exists(GameController.GamePath + @"\Save\") == false)
        {
            Directory.CreateDirectory(GameController.GamePath + @"\Save\");
        }

        GameJolt.Emblem.ClearOnlineSpriteCache();

        _logoTexture = TextureManager.GetTexture(@"GUI\Logos\Pokemon_Small");
        _shineTexture = TextureManager.GetTexture(@"GUI\Logos\logo_shine");

        _logoRenderer = new SpriteBatch(Core.GraphicsDevice);
        _shineRenderer = new SpriteBatch(Core.GraphicsDevice);
        _backgroundRenderer = new SpriteBatch(Core.GraphicsDevice);

        _target = new RenderTarget2D(Core.GraphicsDevice, 1200, 680, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

        NewNewGameScreen.CharacterSelectionScreen.SelectedSkin = String.Empty;
        Core.Player.Unload();

        _camera = new Screens.MainMenu.Scene.MainMenuCamera();
        World.setDaytime = -1;
        World.DayTimes dayTime = World.GetTime();

        switch (dayTime)
        {
            case World.DayTimes.Morning:
                _fromColor = new Color(246, 170, 109);
                _toColor = new Color(248, 248, 248);
                _textColor = Color.Black;
                break;
            case World.DayTimes.Day:
                _fromColor = new Color(120, 160, 248);
                _toColor = new Color(248, 248, 248);
                _textColor = Color.Black;
                break;
            case World.DayTimes.Evening:
                _fromColor = new Color(32, 64, 168);
                _toColor = new Color(40, 80, 88);
                _textColor = Color.White;
                break;
            case World.DayTimes.Night:
                _fromColor = new Color(32, 64, 168);
                _toColor = new Color(0, 0, 0);
                _textColor = Color.White;
                break;
        }

        if (dayTime == World.DayTimes.Day || dayTime == World.DayTimes.Morning)
        {
            Screens.MainMenu.Scene.Clouds clouds = new Screens.MainMenu.Scene.Clouds();
            clouds.LoadContent();
            _entities.Add(clouds);

            Screens.MainMenu.Scene.HoOh hooh = new Screens.MainMenu.Scene.HoOh(_entities);
            hooh.LoadContent();
            _entities.Add(hooh);
        }
        else
        {
            Screens.MainMenu.Scene.Ground ground = new Screens.MainMenu.Scene.Ground();
            ground.LoadContent();
            _entities.Add(ground);

            Screens.MainMenu.Scene.Lugia lugia = new Screens.MainMenu.Scene.Lugia(_entities);
            lugia.LoadContent();
            _entities.Add(lugia);
        }
    }

    public override void Update()
    {
        for (int i = 0; i <= _entities.Count - 1; i++)
        {
            if (i < _entities.Count)
            {
                _entities[i].Update();
                if (_entities[i].ToBeRemoved == true)
                {
                    _entities[i].Dispose();
                    _entities.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        _camera?.Update();

        if (_introDelay > 0f)
        {
            _introDelay -= 0.1f;
            if (_introDelay <= 0f) _introDelay = 0f;
        }
        else
        {
            if (IsCurrentScreen() == true)
            {
                if (_fadeInMain > 0.0f)
                {
                    _fadeInMain = MathHelper.Lerp(0.0f, _fadeInMain, 0.93f);
                    if (_fadeInMain - 0.01f <= 0.0f) _fadeInMain = 0.0f;
                }
            }
            else
            {
                if (_logoFade > 0.0f)
                {
                    _logoFade = MathHelper.Lerp(0.0f, _logoFade, 0.2f);
                    if (_logoFade - 0.01f <= 0.0f) _logoFade = 0.0f;
                }
            }

            _tempF += 0.01f;
            _tempG += 0.04f;

            if (IsCurrentScreen() == true)
            {
                if (KeyBoardHandler.KeyPressed(KeyBindings.EnterKey1) == true ||
                    KeyBoardHandler.KeyPressed(KeyBindings.EnterKey2) == true ||
                    ControllerHandler.ButtonPressed(Buttons.A) == true ||
                    MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                {
                    _fadeInMain = 0.0f;
                    Core.SetScreen(new NewMainMenuScreen(this));
                }
            }
        }
    }

    public override void Draw()
    {
        if (_blurHandler == null && _target != null)
        {
            _blurHandler = new Resources.Blur.BlurHandler(_target.Width, _target.Height);
        }

        if (_target != null && _backgroundRenderer != null)
        {
            Core.GraphicsDevice.SetRenderTarget(_target);
            Core.GraphicsDevice.Clear(_fromColor);

            _backgroundRenderer.Begin();
            Canvas.DrawGradient(_backgroundRenderer,
                new Rectangle(0, _target.Height / 2, _target.Width, _target.Height / 4),
                _fromColor, _toColor, false, 10);
            Canvas.DrawRectangle(
                new Rectangle(0, _target.Height / 4 * 3, _target.Width, _target.Height / 4),
                _toColor);
            _backgroundRenderer.End();

            Core.GraphicsDevice.SetRenderTarget(null);

            _backgroundRenderer.Begin();
            if (_blurHandler != null)
            {
                Texture2D blurred = _blurHandler.Perform(_target);
                _backgroundRenderer.Draw(blurred, new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), Color.White);
            }
            _backgroundRenderer.End();
        }

        if (IsCurrentScreen() == true)
        {
            _logoRenderer?.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
        }
        else
        {
            _logoRenderer?.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
        }
        _shineRenderer?.Begin(SpriteSortMode.Texture, BlendState.Additive);

        if (_logoTexture != null)
        {
            _logoRenderer?.Draw(_logoTexture,
                new Rectangle(
                    (int)(Core.windowSize.Width / 2 - 350 * Core.SpriteBatch.InterfaceScale()),
                    (int)(160 * _fadeInMain + 64),
                    (int)(700 * Core.SpriteBatch.InterfaceScale()),
                    (int)(300 * Core.SpriteBatch.InterfaceScale())),
                new Color(255, 255, 255, (int)(255 * _logoFade)));
        }

        if (_shineTexture != null)
        {
            _shineRenderer?.Draw(_shineTexture,
                new Rectangle(
                    (int)(Core.windowSize.Width / 2 - 250 * Core.SpriteBatch.InterfaceScale() + Math.Sin(_tempF) * 240.0f),
                    (int)(-100 + Math.Sin(_tempG) * 10.0f + 160 * _fadeInMain + 64),
                    (int)(512 * Core.SpriteBatch.InterfaceScale()),
                    (int)(512 * Core.SpriteBatch.InterfaceScale())),
                new Color(255, 255, 255, (int)(255 * _logoFade)));
        }

        if (_fadeInMain == 0f)
        {
            if (IsCurrentScreen() == true && Core.GameOptions.ShowGUI == true)
            {
                String text = String.Empty;
                Vector2 textSizeUntilButton = Vector2.Zero;

                if (ControllerHandler.IsConnected() == true)
                {
                    text = Localization.GetString("start_screen_press", "Press") + "<button>" + Localization.GetString("start_screen_tostart", "to start.");
                    textSizeUntilButton = FontManager.InGameFont.MeasureString(text.GetSplit(0, "<button>"));
                    text = text.Replace("<button>", "     ");
                }
                else
                {
                    text = Localization.GetString("start_screen_press", "Press") + " " + KeyBindings.EnterKey1.ToString().ToUpper() + " " + Localization.GetString("start_screen_tostart", "to start.");
                }

                Vector2 textSize = FontManager.InGameFont.MeasureString(text);

                GetFontRenderer().DrawString(FontManager.InGameFont, text,
                    new Vector2(
                        (int)(Core.windowSize.Width / 2.0f - textSize.X / 2.0f),
                        (int)(Core.windowSize.Height - textSize.Y - 50)),
                    _textColor);

                if (ControllerHandler.IsConnected() == true && Core.GameOptions.GamePadEnabled == true)
                {
                    _logoRenderer?.Draw(TextureManager.GetTexture(@"GUI\GamePad\xboxControllerButtonA"),
                        new Rectangle(
                            (int)(Core.windowSize.Width / 2 - textSize.X / 2 + textSizeUntilButton.X + FontManager.InGameFont.MeasureString(" ").X + 2),
                            (int)(Core.windowSize.Height - textSize.Y - 58), 40, 40),
                        Color.White);
                }
            }
        }

        _logoRenderer?.End();
        _shineRenderer?.End();

        Canvas.DrawRectangle(Core.windowSize, new Color(0, 0, 0, (int)(255 * _fadeInMain)));
    }

    public override void ChangeTo()
    {
        Core.Player.Unload();
        Core.Player.Skin = "Hilbert";
        TextBox.Hide();
        TextBox.CanProceed = true;
        OverworldScreen.FadeValue = 0;

        MusicManager.Play("title", true);
    }
}
