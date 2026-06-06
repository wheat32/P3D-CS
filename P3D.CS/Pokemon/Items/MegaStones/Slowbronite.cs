using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(551, "Slowbronite")]
public class Slowbronite : MegaStone
{
    public Slowbronite() : base("Slowbro", 80)
    {
        _textureRectangle = new Rectangle(144, 96, 24, 24);
    }

}
