using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(153, "Balm Mushroom")]
public class BalmMushroom : Item
{
    public override String Description { get; protected set; } = "A rare mushroom which gives off a nice fragrance. A maniac will buy it for a high price.";
    public override int PokeDollarPrice { get; protected set; } = 50000;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public BalmMushroom()
    {
        _textureRectangle = new Rectangle(48, 216, 24, 24);
    }

}
