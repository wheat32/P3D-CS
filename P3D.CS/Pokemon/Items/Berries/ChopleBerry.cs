using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2040, "Chople")]
public class ChopleBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public ChopleBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Fighting-type attack.", "7.7cm", "Soft", 1, 5)
    {

        Spicy = 15;
        Dry = 0;
        Sweet = 0;
        Bitter = 10;
        Sour = 0;

        type = (int)Element.Types.Fighting;
        Power = 80;
        JuiceColor = "red";
        JuiceGroup = 2;
    }

}
