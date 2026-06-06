using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(174, "Luxury Ball")]
public class LuxuryBall : BallItem
{
    public override String Description { get; protected set; } = "A particularly comfortable Pokéball that makes a wild Pokémon quickly grow friendlier after being caught.";
    public LuxuryBall()
    {
        _textureRectangle = new Rectangle(432, 216, 24, 24);
    }

}
