using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(543, "Latiosite")]
public class Latiosite : MegaStone
{
    public Latiosite() : base("Latios", 381)
    {
        _textureRectangle = new Rectangle(192, 72, 24, 24);
    }

}
