using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(508, "Absolite")]
public class Absolite : MegaStone
{
    public Absolite() : base("Absol", 359)
    {
        _textureRectangle = new Rectangle(24, 0, 24, 24);
    }

}
