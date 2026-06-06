using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(111, "Big Pearl")]
public class BigPearl : Item
{
    public override String Description { get; protected set; } = "A rather large pearl that has a very nice silvery sheen. It can be sold to shops for a high price.";
    public override int PokeDollarPrice { get; protected set; } = 7500;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public BigPearl()
    {
        _textureRectangle = new Rectangle(288, 96, 24, 24);
    }

}
