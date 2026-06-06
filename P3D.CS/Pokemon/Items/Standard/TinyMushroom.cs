using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Plants;

[Item(86, "Tiny Mushroom")]
public class TinyMushroom : Item
{
    public override ItemTypes ItemType { get; } = ItemTypes.Standard;
    public override String Description { get; protected set; } = "A very small && rare mushroom. It's popular with a certain class of collectors && sought out by them.";
    public override int PokeDollarPrice { get; protected set; } = 500;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public TinyMushroom()
    {
        _textureRectangle = new Rectangle(264, 72, 24, 24);
    }

}
