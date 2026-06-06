using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(538, "Cameruptite")]
public class Cameruptite : MegaStone
{
    public Cameruptite() : base("Camerupt", 323)
    {
        _textureRectangle = new Rectangle(72, 72, 24, 24);
    }

}
