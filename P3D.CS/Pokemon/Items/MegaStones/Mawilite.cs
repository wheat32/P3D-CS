using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(527, "Mawilite")]
public class Mawilite : MegaStone
{
    public Mawilite() : base("Mawile", 303)
    {
        _textureRectangle = new Rectangle(0, 48, 24, 24);
    }

}
