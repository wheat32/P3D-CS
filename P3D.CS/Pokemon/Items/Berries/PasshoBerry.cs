using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2036, "Passho")]
public class PasshoBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public PasshoBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Water-type attack.", "3.3cm", "Soft", 1, 5)
    {

        Spicy = 0;
        Dry = 15;
        Sweet = 0;
        Bitter = 10;
        Sour = 0;

        type = (int)Element.Types.Water;
        Power = 80;
        JuiceColor = "blue";
        JuiceGroup = 2;
    }

}
