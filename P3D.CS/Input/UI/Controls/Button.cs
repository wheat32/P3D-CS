using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.UI.GameControls;

public class Button : Control
{
    private const int DEFAULT_WIDTH = 160;
    private const int DEFAULT_HEIGHT = 40;
    private const int DEFAULT_BORDER_WIDTH = 0;
    private const float IMAGE_X_OFFSET = 12f;
    private const float IMAGE_X_PADDING = 4f;

    private bool _setSelectedBackColor = false;
    private bool _setSelectedFontColor = false;
    private Color _selectedBackColor;
    private Color _selectedFontColor;

    public Color SelectedBackColor
    {
        get => _selectedBackColor;
        set
        {
            _selectedBackColor = value;
            _setSelectedBackColor = true;
        }
    }

    public Color SelectedFontColor
    {
        get => _selectedFontColor;
        set
        {
            _selectedFontColor = value;
            _setSelectedFontColor = true;
        }
    }

    public Texture2D? Image { get; set; }

    public Button(Screen screenInstance, SpriteFont font) : base(screenInstance)
    {
        Font = font;
        BorderWidth = DEFAULT_BORDER_WIDTH;
        Width = DEFAULT_WIDTH;
        Height = DEFAULT_HEIGHT;
    }

    protected override void DrawClient()
    {
        if (Visible == false) return;
        if (BorderWidth > 0)
        {
            Core.SpriteBatch.DrawRectangle(new Rectangle(Position.X, Position.Y,
                Width + (BorderWidth * 2), Height + (BorderWidth * 2)), BorderColor);
        }

        Color foreColor = FontColor;
        Color contentColor = BackColor;

        if (IsFocused == true || MouseInClientArea())
        {
            if (_setSelectedBackColor)
            {
                contentColor = _selectedBackColor;
            }
            if (_setSelectedFontColor)
            {
                foreColor = _selectedFontColor;
            }
        }

        Core.SpriteBatch.DrawRectangle(
            new Rectangle(Position.X + BorderWidth, Position.Y + BorderWidth, Width, Height),
            contentColor);

        if (Font == null) return;

        Vector2 textSize = Font.MeasureString(TESTFORHEIGHTCHARS);
        textSize.X = Font.MeasureString(Text).X;

        Vector2 textPos = new Vector2(
            Position.X + BorderWidth + (Width / 2.0f) - ((textSize.X * FontSize) / 2.0f),
            Position.Y + BorderWidth + (Height / 2.0f) - ((textSize.Y * FontSize) / 2.0f));

        if (Image != null)
        {
            textPos.X += Image.Width / 2.0f + IMAGE_X_PADDING;

            Core.SpriteBatch.Draw(Image,
                new Rectangle(
                    (int)(textPos.X - IMAGE_X_OFFSET - Image.Width),
                    (int)(Position.Y + (Height / 2.0f) - (Image.Height / 2.0f)),
                    Image.Width, Image.Height),
                new Color((byte)255, (byte)255, (byte)255, foreColor.A));
        }

        FontRenderer.DrawString(Font, Text, textPos, foreColor, 0f, Vector2.Zero, FontSize,
            SpriteEffects.None, 0f);
    }
}
