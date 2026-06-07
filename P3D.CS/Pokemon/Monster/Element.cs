namespace P3D;

// TODO Phase 3: full Element port
public class Element
{
    public enum Types
    {
        Blank = -1,
        Normal, Fire, Water, Electric, Grass, Ice, Fighting,
        Poison, Ground, Flying, Psychic, Bug, Rock, Ghost,
        Dragon, Dark, Steel, Fairy, Shadow
    }

    public Types Type { get; set; }
    public bool IsGameModeElement { get; set; }
    public String gmOriginalName { get; set; } = "";

    public Element(Types type)
    {
        Type = type;
    }

    /// <summary>Returns the type effectiveness multiplier (2.0, 0.5, 0.0, 1.0, etc.).</summary>
    public static float GetElementMultiplier(Element attackType, Element defenseType)
    {
        return 1.0f; // TODO Phase 5: full type chart
    }

    public static String GetElementTexturePath()
    {
        String langPath = $"GUI\\Menus\\Types_{Localization.LanguageSuffix}";
        if (File.Exists(Path.Combine(GameController.GamePath, langPath + ".png")))
        {
            return langPath;
        }
        return "GUI\\Menus\\Types_en";
    }
}
