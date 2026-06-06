using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(180, "Brick Piece")]
public class BrickPiece : Item
{
    public override String Description { get; protected set; } = "A rare chunk of brick.";
    public override int PokeDollarPrice { get; protected set; } = 2500;
    public override bool CanBeUsedInBattle { get; } = false;
    public override bool CanBeUsed { get; } = false;
    public BrickPiece()
    {
        _textureRectangle = new Rectangle(72, 240, 24, 24);
    }

}
