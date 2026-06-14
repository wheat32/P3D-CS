namespace P3D;

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
        if (attackType.IsGameModeElement || defenseType.IsGameModeElement)
        {
            return 1.0f;
        }

        return (attackType.Type, defenseType.Type) switch
        {
            // Normal
            (Types.Normal, Types.Rock) or (Types.Normal, Types.Steel) => 0.5f,
            (Types.Normal, Types.Ghost) => 0.0f,
            // Fire
            (Types.Fire, Types.Fire) or (Types.Fire, Types.Water) or
            (Types.Fire, Types.Rock) or (Types.Fire, Types.Dragon) => 0.5f,
            (Types.Fire, Types.Grass) or (Types.Fire, Types.Ice) or
            (Types.Fire, Types.Bug) or (Types.Fire, Types.Steel) => 2.0f,
            // Water
            (Types.Water, Types.Water) or (Types.Water, Types.Grass) or
            (Types.Water, Types.Dragon) => 0.5f,
            (Types.Water, Types.Fire) or (Types.Water, Types.Ground) or
            (Types.Water, Types.Rock) => 2.0f,
            // Electric
            (Types.Electric, Types.Electric) or (Types.Electric, Types.Grass) or
            (Types.Electric, Types.Dragon) => 0.5f,
            (Types.Electric, Types.Ground) => 0.0f,
            (Types.Electric, Types.Water) or (Types.Electric, Types.Flying) => 2.0f,
            // Grass
            (Types.Grass, Types.Fire) or (Types.Grass, Types.Grass) or
            (Types.Grass, Types.Poison) or (Types.Grass, Types.Flying) or
            (Types.Grass, Types.Bug) or (Types.Grass, Types.Dragon) or
            (Types.Grass, Types.Steel) => 0.5f,
            (Types.Grass, Types.Water) or (Types.Grass, Types.Ground) or
            (Types.Grass, Types.Rock) => 2.0f,
            // Ice
            (Types.Ice, Types.Water) or (Types.Ice, Types.Ice) or
            (Types.Ice, Types.Steel) => 0.5f,
            (Types.Ice, Types.Grass) or (Types.Ice, Types.Ground) or
            (Types.Ice, Types.Flying) or (Types.Ice, Types.Dragon) => 2.0f,
            // Fighting
            (Types.Fighting, Types.Poison) or (Types.Fighting, Types.Bug) or
            (Types.Fighting, Types.Psychic) or (Types.Fighting, Types.Flying) or
            (Types.Fighting, Types.Fairy) => 0.5f,
            (Types.Fighting, Types.Ghost) => 0.0f,
            (Types.Fighting, Types.Normal) or (Types.Fighting, Types.Ice) or
            (Types.Fighting, Types.Rock) or (Types.Fighting, Types.Dark) or
            (Types.Fighting, Types.Steel) => 2.0f,
            // Poison
            (Types.Poison, Types.Poison) or (Types.Poison, Types.Ground) or
            (Types.Poison, Types.Rock) or (Types.Poison, Types.Ghost) => 0.5f,
            (Types.Poison, Types.Steel) => 0.0f,
            (Types.Poison, Types.Grass) or (Types.Poison, Types.Fairy) => 2.0f,
            // Ground
            (Types.Ground, Types.Grass) or (Types.Ground, Types.Bug) => 0.5f,
            (Types.Ground, Types.Flying) => 0.0f,
            (Types.Ground, Types.Fire) or (Types.Ground, Types.Electric) or
            (Types.Ground, Types.Poison) or (Types.Ground, Types.Rock) or
            (Types.Ground, Types.Steel) => 2.0f,
            // Flying
            (Types.Flying, Types.Electric) or (Types.Flying, Types.Rock) or
            (Types.Flying, Types.Steel) => 0.5f,
            (Types.Flying, Types.Grass) or (Types.Flying, Types.Fighting) or
            (Types.Flying, Types.Bug) => 2.0f,
            // Psychic
            (Types.Psychic, Types.Psychic) or (Types.Psychic, Types.Steel) => 0.5f,
            (Types.Psychic, Types.Dark) => 0.0f,
            (Types.Psychic, Types.Fighting) or (Types.Psychic, Types.Poison) => 2.0f,
            // Bug
            (Types.Bug, Types.Fire) or (Types.Bug, Types.Fighting) or
            (Types.Bug, Types.Poison) or (Types.Bug, Types.Flying) or
            (Types.Bug, Types.Ghost) or (Types.Bug, Types.Steel) or
            (Types.Bug, Types.Fairy) => 0.5f,
            (Types.Bug, Types.Grass) or (Types.Bug, Types.Psychic) or
            (Types.Bug, Types.Dark) => 2.0f,
            // Rock
            (Types.Rock, Types.Fighting) or (Types.Rock, Types.Ground) or
            (Types.Rock, Types.Steel) => 0.5f,
            (Types.Rock, Types.Fire) or (Types.Rock, Types.Ice) or
            (Types.Rock, Types.Flying) or (Types.Rock, Types.Bug) => 2.0f,
            // Ghost
            (Types.Ghost, Types.Normal) => 0.0f,
            (Types.Ghost, Types.Dark) => 0.5f,
            (Types.Ghost, Types.Psychic) or (Types.Ghost, Types.Ghost) => 2.0f,
            // Dragon
            (Types.Dragon, Types.Steel) => 0.5f,
            (Types.Dragon, Types.Fairy) => 0.0f,
            (Types.Dragon, Types.Dragon) => 2.0f,
            // Dark
            (Types.Dark, Types.Fighting) or (Types.Dark, Types.Dark) or
            (Types.Dark, Types.Fairy) => 0.5f,
            (Types.Dark, Types.Psychic) or (Types.Dark, Types.Ghost) => 2.0f,
            // Steel
            (Types.Steel, Types.Fire) or (Types.Steel, Types.Water) or
            (Types.Steel, Types.Electric) or (Types.Steel, Types.Steel) => 0.5f,
            (Types.Steel, Types.Ice) or (Types.Steel, Types.Rock) or
            (Types.Steel, Types.Fairy) => 2.0f,
            // Fairy
            (Types.Fairy, Types.Fire) or (Types.Fairy, Types.Poison) or
            (Types.Fairy, Types.Steel) => 0.5f,
            (Types.Fairy, Types.Fighting) or (Types.Fairy, Types.Dragon) or
            (Types.Fairy, Types.Dark) => 2.0f,
            _ => 1.0f
        };
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
