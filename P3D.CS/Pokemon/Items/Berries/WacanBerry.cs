using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2037, "Wacan")]
public class WacanBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public WacanBerry() : base(64800, "If held by a Pokémon, this berry will lessen the damage taken from one supereffective Electric-type attack.", "25.0cm", "Very Soft", 1, 5)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 15;
        Bitter = 0;
        Sour = 10;

        type = (int)Element.Types.Electric;
        Power = 80;
        JuiceColor = "yellow";
        JuiceGroup = 2;
    }

}
