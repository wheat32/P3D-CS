using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(526, "Manectite")]
public class Manectite : MegaStone
{
    public Manectite() : base("Manectric", 310)
    {
        _textureRectangle = new Rectangle(216, 24, 24, 24);
    }

}
