using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(60, "Silver Leaf")]
public class SilverLeaf : Item
{
    public override String Description { get; protected set; } = "A strange, silver-colored leaf.";
    public override int PokeDollarPrice { get; protected set; } = 1000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName { get; } = "Silver Leaves";
    public SilverLeaf()
    {
        _textureRectangle = new Rectangle(288, 48, 24, 24);
    }

}
