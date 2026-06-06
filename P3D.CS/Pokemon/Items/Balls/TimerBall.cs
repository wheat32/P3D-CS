using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(150, "Timer Ball")]
public class TimerBall : BallItem
{
    public override String Description { get; protected set; } = "A somewhat different Pokéball that becomes progressively more effective the more turns that are taken in battle.";
    public override int PokeDollarPrice { get; protected set; } = 150;
    public TimerBall()
    {
        _textureRectangle = new Rectangle(336, 216, 24, 24);
    }

}
