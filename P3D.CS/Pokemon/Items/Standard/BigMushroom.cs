using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plants;

[Item(87, "Big Mushroom")]
public class BigMushroom : Item
{
    public override ItemTypes ItemType { get; } = ItemTypes.Standard;
    public override String Description { get; protected set; } = "A very large && rare mushroom. It's popular with a certain class of collectors && sought out by them.";
    public override int PokeDollarPrice { get; protected set; } = 5000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public BigMushroom()
    {
        _textureRectangle = new Rectangle(288, 72, 24, 24);
    }

}
