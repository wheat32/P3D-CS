using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2038, "Rindo")]
public class RindoBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public RindoBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Grass-type attack.", "15.5cm", "Soft", 1, 5)
    {

        Spicy = 10;
        Dry = 0;
        Sweet = 0;
        Bitter = 15;
        Sour = 0;

        type = (int)Element.Types.Grass;
        Power = 80;
        JuiceColor = "green";
        JuiceGroup = 2;
    }

}
