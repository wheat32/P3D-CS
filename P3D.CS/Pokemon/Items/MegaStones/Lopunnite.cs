using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(544, "Lopunnite")]
public class Lopunnite : MegaStone
{
    public Lopunnite() : base("Lopunny", 428)
    {
        _textureRectangle = new Rectangle(216, 72, 24, 24);
    }

}
