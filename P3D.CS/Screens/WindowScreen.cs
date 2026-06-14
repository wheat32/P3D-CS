using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;

namespace P3D;

public abstract class WindowScreen : Screen
{
    public static bool IsDrawingGradients = false;

    private const int STARTWINDOWSINK = -35;

    private int _gradientFade = 0;
    private int _windowSink = STARTWINDOWSINK;
    private Vector2 _windowLocation = new Vector2(0, 0);
    private bool _isCentered = false;
    private Texture2D _texture = null!;
    private int _windowElementsX = 8;
    private int _windowElementsY = 7;
    private float _textureScale = 5.0F;
    private String _title = "{WindowTitle}";
    private bool _drawingGradient = false;
    private bool _closing = false;

    protected WindowScreen(Screen preScreen, Identifications identification, String title)
    {
        PreScreen = preScreen;
        Identification = identification;
        _isCentered = true;
        _title = title;
        MouseVisible = false;
        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
    }

    protected WindowScreen(Screen preScreen, Identifications identification, String title, Vector2 windowLocation)
    {
        PreScreen = preScreen;
        Identification = identification;
        _windowLocation = windowLocation;
        _isCentered = false;
        _title = title;
        MouseVisible = false;
        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
    }

    public override void Draw()
    {
        if (_drawingGradient == true)
        {
            Canvas.DrawGradient(new Rectangle(0, 0, (int)Core.windowSize.Width, 200), new Color(42, 167, 198, _gradientFade), new Color(42, 167, 198, 0), false, -1);
            Canvas.DrawGradient(new Rectangle(0, (int)(Core.windowSize.Height - 200), (int)Core.windowSize.Width, 200), new Color(42, 167, 198, 0), new Color(42, 167, 198, _gradientFade), false, -1);
        }

        DrawWindow();
    }

    private void DrawWindow()
    {
        if (_gradientFade != 255) return;

        Vector2 startPosition = _windowLocation;
        if (_isCentered == true)
        {
            int windowWidth = (int)(GetWindowElementSize() * _windowElementsX);
            int windowHeight = (int)(GetWindowElementSize() * _windowElementsY);
            startPosition = new Vector2((float)(Core.windowSize.Width / 2 - windowWidth / 2), (float)(Core.windowSize.Height / 2 - windowHeight / 2));
        }

        for (int x = 0; x <= _windowElementsX - 1; x++)
        {
            for (int y = 0; y <= _windowElementsY - 1; y++)
            {
                Vector2 r = new Vector2(0, 64);
                SpriteEffects e = SpriteEffects.None;

                if (x == 0 && y == 0) { r = new Vector2(0, 64); }
                else if (x == 1 && y == 0) { r = new Vector2(16, 64); }
                else if (x > 1 && x < _windowElementsX - 2 && y == 0) { r = new Vector2(32, 64); }
                else if (x == _windowElementsX - 2 && y == 0) { r = new Vector2(16, 64); e = SpriteEffects.FlipHorizontally; }
                else if (x == _windowElementsX - 1 && y == 0) { r = new Vector2(0, 64); e = SpriteEffects.FlipHorizontally; }
                else if (x == 0 && y == 1) { r = new Vector2(0, 80); }
                else if (x == _windowElementsX - 1 && y == 1) { r = new Vector2(32, 80); }
                else if (x == 0 && y == _windowElementsY - 1) { r = new Vector2(0, 112); }
                else if (x == _windowElementsX - 1 && y == _windowElementsY - 1) { r = new Vector2(32, 112); }
                else if (x > 0 && x < _windowElementsX - 1 && y == 1) { r = new Vector2(16, 80); }
                else if (x == 0 && y > 1 && y < _windowElementsY - 1) { r = new Vector2(0, 96); }
                else if (x == _windowElementsX - 1 && y > 1 && y < _windowElementsY - 1) { r = new Vector2(32, 96); }
                else if (x > 0 && x < _windowElementsX - 1 && y == _windowElementsY - 1) { r = new Vector2(16, 112); }
                else { r = new Vector2(16, 96); }

                Core.SpriteBatch.Draw(_texture, new Rectangle((int)(startPosition.X + x * GetWindowElementSize()), (int)(startPosition.Y + y * GetWindowElementSize()) + _windowSink, GetWindowElementSize(), GetWindowElementSize()), new Rectangle((int)r.X, (int)r.Y, 16, 16), Color.White, 0.0F, Vector2.Zero, e, 0.0F);
            }
        }

        int titleStartX = (int)(32 * _textureScale + startPosition.X);
        int titleStartY = (int)startPosition.Y;
        int titleAreaWidth = (int)((_windowElementsX - 4) * GetWindowElementSize());
        int titleAreaHeight = GetWindowElementSize();
        Vector2 fontSize = FontManager.MainFont.MeasureString(_title);

        Core.SpriteBatch.DrawString(FontManager.MainFont, _title, new Vector2((float)(titleStartX + titleAreaWidth / 2 - fontSize.X / 2), (float)(titleStartY + titleAreaHeight / 2 - fontSize.Y / 2) + _textureScale + _windowSink), Color.White);
    }

    private int GetWindowElementSize() => (int)(16 * _textureScale);

    public override void Update()
    {
        if (_drawingGradient == false && IsDrawingGradients == false)
        {
            _drawingGradient = true;
            IsDrawingGradients = true;
        }

        if (_closing == true)
        {
            if (_windowSink > STARTWINDOWSINK)
            {
                _windowSink -= 5;
                if (_windowSink < STARTWINDOWSINK)
                    _windowSink = STARTWINDOWSINK;
            }
            else
            {
                if (_drawingGradient == false)
                    _gradientFade = 0;
                else
                    _gradientFade -= 25;

                if (_gradientFade <= 0)
                {
                    _gradientFade = 0;
                    Core.SetScreen(PreScreen!);
                }
            }
        }
        else
        {
            if (_gradientFade < 255)
            {
                if (_drawingGradient == false)
                    _gradientFade = 255;
                else
                    _gradientFade += 25;

                if (_gradientFade >= 255)
                    _gradientFade = 255;
            }
            else
            {
                if (_windowSink < 0)
                {
                    if (_windowSink < -20) _windowSink += 4;
                    else if (_windowSink < -10) _windowSink += 3;
                    else _windowSink += 2;

                    if (_windowSink >= 0)
                    {
                        _windowSink = 0;
                        MouseVisible = true;
                    }
                }
            }
        }
    }

    public bool FadedIn => (_gradientFade == 255);

    public bool PlayerCanInteract => (_gradientFade == 255 && _windowSink == 0 && _closing == false);

    public void CloseScreen()
    {
        _closing = true;
    }

    protected Vector2 GetPositionInWindowTopLeft(float x, float y)
    {
        Vector2 startPosition = _windowLocation;
        if (_isCentered == true)
        {
            int windowWidth = (int)(GetWindowElementSize() * _windowElementsX);
            int windowHeight = (int)(GetWindowElementSize() * _windowElementsY);
            startPosition = new Vector2((float)(Core.windowSize.Width / 2 - windowWidth / 2), (float)(Core.windowSize.Height / 2 - windowHeight / 2));
        }
        return new Vector2(startPosition.X + x, startPosition.Y + y);
    }

    protected Vector2 OffsetVector(Vector2 p) => new Vector2(p.X, p.Y + _windowSink);

    protected Rectangle OffsetRectangle(Rectangle r) => new Rectangle(r.X, r.Y + _windowSink, r.Width, r.Height);

    public override void ChangeFrom()
    {
        base.ChangeFrom();
        if (_drawingGradient == true)
            IsDrawingGradients = false;
    }
}
