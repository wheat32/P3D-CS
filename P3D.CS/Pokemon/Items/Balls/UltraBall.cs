using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(2, "Ultra Ball")]
public class UltraBall : BallItem
{
    public override String Description { get; protected set; } = "An ultra-high performance Pokéball that provides a higher success rate for catching Pokémon than a Great Ball.";
    public override int PokeDollarPrice { get; protected set; } = 1200;
    public override float CatchMultiplier { get; } = 2.0F;
    public UltraBall()
    {
        _textureRectangle = new Rectangle(24, 0, 24, 24);
    }

}
