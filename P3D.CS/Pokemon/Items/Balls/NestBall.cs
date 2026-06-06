using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(188, "Nest Ball")]
public class NestBall : BallItem
{
    public override String Description { get; protected set; } = "A somewhat different Pokéball that becomes more effective the lower the level of the wild Pokémon.";
    public NestBall()
    {
        _textureRectangle = new Rectangle(24, 240, 24, 24);
    }

}
