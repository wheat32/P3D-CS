using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(161, "Fast Ball")]
public class FastBall : BallItem
{
    public override String Description { get; protected set; } = "A Pokéball that makes it easier to catch fast Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 150;
    public FastBall()
    {
        _textureRectangle = new Rectangle(144, 144, 24, 24);
    }

}
