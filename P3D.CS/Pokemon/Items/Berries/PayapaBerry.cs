using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2044, "Payapa")]
public class PayapaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public PayapaBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Psychic-type attack.", "25.1cm", "Soft", 1, 5)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 10;
        Bitter = 0;
        Sour = 15;

        type = (int)Element.Types.Psychic;
        Power = 80;
        JuiceColor = "purple";
        JuiceGroup = 2;
    }

}
