using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

/// <summary>Displays a message to the player as an overlay screen.</summary>
public class MessageBox : Screen
{
    private const float SCALE = 2f;
    private const int DEFAULT_WIDTH = 320;
    private const int DEFAULT_HEIGHT = 320;
    private const int TEXT_WIDTH_THRESHOLD = 196;
    private const int TEXT_PADDING = 24;
    private const int OVERLAY_ALPHA = 140;
    private const int ALPHA_MAX = 255;
    private const float FADE_OUT_LERP = 0.7f;
    private const float FADE_IN_LERP = 0.95f;
    private const float FADE_EPSILON = 0.01f;
    private const float FADE_DISMISS_THRESHOLD = 0.75f;
    private const int BOX_X_INSET_DIVISOR = 10;
    private const int BOX_HEIGHT_DIVISOR = 5;

    private float _fadeIn = 0f;
    private bool _closing = false;

    private String _text = String.Empty;
    private int _width = DEFAULT_WIDTH;
    private int _height = DEFAULT_HEIGHT;
    private Color _backColor = Color.Black;
    private Color _textColor = Color.White;
    private Color _shadowColor = Color.Black;

    public MessageBox(Screen currentScreen)
    {
        PreScreen = currentScreen;
        Identification = Identifications.MessageBoxScreen;

        CanBePaused = false;
        CanChat = false;
        CanDrawDebug = true;
        CanGoFullscreen = true;
        CanMuteAudio = true;
        CanTakeScreenshot = true;
        MouseVisible = true;
    }

    public void Show(String text, Color backColor = default, Color textColor = default,
                     Color shadowColor = default)
    {
        _fadeIn = 0f;
        _text = text;
        _closing = false;

        if (backColor != default)
        {
            _backColor = backColor;
        }
        if (textColor != default)
        {
            _textColor = textColor;
        }
        if (shadowColor != default)
        {
            _shadowColor = shadowColor;
        }

        if (FontManager.MainFont != null)
        {
            Vector2 fontSize = FontManager.MainFont.MeasureString(_text) * SCALE;
            if (fontSize.X > TEXT_WIDTH_THRESHOLD)
            {
                _width = (int)(fontSize.X + TEXT_PADDING);
            }
        }

        Core.SetScreen(this);
    }

    public override void Draw()
    {
        PreScreen?.Draw();

        if (FontManager.MainFont == null) return;

        Vector2 fontSize = new Vector2(
            (int)(FontManager.MainFont.MeasureString(_text).X * SCALE * Core.SpriteBatch.InterfaceScale()),
            (int)(FontManager.MainFont.MeasureString(_text).Y * SCALE * Core.SpriteBatch.InterfaceScale()));

        Canvas.DrawRectangle(Core.windowSize, new Color(0, 0, 0, (int)(OVERLAY_ALPHA * _fadeIn)));

        Rectangle boxRect = new Rectangle(
            (int)(Core.ScreenSize.Width / 2 - _width / 2 - (_width / BOX_X_INSET_DIVISOR)),
            (int)(Core.ScreenSize.Height / 2 - _height / 2 - (_height / BOX_HEIGHT_DIVISOR) * (1 - _fadeIn)),
            (int)(_width + (_width / BOX_HEIGHT_DIVISOR)),
            (int)(_height + fontSize.Y));

        Canvas.DrawRectangle(boxRect,
            new Color(_backColor.R, _backColor.G, _backColor.B, (int)(ALPHA_MAX * _fadeIn)),
            true);

        Vector2 shadowPos = new Vector2(
            (int)(Core.windowSize.Width / 2.0f - fontSize.X / 2.0f + 2 * SCALE),
            (int)(boxRect.Y + (boxRect.Height / 2) - fontSize.Y / 2.0f + 2 * SCALE));
        Vector2 textPos = new Vector2(
            (int)(Core.windowSize.Width / 2.0f - fontSize.X / 2.0f),
            (int)(boxRect.Y + (boxRect.Height / 2) - fontSize.Y / 2.0f));

        Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, _text, shadowPos,
            new Color(_shadowColor.R, _shadowColor.G, _shadowColor.B, (int)(ALPHA_MAX * _fadeIn)),
            0f, Vector2.Zero, SCALE, SpriteEffects.None, 0f);
        Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, _text, textPos,
            new Color(_textColor.R, _textColor.G, _textColor.B, (int)(ALPHA_MAX * _fadeIn)),
            0f, Vector2.Zero, SCALE, SpriteEffects.None, 0f);
    }

    public override void Update()
    {
        if (_closing)
        {
            if (_fadeIn > 0.0f)
            {
                _fadeIn = MathHelper.Lerp(0.0f, _fadeIn, FADE_OUT_LERP);
                if (_fadeIn - FADE_EPSILON <= 0.0f)
                {
                    _fadeIn = 0.0f;
                    Core.SetScreen(PreScreen!);
                }
            }
        }
        else
        {
            if (_fadeIn < 1.0f)
            {
                _fadeIn = MathHelper.Lerp(1.0f, _fadeIn, FADE_IN_LERP);
                if (_fadeIn + FADE_EPSILON >= 1.0f)
                {
                    _fadeIn = 1.0f;
                }
            }
            if (_fadeIn > FADE_DISMISS_THRESHOLD)
            {
                if (Controls.Dismiss(true, true, true) || Controls.Accept(true, true, true))
                {
                    _closing = true;
                }
            }
        }
    }
}
