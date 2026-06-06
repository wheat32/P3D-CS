using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2066, "Maranga")]
public class MarangaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public MarangaBerry() : base(86400, "If held by a Pokémon, this Berry will increase the holder's Sp. Def. if it's hit with a special move.", "18.6cm", "Hard", 1, 5)
    {

        Spicy = 10;
        Dry = 0;
        Sweet = 40;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Dark;
        Power = 100;
        JuiceColor = "blue";
        JuiceGroup = 3;
    }

}
