using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(525, "Lucarionite")]
public class Lucarionite : MegaStone
{
    public Lucarionite() : base("Lucario", 448)
    {
        _textureRectangle = new Rectangle(192, 24, 24, 24);
    }

}
