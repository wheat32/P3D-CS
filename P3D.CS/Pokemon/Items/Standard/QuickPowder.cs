using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(155, "Quick Powder")]
public class QuickPowder : Item
{
    public override String Description { get; protected set; } = "An item to be held by Ditto. Extremely fine yet hard, this odd powder boosts the Speed stat.";
    public override int PokeDollarPrice { get; protected set; } = 10;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public QuickPowder()
    {
        _textureRectangle = new Rectangle(96, 216, 24, 24);
    }

}
