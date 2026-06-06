using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(75, "Gold Leaf")]
public class GoldLeaf : Item
{
    public override String Description { get; protected set; } = "A strange, gold-colored leaf.";
    public override int PokeDollarPrice { get; protected set; } = 2000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public override String PluralName { get; } = "Gold Leaves";
    public GoldLeaf()
    {
        _textureRectangle = new Rectangle(120, 72, 24, 24);
    }

}
