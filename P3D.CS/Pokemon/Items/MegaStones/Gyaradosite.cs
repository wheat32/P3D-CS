using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(521, "Gyaradosite")]
public class Gyaradosite : MegaStone
{
    public Gyaradosite() : base("Gyarados", 130)
    {
        _textureRectangle = new Rectangle(96, 24, 24, 24);
    }

}
