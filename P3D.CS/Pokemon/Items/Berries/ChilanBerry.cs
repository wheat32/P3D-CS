using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2051, "Chilan")]
public class ChilanBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public ChilanBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one Normal-type attack.", "3.3cm", "Very Soft", 1, 5)
    {

        Spicy = 0;
        Dry = 25;
        Sweet = 10;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Normal;
        Power = 80;
        JuiceColor = "yellow";
        JuiceGroup = 2;
    }

}
