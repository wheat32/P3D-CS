using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2043, "Coba")]
public class CobaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public CobaBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Flying-type attack.", "27.7cm", "Very Hard", 1, 5)
    {

        Spicy = 0;
        Dry = 10;
        Sweet = 0;
        Bitter = 15;
        Sour = 0;

        type = (int)Element.Types.Flying;
        Power = 80;
        JuiceColor = "blue";
        JuiceGroup = 2;
    }

}
