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
    public String gmOriginalName { get; set; } = String.Empty;
    public Microsoft.Xna.Framework.Rectangle gmTypeRectangle { get; set; }

    public Element(Types type)
    {
        Type = type;
    }

    /// <summary>Returns the type effectiveness multiplier (2.0, 0.5, 0.0, 1.0, etc.).</summary>
    public static float GetElementMultiplier(Element attackType, Element defenseType)
    {
        return 1.0f; // TODO Phase 5: full type chart
    }

    /// <summary>Returns the sprite-sheet rectangle for this element's type icon.</summary>
    public Microsoft.Xna.Framework.Rectangle GetElementImage()
    {
        if (IsGameModeElement == true)
        {
            return gmTypeRectangle;
        }
        switch (Type)
        {
            case Types.Normal:
                return new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 16);
            case Types.Grass:
                return new Microsoft.Xna.Framework.Rectangle(0, 16, 48, 16);
            case Types.Fire:
                return new Microsoft.Xna.Framework.Rectangle(0, 32, 48, 16);
            case Types.Water:
                return new Microsoft.Xna.Framework.Rectangle(0, 48, 48, 16);
            case Types.Electric:
                return new Microsoft.Xna.Framework.Rectangle(0, 64, 48, 16);
            case Types.Ground:
                return new Microsoft.Xna.Framework.Rectangle(0, 80, 48, 16);
            case Types.Rock:
                return new Microsoft.Xna.Framework.Rectangle(0, 96, 48, 16);
            case Types.Ice:
                return new Microsoft.Xna.Framework.Rectangle(0, 112, 48, 16);
            case Types.Steel:
                return new Microsoft.Xna.Framework.Rectangle(0, 128, 48, 16);
            case Types.Bug:
                return new Microsoft.Xna.Framework.Rectangle(48, 0, 48, 16);
            case Types.Fighting:
                return new Microsoft.Xna.Framework.Rectangle(48, 16, 48, 16);
            case Types.Flying:
                return new Microsoft.Xna.Framework.Rectangle(48, 32, 48, 16);
            case Types.Poison:
                return new Microsoft.Xna.Framework.Rectangle(48, 48, 48, 16);
            case Types.Ghost:
                return new Microsoft.Xna.Framework.Rectangle(48, 64, 48, 16);
            case Types.Dark:
                return new Microsoft.Xna.Framework.Rectangle(48, 80, 48, 16);
            case Types.Psychic:
                return new Microsoft.Xna.Framework.Rectangle(48, 96, 48, 16);
            case Types.Dragon:
                return new Microsoft.Xna.Framework.Rectangle(48, 128, 48, 16);
            case Types.Fairy:
                return new Microsoft.Xna.Framework.Rectangle(96, 48, 48, 16);
            case Types.Shadow:
                return new Microsoft.Xna.Framework.Rectangle(96, 64, 48, 16);
            default:
                return new Microsoft.Xna.Framework.Rectangle(48, 112, 48, 16);
        }
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
