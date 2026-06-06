using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(522, "Heracronite")]
public class Heracronite : MegaStone
{
    public Heracronite() : base("Heracross", 214)
    {
        _textureRectangle = new Rectangle(120, 24, 24, 24);
    }

}
