using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(548, "Salamencite")]
public class Salamencite : MegaStone
{
    public Salamencite() : base("Salamence", 373)
    {
        _textureRectangle = new Rectangle(72, 96, 24, 24);
    }

}
