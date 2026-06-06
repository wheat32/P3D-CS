using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2050, "Babiri")]
public class BabiriBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public BabiriBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Steel-type attack.", "26.5cm", "Super Hard", 1, 5)
    {

        Spicy = 25;
        Dry = 10;
        Sweet = 0;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Steel;
        Power = 80;
        JuiceColor = "green";
        JuiceGroup = 2;
    }

}
