using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(536, "Audinite")]
public class Audinite : MegaStone
{
    public Audinite() : base("Audino", 531)
    {
        _textureRectangle = new Rectangle(24, 72, 24, 24);
    }

}
