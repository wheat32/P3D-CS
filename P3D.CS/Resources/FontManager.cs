using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public static class FontManager
{
    private static Dictionary<String, FontContainer> _fonts = [];

    public static void LoadFonts()
    {
        _fonts.Clear();

        String baseFontDir = Path.Combine(GameController.GamePath, "Content", "Fonts", "BMP");
        if (Directory.Exists(baseFontDir) == true)
        {
            foreach (String s in Directory.GetFiles(baseFontDir))
            {
                if (s.EndsWith(".xnb") == true)
                {
                    String name = Path.GetFileNameWithoutExtension(s);
                    if (_fonts.ContainsKey(name.ToLower()) == false)
                    {
                        ContentManager cm = ContentPackManager.GetContentManager("Fonts/BMP/" + name, ".xnb");
                        SpriteFont font = cm.Load<SpriteFont>("Fonts/BMP/" + name);
                        _fonts.Add(name.ToLower(), new FontContainer(name, font));
                    }
                }
            }
        }

        foreach (String c in Core.GameOptions.ContentPackNames)
        {
            String packFontDir = Path.Combine(GameController.GamePath, "ContentPacks", c, "Content", "Fonts", "BMP");
            if (Directory.Exists(packFontDir) == true)
            {
                foreach (String s in Directory.GetFiles(packFontDir))
                {
                    if (s.EndsWith(".xnb") == true)
                    {
                        String name = Path.GetFileNameWithoutExtension(s);
                        if (_fonts.ContainsKey(name.ToLower()) == false)
                        {
                            ContentManager cm = ContentPackManager.GetContentManager("Fonts/BMP/" + name, ".xnb");
                            SpriteFont font = cm.Load<SpriteFont>("Fonts/BMP/" + name);
                            _fonts.Add(name.ToLower(), new FontContainer(name, font));
                        }
                    }
                }
            }
        }

        if (GameModeManager.ActiveGameMode.DirectoryName != "Kolben" &&
            GameModeManager.ActiveGameMode.ContentPath != "\\Content\\")
        {
            String[] cParts = GameModeManager.ActiveGameMode.ContentPath
                .Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            String modeFontDir = Path.Combine([GameController.GamePath, ..cParts, "Fonts", "BMP"]);
            if (Directory.Exists(modeFontDir) == true)
            {
                foreach (String s in Directory.GetFiles(modeFontDir))
                {
                    if (s.EndsWith(".xnb") == true)
                    {
                        String name = Path.GetFileNameWithoutExtension(s);
                        if (_fonts.ContainsKey(name.ToLower()) == false)
                        {
                            ContentManager cm = ContentPackManager.GetContentManager("Fonts/BMP/" + name, ".xnb");
                            SpriteFont font = cm.Load<SpriteFont>("Fonts/BMP/" + name);
                            _fonts.Add(name.ToLower(), new FontContainer(name, font));
                        }
                    }
                }
            }
        }
    }

    public static SpriteFont? GetFont(String fontName)
    {
        if (_fonts.ContainsKey(fontName.ToLower()) == true)
            return _fonts[fontName.ToLower()].SpriteFont;
        return null;
    }

    public static FontContainer? GetFontContainer(String fontName)
    {
        if (_fonts.ContainsKey(fontName.ToLower()) == true)
            return _fonts[fontName.ToLower()];
        return null;
    }

    public static SpriteFont? MainFont       => GetFont("mainfont");
    public static SpriteFont? TextFont       => GetFont("textfont") ?? MainFont;
    public static SpriteFont? InGameFont     => GetFont("ingame") ?? MainFont;
    public static SpriteFont? MiniFont       => GetFont("minifont") ?? MainFont;
    public static SpriteFont? ChatFont       => GetFont("chatfont");
    public static SpriteFont? UnownFont      => GetFont("unown") ?? MainFont;
    public static SpriteFont? BrailleFont    => GetFont("braille") ?? MainFont;
    public static SpriteFont? VoltorbFlipFont => GetFont("voltorbflipfont") ?? MainFont;

    private static SpriteFont? _gameJoltFont;
    private static bool _hasLoadedGameJoltFont = false;

    public static SpriteFont? GameJoltFont
    {
        get
        {
            if (_hasLoadedGameJoltFont == true)
            {
                while (_gameJoltFont == null) { }
                return _gameJoltFont;
            }
            _hasLoadedGameJoltFont = true;
            _gameJoltFont = Core.Content.Load<SpriteFont>("Fonts/BMP/GameJolt");
            _gameJoltFont.DefaultCharacter = ' ';
            return _gameJoltFont;
        }
    }
}
