using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(546, "Pidgeotite")]
public class Pidgeotite : MegaStone
{
    public Pidgeotite() : base("Pidgeot", 18)
    {
        _textureRectangle = new Rectangle(24, 96, 24, 24);
    }

}
