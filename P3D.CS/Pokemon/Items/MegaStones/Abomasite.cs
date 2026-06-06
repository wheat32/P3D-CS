using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(507, "Abomasite")]
public class Abomasite : MegaStone
{
    public Abomasite() : base("Abomasnow", 460)
    {
        _textureRectangle = new Rectangle(0, 0, 24, 24);
    }

}
