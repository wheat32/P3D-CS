using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2058, "Starf")]
public class StarfBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public StarfBerry() : base(86400, "A Berry to be consumed by Pokémon. If a Pokémon holds one, one of its stats will sharply increase when it's in a pinch.", "15.2cm", "Super Hard", 1, 2)
    {

        Spicy = 30;
        Dry = 10;
        Sweet = 30;
        Bitter = 10;
        Sour = 30;

        type = (int)Element.Types.Psychic;
        Power = 100;
        JuiceColor = "green";
        JuiceGroup = 3;
    }

}
