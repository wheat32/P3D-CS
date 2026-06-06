using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(79, "Dive Ball")]
public class DiveBall : BallItem
{
    public override String Description { get; protected set; } = "A somewhat different Pokéball that works especially well when catching Pokémon that live underwater.";
    public DiveBall()
    {
        _textureRectangle = new Rectangle(288, 144, 24, 24);
    }

}
