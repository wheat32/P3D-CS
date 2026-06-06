using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(532, "Scizorite")]
public class Scizorite : MegaStone
{
    public Scizorite() : base("Scizor", 212)
    {
        _textureRectangle = new Rectangle(120, 48, 24, 24);
    }

}
