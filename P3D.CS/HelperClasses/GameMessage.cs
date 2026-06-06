using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

/// <summary>Displays a timed overlay message during gameplay.</summary>
public class GameMessage : BasicObject
{
    public enum DockStyles
    {
        Top,
        Down,
        Left,
        Right,
        None
    }

    private const float DURATION_STEP = 0.1f;
    private const int ALPHA_FADE_STEP = 5;
    private static readonly (int Min, int Max) ALPHA_RANGE = (0, 255);

    private int _alpha;

    public float Duration { get; set; }
    public Color BackgroundColor { get; set; } = Color.White;
    public Rectangle TextureRectangle { get; set; } = new Rectangle(0, 0, 0, 0);
    public bool Fullscreen { get; set; }
    public DockStyles Dock { get; set; } = DockStyles.Top;
    public String Text { get; set; } = "";
    public Vector2 TextPosition { get; set; } = Vector2.Zero;
    public SpriteFont? SpriteFont { get; set; }
    public Color TextColor { get; set; } = Color.White;
    public bool ShowAlways { get; set; }
    public bool AlphaBlend { get; set; } = true;

    public GameMessage(Texture2D? texture, int width, int height, Vector2 position)
        : this(texture, new Size(width, height), position)
    {
    }

    public GameMessage(Texture2D? texture, Size size, Vector2 position)
        : base(texture, size.Width, size.Height, position)
    {
        Size = size;
        Visible = false;
    }

    public void SetupText(String text, SpriteFont? spriteFont, Color textColor)
    {
        Text = text;
        SpriteFont = spriteFont;
        TextColor = textColor;
    }

    public void Update()
    {
        if (ShowAlways == true)
        {
            return;
        }

        if (Duration > 0f)
        {
            Visible = true;
            Duration -= DURATION_STEP;
            if (Duration < 0f)
            {
                Duration = 0f;
            }

            if (AlphaBlend == true)
            {
                if (_alpha < ALPHA_RANGE.Max)
                {
                    _alpha += ALPHA_FADE_STEP;
                    if (_alpha > ALPHA_RANGE.Max)
                    {
                        _alpha = ALPHA_RANGE.Max;
                    }
                }
            }
            else
            {
                _alpha = ALPHA_RANGE.Max;
            }
        }
        else
        {
            if (AlphaBlend == true)
            {
                if (_alpha > ALPHA_RANGE.Min)
                {
                    _alpha -= ALPHA_FADE_STEP;
                    if (_alpha <= ALPHA_RANGE.Min)
                    {
                        _alpha = ALPHA_RANGE.Min;
                        Visible = false;
                    }
                }
            }
            else
            {
                Visible = false;
            }
        }
    }

    public void Draw()
    {
        if (Visible == false)
        {
            return;
        }

        if (Fullscreen == true)
        {
            DrawMe(
                new Size(Core.GraphicsDevice.Viewport.Width, Core.GraphicsDevice.Viewport.Height),
                Vector2.Zero);
        }
        else
        {
            switch (Dock)
            {
                case DockStyles.None:
                    DrawMe(Size, Position);
                    break;

                case DockStyles.Top:
                    DrawMe(new Size(Core.ScreenSize.Width, Size.Height), Vector2.Zero);
                    break;

                case DockStyles.Down:
                    DrawMe(new Size(Core.ScreenSize.Width, Size.Height),
                        new Vector2(0, Core.ScreenSize.Height - Size.Height));
                    break;

                case DockStyles.Left:
                    DrawMe(new Size(Size.Width, Core.ScreenSize.Height), Vector2.Zero);
                    break;

                case DockStyles.Right:
                    DrawMe(new Size(Size.Width, Core.ScreenSize.Height),
                        new Vector2(Core.ScreenSize.Width - Size.Width, 0));
                    break;
            }
        }
    }

    private void DrawMe(Size drawSize, Vector2 drawPosition)
    {
        Color bg = new Color(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, _alpha);
        Color fg = new Color(TextColor.R, TextColor.G, TextColor.B, _alpha);

        Core.SpriteBatch.DrawInterface(Texture!,
            new Rectangle((int)drawPosition.X, (int)drawPosition.Y, drawSize.Width, drawSize.Height),
            TextureRectangle, bg);

        if (SpriteFont != null)
        {
            Core.SpriteBatch.DrawInterfaceString(SpriteFont, Text,
                LinkVector2(Position, TextPosition, 1f), fg);
        }
    }

    private static Vector2 LinkVector2(Vector2 v1, Vector2 v2, float factor)
    {
        return new Vector2(v1.X + factor * v2.X, v1.Y + factor * v2.Y);
    }

    public void ShowMessage(float duration, GraphicsDevice graphics)
    {
        Duration = duration;
        Visible = true;
        _alpha = ALPHA_RANGE.Min;
    }

    public void ShowMessage(String text, float duration, SpriteFont? spriteFont, Color textColor)
    {
        Text = text;
        SpriteFont = spriteFont;
        TextColor = textColor;
        Duration = duration;
        Visible = true;
        _alpha = ALPHA_RANGE.Min;
    }

    public void HideMessage()
    {
        Visible = false;
        Duration = 0f;
        _alpha = ALPHA_RANGE.Min;
    }
}
