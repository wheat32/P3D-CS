using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2064, "Roseli")]
public class RoseliBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public RoseliBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Fairy-type attack.", "3.2cm", "Soft", 1, 5)
    {

        Spicy = 0;
        Dry = 0;
        Sweet = 25;
        Bitter = 0;
        Sour = 10;

        type = (int)Element.Types.Fairy;
        Power = 80;
        JuiceColor = "pink";
        JuiceGroup = 3;
    }

}
