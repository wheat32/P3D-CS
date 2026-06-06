using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(542, "Latiasite")]
public class Latiasite : MegaStone
{
    public Latiasite() : base("Latias", 380)
    {
        _textureRectangle = new Rectangle(168, 72, 24, 24);
    }

}
