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

        char preferred = fontName.ToLower() switch
        {
            "braille" or "voltorbflipfont" => ' ',
            _ => '?'
        };
        if (SpriteFont.Characters.Contains(preferred))
        {
            SpriteFont.DefaultCharacter = preferred;
        }
        else if (SpriteFont.Characters.Contains(' '))
        {
            SpriteFont.DefaultCharacter = ' ';
        }
    }
}
