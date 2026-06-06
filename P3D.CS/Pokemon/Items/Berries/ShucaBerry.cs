using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Berries;

[Item(2042, "Shuca")]
public class ShucaBerry : Berry
{
    public override bool CanBeUsed { get; } = false;
    public override bool CanBeUsedInBattle { get; } = false;
    public ShucaBerry() : base(64800, "If held by a Pokémon, this Berry will lessen the damage taken from one supereffective Ground-type attack.", "4.2cm", "Soft", 1, 5)
    {

        Spicy = 10;
        Dry = 0;
        Sweet = 15;
        Bitter = 0;
        Sour = 0;

        type = (int)Element.Types.Ground;
        Power = 80;
        JuiceColor = "yellow";
        JuiceGroup = 2;
    }

}
