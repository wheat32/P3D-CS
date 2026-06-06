using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(159, "Level Ball")]
public class LevelBall : BallItem
{
    public override String Description { get; protected set; } = "A Pokéball for catching Pokémon that are a lower level than your own. ";
    public override int PokeDollarPrice { get; protected set; } = 150;
    public LevelBall()
    {
        _textureRectangle = new Rectangle(96, 144, 24, 24);
    }

}
