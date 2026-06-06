using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(115, "GS Ball")]
public class GSBall : KeyItem
{
    public override String Description { get; protected set; } = "A mysterious Pokéball. Its purpose == unknown.";
    public GSBall()
    {
        _textureRectangle = new Rectangle(384, 96, 24, 24);
    }

}
