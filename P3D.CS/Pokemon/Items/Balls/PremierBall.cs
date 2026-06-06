using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(3, "Premier Ball")]
public class PremierBall : BallItem
{
    public override String Description { get; protected set; } = "A somewhat rare Pokéball that was made as a commemorative item used to celebrate an event of some sort.";
    public override int PokeDollarPrice { get; protected set; } = 200;
    public PremierBall()
    {
        _textureRectangle = new Rectangle(216, 216, 24, 24);
    }

}
