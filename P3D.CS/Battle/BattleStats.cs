using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;

namespace P3D.BattleSystem;

public class BattleStats
{
    public static Texture2D? GetStatImage(Pokemon.StatusProblems status)
    {
        Rectangle r = new Rectangle(0, 0, 0, 0);

        switch (status)
        {
            case Pokemon.StatusProblems.BadPoison:
            case Pokemon.StatusProblems.Poison:
                r = new Rectangle(96, 18, 19, 6);
                break;

            case Pokemon.StatusProblems.Burn:
                r = new Rectangle(96, 0, 19, 6);
                break;

            case Pokemon.StatusProblems.Fainted:
                r = new Rectangle(96, 30, 19, 6);
                break;

            case Pokemon.StatusProblems.Freeze:
                r = new Rectangle(96, 12, 19, 6);
                break;

            case Pokemon.StatusProblems.Paralyzed:
                r = new Rectangle(96, 6, 19, 6);
                break;

            case Pokemon.StatusProblems.Sleep:
                r = new Rectangle(96, 24, 19, 6);
                break;

            case Pokemon.StatusProblems.None:
                return null;
        }

        return TextureManager.GetTexture(Element.GetElementTexturePath(), r, String.Empty);
    }

    public static Color GetStatColor(Pokemon.StatusProblems status)
    {
        switch (status)
        {
            case Pokemon.StatusProblems.BadPoison:
            case Pokemon.StatusProblems.Poison:
                return new Color(214, 49, 222);

            case Pokemon.StatusProblems.Burn:
                return new Color(231, 90, 74);

            case Pokemon.StatusProblems.Paralyzed:
                return new Color(239, 173, 0);

            case Pokemon.StatusProblems.Freeze:
                return new Color(33, 140, 247);

            case Pokemon.StatusProblems.Sleep:
                return new Color(132, 132, 132);

            case Pokemon.StatusProblems.Fainted:
                return new Color(181, 0, 0);
        }

        return Color.White;
    }
}
