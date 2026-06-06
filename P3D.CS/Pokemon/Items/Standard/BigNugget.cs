using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(189, "Big Nugget")]
public class BigNugget : Item
{
    public override String Description { get; protected set; } = "A big nugget of pure gold that gives off a lustrous gleam. It can be sold at a high price to shops.";
    public override int PokeDollarPrice { get; protected set; } = 20000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public BigNugget()
    {
        _textureRectangle = new Rectangle(48, 240, 24, 24);
    }

}
