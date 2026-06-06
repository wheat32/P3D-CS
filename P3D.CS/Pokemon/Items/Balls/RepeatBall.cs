using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(168, "Repeat Ball")]
public class RepeatBall : BallItem
{
    public override String Description { get; protected set; } = "A somewhat different Pokéball that works especially well on Pokémon species that have been caught before.";
    public RepeatBall()
    {
        _textureRectangle = new Rectangle(384, 216, 24, 24);
    }

}
