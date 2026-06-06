using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(132, "Star Piece")]
public class StarPiece : Item
{
    public override String Description { get; protected set; } = "A small shard of a beautiful gem that demonstrates a distinctly red sparkle. It can be sold at a high price to shops.";
    public override int PokeDollarPrice { get; protected set; } = 9800;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public StarPiece()
    {
        _textureRectangle = new Rectangle(264, 120, 24, 24);
    }

}
