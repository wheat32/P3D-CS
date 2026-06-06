using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(45, "Cherish Ball")]
public class CherishBall : BallItem
{
    public override String Description { get; protected set; } = "A quite rare Pokéball that has been specially crafted to commemorate an occasion of some sort.";
    public CherishBall()
    {
        _textureRectangle = new Rectangle(216, 192, 24, 24);
    }

}
