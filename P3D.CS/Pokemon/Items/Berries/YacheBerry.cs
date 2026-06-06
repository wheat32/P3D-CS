using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2039, "Yache")]
public class YacheBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public YacheBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Ice-type attack.", "13.5cm", "Very Hard", 1, 5)
    {

        Spicy = 0;
        Dry = 10;
        Sweet = 0;
        Bitter = 15;
        Sour = 0;

        type = (int)Element.Types.Ice;
        Power = 80;
        JuiceColor = "blue";
        JuiceGroup = 2;
    }

}
