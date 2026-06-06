using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(531, "Pinsirite")]
public class Pinsirite : MegaStone
{
    public Pinsirite() : base("Pinsir", 127)
    {
        _textureRectangle = new Rectangle(96, 48, 24, 24);
    }

}
