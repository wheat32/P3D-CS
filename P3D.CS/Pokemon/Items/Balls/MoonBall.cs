using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(165, "Moon Ball")]
public class MoonBall : BallItem
{
    public override String Description { get; protected set; } = "A Pokéball for catching Pokémon that evolve using the Moon Stone.";
    public override int PokeDollarPrice { get; protected set; } = 150;
    public MoonBall()
    {
        _textureRectangle = new Rectangle(216, 144, 24, 24);
    }

}
