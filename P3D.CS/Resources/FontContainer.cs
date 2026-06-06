using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class FontContainer
{
    public String FontName { get; }
    public SpriteFont SpriteFont { get; }

    public FontContainer(String fontName, SpriteFont font)
    {
        FontName = fontName;
        SpriteFont = font;

        switch (fontName.ToLower())
        {
            case "braille":
            case "voltorbflipfont":
                SpriteFont.DefaultCharacter = ' ';
                break;

            default:
                SpriteFont.DefaultCharacter = '?';
                break;
        }
    }
}
