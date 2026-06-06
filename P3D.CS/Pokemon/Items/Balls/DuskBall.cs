using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(158, "Dusk Ball")]
public class DuskBall : BallItem
{
    public override String Description { get; protected set; } = "A somewhat different Pokéball that makes it easier to catch wild Pokémon at night || in dark places like caves.";
    public DuskBall()
    {
        _textureRectangle = new Rectangle(360, 216, 24, 24);
    }

}
