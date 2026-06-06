using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2035, "Occa")]
public class OccaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public OccaBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Fire-type attack.", "8.9cm", "Super Hard", 1, 5)
    {

        Spicy = 15;
        Dry = 0;
        Sweet = 10;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Fire;
        Power = 80;
        JuiceColor = "red";
        JuiceGroup = 2;
    }

}
