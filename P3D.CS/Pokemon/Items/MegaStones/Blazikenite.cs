using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.MegaStones;

[Item(515, "Blazikenite")]
public class Blazikenite : MegaStone
{
    public Blazikenite() : base("Blaziken", 257)
    {
        _textureRectangle = new Rectangle(192, 0, 24, 24);
    }

}
