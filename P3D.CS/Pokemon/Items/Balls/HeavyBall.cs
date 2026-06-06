using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(157, "Heavy Ball")]
public class HeavyBall : BallItem
{
    public override String Description { get; protected set; } = "A Pokéball for catching very heavy Pokémon.";
    public override int PokeDollarPrice { get; protected set; } = 150;
    public HeavyBall()
    {
        _textureRectangle = new Rectangle(48, 144, 24, 24);
    }

}
