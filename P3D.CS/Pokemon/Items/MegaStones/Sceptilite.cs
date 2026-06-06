using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(549, "Sceptilite")]
public class Sceptilite : MegaStone
{
    public Sceptilite() : base("Sceptile", 254)
    {
        _textureRectangle = new Rectangle(96, 96, 24, 24);
    }

}
