using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(547, "Sablenite")]
public class Sablenite : MegaStone
{
    public Sablenite() : base("Sableye", 302)
    {
        _textureRectangle = new Rectangle(48, 96, 24, 24);
    }

}
