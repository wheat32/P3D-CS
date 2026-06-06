using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(530, "Mewtwonite Y")]
public class MewtwoniteY : MegaStone
{
    public MewtwoniteY() : base("Mewtwo", 150)
    {
        _textureRectangle = new Rectangle(72, 48, 24, 24);
    }

}
