using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2063, "Rowap")]
public class RowapBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public RowapBerry() : base(86400, "If held by a Pokémon && a special attack lands, the attacker takes damage. ", "13.2cm", "Very Soft", 1, 5)
    {

        Spicy = 10;
        Dry = 0;
        Sweet = 0;
        Bitter = 0;
        Sour = 40;

        type = (int)Element.Types.Dark;
        Power = 100;
        JuiceColor = "blue";
        JuiceGroup = 3;
    }

}
