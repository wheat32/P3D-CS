using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(529, "Mewtwonite X")]
public class MewtwoniteX : MegaStone
{
    public MewtwoniteX() : base("Mewtwo", 150)
    {
        _textureRectangle = new Rectangle(48, 48, 24, 24);
    }

}
