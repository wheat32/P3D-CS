using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(553, "Swampertite")]
public class Swampertite : MegaStone
{
    public Swampertite() : base("Swampert", 260)
    {
        _textureRectangle = new Rectangle(192, 96, 24, 24);
    }

}
