using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Balls;

[Item(186, "Heal Ball")]
public class HealBall : BallItem
{
    public override int PokeDollarPrice { get; protected set; } = 300;
    public override String Description { get; protected set; } = "A remedial Pokéball that restores the HP of a Pokémon caught with it && eliminiates any status conditions. ";
    public HealBall()
    {
        _textureRectangle = new Rectangle(456, 216, 24, 24);
    }

}
