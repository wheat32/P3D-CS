using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2047, "Kasib")]
public class KasibBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public KasibBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Ghost-type attack.", "14.4cm", "Hard", 1, 5)
    {

        Spicy = 0;
        Dry = 10;
        Sweet = 20;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Ghost;
        Power = 80;
        JuiceColor = "purple";
        JuiceGroup = 2;
    }

}
