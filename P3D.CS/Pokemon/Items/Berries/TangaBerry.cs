using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2045, "Tanga")]
public class TangaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public TangaBerry() : base(64800, "If held by a Pokémon, this berry will lessen the damage taken from one supereffective Bug-type attack.", "4.3cm", "Very Soft", 1, 5)
    {

        Spicy = 20;
        Dry = 0;
        Sweet = 0;
        Bitter = 0;
        Sour = 10;

        type = (int)Element.Types.Bug;
        Power = 80;
        JuiceColor = "green";
        JuiceGroup = 2;
    }

}
