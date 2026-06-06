using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2046, "Charti")]
public class ChartiBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public ChartiBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Rock-type attack.", "2.8cm", "Very Soft", 1, 5)
    {

        Spicy = 10;
        Dry = 20;
        Sweet = 0;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Rock;
        Power = 80;
        JuiceColor = "yellow";
        JuiceGroup = 2;
    }

}
