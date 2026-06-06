using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(129, "Quick Ball")]
public class QuickBall : BallItem
{
    public override String Description { get; protected set; } = "A somewhat different Pokéball that has a more successful catch rate if used at the start of a wild encounter.";
    public QuickBall()
    {
        _textureRectangle = new Rectangle(120, 168, 24, 24);
    }

}
