using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(35, "Metal Powder")]
public class MetalPowder : Item
{
    public override String Description { get; protected set; } = "An item to be held by Ditto. Extremely fine yet hard, this odd powder boosts the Defense stat.";
    public override int PokeDollarPrice { get; protected set; } = 10;
    public override int FlingDamage { get; } = 10;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public MetalPowder()
    {
        _textureRectangle = new Rectangle(264, 24, 24, 24);
    }

}
