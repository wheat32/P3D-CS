using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(539, "Diancite")]
public class Diancite : MegaStone
{
    public Diancite() : base("Diancie", 719)
    {
        _textureRectangle = new Rectangle(96, 72, 24, 24);
    }

}
