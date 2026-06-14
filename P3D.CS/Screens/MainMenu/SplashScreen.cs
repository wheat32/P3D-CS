using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class SplashScreen : Screen
{
    private const String LICENSE_TEXT =
        "\"MonoGame\", the MonoGame Logo, and its source code are copyrights of MonoGame Team (monogame.net).\n" +
        "Pokémon 3D is not affiliated with The Pokémon Company, Nintendo, Creatures inc. or GAME FREAK inc. \n" +
        "Please support the official release!";

    private readonly Texture2D _monoGameLogo;
    private readonly SpriteFont _licenseFont;
    private readonly Vector2 _licenseTextSize;

    private float _delay = 7.0f;
    private Thread _loadThread;
    private bool _startedLoad;
    private GameController _game;
    private String _croppedLicenseText = String.Empty;

    public SplashScreen(GameController game)
    {
        _game = game;

        CanBePaused = false;
        CanMuteAudio = true;
        CanChat = false;
        CanTakeScreenshot = true;
        CanDrawDebug = false;
        MouseVisible = false;
        CanGoFullscreen = true;

        _monoGameLogo = TextureManager.LoadDirect(@"GUI\Logos\MonoGame.png");
        _licenseFont = Core.Content.Load<SpriteFont>("Fonts/BMP/mainFont");

        _croppedLicenseText = LICENSE_TEXT.CropStringToWidth(_licenseFont, MathHelper.Max(Core.windowSize.X - 64, 800));
        _licenseTextSize = _licenseFont.MeasureString(_croppedLicenseText);

        Identification = Identifications.SplashScreen;
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(Core.windowSize, Color.Black);

        Core.SpriteBatch.Draw(_monoGameLogo, new Vector2(
            (float)(Core.windowSize.Width / 2 - _monoGameLogo.Width / 2),
            (float)(Core.windowSize.Height / 2 - _monoGameLogo.Height / 2 - 50)),
            Color.White);

        Core.SpriteBatch.DrawString(_licenseFont, _croppedLicenseText, new Vector2(
            (float)(Core.windowSize.Width / 2 - _licenseTextSize.X / 2),
            (float)(Core.windowSize.Height - _licenseTextSize.Y - 50)),
            Color.White);
    }

    public override void Update()
    {
        if (_startedLoad == false)
        {
            _startedLoad = true;
            _loadThread = new Thread(LoadContent);
            _loadThread.Start();
        }

        if (_loadThread.IsAlive == false)
        {
            if (_delay <= 0.0f || GameController.IS_DEBUG_ACTIVE == true)
            {
                Core.GraphicsManager.ApplyChanges();
                Logger.Debug("---Loading content ready---");

                if (MapPreviewScreen.MapViewMode == true)
                {
                    Core.SetScreen(new MapPreviewScreen());
                }
                else
                {
                    Core.SetScreen(new PressStartScreen());
                }
            }
        }

        _delay -= 0.1f;
    }

    private void LoadContent()
    {
        Logger.Debug("---Start loading content---");
        Core.LoadContent();
    }
}
