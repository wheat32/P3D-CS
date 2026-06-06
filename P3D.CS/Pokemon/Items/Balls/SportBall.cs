using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(177, "Sport Ball")]
public class SportBall : BallItem
{
    public override bool CanBeHeld { get; } = false;
    public override String Description { get; protected set; } = "A special Pokéball for the Bug-Catching Contest.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public override float CatchMultiplier { get; } = 1.5F;
    public SportBall()
    {
        _textureRectangle = new Rectangle(384, 144, 24, 24);
    }

}
