using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(523, "Houndoominite")]
public class Houndoominite : MegaStone
{
    public Houndoominite() : base("Houndoom", 229)
    {
        _textureRectangle = new Rectangle(144, 24, 24, 24);
    }

}
