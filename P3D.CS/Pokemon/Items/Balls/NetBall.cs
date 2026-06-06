using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(80, "Net Ball")]
public class NetBall : BallItem
{
    public override String Description { get; protected set; } = "A somewhat different Pokéball that == more effective when attempting to catch Water- || Bug-type Pokémon.";
    public NetBall()
    {
        _textureRectangle = new Rectangle(48, 168, 24, 24);
    }

}
