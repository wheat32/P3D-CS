using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(512, "Ampharosite")]
public class Ampharosite : MegaStone
{
    public Ampharosite() : base("Ampharos", 181)
    {
        _textureRectangle = new Rectangle(120, 0, 24, 24);
    }

}
