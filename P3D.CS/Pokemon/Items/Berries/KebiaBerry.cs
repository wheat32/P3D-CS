using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2041, "Kebia")]
public class KebiaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public KebiaBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Poison-type attack.", "8.9cm", "Hard", 1, 5)
    {

        Spicy = 0;
        Dry = 15;
        Sweet = 0;
        Bitter = 0;
        Sour = 10;

        type = (int)Element.Types.Poison;
        Power = 80;
        JuiceColor = "green";
        JuiceGroup = 2;
    }

}
