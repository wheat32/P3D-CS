using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.UI.GameControls;

/// <summary>Base class for all UI controls.</summary>
public abstract class Control
{
    protected const String TESTFORHEIGHTCHARS =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz;:_-.,*~+'#1234567890?ß\\/!\"§$%&/()}][{";

    private const int DEFAULT_WIDTH = 120;
    private const int DEFAULT_POSITION_X = 100;
    private const int DEFAULT_POSITION_Y = 100;

    private String _text = String.Empty;
    private bool _isFocused = false;
    private bool _focusOnClick = true;

    private SpriteFont? _font;
    private float _fontSize = 1.0f;
    private Color _fontColor = Color.Black;
    private Color _backColor = Color.White;
    private Color _borderColor = Color.Black;
    private int _width = DEFAULT_WIDTH;
    private int _height = -1;
    private int _borderWidth = 1;
    private Point _position = new Point(DEFAULT_POSITION_X, DEFAULT_POSITION_Y);
    private bool _visible = true;

    private Screen _createdScreenInstance;

    protected SpriteBatch FontRenderer
    {
        get
        {
            if (_createdScreenInstance.Equals(Core.CurrentScreen))
            {
                return Core.FontRenderer;
            }
            return Core.SpriteBatch;
        }
    }

    protected Control(Screen createdScreenInstance)
    {
        _createdScreenInstance = createdScreenInstance;
    }

    public String Text { get; set; } = String.Empty;

    public bool IsFocused
    {
        get => _isFocused;
        set
        {
            if (value)
            {
                OnFocused(EventArgs.Empty);
            }
            else
            {
                OnDeFocused(EventArgs.Empty);
            }
        }
    }

    public SpriteFont? Font { get; set; }
    public int Width { get; set; } = DEFAULT_WIDTH;
    public int Height { get; set; } = -1;
    public Point Position { get; set; } = new Point(DEFAULT_POSITION_X, DEFAULT_POSITION_Y);
    public float FontSize { get; set; } = 1.0f;
    public Color FontColor { get; set; } = Color.Black;
    public Color BorderColor { get; set; } = Color.Black;
    public Color BackColor { get; set; } = Color.White;
    public int BorderWidth { get; set; } = 1;
    public bool FocusOnClick { get; set; } = true;
    public bool Visible { get; set; } = true;

    public void Draw()
    {
        if (Visible == true)
        {
            DrawClient();
        }
    }

    protected virtual void DrawClient() { }

    public void Update()
    {
        if (Controls.Accept(true, false, false) == true)
        {
            if (MouseInClientArea())
            {
                if (FocusOnClick)
                {
                    IsFocused = true;
                }
                if (IsFocused)
                {
                    OnClick(new OnClickEventArgs(ActivationMethod.Click));
                }
            }
        }
        if (IsFocused)
        {
            if (Controls.Accept(false, true, false))
            {
                OnClick(new OnClickEventArgs(ActivationMethod.Keyboard));
            }
            else if (Controls.Accept(false, false, true))
            {
                OnClick(new OnClickEventArgs(ActivationMethod.Controller));
            }
        }

        UpdateClient();
    }

    protected virtual void UpdateClient() { }

    protected virtual Rectangle GetClientRectangle()
    {
        return new Rectangle(
            Position.X,
            Position.Y,
            Width + (BorderWidth * 2),
            Height + (BorderWidth * 2));
    }

    protected bool MouseInClientArea()
    {
        return GetClientRectangle().Contains(MouseHandler.MousePosition);
    }

    // Events

    public event EventHandler? Focused;
    public event EventHandler? DeFocused;
    public event EventHandler? Click;

    protected void OnFocused(EventArgs e)
    {
        _isFocused = true;
        Focused?.Invoke(this, e);
    }

    protected void OnDeFocused(EventArgs e)
    {
        _isFocused = false;
        DeFocused?.Invoke(this, e);
    }

    protected void OnClick(OnClickEventArgs e)
    {
        Click?.Invoke(this, e);
    }
}

public enum ActivationMethod
{
    Click,
    Keyboard,
    Controller
}

public class OnClickEventArgs : EventArgs
{
    private readonly ActivationMethod _method;

    public OnClickEventArgs(ActivationMethod method)
    {
        _method = method;
    }

    public ActivationMethod ActivationMethod => _method;
}
