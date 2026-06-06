using Microsoft.Xna.Framework.Graphics;

namespace P3D;

// TODO Phase 7: full FontManager port
public static class FontManager
{
    private static Dictionary<String, FontContainer> _fonts = [];

    public static SpriteFont? MainFont => GetFontContainer("mainfont")?.SpriteFont;
    public static SpriteFont? ChatFont => GetFontContainer("chatfont")?.SpriteFont;

    public static void LoadFonts() { }

    public static FontContainer? GetFontContainer(String name)
    {
        _fonts.TryGetValue(name.ToLower(), out FontContainer? fc);
        return fc;
    }
}
