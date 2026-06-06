using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(513, "Banettite")]
public class Banettite : MegaStone
{
    public Banettite() : base("Banette", 354)
    {
        _textureRectangle = new Rectangle(144, 0, 24, 24);
    }

}
