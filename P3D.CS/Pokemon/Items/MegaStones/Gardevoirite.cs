using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(519, "Gardevoirite")]
public class Gardevoirite : MegaStone
{
    public Gardevoirite() : base("Gardevoir", 282)
    {
        _textureRectangle = new Rectangle(48, 24, 24, 24);
    }

}
