using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(4, "Great Ball")]
public class GreatBall : BallItem
{
    public override String Description { get; protected set; } = "A good, high-performance Pokéball that provides a higher Pokémon catch rate than a standard Pokéball can.";
    public override int PokeDollarPrice { get; protected set; } = 600;
    public override float CatchMultiplier { get; } = 1.5F;
    public GreatBall()
    {
        _textureRectangle = new Rectangle(72, 0, 24, 24);
    }

}
